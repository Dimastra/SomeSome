using System;
using System.Runtime.CompilerServices;
using Content.Client.Rotation;
using Content.Shared.ActionBlocker;
using Content.Shared.Buckle;
using Content.Shared.Buckle.Components;
using Content.Shared.Vehicle.Components;
using Robust.Client.GameObjects;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.IoC;
using Robust.Shared.Maths;

namespace Content.Client.Buckle
{
	// Token: 0x02000413 RID: 1043
	[NullableContext(1)]
	[Nullable(0)]
	internal sealed class BuckleSystem : SharedBuckleSystem
	{
		// Token: 0x060019A9 RID: 6569 RVA: 0x00093489 File Offset: 0x00091689
		public override void Initialize()
		{
			base.Initialize();
			base.SubscribeLocalEvent<BuckleComponent, ComponentHandleState>(new ComponentEventRefHandler<BuckleComponent, ComponentHandleState>(this.OnBuckleHandleState), null, null);
			base.SubscribeLocalEvent<BuckleComponent, AppearanceChangeEvent>(new ComponentEventRefHandler<BuckleComponent, AppearanceChangeEvent>(this.OnAppearanceChange), null, null);
		}

		// Token: 0x060019AA RID: 6570 RVA: 0x000934BC File Offset: 0x000916BC
		private void OnBuckleHandleState(EntityUid uid, BuckleComponent buckle, ref ComponentHandleState args)
		{
			BuckleComponentState buckleComponentState = args.Current as BuckleComponentState;
			if (buckleComponentState == null)
			{
				return;
			}
			buckle.Buckled = buckleComponentState.Buckled;
			buckle.LastEntityBuckledTo = buckleComponentState.LastEntityBuckledTo;
			buckle.DontCollide = buckleComponentState.DontCollide;
			this._actionBlocker.UpdateCanMove(uid, null);
			SpriteComponent spriteComponent;
			if (!base.TryComp<SpriteComponent>(uid, ref spriteComponent))
			{
				return;
			}
			if (base.HasComp<VehicleComponent>(buckle.LastEntityBuckledTo))
			{
				return;
			}
			SpriteComponent spriteComponent2;
			if (buckle.Buckled && buckle.LastEntityBuckledTo != null && base.Transform(buckle.LastEntityBuckledTo.Value).LocalRotation.GetCardinalDir() == 4 && base.TryComp<SpriteComponent>(buckle.LastEntityBuckledTo, ref spriteComponent2))
			{
				int? originalDrawDepth = buckle.OriginalDrawDepth;
				int value = originalDrawDepth.GetValueOrDefault();
				if (originalDrawDepth == null)
				{
					value = spriteComponent.DrawDepth;
					int? originalDrawDepth2 = new int?(value);
					buckle.OriginalDrawDepth = originalDrawDepth2;
				}
				spriteComponent.DrawDepth = spriteComponent2.DrawDepth - 1;
				return;
			}
			if (buckle.OriginalDrawDepth != null)
			{
				spriteComponent.DrawDepth = buckle.OriginalDrawDepth.Value;
				buckle.OriginalDrawDepth = null;
			}
		}

		// Token: 0x060019AB RID: 6571 RVA: 0x000935F4 File Offset: 0x000917F4
		private void OnAppearanceChange(EntityUid uid, BuckleComponent component, ref AppearanceChangeEvent args)
		{
			RotationVisualsComponent rotationVisualsComponent;
			if (!base.TryComp<RotationVisualsComponent>(uid, ref rotationVisualsComponent))
			{
				return;
			}
			int num;
			bool flag;
			if (!this._appearanceSystem.TryGetData<int>(uid, StrapVisuals.RotationAngle, ref num, args.Component) || !this._appearanceSystem.TryGetData<bool>(uid, BuckleVisuals.Buckled, ref flag, args.Component) || !flag || args.Sprite == null)
			{
				this._rotationVisualizerSystem.SetHorizontalAngle(uid, RotationVisualsComponent.DefaultRotation, rotationVisualsComponent);
				return;
			}
			this._rotationVisualizerSystem.SetHorizontalAngle(uid, Angle.FromDegrees((double)num), rotationVisualsComponent);
			this._rotationVisualizerSystem.AnimateSpriteRotation(uid, args.Sprite, rotationVisualsComponent.HorizontalRotation, 0.125f);
		}

		// Token: 0x04000D03 RID: 3331
		[Dependency]
		private readonly ActionBlockerSystem _actionBlocker;

		// Token: 0x04000D04 RID: 3332
		[Dependency]
		private readonly AppearanceSystem _appearanceSystem;

		// Token: 0x04000D05 RID: 3333
		[Dependency]
		private readonly RotationVisualizerSystem _rotationVisualizerSystem;
	}
}
