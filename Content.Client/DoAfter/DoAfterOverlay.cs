using System;
using System.Runtime.CompilerServices;
using Content.Shared.DoAfter;
using Robust.Client.GameObjects;
using Robust.Client.Graphics;
using Robust.Shared.Enums;
using Robust.Shared.GameObjects;
using Robust.Shared.Maths;
using Robust.Shared.Prototypes;
using Robust.Shared.Utility;

namespace Content.Client.DoAfter
{
	// Token: 0x02000347 RID: 839
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class DoAfterOverlay : Overlay
	{
		// Token: 0x17000437 RID: 1079
		// (get) Token: 0x060014CC RID: 5324 RVA: 0x00005516 File Offset: 0x00003716
		public override OverlaySpace Space
		{
			get
			{
				return 8;
			}
		}

		// Token: 0x060014CD RID: 5325 RVA: 0x0007A078 File Offset: 0x00078278
		public DoAfterOverlay(IEntityManager entManager, IPrototypeManager protoManager)
		{
			this._entManager = entManager;
			this._transform = this._entManager.EntitySysManager.GetEntitySystem<SharedTransformSystem>();
			SpriteSpecifier.Rsi rsi = new SpriteSpecifier.Rsi(new ResourcePath("/Textures/Interface/Misc/progress_bar.rsi", "/"), "icon");
			this._barTexture = this._entManager.EntitySysManager.GetEntitySystem<SpriteSystem>().Frame0(rsi);
			this._shader = protoManager.Index<ShaderPrototype>("unshaded").Instance();
		}

		// Token: 0x060014CE RID: 5326 RVA: 0x0007A0F4 File Offset: 0x000782F4
		protected override void Draw(in OverlayDrawArgs args)
		{
			DrawingHandleWorld worldHandle = args.WorldHandle;
			IEye eye = args.Viewport.Eye;
			Angle angle = (eye != null) ? eye.Rotation : Angle.Zero;
			EntityQuery<SpriteComponent> entityQuery = this._entManager.GetEntityQuery<SpriteComponent>();
			EntityQuery<TransformComponent> entityQuery2 = this._entManager.GetEntityQuery<TransformComponent>();
			Vector2 vector = new Vector2(1f, 1f);
			Matrix3 matrix = Matrix3.CreateScale(ref vector);
			Matrix3 matrix2 = Matrix3.CreateRotation(-angle);
			worldHandle.UseShader(this._shader);
			foreach (DoAfterComponent doAfterComponent in this._entManager.EntityQuery<DoAfterComponent>(true))
			{
				TransformComponent transformComponent;
				if (doAfterComponent.DoAfters.Count != 0 && entityQuery2.TryGetComponent(doAfterComponent.Owner, ref transformComponent) && !(transformComponent.MapID != args.MapId))
				{
					Vector2 worldPosition = this._transform.GetWorldPosition(transformComponent);
					int num = 0;
					Matrix3 matrix3 = Matrix3.CreateTranslation(worldPosition);
					foreach (DoAfter doAfter in doAfterComponent.DoAfters.Values)
					{
						TimeSpan elapsed = doAfter.Elapsed;
						float num2 = MathF.Min(1f, (float)elapsed.TotalSeconds / doAfter.Delay);
						Matrix3 matrix4;
						Matrix3.Multiply(ref matrix, ref matrix3, ref matrix4);
						Matrix3 matrix5;
						Matrix3.Multiply(ref matrix2, ref matrix4, ref matrix5);
						worldHandle.SetTransform(ref matrix5);
						float num3 = (float)this._barTexture.Height / 1f * (float)num;
						SpriteComponent spriteComponent;
						float num4;
						if (entityQuery.TryGetComponent(doAfterComponent.Owner, ref spriteComponent))
						{
							num4 = spriteComponent.Bounds.Height / 2f + 0.05f;
						}
						else
						{
							num4 = 0.5f;
						}
						Vector2 vector2;
						vector2..ctor((float)(-(float)this._barTexture.Width) / 2f / 32f, num4 / 1f + num3 / 32f * 1f);
						worldHandle.DrawTexture(this._barTexture, vector2, null);
						Color progressColor;
						if (doAfter.Cancelled)
						{
							bool flag = Math.Floor((double)((float)doAfter.CancelledElapsed.TotalSeconds / 0.125f)) % 2.0 == 0.0;
							progressColor..ctor(1f, 0f, 0f, flag ? 1f : 0f);
						}
						else
						{
							progressColor = DoAfterOverlay.GetProgressColor(num2);
						}
						float num5 = 20f * num2 + 2f;
						Box2 box;
						box..ctor(new Vector2(2f, 3f) / 32f, new Vector2(num5, 4f) / 32f);
						box = box.Translated(vector2);
						worldHandle.DrawRect(box, progressColor, true);
						num++;
					}
				}
			}
			worldHandle.UseShader(null);
			worldHandle.SetTransform(ref Matrix3.Identity);
		}

		// Token: 0x060014CF RID: 5327 RVA: 0x0007A430 File Offset: 0x00078630
		public static Color GetProgressColor(float progress)
		{
			if (progress >= 1f)
			{
				return new Color(0f, 1f, 0f, 1f);
			}
			return Color.FromHsv(new ValueTuple<float, float, float, float>(0.2777778f * progress, 1f, 0.75f, 1f));
		}

		// Token: 0x04000AD6 RID: 2774
		private readonly IEntityManager _entManager;

		// Token: 0x04000AD7 RID: 2775
		private readonly SharedTransformSystem _transform;

		// Token: 0x04000AD8 RID: 2776
		private readonly Texture _barTexture;

		// Token: 0x04000AD9 RID: 2777
		private readonly ShaderInstance _shader;
	}
}
