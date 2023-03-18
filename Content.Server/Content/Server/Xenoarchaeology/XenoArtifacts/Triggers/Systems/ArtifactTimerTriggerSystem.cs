using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Content.Server.Xenoarchaeology.XenoArtifacts.Triggers.Components;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Timing;

namespace Content.Server.Xenoarchaeology.XenoArtifacts.Triggers.Systems
{
	// Token: 0x0200002E RID: 46
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class ArtifactTimerTriggerSystem : EntitySystem
	{
		// Token: 0x060000AE RID: 174 RVA: 0x000059FC File Offset: 0x00003BFC
		public override void Initialize()
		{
			base.Initialize();
			base.SubscribeLocalEvent<ArtifactTimerTriggerComponent, ComponentStartup>(new ComponentEventHandler<ArtifactTimerTriggerComponent, ComponentStartup>(this.OnStartup), null, null);
		}

		// Token: 0x060000AF RID: 175 RVA: 0x00005A18 File Offset: 0x00003C18
		private void OnStartup(EntityUid uid, ArtifactTimerTriggerComponent component, ComponentStartup args)
		{
			component.LastActivation = this._time.CurTime;
		}

		// Token: 0x060000B0 RID: 176 RVA: 0x00005A2C File Offset: 0x00003C2C
		public override void Update(float frameTime)
		{
			base.Update(frameTime);
			List<ArtifactComponent> toUpdate = new List<ArtifactComponent>();
			foreach (ValueTuple<ArtifactTimerTriggerComponent, ArtifactComponent> valueTuple in base.EntityQuery<ArtifactTimerTriggerComponent, ArtifactComponent>(false))
			{
				ArtifactTimerTriggerComponent trigger = valueTuple.Item1;
				ArtifactComponent artifact = valueTuple.Item2;
				if (!(this._time.CurTime - trigger.LastActivation <= trigger.ActivationRate))
				{
					toUpdate.Add(artifact);
					trigger.LastActivation = this._time.CurTime;
				}
			}
			foreach (ArtifactComponent a in toUpdate)
			{
				this._artifactSystem.TryActivateArtifact(a.Owner, null, a);
			}
		}

		// Token: 0x04000075 RID: 117
		[Dependency]
		private readonly IGameTiming _time;

		// Token: 0x04000076 RID: 118
		[Dependency]
		private readonly ArtifactSystem _artifactSystem;
	}
}
