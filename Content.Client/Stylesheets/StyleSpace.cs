using System;
using System.Linq;
using System.Runtime.CompilerServices;
using Content.Client.Resources;
using Robust.Client.Graphics;
using Robust.Client.ResourceManagement;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.Controls;
using Robust.Shared.Maths;

namespace Content.Client.Stylesheets
{
	// Token: 0x02000115 RID: 277
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class StyleSpace : StyleBase
	{
		// Token: 0x17000163 RID: 355
		// (get) Token: 0x060007B6 RID: 1974 RVA: 0x0002C650 File Offset: 0x0002A850
		public override Stylesheet Stylesheet { get; }

		// Token: 0x060007B7 RID: 1975 RVA: 0x0002C658 File Offset: 0x0002A858
		public StyleSpace(IResourceCache resCache) : base(resCache)
		{
			Font font = resCache.GetFont(new string[]
			{
				"/Fonts/NotoSans/NotoSans-Regular.ttf",
				"/Fonts/NotoSans/NotoSansSymbols-Regular.ttf",
				"/Fonts/NotoSans/NotoSansSymbols2-Regular.ttf"
			}, 10);
			Font font2 = resCache.GetFont(new string[]
			{
				"/Fonts/NotoSans/NotoSans-Bold.ttf",
				"/Fonts/NotoSans/NotoSansSymbols-Regular.ttf",
				"/Fonts/NotoSans/NotoSansSymbols2-Regular.ttf"
			}, 16);
			StyleBoxFlat styleBoxFlat = new StyleBoxFlat
			{
				BackgroundColor = new Color(0.25f, 0.25f, 0.25f, 1f)
			};
			styleBoxFlat.SetContentMarginOverride(3, 5f);
			StyleBoxFlat styleBoxFlat2 = new StyleBoxFlat
			{
				BackgroundColor = new Color(0.25f, 0.5f, 0.25f, 1f)
			};
			styleBoxFlat2.SetContentMarginOverride(3, 5f);
			Texture texture = resCache.GetTexture("/Textures/Interface/Nano/inverted_triangle.svg.png");
			this.Stylesheet = new Stylesheet(base.BaseRules.Concat(new StyleRule[]
			{
				StylesheetHelpers.Element<Label>().Class("LabelHeading").Prop("font", font2).Prop("font-color", StyleSpace.SpaceRed),
				StylesheetHelpers.Element<Label>().Class("LabelSubText").Prop("font", font).Prop("font-color", Color.DarkGray),
				StylesheetHelpers.Element<PanelContainer>().Class("HighDivider").Prop("panel", new StyleBoxFlat
				{
					BackgroundColor = StyleSpace.SpaceRed,
					ContentMarginBottomOverride = new float?((float)2),
					ContentMarginLeftOverride = new float?((float)2)
				}),
				StylesheetHelpers.Element<PanelContainer>().Class("LowDivider").Prop("panel", new StyleBoxFlat
				{
					BackgroundColor = Color.FromHex("#444", null),
					ContentMarginLeftOverride = new float?((float)2),
					ContentMarginBottomOverride = new float?((float)2)
				}),
				StylesheetHelpers.Element<ContainerButton>().Class("button").Prop("stylebox", base.BaseButton),
				StylesheetHelpers.Element<ContainerButton>().Class("button").Class("OpenRight").Prop("stylebox", base.BaseButtonOpenRight),
				StylesheetHelpers.Element<ContainerButton>().Class("button").Class("OpenLeft").Prop("stylebox", base.BaseButtonOpenLeft),
				StylesheetHelpers.Element<ContainerButton>().Class("button").Class("OpenBoth").Prop("stylebox", base.BaseButtonOpenBoth),
				StylesheetHelpers.Element<ContainerButton>().Class("button").Class("ButtonSquare").Prop("stylebox", base.BaseButtonSquare),
				StylesheetHelpers.Element<ContainerButton>().Class("button").Pseudo("normal").Prop("modulate-self", StyleSpace.ButtonColorDefault),
				StylesheetHelpers.Element<ContainerButton>().Class("button").Pseudo("hover").Prop("modulate-self", StyleSpace.ButtonColorHovered),
				StylesheetHelpers.Element<ContainerButton>().Class("button").Pseudo("pressed").Prop("modulate-self", StyleSpace.ButtonColorPressed),
				StylesheetHelpers.Element<ContainerButton>().Class("button").Pseudo("disabled").Prop("modulate-self", StyleSpace.ButtonColorDisabled),
				StylesheetHelpers.Element<ContainerButton>().Class("button").Class("Caution").Pseudo("normal").Prop("modulate-self", StyleSpace.ButtonColorCautionDefault),
				StylesheetHelpers.Element<ContainerButton>().Class("button").Class("Caution").Pseudo("hover").Prop("modulate-self", StyleSpace.ButtonColorCautionHovered),
				StylesheetHelpers.Element<ContainerButton>().Class("button").Class("Caution").Pseudo("pressed").Prop("modulate-self", StyleSpace.ButtonColorCautionPressed),
				StylesheetHelpers.Element<ContainerButton>().Class("button").Class("Caution").Pseudo("disabled").Prop("modulate-self", StyleSpace.ButtonColorCautionDisabled),
				StylesheetHelpers.Element<Label>().Class("button").Prop("alignMode", 1),
				StylesheetHelpers.Element<PanelContainer>().Class("AngleRect").Prop("panel", base.BaseAngleRect).Prop("modulate-self", Color.FromHex("#202030", null)),
				StylesheetHelpers.Child().Parent(StylesheetHelpers.Element<Button>().Class("disabled")).Child(StylesheetHelpers.Element<Label>()).Prop("font-color", Color.FromHex("#E5E5E581", null)),
				StylesheetHelpers.Element<ProgressBar>().Prop("background", styleBoxFlat).Prop("foreground", styleBoxFlat2),
				StylesheetHelpers.Element<OptionButton>().Prop("stylebox", base.BaseButton),
				StylesheetHelpers.Element<OptionButton>().Pseudo("normal").Prop("modulate-self", StyleSpace.ButtonColorDefault),
				StylesheetHelpers.Element<OptionButton>().Pseudo("hover").Prop("modulate-self", StyleSpace.ButtonColorHovered),
				StylesheetHelpers.Element<OptionButton>().Pseudo("pressed").Prop("modulate-self", StyleSpace.ButtonColorPressed),
				StylesheetHelpers.Element<OptionButton>().Pseudo("disabled").Prop("modulate-self", StyleSpace.ButtonColorDisabled),
				StylesheetHelpers.Element<TextureRect>().Class("optionTriangle").Prop("texture", texture),
				StylesheetHelpers.Element<Label>().Class("optionButton").Prop("alignMode", 1)
			}).ToList<StyleRule>());
		}

		// Token: 0x040003E3 RID: 995
		public static readonly Color SpaceRed = Color.FromHex("#9b2236", null);

		// Token: 0x040003E4 RID: 996
		public static readonly Color ButtonColorDefault = Color.FromHex("#464946", null);

		// Token: 0x040003E5 RID: 997
		public static readonly Color ButtonColorHovered = Color.FromHex("#575b61", null);

		// Token: 0x040003E6 RID: 998
		public static readonly Color ButtonColorPressed = Color.FromHex("#3e6c45", null);

		// Token: 0x040003E7 RID: 999
		public static readonly Color ButtonColorDisabled = Color.FromHex("#303133", null);

		// Token: 0x040003E8 RID: 1000
		public static readonly Color ButtonColorCautionDefault = Color.FromHex("#ab3232", null);

		// Token: 0x040003E9 RID: 1001
		public static readonly Color ButtonColorCautionHovered = Color.FromHex("#cf2f2f", null);

		// Token: 0x040003EA RID: 1002
		public static readonly Color ButtonColorCautionPressed = Color.FromHex("#3e6c45", null);

		// Token: 0x040003EB RID: 1003
		public static readonly Color ButtonColorCautionDisabled = Color.FromHex("#602a2a", null);
	}
}
