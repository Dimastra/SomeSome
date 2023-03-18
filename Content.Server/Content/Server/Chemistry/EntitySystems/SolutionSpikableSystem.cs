using System;
using System.Runtime.CompilerServices;
using Content.Server.Chemistry.Components;
using Content.Server.Chemistry.Components.SolutionManager;
using Content.Server.Explosion.EntitySystems;
using Content.Server.Popups;
using Content.Shared.Chemistry.Components;
using Content.Shared.Interaction;
using Content.Shared.Popups;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Localization;

namespace Content.Server.Chemistry.EntitySystems
{
	// Token: 0x0200069E RID: 1694
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class SolutionSpikableSystem : EntitySystem
	{
		// Token: 0x0600235A RID: 9050 RVA: 0x000B893D File Offset: 0x000B6B3D
		public override void Initialize()
		{
			base.SubscribeLocalEvent<RefillableSolutionComponent, InteractUsingEvent>(new ComponentEventHandler<RefillableSolutionComponent, InteractUsingEvent>(this.OnInteractUsing), null, null);
		}

		// Token: 0x0600235B RID: 9051 RVA: 0x000B8953 File Offset: 0x000B6B53
		private void OnInteractUsing(EntityUid uid, RefillableSolutionComponent target, InteractUsingEvent args)
		{
			this.TrySpike(args.Used, args.Target, args.User, target, null, null, null);
		}

		// Token: 0x0600235C RID: 9052 RVA: 0x000B8974 File Offset: 0x000B6B74
		[NullableContext(2)]
		private void TrySpike(EntityUid source, EntityUid target, EntityUid user, RefillableSolutionComponent spikableTarget = null, SolutionSpikerComponent spikableSource = null, SolutionContainerManagerComponent managerSource = null, SolutionContainerManagerComponent managerTarget = null)
		{
			Solution targetSolution;
			Solution sourceSolution;
			if (!base.Resolve<SolutionSpikerComponent, SolutionContainerManagerComponent>(source, ref spikableSource, ref managerSource, false) || !base.Resolve<RefillableSolutionComponent, SolutionContainerManagerComponent>(target, ref spikableTarget, ref managerTarget, false) || !this._solutionSystem.TryGetRefillableSolution(target, out targetSolution, managerTarget, spikableTarget) || !managerSource.Solutions.TryGetValue(spikableSource.SourceSolution, out sourceSolution))
			{
				return;
			}
			if (targetSolution.Volume == 0 && !spikableSource.IgnoreEmpty)
			{
				this._popupSystem.PopupEntity(Loc.GetString(spikableSource.PopupEmpty, new ValueTuple<string, object>[]
				{
					new ValueTuple<string, object>("spiked-entity", target),
					new ValueTuple<string, object>("spike-entity", source)
				}), user, user, PopupType.Small);
				return;
			}
			Solution overflow;
			if (this._solutionSystem.TryMixAndOverflow(target, targetSolution, sourceSolution, targetSolution.MaxVolume, out overflow))
			{
				if (overflow.Volume > 0)
				{
					base.RaiseLocalEvent<SolutionSpikeOverflowEvent>(target, new SolutionSpikeOverflowEvent(overflow), false);
				}
				this._popupSystem.PopupEntity(Loc.GetString(spikableSource.Popup, new ValueTuple<string, object>[]
				{
					new ValueTuple<string, object>("spiked-entity", target),
					new ValueTuple<string, object>("spike-entity", source)
				}), user, user, PopupType.Small);
				sourceSolution.RemoveAllSolution();
				this._triggerSystem.Trigger(source, new EntityUid?(user));
			}
		}

		// Token: 0x040015C3 RID: 5571
		[Dependency]
		private readonly SolutionContainerSystem _solutionSystem;

		// Token: 0x040015C4 RID: 5572
		[Dependency]
		private readonly TriggerSystem _triggerSystem;

		// Token: 0x040015C5 RID: 5573
		[Dependency]
		private readonly PopupSystem _popupSystem;
	}
}
