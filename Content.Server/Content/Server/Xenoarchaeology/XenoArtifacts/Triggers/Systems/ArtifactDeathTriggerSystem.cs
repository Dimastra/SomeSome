using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Content.Server.Xenoarchaeology.XenoArtifacts.Triggers.Components;
using Content.Shared.Mobs;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;

namespace Content.Server.Xenoarchaeology.XenoArtifacts.Triggers.Systems
{
	// Token: 0x02000023 RID: 35
	public sealed class ArtifactDeathTriggerSystem : EntitySystem
	{
		// Token: 0x06000086 RID: 134 RVA: 0x00004D58 File Offset: 0x00002F58
		public override void Initialize()
		{
			base.SubscribeLocalEvent<MobStateChangedEvent>(new EntityEventHandler<MobStateChangedEvent>(this.OnMobStateChanged), null, null);
		}

		// Token: 0x06000087 RID: 135 RVA: 0x00004D70 File Offset: 0x00002F70
		private void OnMobStateChanged(MobStateChangedEvent ev)
		{
			if (ev.NewMobState != MobState.Dead)
			{
				return;
			}
			TransformComponent deathXform = base.Transform(ev.Target);
			List<ArtifactDeathTriggerComponent> toActivate = new List<ArtifactDeathTriggerComponent>();
			foreach (ValueTuple<ArtifactDeathTriggerComponent, TransformComponent> valueTuple in base.EntityQuery<ArtifactDeathTriggerComponent, TransformComponent>(false))
			{
				ArtifactDeathTriggerComponent trigger = valueTuple.Item1;
				TransformComponent xform = valueTuple.Item2;
				float distance;
				if (deathXform.Coordinates.TryDistance(this.EntityManager, xform.Coordinates, ref distance) && distance <= trigger.Range)
				{
					toActivate.Add(trigger);
				}
			}
			foreach (ArtifactDeathTriggerComponent a in toActivate)
			{
				this._artifact.TryActivateArtifact(a.Owner, null, null);
			}
		}

		// Token: 0x04000064 RID: 100
		[Nullable(1)]
		[Dependency]
		private readonly ArtifactSystem _artifact;
	}
}
