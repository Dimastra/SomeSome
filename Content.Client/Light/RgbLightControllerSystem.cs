using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Content.Client.Items.Systems;
using Content.Shared.Clothing;
using Content.Shared.Hands;
using Content.Shared.Inventory.Events;
using Content.Shared.Light;
using Content.Shared.Light.Component;
using Robust.Client.GameObjects;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.IoC;
using Robust.Shared.Log;
using Robust.Shared.Map.Components;
using Robust.Shared.Maths;
using Robust.Shared.Timing;

namespace Content.Client.Light
{
	// Token: 0x02000264 RID: 612
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class RgbLightControllerSystem : SharedRgbLightControllerSystem
	{
		// Token: 0x06000FA5 RID: 4005 RVA: 0x0005E348 File Offset: 0x0005C548
		public override void Initialize()
		{
			base.Initialize();
			base.SubscribeLocalEvent<RgbLightControllerComponent, ComponentHandleState>(new ComponentEventRefHandler<RgbLightControllerComponent, ComponentHandleState>(this.OnHandleState), null, null);
			base.SubscribeLocalEvent<RgbLightControllerComponent, ComponentShutdown>(new ComponentEventHandler<RgbLightControllerComponent, ComponentShutdown>(this.OnComponentShutdown), null, null);
			base.SubscribeLocalEvent<RgbLightControllerComponent, ComponentStartup>(new ComponentEventHandler<RgbLightControllerComponent, ComponentStartup>(this.OnComponentStart), null, null);
			base.SubscribeLocalEvent<RgbLightControllerComponent, GotUnequippedEvent>(new ComponentEventHandler<RgbLightControllerComponent, GotUnequippedEvent>(this.OnGotUnequipped), null, null);
			base.SubscribeLocalEvent<RgbLightControllerComponent, EquipmentVisualsUpdatedEvent>(new ComponentEventHandler<RgbLightControllerComponent, EquipmentVisualsUpdatedEvent>(this.OnEquipmentVisualsUpdated), null, null);
			base.SubscribeLocalEvent<RgbLightControllerComponent, HeldVisualsUpdatedEvent>(new ComponentEventHandler<RgbLightControllerComponent, HeldVisualsUpdatedEvent>(this.OnHeldVisualsUpdated), null, null);
		}

		// Token: 0x06000FA6 RID: 4006 RVA: 0x0005E3D3 File Offset: 0x0005C5D3
		private void OnComponentStart(EntityUid uid, RgbLightControllerComponent rgb, ComponentStartup args)
		{
			this.GetOriginalColors(uid, rgb, null, null);
			this._itemSystem.VisualsChanged(uid);
		}

		// Token: 0x06000FA7 RID: 4007 RVA: 0x0005E3EB File Offset: 0x0005C5EB
		private void OnComponentShutdown(EntityUid uid, RgbLightControllerComponent rgb, ComponentShutdown args)
		{
			if (base.LifeStage(uid, null) >= 4)
			{
				return;
			}
			this.ResetOriginalColors(uid, rgb, null, null);
			this._itemSystem.VisualsChanged(uid);
		}

		// Token: 0x06000FA8 RID: 4008 RVA: 0x0005E40F File Offset: 0x0005C60F
		private void OnGotUnequipped(EntityUid uid, RgbLightControllerComponent rgb, GotUnequippedEvent args)
		{
			rgb.Holder = null;
			rgb.HolderLayers = null;
		}

		// Token: 0x06000FA9 RID: 4009 RVA: 0x0005E424 File Offset: 0x0005C624
		private void OnHeldVisualsUpdated(EntityUid uid, RgbLightControllerComponent rgb, HeldVisualsUpdatedEvent args)
		{
			if (args.RevealedLayers.Count == 0)
			{
				rgb.Holder = null;
				rgb.HolderLayers = null;
				return;
			}
			rgb.Holder = new EntityUid?(args.User);
			rgb.HolderLayers = new List<string>();
			SpriteComponent spriteComponent;
			if (!base.TryComp<SpriteComponent>(args.User, ref spriteComponent))
			{
				return;
			}
			foreach (string text in args.RevealedLayers)
			{
				int num;
				if (spriteComponent.LayerMapTryGet(text, ref num, false))
				{
					SpriteComponent.Layer layer = spriteComponent[num] as SpriteComponent.Layer;
					if (layer != null && layer.ShaderPrototype == "unshaded")
					{
						rgb.HolderLayers.Add(text);
					}
				}
			}
		}

		// Token: 0x06000FAA RID: 4010 RVA: 0x0005E4FC File Offset: 0x0005C6FC
		private void OnEquipmentVisualsUpdated(EntityUid uid, RgbLightControllerComponent rgb, EquipmentVisualsUpdatedEvent args)
		{
			rgb.Holder = new EntityUid?(args.Equipee);
			rgb.HolderLayers = new List<string>();
			SpriteComponent spriteComponent;
			if (!base.TryComp<SpriteComponent>(args.Equipee, ref spriteComponent))
			{
				return;
			}
			foreach (string text in args.RevealedLayers)
			{
				int num;
				if (spriteComponent.LayerMapTryGet(text, ref num, false))
				{
					SpriteComponent.Layer layer = spriteComponent[num] as SpriteComponent.Layer;
					if (layer != null && layer.ShaderPrototype == "unshaded")
					{
						rgb.HolderLayers.Add(text);
					}
				}
			}
		}

		// Token: 0x06000FAB RID: 4011 RVA: 0x0005E5B4 File Offset: 0x0005C7B4
		private void OnHandleState(EntityUid uid, RgbLightControllerComponent rgb, ref ComponentHandleState args)
		{
			RgbLightControllerState rgbLightControllerState = args.Current as RgbLightControllerState;
			if (rgbLightControllerState == null)
			{
				return;
			}
			this.ResetOriginalColors(uid, rgb, null, null);
			rgb.CycleRate = rgbLightControllerState.CycleRate;
			rgb.Layers = rgbLightControllerState.Layers;
			this.GetOriginalColors(uid, rgb, null, null);
		}

		// Token: 0x06000FAC RID: 4012 RVA: 0x0005E600 File Offset: 0x0005C800
		[NullableContext(2)]
		private void GetOriginalColors(EntityUid uid, RgbLightControllerComponent rgb = null, PointLightComponent light = null, SpriteComponent sprite = null)
		{
			if (!base.Resolve<RgbLightControllerComponent, SpriteComponent, PointLightComponent>(uid, ref rgb, ref sprite, ref light, true))
			{
				return;
			}
			rgb.OriginalLightColor = light.Color;
			rgb.OriginalLayerColors = new Dictionary<int, Color>();
			int num = sprite.AllLayers.Count<ISpriteLayer>();
			if (rgb.Layers == null)
			{
				rgb.Layers = new List<int>();
				for (int i = 0; i < num; i++)
				{
					SpriteComponent.Layer layer = sprite[i] as SpriteComponent.Layer;
					if (layer != null && layer.ShaderPrototype == "unshaded")
					{
						rgb.Layers.Add(i);
						rgb.OriginalLayerColors[i] = layer.Color;
					}
				}
				return;
			}
			foreach (int num2 in rgb.Layers.ToArray())
			{
				if (num2 < num)
				{
					rgb.OriginalLayerColors[num2] = sprite[num2].Color;
				}
				else
				{
					DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(59, 2);
					defaultInterpolatedStringHandler.AppendLiteral("RGB light attempted to use invalid sprite index ");
					defaultInterpolatedStringHandler.AppendFormatted<int>(num2);
					defaultInterpolatedStringHandler.AppendLiteral(" on entity ");
					defaultInterpolatedStringHandler.AppendFormatted<EntityStringRepresentation>(base.ToPrettyString(uid));
					Logger.Warning(defaultInterpolatedStringHandler.ToStringAndClear());
					rgb.Layers.Remove(num2);
				}
			}
		}

		// Token: 0x06000FAD RID: 4013 RVA: 0x0005E744 File Offset: 0x0005C944
		[NullableContext(2)]
		private void ResetOriginalColors(EntityUid uid, RgbLightControllerComponent rgb = null, PointLightComponent light = null, SpriteComponent sprite = null)
		{
			if (!base.Resolve<RgbLightControllerComponent, SpriteComponent, PointLightComponent>(uid, ref rgb, ref sprite, ref light, false))
			{
				return;
			}
			light.Color = rgb.OriginalLightColor;
			if (rgb.Layers == null || rgb.OriginalLayerColors == null)
			{
				return;
			}
			foreach (KeyValuePair<int, Color> keyValuePair in rgb.OriginalLayerColors)
			{
				int num;
				Color color;
				keyValuePair.Deconstruct(out num, out color);
				int num2 = num;
				Color color2 = color;
				sprite.LayerSetColor(num2, color2);
			}
		}

		// Token: 0x06000FAE RID: 4014 RVA: 0x0005E7D8 File Offset: 0x0005C9D8
		public override void FrameUpdate(float frameTime)
		{
			foreach (ValueTuple<RgbLightControllerComponent, PointLightComponent, SpriteComponent> valueTuple in this.EntityManager.EntityQuery<RgbLightControllerComponent, PointLightComponent, SpriteComponent>(false))
			{
				RgbLightControllerComponent item = valueTuple.Item1;
				PointLightComponent item2 = valueTuple.Item2;
				SpriteComponent item3 = valueTuple.Item3;
				Color currentRgbColor = RgbLightControllerSystem.GetCurrentRgbColor(this._gameTiming.RealTime, item.CreationTick.Value * this._gameTiming.TickPeriod, item);
				item2.Color = currentRgbColor;
				if (item.Layers != null)
				{
					foreach (int num in item.Layers)
					{
						item3.LayerSetColor(num, currentRgbColor);
					}
				}
				SpriteComponent spriteComponent;
				if (item.HolderLayers != null && base.TryComp<SpriteComponent>(item.Holder, ref spriteComponent))
				{
					foreach (string text in item.HolderLayers)
					{
						spriteComponent.LayerSetColor(text, currentRgbColor);
					}
				}
			}
			foreach (ValueTuple<RgbLightControllerComponent, MapLightComponent> valueTuple2 in base.EntityQuery<RgbLightControllerComponent, MapLightComponent>(false))
			{
				RgbLightControllerComponent item4 = valueTuple2.Item1;
				MapLightComponent item5 = valueTuple2.Item2;
				Color currentRgbColor2 = RgbLightControllerSystem.GetCurrentRgbColor(this._gameTiming.RealTime, item4.CreationTick.Value * this._gameTiming.TickPeriod, item4);
				item5.AmbientLightColor = currentRgbColor2;
			}
		}

		// Token: 0x06000FAF RID: 4015 RVA: 0x0005E9A4 File Offset: 0x0005CBA4
		public static Color GetCurrentRgbColor(TimeSpan curTime, TimeSpan offset, RgbLightControllerComponent rgb)
		{
			return Color.FromHsv(new Vector4((float)(((curTime.TotalSeconds - offset.TotalSeconds) * (double)rgb.CycleRate + Math.Abs((double)rgb.Owner.GetHashCode() * 0.1)) % 1.0), 1f, 1f, 1f));
		}

		// Token: 0x040007B9 RID: 1977
		[Dependency]
		private readonly IGameTiming _gameTiming;

		// Token: 0x040007BA RID: 1978
		[Dependency]
		private readonly ItemSystem _itemSystem;
	}
}
