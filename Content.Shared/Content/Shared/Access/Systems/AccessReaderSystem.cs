using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Runtime.CompilerServices;
using Content.Shared.Access.Components;
using Content.Shared.Emag.Components;
using Content.Shared.Emag.Systems;
using Content.Shared.Hands.EntitySystems;
using Content.Shared.Inventory;
using Content.Shared.MachineLinking.Events;
using Content.Shared.PDA;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Log;
using Robust.Shared.Prototypes;

namespace Content.Shared.Access.Systems
{
	// Token: 0x02000775 RID: 1909
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class AccessReaderSystem : EntitySystem
	{
		// Token: 0x06001786 RID: 6022 RVA: 0x0004C550 File Offset: 0x0004A750
		public override void Initialize()
		{
			base.Initialize();
			base.SubscribeLocalEvent<AccessReaderComponent, ComponentInit>(new ComponentEventHandler<AccessReaderComponent, ComponentInit>(this.OnInit), null, null);
			base.SubscribeLocalEvent<AccessReaderComponent, GotEmaggedEvent>(new ComponentEventRefHandler<AccessReaderComponent, GotEmaggedEvent>(this.OnEmagged), null, null);
			base.SubscribeLocalEvent<AccessReaderComponent, LinkAttemptEvent>(new ComponentEventHandler<AccessReaderComponent, LinkAttemptEvent>(this.OnLinkAttempt), null, null);
		}

		// Token: 0x06001787 RID: 6023 RVA: 0x0004C59F File Offset: 0x0004A79F
		private void OnLinkAttempt(EntityUid uid, AccessReaderComponent component, LinkAttemptEvent args)
		{
			if (args.User == null)
			{
				return;
			}
			if (!base.HasComp<EmaggedComponent>(uid) && !this.IsAllowed(args.User.Value, component))
			{
				args.Cancel();
			}
		}

		// Token: 0x06001788 RID: 6024 RVA: 0x0004C5D4 File Offset: 0x0004A7D4
		private void OnInit(EntityUid uid, AccessReaderComponent reader, ComponentInit args)
		{
			foreach (string level in reader.AccessLists.SelectMany((HashSet<string> c) => c).Union(reader.DenyTags))
			{
				if (!this._prototypeManager.HasIndex<AccessLevelPrototype>(level))
				{
					Logger.ErrorS("access", "Invalid access level: " + level);
				}
			}
		}

		// Token: 0x06001789 RID: 6025 RVA: 0x0004C66C File Offset: 0x0004A86C
		private void OnEmagged(EntityUid uid, AccessReaderComponent reader, ref GotEmaggedEvent args)
		{
			args.Handled = true;
		}

		// Token: 0x0600178A RID: 6026 RVA: 0x0004C678 File Offset: 0x0004A878
		[NullableContext(2)]
		public bool IsAllowed(EntityUid source, EntityUid target, AccessReaderComponent reader = null)
		{
			if (!base.Resolve<AccessReaderComponent>(target, ref reader, false))
			{
				return true;
			}
			ICollection<string> tags = this.FindAccessTags(source);
			return this.IsAllowed(tags, reader);
		}

		// Token: 0x0600178B RID: 6027 RVA: 0x0004C6A4 File Offset: 0x0004A8A4
		public bool IsAllowed(EntityUid entity, AccessReaderComponent reader)
		{
			ICollection<string> tags = this.FindAccessTags(entity);
			return this.IsAllowed(tags, reader);
		}

		// Token: 0x0600178C RID: 6028 RVA: 0x0004C6C4 File Offset: 0x0004A8C4
		public bool IsAllowed(ICollection<string> accessTags, AccessReaderComponent reader)
		{
			return base.HasComp<EmaggedComponent>(reader.Owner) || (!reader.DenyTags.Overlaps(accessTags) && (reader.AccessLists.Count == 0 || reader.AccessLists.Any((HashSet<string> a) => a.IsSubsetOf(accessTags))));
		}

		// Token: 0x0600178D RID: 6029 RVA: 0x0004C72C File Offset: 0x0004A92C
		public ICollection<string> FindAccessTags(EntityUid uid)
		{
			HashSet<string> tags = null;
			bool owned = false;
			this.FindAccessTagsItem(uid, ref tags, ref owned);
			HashSet<EntityUid> items;
			this.FindAccessItemsInventory(uid, out items);
			GetAdditionalAccessEvent ev = new GetAdditionalAccessEvent
			{
				Entities = items
			};
			base.RaiseLocalEvent<GetAdditionalAccessEvent>(uid, ref ev, false);
			foreach (EntityUid ent in ev.Entities)
			{
				this.FindAccessTagsItem(ent, ref tags, ref owned);
			}
			ICollection<string> collection = tags;
			return collection ?? Array.Empty<string>();
		}

		// Token: 0x0600178E RID: 6030 RVA: 0x0004C7CC File Offset: 0x0004A9CC
		private void FindAccessTagsItem(EntityUid uid, [Nullable(new byte[]
		{
			2,
			1
		})] ref HashSet<string> tags, ref bool owned)
		{
			HashSet<string> targetTags;
			if (!this.FindAccessTagsItem(uid, out targetTags))
			{
				return;
			}
			if (tags != null)
			{
				if (!owned)
				{
					tags = new HashSet<string>(tags);
					owned = true;
				}
				tags.UnionWith(targetTags);
				return;
			}
			tags = targetTags;
			owned = false;
		}

		// Token: 0x0600178F RID: 6031 RVA: 0x0004C808 File Offset: 0x0004AA08
		public bool FindAccessItemsInventory(EntityUid uid, out HashSet<EntityUid> items)
		{
			items = new HashSet<EntityUid>();
			foreach (EntityUid item in this._handsSystem.EnumerateHeld(uid, null))
			{
				items.Add(item);
			}
			EntityUid? idUid;
			if (this._inventorySystem.TryGetSlotEntity(uid, "id", out idUid, null, null))
			{
				items.Add(idUid.Value);
			}
			return items.Any<EntityUid>();
		}

		// Token: 0x06001790 RID: 6032 RVA: 0x0004C894 File Offset: 0x0004AA94
		private bool FindAccessTagsItem(EntityUid uid, [Nullable(new byte[]
		{
			2,
			1
		})] [NotNullWhen(true)] out HashSet<string> tags)
		{
			AccessComponent access;
			if (this.EntityManager.TryGetComponent<AccessComponent>(uid, ref access))
			{
				tags = access.Tags;
				return true;
			}
			PDAComponent pda;
			if (this.EntityManager.TryGetComponent<PDAComponent>(uid, ref pda))
			{
				IdCardComponent containedID = pda.ContainedID;
				EntityUid? entityUid = (containedID != null) ? new EntityUid?(containedID.Owner) : null;
				if (entityUid != null)
				{
					EntityUid id = entityUid.GetValueOrDefault();
					if (id.Valid)
					{
						tags = this.EntityManager.GetComponent<AccessComponent>(id).Tags;
						return true;
					}
				}
			}
			tags = null;
			return false;
		}

		// Token: 0x04001750 RID: 5968
		[Dependency]
		private readonly IPrototypeManager _prototypeManager;

		// Token: 0x04001751 RID: 5969
		[Dependency]
		private readonly InventorySystem _inventorySystem;

		// Token: 0x04001752 RID: 5970
		[Dependency]
		private readonly SharedHandsSystem _handsSystem;
	}
}
