using System;
using System.Runtime.CompilerServices;
using Content.Shared.Movement.Systems;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Map;
using Robust.Shared.Maths;
using Robust.Shared.Timing;
using Robust.Shared.ViewVariables;

namespace Content.Shared.Shuttles.Components
{
	// Token: 0x020001CB RID: 459
	[NullableContext(2)]
	[Nullable(0)]
	[RegisterComponent]
	[NetworkedComponent]
	public sealed class PilotComponent : Component
	{
		// Token: 0x170000EB RID: 235
		// (get) Token: 0x06000539 RID: 1337 RVA: 0x000139E9 File Offset: 0x00011BE9
		// (set) Token: 0x0600053A RID: 1338 RVA: 0x000139F1 File Offset: 0x00011BF1
		[ViewVariables]
		public SharedShuttleConsoleComponent Console { get; set; }

		// Token: 0x170000EC RID: 236
		// (get) Token: 0x0600053B RID: 1339 RVA: 0x000139FA File Offset: 0x00011BFA
		// (set) Token: 0x0600053C RID: 1340 RVA: 0x00013A02 File Offset: 0x00011C02
		[ViewVariables]
		public EntityCoordinates? Position { get; set; }

		// Token: 0x170000ED RID: 237
		// (get) Token: 0x0600053D RID: 1341 RVA: 0x00013A0B File Offset: 0x00011C0B
		public override bool SendOnlyToOwner
		{
			get
			{
				return true;
			}
		}

		// Token: 0x0400052E RID: 1326
		public const float BreakDistance = 0.25f;

		// Token: 0x0400052F RID: 1327
		public Vector2 CurTickStrafeMovement = Vector2.Zero;

		// Token: 0x04000530 RID: 1328
		public float CurTickRotationMovement;

		// Token: 0x04000531 RID: 1329
		public float CurTickBraking;

		// Token: 0x04000532 RID: 1330
		public GameTick LastInputTick = GameTick.Zero;

		// Token: 0x04000533 RID: 1331
		public ushort LastInputSubTick;

		// Token: 0x04000534 RID: 1332
		[ViewVariables]
		public ShuttleButtons HeldButtons;
	}
}
