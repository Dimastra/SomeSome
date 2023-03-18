using System;
using System.Linq;
using System.Runtime.CompilerServices;
using Content.Shared.FixedPoint;
using Content.Shared.Fluids;
using Robust.Client.GameObjects;
using Robust.Client.Graphics;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Log;
using Robust.Shared.Maths;
using Robust.Shared.Random;

namespace Content.Client.Fluids
{
	// Token: 0x02000313 RID: 787
	[NullableContext(1)]
	[Nullable(new byte[]
	{
		0,
		1
	})]
	public sealed class PuddleVisualizerSystem : VisualizerSystem<PuddleVisualizerComponent>
	{
		// Token: 0x060013D5 RID: 5077 RVA: 0x00074AFC File Offset: 0x00072CFC
		public override void Initialize()
		{
			base.Initialize();
			base.SubscribeLocalEvent<PuddleVisualizerComponent, ComponentInit>(new ComponentEventHandler<PuddleVisualizerComponent, ComponentInit>(this.OnComponentInit), null, null);
		}

		// Token: 0x060013D6 RID: 5078 RVA: 0x00074B18 File Offset: 0x00072D18
		private void OnComponentInit(EntityUid uid, PuddleVisualizerComponent puddleVisuals, ComponentInit args)
		{
			SpriteComponent spriteComponent;
			if (!base.TryComp<SpriteComponent>(uid, ref spriteComponent))
			{
				return;
			}
			puddleVisuals.OriginalRsi = spriteComponent.BaseRSI;
			this.RandomizeState(spriteComponent, puddleVisuals.OriginalRsi);
			this.RandomizeRotation(spriteComponent);
		}

		// Token: 0x060013D7 RID: 5079 RVA: 0x00074B54 File Offset: 0x00072D54
		protected override void OnAppearanceChange(EntityUid uid, PuddleVisualizerComponent component, ref AppearanceChangeEvent args)
		{
			if (args.Sprite == null)
			{
				DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(66, 1);
				defaultInterpolatedStringHandler.AppendLiteral("Missing SpriteComponent for PuddleVisualizerSystem on entityUid = ");
				defaultInterpolatedStringHandler.AppendFormatted<EntityUid>(uid);
				Logger.Warning(defaultInterpolatedStringHandler.ToStringAndClear());
				return;
			}
			float num;
			FixedPoint2 a;
			Color color;
			bool flag;
			if (!this.AppearanceSystem.TryGetData<float>(uid, PuddleVisuals.VolumeScale, ref num, null) || !this.AppearanceSystem.TryGetData<FixedPoint2>(uid, PuddleVisuals.CurrentVolume, ref a, null) || !this.AppearanceSystem.TryGetData<Color>(uid, PuddleVisuals.SolutionColor, ref color, null) || !this.AppearanceSystem.TryGetData<bool>(uid, PuddleVisuals.IsEvaporatingVisual, ref flag, null))
			{
				return;
			}
			float num2 = Math.Min(1f, num * 0.75f + 0.25f);
			Color color2 = component.Recolor ? color.WithAlpha(num2) : args.Sprite.Color.WithAlpha(num2);
			args.Sprite.LayerSetColor(0, color2);
			if (component.CustomPuddleSprite)
			{
				return;
			}
			if (flag && a <= component.WetFloorEffectThreshold)
			{
				if (args.Sprite.LayerGetState(0) != "sparkles")
				{
					this.StartWetFloorEffect(args.Sprite, component.WetFloorEffectAlpha);
					return;
				}
			}
			else if (args.Sprite.LayerGetState(0) == "sparkles")
			{
				this.EndWetFloorEffect(args.Sprite, component.OriginalRsi);
			}
		}

		// Token: 0x060013D8 RID: 5080 RVA: 0x00074CBC File Offset: 0x00072EBC
		private void StartWetFloorEffect(SpriteComponent sprite, float alpha)
		{
			sprite.LayerSetState(0, "sparkles", "Fluids/wet_floor_sparkles.rsi");
			sprite.Color = sprite.Color.WithAlpha(alpha);
			sprite.LayerSetAutoAnimated(0, false);
			sprite.LayerSetAutoAnimated(0, true);
		}

		// Token: 0x060013D9 RID: 5081 RVA: 0x00074D04 File Offset: 0x00072F04
		private void EndWetFloorEffect(SpriteComponent sprite, [Nullable(2)] RSI originalRSI)
		{
			this.RandomizeState(sprite, originalRSI);
			sprite.LayerSetAutoAnimated(0, false);
		}

		// Token: 0x060013DA RID: 5082 RVA: 0x00074D18 File Offset: 0x00072F18
		private void RandomizeState(SpriteComponent sprite, [Nullable(2)] RSI rsi)
		{
			RSI.State[] array = (rsi != null) ? rsi.ToArray<RSI.State>() : null;
			if (array == null || array.Length <= 0)
			{
				return;
			}
			int num = this._random.Next(0, array.Length - 1);
			sprite.LayerSetState(0, array[num].StateId, rsi);
		}

		// Token: 0x060013DB RID: 5083 RVA: 0x00074D60 File Offset: 0x00072F60
		private void RandomizeRotation(SpriteComponent sprite)
		{
			float num = (float)this._random.Next(0, 359);
			sprite.Rotation = Angle.FromDegrees((double)num);
		}

		// Token: 0x040009F8 RID: 2552
		[Dependency]
		private readonly IRobustRandom _random;
	}
}
