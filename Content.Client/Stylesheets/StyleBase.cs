using System;
using System.Runtime.CompilerServices;
using Content.Client.Resources;
using Robust.Client.Graphics;
using Robust.Client.ResourceManagement;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.Controls;
using Robust.Shared.Maths;

namespace Content.Client.Stylesheets
{
	// Token: 0x02000111 RID: 273
	[NullableContext(1)]
	[Nullable(0)]
	public abstract class StyleBase
	{
		// Token: 0x17000157 RID: 343
		// (get) Token: 0x060007A2 RID: 1954
		public abstract Stylesheet Stylesheet { get; }

		// Token: 0x17000158 RID: 344
		// (get) Token: 0x060007A3 RID: 1955 RVA: 0x00027E54 File Offset: 0x00026054
		protected StyleRule[] BaseRules { get; }

		// Token: 0x17000159 RID: 345
		// (get) Token: 0x060007A4 RID: 1956 RVA: 0x00027E5C File Offset: 0x0002605C
		protected StyleBoxTexture BaseButton { get; }

		// Token: 0x1700015A RID: 346
		// (get) Token: 0x060007A5 RID: 1957 RVA: 0x00027E64 File Offset: 0x00026064
		protected StyleBoxTexture BaseButtonOpenRight { get; }

		// Token: 0x1700015B RID: 347
		// (get) Token: 0x060007A6 RID: 1958 RVA: 0x00027E6C File Offset: 0x0002606C
		protected StyleBoxTexture BaseButtonOpenLeft { get; }

		// Token: 0x1700015C RID: 348
		// (get) Token: 0x060007A7 RID: 1959 RVA: 0x00027E74 File Offset: 0x00026074
		protected StyleBoxTexture BaseButtonOpenBoth { get; }

		// Token: 0x1700015D RID: 349
		// (get) Token: 0x060007A8 RID: 1960 RVA: 0x00027E7C File Offset: 0x0002607C
		protected StyleBoxTexture BaseButtonSquare { get; }

		// Token: 0x1700015E RID: 350
		// (get) Token: 0x060007A9 RID: 1961 RVA: 0x00027E84 File Offset: 0x00026084
		protected StyleBoxTexture BaseAngleRect { get; }

		// Token: 0x1700015F RID: 351
		// (get) Token: 0x060007AA RID: 1962 RVA: 0x00027E8C File Offset: 0x0002608C
		protected StyleBoxTexture AngleBorderRect { get; }

		// Token: 0x060007AB RID: 1963 RVA: 0x00027E94 File Offset: 0x00026094
		protected StyleBase(IResourceCache resCache)
		{
			Font font = resCache.GetFont(new string[]
			{
				"/Fonts/NotoSans/NotoSans-Regular.ttf",
				"/Fonts/NotoSans/NotoSansSymbols-Regular.ttf",
				"/Fonts/NotoSans/NotoSansSymbols2-Regular.ttf"
			}, 12);
			Font font2 = resCache.GetFont(new string[]
			{
				"/Fonts/NotoSans/NotoSans-Italic.ttf",
				"/Fonts/NotoSans/NotoSansSymbols-Regular.ttf",
				"/Fonts/NotoSans/NotoSansSymbols2-Regular.ttf"
			}, 12);
			Texture texture = resCache.GetTexture("/Textures/Interface/Nano/cross.svg.png");
			Texture texture2 = resCache.GetTexture("/Textures/Interface/Nano/button.svg.96dpi.png");
			this.BaseButton = new StyleBoxTexture
			{
				Texture = texture2
			};
			this.BaseButton.SetPatchMargin(15, 10f);
			this.BaseButton.SetPadding(15, 1f);
			this.BaseButton.SetContentMarginOverride(3, 2f);
			this.BaseButton.SetContentMarginOverride(12, 14f);
			this.BaseButtonOpenRight = new StyleBoxTexture(this.BaseButton)
			{
				Texture = new AtlasTexture(texture2, UIBox2.FromDimensions(new ValueTuple<float, float>(0f, 0f), new ValueTuple<float, float>(14f, 24f)))
			};
			this.BaseButtonOpenRight.SetPatchMargin(4, 0f);
			this.BaseButtonOpenRight.SetContentMarginOverride(4, 8f);
			this.BaseButtonOpenRight.SetPadding(4, 2f);
			this.BaseButtonOpenLeft = new StyleBoxTexture(this.BaseButton)
			{
				Texture = new AtlasTexture(texture2, UIBox2.FromDimensions(new ValueTuple<float, float>(10f, 0f), new ValueTuple<float, float>(14f, 24f)))
			};
			this.BaseButtonOpenLeft.SetPatchMargin(8, 0f);
			this.BaseButtonOpenLeft.SetContentMarginOverride(8, 8f);
			this.BaseButtonOpenLeft.SetPadding(8, 1f);
			this.BaseButtonOpenBoth = new StyleBoxTexture(this.BaseButton)
			{
				Texture = new AtlasTexture(texture2, UIBox2.FromDimensions(new ValueTuple<float, float>(10f, 0f), new ValueTuple<float, float>(3f, 24f)))
			};
			this.BaseButtonOpenBoth.SetPatchMargin(12, 0f);
			this.BaseButtonOpenBoth.SetContentMarginOverride(12, 8f);
			this.BaseButtonOpenBoth.SetPadding(4, 2f);
			this.BaseButtonOpenBoth.SetPadding(8, 1f);
			this.BaseButtonSquare = new StyleBoxTexture(this.BaseButton)
			{
				Texture = new AtlasTexture(texture2, UIBox2.FromDimensions(new ValueTuple<float, float>(10f, 0f), new ValueTuple<float, float>(3f, 24f)))
			};
			this.BaseButtonSquare.SetPatchMargin(12, 0f);
			this.BaseButtonSquare.SetContentMarginOverride(12, 8f);
			this.BaseButtonSquare.SetPadding(4, 2f);
			this.BaseButtonSquare.SetPadding(8, 1f);
			this.BaseAngleRect = new StyleBoxTexture
			{
				Texture = texture2
			};
			this.BaseAngleRect.SetPatchMargin(15, 10f);
			this.AngleBorderRect = new StyleBoxTexture
			{
				Texture = resCache.GetTexture("/Textures/Interface/Nano/geometric_panel_border.svg.96dpi.png")
			};
			this.AngleBorderRect.SetPatchMargin(15, 10f);
			StyleBoxFlat styleBoxFlat = new StyleBoxFlat
			{
				BackgroundColor = Color.LightGreen.WithAlpha(0.35f),
				ContentMarginLeftOverride = new float?((float)10),
				ContentMarginTopOverride = new float?((float)10)
			};
			StyleBoxFlat styleBoxFlat2 = new StyleBoxFlat
			{
				BackgroundColor = new Color(140, 140, 140, byte.MaxValue).WithAlpha(0.35f),
				ContentMarginLeftOverride = new float?((float)10),
				ContentMarginTopOverride = new float?((float)10)
			};
			StyleBoxFlat styleBoxFlat3 = new StyleBoxFlat
			{
				BackgroundColor = new Color(160, 160, 160, byte.MaxValue).WithAlpha(0.35f),
				ContentMarginLeftOverride = new float?((float)10),
				ContentMarginTopOverride = new float?((float)10)
			};
			StyleBoxFlat styleBoxFlat4 = new StyleBoxFlat
			{
				BackgroundColor = Color.LightGreen.WithAlpha(0.35f),
				ContentMarginTopOverride = new float?((float)10)
			};
			StyleBoxFlat styleBoxFlat5 = new StyleBoxFlat
			{
				BackgroundColor = new Color(140, 140, 140, byte.MaxValue).WithAlpha(0.35f),
				ContentMarginTopOverride = new float?((float)10)
			};
			StyleBoxFlat styleBoxFlat6 = new StyleBoxFlat
			{
				BackgroundColor = new Color(160, 160, 160, byte.MaxValue).WithAlpha(0.35f),
				ContentMarginTopOverride = new float?((float)10)
			};
			this.BaseRules = new StyleRule[]
			{
				new StyleRule(new SelectorElement(null, null, null, null), new StyleProperty[]
				{
					new StyleProperty("font", font)
				}),
				new StyleRule(new SelectorElement(null, new string[]
				{
					"Italic"
				}, null, null), new StyleProperty[]
				{
					new StyleProperty("font", font2)
				}),
				new StyleRule(new SelectorElement(typeof(TextureButton), new string[]
				{
					"windowCloseButton"
				}, null, null), new StyleProperty[]
				{
					new StyleProperty("texture", texture),
					new StyleProperty("modulate-self", StyleNano.NanoGold)
				}),
				new StyleRule(new SelectorElement(typeof(TextureButton), new string[]
				{
					"windowCloseButton"
				}, null, new string[]
				{
					"hover"
				}), new StyleProperty[]
				{
					new StyleProperty("modulate-self", Color.FromHex("#7F3636", null))
				}),
				new StyleRule(new SelectorElement(typeof(TextureButton), new string[]
				{
					"windowCloseButton"
				}, null, new string[]
				{
					"pressed"
				}), new StyleProperty[]
				{
					new StyleProperty("modulate-self", Color.FromHex("#753131", null))
				}),
				new StyleRule(new SelectorElement(typeof(VScrollBar), null, null, null), new StyleProperty[]
				{
					new StyleProperty("grabber", styleBoxFlat)
				}),
				new StyleRule(new SelectorElement(typeof(VScrollBar), null, null, new string[]
				{
					"hover"
				}), new StyleProperty[]
				{
					new StyleProperty("grabber", styleBoxFlat2)
				}),
				new StyleRule(new SelectorElement(typeof(VScrollBar), null, null, new string[]
				{
					"grabbed"
				}), new StyleProperty[]
				{
					new StyleProperty("grabber", styleBoxFlat3)
				}),
				new StyleRule(new SelectorElement(typeof(HScrollBar), null, null, null), new StyleProperty[]
				{
					new StyleProperty("grabber", styleBoxFlat4)
				}),
				new StyleRule(new SelectorElement(typeof(HScrollBar), null, null, new string[]
				{
					"hover"
				}), new StyleProperty[]
				{
					new StyleProperty("grabber", styleBoxFlat5)
				}),
				new StyleRule(new SelectorElement(typeof(HScrollBar), null, null, new string[]
				{
					"grabbed"
				}), new StyleProperty[]
				{
					new StyleProperty("grabber", styleBoxFlat6)
				})
			};
		}

		// Token: 0x04000381 RID: 897
		public const string ClassHighDivider = "HighDivider";

		// Token: 0x04000382 RID: 898
		public const string ClassLowDivider = "LowDivider";

		// Token: 0x04000383 RID: 899
		public const string StyleClassLabelHeading = "LabelHeading";

		// Token: 0x04000384 RID: 900
		public const string StyleClassLabelSubText = "LabelSubText";

		// Token: 0x04000385 RID: 901
		public const string StyleClassItalic = "Italic";

		// Token: 0x04000386 RID: 902
		public const string ClassAngleRect = "AngleRect";

		// Token: 0x04000387 RID: 903
		public const string ButtonOpenRight = "OpenRight";

		// Token: 0x04000388 RID: 904
		public const string ButtonOpenLeft = "OpenLeft";

		// Token: 0x04000389 RID: 905
		public const string ButtonOpenBoth = "OpenBoth";

		// Token: 0x0400038A RID: 906
		public const string ButtonSquare = "ButtonSquare";

		// Token: 0x0400038B RID: 907
		public const string ButtonCaution = "Caution";

		// Token: 0x0400038C RID: 908
		public const int DefaultGrabberSize = 10;
	}
}
