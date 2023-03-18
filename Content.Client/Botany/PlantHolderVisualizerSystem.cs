using System;
using System.Runtime.CompilerServices;
using Content.Client.Botany.Components;
using Content.Shared.Botany;
using Robust.Client.GameObjects;
using Robust.Shared.GameObjects;

namespace Content.Client.Botany
{
	// Token: 0x02000414 RID: 1044
	[NullableContext(1)]
	[Nullable(new byte[]
	{
		0,
		1
	})]
	public sealed class PlantHolderVisualizerSystem : VisualizerSystem<PlantHolderVisualsComponent>
	{
		// Token: 0x060019AD RID: 6573 RVA: 0x0009369C File Offset: 0x0009189C
		public override void Initialize()
		{
			base.Initialize();
			base.SubscribeLocalEvent<PlantHolderVisualsComponent, ComponentInit>(new ComponentEventHandler<PlantHolderVisualsComponent, ComponentInit>(this.OnComponentInit), null, null);
		}

		// Token: 0x060019AE RID: 6574 RVA: 0x000936B8 File Offset: 0x000918B8
		private void OnComponentInit(EntityUid uid, PlantHolderVisualsComponent component, ComponentInit args)
		{
			SpriteComponent spriteComponent;
			if (!base.TryComp<SpriteComponent>(uid, ref spriteComponent))
			{
				return;
			}
			spriteComponent.LayerMapReserveBlank(PlantHolderLayers.Plant);
			spriteComponent.LayerSetVisible(PlantHolderLayers.Plant, false);
		}

		// Token: 0x060019AF RID: 6575 RVA: 0x000936EC File Offset: 0x000918EC
		protected override void OnAppearanceChange(EntityUid uid, PlantHolderVisualsComponent component, ref AppearanceChangeEvent args)
		{
			if (args.Sprite == null)
			{
				return;
			}
			string text;
			string text2;
			if (this.AppearanceSystem.TryGetData<string>(uid, PlantHolderVisuals.PlantRsi, ref text, args.Component) && this.AppearanceSystem.TryGetData<string>(uid, PlantHolderVisuals.PlantState, ref text2, args.Component))
			{
				bool flag = !string.IsNullOrWhiteSpace(text2);
				args.Sprite.LayerSetVisible(PlantHolderLayers.Plant, flag);
				if (flag)
				{
					args.Sprite.LayerSetRSI(PlantHolderLayers.Plant, text);
					args.Sprite.LayerSetState(PlantHolderLayers.Plant, text2);
				}
			}
		}
	}
}
