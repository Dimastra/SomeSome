using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Content.Shared.Decals;
using Robust.Client.GameObjects;
using Robust.Client.Graphics;
using Robust.Shared.Enums;
using Robust.Shared.GameObjects;
using Robust.Shared.Maths;
using Robust.Shared.Prototypes;

namespace Content.Client.Decals.Overlays
{
	// Token: 0x0200035F RID: 863
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class DecalOverlay : Overlay
	{
		// Token: 0x17000456 RID: 1110
		// (get) Token: 0x06001569 RID: 5481 RVA: 0x0007DF99 File Offset: 0x0007C199
		public override OverlaySpace Space
		{
			get
			{
				return 32;
			}
		}

		// Token: 0x0600156A RID: 5482 RVA: 0x0007DF9D File Offset: 0x0007C19D
		public DecalOverlay(SpriteSystem sprites, IEntityManager entManager, IPrototypeManager prototypeManager)
		{
			this._sprites = sprites;
			this._entManager = entManager;
			this._prototypeManager = prototypeManager;
		}

		// Token: 0x0600156B RID: 5483 RVA: 0x0007DFC8 File Offset: 0x0007C1C8
		protected override void Draw(in OverlayDrawArgs args)
		{
			DrawingHandleWorld worldHandle = args.WorldHandle;
			EntityQuery<TransformComponent> entityQuery = this._entManager.GetEntityQuery<TransformComponent>();
			IEye eye = args.Viewport.Eye;
			Angle angle = (eye != null) ? eye.Rotation : Angle.Zero;
			foreach (ValueTuple<DecalGridComponent, TransformComponent> valueTuple in this._entManager.EntityQuery<DecalGridComponent, TransformComponent>(true))
			{
				DecalGridComponent item = valueTuple.Item1;
				TransformComponent item2 = valueTuple.Item2;
				EntityUid owner = item.Owner;
				SortedDictionary<int, SortedDictionary<uint, Decal>> decalRenderIndex = item.DecalRenderIndex;
				if (decalRenderIndex.Count != 0 && !(item2.MapID != args.MapId))
				{
					ValueTuple<Vector2, Angle, Matrix3> worldPositionRotationMatrix = item2.GetWorldPositionRotationMatrix(entityQuery);
					Angle item3 = worldPositionRotationMatrix.Item2;
					Matrix3 item4 = worldPositionRotationMatrix.Item3;
					worldHandle.SetTransform(ref item4);
					foreach (SortedDictionary<uint, Decal> sortedDictionary in decalRenderIndex.Values)
					{
						foreach (Decal decal in sortedDictionary.Values)
						{
							ValueTuple<Texture, bool> valueTuple2;
							DecalPrototype decalPrototype;
							if (!this._cachedTextures.TryGetValue(decal.Id, out valueTuple2) && this._prototypeManager.TryIndex<DecalPrototype>(decal.Id, ref decalPrototype))
							{
								valueTuple2 = new ValueTuple<Texture, bool>(this._sprites.Frame0(decalPrototype.Sprite), decalPrototype.SnapCardinals);
								this._cachedTextures[decal.Id] = valueTuple2;
							}
							Angle angle2 = Angle.Zero;
							if (valueTuple2.Item2)
							{
								angle2 = DirectionExtensions.ToAngle((angle + item3).GetCardinalDir());
							}
							Angle angle3 = decal.Angle - angle2;
							if (angle3.Equals(Angle.Zero))
							{
								worldHandle.DrawTexture(valueTuple2.Item1, decal.Coordinates, decal.Color);
							}
							else
							{
								worldHandle.DrawTexture(valueTuple2.Item1, decal.Coordinates, angle3, decal.Color);
							}
						}
					}
				}
			}
			worldHandle.SetTransform(ref Matrix3.Identity);
		}

		// Token: 0x04000B20 RID: 2848
		private readonly SpriteSystem _sprites;

		// Token: 0x04000B21 RID: 2849
		private readonly IEntityManager _entManager;

		// Token: 0x04000B22 RID: 2850
		private readonly IPrototypeManager _prototypeManager;

		// Token: 0x04000B23 RID: 2851
		[TupleElementNames(new string[]
		{
			"Texture",
			"SnapCardinals"
		})]
		[Nullable(new byte[]
		{
			1,
			1,
			0,
			1
		})]
		private readonly Dictionary<string, ValueTuple<Texture, bool>> _cachedTextures = new Dictionary<string, ValueTuple<Texture, bool>>(64);
	}
}
