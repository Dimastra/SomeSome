using System;
using System.Runtime.CompilerServices;
using Robust.Shared.GameObjects;
using Robust.Shared.Log;
using Robust.Shared.Map;
using Robust.Shared.Maths;

namespace Content.Server.StationEvents.Events
{
	// Token: 0x0200018E RID: 398
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class RevenantSpawn : StationEventSystem
	{
		// Token: 0x1700015C RID: 348
		// (get) Token: 0x060007DB RID: 2011 RVA: 0x00027420 File Offset: 0x00025620
		public override string Prototype
		{
			get
			{
				return "RevenantSpawn";
			}
		}

		// Token: 0x060007DC RID: 2012 RVA: 0x00027428 File Offset: 0x00025628
		public override void Started()
		{
			base.Started();
			Vector2i vector2i;
			EntityUid entityUid;
			EntityUid entityUid2;
			EntityCoordinates coords;
			if (base.TryFindRandomTile(out vector2i, out entityUid, out entityUid2, out coords))
			{
				ISawmill sawmill = this.Sawmill;
				DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(21, 1);
				defaultInterpolatedStringHandler.AppendLiteral("Spawning revenant at ");
				defaultInterpolatedStringHandler.AppendFormatted<EntityCoordinates>(coords);
				sawmill.Info(defaultInterpolatedStringHandler.ToStringAndClear());
				this.EntityManager.SpawnEntity(RevenantSpawn.RevenantPrototype, coords);
			}
		}

		// Token: 0x040004D3 RID: 1235
		private static readonly string RevenantPrototype = "MobRevenant";
	}
}
