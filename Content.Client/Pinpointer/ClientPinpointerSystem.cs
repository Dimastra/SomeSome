using System;
using System.Runtime.CompilerServices;
using Content.Shared.Pinpointer;
using Robust.Client.Graphics;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.IoC;
using Robust.Shared.Maths;

namespace Content.Client.Pinpointer
{
	// Token: 0x020001B8 RID: 440
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class ClientPinpointerSystem : SharedPinpointerSystem
	{
		// Token: 0x06000B55 RID: 2901 RVA: 0x00041D2B File Offset: 0x0003FF2B
		public override void Initialize()
		{
			base.Initialize();
			base.SubscribeLocalEvent<PinpointerComponent, ComponentHandleState>(new ComponentEventRefHandler<PinpointerComponent, ComponentHandleState>(this.HandleCompState), null, null);
		}

		// Token: 0x06000B56 RID: 2902 RVA: 0x00041D48 File Offset: 0x0003FF48
		public override void FrameUpdate(float frameTime)
		{
			base.FrameUpdate(frameTime);
			foreach (PinpointerComponent pinpointerComponent in base.EntityQuery<PinpointerComponent>(false))
			{
				this.UpdateAppearance(pinpointerComponent.Owner, pinpointerComponent, null);
				this.UpdateEyeDir(pinpointerComponent.Owner, pinpointerComponent);
			}
		}

		// Token: 0x06000B57 RID: 2903 RVA: 0x00041DB4 File Offset: 0x0003FFB4
		private void HandleCompState(EntityUid uid, PinpointerComponent pinpointer, ref ComponentHandleState args)
		{
			PinpointerComponentState pinpointerComponentState = args.Current as PinpointerComponentState;
			if (pinpointerComponentState == null)
			{
				return;
			}
			pinpointer.IsActive = pinpointerComponentState.IsActive;
			pinpointer.ArrowAngle = pinpointerComponentState.ArrowAngle;
			pinpointer.DistanceToTarget = pinpointerComponentState.DistanceToTarget;
		}

		// Token: 0x06000B58 RID: 2904 RVA: 0x00041DF8 File Offset: 0x0003FFF8
		[NullableContext(2)]
		private void UpdateAppearance(EntityUid uid, PinpointerComponent pinpointer = null, AppearanceComponent appearance = null)
		{
			if (!base.Resolve<PinpointerComponent, AppearanceComponent>(uid, ref pinpointer, ref appearance, true))
			{
				return;
			}
			this._appearance.SetData(uid, PinpointerVisuals.IsActive, pinpointer.IsActive, appearance);
			this._appearance.SetData(uid, PinpointerVisuals.TargetDistance, pinpointer.DistanceToTarget, appearance);
		}

		// Token: 0x06000B59 RID: 2905 RVA: 0x00041E50 File Offset: 0x00040050
		[NullableContext(2)]
		private void UpdateArrowAngle(EntityUid uid, Angle angle, PinpointerComponent pinpointer = null, AppearanceComponent appearance = null)
		{
			if (!base.Resolve<PinpointerComponent, AppearanceComponent>(uid, ref pinpointer, ref appearance, true))
			{
				return;
			}
			this._appearance.SetData(uid, PinpointerVisuals.ArrowAngle, angle, appearance);
		}

		// Token: 0x06000B5A RID: 2906 RVA: 0x00041E7C File Offset: 0x0004007C
		[NullableContext(2)]
		private void UpdateEyeDir(EntityUid uid, PinpointerComponent pinpointer = null)
		{
			if (!base.Resolve<PinpointerComponent>(uid, ref pinpointer, true) || !pinpointer.HasTarget)
			{
				return;
			}
			IEye currentEye = this._eyeManager.CurrentEye;
			Angle angle = pinpointer.ArrowAngle + currentEye.Rotation;
			this.UpdateArrowAngle(uid, angle, pinpointer, null);
		}

		// Token: 0x0400058D RID: 1421
		[Dependency]
		private readonly IEyeManager _eyeManager;

		// Token: 0x0400058E RID: 1422
		[Dependency]
		private readonly SharedAppearanceSystem _appearance;
	}
}
