using System;
using System.Runtime.CompilerServices;
using Content.Server.Explosion.EntitySystems;
using Content.Shared.Popups;
using Content.Shared.StepTrigger.Systems;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Localization;

namespace Content.Server.LandMines
{
	// Token: 0x02000424 RID: 1060
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class LandMineSystem : EntitySystem
	{
		// Token: 0x06001573 RID: 5491 RVA: 0x00070971 File Offset: 0x0006EB71
		public override void Initialize()
		{
			base.SubscribeLocalEvent<LandMineComponent, StepTriggeredEvent>(new ComponentEventRefHandler<LandMineComponent, StepTriggeredEvent>(this.HandleTriggered), null, null);
			ComponentEventRefHandler<LandMineComponent, StepTriggerAttemptEvent> componentEventRefHandler;
			if ((componentEventRefHandler = LandMineSystem.<>O.<0>__HandleTriggerAttempt) == null)
			{
				componentEventRefHandler = (LandMineSystem.<>O.<0>__HandleTriggerAttempt = new ComponentEventRefHandler<LandMineComponent, StepTriggerAttemptEvent>(LandMineSystem.HandleTriggerAttempt));
			}
			base.SubscribeLocalEvent<LandMineComponent, StepTriggerAttemptEvent>(componentEventRefHandler, null, null);
		}

		// Token: 0x06001574 RID: 5492 RVA: 0x000709AA File Offset: 0x0006EBAA
		private static void HandleTriggerAttempt(EntityUid uid, LandMineComponent component, ref StepTriggerAttemptEvent args)
		{
			args.Continue = true;
		}

		// Token: 0x06001575 RID: 5493 RVA: 0x000709B4 File Offset: 0x0006EBB4
		private void HandleTriggered(EntityUid uid, LandMineComponent component, ref StepTriggeredEvent args)
		{
			if (this._trigger.Trigger(uid, new EntityUid?(args.Tripper)))
			{
				this._popupSystem.PopupCoordinates(Loc.GetString("land-mine-triggered", new ValueTuple<string, object>[]
				{
					new ValueTuple<string, object>("mine", uid)
				}), base.Transform(uid).Coordinates, args.Tripper, PopupType.LargeCaution);
			}
		}

		// Token: 0x04000D59 RID: 3417
		[Dependency]
		private readonly SharedPopupSystem _popupSystem;

		// Token: 0x04000D5A RID: 3418
		[Dependency]
		private readonly TriggerSystem _trigger;

		// Token: 0x020009C2 RID: 2498
		[CompilerGenerated]
		private static class <>O
		{
			// Token: 0x04002204 RID: 8708
			[Nullable(new byte[]
			{
				0,
				1
			})]
			public static ComponentEventRefHandler<LandMineComponent, StepTriggerAttemptEvent> <0>__HandleTriggerAttempt;
		}
	}
}
