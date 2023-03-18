using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Content.Client.Clothing;
using Content.Client.Items.Systems;
using Content.Shared.Clothing;
using Content.Shared.Hands;
using Content.Shared.Item;
using Content.Shared.Toggleable;
using Robust.Client.GameObjects;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Maths;

namespace Content.Client.Toggleable
{
	// Token: 0x020000F6 RID: 246
	[NullableContext(1)]
	[Nullable(new byte[]
	{
		0,
		1
	})]
	public sealed class ToggleableLightVisualsSystem : VisualizerSystem<ToggleableLightVisualsComponent>
	{
		// Token: 0x060006E6 RID: 1766 RVA: 0x00024194 File Offset: 0x00022394
		public override void Initialize()
		{
			base.Initialize();
			base.SubscribeLocalEvent<ToggleableLightVisualsComponent, GetInhandVisualsEvent>(new ComponentEventHandler<ToggleableLightVisualsComponent, GetInhandVisualsEvent>(this.OnGetHeldVisuals), null, new Type[]
			{
				typeof(ItemSystem)
			});
			base.SubscribeLocalEvent<ToggleableLightVisualsComponent, GetEquipmentVisualsEvent>(new ComponentEventHandler<ToggleableLightVisualsComponent, GetEquipmentVisualsEvent>(this.OnGetEquipmentVisuals), null, new Type[]
			{
				typeof(ClientClothingSystem)
			});
		}

		// Token: 0x060006E7 RID: 1767 RVA: 0x000241F4 File Offset: 0x000223F4
		protected override void OnAppearanceChange(EntityUid uid, ToggleableLightVisualsComponent component, ref AppearanceChangeEvent args)
		{
			bool flag;
			if (!this.AppearanceSystem.TryGetData<bool>(uid, ToggleableLightVisuals.Enabled, ref flag, args.Component))
			{
				return;
			}
			Color color;
			bool flag2 = this.AppearanceSystem.TryGetData<Color>(uid, ToggleableLightVisuals.Color, ref color, args.Component);
			int num;
			if (args.Sprite != null && args.Sprite.LayerMapTryGet(component.SpriteLayer, ref num, false))
			{
				args.Sprite.LayerSetVisible(num, flag);
				if (flag2)
				{
					args.Sprite.LayerSetColor(num, color);
				}
			}
			PointLightComponent pointLightComponent;
			if (base.TryComp<PointLightComponent>(uid, ref pointLightComponent))
			{
				pointLightComponent.Enabled = flag;
				if (flag && flag2)
				{
					pointLightComponent.Color = color;
				}
			}
			this._itemSys.VisualsChanged(uid);
		}

		// Token: 0x060006E8 RID: 1768 RVA: 0x000242A0 File Offset: 0x000224A0
		private void OnGetEquipmentVisuals(EntityUid uid, ToggleableLightVisualsComponent component, GetEquipmentVisualsEvent args)
		{
			AppearanceComponent appearanceComponent;
			bool flag;
			if (!base.TryComp<AppearanceComponent>(uid, ref appearanceComponent) || !this.AppearanceSystem.TryGetData<bool>(uid, ToggleableLightVisuals.Enabled, ref flag, appearanceComponent) || !flag)
			{
				return;
			}
			List<SharedSpriteComponent.PrototypeLayerData> list;
			if (!component.ClothingVisuals.TryGetValue(args.Slot, out list))
			{
				return;
			}
			Color value;
			bool flag2 = this.AppearanceSystem.TryGetData<Color>(uid, ToggleableLightVisuals.Color, ref value, appearanceComponent);
			int num = 0;
			foreach (SharedSpriteComponent.PrototypeLayerData prototypeLayerData in list)
			{
				HashSet<string> mapKeys = prototypeLayerData.MapKeys;
				string text = (mapKeys != null) ? mapKeys.FirstOrDefault<string>() : null;
				if (text == null)
				{
					string text2;
					if (num != 0)
					{
						DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(8, 2);
						defaultInterpolatedStringHandler.AppendFormatted(args.Slot);
						defaultInterpolatedStringHandler.AppendLiteral("-toggle-");
						defaultInterpolatedStringHandler.AppendFormatted<int>(num);
						text2 = defaultInterpolatedStringHandler.ToStringAndClear();
					}
					else
					{
						text2 = args.Slot + "-toggle";
					}
					text = text2;
					num++;
				}
				if (flag2)
				{
					prototypeLayerData.Color = new Color?(value);
				}
				args.Layers.Add(new ValueTuple<string, SharedSpriteComponent.PrototypeLayerData>(text, prototypeLayerData));
			}
		}

		// Token: 0x060006E9 RID: 1769 RVA: 0x000243D4 File Offset: 0x000225D4
		private void OnGetHeldVisuals(EntityUid uid, ToggleableLightVisualsComponent component, GetInhandVisualsEvent args)
		{
			AppearanceComponent appearanceComponent;
			bool flag;
			if (!base.TryComp<AppearanceComponent>(uid, ref appearanceComponent) || !this.AppearanceSystem.TryGetData<bool>(uid, ToggleableLightVisuals.Enabled, ref flag, appearanceComponent) || !flag)
			{
				return;
			}
			List<SharedSpriteComponent.PrototypeLayerData> list;
			if (!component.InhandVisuals.TryGetValue(args.Location, out list))
			{
				return;
			}
			Color value;
			bool flag2 = this.AppearanceSystem.TryGetData<Color>(uid, ToggleableLightVisuals.Color, ref value, appearanceComponent);
			int num = 0;
			string text = "inhand-" + args.Location.ToString().ToLowerInvariant() + "-toggle";
			foreach (SharedSpriteComponent.PrototypeLayerData prototypeLayerData in list)
			{
				HashSet<string> mapKeys = prototypeLayerData.MapKeys;
				string text2 = (mapKeys != null) ? mapKeys.FirstOrDefault<string>() : null;
				if (text2 == null)
				{
					string text3;
					if (num != 0)
					{
						DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(1, 2);
						defaultInterpolatedStringHandler.AppendFormatted(text);
						defaultInterpolatedStringHandler.AppendLiteral("-");
						defaultInterpolatedStringHandler.AppendFormatted<int>(num);
						text3 = defaultInterpolatedStringHandler.ToStringAndClear();
					}
					else
					{
						text3 = text;
					}
					text2 = text3;
					num++;
				}
				if (flag2)
				{
					prototypeLayerData.Color = new Color?(value);
				}
				args.Layers.Add(new ValueTuple<string, SharedSpriteComponent.PrototypeLayerData>(text2, prototypeLayerData));
			}
		}

		// Token: 0x0400032A RID: 810
		[Dependency]
		private readonly SharedItemSystem _itemSys;
	}
}
