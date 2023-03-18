using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Content.Server.Atmos;
using Content.Server.Atmos.EntitySystems;
using Content.Server.Xenoarchaeology.XenoArtifacts.Triggers.Components;
using Robust.Server.GameObjects;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;

namespace Content.Server.Xenoarchaeology.XenoArtifacts.Triggers.Systems
{
	// Token: 0x0200002D RID: 45
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class ArtifactPressureTriggerSystem : EntitySystem
	{
		// Token: 0x060000AC RID: 172 RVA: 0x000058A8 File Offset: 0x00003AA8
		public override void Update(float frameTime)
		{
			base.Update(frameTime);
			List<ArtifactComponent> toUpdate = new List<ArtifactComponent>();
			foreach (ValueTuple<ArtifactPressureTriggerComponent, ArtifactComponent, TransformComponent> valueTuple in base.EntityQuery<ArtifactPressureTriggerComponent, ArtifactComponent, TransformComponent>(false))
			{
				ArtifactPressureTriggerComponent trigger = valueTuple.Item1;
				ArtifactComponent artifact = valueTuple.Item2;
				TransformComponent transform = valueTuple.Item3;
				EntityUid uid = trigger.Owner;
				GasMixture environment = this._atmosphereSystem.GetTileMixture(transform.GridUid, transform.MapUid, this._transformSystem.GetGridOrMapTilePosition(uid, transform), false);
				if (environment != null)
				{
					float pressure = environment.Pressure;
					float num = pressure;
					float? num2 = trigger.MaxPressureThreshold;
					if (!(num >= num2.GetValueOrDefault() & num2 != null))
					{
						float num3 = pressure;
						num2 = trigger.MinPressureThreshold;
						if (!(num3 <= num2.GetValueOrDefault() & num2 != null))
						{
							continue;
						}
					}
					toUpdate.Add(artifact);
				}
			}
			foreach (ArtifactComponent a in toUpdate)
			{
				this._artifactSystem.TryActivateArtifact(a.Owner, null, a);
			}
		}

		// Token: 0x04000072 RID: 114
		[Dependency]
		private readonly AtmosphereSystem _atmosphereSystem;

		// Token: 0x04000073 RID: 115
		[Dependency]
		private readonly ArtifactSystem _artifactSystem;

		// Token: 0x04000074 RID: 116
		[Dependency]
		private readonly TransformSystem _transformSystem;
	}
}
