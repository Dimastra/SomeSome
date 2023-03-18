using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Content.Server.Instruments;
using Content.Server.Xenoarchaeology.XenoArtifacts.Triggers.Components;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;

namespace Content.Server.Xenoarchaeology.XenoArtifacts.Triggers.Systems
{
	// Token: 0x0200002C RID: 44
	public sealed class ArtifactMusicTriggerSystem : EntitySystem
	{
		// Token: 0x060000AA RID: 170 RVA: 0x0000576C File Offset: 0x0000396C
		public override void Update(float frameTime)
		{
			base.Update(frameTime);
			ValueTuple<ArtifactMusicTriggerComponent, TransformComponent>[] artifactQuery = base.EntityQuery<ArtifactMusicTriggerComponent, TransformComponent>(false).ToArray<ValueTuple<ArtifactMusicTriggerComponent, TransformComponent>>();
			if (!artifactQuery.Any<ValueTuple<ArtifactMusicTriggerComponent, TransformComponent>>())
			{
				return;
			}
			List<EntityUid> toActivate = new List<EntityUid>();
			foreach (ActiveInstrumentComponent activeinstrument in base.EntityQuery<ActiveInstrumentComponent>(false))
			{
				TransformComponent instXform = base.Transform(activeinstrument.Owner);
				foreach (ValueTuple<ArtifactMusicTriggerComponent, TransformComponent> valueTuple in artifactQuery)
				{
					ArtifactMusicTriggerComponent trigger = valueTuple.Item1;
					TransformComponent xform = valueTuple.Item2;
					float distance;
					if (instXform.Coordinates.TryDistance(this.EntityManager, xform.Coordinates, ref distance) && distance <= trigger.Range)
					{
						toActivate.Add(trigger.Owner);
					}
				}
			}
			foreach (EntityUid a in toActivate)
			{
				this._artifact.TryActivateArtifact(a, null, null);
			}
		}

		// Token: 0x04000071 RID: 113
		[Nullable(1)]
		[Dependency]
		private readonly ArtifactSystem _artifact;
	}
}
