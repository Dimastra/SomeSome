using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using Content.Server.Atmos.EntitySystems;
using Content.Server.Atmos.Piping.Components;
using Content.Server.Atmos.Piping.Other.Components;
using Robust.Server.GameObjects;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Maths;

namespace Content.Server.Atmos.Piping.Other.EntitySystems
{
	// Token: 0x0200075C RID: 1884
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class GasMinerSystem : EntitySystem
	{
		// Token: 0x060027E0 RID: 10208 RVA: 0x000D1469 File Offset: 0x000CF669
		public override void Initialize()
		{
			base.Initialize();
			base.SubscribeLocalEvent<GasMinerComponent, AtmosDeviceUpdateEvent>(new ComponentEventHandler<GasMinerComponent, AtmosDeviceUpdateEvent>(this.OnMinerUpdated), null, null);
		}

		// Token: 0x060027E1 RID: 10209 RVA: 0x000D1488 File Offset: 0x000CF688
		private void OnMinerUpdated(EntityUid uid, GasMinerComponent miner, AtmosDeviceUpdateEvent args)
		{
			GasMixture environment;
			if (!this.CheckMinerOperation(miner, out environment) || !miner.Enabled || miner.SpawnGas == null || miner.SpawnAmount <= 0f)
			{
				return;
			}
			GasMixture merger = new GasMixture(1f)
			{
				Temperature = miner.SpawnTemperature
			};
			merger.SetMoles(miner.SpawnGas.Value, miner.SpawnAmount);
			this._atmosphereSystem.Merge(environment, merger);
		}

		// Token: 0x060027E2 RID: 10210 RVA: 0x000D1504 File Offset: 0x000CF704
		private bool CheckMinerOperation(GasMinerComponent miner, [Nullable(2)] [NotNullWhen(true)] out GasMixture environment)
		{
			EntityUid uid = miner.Owner;
			environment = this._atmosphereSystem.GetContainingMixture(uid, true, true, null);
			TransformComponent transform = base.Transform(uid);
			Vector2i position = this._transformSystem.GetGridOrMapTilePosition(uid, transform);
			if (this._atmosphereSystem.IsTileSpace(transform.GridUid, transform.MapUid, position, null))
			{
				miner.Broken = true;
				return false;
			}
			if (environment == null)
			{
				miner.Broken = true;
				return false;
			}
			if (!float.IsInfinity(miner.MaxExternalPressure) && environment.Pressure > miner.MaxExternalPressure - miner.SpawnAmount * miner.SpawnTemperature * 8.314463f / environment.Volume)
			{
				miner.Broken = true;
				return false;
			}
			if (!float.IsInfinity(miner.MaxExternalAmount) && environment.TotalMoles > miner.MaxExternalAmount)
			{
				miner.Broken = true;
				return false;
			}
			miner.Broken = false;
			return true;
		}

		// Token: 0x040018DB RID: 6363
		[Dependency]
		private readonly AtmosphereSystem _atmosphereSystem;

		// Token: 0x040018DC RID: 6364
		[Dependency]
		private readonly TransformSystem _transformSystem;
	}
}
