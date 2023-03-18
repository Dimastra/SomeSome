using System;
using System.Runtime.CompilerServices;
using Content.Shared.Decals;
using Robust.Client.GameObjects;
using Robust.Client.Graphics;
using Robust.Client.Input;
using Robust.Shared.Enums;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Map;
using Robust.Shared.Map.Components;
using Robust.Shared.Maths;

namespace Content.Client.Decals.Overlays
{
	// Token: 0x02000360 RID: 864
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class DecalPlacementOverlay : Overlay
	{
		// Token: 0x17000457 RID: 1111
		// (get) Token: 0x0600156C RID: 5484 RVA: 0x0000689B File Offset: 0x00004A9B
		public override OverlaySpace Space
		{
			get
			{
				return 4;
			}
		}

		// Token: 0x0600156D RID: 5485 RVA: 0x0007E240 File Offset: 0x0007C440
		public DecalPlacementOverlay(DecalPlacementSystem placement, SharedTransformSystem transform, SpriteSystem sprite)
		{
			IoCManager.InjectDependencies<DecalPlacementOverlay>(this);
			this._placement = placement;
			this._transform = transform;
			this._sprite = sprite;
		}

		// Token: 0x0600156E RID: 5486 RVA: 0x0007E264 File Offset: 0x0007C464
		protected override void Draw(in OverlayDrawArgs args)
		{
			ValueTuple<DecalPrototype, bool, Angle, Color> activeDecal = this._placement.GetActiveDecal();
			DecalPrototype item = activeDecal.Item1;
			bool item2 = activeDecal.Item2;
			Angle item3 = activeDecal.Item3;
			Color item4 = activeDecal.Item4;
			if (item == null)
			{
				return;
			}
			ScreenCoordinates mouseScreenPosition = this._inputManager.MouseScreenPosition;
			MapCoordinates mapCoordinates = this._eyeManager.ScreenToMap(mouseScreenPosition);
			if (mapCoordinates.MapId != args.MapId)
			{
				return;
			}
			MapGridComponent mapGridComponent;
			if (!this._mapManager.TryFindGridAt(mapCoordinates, ref mapGridComponent))
			{
				return;
			}
			Matrix3 worldMatrix = this._transform.GetWorldMatrix(mapGridComponent.Owner);
			Matrix3 invWorldMatrix = this._transform.GetInvWorldMatrix(mapGridComponent.Owner);
			DrawingHandleWorld worldHandle = args.WorldHandle;
			worldHandle.SetTransform(ref worldMatrix);
			Vector2 vector = invWorldMatrix.Transform(mapCoordinates.Position);
			if (item2)
			{
				vector = vector.Floored() + (float)mapGridComponent.TileSize / 2f;
			}
			Box2 box = Box2.UnitCentered.Translated(vector);
			Box2Rotated box2Rotated;
			box2Rotated..ctor(box, item3, vector);
			worldHandle.DrawTextureRect(this._sprite.Frame0(item.Sprite), ref box2Rotated, new Color?(item4));
			worldHandle.SetTransform(ref Matrix3.Identity);
		}

		// Token: 0x04000B24 RID: 2852
		[Dependency]
		private readonly IEyeManager _eyeManager;

		// Token: 0x04000B25 RID: 2853
		[Dependency]
		private readonly IInputManager _inputManager;

		// Token: 0x04000B26 RID: 2854
		[Dependency]
		private readonly IMapManager _mapManager;

		// Token: 0x04000B27 RID: 2855
		private readonly DecalPlacementSystem _placement;

		// Token: 0x04000B28 RID: 2856
		private readonly SharedTransformSystem _transform;

		// Token: 0x04000B29 RID: 2857
		private readonly SpriteSystem _sprite;
	}
}
