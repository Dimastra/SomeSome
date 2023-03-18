using System;
using System.Runtime.CompilerServices;
using Content.Shared.Inventory;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;

namespace Content.Shared.Electrocution
{
	// Token: 0x020004D0 RID: 1232
	[NullableContext(1)]
	[Nullable(0)]
	public abstract class SharedElectrocutionSystem : EntitySystem
	{
		// Token: 0x06000EE4 RID: 3812 RVA: 0x0002FDD0 File Offset: 0x0002DFD0
		public override void Initialize()
		{
			base.Initialize();
			base.SubscribeLocalEvent<InsulatedComponent, ElectrocutionAttemptEvent>(new ComponentEventHandler<InsulatedComponent, ElectrocutionAttemptEvent>(this.OnInsulatedElectrocutionAttempt), null, null);
			base.SubscribeLocalEvent<InsulatedComponent, InventoryRelayedEvent<ElectrocutionAttemptEvent>>(delegate(EntityUid e, InsulatedComponent c, InventoryRelayedEvent<ElectrocutionAttemptEvent> ev)
			{
				this.OnInsulatedElectrocutionAttempt(e, c, ev.Args);
			}, null, null);
			base.SubscribeLocalEvent<InsulatedComponent, ComponentGetState>(new ComponentEventRefHandler<InsulatedComponent, ComponentGetState>(this.OnInsulatedGetState), null, null);
			base.SubscribeLocalEvent<InsulatedComponent, ComponentHandleState>(new ComponentEventRefHandler<InsulatedComponent, ComponentHandleState>(this.OnInsulatedHandleState), null, null);
		}

		// Token: 0x06000EE5 RID: 3813 RVA: 0x0002FE33 File Offset: 0x0002E033
		[NullableContext(2)]
		public void SetInsulatedSiemensCoefficient(EntityUid uid, float siemensCoefficient, InsulatedComponent insulated = null)
		{
			if (!base.Resolve<InsulatedComponent>(uid, ref insulated, true))
			{
				return;
			}
			insulated.SiemensCoefficient = siemensCoefficient;
			base.Dirty(insulated, null);
		}

		// Token: 0x06000EE6 RID: 3814 RVA: 0x0002FE51 File Offset: 0x0002E051
		private void OnInsulatedElectrocutionAttempt(EntityUid uid, InsulatedComponent insulated, ElectrocutionAttemptEvent args)
		{
			args.SiemensCoefficient *= insulated.SiemensCoefficient;
		}

		// Token: 0x06000EE7 RID: 3815 RVA: 0x0002FE66 File Offset: 0x0002E066
		private void OnInsulatedGetState(EntityUid uid, InsulatedComponent insulated, ref ComponentGetState args)
		{
			args.State = new InsulatedComponentState(insulated.SiemensCoefficient);
		}

		// Token: 0x06000EE8 RID: 3816 RVA: 0x0002FE7C File Offset: 0x0002E07C
		private void OnInsulatedHandleState(EntityUid uid, InsulatedComponent insulated, ref ComponentHandleState args)
		{
			InsulatedComponentState state = args.Current as InsulatedComponentState;
			if (state == null)
			{
				return;
			}
			insulated.SiemensCoefficient = state.SiemensCoefficient;
		}
	}
}
