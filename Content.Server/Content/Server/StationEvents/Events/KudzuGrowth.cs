using System;
using System.Runtime.CompilerServices;
using Robust.Shared.GameObjects;
using Robust.Shared.Log;
using Robust.Shared.Map;
using Robust.Shared.Maths;

namespace Content.Server.StationEvents.Events
{
	// Token: 0x02000189 RID: 393
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class KudzuGrowth : StationEventSystem
	{
		// Token: 0x17000156 RID: 342
		// (get) Token: 0x060007C4 RID: 1988 RVA: 0x00026864 File Offset: 0x00024A64
		public override string Prototype
		{
			get
			{
				return "KudzuGrowth";
			}
		}

		// Token: 0x060007C5 RID: 1989 RVA: 0x0002686C File Offset: 0x00024A6C
		public override void Started()
		{
			base.Started();
			EntityUid entityUid;
			if (base.TryFindRandomTile(out this._targetTile, out entityUid, out this._targetGrid, out this._targetCoords))
			{
				this.EntityManager.SpawnEntity("Kudzu", this._targetCoords);
				ISawmill sawmill = this.Sawmill;
				DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(24, 2);
				defaultInterpolatedStringHandler.AppendLiteral("Spawning a Kudzu at ");
				defaultInterpolatedStringHandler.AppendFormatted<Vector2i>(this._targetTile);
				defaultInterpolatedStringHandler.AppendLiteral(" on ");
				defaultInterpolatedStringHandler.AppendFormatted<EntityUid>(this._targetGrid);
				sawmill.Info(defaultInterpolatedStringHandler.ToStringAndClear());
			}
		}

		// Token: 0x040004BB RID: 1211
		private EntityUid _targetGrid;

		// Token: 0x040004BC RID: 1212
		private Vector2i _targetTile;

		// Token: 0x040004BD RID: 1213
		private EntityCoordinates _targetCoords;
	}
}
