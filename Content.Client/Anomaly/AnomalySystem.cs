using System;
using System.Runtime.CompilerServices;
using Content.Client.Gravity;
using Content.Shared.Anomaly;
using Content.Shared.Anomaly.Components;
using Robust.Client.GameObjects;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Maths;
using Robust.Shared.Timing;

namespace Content.Client.Anomaly
{
	// Token: 0x0200046A RID: 1130
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class AnomalySystem : SharedAnomalySystem
	{
		// Token: 0x06001C09 RID: 7177 RVA: 0x000A29F8 File Offset: 0x000A0BF8
		public override void Initialize()
		{
			base.Initialize();
			base.SubscribeLocalEvent<AnomalyComponent, AppearanceChangeEvent>(new ComponentEventRefHandler<AnomalyComponent, AppearanceChangeEvent>(this.OnAppearanceChanged), null, null);
			base.SubscribeLocalEvent<AnomalyComponent, ComponentStartup>(new ComponentEventHandler<AnomalyComponent, ComponentStartup>(this.OnStartup), null, null);
			base.SubscribeLocalEvent<AnomalyComponent, AnimationCompletedEvent>(new ComponentEventHandler<AnomalyComponent, AnimationCompletedEvent>(this.OnAnimationComplete), null, null);
		}

		// Token: 0x06001C0A RID: 7178 RVA: 0x000A2A47 File Offset: 0x000A0C47
		private void OnStartup(EntityUid uid, AnomalyComponent component, ComponentStartup args)
		{
			this._floating.FloatAnimation(uid, component.FloatingOffset, component.AnimationKey, component.AnimationTime, false);
		}

		// Token: 0x06001C0B RID: 7179 RVA: 0x000A2A68 File Offset: 0x000A0C68
		private void OnAnimationComplete(EntityUid uid, AnomalyComponent component, AnimationCompletedEvent args)
		{
			if (args.Key != component.AnimationKey)
			{
				return;
			}
			this._floating.FloatAnimation(uid, component.FloatingOffset, component.AnimationKey, component.AnimationTime, false);
		}

		// Token: 0x06001C0C RID: 7180 RVA: 0x000A2AA0 File Offset: 0x000A0CA0
		private void OnAppearanceChanged(EntityUid uid, AnomalyComponent component, ref AppearanceChangeEvent args)
		{
			SpriteComponent sprite = args.Sprite;
			if (sprite == null)
			{
				return;
			}
			bool flag;
			if (!this.Appearance.TryGetData<bool>(uid, AnomalyVisuals.IsPulsing, ref flag, args.Component))
			{
				flag = false;
			}
			bool flag2;
			if (this.Appearance.TryGetData<bool>(uid, AnomalyVisuals.Supercritical, ref flag2, args.Component) && flag2)
			{
				flag = flag2;
			}
			if (base.HasComp<AnomalySupercriticalComponent>(uid))
			{
				flag = true;
			}
			int num;
			int num2;
			if (!sprite.LayerMapTryGet(AnomalyVisualLayers.Base, ref num, false) || !sprite.LayerMapTryGet(AnomalyVisualLayers.Animated, ref num2, false))
			{
				return;
			}
			sprite.LayerSetVisible(num, !flag);
			sprite.LayerSetVisible(num2, flag);
		}

		// Token: 0x06001C0D RID: 7181 RVA: 0x000A2B38 File Offset: 0x000A0D38
		public override void Update(float frameTime)
		{
			base.Update(frameTime);
			foreach (ValueTuple<AnomalySupercriticalComponent, SpriteComponent> valueTuple in base.EntityQuery<AnomalySupercriticalComponent, SpriteComponent>(false))
			{
				AnomalySupercriticalComponent item = valueTuple.Item1;
				SpriteComponent item2 = valueTuple.Item2;
				float num = 1f - (float)((item.EndTime - this._timing.CurTime) / item.SupercriticalDuration);
				float num2 = num * (item.MaxScaleAmount - 1f) + 1f;
				item2.Scale = new Vector2(num2, num2);
				byte b = (byte)(65f * (1f - num) + 190f);
				if (b < item2.Color.AByte)
				{
					item2.Color = item2.Color.WithAlpha(b);
				}
			}
		}

		// Token: 0x04000E1A RID: 3610
		[Dependency]
		private readonly IGameTiming _timing;

		// Token: 0x04000E1B RID: 3611
		[Dependency]
		private readonly FloatingVisualizerSystem _floating;
	}
}
