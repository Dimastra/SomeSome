using System;
using System.Runtime.CompilerServices;
using Robust.Client.GameObjects;
using Robust.Client.Graphics;
using Robust.Client.Utility;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Maths;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Client.Clickable
{
	// Token: 0x020003BF RID: 959
	[NullableContext(1)]
	[Nullable(0)]
	[RegisterComponent]
	public sealed class ClickableComponent : Component
	{
		// Token: 0x060017D3 RID: 6099 RVA: 0x000890AC File Offset: 0x000872AC
		public bool CheckClick(SpriteComponent sprite, TransformComponent transform, [Nullable(new byte[]
		{
			0,
			1
		})] EntityQuery<TransformComponent> xformQuery, Vector2 worldPos, IEye eye, out int drawDepth, out uint renderOrder, out float bottom)
		{
			if (!sprite.Visible)
			{
				drawDepth = 0;
				renderOrder = 0U;
				bottom = 0f;
				return false;
			}
			drawDepth = sprite.DrawDepth;
			renderOrder = sprite.RenderOrder;
			ValueTuple<Vector2, Angle> worldPositionRotation = transform.GetWorldPositionRotation(xformQuery);
			Vector2 item = worldPositionRotation.Item1;
			Angle item2 = worldPositionRotation.Item2;
			bottom = sprite.CalculateRotatedBoundingBox(item, item2, eye.Rotation).CalcBoundingBox().Bottom;
			Matrix3 localMatrix = sprite.GetLocalMatrix();
			Matrix3 matrix = Matrix3.Invert(ref localMatrix);
			Angle angle = item2 + eye.Rotation;
			angle = angle.Reduced();
			Angle angle2 = angle.FlipPositive();
			Angle angle3 = sprite.SnapCardinals ? DirectionExtensions.ToAngle(angle2.GetCardinalDir()) : Angle.Zero;
			Vector2 worldPosition = transform.WorldPosition;
			angle = (sprite.NoRotation ? (-eye.Rotation) : (item2 - angle3));
			Vector2 vector = matrix.Transform(Matrix3.CreateInverseTransform(ref worldPosition, ref angle).Transform(worldPos));
			if (this.CheckDirBound(sprite, angle2, vector))
			{
				return true;
			}
			foreach (ISpriteLayer spriteLayer in sprite.AllLayers)
			{
				if (spriteLayer.Visible)
				{
					SpriteComponent.Layer layer = spriteLayer as SpriteComponent.Layer;
					if (layer != null)
					{
						if (layer.Texture != null)
						{
							Vector2i pos = (Vector2i)(vector * 32f * new ValueTuple<float, float>(1f, -1f) + layer.Texture.Size / 2f);
							if (this._clickMapManager.IsOccluding(layer.Texture, pos))
							{
								return true;
							}
						}
						RSI actualRsi = layer.ActualRsi;
						RSI.State state;
						if (actualRsi != null && actualRsi.TryGetState(layer.State, ref state))
						{
							RSI.State.Direction direction = SpriteComponent.Layer.GetDirection(state.Directions, angle2);
							Matrix3 matrix2;
							layer.GetLayerDrawMatrix(direction, ref matrix2);
							Vector2i pos2 = (Vector2i)(Matrix3.Invert(ref matrix2).Transform(vector) * 32f * new ValueTuple<float, float>(1f, -1f) + state.Size / 2f);
							if (sprite.EnableDirectionOverride)
							{
								direction = DirExt.Convert(sprite.DirectionOverride, state.Directions);
							}
							direction = DirExt.OffsetRsiDir(direction, layer.DirOffset);
							if (this._clickMapManager.IsOccluding(layer.ActualRsi, layer.State, direction, layer.AnimationFrame, pos2))
							{
								return true;
							}
						}
					}
				}
			}
			drawDepth = 0;
			renderOrder = 0U;
			bottom = 0f;
			return false;
		}

		// Token: 0x060017D4 RID: 6100 RVA: 0x00089394 File Offset: 0x00087594
		public bool CheckDirBound(SpriteComponent sprite, Angle relativeRotation, Vector2 localPos)
		{
			if (this.Bounds == null)
			{
				return false;
			}
			Direction cardinalDir = relativeRotation.GetCardinalDir();
			Vector2 vector = sprite.NoRotation ? localPos : DirectionExtensions.ToAngle(cardinalDir).RotateVec(ref localPos);
			if (this.Bounds.All.Contains(vector, true))
			{
				return true;
			}
			Box2 box;
			switch (sprite.EnableDirectionOverride ? sprite.DirectionOverride : cardinalDir)
			{
			case 0:
				box = this.Bounds.South;
				goto IL_BE;
			case 2:
				box = this.Bounds.East;
				goto IL_BE;
			case 4:
				box = this.Bounds.North;
				goto IL_BE;
			case 6:
				box = this.Bounds.West;
				goto IL_BE;
			}
			throw new InvalidOperationException();
			IL_BE:
			Box2 box2 = box;
			return box2.Contains(vector, true);
		}

		// Token: 0x04000C20 RID: 3104
		[Dependency]
		private readonly IClickMapManager _clickMapManager;

		// Token: 0x04000C21 RID: 3105
		[Nullable(2)]
		[DataField("bounds", false, 1, false, false, null)]
		public ClickableComponent.DirBoundData Bounds;

		// Token: 0x020003C0 RID: 960
		[NullableContext(0)]
		[DataDefinition]
		public sealed class DirBoundData
		{
			// Token: 0x04000C22 RID: 3106
			[DataField("all", false, 1, false, false, null)]
			public Box2 All;

			// Token: 0x04000C23 RID: 3107
			[DataField("north", false, 1, false, false, null)]
			public Box2 North;

			// Token: 0x04000C24 RID: 3108
			[DataField("south", false, 1, false, false, null)]
			public Box2 South;

			// Token: 0x04000C25 RID: 3109
			[DataField("east", false, 1, false, false, null)]
			public Box2 East;

			// Token: 0x04000C26 RID: 3110
			[DataField("west", false, 1, false, false, null)]
			public Box2 West;
		}
	}
}
