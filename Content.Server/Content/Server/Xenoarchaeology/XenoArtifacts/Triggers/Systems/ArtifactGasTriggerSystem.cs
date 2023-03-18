using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Content.Server.Atmos;
using Content.Server.Atmos.EntitySystems;
using Content.Server.Xenoarchaeology.XenoArtifacts.Events;
using Content.Server.Xenoarchaeology.XenoArtifacts.Triggers.Components;
using Content.Shared.Atmos;
using Robust.Server.GameObjects;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;

namespace Content.Server.Xenoarchaeology.XenoArtifacts.Triggers.Systems
{
	// Token: 0x02000026 RID: 38
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class ArtifactGasTriggerSystem : EntitySystem
	{
		// Token: 0x06000091 RID: 145 RVA: 0x0000503C File Offset: 0x0000323C
		public override void Initialize()
		{
			base.Initialize();
			base.SubscribeLocalEvent<ArtifactGasTriggerComponent, ArtifactNodeEnteredEvent>(new ComponentEventHandler<ArtifactGasTriggerComponent, ArtifactNodeEnteredEvent>(this.OnRandomizeTrigger), null, null);
		}

		// Token: 0x06000092 RID: 146 RVA: 0x00005058 File Offset: 0x00003258
		private void OnRandomizeTrigger(EntityUid uid, ArtifactGasTriggerComponent component, ArtifactNodeEnteredEvent args)
		{
			if (component.ActivationGas != null)
			{
				return;
			}
			Gas gas = component.PossibleGases[args.RandomSeed % component.PossibleGases.Count];
			component.ActivationGas = new Gas?(gas);
		}

		// Token: 0x06000093 RID: 147 RVA: 0x000050A0 File Offset: 0x000032A0
		public override void Update(float frameTime)
		{
			base.Update(frameTime);
			List<ArtifactComponent> toUpdate = new List<ArtifactComponent>();
			foreach (ValueTuple<ArtifactGasTriggerComponent, ArtifactComponent, TransformComponent> valueTuple in base.EntityQuery<ArtifactGasTriggerComponent, ArtifactComponent, TransformComponent>(false))
			{
				ArtifactGasTriggerComponent trigger = valueTuple.Item1;
				ArtifactComponent artifact = valueTuple.Item2;
				TransformComponent transform = valueTuple.Item3;
				EntityUid uid = trigger.Owner;
				if (trigger.ActivationGas != null)
				{
					GasMixture environment = this._atmosphereSystem.GetTileMixture(transform.GridUid, transform.MapUid, this._transformSystem.GetGridOrMapTilePosition(uid, transform), false);
					if (environment != null && environment.GetMoles(trigger.ActivationGas.Value) >= trigger.ActivationMoles)
					{
						toUpdate.Add(artifact);
					}
				}
			}
			foreach (ArtifactComponent a in toUpdate)
			{
				this._artifactSystem.TryActivateArtifact(a.Owner, null, a);
			}
		}

		// Token: 0x04000067 RID: 103
		[Dependency]
		private readonly AtmosphereSystem _atmosphereSystem;

		// Token: 0x04000068 RID: 104
		[Dependency]
		private readonly ArtifactSystem _artifactSystem;

		// Token: 0x04000069 RID: 105
		[Dependency]
		private readonly TransformSystem _transformSystem;
	}
}
