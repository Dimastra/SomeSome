using System;
using System.Runtime.CompilerServices;
using Content.Shared.Singularity.Components;
using Robust.Client.Graphics;
using Robust.Shared.Enums;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Maths;
using Robust.Shared.Prototypes;

namespace Content.Client.Singularity
{
	// Token: 0x0200013F RID: 319
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class SingularityOverlay : Overlay
	{
		// Token: 0x17000175 RID: 373
		// (get) Token: 0x06000865 RID: 2149 RVA: 0x0000689B File Offset: 0x00004A9B
		public override OverlaySpace Space
		{
			get
			{
				return 4;
			}
		}

		// Token: 0x17000176 RID: 374
		// (get) Token: 0x06000866 RID: 2150 RVA: 0x00003C56 File Offset: 0x00001E56
		public override bool RequestScreenTexture
		{
			get
			{
				return true;
			}
		}

		// Token: 0x06000867 RID: 2151 RVA: 0x00030B9C File Offset: 0x0002ED9C
		public SingularityOverlay()
		{
			IoCManager.InjectDependencies<SingularityOverlay>(this);
			this._shader = this._prototypeManager.Index<ShaderPrototype>("Singularity").Instance().Duplicate();
			this._shader.SetParameter("maxDistance", 640f);
		}

		// Token: 0x06000868 RID: 2152 RVA: 0x00030C10 File Offset: 0x0002EE10
		protected override bool BeforeDraw(in OverlayDrawArgs args)
		{
			if (args.Viewport.Eye == null)
			{
				return false;
			}
			this._count = 0;
			foreach (ValueTuple<SingularityDistortionComponent, TransformComponent> valueTuple in this._entMan.EntityQuery<SingularityDistortionComponent, TransformComponent>(false))
			{
				SingularityDistortionComponent item = valueTuple.Item1;
				TransformComponent item2 = valueTuple.Item2;
				if (!(item2.MapID != args.MapId))
				{
					Vector2 worldPosition = item2.WorldPosition;
					if ((worldPosition - args.WorldAABB.ClosestPoint(ref worldPosition)).LengthSquared <= 400f)
					{
						Vector2 vector = args.Viewport.WorldToLocal(worldPosition);
						vector.Y = (float)args.Viewport.Size.Y - vector.Y;
						this._positions[this._count] = vector;
						this._intensities[this._count] = item.Intensity;
						this._falloffPowers[this._count] = item.FalloffPower;
						this._count++;
						if (this._count == 5)
						{
							break;
						}
					}
				}
			}
			return this._count > 0;
		}

		// Token: 0x06000869 RID: 2153 RVA: 0x00030D50 File Offset: 0x0002EF50
		protected override void Draw(in OverlayDrawArgs args)
		{
			if (this.ScreenTexture == null || args.Viewport.Eye == null)
			{
				return;
			}
			ShaderInstance shader = this._shader;
			if (shader != null)
			{
				shader.SetParameter("renderScale", args.Viewport.RenderScale);
			}
			ShaderInstance shader2 = this._shader;
			if (shader2 != null)
			{
				shader2.SetParameter("count", this._count);
			}
			ShaderInstance shader3 = this._shader;
			if (shader3 != null)
			{
				shader3.SetParameter("position", this._positions);
			}
			ShaderInstance shader4 = this._shader;
			if (shader4 != null)
			{
				shader4.SetParameter("intensity", this._intensities);
			}
			ShaderInstance shader5 = this._shader;
			if (shader5 != null)
			{
				shader5.SetParameter("falloffPower", this._falloffPowers);
			}
			ShaderInstance shader6 = this._shader;
			if (shader6 != null)
			{
				shader6.SetParameter("SCREEN_TEXTURE", this.ScreenTexture);
			}
			DrawingHandleWorld worldHandle = args.WorldHandle;
			worldHandle.UseShader(this._shader);
			worldHandle.DrawRect(args.WorldAABB, Color.White, true);
			worldHandle.UseShader(null);
		}

		// Token: 0x0400043F RID: 1087
		[Dependency]
		private readonly IEntityManager _entMan;

		// Token: 0x04000440 RID: 1088
		[Dependency]
		private readonly IPrototypeManager _prototypeManager;

		// Token: 0x04000441 RID: 1089
		public const int MaxCount = 5;

		// Token: 0x04000442 RID: 1090
		private const float MaxDistance = 20f;

		// Token: 0x04000443 RID: 1091
		private readonly ShaderInstance _shader;

		// Token: 0x04000444 RID: 1092
		private Vector2[] _positions = new Vector2[5];

		// Token: 0x04000445 RID: 1093
		private float[] _intensities = new float[5];

		// Token: 0x04000446 RID: 1094
		private float[] _falloffPowers = new float[5];

		// Token: 0x04000447 RID: 1095
		private int _count;
	}
}
