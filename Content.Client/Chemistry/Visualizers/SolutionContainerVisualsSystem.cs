using System;
using System.Runtime.CompilerServices;
using Content.Shared.Chemistry;
using Content.Shared.Chemistry.Reagent;
using Robust.Client.GameObjects;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Localization;
using Robust.Shared.Log;
using Robust.Shared.Maths;
using Robust.Shared.Prototypes;
using Robust.Shared.Utility;

namespace Content.Client.Chemistry.Visualizers
{
	// Token: 0x020003CC RID: 972
	[NullableContext(1)]
	[Nullable(new byte[]
	{
		0,
		1
	})]
	public sealed class SolutionContainerVisualsSystem : VisualizerSystem<SolutionContainerVisualsComponent>
	{
		// Token: 0x060017F4 RID: 6132 RVA: 0x00089A77 File Offset: 0x00087C77
		public override void Initialize()
		{
			base.Initialize();
			base.SubscribeLocalEvent<SolutionContainerVisualsComponent, MapInitEvent>(new ComponentEventHandler<SolutionContainerVisualsComponent, MapInitEvent>(this.OnMapInit), null, null);
		}

		// Token: 0x060017F5 RID: 6133 RVA: 0x00089A94 File Offset: 0x00087C94
		private void OnMapInit(EntityUid uid, SolutionContainerVisualsComponent component, MapInitEvent args)
		{
			MetaDataComponent metaDataComponent = base.MetaData(uid);
			component.InitialName = metaDataComponent.EntityName;
			component.InitialDescription = metaDataComponent.EntityDescription;
		}

		// Token: 0x060017F6 RID: 6134 RVA: 0x00089AC4 File Offset: 0x00087CC4
		protected override void OnAppearanceChange(EntityUid uid, SolutionContainerVisualsComponent component, ref AppearanceChangeEvent args)
		{
			float num;
			if (!this.AppearanceSystem.TryGetData<float>(uid, SolutionContainerVisuals.FillFraction, ref num, args.Component))
			{
				return;
			}
			if (args.Sprite == null)
			{
				return;
			}
			int num2;
			if (!args.Sprite.LayerMapTryGet(component.FillLayer, ref num2, false))
			{
				return;
			}
			if (num > 1f)
			{
				Logger.Error("Attempted to set solution container visuals volume ratio on " + base.ToPrettyString(uid) + " to a value greater than 1. Volume should never be greater than max volume!");
				num = 1f;
			}
			int num3;
			if (component.Metamorphic && args.Sprite.LayerMapTryGet(component.BaseLayer, ref num3, false))
			{
				int num4;
				bool flag = args.Sprite.LayerMapTryGet(component.OverlayLayer, ref num4, false);
				string text;
				if (this.AppearanceSystem.TryGetData<string>(uid, SolutionContainerVisuals.BaseOverride, ref text, args.Component))
				{
					ReagentPrototype reagentPrototype;
					this._prototype.TryIndex<ReagentPrototype>(text, ref reagentPrototype);
					MetaDataComponent metaDataComponent = base.MetaData(uid);
					SpriteSpecifier spriteSpecifier = (reagentPrototype != null) ? reagentPrototype.MetamorphicSprite : null;
					if (spriteSpecifier != null)
					{
						args.Sprite.LayerSetSprite(num3, spriteSpecifier);
						args.Sprite.LayerSetVisible(num2, false);
						if (flag)
						{
							args.Sprite.LayerSetVisible(num4, false);
						}
						metaDataComponent.EntityName = Loc.GetString(component.MetamorphicNameFull, new ValueTuple<string, object>[]
						{
							new ValueTuple<string, object>("name", reagentPrototype.LocalizedName)
						});
						metaDataComponent.EntityDescription = reagentPrototype.LocalizedDescription;
						return;
					}
					if (flag)
					{
						args.Sprite.LayerSetVisible(num4, true);
					}
					args.Sprite.LayerSetSprite(num3, component.MetamorphicDefaultSprite);
					metaDataComponent.EntityName = component.InitialName;
					metaDataComponent.EntityDescription = component.InitialDescription;
				}
			}
			int num5 = (int)Math.Round((double)(num * (float)component.MaxFillLevels));
			if (num5 > 0)
			{
				if (component.FillBaseName == null)
				{
					return;
				}
				args.Sprite.LayerSetVisible(num2, true);
				string text2 = component.FillBaseName + num5.ToString();
				args.Sprite.LayerSetState(num2, text2);
				Color color;
				if (component.ChangeColor && this.AppearanceSystem.TryGetData<Color>(uid, SolutionContainerVisuals.Color, ref color, args.Component))
				{
					args.Sprite.LayerSetColor(num2, color);
					return;
				}
			}
			else
			{
				if (component.EmptySpriteName == null)
				{
					args.Sprite.LayerSetVisible(num2, false);
					return;
				}
				args.Sprite.LayerSetState(num2, component.EmptySpriteName);
				if (component.ChangeColor)
				{
					args.Sprite.LayerSetColor(num2, component.EmptySpriteColor);
				}
			}
		}

		// Token: 0x04000C45 RID: 3141
		[Dependency]
		private readonly IPrototypeManager _prototype;
	}
}
