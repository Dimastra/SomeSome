using System;
using System.Runtime.CompilerServices;
using Content.Server.Atmos.Components;
using Content.Server.Atmos.EntitySystems;
using Content.Shared.StepTrigger.Systems;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;

namespace Content.Server.Tiles
{
	// Token: 0x02000123 RID: 291
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class LavaSystem : EntitySystem
	{
		// Token: 0x0600053E RID: 1342 RVA: 0x000198EB File Offset: 0x00017AEB
		public override void Initialize()
		{
			base.Initialize();
			base.SubscribeLocalEvent<LavaComponent, StepTriggeredEvent>(new ComponentEventRefHandler<LavaComponent, StepTriggeredEvent>(this.OnLavaStepTriggered), null, null);
			base.SubscribeLocalEvent<LavaComponent, StepTriggerAttemptEvent>(new ComponentEventRefHandler<LavaComponent, StepTriggerAttemptEvent>(this.OnLavaStepTriggerAttempt), null, null);
		}

		// Token: 0x0600053F RID: 1343 RVA: 0x0001991B File Offset: 0x00017B1B
		private void OnLavaStepTriggerAttempt(EntityUid uid, LavaComponent component, ref StepTriggerAttemptEvent args)
		{
			if (!base.HasComp<FlammableComponent>(args.Tripper))
			{
				return;
			}
			args.Continue = true;
		}

		// Token: 0x06000540 RID: 1344 RVA: 0x00019934 File Offset: 0x00017B34
		private void OnLavaStepTriggered(EntityUid uid, LavaComponent component, ref StepTriggeredEvent args)
		{
			EntityUid otherUid = args.Tripper;
			FlammableComponent flammable;
			if (base.TryComp<FlammableComponent>(otherUid, ref flammable))
			{
				float multiplier = (flammable.FireStacks == 0f) ? 5f : 1f;
				this._flammable.AdjustFireStacks(otherUid, component.FireStacks * multiplier, flammable);
				this._flammable.Ignite(otherUid, flammable);
			}
		}

		// Token: 0x0400032A RID: 810
		[Dependency]
		private readonly FlammableSystem _flammable;
	}
}
