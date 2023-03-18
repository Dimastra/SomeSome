using System;
using System.Runtime.CompilerServices;
using System.Threading;
using Content.Server.DoAfter;
using Content.Server.Mech.Components;
using Content.Server.Popups;
using Content.Shared.DoAfter;
using Content.Shared.Interaction;
using Content.Shared.Mech.Equipment.Components;
using Content.Shared.Popups;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Localization;

namespace Content.Server.Mech.Systems
{
	// Token: 0x020003C5 RID: 965
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class MechEquipmentSystem : EntitySystem
	{
		// Token: 0x060013D3 RID: 5075 RVA: 0x00066D20 File Offset: 0x00064F20
		public override void Initialize()
		{
			base.SubscribeLocalEvent<MechEquipmentComponent, AfterInteractEvent>(new ComponentEventHandler<MechEquipmentComponent, AfterInteractEvent>(this.OnUsed), null, null);
			base.SubscribeLocalEvent<MechEquipmentComponent, DoAfterEvent<MechEquipmentSystem.InsertEquipmentEvent>>(new ComponentEventHandler<MechEquipmentComponent, DoAfterEvent<MechEquipmentSystem.InsertEquipmentEvent>>(this.OnInsertEquipment), null, null);
		}

		// Token: 0x060013D4 RID: 5076 RVA: 0x00066D4C File Offset: 0x00064F4C
		private void OnUsed(EntityUid uid, MechEquipmentComponent component, AfterInteractEvent args)
		{
			if (args.Handled || !args.CanReach || args.Target == null)
			{
				return;
			}
			EntityUid mech = args.Target.Value;
			MechComponent mechComp;
			if (!base.TryComp<MechComponent>(mech, ref mechComp))
			{
				return;
			}
			if (mechComp.Broken)
			{
				return;
			}
			EntityUid user = args.User;
			EntityUid? containedEntity = mechComp.PilotSlot.ContainedEntity;
			if (user == containedEntity)
			{
				return;
			}
			if (mechComp.EquipmentContainer.ContainedEntities.Count >= mechComp.MaxEquipmentAmount)
			{
				return;
			}
			if (mechComp.EquipmentWhitelist != null && !mechComp.EquipmentWhitelist.IsValid(uid, null))
			{
				return;
			}
			this._popup.PopupEntity(Loc.GetString("mech-equipment-begin-install", new ValueTuple<string, object>[]
			{
				new ValueTuple<string, object>("item", uid)
			}), mech, PopupType.Small);
			MechEquipmentSystem.InsertEquipmentEvent insertEquipment = new MechEquipmentSystem.InsertEquipmentEvent();
			EntityUid user2 = args.User;
			float installDuration = component.InstallDuration;
			containedEntity = new EntityUid?(mech);
			EntityUid? used = new EntityUid?(uid);
			DoAfterEventArgs doAfterEventArgs = new DoAfterEventArgs(user2, installDuration, default(CancellationToken), containedEntity, used)
			{
				BreakOnStun = true,
				BreakOnTargetMove = true,
				BreakOnUserMove = true
			};
			this._doAfter.DoAfter<MechEquipmentSystem.InsertEquipmentEvent>(doAfterEventArgs, insertEquipment);
		}

		// Token: 0x060013D5 RID: 5077 RVA: 0x00066E90 File Offset: 0x00065090
		private void OnInsertEquipment(EntityUid uid, MechEquipmentComponent component, DoAfterEvent<MechEquipmentSystem.InsertEquipmentEvent> args)
		{
			if (args.Handled || args.Cancelled || args.Args.Target == null)
			{
				return;
			}
			this._popup.PopupEntity(Loc.GetString("mech-equipment-finish-install", new ValueTuple<string, object>[]
			{
				new ValueTuple<string, object>("item", uid)
			}), args.Args.Target.Value, PopupType.Small);
			this._mech.InsertEquipment(args.Args.Target.Value, uid, null, null);
			args.Handled = true;
		}

		// Token: 0x04000C43 RID: 3139
		[Dependency]
		private readonly MechSystem _mech;

		// Token: 0x04000C44 RID: 3140
		[Dependency]
		private readonly DoAfterSystem _doAfter;

		// Token: 0x04000C45 RID: 3141
		[Dependency]
		private readonly PopupSystem _popup;

		// Token: 0x020009A8 RID: 2472
		[NullableContext(0)]
		private sealed class InsertEquipmentEvent : EntityEventArgs
		{
		}
	}
}
