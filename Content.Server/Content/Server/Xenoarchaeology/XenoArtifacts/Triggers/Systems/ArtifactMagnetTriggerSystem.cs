using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Content.Server.Salvage;
using Content.Server.Xenoarchaeology.XenoArtifacts.Triggers.Components;
using Content.Shared.Clothing;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;

namespace Content.Server.Xenoarchaeology.XenoArtifacts.Triggers.Systems
{
	// Token: 0x0200002A RID: 42
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class ArtifactMagnetTriggerSystem : EntitySystem
	{
		// Token: 0x060000A3 RID: 163 RVA: 0x000054C0 File Offset: 0x000036C0
		public override void Initialize()
		{
			base.SubscribeLocalEvent<SalvageMagnetActivatedEvent>(new EntityEventHandler<SalvageMagnetActivatedEvent>(this.OnMagnetActivated), null, null);
		}

		// Token: 0x060000A4 RID: 164 RVA: 0x000054D8 File Offset: 0x000036D8
		public override void Update(float frameTime)
		{
			base.Update(frameTime);
			HashSet<ValueTuple<ArtifactMagnetTriggerComponent, TransformComponent>> artifactQuery = base.EntityQuery<ArtifactMagnetTriggerComponent, TransformComponent>(false).ToHashSet<ValueTuple<ArtifactMagnetTriggerComponent, TransformComponent>>();
			if (!artifactQuery.Any<ValueTuple<ArtifactMagnetTriggerComponent, TransformComponent>>())
			{
				return;
			}
			List<EntityUid> toActivate = new List<EntityUid>();
			foreach (MagbootsComponent magboot in base.EntityQuery<MagbootsComponent>(false))
			{
				if (magboot.On)
				{
					TransformComponent magXform = base.Transform(magboot.Owner);
					foreach (ValueTuple<ArtifactMagnetTriggerComponent, TransformComponent> valueTuple in artifactQuery)
					{
						ArtifactMagnetTriggerComponent trigger = valueTuple.Item1;
						TransformComponent xform = valueTuple.Item2;
						float distance;
						if (magXform.Coordinates.TryDistance(this.EntityManager, xform.Coordinates, ref distance) && distance <= trigger.Range)
						{
							toActivate.Add(trigger.Owner);
						}
					}
				}
			}
			foreach (EntityUid a in toActivate)
			{
				this._artifact.TryActivateArtifact(a, null, null);
			}
		}

		// Token: 0x060000A5 RID: 165 RVA: 0x00005630 File Offset: 0x00003830
		private void OnMagnetActivated(SalvageMagnetActivatedEvent ev)
		{
			TransformComponent magXform = base.Transform(ev.Magnet);
			List<EntityUid> toActivate = new List<EntityUid>();
			foreach (ValueTuple<ArtifactMagnetTriggerComponent, TransformComponent> valueTuple in base.EntityQuery<ArtifactMagnetTriggerComponent, TransformComponent>(false))
			{
				ArtifactMagnetTriggerComponent artifact = valueTuple.Item1;
				TransformComponent xform = valueTuple.Item2;
				float distance;
				if (magXform.Coordinates.TryDistance(this.EntityManager, xform.Coordinates, ref distance) && distance <= artifact.Range)
				{
					toActivate.Add(artifact.Owner);
				}
			}
			foreach (EntityUid a in toActivate)
			{
				this._artifact.TryActivateArtifact(a, null, null);
			}
		}

		// Token: 0x0400006F RID: 111
		[Dependency]
		private readonly ArtifactSystem _artifact;
	}
}
