using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading;
using Content.Server.NPC.Pathfinding;
using Robust.Shared.GameObjects;
using Robust.Shared.Map;
using Robust.Shared.Maths;
using Robust.Shared.ViewVariables;

namespace Content.Server.NPC.Components
{
	// Token: 0x02000372 RID: 882
	[NullableContext(1)]
	[Nullable(0)]
	[RegisterComponent]
	public sealed class NPCSteeringComponent : Component
	{
		// Token: 0x17000281 RID: 641
		// (get) Token: 0x06001210 RID: 4624 RVA: 0x0005EAF0 File Offset: 0x0005CCF0
		[ViewVariables]
		public bool Pathfind
		{
			get
			{
				return this.PathfindToken != null;
			}
		}

		// Token: 0x04000B1A RID: 2842
		[ViewVariables]
		public bool CanSeek = true;

		// Token: 0x04000B1B RID: 2843
		[ViewVariables]
		public float Radius = 0.35f;

		// Token: 0x04000B1C RID: 2844
		[ViewVariables]
		public readonly float[] Interest = new float[12];

		// Token: 0x04000B1D RID: 2845
		[ViewVariables]
		public readonly float[] Danger = new float[12];

		// Token: 0x04000B1E RID: 2846
		public readonly List<Vector2> DangerPoints = new List<Vector2>();

		// Token: 0x04000B1F RID: 2847
		public TimeSpan NextSteer = TimeSpan.Zero;

		// Token: 0x04000B20 RID: 2848
		public Vector2 LastSteerDirection = Vector2.Zero;

		// Token: 0x04000B21 RID: 2849
		public const int SteeringFrequency = 10;

		// Token: 0x04000B22 RID: 2850
		[Nullable(2)]
		[ViewVariables]
		public CancellationTokenSource PathfindToken;

		// Token: 0x04000B23 RID: 2851
		[ViewVariables]
		public Queue<PathPoly> CurrentPath = new Queue<PathPoly>();

		// Token: 0x04000B24 RID: 2852
		[ViewVariables]
		public EntityCoordinates Coordinates;

		// Token: 0x04000B25 RID: 2853
		[ViewVariables]
		public float Range = 0.2f;

		// Token: 0x04000B26 RID: 2854
		[ViewVariables]
		public float RepathRange = 1.5f;

		// Token: 0x04000B27 RID: 2855
		public const int FailedPathLimit = 3;

		// Token: 0x04000B28 RID: 2856
		[ViewVariables]
		public int FailedPathCount;

		// Token: 0x04000B29 RID: 2857
		[ViewVariables]
		public SteeringStatus Status = SteeringStatus.Moving;

		// Token: 0x04000B2A RID: 2858
		[ViewVariables]
		public PathFlags Flags;
	}
}
