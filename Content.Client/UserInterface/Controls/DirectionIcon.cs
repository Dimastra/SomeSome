using System;
using System.Runtime.CompilerServices;
using Robust.Client.Graphics;
using Robust.Client.UserInterface.Controls;
using Robust.Shared.Maths;

namespace Content.Client.UserInterface.Controls
{
	// Token: 0x020000D4 RID: 212
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class DirectionIcon : TextureRect
	{
		// Token: 0x170000F8 RID: 248
		// (get) Token: 0x060005EC RID: 1516 RVA: 0x00020524 File Offset: 0x0001E724
		// (set) Token: 0x060005ED RID: 1517 RVA: 0x0002052C File Offset: 0x0001E72C
		public Angle? Rotation
		{
			get
			{
				return this._rotation;
			}
			set
			{
				this._rotation = value;
				base.SetOnlyStyleClass((value == null) ? DirectionIcon.StyleClassDirectionIconUnknown : DirectionIcon.StyleClassDirectionIconArrow);
			}
		}

		// Token: 0x060005EE RID: 1518 RVA: 0x00020550 File Offset: 0x0001E750
		public DirectionIcon()
		{
			base.Stretch = 7;
			base.SetOnlyStyleClass(DirectionIcon.StyleClassDirectionIconUnknown);
		}

		// Token: 0x060005EF RID: 1519 RVA: 0x0002056A File Offset: 0x0001E76A
		public DirectionIcon(bool snap = true, float minDistance = 0.1f) : this()
		{
			this._snap = snap;
			this._minDistance = minDistance;
		}

		// Token: 0x060005F0 RID: 1520 RVA: 0x00020580 File Offset: 0x0001E780
		public void UpdateDirection(Direction direction)
		{
			this.Rotation = new Angle?(DirectionExtensions.ToAngle(direction));
		}

		// Token: 0x060005F1 RID: 1521 RVA: 0x00020594 File Offset: 0x0001E794
		public void UpdateDirection(Vector2 direction, Angle relativeAngle)
		{
			if (direction.EqualsApprox(Vector2.Zero, (double)this._minDistance))
			{
				base.SetOnlyStyleClass(DirectionIcon.StyleClassDirectionIconHere);
				return;
			}
			Angle angle = DirectionExtensions.ToWorldAngle(direction) - relativeAngle;
			this.Rotation = new Angle?(this._snap ? DirectionExtensions.ToAngle(angle.GetDir()) : angle);
		}

		// Token: 0x060005F2 RID: 1522 RVA: 0x000205F4 File Offset: 0x0001E7F4
		protected override void Draw(DrawingHandleScreen handle)
		{
			if (this._rotation != null)
			{
				Angle angle = -this._rotation.Value;
				Vector2 vector = base.Size * this.UIScale / 2f;
				Vector2 vector2 = angle.RotateVec(ref vector) - base.Size * this.UIScale / 2f;
				vector = base.GlobalPixelPosition - vector2;
				angle = -this._rotation.Value;
				Matrix3 matrix = Matrix3.CreateTransform(ref vector, ref angle);
				handle.SetTransform(ref matrix);
			}
			base.Draw(handle);
		}

		// Token: 0x040002A7 RID: 679
		public static string StyleClassDirectionIconArrow = "direction-icon-arrow";

		// Token: 0x040002A8 RID: 680
		public static string StyleClassDirectionIconHere = "direction-icon-here";

		// Token: 0x040002A9 RID: 681
		public static string StyleClassDirectionIconUnknown = "direction-icon-unknown";

		// Token: 0x040002AA RID: 682
		private Angle? _rotation;

		// Token: 0x040002AB RID: 683
		private bool _snap;

		// Token: 0x040002AC RID: 684
		private float _minDistance;
	}
}
