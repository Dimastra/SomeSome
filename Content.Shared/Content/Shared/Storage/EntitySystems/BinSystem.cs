using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Content.Shared.Administration.Logs;
using Content.Shared.Database;
using Content.Shared.Hands.EntitySystems;
using Content.Shared.Interaction;
using Content.Shared.Storage.Components;
using Robust.Shared.Containers;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.IoC;
using Robust.Shared.Log;
using Robust.Shared.Network;
using Robust.Shared.Timing;

namespace Content.Shared.Storage.EntitySystems
{
	// Token: 0x0200012F RID: 303
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class BinSystem : EntitySystem
	{
		// Token: 0x06000371 RID: 881 RVA: 0x0000E9E0 File Offset: 0x0000CBE0
		public override void Initialize()
		{
			base.SubscribeLocalEvent<BinComponent, ComponentGetState>(new ComponentEventRefHandler<BinComponent, ComponentGetState>(this.OnGetState), null, null);
			base.SubscribeLocalEvent<BinComponent, ComponentHandleState>(new ComponentEventRefHandler<BinComponent, ComponentHandleState>(this.OnHandleState), null, null);
			base.SubscribeLocalEvent<BinComponent, ComponentStartup>(new ComponentEventHandler<BinComponent, ComponentStartup>(this.OnStartup), null, null);
			base.SubscribeLocalEvent<BinComponent, MapInitEvent>(new ComponentEventHandler<BinComponent, MapInitEvent>(this.OnMapInit), null, null);
			base.SubscribeLocalEvent<BinComponent, EntRemovedFromContainerMessage>(new ComponentEventHandler<BinComponent, EntRemovedFromContainerMessage>(this.OnEntRemoved), null, null);
			base.SubscribeLocalEvent<BinComponent, InteractHandEvent>(new ComponentEventHandler<BinComponent, InteractHandEvent>(this.OnInteractHand), null, null);
			base.SubscribeLocalEvent<BinComponent, AfterInteractUsingEvent>(new ComponentEventHandler<BinComponent, AfterInteractUsingEvent>(this.OnAfterInteractUsing), null, null);
		}

		// Token: 0x06000372 RID: 882 RVA: 0x0000EA79 File Offset: 0x0000CC79
		private void OnGetState(EntityUid uid, BinComponent component, ref ComponentGetState args)
		{
			args.State = new BinComponentState(component.Items, component.Whitelist, component.MaxItems);
		}

		// Token: 0x06000373 RID: 883 RVA: 0x0000EA98 File Offset: 0x0000CC98
		private void OnHandleState(EntityUid uid, BinComponent component, ref ComponentHandleState args)
		{
			BinComponentState state = args.Current as BinComponentState;
			if (state == null)
			{
				return;
			}
			component.Items = new List<EntityUid>(state.Items);
			component.Whitelist = state.Whitelist;
			component.MaxItems = state.MaxItems;
		}

		// Token: 0x06000374 RID: 884 RVA: 0x0000EADE File Offset: 0x0000CCDE
		private void OnStartup(EntityUid uid, BinComponent component, ComponentStartup args)
		{
			component.ItemContainer = this._container.EnsureContainer<Container>(uid, "bin-container", null);
		}

		// Token: 0x06000375 RID: 885 RVA: 0x0000EAF8 File Offset: 0x0000CCF8
		private void OnMapInit(EntityUid uid, BinComponent component, MapInitEvent args)
		{
			if (this._net.IsClient)
			{
				return;
			}
			TransformComponent xform = base.Transform(uid);
			foreach (string id in component.InitialContents)
			{
				EntityUid ent = base.Spawn(id, xform.Coordinates);
				if (!this.TryInsertIntoBin(uid, ent, component))
				{
					DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(46, 2);
					defaultInterpolatedStringHandler.AppendLiteral("Entity ");
					defaultInterpolatedStringHandler.AppendFormatted<EntityStringRepresentation>(base.ToPrettyString(ent));
					defaultInterpolatedStringHandler.AppendLiteral(" was unable to be initialized into bin ");
					defaultInterpolatedStringHandler.AppendFormatted<EntityStringRepresentation>(base.ToPrettyString(uid));
					Logger.Error(defaultInterpolatedStringHandler.ToStringAndClear());
					break;
				}
			}
		}

		// Token: 0x06000376 RID: 886 RVA: 0x0000EBC0 File Offset: 0x0000CDC0
		private void OnEntRemoved(EntityUid uid, BinComponent component, EntRemovedFromContainerMessage args)
		{
			component.Items.Remove(args.Entity);
		}

		// Token: 0x06000377 RID: 887 RVA: 0x0000EBD4 File Offset: 0x0000CDD4
		private void OnInteractHand(EntityUid uid, BinComponent component, InteractHandEvent args)
		{
			if (args.Handled || !this._timing.IsFirstTimePredicted)
			{
				return;
			}
			EntityUid? toGrab = new EntityUid?(component.Items.LastOrDefault<EntityUid>());
			if (!this.TryRemoveFromBin(uid, toGrab, component))
			{
				return;
			}
			this._hands.TryPickupAnyHand(args.User, toGrab.Value, true, false, null, null);
			ISharedAdminLogManager admin = this._admin;
			LogType type = LogType.Pickup;
			LogImpact impact = LogImpact.Low;
			LogStringHandler logStringHandler = new LogStringHandler(20, 3);
			logStringHandler.AppendFormatted<EntityStringRepresentation>(base.ToPrettyString(uid), "player", "ToPrettyString(uid)");
			logStringHandler.AppendLiteral(" removed ");
			logStringHandler.AppendFormatted<EntityStringRepresentation>(base.ToPrettyString(toGrab.Value), "ToPrettyString(toGrab.Value)");
			logStringHandler.AppendLiteral(" from bin ");
			logStringHandler.AppendFormatted<EntityStringRepresentation>(base.ToPrettyString(uid), "ToPrettyString(uid)");
			logStringHandler.AppendLiteral(".");
			admin.Add(type, impact, ref logStringHandler);
			args.Handled = true;
		}

		// Token: 0x06000378 RID: 888 RVA: 0x0000ECBC File Offset: 0x0000CEBC
		private void OnAfterInteractUsing(EntityUid uid, BinComponent component, AfterInteractUsingEvent args)
		{
			if (args.Handled || !args.CanReach)
			{
				return;
			}
			if (!this._timing.IsFirstTimePredicted)
			{
				return;
			}
			if (!this.TryInsertIntoBin(uid, args.Used, component))
			{
				return;
			}
			ISharedAdminLogManager admin = this._admin;
			LogType type = LogType.Pickup;
			LogImpact impact = LogImpact.Low;
			LogStringHandler logStringHandler = new LogStringHandler(21, 3);
			logStringHandler.AppendFormatted<EntityStringRepresentation>(base.ToPrettyString(uid), "player", "ToPrettyString(uid)");
			logStringHandler.AppendLiteral(" inserted ");
			logStringHandler.AppendFormatted<EntityStringRepresentation>(base.ToPrettyString(args.User), "ToPrettyString(args.User)");
			logStringHandler.AppendLiteral(" into bin ");
			logStringHandler.AppendFormatted<EntityStringRepresentation>(base.ToPrettyString(uid), "ToPrettyString(uid)");
			logStringHandler.AppendLiteral(".");
			admin.Add(type, impact, ref logStringHandler);
			args.Handled = true;
		}

		// Token: 0x06000379 RID: 889 RVA: 0x0000ED84 File Offset: 0x0000CF84
		[NullableContext(2)]
		public bool TryInsertIntoBin(EntityUid uid, EntityUid toInsert, BinComponent component = null)
		{
			if (!base.Resolve<BinComponent>(uid, ref component, true))
			{
				return false;
			}
			if (component.Items.Count >= component.MaxItems)
			{
				return false;
			}
			if (component.Whitelist != null && !component.Whitelist.IsValid(toInsert, null))
			{
				return false;
			}
			component.ItemContainer.Insert(toInsert, null, null, null, null, null);
			component.Items.Add(toInsert);
			base.Dirty(component, null);
			return true;
		}

		// Token: 0x0600037A RID: 890 RVA: 0x0000EDF4 File Offset: 0x0000CFF4
		[NullableContext(2)]
		public bool TryRemoveFromBin(EntityUid uid, EntityUid? toRemove, BinComponent component = null)
		{
			if (!base.Resolve<BinComponent>(uid, ref component, true))
			{
				return false;
			}
			if (!component.Items.Any<EntityUid>())
			{
				return false;
			}
			if (toRemove != null)
			{
				EntityUid? entityUid = toRemove;
				EntityUid entityUid2 = component.Items.LastOrDefault<EntityUid>();
				if (entityUid != null && (entityUid == null || !(entityUid.GetValueOrDefault() != entityUid2)))
				{
					if (!component.ItemContainer.Remove(toRemove.Value, null, null, null, true, false, null, null))
					{
						return false;
					}
					component.Items.Remove(toRemove.Value);
					base.Dirty(component, null);
					return true;
				}
			}
			return false;
		}

		// Token: 0x04000398 RID: 920
		[Dependency]
		private readonly IGameTiming _timing;

		// Token: 0x04000399 RID: 921
		[Dependency]
		private readonly INetManager _net;

		// Token: 0x0400039A RID: 922
		[Dependency]
		private readonly ISharedAdminLogManager _admin;

		// Token: 0x0400039B RID: 923
		[Dependency]
		private readonly SharedContainerSystem _container;

		// Token: 0x0400039C RID: 924
		[Dependency]
		private readonly SharedHandsSystem _hands;

		// Token: 0x0400039D RID: 925
		public const string BinContainerId = "bin-container";
	}
}
