using System;
using System.Linq;
using System.Runtime.CompilerServices;
using Content.Shared.Storage.Components;
using Content.Shared.Storage.EntitySystems;
using Robust.Client.GameObjects;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Utility;

namespace Content.Client.Storage.Systems
{
	// Token: 0x0200012C RID: 300
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class ItemMapperSystem : SharedItemMapperSystem
	{
		// Token: 0x0600081B RID: 2075 RVA: 0x0002F3D3 File Offset: 0x0002D5D3
		public override void Initialize()
		{
			base.Initialize();
			base.SubscribeLocalEvent<ItemMapperComponent, ComponentStartup>(new ComponentEventHandler<ItemMapperComponent, ComponentStartup>(this.OnStartup), null, null);
			base.SubscribeLocalEvent<ItemMapperComponent, AppearanceChangeEvent>(new ComponentEventRefHandler<ItemMapperComponent, AppearanceChangeEvent>(this.OnAppearance), null, null);
		}

		// Token: 0x0600081C RID: 2076 RVA: 0x0002F404 File Offset: 0x0002D604
		private void OnStartup(EntityUid uid, ItemMapperComponent component, ComponentStartup args)
		{
			SpriteComponent spriteComponent;
			if (base.TryComp<SpriteComponent>(uid, ref spriteComponent) && component.RSIPath == null)
			{
				component.RSIPath = spriteComponent.BaseRSI.Path;
			}
		}

		// Token: 0x0600081D RID: 2077 RVA: 0x0002F438 File Offset: 0x0002D638
		private void OnAppearance(EntityUid uid, ItemMapperComponent component, ref AppearanceChangeEvent args)
		{
			SpriteComponent spriteComponent;
			if (base.TryComp<SpriteComponent>(component.Owner, ref spriteComponent))
			{
				if (component.SpriteLayers.Count == 0)
				{
					this.InitLayers(component, spriteComponent, args.Component);
				}
				this.EnableLayers(component, spriteComponent, args.Component);
			}
		}

		// Token: 0x0600081E RID: 2078 RVA: 0x0002F480 File Offset: 0x0002D680
		private void InitLayers(ItemMapperComponent component, SpriteComponent spriteComponent, AppearanceComponent appearance)
		{
			ShowLayerData showLayerData;
			if (!this._appearance.TryGetData<ShowLayerData>(appearance.Owner, StorageMapVisuals.InitLayers, ref showLayerData, appearance))
			{
				return;
			}
			component.SpriteLayers.AddRange(showLayerData.QueuedEntities);
			foreach (string text in component.SpriteLayers)
			{
				spriteComponent.LayerMapReserveBlank(text);
				spriteComponent.LayerSetSprite(text, new SpriteSpecifier.Rsi(component.RSIPath, text));
				spriteComponent.LayerSetVisible(text, false);
			}
		}

		// Token: 0x0600081F RID: 2079 RVA: 0x0002F520 File Offset: 0x0002D720
		private void EnableLayers(ItemMapperComponent component, SpriteComponent spriteComponent, AppearanceComponent appearance)
		{
			ShowLayerData showLayerData;
			if (!this._appearance.TryGetData<ShowLayerData>(appearance.Owner, StorageMapVisuals.LayerChanged, ref showLayerData, appearance))
			{
				return;
			}
			foreach (string text in component.SpriteLayers)
			{
				bool flag = showLayerData.QueuedEntities.Contains(text);
				spriteComponent.LayerSetVisible(text, flag);
			}
		}

		// Token: 0x04000424 RID: 1060
		[Dependency]
		private readonly SharedAppearanceSystem _appearance;
	}
}
