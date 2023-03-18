using System;
using System.Runtime.CompilerServices;
using Robust.Client.Graphics;
using Robust.Client.ResourceManagement;
using Robust.Shared.Enums;
using Robust.Shared.GameObjects;
using Robust.Shared.Maths;

namespace Content.Client.NPC.HTN
{
	// Token: 0x02000214 RID: 532
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class HTNOverlay : Overlay
	{
		// Token: 0x170002F6 RID: 758
		// (get) Token: 0x06000DE4 RID: 3556 RVA: 0x00026117 File Offset: 0x00024317
		public override OverlaySpace Space
		{
			get
			{
				return 2;
			}
		}

		// Token: 0x06000DE5 RID: 3557 RVA: 0x00054718 File Offset: 0x00052918
		public HTNOverlay(IEntityManager entManager, IResourceCache resourceCache)
		{
			this._entManager = entManager;
			this._font = new VectorFont(resourceCache.GetResource<FontResource>("/Fonts/NotoSans/NotoSans-Regular.ttf", true), 10);
		}

		// Token: 0x06000DE6 RID: 3558 RVA: 0x00054740 File Offset: 0x00052940
		protected override void Draw(in OverlayDrawArgs args)
		{
			if (args.ViewportControl == null)
			{
				return;
			}
			DrawingHandleScreen screenHandle = args.ScreenHandle;
			foreach (ValueTuple<HTNComponent, TransformComponent> valueTuple in this._entManager.EntityQuery<HTNComponent, TransformComponent>(true))
			{
				HTNComponent item = valueTuple.Item1;
				TransformComponent item2 = valueTuple.Item2;
				if (!string.IsNullOrEmpty(item.DebugText) && !(item2.MapID != args.MapId))
				{
					Vector2 worldPosition = item2.WorldPosition;
					if (args.WorldAABB.Contains(worldPosition, true))
					{
						Vector2 vector = args.ViewportControl.WorldToScreen(worldPosition);
						screenHandle.DrawString(this._font, vector + new Vector2(0f, 10f), item.DebugText);
					}
				}
			}
		}

		// Token: 0x040006D9 RID: 1753
		private readonly IEntityManager _entManager;

		// Token: 0x040006DA RID: 1754
		private readonly Font _font;
	}
}
