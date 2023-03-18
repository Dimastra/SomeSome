using System;
using System.Runtime.CompilerServices;
using Content.Server.Atmos;
using Content.Server.Atmos.EntitySystems;
using Content.Server.Xenoarchaeology.XenoArtifacts.Effects.Components;
using Content.Server.Xenoarchaeology.XenoArtifacts.Events;
using Robust.Server.GameObjects;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;

namespace Content.Server.Xenoarchaeology.XenoArtifacts.Effects.Systems
{
	// Token: 0x0200004E RID: 78
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class TemperatureArtifactSystem : EntitySystem
	{
		// Token: 0x060000F2 RID: 242 RVA: 0x00006904 File Offset: 0x00004B04
		public override void Initialize()
		{
			base.Initialize();
			base.SubscribeLocalEvent<TemperatureArtifactComponent, ArtifactActivatedEvent>(new ComponentEventHandler<TemperatureArtifactComponent, ArtifactActivatedEvent>(this.OnActivate), null, null);
		}

		// Token: 0x060000F3 RID: 243 RVA: 0x00006920 File Offset: 0x00004B20
		private void OnActivate(EntityUid uid, TemperatureArtifactComponent component, ArtifactActivatedEvent args)
		{
			TransformComponent transform = base.Transform(uid);
			GasMixture center = this._atmosphereSystem.GetContainingMixture(uid, false, true, null);
			if (center == null)
			{
				return;
			}
			this.UpdateTileTemperature(component, center);
			if (component.AffectAdjacentTiles && transform.GridUid != null)
			{
				foreach (GasMixture mixture in this._atmosphereSystem.GetAdjacentTileMixtures(transform.GridUid.Value, this._transformSystem.GetGridOrMapTilePosition(uid, transform), false, true))
				{
					this.UpdateTileTemperature(component, mixture);
				}
			}
		}

		// Token: 0x060000F4 RID: 244 RVA: 0x000069D0 File Offset: 0x00004BD0
		private void UpdateTileTemperature(TemperatureArtifactComponent component, GasMixture environment)
		{
			float dif = component.TargetTemperature - environment.Temperature;
			float absDif = Math.Abs(dif);
			if (absDif < component.MaxTemperatureDifference)
			{
				return;
			}
			float step = Math.Min(absDif, component.SpawnTemperature);
			environment.Temperature += ((dif > 0f) ? step : (-step));
		}

		// Token: 0x040000B0 RID: 176
		[Dependency]
		private readonly AtmosphereSystem _atmosphereSystem;

		// Token: 0x040000B1 RID: 177
		[Dependency]
		private readonly TransformSystem _transformSystem;
	}
}
