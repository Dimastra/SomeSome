using System;
using System.Runtime.CompilerServices;
using Content.Shared.Spawners.Components;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Map;
using Robust.Shared.Map.Components;
using Robust.Shared.Maths;
using Robust.Shared.Physics.Components;
using Robust.Shared.Physics.Systems;

namespace Content.Server.StationEvents.Events
{
	// Token: 0x0200018A RID: 394
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class MeteorSwarm : StationEventSystem
	{
		// Token: 0x17000157 RID: 343
		// (get) Token: 0x060007C7 RID: 1991 RVA: 0x00026908 File Offset: 0x00024B08
		public override string Prototype
		{
			get
			{
				return "MeteorSwarm";
			}
		}

		// Token: 0x060007C8 RID: 1992 RVA: 0x00026910 File Offset: 0x00024B10
		public override void Started()
		{
			base.Started();
			double mod = Math.Sqrt((double)base.GetSeverityModifier());
			this._waveCounter = (int)((double)this.RobustRandom.Next(3, 8) * mod);
		}

		// Token: 0x060007C9 RID: 1993 RVA: 0x00026947 File Offset: 0x00024B47
		public override void Ended()
		{
			base.Ended();
			this._waveCounter = 0;
			this._cooldown = 0f;
		}

		// Token: 0x060007CA RID: 1994 RVA: 0x00026964 File Offset: 0x00024B64
		public override void Update(float frameTime)
		{
			base.Update(frameTime);
			if (!base.RuleStarted)
			{
				return;
			}
			if (this._waveCounter <= 0)
			{
				base.ForceEndSelf();
				return;
			}
			float mod = base.GetSeverityModifier();
			this._cooldown -= frameTime;
			if (this._cooldown > 0f)
			{
				return;
			}
			this._waveCounter--;
			this._cooldown += 50f * this.RobustRandom.NextFloat() / mod + 10f;
			Box2? playableArea = null;
			MapId mapId = this.GameTicker.DefaultMap;
			foreach (MapGridComponent grid in this.MapManager.GetAllMapGrids(mapId))
			{
				Box2 aabb = this._physics.GetWorldAABB(grid.Owner, null, null, null);
				playableArea = new Box2?((playableArea != null) ? playableArea.GetValueOrDefault().Union(ref aabb) : aabb);
			}
			if (playableArea == null)
			{
				base.ForceEndSelf();
				return;
			}
			Vector2 vector = playableArea.Value.TopRight - playableArea.Value.Center;
			float minimumDistance = vector.Length + 50f;
			float maximumDistance = minimumDistance + 100f;
			Vector2 center = playableArea.Value.Center;
			for (int i = 0; i < 5; i++)
			{
				Angle angle;
				angle..ctor((double)(this.RobustRandom.NextFloat() * 6.2831855f));
				vector = new Vector2((maximumDistance - minimumDistance) * this.RobustRandom.NextFloat() + minimumDistance, 0f);
				Vector2 offset = angle.RotateVec(ref vector);
				MapCoordinates spawnPosition;
				spawnPosition..ctor(center + offset, mapId);
				EntityUid meteor = this.EntityManager.SpawnEntity("MeteorLarge", spawnPosition);
				PhysicsComponent physics = this.EntityManager.GetComponent<PhysicsComponent>(meteor);
				this._physics.SetBodyStatus(physics, 1, true);
				this._physics.SetLinearDamping(physics, 0f, true);
				this._physics.SetAngularDamping(physics, 0f, true);
				this._physics.ApplyLinearImpulse(meteor, -offset.Normalized * 10f * physics.Mass, null, physics);
				this._physics.ApplyAngularImpulse(meteor, physics.Mass * (0.5f * this.RobustRandom.NextFloat() + -0.25f), null, physics);
				base.EnsureComp<TimedDespawnComponent>(meteor).Lifetime = 120f;
			}
		}

		// Token: 0x040004BE RID: 1214
		[Dependency]
		private readonly SharedPhysicsSystem _physics;

		// Token: 0x040004BF RID: 1215
		private float _cooldown;

		// Token: 0x040004C0 RID: 1216
		private int _waveCounter;

		// Token: 0x040004C1 RID: 1217
		private const int MinimumWaves = 3;

		// Token: 0x040004C2 RID: 1218
		private const int MaximumWaves = 8;

		// Token: 0x040004C3 RID: 1219
		private const float MinimumCooldown = 10f;

		// Token: 0x040004C4 RID: 1220
		private const float MaximumCooldown = 60f;

		// Token: 0x040004C5 RID: 1221
		private const int MeteorsPerWave = 5;

		// Token: 0x040004C6 RID: 1222
		private const float MeteorVelocity = 10f;

		// Token: 0x040004C7 RID: 1223
		private const float MaxAngularVelocity = 0.25f;

		// Token: 0x040004C8 RID: 1224
		private const float MinAngularVelocity = -0.25f;
	}
}
