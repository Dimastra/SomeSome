using System;
using System.Runtime.CompilerServices;
using Content.Client.Atmos.Components;
using Content.Shared.Atmos;
using Robust.Client.GameObjects;
using Robust.Shared.GameObjects;
using Robust.Shared.Map;
using Robust.Shared.Maths;

namespace Content.Client.Atmos.EntitySystems
{
	// Token: 0x0200045B RID: 1115
	[NullableContext(1)]
	[Nullable(new byte[]
	{
		0,
		1
	})]
	public sealed class FireVisualizerSystem : VisualizerSystem<FireVisualsComponent>
	{
		// Token: 0x06001BC4 RID: 7108 RVA: 0x000A08E6 File Offset: 0x0009EAE6
		public override void Initialize()
		{
			base.Initialize();
			base.SubscribeLocalEvent<FireVisualsComponent, ComponentInit>(new ComponentEventHandler<FireVisualsComponent, ComponentInit>(this.OnComponentInit), null, null);
			base.SubscribeLocalEvent<FireVisualsComponent, ComponentShutdown>(new ComponentEventHandler<FireVisualsComponent, ComponentShutdown>(this.OnShutdown), null, null);
		}

		// Token: 0x06001BC5 RID: 7109 RVA: 0x000A0918 File Offset: 0x0009EB18
		private void OnShutdown(EntityUid uid, FireVisualsComponent component, ComponentShutdown args)
		{
			if (component.LightEntity != null)
			{
				base.Del(component.LightEntity.Value);
				component.LightEntity = null;
			}
			SpriteComponent spriteComponent;
			if (base.TryComp<SpriteComponent>(uid, ref spriteComponent))
			{
				spriteComponent.RemoveLayer(FireVisualLayers.Fire);
			}
		}

		// Token: 0x06001BC6 RID: 7110 RVA: 0x000A0968 File Offset: 0x0009EB68
		private void OnComponentInit(EntityUid uid, FireVisualsComponent component, ComponentInit args)
		{
			SpriteComponent spriteComponent;
			AppearanceComponent appearance;
			if (!base.TryComp<SpriteComponent>(uid, ref spriteComponent) || !base.TryComp<AppearanceComponent>(uid, ref appearance))
			{
				return;
			}
			spriteComponent.LayerMapReserveBlank(FireVisualLayers.Fire);
			spriteComponent.LayerSetVisible(FireVisualLayers.Fire, false);
			spriteComponent.LayerSetShader(FireVisualLayers.Fire, "unshaded");
			if (component.Sprite != null)
			{
				spriteComponent.LayerSetRSI(FireVisualLayers.Fire, component.Sprite);
			}
			this.UpdateAppearance(uid, component, spriteComponent, appearance);
		}

		// Token: 0x06001BC7 RID: 7111 RVA: 0x000A09DB File Offset: 0x0009EBDB
		protected override void OnAppearanceChange(EntityUid uid, FireVisualsComponent component, ref AppearanceChangeEvent args)
		{
			if (args.Sprite != null)
			{
				this.UpdateAppearance(uid, component, args.Sprite, args.Component);
			}
		}

		// Token: 0x06001BC8 RID: 7112 RVA: 0x000A09FC File Offset: 0x0009EBFC
		private void UpdateAppearance(EntityUid uid, FireVisualsComponent component, SpriteComponent sprite, AppearanceComponent appearance)
		{
			int num;
			if (!sprite.LayerMapTryGet(FireVisualLayers.Fire, ref num, false))
			{
				return;
			}
			bool flag;
			this.AppearanceSystem.TryGetData<bool>(uid, FireVisuals.OnFire, ref flag, appearance);
			float num2;
			this.AppearanceSystem.TryGetData<float>(uid, FireVisuals.FireStacks, ref num2, appearance);
			sprite.LayerSetVisible(num, flag);
			if (!flag)
			{
				if (component.LightEntity != null)
				{
					base.Del(component.LightEntity.Value);
					component.LightEntity = null;
				}
				return;
			}
			if (num2 > (float)component.FireStackAlternateState && !string.IsNullOrEmpty(component.AlternateState))
			{
				sprite.LayerSetState(num, component.AlternateState);
			}
			else
			{
				sprite.LayerSetState(num, component.NormalState);
			}
			EntityUid value = component.LightEntity.GetValueOrDefault();
			if (component.LightEntity == null)
			{
				value = base.Spawn(null, new EntityCoordinates(uid, default(Vector2)));
				component.LightEntity = new EntityUid?(value);
			}
			PointLightComponent pointLightComponent = base.EnsureComp<PointLightComponent>(component.LightEntity.Value);
			pointLightComponent.Color = component.LightColor;
			pointLightComponent.Radius = Math.Clamp(1.5f + component.LightRadiusPerStack * num2, 0f, component.MaxLightRadius);
			pointLightComponent.Energy = Math.Clamp(1f + component.LightEnergyPerStack * num2, 0f, component.MaxLightEnergy);
		}
	}
}
