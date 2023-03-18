using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Content.Server.Actions;
using Content.Server.Inventory;
using Content.Server.Mind.Components;
using Content.Server.Polymorph.Components;
using Content.Server.Popups;
using Content.Shared.Actions.ActionTypes;
using Content.Shared.Damage;
using Content.Shared.Hands.EntitySystems;
using Content.Shared.IdentityManagement;
using Content.Shared.Inventory;
using Content.Shared.Mobs.Components;
using Content.Shared.Mobs.Systems;
using Content.Shared.Polymorph;
using Content.Shared.Popups;
using Robust.Server.Containers;
using Robust.Shared.Containers;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Localization;
using Robust.Shared.Log;
using Robust.Shared.Prototypes;

namespace Content.Server.Polymorph.Systems
{
	// Token: 0x020002C6 RID: 710
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class PolymorphedEntitySystem : EntitySystem
	{
		// Token: 0x06000E5C RID: 3676 RVA: 0x000489A8 File Offset: 0x00046BA8
		public override void Initialize()
		{
			base.Initialize();
			base.SubscribeLocalEvent<PolymorphedEntityComponent, ComponentStartup>(new ComponentEventHandler<PolymorphedEntityComponent, ComponentStartup>(this.OnInit), null, null);
			base.SubscribeLocalEvent<PolymorphedEntityComponent, RevertPolymorphActionEvent>(new ComponentEventHandler<PolymorphedEntityComponent, RevertPolymorphActionEvent>(this.OnRevertPolymorphActionEvent), null, null);
		}

		// Token: 0x06000E5D RID: 3677 RVA: 0x000489D8 File Offset: 0x00046BD8
		private void OnRevertPolymorphActionEvent(EntityUid uid, PolymorphedEntityComponent component, RevertPolymorphActionEvent args)
		{
			this.Revert(uid);
		}

		// Token: 0x06000E5E RID: 3678 RVA: 0x000489E4 File Offset: 0x00046BE4
		public void Revert(EntityUid uid)
		{
			if (base.Deleted(uid, null))
			{
				return;
			}
			PolymorphedEntityComponent component;
			if (!base.TryComp<PolymorphedEntityComponent>(uid, ref component))
			{
				return;
			}
			if (base.Deleted(component.Parent, null))
			{
				return;
			}
			PolymorphPrototype proto;
			if (!this._proto.TryIndex<PolymorphPrototype>(component.Prototype, ref proto))
			{
				DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(96, 3);
				defaultInterpolatedStringHandler.AppendFormatted("PolymorphedEntitySystem");
				defaultInterpolatedStringHandler.AppendLiteral(" encountered an improperly initialized polymorph component while reverting. Entity ");
				defaultInterpolatedStringHandler.AppendFormatted<EntityStringRepresentation>(base.ToPrettyString(uid));
				defaultInterpolatedStringHandler.AppendLiteral(". Prototype: ");
				defaultInterpolatedStringHandler.AppendFormatted(component.Prototype);
				Logger.Error(defaultInterpolatedStringHandler.ToStringAndClear());
				return;
			}
			TransformComponent uidXform = base.Transform(uid);
			TransformComponent transformComponent = base.Transform(component.Parent);
			transformComponent.AttachParent(uidXform.ParentUid);
			transformComponent.Coordinates = uidXform.Coordinates;
			transformComponent.LocalRotation = uidXform.LocalRotation;
			IContainer cont;
			if (this._container.TryGetContainingContainer(uid, ref cont, null, null))
			{
				cont.Insert(component.Parent, null, null, null, null, null);
			}
			DamageableComponent damageParent;
			DamageSpecifier damage;
			if (proto.TransferDamage && base.TryComp<DamageableComponent>(component.Parent, ref damageParent) && this._mobThresholdSystem.GetScaledDamage(uid, component.Parent, out damage) && damage != null)
			{
				this._damageable.SetDamage(damageParent, damage);
			}
			if (proto.Inventory == PolymorphInventoryChange.Transfer)
			{
				this._inventory.TransferEntityInventories(uid, component.Parent);
				using (IEnumerator<EntityUid> enumerator2 = this._sharedHands.EnumerateHeld(component.Parent, null).GetEnumerator())
				{
					while (enumerator2.MoveNext())
					{
						EntityUid hand = enumerator2.Current;
						ContainerHelpers.TryRemoveFromContainer(hand, false, null);
						this._sharedHands.TryPickupAnyHand(component.Parent, hand, true, false, null, null);
					}
					goto IL_220;
				}
			}
			if (proto.Inventory == PolymorphInventoryChange.Drop)
			{
				InventorySystem.ContainerSlotEnumerator enumerator;
				if (this._inventory.TryGetContainerSlotEnumerator(uid, out enumerator, null))
				{
					ContainerSlot slot;
					while (enumerator.MoveNext(out slot))
					{
						ContainerHelpers.EmptyContainer(slot, false, null, false, null);
					}
				}
				foreach (EntityUid entityUid in this._sharedHands.EnumerateHeld(uid, null))
				{
					ContainerHelpers.TryRemoveFromContainer(entityUid, false, null);
				}
			}
			IL_220:
			MindComponent mind;
			if (base.TryComp<MindComponent>(uid, ref mind) && mind.Mind != null)
			{
				mind.Mind.TransferTo(new EntityUid?(component.Parent), false, true);
			}
			this._popup.PopupEntity(Loc.GetString("polymorph-revert-popup-generic", new ValueTuple<string, object>[]
			{
				new ValueTuple<string, object>("parent", Identity.Entity(uid, this.EntityManager)),
				new ValueTuple<string, object>("child", Identity.Entity(component.Parent, this.EntityManager))
			}), component.Parent, PopupType.Small);
			base.QueueDel(uid);
		}

		// Token: 0x06000E5F RID: 3679 RVA: 0x00048CCC File Offset: 0x00046ECC
		public void OnInit(EntityUid uid, PolymorphedEntityComponent component, ComponentStartup args)
		{
			PolymorphPrototype proto;
			if (!this._proto.TryIndex<PolymorphPrototype>(component.Prototype, ref proto))
			{
				DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(94, 3);
				defaultInterpolatedStringHandler.AppendFormatted("PolymorphedEntitySystem");
				defaultInterpolatedStringHandler.AppendLiteral(" encountered an improperly set up polymorph component while initializing. Entity ");
				defaultInterpolatedStringHandler.AppendFormatted<EntityStringRepresentation>(base.ToPrettyString(uid));
				defaultInterpolatedStringHandler.AppendLiteral(". Prototype: ");
				defaultInterpolatedStringHandler.AppendFormatted(component.Prototype);
				Logger.Warning(defaultInterpolatedStringHandler.ToStringAndClear());
				base.RemCompDeferred(uid, component);
				return;
			}
			if (proto.Forced)
			{
				return;
			}
			InstantAction act = new InstantAction
			{
				Event = new RevertPolymorphActionEvent(),
				EntityIcon = new EntityUid?(component.Parent),
				DisplayName = Loc.GetString("polymorph-revert-action-name"),
				Description = Loc.GetString("polymorph-revert-action-description"),
				UseDelay = new TimeSpan?(TimeSpan.FromSeconds((double)proto.Delay))
			};
			this._actions.AddAction(uid, act, null, null, true);
		}

		// Token: 0x06000E60 RID: 3680 RVA: 0x00048DC8 File Offset: 0x00046FC8
		public override void Update(float frameTime)
		{
			base.Update(frameTime);
			foreach (PolymorphedEntityComponent comp in base.EntityQuery<PolymorphedEntityComponent>(false))
			{
				comp.Time += frameTime;
				PolymorphPrototype proto;
				if (!this._proto.TryIndex<PolymorphPrototype>(comp.Prototype, ref proto))
				{
					DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(95, 3);
					defaultInterpolatedStringHandler.AppendFormatted("PolymorphedEntitySystem");
					defaultInterpolatedStringHandler.AppendLiteral(" encountered an improperly initialized polymorph component while updating. Entity ");
					defaultInterpolatedStringHandler.AppendFormatted<EntityStringRepresentation>(base.ToPrettyString(comp.Owner));
					defaultInterpolatedStringHandler.AppendLiteral(". Prototype: ");
					defaultInterpolatedStringHandler.AppendFormatted(comp.Prototype);
					Logger.Error(defaultInterpolatedStringHandler.ToStringAndClear());
					base.RemCompDeferred(comp.Owner, comp);
				}
				else
				{
					if (proto.Duration != null)
					{
						float time = comp.Time;
						int? duration = proto.Duration;
						float? num = (duration != null) ? new float?((float)duration.GetValueOrDefault()) : null;
						if (time >= num.GetValueOrDefault() & num != null)
						{
							this.Revert(comp.Owner);
						}
					}
					MobStateComponent mob;
					if (base.TryComp<MobStateComponent>(comp.Owner, ref mob) && ((proto.RevertOnDeath && this._mobStateSystem.IsDead(comp.Owner, mob)) || (proto.RevertOnCrit && this._mobStateSystem.IsCritical(comp.Owner, mob))))
					{
						this.Revert(comp.Owner);
					}
				}
			}
		}

		// Token: 0x04000869 RID: 2153
		[Dependency]
		private readonly IPrototypeManager _proto;

		// Token: 0x0400086A RID: 2154
		[Dependency]
		private readonly ActionsSystem _actions;

		// Token: 0x0400086B RID: 2155
		[Dependency]
		private readonly DamageableSystem _damageable;

		// Token: 0x0400086C RID: 2156
		[Dependency]
		private readonly MobThresholdSystem _mobThresholdSystem;

		// Token: 0x0400086D RID: 2157
		[Dependency]
		private readonly PopupSystem _popup;

		// Token: 0x0400086E RID: 2158
		[Dependency]
		private readonly ServerInventorySystem _inventory;

		// Token: 0x0400086F RID: 2159
		[Dependency]
		private readonly SharedHandsSystem _sharedHands;

		// Token: 0x04000870 RID: 2160
		[Dependency]
		private readonly MobStateSystem _mobStateSystem;

		// Token: 0x04000871 RID: 2161
		[Dependency]
		private readonly ContainerSystem _container;
	}
}
