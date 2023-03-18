using System;
using System.Runtime.CompilerServices;
using Content.Server.Atmos;
using Content.Server.Atmos.EntitySystems;
using Content.Server.GameTicking.Rules.Configurations;
using Content.Shared.Atmos;
using Robust.Shared.Audio;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Log;
using Robust.Shared.Map;
using Robust.Shared.Maths;
using Robust.Shared.Player;
using Robust.Shared.Random;

namespace Content.Server.StationEvents.Events
{
	// Token: 0x02000188 RID: 392
	[NullableContext(1)]
	[Nullable(0)]
	internal sealed class GasLeak : StationEventSystem
	{
		// Token: 0x17000155 RID: 341
		// (get) Token: 0x060007BD RID: 1981 RVA: 0x000264EA File Offset: 0x000246EA
		public override string Prototype
		{
			get
			{
				return "GasLeak";
			}
		}

		// Token: 0x060007BE RID: 1982 RVA: 0x000264F4 File Offset: 0x000246F4
		public override void Started()
		{
			base.Started();
			float mod = MathF.Sqrt(base.GetSeverityModifier());
			if (base.TryFindRandomTile(out this._targetTile, out this._targetStation, out this._targetGrid, out this._targetCoords))
			{
				this._foundTile = true;
				this._leakGas = RandomExtensions.Pick<Gas>(this.RobustRandom, GasLeak.LeakableGases);
				float totalGas = (float)this.RobustRandom.Next(250, 1000) * mod;
				float startAfter = ((StationEventRuleConfiguration)this.Configuration).StartAfter;
				this._molesPerSecond = (float)this.RobustRandom.Next(20, 50);
				this._endAfter = totalGas / this._molesPerSecond + startAfter;
				ISawmill sawmill = this.Sawmill;
				DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(30, 4);
				defaultInterpolatedStringHandler.AppendLiteral("Leaking ");
				defaultInterpolatedStringHandler.AppendFormatted<float>(totalGas);
				defaultInterpolatedStringHandler.AppendLiteral(" of ");
				defaultInterpolatedStringHandler.AppendFormatted<Gas>(this._leakGas);
				defaultInterpolatedStringHandler.AppendLiteral(" over ");
				defaultInterpolatedStringHandler.AppendFormatted<float>(this._endAfter - startAfter);
				defaultInterpolatedStringHandler.AppendLiteral(" seconds at ");
				defaultInterpolatedStringHandler.AppendFormatted<Vector2i>(this._targetTile);
				sawmill.Info(defaultInterpolatedStringHandler.ToStringAndClear());
			}
		}

		// Token: 0x060007BF RID: 1983 RVA: 0x00026620 File Offset: 0x00024820
		public override void Update(float frameTime)
		{
			base.Update(frameTime);
			if (!base.RuleStarted)
			{
				return;
			}
			if (base.Elapsed > this._endAfter)
			{
				base.ForceEndSelf();
				return;
			}
			this._timeUntilLeak -= frameTime;
			if (this._timeUntilLeak > 0f)
			{
				return;
			}
			this._timeUntilLeak += 1f;
			if (!this._foundTile || this._targetGrid == default(EntityUid) || this.EntityManager.Deleted(this._targetGrid) || !this._atmosphere.IsSimulatedGrid(this._targetGrid))
			{
				base.ForceEndSelf();
				return;
			}
			GasMixture tileMixture = this._atmosphere.GetTileMixture(new EntityUid?(this._targetGrid), null, this._targetTile, true);
			if (tileMixture == null)
			{
				return;
			}
			tileMixture.AdjustMoles(this._leakGas, 1f * this._molesPerSecond);
		}

		// Token: 0x060007C0 RID: 1984 RVA: 0x00026710 File Offset: 0x00024910
		public override void Ended()
		{
			base.Ended();
			this.Spark();
			this._foundTile = false;
			this._targetGrid = default(EntityUid);
			this._targetTile = default(Vector2i);
			this._targetCoords = default(EntityCoordinates);
			this._leakGas = Gas.Oxygen;
			this._endAfter = float.MaxValue;
		}

		// Token: 0x060007C1 RID: 1985 RVA: 0x00026768 File Offset: 0x00024968
		private void Spark()
		{
			if (this.RobustRandom.NextFloat() <= 0.05f)
			{
				if (!this._foundTile || this._targetGrid == default(EntityUid) || ((!this.EntityManager.EntityExists(this._targetGrid)) ? 5 : this.EntityManager.GetComponent<MetaDataComponent>(this._targetGrid).EntityLifeStage) >= 5 || !this._atmosphere.IsSimulatedGrid(this._targetGrid))
				{
					return;
				}
				this._atmosphere.HotspotExpose(this._targetGrid, this._targetTile, 700f, 50f, true);
				SoundSystem.Play("/Audio/Effects/sparks4.ogg", Filter.Pvs(this._targetCoords, 2f, null, null), this._targetCoords, null);
			}
		}

		// Token: 0x060007C3 RID: 1987 RVA: 0x0002684C File Offset: 0x00024A4C
		// Note: this type is marked as 'beforefieldinit'.
		static GasLeak()
		{
			Gas[] array = new Gas[4];
			RuntimeHelpers.InitializeArray(array, fieldof(<PrivateImplementationDetails>.D7BEDBE6BF50CF16A270A6F1C4C1C9496E12B02C887976D8F2870EAFB1C5811A).FieldHandle);
			GasLeak.LeakableGases = array;
		}

		// Token: 0x040004AA RID: 1194
		[Dependency]
		private readonly AtmosphereSystem _atmosphere;

		// Token: 0x040004AB RID: 1195
		private static readonly Gas[] LeakableGases;

		// Token: 0x040004AC RID: 1196
		private float _timeUntilLeak;

		// Token: 0x040004AD RID: 1197
		private const float LeakCooldown = 1f;

		// Token: 0x040004AE RID: 1198
		private EntityUid _targetStation;

		// Token: 0x040004AF RID: 1199
		private EntityUid _targetGrid;

		// Token: 0x040004B0 RID: 1200
		private Vector2i _targetTile;

		// Token: 0x040004B1 RID: 1201
		private EntityCoordinates _targetCoords;

		// Token: 0x040004B2 RID: 1202
		private bool _foundTile;

		// Token: 0x040004B3 RID: 1203
		private Gas _leakGas;

		// Token: 0x040004B4 RID: 1204
		private float _molesPerSecond;

		// Token: 0x040004B5 RID: 1205
		private const int MinimumMolesPerSecond = 20;

		// Token: 0x040004B6 RID: 1206
		private float _endAfter = float.MaxValue;

		// Token: 0x040004B7 RID: 1207
		private const int MaximumMolesPerSecond = 50;

		// Token: 0x040004B8 RID: 1208
		private const int MinimumGas = 250;

		// Token: 0x040004B9 RID: 1209
		private const int MaximumGas = 1000;

		// Token: 0x040004BA RID: 1210
		private const float SparkChance = 0.05f;
	}
}
