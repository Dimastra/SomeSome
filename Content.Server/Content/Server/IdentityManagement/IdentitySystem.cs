using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Content.Server.Access.Systems;
using Content.Server.Administration.Logs;
using Content.Shared.Access.Components;
using Content.Shared.Administration.Logs;
using Content.Shared.Database;
using Content.Shared.Hands;
using Content.Shared.Humanoid;
using Content.Shared.IdentityManagement;
using Content.Shared.IdentityManagement.Components;
using Content.Shared.Inventory;
using Content.Shared.Inventory.Events;
using Robust.Shared.Enums;
using Robust.Shared.GameObjects;
using Robust.Shared.GameObjects.Components.Localization;
using Robust.Shared.IoC;

namespace Content.Server.IdentityManagement
{
	// Token: 0x02000457 RID: 1111
	[NullableContext(1)]
	[Nullable(0)]
	public class IdentitySystem : SharedIdentitySystem
	{
		// Token: 0x06001666 RID: 5734 RVA: 0x000763A8 File Offset: 0x000745A8
		public override void Initialize()
		{
			base.Initialize();
			base.SubscribeLocalEvent<IdentityComponent, DidEquipEvent>(delegate(EntityUid uid, IdentityComponent _, DidEquipEvent _)
			{
				this.QueueIdentityUpdate(uid);
			}, null, null);
			base.SubscribeLocalEvent<IdentityComponent, DidEquipHandEvent>(delegate(EntityUid uid, IdentityComponent _, DidEquipHandEvent _)
			{
				this.QueueIdentityUpdate(uid);
			}, null, null);
			base.SubscribeLocalEvent<IdentityComponent, DidUnequipEvent>(delegate(EntityUid uid, IdentityComponent _, DidUnequipEvent _)
			{
				this.QueueIdentityUpdate(uid);
			}, null, null);
			base.SubscribeLocalEvent<IdentityComponent, DidUnequipHandEvent>(delegate(EntityUid uid, IdentityComponent _, DidUnequipHandEvent _)
			{
				this.QueueIdentityUpdate(uid);
			}, null, null);
		}

		// Token: 0x06001667 RID: 5735 RVA: 0x0007640C File Offset: 0x0007460C
		public override void Update(float frameTime)
		{
			base.Update(frameTime);
			foreach (EntityUid ent in this._queuedIdentityUpdates)
			{
				IdentityComponent identity;
				if (base.TryComp<IdentityComponent>(ent, ref identity))
				{
					this.UpdateIdentityInfo(ent, identity);
				}
			}
			this._queuedIdentityUpdates.Clear();
		}

		// Token: 0x06001668 RID: 5736 RVA: 0x00076480 File Offset: 0x00074680
		protected override void OnComponentInit(EntityUid uid, IdentityComponent component, ComponentInit args)
		{
			base.OnComponentInit(uid, component, args);
			EntityUid ident = base.Spawn(null, base.Transform(uid).Coordinates);
			this.QueueIdentityUpdate(uid);
			component.IdentityEntitySlot.Insert(ident, null, null, null, null, null);
		}

		// Token: 0x06001669 RID: 5737 RVA: 0x000764C3 File Offset: 0x000746C3
		public void QueueIdentityUpdate(EntityUid uid)
		{
			this._queuedIdentityUpdates.Add(uid);
		}

		// Token: 0x0600166A RID: 5738 RVA: 0x000764D4 File Offset: 0x000746D4
		private void UpdateIdentityInfo(EntityUid uid, IdentityComponent identity)
		{
			EntityUid? containedEntity = identity.IdentityEntitySlot.ContainedEntity;
			if (containedEntity == null)
			{
				return;
			}
			EntityUid ident = containedEntity.GetValueOrDefault();
			IdentityRepresentation representation = this.GetIdentityRepresentation(uid, null, null);
			string name = this.GetIdentityName(uid, representation);
			GrammarComponent grammar;
			if (base.TryComp<GrammarComponent>(uid, ref grammar))
			{
				GrammarComponent identityGrammar = base.EnsureComp<GrammarComponent>(ident);
				identityGrammar.Attributes.Clear();
				foreach (KeyValuePair<string, string> keyValuePair in grammar.Attributes)
				{
					string text;
					string text2;
					keyValuePair.Deconstruct(out text, out text2);
					string i = text;
					string v = text2;
					identityGrammar.Attributes.Add(i, v);
				}
				if (name != representation.TrueName && representation.PresumedName == null)
				{
					identityGrammar.ProperNoun = new bool?(false);
				}
			}
			if (name == base.Name(ident, null))
			{
				return;
			}
			base.MetaData(ident).EntityName = name;
			ISharedAdminLogManager adminLog = this._adminLog;
			LogType type = LogType.Identity;
			LogImpact impact = LogImpact.Medium;
			LogStringHandler logStringHandler = new LogStringHandler(21, 2);
			logStringHandler.AppendFormatted<EntityStringRepresentation>(base.ToPrettyString(uid), "ToPrettyString(uid)");
			logStringHandler.AppendLiteral(" changed identity to ");
			logStringHandler.AppendFormatted(name);
			adminLog.Add(type, impact, ref logStringHandler);
			base.RaiseLocalEvent<IdentityChangedEvent>(new IdentityChangedEvent(uid, ident));
		}

		// Token: 0x0600166B RID: 5739 RVA: 0x0007662C File Offset: 0x0007482C
		private string GetIdentityName(EntityUid target, IdentityRepresentation representation)
		{
			SeeIdentityAttemptEvent ev = new SeeIdentityAttemptEvent();
			base.RaiseLocalEvent<SeeIdentityAttemptEvent>(target, ev, false);
			return representation.ToStringKnown(!ev.Cancelled);
		}

		// Token: 0x0600166C RID: 5740 RVA: 0x00076658 File Offset: 0x00074858
		[NullableContext(2)]
		[return: Nullable(1)]
		private IdentityRepresentation GetIdentityRepresentation(EntityUid target, InventoryComponent inventory = null, HumanoidAppearanceComponent appearance = null)
		{
			int age = 18;
			Gender gender = 1;
			if (base.Resolve<HumanoidAppearanceComponent>(target, ref appearance, false))
			{
				gender = appearance.Gender;
				age = appearance.Age;
			}
			string trueName = base.Name(target, null);
			if (!base.Resolve<InventoryComponent>(target, ref inventory, false))
			{
				return new IdentityRepresentation(trueName, age, gender, string.Empty, null);
			}
			string presumedJob = null;
			string presumedName = null;
			IdCardComponent id;
			if (this._idCard.TryFindIdCard(target, out id))
			{
				presumedName = (string.IsNullOrWhiteSpace(id.FullName) ? null : id.FullName);
				string jobTitle = id.JobTitle;
				presumedJob = ((jobTitle != null) ? jobTitle.ToLowerInvariant() : null);
			}
			return new IdentityRepresentation(trueName, age, gender, presumedName, presumedJob);
		}

		// Token: 0x04000E07 RID: 3591
		[Dependency]
		private readonly IdCardSystem _idCard;

		// Token: 0x04000E08 RID: 3592
		[Dependency]
		private readonly IAdminLogManager _adminLog;

		// Token: 0x04000E09 RID: 3593
		private HashSet<EntityUid> _queuedIdentityUpdates = new HashSet<EntityUid>();
	}
}
