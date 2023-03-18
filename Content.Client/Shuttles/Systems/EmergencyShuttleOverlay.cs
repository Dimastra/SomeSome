using System;
using System.Runtime.CompilerServices;
using Robust.Client.Graphics;
using Robust.Shared.Enums;
using Robust.Shared.GameObjects;
using Robust.Shared.Maths;

namespace Content.Client.Shuttles.Systems
{
	// Token: 0x02000156 RID: 342
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class EmergencyShuttleOverlay : Overlay
	{
		// Token: 0x170001AD RID: 429
		// (get) Token: 0x0600090A RID: 2314 RVA: 0x0000689B File Offset: 0x00004A9B
		public override OverlaySpace Space
		{
			get
			{
				return 4;
			}
		}

		// Token: 0x0600090B RID: 2315 RVA: 0x0003582B File Offset: 0x00033A2B
		public EmergencyShuttleOverlay(IEntityManager entManager)
		{
			this._entManager = entManager;
		}

		// Token: 0x0600090C RID: 2316 RVA: 0x0003583C File Offset: 0x00033A3C
		protected override void Draw(in OverlayDrawArgs args)
		{
			TransformComponent transformComponent;
			if (this.Position == null || !this._entManager.TryGetComponent<TransformComponent>(this.StationUid, ref transformComponent))
			{
				return;
			}
			DrawingHandleBase worldHandle = args.WorldHandle;
			Matrix3 worldMatrix = transformComponent.WorldMatrix;
			worldHandle.SetTransform(ref worldMatrix);
			args.WorldHandle.DrawRect(this.Position.Value, Color.Red.WithAlpha(100), true);
			args.WorldHandle.SetTransform(ref Matrix3.Identity);
		}

		// Token: 0x0400048C RID: 1164
		private IEntityManager _entManager;

		// Token: 0x0400048D RID: 1165
		public EntityUid? StationUid;

		// Token: 0x0400048E RID: 1166
		public Box2? Position;
	}
}
