using System;
using System.Linq;
using System.Runtime.CompilerServices;
using Content.Client.ContextMenu.UI;
using Content.Client.Examine;
using Content.Client.PDA;
using Content.Client.Resources;
using Content.Client.Targeting.UI;
using Content.Client.UserInterface.Controls;
using Content.Shared.Verbs;
using Robust.Client.Graphics;
using Robust.Client.ResourceManagement;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.Controls;
using Robust.Client.UserInterface.CustomControls;
using Robust.Shared.Maths;

namespace Content.Client.Stylesheets
{
	// Token: 0x02000113 RID: 275
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class StyleNano : StyleBase
	{
		// Token: 0x17000160 RID: 352
		// (get) Token: 0x060007AD RID: 1965 RVA: 0x0002871C File Offset: 0x0002691C
		public override Stylesheet Stylesheet { get; }

		// Token: 0x060007AE RID: 1966 RVA: 0x00028724 File Offset: 0x00026924
		public StyleNano(IResourceCache resCache) : base(resCache)
		{
			Font font = resCache.NotoStack("Regular", 10, false);
			resCache.NotoStack("Italic", 10, false);
			Font font2 = resCache.NotoStack("Regular", 12, false);
			Font font3 = resCache.NotoStack("Italic", 12, false);
			Font font4 = resCache.NotoStack("Bold", 12, false);
			Font font5 = resCache.NotoStack("BoldItalic", 12, false);
			resCache.NotoStack("BoldItalic", 14, false);
			resCache.NotoStack("BoldItalic", 16, false);
			Font font6 = resCache.NotoStack("Bold", 14, true);
			Font font7 = resCache.NotoStack("Bold", 16, true);
			Font font8 = resCache.NotoStack("Regular", 15, false);
			Font font9 = resCache.NotoStack("Regular", 16, false);
			Font font10 = resCache.NotoStack("Bold", 16, false);
			Font font11 = resCache.NotoStack("Bold", 18, false);
			Font font12 = resCache.NotoStack("Bold", 20, false);
			Texture texture = resCache.GetTexture("/Textures/Interface/Nano/window_header.png");
			StyleBoxTexture styleBoxTexture = new StyleBoxTexture
			{
				Texture = texture,
				PatchMarginBottom = 3f,
				ExpandMarginBottom = 3f,
				ContentMarginBottomOverride = new float?(0f)
			};
			Texture texture2 = resCache.GetTexture("/Textures/Interface/Nano/window_header_alert.png");
			StyleBoxTexture styleBoxTexture2 = new StyleBoxTexture
			{
				Texture = texture2,
				PatchMarginBottom = 3f,
				ExpandMarginBottom = 3f,
				ContentMarginBottomOverride = new float?(0f)
			};
			Texture texture3 = resCache.GetTexture("/Textures/Interface/Nano/window_background.png");
			StyleBoxTexture styleBoxTexture3 = new StyleBoxTexture
			{
				Texture = texture3
			};
			styleBoxTexture3.SetPatchMargin(14, 2f);
			styleBoxTexture3.SetExpandMargin(14, 2f);
			Texture texture4 = resCache.GetTexture("/Textures/Interface/Nano/window_background_bordered.png");
			StyleBoxTexture styleBoxTexture4 = new StyleBoxTexture
			{
				Texture = texture4
			};
			styleBoxTexture4.SetPatchMargin(15, 2f);
			StyleBoxTexture styleBoxTexture5 = new StyleBoxTexture
			{
				Texture = texture4
			};
			styleBoxTexture5.SetPatchMargin(15, 2f);
			Texture texture5 = resCache.GetTexture("/Textures/Interface/Inventory/inv_slot_background.png");
			StyleBoxTexture styleBoxTexture6 = new StyleBoxTexture
			{
				Texture = texture5
			};
			styleBoxTexture6.SetPatchMargin(15, 2f);
			styleBoxTexture6.SetContentMarginOverride(15, 0f);
			Texture texture6 = resCache.GetTexture("/Textures/Interface/Inventory/hand_slot_highlight.png");
			StyleBoxTexture styleBoxTexture7 = new StyleBoxTexture
			{
				Texture = texture6
			};
			styleBoxTexture7.SetPatchMargin(15, 2f);
			Texture texture7 = resCache.GetTexture("/Textures/Interface/Nano/transparent_window_background_bordered.png");
			StyleBoxTexture styleBoxTexture8 = new StyleBoxTexture
			{
				Texture = texture7
			};
			styleBoxTexture8.SetPatchMargin(15, 2f);
			StyleBoxTexture styleBoxTexture9 = new StyleBoxTexture
			{
				Texture = texture4
			};
			styleBoxTexture9.SetPatchMargin(15, 2f);
			styleBoxTexture9.SetExpandMargin(15, 4f);
			StyleBoxTexture styleBoxTexture10 = new StyleBoxTexture(base.BaseButton);
			styleBoxTexture10.SetPatchMargin(15, 10f);
			styleBoxTexture10.SetPadding(15, 0f);
			styleBoxTexture10.SetContentMarginOverride(3, 0f);
			styleBoxTexture10.SetContentMarginOverride(12, 4f);
			StyleBoxTexture styleBoxTexture11 = new StyleBoxTexture
			{
				Texture = Texture.White
			};
			Texture texture8 = resCache.GetTexture("/Textures/Interface/Nano/light_panel_background_bordered.png");
			StyleBoxTexture styleBoxTexture12 = new StyleBoxTexture(base.BaseButton);
			styleBoxTexture12.Texture = texture8;
			styleBoxTexture12.SetPatchMargin(15, 2f);
			styleBoxTexture12.SetPadding(15, 2f);
			styleBoxTexture12.SetContentMarginOverride(3, 2f);
			styleBoxTexture12.SetContentMarginOverride(12, 2f);
			new StyleBoxTexture(styleBoxTexture12).Modulate = StyleNano.ButtonColorHovered;
			new StyleBoxTexture(styleBoxTexture12).Modulate = StyleNano.ButtonColorPressed;
			new StyleBoxTexture(styleBoxTexture12).Modulate = StyleNano.ButtonColorDisabled;
			Texture texture9 = resCache.GetTexture("/Textures/Interface/Nano/black_panel_light_thin_border.png");
			Texture texture10 = resCache.GetTexture("/Textures/Interface/Nano/black_panel_red_thin_border.png");
			StyleBoxTexture styleBoxTexture13 = new StyleBoxTexture(base.BaseButton);
			styleBoxTexture13.Texture = texture9;
			styleBoxTexture13.SetPatchMargin(15, 2f);
			styleBoxTexture13.SetPadding(15, 2f);
			styleBoxTexture13.SetContentMarginOverride(3, 2f);
			styleBoxTexture13.SetContentMarginOverride(12, 2f);
			new StyleBoxTexture(styleBoxTexture13).Texture = texture10;
			new StyleBoxTexture(styleBoxTexture13).Modulate = StyleNano.ButtonColorHovered;
			new StyleBoxTexture(styleBoxTexture13).Modulate = StyleNano.ButtonColorPressed;
			Texture texture11 = resCache.GetTexture("/Textures/Interface/Nano/button.svg.96dpi.png");
			StyleBoxTexture styleBoxTexture14 = new StyleBoxTexture();
			styleBoxTexture14.Texture = texture11;
			styleBoxTexture14.SetPatchMargin(15, 10f);
			styleBoxTexture14.SetPadding(15, 0f);
			styleBoxTexture14.SetContentMarginOverride(15, 0f);
			StyleBoxTexture styleBoxTexture15 = new StyleBoxTexture(styleBoxTexture14)
			{
				Texture = new AtlasTexture(texture11, UIBox2.FromDimensions(new ValueTuple<float, float>(0f, 0f), new ValueTuple<float, float>(14f, 24f)))
			};
			styleBoxTexture15.SetPatchMargin(4, 0f);
			StyleBoxTexture styleBoxTexture16 = new StyleBoxTexture(styleBoxTexture14)
			{
				Texture = new AtlasTexture(texture11, UIBox2.FromDimensions(new ValueTuple<float, float>(10f, 0f), new ValueTuple<float, float>(14f, 24f)))
			};
			styleBoxTexture16.SetPatchMargin(8, 0f);
			StyleBoxTexture styleBoxTexture17 = new StyleBoxTexture(styleBoxTexture14)
			{
				Texture = new AtlasTexture(texture11, UIBox2.FromDimensions(new ValueTuple<float, float>(10f, 0f), new ValueTuple<float, float>(3f, 24f)))
			};
			styleBoxTexture17.SetPatchMargin(12, 0f);
			Texture texture12 = resCache.GetTexture("/Textures/Interface/Nano/rounded_button.svg.96dpi.png");
			StyleBoxTexture styleBoxTexture18 = new StyleBoxTexture
			{
				Texture = texture12
			};
			styleBoxTexture18.SetPatchMargin(15, 5f);
			styleBoxTexture18.SetPadding(15, 2f);
			Texture texture13 = resCache.GetTexture("/Textures/Interface/Nano/rounded_button_bordered.svg.96dpi.png");
			StyleBoxTexture styleBoxTexture19 = new StyleBoxTexture
			{
				Texture = texture13
			};
			styleBoxTexture19.SetPatchMargin(15, 5f);
			styleBoxTexture19.SetPadding(15, 2f);
			Texture texture14 = resCache.GetTexture("/Textures/Interface/Nano/inverted_triangle.svg.png");
			Texture texture15 = resCache.GetTexture("/Textures/Interface/Nano/lineedit.png");
			StyleBoxTexture styleBoxTexture20 = new StyleBoxTexture
			{
				Texture = texture15
			};
			styleBoxTexture20.SetPatchMargin(15, 3f);
			styleBoxTexture20.SetContentMarginOverride(12, 5f);
			Texture texture16 = resCache.GetTexture("/Textures/Interface/Nano/chat_sub_background.png");
			StyleBoxTexture styleBoxTexture21 = new StyleBoxTexture
			{
				Texture = texture16
			};
			styleBoxTexture21.SetPatchMargin(15, 2f);
			Texture texture17 = resCache.GetTexture("/Textures/Interface/Nano/black_panel_dark_thin_border.png");
			StyleBoxTexture styleBoxTexture22 = new StyleBoxTexture
			{
				Texture = texture17
			};
			styleBoxTexture22.SetPatchMargin(15, 3f);
			styleBoxTexture22.SetContentMarginOverride(12, 5f);
			Texture texture18 = resCache.GetTexture("/Textures/Interface/Nano/tabcontainer_panel.png");
			StyleBoxTexture styleBoxTexture23 = new StyleBoxTexture
			{
				Texture = texture18
			};
			styleBoxTexture23.SetPatchMargin(15, 2f);
			StyleBoxFlat styleBoxFlat = new StyleBoxFlat
			{
				BackgroundColor = new Color(64, 64, 64, byte.MaxValue)
			};
			styleBoxFlat.SetContentMarginOverride(12, 5f);
			StyleBoxFlat styleBoxFlat2 = new StyleBoxFlat
			{
				BackgroundColor = new Color(32, 32, 32, byte.MaxValue)
			};
			styleBoxFlat2.SetContentMarginOverride(12, 5f);
			StyleBoxFlat styleBoxFlat3 = new StyleBoxFlat
			{
				BackgroundColor = new Color(0.25f, 0.25f, 0.25f, 1f)
			};
			styleBoxFlat3.SetContentMarginOverride(3, 5f);
			StyleBoxFlat styleBoxFlat4 = new StyleBoxFlat
			{
				BackgroundColor = new Color(0.25f, 0.5f, 0.25f, 1f)
			};
			styleBoxFlat4.SetContentMarginOverride(3, 5f);
			Texture texture19 = resCache.GetTexture("/Textures/Interface/Nano/checkbox_checked.svg.96dpi.png");
			Texture texture20 = resCache.GetTexture("/Textures/Interface/Nano/checkbox_unchecked.svg.96dpi.png");
			Texture texture21 = resCache.GetTexture("/Textures/Interface/Nano/tooltip.png");
			StyleBoxTexture styleBoxTexture24 = new StyleBoxTexture
			{
				Texture = texture21
			};
			styleBoxTexture24.SetPatchMargin(15, 2f);
			styleBoxTexture24.SetContentMarginOverride(12, 7f);
			Texture texture22 = resCache.GetTexture("/Textures/Interface/Nano/whisper.png");
			StyleBoxTexture styleBoxTexture25 = new StyleBoxTexture
			{
				Texture = texture22
			};
			styleBoxTexture25.SetPatchMargin(15, 2f);
			styleBoxTexture25.SetContentMarginOverride(12, 7f);
			Texture texture23 = resCache.GetTexture("/Textures/Interface/Nano/placeholder.png");
			StyleBoxTexture styleBoxTexture26 = new StyleBoxTexture
			{
				Texture = texture23
			};
			styleBoxTexture26.SetPatchMargin(15, 19f);
			styleBoxTexture26.SetExpandMargin(15, -5f);
			styleBoxTexture26.Mode = 1;
			StyleBoxFlat styleBoxFlat5 = new StyleBoxFlat
			{
				BackgroundColor = new Color(75, 75, 75, byte.MaxValue)
			};
			styleBoxFlat5.SetContentMarginOverride(3, 2f);
			styleBoxFlat5.SetContentMarginOverride(12, 4f);
			StyleBoxFlat styleBoxFlat6 = new StyleBoxFlat
			{
				BackgroundColor = new Color(10, 10, 10, byte.MaxValue)
			};
			styleBoxFlat6.SetContentMarginOverride(3, 2f);
			styleBoxFlat6.SetContentMarginOverride(12, 4f);
			StyleBoxFlat styleBoxFlat7 = new StyleBoxFlat
			{
				BackgroundColor = new Color(55, 55, 55, byte.MaxValue)
			};
			styleBoxFlat7.SetContentMarginOverride(3, 2f);
			styleBoxFlat7.SetContentMarginOverride(12, 4f);
			StyleBoxFlat styleBoxFlat8 = new StyleBoxFlat
			{
				BackgroundColor = Color.Transparent
			};
			styleBoxFlat8.SetContentMarginOverride(3, 2f);
			styleBoxFlat8.SetContentMarginOverride(12, 4f);
			Texture texture24 = resCache.GetTexture("/Textures/Interface/Nano/square.png");
			StyleBoxTexture styleBoxTexture27 = new StyleBoxTexture
			{
				Texture = texture24,
				ContentMarginLeftOverride = new float?((float)10)
			};
			Texture texture25 = resCache.GetTexture("/Textures/Interface/Nano/nanoheading.svg.96dpi.png");
			StyleBoxTexture styleBoxTexture28 = new StyleBoxTexture
			{
				Texture = texture25,
				PatchMarginRight = 10f,
				PatchMarginTop = 10f,
				ContentMarginTopOverride = new float?((float)2),
				ContentMarginLeftOverride = new float?((float)10),
				PaddingTop = 4f
			};
			styleBoxTexture28.SetPatchMargin(10, 2f);
			Texture texture26 = resCache.GetTexture("/Textures/Interface/Nano/stripeback.svg.96dpi.png");
			StyleBoxTexture styleBoxTexture29 = new StyleBoxTexture
			{
				Texture = texture26,
				Mode = 1
			};
			Texture texture27 = resCache.GetTexture("/Textures/Interface/Nano/slider_outline.svg.96dpi.png");
			Texture texture28 = resCache.GetTexture("/Textures/Interface/Nano/slider_fill.svg.96dpi.png");
			Texture texture29 = resCache.GetTexture("/Textures/Interface/Nano/slider_grabber.svg.96dpi.png");
			StyleBoxTexture styleBoxTexture30 = new StyleBoxTexture
			{
				Texture = texture28,
				Modulate = Color.FromHex("#3E6C45", null)
			};
			StyleBoxTexture styleBoxTexture31 = new StyleBoxTexture
			{
				Texture = texture28,
				Modulate = Color.FromHex("#1E1E22", null)
			};
			StyleBoxTexture styleBoxTexture32 = new StyleBoxTexture
			{
				Texture = texture27,
				Modulate = Color.FromHex("#494949", null)
			};
			StyleBoxTexture styleBoxTexture33 = new StyleBoxTexture
			{
				Texture = texture29
			};
			styleBoxTexture30.SetPatchMargin(15, 12f);
			styleBoxTexture31.SetPatchMargin(15, 12f);
			styleBoxTexture32.SetPatchMargin(15, 12f);
			styleBoxTexture33.SetPatchMargin(15, 12f);
			StyleBoxTexture styleBoxTexture34 = new StyleBoxTexture(styleBoxTexture30)
			{
				Modulate = Color.LimeGreen
			};
			StyleBoxTexture styleBoxTexture35 = new StyleBoxTexture(styleBoxTexture30)
			{
				Modulate = Color.Red
			};
			StyleBoxTexture styleBoxTexture36 = new StyleBoxTexture(styleBoxTexture30)
			{
				Modulate = Color.Blue
			};
			StyleBoxTexture styleBoxTexture37 = new StyleBoxTexture(styleBoxTexture30)
			{
				Modulate = Color.White
			};
			Font font13 = resCache.GetFont("/Fonts/Boxfont-round/Boxfont Round.ttf", 13);
			new StyleBoxTexture
			{
				Texture = texture11,
				Modulate = Color.FromHex("#202023", null)
			}.SetPatchMargin(15, 10f);
			StyleBoxTexture styleBoxTexture38 = new StyleBoxTexture
			{
				Texture = resCache.GetTexture("/Textures/Interface/Paper/paper_background_default.svg.96dpi.png"),
				Modulate = Color.FromHex("#eaedde", null)
			};
			styleBoxTexture38.SetPatchMargin(15, 16f);
			Texture texture30 = resCache.GetTexture("/Textures/Interface/VerbIcons/group.svg.192dpi.png");
			Texture texture31 = resCache.GetTexture("/Textures/Interface/VerbIcons/group.svg.192dpi.png");
			Texture texture32 = resCache.GetTexture("/Textures/Interface/VerbIcons/drop.svg.192dpi.png");
			Texture texture33 = resCache.GetTexture("/Textures/Interface/VerbIcons/information.svg.192dpi.png");
			Texture texture34 = resCache.GetTexture("/Textures/Interface/VerbIcons/dot.svg.192dpi.png");
			this.Stylesheet = new Stylesheet(base.BaseRules.Concat(new StyleRule[]
			{
				new StyleRule(new SelectorElement(typeof(Label), new string[]
				{
					"windowTitle"
				}, null, null), new StyleProperty[]
				{
					new StyleProperty("font-color", StyleNano.NanoGold),
					new StyleProperty("font", font6)
				}),
				new StyleRule(new SelectorElement(typeof(Label), new string[]
				{
					"windowTitleAlert"
				}, null, null), new StyleProperty[]
				{
					new StyleProperty("font-color", Color.White),
					new StyleProperty("font", font6)
				}),
				new StyleRule(new SelectorElement(null, new string[]
				{
					"windowPanel"
				}, null, null), new StyleProperty[]
				{
					new StyleProperty("panel", styleBoxTexture3)
				}),
				new StyleRule(new SelectorElement(null, new string[]
				{
					"BorderedWindowPanel"
				}, null, null), new StyleProperty[]
				{
					new StyleProperty("panel", styleBoxTexture4)
				}),
				new StyleRule(new SelectorElement(null, new string[]
				{
					"TransparentBorderedWindowPanel"
				}, null, null), new StyleProperty[]
				{
					new StyleProperty("panel", styleBoxTexture8)
				}),
				new StyleRule(new SelectorElement(null, new string[]
				{
					"InventorySlotBackground"
				}, null, null), new StyleProperty[]
				{
					new StyleProperty("panel", styleBoxTexture6)
				}),
				new StyleRule(new SelectorElement(null, new string[]
				{
					"HandSlotHighlight"
				}, null, null), new StyleProperty[]
				{
					new StyleProperty("panel", styleBoxTexture7)
				}),
				new StyleRule(new SelectorElement(typeof(PanelContainer), new string[]
				{
					"HotbarPanel"
				}, null, null), new StyleProperty[]
				{
					new StyleProperty("panel", styleBoxTexture9)
				}),
				new StyleRule(new SelectorElement(typeof(PanelContainer), new string[]
				{
					"windowHeader"
				}, null, null), new StyleProperty[]
				{
					new StyleProperty("panel", styleBoxTexture)
				}),
				new StyleRule(new SelectorElement(typeof(PanelContainer), new string[]
				{
					"windowHeaderAlert"
				}, null, null), new StyleProperty[]
				{
					new StyleProperty("panel", styleBoxTexture2)
				}),
				StylesheetHelpers.Element<ContainerButton>().Class("button").Prop("stylebox", base.BaseButton),
				StylesheetHelpers.Element<ContainerButton>().Class("button").Class("OpenRight").Prop("stylebox", base.BaseButtonOpenRight),
				StylesheetHelpers.Element<ContainerButton>().Class("button").Class("OpenLeft").Prop("stylebox", base.BaseButtonOpenLeft),
				StylesheetHelpers.Element<ContainerButton>().Class("button").Class("OpenBoth").Prop("stylebox", base.BaseButtonOpenBoth),
				StylesheetHelpers.Element<ContainerButton>().Class("button").Class("ButtonSquare").Prop("stylebox", base.BaseButtonSquare),
				new StyleRule(new SelectorElement(typeof(Label), new string[]
				{
					"button"
				}, null, null), new StyleProperty[]
				{
					new StyleProperty("alignMode", 1)
				}),
				StylesheetHelpers.Element<ContainerButton>().Class("button").Pseudo("normal").Prop("modulate-self", StyleNano.ButtonColorDefault),
				StylesheetHelpers.Element<ContainerButton>().Class("button").Pseudo("hover").Prop("modulate-self", StyleNano.ButtonColorHovered),
				StylesheetHelpers.Element<ContainerButton>().Class("button").Pseudo("pressed").Prop("modulate-self", StyleNano.ButtonColorPressed),
				StylesheetHelpers.Element<ContainerButton>().Class("button").Pseudo("disabled").Prop("modulate-self", StyleNano.ButtonColorDisabled),
				StylesheetHelpers.Element<ContainerButton>().Class("button").Class("Caution").Pseudo("normal").Prop("modulate-self", StyleNano.ButtonColorCautionDefault),
				StylesheetHelpers.Element<ContainerButton>().Class("button").Class("Caution").Pseudo("hover").Prop("modulate-self", StyleNano.ButtonColorCautionHovered),
				StylesheetHelpers.Element<ContainerButton>().Class("button").Class("Caution").Pseudo("pressed").Prop("modulate-self", StyleNano.ButtonColorCautionPressed),
				StylesheetHelpers.Element<ContainerButton>().Class("button").Class("Caution").Pseudo("disabled").Prop("modulate-self", StyleNano.ButtonColorCautionDisabled),
				new StyleRule(new SelectorChild(new SelectorElement(typeof(Button), null, null, new string[]
				{
					"disabled"
				}), new SelectorElement(typeof(Label), null, null, null)), new StyleProperty[]
				{
					new StyleProperty("font-color", Color.FromHex("#E5E5E581", null))
				}),
				StylesheetHelpers.Element<PanelContainer>().Class("contextMenuPopup").Prop("panel", styleBoxTexture5),
				StylesheetHelpers.Element<ContextMenuElement>().Class("contextMenuButton").Prop("stylebox", styleBoxTexture11),
				StylesheetHelpers.Element<ContextMenuElement>().Class("contextMenuButton").Pseudo("normal").Prop("modulate-self", StyleNano.ButtonColorContext),
				StylesheetHelpers.Element<ContextMenuElement>().Class("contextMenuButton").Pseudo("hover").Prop("modulate-self", StyleNano.ButtonColorContextHover),
				StylesheetHelpers.Element<ContextMenuElement>().Class("contextMenuButton").Pseudo("pressed").Prop("modulate-self", StyleNano.ButtonColorContextPressed),
				StylesheetHelpers.Element<ContextMenuElement>().Class("contextMenuButton").Pseudo("disabled").Prop("modulate-self", StyleNano.ButtonColorContextDisabled),
				StylesheetHelpers.Element<RichTextLabel>().Class(InteractionVerb.DefaultTextStyleClass).Prop("font", font5),
				StylesheetHelpers.Element<RichTextLabel>().Class(ActivationVerb.DefaultTextStyleClass).Prop("font", font4),
				StylesheetHelpers.Element<RichTextLabel>().Class(AlternativeVerb.DefaultTextStyleClass).Prop("font", font3),
				StylesheetHelpers.Element<RichTextLabel>().Class(Verb.DefaultTextStyleClass).Prop("font", font2),
				StylesheetHelpers.Element<TextureRect>().Class("contextMenuExpansionTexture").Prop("texture", texture30),
				StylesheetHelpers.Element<TextureRect>().Class("verbMenuConfirmationTexture").Prop("texture", texture31),
				StylesheetHelpers.Element<ContextMenuElement>().Class("confirmationContextMenuButton").Prop("stylebox", styleBoxTexture11),
				StylesheetHelpers.Element<ContextMenuElement>().Class("confirmationContextMenuButton").Pseudo("normal").Prop("modulate-self", StyleNano.ButtonColorCautionDefault),
				StylesheetHelpers.Element<ContextMenuElement>().Class("confirmationContextMenuButton").Pseudo("hover").Prop("modulate-self", StyleNano.ButtonColorCautionHovered),
				StylesheetHelpers.Element<ContextMenuElement>().Class("confirmationContextMenuButton").Pseudo("pressed").Prop("modulate-self", StyleNano.ButtonColorCautionPressed),
				StylesheetHelpers.Element<ContextMenuElement>().Class("confirmationContextMenuButton").Pseudo("disabled").Prop("modulate-self", StyleNano.ButtonColorCautionDisabled),
				StylesheetHelpers.Element<ExamineButton>().Class("examine-button").Prop("stylebox", styleBoxTexture11),
				StylesheetHelpers.Element<ExamineButton>().Class("examine-button").Pseudo("normal").Prop("modulate-self", StyleNano.ExamineButtonColorContext),
				StylesheetHelpers.Element<ExamineButton>().Class("examine-button").Pseudo("hover").Prop("modulate-self", StyleNano.ExamineButtonColorContextHover),
				StylesheetHelpers.Element<ExamineButton>().Class("examine-button").Pseudo("pressed").Prop("modulate-self", StyleNano.ExamineButtonColorContextPressed),
				StylesheetHelpers.Element<ExamineButton>().Class("examine-button").Pseudo("disabled").Prop("modulate-self", StyleNano.ExamineButtonColorContextDisabled),
				StylesheetHelpers.Element<DirectionIcon>().Class(DirectionIcon.StyleClassDirectionIconArrow).Prop("texture", texture32),
				StylesheetHelpers.Element<DirectionIcon>().Class(DirectionIcon.StyleClassDirectionIconUnknown).Prop("texture", texture33),
				StylesheetHelpers.Element<DirectionIcon>().Class(DirectionIcon.StyleClassDirectionIconHere).Prop("texture", texture34),
				StylesheetHelpers.Element<ContainerButton>().Class("storageButton").Prop("stylebox", styleBoxTexture10),
				StylesheetHelpers.Element<ContainerButton>().Class("storageButton").Pseudo("normal").Prop("modulate-self", StyleNano.ButtonColorDefault),
				StylesheetHelpers.Element<ContainerButton>().Class("storageButton").Pseudo("hover").Prop("modulate-self", StyleNano.ButtonColorHovered),
				StylesheetHelpers.Element<ContainerButton>().Class("storageButton").Pseudo("pressed").Prop("modulate-self", StyleNano.ButtonColorPressed),
				StylesheetHelpers.Element<ContainerButton>().Class("storageButton").Pseudo("disabled").Prop("modulate-self", StyleNano.ButtonColorDisabled),
				StylesheetHelpers.Element<ContainerButton>().Class("list-container-button").Prop("stylebox", styleBoxTexture27),
				StylesheetHelpers.Element<ContainerButton>().Class("list-container-button").Pseudo("normal").Prop("modulate-self", new Color(55, 55, 68, byte.MaxValue)),
				StylesheetHelpers.Element<ContainerButton>().Class("list-container-button").Pseudo("hover").Prop("modulate-self", new Color(75, 75, 86, byte.MaxValue)),
				StylesheetHelpers.Element<ContainerButton>().Class("list-container-button").Pseudo("pressed").Prop("modulate-self", new Color(75, 75, 86, byte.MaxValue)),
				StylesheetHelpers.Element<ContainerButton>().Class("list-container-button").Pseudo("disabled").Prop("modulate-self", new Color(10, 10, 12, byte.MaxValue)),
				new StyleRule(new SelectorChild(new SelectorElement(typeof(Button), null, "mainMenu", null), new SelectorElement(typeof(Label), null, null, null)), new StyleProperty[]
				{
					new StyleProperty("font", font10)
				}),
				new StyleRule(new SelectorElement(typeof(BoxContainer), null, "mainMenuVBox", null), new StyleProperty[]
				{
					new StyleProperty("separation", 2)
				}),
				new StyleRule(new SelectorElement(typeof(LineEdit), null, null, null), new StyleProperty[]
				{
					new StyleProperty("stylebox", styleBoxTexture20)
				}),
				new StyleRule(new SelectorElement(typeof(LineEdit), new string[]
				{
					"notEditable"
				}, null, null), new StyleProperty[]
				{
					new StyleProperty("font-color", new Color(192, 192, 192, byte.MaxValue))
				}),
				new StyleRule(new SelectorElement(typeof(LineEdit), null, null, new string[]
				{
					"placeholder"
				}), new StyleProperty[]
				{
					new StyleProperty("font-color", Color.FromHex("#C6B171", null))
				}),
				StylesheetHelpers.Element<TextEdit>().Pseudo("placeholder").Prop("font-color", Color.Gray),
				new StyleRule(new SelectorElement(typeof(LineEdit), new string[]
				{
					"chatLineEdit"
				}, null, null), new StyleProperty[]
				{
					new StyleProperty("stylebox", new StyleBoxEmpty())
				}),
				new StyleRule(new SelectorElement(typeof(PanelContainer), new string[]
				{
					"ChatSubPanel"
				}, null, null), new StyleProperty[]
				{
					new StyleProperty("panel", styleBoxTexture21)
				}),
				new StyleRule(new SelectorElement(typeof(LineEdit), new string[]
				{
					"actionSearchBox"
				}, null, null), new StyleProperty[]
				{
					new StyleProperty("stylebox", styleBoxTexture22)
				}),
				new StyleRule(new SelectorElement(typeof(TabContainer), null, null, null), new StyleProperty[]
				{
					new StyleProperty("panel-stylebox", styleBoxTexture23),
					new StyleProperty("tab-stylebox", styleBoxFlat),
					new StyleProperty("tab-stylebox-inactive", styleBoxFlat2)
				}),
				new StyleRule(new SelectorElement(typeof(ProgressBar), null, null, null), new StyleProperty[]
				{
					new StyleProperty("background", styleBoxFlat3),
					new StyleProperty("foreground", styleBoxFlat4)
				}),
				new StyleRule(new SelectorElement(typeof(TextureRect), new string[]
				{
					"checkBox"
				}, null, null), new StyleProperty[]
				{
					new StyleProperty("texture", texture20)
				}),
				new StyleRule(new SelectorElement(typeof(TextureRect), new string[]
				{
					"checkBox",
					"checkBoxChecked"
				}, null, null), new StyleProperty[]
				{
					new StyleProperty("texture", texture19)
				}),
				new StyleRule(new SelectorElement(typeof(BoxContainer), new string[]
				{
					"checkBox"
				}, null, null), new StyleProperty[]
				{
					new StyleProperty("separation", 10)
				}),
				new StyleRule(new SelectorElement(typeof(Tooltip), null, null, null), new StyleProperty[]
				{
					new StyleProperty("panel", styleBoxTexture24)
				}),
				new StyleRule(new SelectorElement(typeof(PanelContainer), new string[]
				{
					"tooltipBox"
				}, null, null), new StyleProperty[]
				{
					new StyleProperty("panel", styleBoxTexture24)
				}),
				new StyleRule(new SelectorElement(typeof(PanelContainer), new string[]
				{
					"speechBox",
					"sayBox"
				}, null, null), new StyleProperty[]
				{
					new StyleProperty("panel", styleBoxTexture24)
				}),
				new StyleRule(new SelectorElement(typeof(PanelContainer), new string[]
				{
					"speechBox",
					"whisperBox"
				}, null, null), new StyleProperty[]
				{
					new StyleProperty("panel", styleBoxTexture25)
				}),
				new StyleRule(new SelectorChild(new SelectorElement(typeof(PanelContainer), new string[]
				{
					"speechBox",
					"emoteBox"
				}, null, null), new SelectorElement(typeof(RichTextLabel), null, null, null)), new StyleProperty[]
				{
					new StyleProperty("font", font3)
				}),
				new StyleRule(new SelectorElement(typeof(RichTextLabel), new string[]
				{
					"tooltipAlertTitle"
				}, null, null), new StyleProperty[]
				{
					new StyleProperty("font", font11)
				}),
				new StyleRule(new SelectorElement(typeof(RichTextLabel), new string[]
				{
					"tooltipAlertDesc"
				}, null, null), new StyleProperty[]
				{
					new StyleProperty("font", font9)
				}),
				new StyleRule(new SelectorElement(typeof(RichTextLabel), new string[]
				{
					"tooltipAlertCooldown"
				}, null, null), new StyleProperty[]
				{
					new StyleProperty("font", font9)
				}),
				new StyleRule(new SelectorElement(typeof(RichTextLabel), new string[]
				{
					"tooltipActionTitle"
				}, null, null), new StyleProperty[]
				{
					new StyleProperty("font", font10)
				}),
				new StyleRule(new SelectorElement(typeof(RichTextLabel), new string[]
				{
					"tooltipActionDesc"
				}, null, null), new StyleProperty[]
				{
					new StyleProperty("font", font8)
				}),
				new StyleRule(new SelectorElement(typeof(RichTextLabel), new string[]
				{
					"tooltipActionCooldown"
				}, null, null), new StyleProperty[]
				{
					new StyleProperty("font", font8)
				}),
				new StyleRule(new SelectorElement(typeof(RichTextLabel), new string[]
				{
					"tooltipActionCooldown"
				}, null, null), new StyleProperty[]
				{
					new StyleProperty("font", font8)
				}),
				new StyleRule(new SelectorElement(typeof(Label), new string[]
				{
					"contextMenuCount"
				}, null, null), new StyleProperty[]
				{
					new StyleProperty("font", font),
					new StyleProperty("alignMode", 2)
				}),
				new StyleRule(new SelectorElement(typeof(RichTextLabel), new string[]
				{
					"hotbarSlotNumber"
				}, null, null), new StyleProperty[]
				{
					new StyleProperty("font", font7)
				}),
				new StyleRule(new SelectorElement(typeof(PanelContainer), new string[]
				{
					"entity-tooltip"
				}, null, null), new StyleProperty[]
				{
					new StyleProperty("panel", styleBoxTexture24)
				}),
				new StyleRule(new SelectorElement(typeof(ItemList), null, null, null), new StyleProperty[]
				{
					new StyleProperty("itemlist-background", new StyleBoxFlat
					{
						BackgroundColor = new Color(32, 32, 32, byte.MaxValue)
					}),
					new StyleProperty("item-background", styleBoxFlat7),
					new StyleProperty("disabled-item-background", styleBoxFlat6),
					new StyleProperty("selected-item-background", styleBoxFlat5)
				}),
				new StyleRule(new SelectorElement(typeof(ItemList), new string[]
				{
					"transparentItemList"
				}, null, null), new StyleProperty[]
				{
					new StyleProperty("itemlist-background", new StyleBoxFlat
					{
						BackgroundColor = Color.Transparent
					}),
					new StyleProperty("item-background", styleBoxFlat8),
					new StyleProperty("disabled-item-background", styleBoxFlat6),
					new StyleProperty("selected-item-background", styleBoxFlat5)
				}),
				new StyleRule(new SelectorElement(typeof(Tree), null, null, null), new StyleProperty[]
				{
					new StyleProperty("background", new StyleBoxFlat
					{
						BackgroundColor = new Color(32, 32, 32, byte.MaxValue)
					}),
					new StyleProperty("item-selected", new StyleBoxFlat
					{
						BackgroundColor = new Color(55, 55, 68, byte.MaxValue),
						ContentMarginLeftOverride = new float?((float)4)
					})
				}),
				new StyleRule(new SelectorElement(typeof(Placeholder), null, null, null), new StyleProperty[]
				{
					new StyleProperty("panel", styleBoxTexture26)
				}),
				new StyleRule(new SelectorElement(typeof(Label), new string[]
				{
					"PlaceholderText"
				}, null, null), new StyleProperty[]
				{
					new StyleProperty("font", font9),
					new StyleProperty("font-color", new Color(103, 103, 103, 128))
				}),
				new StyleRule(new SelectorElement(typeof(Label), new string[]
				{
					"LabelHeading"
				}, null, null), new StyleProperty[]
				{
					new StyleProperty("font", font10),
					new StyleProperty("font-color", StyleNano.NanoGold)
				}),
				new StyleRule(new SelectorElement(typeof(Label), new string[]
				{
					"LabelHeadingBigger"
				}, null, null), new StyleProperty[]
				{
					new StyleProperty("font", font12),
					new StyleProperty("font-color", StyleNano.NanoGold)
				}),
				new StyleRule(new SelectorElement(typeof(Label), new string[]
				{
					"LabelSubText"
				}, null, null), new StyleProperty[]
				{
					new StyleProperty("font", font),
					new StyleProperty("font-color", Color.DarkGray)
				}),
				new StyleRule(new SelectorElement(typeof(Label), new string[]
				{
					"LabelKeyText"
				}, null, null), new StyleProperty[]
				{
					new StyleProperty("font", font4),
					new StyleProperty("font-color", StyleNano.NanoGold)
				}),
				new StyleRule(new SelectorElement(typeof(Label), new string[]
				{
					"LabelSecondaryColor"
				}, null, null), new StyleProperty[]
				{
					new StyleProperty("font", font2),
					new StyleProperty("font-color", Color.DarkGray)
				}),
				new StyleRule(new SelectorChild(new SelectorElement(typeof(Button), new string[]
				{
					"ButtonBig"
				}, null, null), new SelectorElement(typeof(Label), null, null, null)), new StyleProperty[]
				{
					new StyleProperty("font", font9)
				}),
				new StyleRule(new SelectorElement(typeof(Label), new string[]
				{
					"PowerStateNone"
				}, null, null), new StyleProperty[]
				{
					new StyleProperty("font-color", new Color(0.8f, 0f, 0f, 1f))
				}),
				new StyleRule(new SelectorElement(typeof(Label), new string[]
				{
					"PowerStateLow"
				}, null, null), new StyleProperty[]
				{
					new StyleProperty("font-color", new Color(0.9f, 0.36f, 0f, 1f))
				}),
				new StyleRule(new SelectorElement(typeof(Label), new string[]
				{
					"PowerStateGood"
				}, null, null), new StyleProperty[]
				{
					new StyleProperty("font-color", new Color(0.024f, 0.8f, 0f, 1f))
				}),
				new StyleRule(new SelectorElement(typeof(MenuButton), new string[]
				{
					"ButtonSquare"
				}, null, null), new StyleProperty[]
				{
					new StyleProperty("stylebox", styleBoxTexture17)
				}),
				new StyleRule(new SelectorElement(typeof(MenuButton), new string[]
				{
					"OpenLeft"
				}, null, null), new StyleProperty[]
				{
					new StyleProperty("stylebox", styleBoxTexture16)
				}),
				new StyleRule(new SelectorElement(typeof(MenuButton), new string[]
				{
					"OpenRight"
				}, null, null), new StyleProperty[]
				{
					new StyleProperty("stylebox", styleBoxTexture15)
				}),
				new StyleRule(new SelectorElement(typeof(MenuButton), null, null, new string[]
				{
					"normal"
				}), new StyleProperty[]
				{
					new StyleProperty("modulate-self", StyleNano.ButtonColorDefault)
				}),
				new StyleRule(new SelectorElement(typeof(MenuButton), new string[]
				{
					"topButtonLabel"
				}, null, new string[]
				{
					"normal"
				}), new StyleProperty[]
				{
					new StyleProperty("modulate-self", StyleNano.ButtonColorDefaultRed)
				}),
				new StyleRule(new SelectorElement(typeof(MenuButton), null, null, new string[]
				{
					"normal"
				}), new StyleProperty[]
				{
					new StyleProperty("modulate-self", StyleNano.ButtonColorDefault)
				}),
				new StyleRule(new SelectorElement(typeof(MenuButton), null, null, new string[]
				{
					"pressed"
				}), new StyleProperty[]
				{
					new StyleProperty("modulate-self", StyleNano.ButtonColorPressed)
				}),
				new StyleRule(new SelectorElement(typeof(MenuButton), null, null, new string[]
				{
					"hover"
				}), new StyleProperty[]
				{
					new StyleProperty("modulate-self", StyleNano.ButtonColorHovered)
				}),
				new StyleRule(new SelectorElement(typeof(MenuButton), new string[]
				{
					"topButtonLabel"
				}, null, new string[]
				{
					"hover"
				}), new StyleProperty[]
				{
					new StyleProperty("modulate-self", StyleNano.ButtonColorHoveredRed)
				}),
				new StyleRule(new SelectorElement(typeof(Label), new string[]
				{
					"topButtonLabel"
				}, null, null), new StyleProperty[]
				{
					new StyleProperty("font", font6)
				}),
				new StyleRule(new SelectorElement(typeof(TextureButton), new string[]
				{
					TargetingDoll.StyleClassTargetDollZone
				}, null, new string[]
				{
					"normal"
				}), new StyleProperty[]
				{
					new StyleProperty("modulate-self", StyleNano.ButtonColorDefault)
				}),
				new StyleRule(new SelectorElement(typeof(TextureButton), new string[]
				{
					TargetingDoll.StyleClassTargetDollZone
				}, null, new string[]
				{
					"hover"
				}), new StyleProperty[]
				{
					new StyleProperty("modulate-self", StyleNano.ButtonColorHovered)
				}),
				new StyleRule(new SelectorElement(typeof(TextureButton), new string[]
				{
					TargetingDoll.StyleClassTargetDollZone
				}, null, new string[]
				{
					"pressed"
				}), new StyleProperty[]
				{
					new StyleProperty("modulate-self", StyleNano.ButtonColorPressed)
				}),
				new StyleRule(new SelectorChild(SelectorElement.Type(typeof(NanoHeading)), SelectorElement.Type(typeof(PanelContainer))), new StyleProperty[]
				{
					new StyleProperty("panel", styleBoxTexture28)
				}),
				new StyleRule(SelectorElement.Type(typeof(StripeBack)), new StyleProperty[]
				{
					new StyleProperty("background", styleBoxTexture29)
				}),
				new StyleRule(SelectorElement.Class(new string[]
				{
					"LabelBig"
				}), new StyleProperty[]
				{
					new StyleProperty("font", font9)
				}),
				new StyleRule(SelectorElement.Class(new string[]
				{
					"ItemStatus"
				}), new StyleProperty[]
				{
					new StyleProperty("font", font)
				}),
				new StyleRule(SelectorElement.Type(typeof(Slider)), new StyleProperty[]
				{
					new StyleProperty("background", styleBoxTexture31),
					new StyleProperty("foreground", styleBoxTexture32),
					new StyleProperty("grabber", styleBoxTexture33),
					new StyleProperty("fill", styleBoxTexture30)
				}),
				new StyleRule(SelectorElement.Type(typeof(ColorableSlider)), new StyleProperty[]
				{
					new StyleProperty("fillWhite", styleBoxTexture37),
					new StyleProperty("backgroundWhite", styleBoxTexture37)
				}),
				new StyleRule(new SelectorElement(typeof(Slider), new string[]
				{
					"Red"
				}, null, null), new StyleProperty[]
				{
					new StyleProperty("fill", styleBoxTexture35)
				}),
				new StyleRule(new SelectorElement(typeof(Slider), new string[]
				{
					"Green"
				}, null, null), new StyleProperty[]
				{
					new StyleProperty("fill", styleBoxTexture34)
				}),
				new StyleRule(new SelectorElement(typeof(Slider), new string[]
				{
					"Blue"
				}, null, null), new StyleProperty[]
				{
					new StyleProperty("fill", styleBoxTexture36)
				}),
				new StyleRule(new SelectorElement(typeof(Slider), new string[]
				{
					"White"
				}, null, null), new StyleProperty[]
				{
					new StyleProperty("fill", styleBoxTexture37)
				}),
				new StyleRule(new SelectorElement(typeof(Button), new string[]
				{
					"chatSelectorOptionButton"
				}, null, null), new StyleProperty[]
				{
					new StyleProperty("stylebox", styleBoxTexture18)
				}),
				new StyleRule(new SelectorElement(typeof(ContainerButton), new string[]
				{
					"chatFilterOptionButton"
				}, null, null), new StyleProperty[]
				{
					new StyleProperty("stylebox", styleBoxTexture19)
				}),
				new StyleRule(new SelectorElement(typeof(ContainerButton), new string[]
				{
					"chatFilterOptionButton"
				}, null, new string[]
				{
					"normal"
				}), new StyleProperty[]
				{
					new StyleProperty("modulate-self", StyleNano.ButtonColorDefault)
				}),
				new StyleRule(new SelectorElement(typeof(ContainerButton), new string[]
				{
					"chatFilterOptionButton"
				}, null, new string[]
				{
					"hover"
				}), new StyleProperty[]
				{
					new StyleProperty("modulate-self", StyleNano.ButtonColorHovered)
				}),
				new StyleRule(new SelectorElement(typeof(ContainerButton), new string[]
				{
					"chatFilterOptionButton"
				}, null, new string[]
				{
					"pressed"
				}), new StyleProperty[]
				{
					new StyleProperty("modulate-self", StyleNano.ButtonColorPressed)
				}),
				new StyleRule(new SelectorElement(typeof(ContainerButton), new string[]
				{
					"chatFilterOptionButton"
				}, null, new string[]
				{
					"disabled"
				}), new StyleProperty[]
				{
					new StyleProperty("modulate-self", StyleNano.ButtonColorDisabled)
				}),
				new StyleRule(new SelectorElement(typeof(OptionButton), null, null, null), new StyleProperty[]
				{
					new StyleProperty("stylebox", base.BaseButton)
				}),
				new StyleRule(new SelectorElement(typeof(OptionButton), null, null, new string[]
				{
					"normal"
				}), new StyleProperty[]
				{
					new StyleProperty("modulate-self", StyleNano.ButtonColorDefault)
				}),
				new StyleRule(new SelectorElement(typeof(OptionButton), null, null, new string[]
				{
					"hover"
				}), new StyleProperty[]
				{
					new StyleProperty("modulate-self", StyleNano.ButtonColorHovered)
				}),
				new StyleRule(new SelectorElement(typeof(OptionButton), null, null, new string[]
				{
					"pressed"
				}), new StyleProperty[]
				{
					new StyleProperty("modulate-self", StyleNano.ButtonColorPressed)
				}),
				new StyleRule(new SelectorElement(typeof(OptionButton), null, null, new string[]
				{
					"disabled"
				}), new StyleProperty[]
				{
					new StyleProperty("modulate-self", StyleNano.ButtonColorDisabled)
				}),
				new StyleRule(new SelectorElement(typeof(TextureRect), new string[]
				{
					"optionTriangle"
				}, null, null), new StyleProperty[]
				{
					new StyleProperty("texture", texture14)
				}),
				new StyleRule(new SelectorElement(typeof(Label), new string[]
				{
					"optionButton"
				}, null, null), new StyleProperty[]
				{
					new StyleProperty("alignMode", 1)
				}),
				new StyleRule(new SelectorElement(typeof(PanelContainer), new string[]
				{
					"HighDivider"
				}, null, null), new StyleProperty[]
				{
					new StyleProperty("panel", new StyleBoxFlat
					{
						BackgroundColor = StyleNano.NanoGold,
						ContentMarginBottomOverride = new float?((float)2),
						ContentMarginLeftOverride = new float?((float)2)
					})
				}),
				StylesheetHelpers.Element<PanelContainer>().Class("AngleRect").Prop("panel", base.BaseAngleRect).Prop("modulate-self", Color.FromHex("#252525", null)),
				StylesheetHelpers.Element<PanelContainer>().Class("LowDivider").Prop("panel", new StyleBoxFlat
				{
					BackgroundColor = Color.FromHex("#444", null),
					ContentMarginLeftOverride = new float?((float)2),
					ContentMarginBottomOverride = new float?((float)2)
				}),
				StylesheetHelpers.Element<Label>().Class("FancyWindowTitle").Prop("font", font13).Prop("font-color", StyleNano.NanoGold),
				StylesheetHelpers.Element<PanelContainer>().Class("WindowHeadingBackground").Prop("panel", new StyleBoxTexture(base.BaseButtonOpenLeft)
				{
					Padding = default(Thickness)
				}).Prop("modulate-self", StyleNano.PanelColorDark),
				StylesheetHelpers.Element<PanelContainer>().Class("WindowHeadingBackgroundLight").Prop("panel", new StyleBoxTexture(base.BaseButtonOpenLeft)
				{
					Padding = default(Thickness)
				}),
				StylesheetHelpers.Element<PanelContainer>().Class("PanelBackgroundBaseDark").Prop("panel", new StyleBoxTexture(base.BaseButtonOpenBoth)
				{
					Padding = default(Thickness)
				}).Prop("modulate-self", StyleNano.PanelColorDark),
				StylesheetHelpers.Element<PanelContainer>().Class("PanelBackgroundAngledDark").Prop("panel", base.BaseAngleRect).Prop("modulate-self", StyleNano.PanelColorDark),
				StylesheetHelpers.Element<TextureButton>().Class("CrossButtonRed").Prop("texture", resCache.GetTexture("/Textures/Interface/Nano/cross.svg.png")).Prop("modulate-self", StyleNano.DangerousRedFore),
				StylesheetHelpers.Element<TextureButton>().Class("CrossButtonRed").Pseudo("hover").Prop("modulate-self", Color.FromHex("#7F3636", null)),
				StylesheetHelpers.Element<TextureButton>().Class("CrossButtonRed").Pseudo("hover").Prop("modulate-self", Color.FromHex("#753131", null)),
				StylesheetHelpers.Element<PanelContainer>().Class("PaperDefaultBorder").Prop("panel", styleBoxTexture38),
				StylesheetHelpers.Element<RichTextLabel>().Class("PaperWrittenText").Prop("font", font2).Prop("modulate-self", Color.FromHex("#111111", null)),
				StylesheetHelpers.Element<LineEdit>().Class("PaperLineEdit").Prop("stylebox", new StyleBoxEmpty()),
				StylesheetHelpers.Element<Button>().Class("ButtonColorRed").Prop("modulate-self", StyleNano.ButtonColorDefaultRed),
				StylesheetHelpers.Element<Button>().Class("ButtonColorRed").Pseudo("normal").Prop("modulate-self", StyleNano.ButtonColorDefaultRed),
				StylesheetHelpers.Element<Button>().Class("ButtonColorRed").Pseudo("hover").Prop("modulate-self", StyleNano.ButtonColorHoveredRed),
				StylesheetHelpers.Element<Button>().Class("ButtonColorGreen").Prop("modulate-self", StyleNano.ButtonColorGoodDefault),
				StylesheetHelpers.Element<Button>().Class("ButtonColorGreen").Pseudo("normal").Prop("modulate-self", StyleNano.ButtonColorGoodDefault),
				StylesheetHelpers.Element<Button>().Class("ButtonColorGreen").Pseudo("hover").Prop("modulate-self", StyleNano.ButtonColorGoodHovered),
				StylesheetHelpers.Element<Label>().Class("StatusFieldTitle").Prop("font-color", StyleNano.NanoGold),
				StylesheetHelpers.Element<Label>().Class("Good").Prop("font-color", StyleNano.GoodGreenFore),
				StylesheetHelpers.Element<Label>().Class("Caution").Prop("font-color", StyleNano.ConcerningOrangeFore),
				StylesheetHelpers.Element<Label>().Class("Danger").Prop("font-color", StyleNano.DangerousRedFore),
				StylesheetHelpers.Element<Label>().Class("Disabled").Prop("font-color", StyleNano.DisabledFore),
				StylesheetHelpers.Element<PanelContainer>().Class("PDAContentBackground").Prop("panel", base.BaseButtonOpenBoth).Prop("modulate-self", Color.FromHex("#25252a", null)),
				StylesheetHelpers.Element<PanelContainer>().Class("PDABackground").Prop("panel", base.BaseButtonOpenBoth).Prop("modulate-self", Color.FromHex("#000000", null)),
				StylesheetHelpers.Element<PanelContainer>().Class("PDABackgroundRect").Prop("panel", base.BaseAngleRect).Prop("modulate-self", Color.FromHex("#717059", null)),
				StylesheetHelpers.Element<PanelContainer>().Class("PDABorderRect").Prop("panel", base.AngleBorderRect),
				StylesheetHelpers.Element<PanelContainer>().Class("BackgroundDark").Prop("panel", new StyleBoxFlat(Color.FromHex("#252525", null))),
				StylesheetHelpers.Element<PDASettingsButton>().Pseudo("normal").Prop("backgroundColor", Color.FromHex("#313138", null)).Prop("foregroundColor", Color.FromHex("#FFFFFF", null)),
				StylesheetHelpers.Element<PDASettingsButton>().Pseudo("hover").Prop("backgroundColor", Color.FromHex("#3E6C45", null)).Prop("foregroundColor", Color.FromHex("#FFFFFF", null)),
				StylesheetHelpers.Element<PDASettingsButton>().Pseudo("pressed").Prop("backgroundColor", Color.FromHex("#3E6C45", null)).Prop("foregroundColor", Color.FromHex("#FFFFFF", null)),
				StylesheetHelpers.Element<PDASettingsButton>().Pseudo("disabled").Prop("backgroundColor", Color.FromHex("#313138", null)).Prop("foregroundColor", Color.FromHex("#5a5a5a", null)),
				StylesheetHelpers.Element<PDAProgramItem>().Pseudo("normal").Prop("backgroundColor", Color.FromHex("#313138", null)),
				StylesheetHelpers.Element<PDAProgramItem>().Pseudo("hover").Prop("backgroundColor", Color.FromHex("#3E6C45", null)),
				StylesheetHelpers.Element<PDAProgramItem>().Pseudo("pressed").Prop("backgroundColor", Color.FromHex("#3E6C45", null)),
				StylesheetHelpers.Element<Label>().Class("PDAContentFooterText").Prop("font", font).Prop("font-color", Color.FromHex("#757575", null)),
				StylesheetHelpers.Element<Label>().Class("PDAWindowFooterText").Prop("font", font).Prop("font-color", Color.FromHex("#333d3b", null)),
				StylesheetHelpers.Element<ContainerButton>().Identifier("tree-button").Class("even-row").Prop("stylebox", new StyleBoxFlat
				{
					BackgroundColor = StyleNano.FancyTreeEvenRowColor
				}),
				StylesheetHelpers.Element<ContainerButton>().Identifier("tree-button").Class("odd-row").Prop("stylebox", new StyleBoxFlat
				{
					BackgroundColor = StyleNano.FancyTreeOddRowColor
				}),
				StylesheetHelpers.Element<ContainerButton>().Identifier("tree-button").Class("selected").Prop("stylebox", new StyleBoxFlat
				{
					BackgroundColor = StyleNano.FancyTreeSelectedRowColor
				}),
				StylesheetHelpers.Element<ContainerButton>().Identifier("tree-button").Pseudo("hover").Prop("stylebox", new StyleBoxFlat
				{
					BackgroundColor = StyleNano.FancyTreeSelectedRowColor
				})
			}).ToList<StyleRule>());
		}

		// Token: 0x04000395 RID: 917
		public const string StyleClassBorderedWindowPanel = "BorderedWindowPanel";

		// Token: 0x04000396 RID: 918
		public const string StyleClassInventorySlotBackground = "InventorySlotBackground";

		// Token: 0x04000397 RID: 919
		public const string StyleClassHandSlotHighlight = "HandSlotHighlight";

		// Token: 0x04000398 RID: 920
		public const string StyleClassChatSubPanel = "ChatSubPanel";

		// Token: 0x04000399 RID: 921
		public const string StyleClassTransparentBorderedWindowPanel = "TransparentBorderedWindowPanel";

		// Token: 0x0400039A RID: 922
		public const string StyleClassHotbarPanel = "HotbarPanel";

		// Token: 0x0400039B RID: 923
		public const string StyleClassTooltipPanel = "tooltipBox";

		// Token: 0x0400039C RID: 924
		public const string StyleClassTooltipAlertTitle = "tooltipAlertTitle";

		// Token: 0x0400039D RID: 925
		public const string StyleClassTooltipAlertDescription = "tooltipAlertDesc";

		// Token: 0x0400039E RID: 926
		public const string StyleClassTooltipAlertCooldown = "tooltipAlertCooldown";

		// Token: 0x0400039F RID: 927
		public const string StyleClassTooltipActionTitle = "tooltipActionTitle";

		// Token: 0x040003A0 RID: 928
		public const string StyleClassTooltipActionDescription = "tooltipActionDesc";

		// Token: 0x040003A1 RID: 929
		public const string StyleClassTooltipActionCooldown = "tooltipActionCooldown";

		// Token: 0x040003A2 RID: 930
		public const string StyleClassTooltipActionRequirements = "tooltipActionCooldown";

		// Token: 0x040003A3 RID: 931
		public const string StyleClassHotbarSlotNumber = "hotbarSlotNumber";

		// Token: 0x040003A4 RID: 932
		public const string StyleClassActionSearchBox = "actionSearchBox";

		// Token: 0x040003A5 RID: 933
		public const string StyleClassActionMenuItemRevoked = "actionMenuItemRevoked";

		// Token: 0x040003A6 RID: 934
		public const string StyleClassChatLineEdit = "chatLineEdit";

		// Token: 0x040003A7 RID: 935
		public const string StyleClassChatChannelSelectorButton = "chatSelectorOptionButton";

		// Token: 0x040003A8 RID: 936
		public const string StyleClassChatFilterOptionButton = "chatFilterOptionButton";

		// Token: 0x040003A9 RID: 937
		public const string StyleClassStorageButton = "storageButton";

		// Token: 0x040003AA RID: 938
		public const string StyleClassSliderRed = "Red";

		// Token: 0x040003AB RID: 939
		public const string StyleClassSliderGreen = "Green";

		// Token: 0x040003AC RID: 940
		public const string StyleClassSliderBlue = "Blue";

		// Token: 0x040003AD RID: 941
		public const string StyleClassSliderWhite = "White";

		// Token: 0x040003AE RID: 942
		public const string StyleClassLabelHeadingBigger = "LabelHeadingBigger";

		// Token: 0x040003AF RID: 943
		public const string StyleClassLabelKeyText = "LabelKeyText";

		// Token: 0x040003B0 RID: 944
		public const string StyleClassLabelSecondaryColor = "LabelSecondaryColor";

		// Token: 0x040003B1 RID: 945
		public const string StyleClassLabelBig = "LabelBig";

		// Token: 0x040003B2 RID: 946
		public const string StyleClassButtonBig = "ButtonBig";

		// Token: 0x040003B3 RID: 947
		public const string StyleClassPopupMessageSmall = "PopupMessageSmall";

		// Token: 0x040003B4 RID: 948
		public const string StyleClassPopupMessageSmallCaution = "PopupMessageSmallCaution";

		// Token: 0x040003B5 RID: 949
		public const string StyleClassPopupMessageMedium = "PopupMessageMedium";

		// Token: 0x040003B6 RID: 950
		public const string StyleClassPopupMessageMediumCaution = "PopupMessageMediumCaution";

		// Token: 0x040003B7 RID: 951
		public const string StyleClassPopupMessageLarge = "PopupMessageLarge";

		// Token: 0x040003B8 RID: 952
		public const string StyleClassPopupMessageLargeCaution = "PopupMessageLargeCaution";

		// Token: 0x040003B9 RID: 953
		public static readonly Color NanoGold = Color.FromHex("#A88B5E", null);

		// Token: 0x040003BA RID: 954
		public static readonly Color GoodGreenFore = Color.FromHex("#31843E", null);

		// Token: 0x040003BB RID: 955
		public static readonly Color ConcerningOrangeFore = Color.FromHex("#A5762F", null);

		// Token: 0x040003BC RID: 956
		public static readonly Color DangerousRedFore = Color.FromHex("#BB3232", null);

		// Token: 0x040003BD RID: 957
		public static readonly Color DisabledFore = Color.FromHex("#5A5A5A", null);

		// Token: 0x040003BE RID: 958
		public static readonly Color ButtonColorDefault = Color.FromHex("#464950", null);

		// Token: 0x040003BF RID: 959
		public static readonly Color ButtonColorDefaultRed = Color.FromHex("#D43B3B", null);

		// Token: 0x040003C0 RID: 960
		public static readonly Color ButtonColorHovered = Color.FromHex("#575b61", null);

		// Token: 0x040003C1 RID: 961
		public static readonly Color ButtonColorHoveredRed = Color.FromHex("#DF6B6B", null);

		// Token: 0x040003C2 RID: 962
		public static readonly Color ButtonColorPressed = Color.FromHex("#3e6c45", null);

		// Token: 0x040003C3 RID: 963
		public static readonly Color ButtonColorDisabled = Color.FromHex("#303133", null);

		// Token: 0x040003C4 RID: 964
		public static readonly Color ButtonColorCautionDefault = Color.FromHex("#ab3232", null);

		// Token: 0x040003C5 RID: 965
		public static readonly Color ButtonColorCautionHovered = Color.FromHex("#cf2f2f", null);

		// Token: 0x040003C6 RID: 966
		public static readonly Color ButtonColorCautionPressed = Color.FromHex("#3e6c45", null);

		// Token: 0x040003C7 RID: 967
		public static readonly Color ButtonColorCautionDisabled = Color.FromHex("#602a2a", null);

		// Token: 0x040003C8 RID: 968
		public static readonly Color ButtonColorGoodDefault = Color.FromHex("#3E6C45", null);

		// Token: 0x040003C9 RID: 969
		public static readonly Color ButtonColorGoodHovered = Color.FromHex("#31843E", null);

		// Token: 0x040003CA RID: 970
		public static readonly Color ButtonColorContext = Color.FromHex("#1119", null);

		// Token: 0x040003CB RID: 971
		public static readonly Color ButtonColorContextHover = Color.FromHex("#575b61", null);

		// Token: 0x040003CC RID: 972
		public static readonly Color ButtonColorContextPressed = Color.FromHex("#3e6c45", null);

		// Token: 0x040003CD RID: 973
		public static readonly Color ButtonColorContextDisabled = Color.Black;

		// Token: 0x040003CE RID: 974
		public static readonly Color ExamineButtonColorContext = Color.Transparent;

		// Token: 0x040003CF RID: 975
		public static readonly Color ExamineButtonColorContextHover = Color.FromHex("#575b61", null);

		// Token: 0x040003D0 RID: 976
		public static readonly Color ExamineButtonColorContextPressed = Color.FromHex("#3e6c45", null);

		// Token: 0x040003D1 RID: 977
		public static readonly Color ExamineButtonColorContextDisabled = Color.FromHex("#5A5A5A", null);

		// Token: 0x040003D2 RID: 978
		public static readonly Color PanelColorDark = Color.FromHex("#1F1F1F", null);

		// Token: 0x040003D3 RID: 979
		public static readonly Color FancyTreeEvenRowColor = Color.FromHex("#25252A", null);

		// Token: 0x040003D4 RID: 980
		public static readonly Color FancyTreeOddRowColor = StyleNano.FancyTreeEvenRowColor * new Color(0.8f, 0.8f, 0.8f, 1f);

		// Token: 0x040003D5 RID: 981
		public static readonly Color FancyTreeSelectedRowColor = new Color(55, 55, 68, byte.MaxValue);

		// Token: 0x040003D6 RID: 982
		public const string StyleClassPowerStateNone = "PowerStateNone";

		// Token: 0x040003D7 RID: 983
		public const string StyleClassPowerStateLow = "PowerStateLow";

		// Token: 0x040003D8 RID: 984
		public const string StyleClassPowerStateGood = "PowerStateGood";

		// Token: 0x040003D9 RID: 985
		public const string StyleClassItemStatus = "ItemStatus";

		// Token: 0x040003DA RID: 986
		public const string StyleClassBackgroundBaseDark = "PanelBackgroundBaseDark";

		// Token: 0x040003DB RID: 987
		public const string StyleClassCrossButtonRed = "CrossButtonRed";

		// Token: 0x040003DC RID: 988
		public const string StyleClassButtonColorRed = "ButtonColorRed";

		// Token: 0x040003DD RID: 989
		public const string StyleClassButtonColorGreen = "ButtonColorGreen";
	}
}
