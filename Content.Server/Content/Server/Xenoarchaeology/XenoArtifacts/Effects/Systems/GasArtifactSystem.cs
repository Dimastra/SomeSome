using System;
using System.Runtime.CompilerServices;
using Content.Server.Atmos;
using Content.Server.Atmos.EntitySystems;
using Content.Server.Xenoarchaeology.XenoArtifacts.Effects.Components;
using Content.Server.Xenoarchaeology.XenoArtifacts.Events;
using Content.Shared.Atmos;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;

namespace Content.Server.Xenoarchaeology.XenoArtifacts.Effects.Systems
{
	// Token: 0x02000044 RID: 68
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class GasArtifactSystem : EntitySystem
	{
		// Token: 0x060000D3 RID: 211 RVA: 0x00006149 File Offset: 0x00004349
		public override void Initialize()
		{
			base.Initialize();
			base.SubscribeLocalEvent<GasArtifactComponent, ArtifactNodeEnteredEvent>(new ComponentEventHandler<GasArtifactComponent, ArtifactNodeEnteredEvent>(this.OnNodeEntered), null, null);
			base.SubscribeLocalEvent<GasArtifactComponent, ArtifactActivatedEvent>(new ComponentEventHandler<GasArtifactComponent, ArtifactActivatedEvent>(this.OnActivate), null, null);
		}

		// Token: 0x060000D4 RID: 212 RVA: 0x0000617C File Offset: 0x0000437C
		private void OnNodeEntered(EntityUid uid, GasArtifactComponent component, ArtifactNodeEnteredEvent args)
		{
			if (component.SpawnGas == null && component.PossibleGases.Count != 0)
			{
				Gas gas = component.PossibleGases[args.RandomSeed % component.PossibleGases.Count];
				component.SpawnGas = new Gas?(gas);
			}
			if (component.SpawnTemperature == null)
			{
				float temp = (float)args.RandomSeed % component.MaxRandomTemperature - component.MinRandomTemperature + component.MinRandomTemperature;
				component.SpawnTemperature = new float?(temp);
			}
		}

		// Token: 0x060000D5 RID: 213 RVA: 0x00006204 File Offset: 0x00004404
		private void OnActivate(EntityUid uid, GasArtifactComponent component, ArtifactActivatedEvent args)
		{
			if (component.SpawnGas == null || component.SpawnTemperature == null)
			{
				return;
			}
			GasMixture environment = this._atmosphereSystem.GetContainingMixture(uid, false, true, null);
			if (environment == null)
			{
				return;
			}
			if (environment.Pressure >= component.MaxExternalPressure)
			{
				return;
			}
			GasMixture merger = new GasMixture(1f)
			{
				Temperature = component.SpawnTemperature.Value
			};
			merger.SetMoles(component.SpawnGas.Value, component.SpawnAmount);
			this._atmosphereSystem.Merge(environment, merger);
		}

		// Token: 0x0400009A RID: 154
		[Dependency]
		private readonly AtmosphereSystem _atmosphereSystem;
	}
}
