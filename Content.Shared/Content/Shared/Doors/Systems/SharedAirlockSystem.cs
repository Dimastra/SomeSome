using System;
using System.Runtime.CompilerServices;
using Content.Shared.Doors.Components;
using Content.Shared.Popups;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.IoC;

namespace Content.Shared.Doors.Systems
{
	// Token: 0x020004E8 RID: 1256
	[NullableContext(1)]
	[Nullable(0)]
	public abstract class SharedAirlockSystem : EntitySystem
	{
		// Token: 0x06000F28 RID: 3880 RVA: 0x00030894 File Offset: 0x0002EA94
		public override void Initialize()
		{
			base.Initialize();
			base.SubscribeLocalEvent<AirlockComponent, ComponentGetState>(new ComponentEventRefHandler<AirlockComponent, ComponentGetState>(this.OnGetState), null, null);
			base.SubscribeLocalEvent<AirlockComponent, ComponentHandleState>(new ComponentEventRefHandler<AirlockComponent, ComponentHandleState>(this.OnHandleState), null, null);
			base.SubscribeLocalEvent<AirlockComponent, BeforeDoorClosedEvent>(new ComponentEventHandler<AirlockComponent, BeforeDoorClosedEvent>(this.OnBeforeDoorClosed), null, null);
		}

		// Token: 0x06000F29 RID: 3881 RVA: 0x000308E4 File Offset: 0x0002EAE4
		private void OnGetState(EntityUid uid, AirlockComponent airlock, ref ComponentGetState args)
		{
			args.State = new AirlockComponentState(airlock.Safety);
		}

		// Token: 0x06000F2A RID: 3882 RVA: 0x000308F8 File Offset: 0x0002EAF8
		private void OnHandleState(EntityUid uid, AirlockComponent airlock, ref ComponentHandleState args)
		{
			AirlockComponentState state = args.Current as AirlockComponentState;
			if (state == null)
			{
				return;
			}
			airlock.Safety = state.Safety;
		}

		// Token: 0x06000F2B RID: 3883 RVA: 0x00030921 File Offset: 0x0002EB21
		protected virtual void OnBeforeDoorClosed(EntityUid uid, AirlockComponent airlock, BeforeDoorClosedEvent args)
		{
			if (!airlock.Safety)
			{
				args.PerformCollisionCheck = false;
			}
		}

		// Token: 0x06000F2C RID: 3884 RVA: 0x00030932 File Offset: 0x0002EB32
		public void UpdateEmergencyLightStatus(EntityUid uid, AirlockComponent component)
		{
			this.Appearance.SetData(uid, DoorVisuals.EmergencyLights, component.EmergencyAccess, null);
		}

		// Token: 0x06000F2D RID: 3885 RVA: 0x00030952 File Offset: 0x0002EB52
		public void ToggleEmergencyAccess(EntityUid uid, AirlockComponent component)
		{
			component.EmergencyAccess = !component.EmergencyAccess;
			this.UpdateEmergencyLightStatus(uid, component);
		}

		// Token: 0x06000F2E RID: 3886 RVA: 0x0003096B File Offset: 0x0002EB6B
		public void SetAutoCloseDelayModifier(AirlockComponent component, float value)
		{
			if (component.AutoCloseDelayModifier.Equals(value))
			{
				return;
			}
			component.AutoCloseDelayModifier = value;
		}

		// Token: 0x06000F2F RID: 3887 RVA: 0x00030983 File Offset: 0x0002EB83
		public void SetSafety(AirlockComponent component, bool value)
		{
			component.Safety = value;
		}

		// Token: 0x06000F30 RID: 3888 RVA: 0x0003098C File Offset: 0x0002EB8C
		public void SetBoltWireCut(AirlockComponent component, bool value)
		{
			component.BoltWireCut = value;
		}

		// Token: 0x04000E37 RID: 3639
		[Dependency]
		protected readonly SharedAppearanceSystem Appearance;

		// Token: 0x04000E38 RID: 3640
		[Dependency]
		protected readonly SharedAudioSystem Audio;

		// Token: 0x04000E39 RID: 3641
		[Dependency]
		protected readonly SharedDoorSystem DoorSystem;

		// Token: 0x04000E3A RID: 3642
		[Dependency]
		protected readonly SharedPopupSystem Popup;
	}
}
