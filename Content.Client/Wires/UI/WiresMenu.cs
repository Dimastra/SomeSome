using System;
using System.Runtime.CompilerServices;
using Content.Client.Resources;
using Content.Client.Stylesheets;
using Content.Shared.Wires;
using Robust.Client.Animations;
using Robust.Client.Graphics;
using Robust.Client.ResourceManagement;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.Controls;
using Robust.Client.UserInterface.CustomControls;
using Robust.Shared.Input;
using Robust.Shared.IoC;
using Robust.Shared.Localization;
using Robust.Shared.Maths;

namespace Content.Client.Wires.UI
{
	// Token: 0x02000016 RID: 22
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class WiresMenu : BaseWindow
	{
		// Token: 0x1700000B RID: 11
		// (get) Token: 0x06000043 RID: 67 RVA: 0x00003430 File Offset: 0x00001630
		public WiresBoundUserInterface Owner { get; }

		// Token: 0x1700000C RID: 12
		// (get) Token: 0x06000044 RID: 68 RVA: 0x00003438 File Offset: 0x00001638
		// (set) Token: 0x06000045 RID: 69 RVA: 0x00003440 File Offset: 0x00001640
		public TextureButton CloseButton { get; set; }

		// Token: 0x06000046 RID: 70 RVA: 0x0000344C File Offset: 0x0000164C
		public WiresMenu(WiresBoundUserInterface owner)
		{
			IoCManager.InjectDependencies<WiresMenu>(this);
			this.Owner = owner;
			LayoutContainer layoutContainer = new LayoutContainer
			{
				Name = "WireRoot"
			};
			base.AddChild(layoutContainer);
			base.MouseFilter = 0;
			Texture texture = this._resourceCache.GetTexture("/Textures/Interface/Nano/button.svg.96dpi.png");
			StyleBoxTexture styleBoxTexture = new StyleBoxTexture
			{
				Texture = texture,
				Modulate = Color.FromHex("#252525", null)
			};
			styleBoxTexture.SetPatchMargin(15, 10f);
			PanelContainer panelContainer = new PanelContainer
			{
				PanelOverride = styleBoxTexture,
				MouseFilter = 1
			};
			LayoutContainer layoutContainer2 = new LayoutContainer
			{
				Name = "BottomWrap"
			};
			PanelContainer panelContainer2 = new PanelContainer
			{
				PanelOverride = styleBoxTexture,
				MouseFilter = 1
			};
			BoxContainer boxContainer = new BoxContainer();
			boxContainer.Orientation = 0;
			boxContainer.Children.Add(new PanelContainer
			{
				MinSize = new ValueTuple<float, float>(2f, 0f),
				PanelOverride = new StyleBoxFlat
				{
					BackgroundColor = Color.FromHex("#525252ff", null)
				}
			});
			boxContainer.Children.Add(new PanelContainer
			{
				HorizontalExpand = true,
				MouseFilter = 0,
				Name = "Shadow",
				PanelOverride = new StyleBoxFlat
				{
					BackgroundColor = Color.Black.WithAlpha(0.5f)
				}
			});
			boxContainer.Children.Add(new PanelContainer
			{
				MinSize = new ValueTuple<float, float>(2f, 0f),
				PanelOverride = new StyleBoxFlat
				{
					BackgroundColor = Color.FromHex("#525252ff", null)
				}
			});
			BoxContainer boxContainer2 = boxContainer;
			BoxContainer boxContainer3 = new BoxContainer
			{
				Orientation = 0
			};
			this._wiresHBox = new BoxContainer
			{
				Orientation = 0,
				SeparationOverride = new int?(4),
				VerticalAlignment = 3
			};
			boxContainer3.AddChild(new Control
			{
				MinSize = new ValueTuple<float, float>(20f, 0f)
			});
			boxContainer3.AddChild(this._wiresHBox);
			boxContainer3.AddChild(new Control
			{
				MinSize = new ValueTuple<float, float>(20f, 0f)
			});
			layoutContainer2.AddChild(panelContainer2);
			LayoutContainer.SetAnchorPreset(panelContainer2, 12, false);
			LayoutContainer.SetMarginTop(panelContainer2, -55f);
			layoutContainer2.AddChild(boxContainer2);
			LayoutContainer.SetAnchorPreset(boxContainer2, 12, false);
			LayoutContainer.SetMarginBottom(boxContainer2, -55f);
			LayoutContainer.SetMarginTop(boxContainer2, -80f);
			LayoutContainer.SetMarginLeft(boxContainer2, 12f);
			LayoutContainer.SetMarginRight(boxContainer2, -12f);
			layoutContainer2.AddChild(boxContainer3);
			LayoutContainer.SetAnchorPreset(boxContainer3, 15, false);
			LayoutContainer.SetMarginBottom(boxContainer3, -4f);
			layoutContainer.AddChild(panelContainer);
			layoutContainer.AddChild(layoutContainer2);
			LayoutContainer.SetAnchorPreset(panelContainer, 15, false);
			LayoutContainer.SetMarginBottom(panelContainer, -80f);
			LayoutContainer.SetAnchorPreset(layoutContainer2, 13, false);
			LayoutContainer.SetGrowHorizontal(layoutContainer2, 2);
			BoxContainer boxContainer4 = new BoxContainer();
			boxContainer4.Orientation = 1;
			Control.OrderedChildCollection children = boxContainer4.Children;
			BoxContainer boxContainer5 = new BoxContainer();
			boxContainer5.Orientation = 1;
			Control control = boxContainer5;
			this._topContainer = boxContainer5;
			children.Add(control);
			boxContainer4.Children.Add(new Control
			{
				MinSize = new ValueTuple<float, float>(0f, 110f)
			});
			BoxContainer boxContainer6 = boxContainer4;
			layoutContainer.AddChild(boxContainer6);
			LayoutContainer.SetAnchorPreset(boxContainer6, 15, false);
			Font font = this._resourceCache.GetFont("/Fonts/Boxfont-round/Boxfont Round.ttf", 13);
			Font font2 = this._resourceCache.GetFont("/Fonts/Boxfont-round/Boxfont Round.ttf", 10);
			BoxContainer boxContainer7 = new BoxContainer();
			boxContainer7.Orientation = 0;
			boxContainer7.Margin = new Thickness(4f, 2f, 12f, 2f);
			Control.OrderedChildCollection children2 = boxContainer7.Children;
			Label label = new Label();
			label.Text = Loc.GetString("wires-menu-name-label");
			label.FontOverride = font;
			label.FontColorOverride = new Color?(StyleNano.NanoGold);
			label.VerticalAlignment = 2;
			Label label2 = label;
			this._nameLabel = label;
			children2.Add(label2);
			Control.OrderedChildCollection children3 = boxContainer7.Children;
			Label label3 = new Label();
			label3.Text = Loc.GetString("wires-menu-dead-beef-text");
			label3.FontOverride = font2;
			label3.FontColorOverride = new Color?(Color.Gray);
			label3.VerticalAlignment = 2;
			label3.Margin = new Thickness(8f, 0f, 20f, 0f);
			label3.HorizontalAlignment = 1;
			label3.HorizontalExpand = true;
			label2 = label3;
			this._serialLabel = label3;
			children3.Add(label2);
			Control.OrderedChildCollection children4 = boxContainer7.Children;
			Button button = new Button();
			button.Text = "?";
			button.Margin = new Thickness(0f, 0f, 2f, 0f);
			Button button2 = button;
			children4.Add(button);
			Control.OrderedChildCollection children5 = boxContainer7.Children;
			TextureButton textureButton = new TextureButton();
			textureButton.StyleClasses.Add("windowCloseButton");
			textureButton.VerticalAlignment = 2;
			TextureButton textureButton2 = textureButton;
			this.CloseButton = textureButton;
			children5.Add(textureButton2);
			BoxContainer boxContainer8 = boxContainer7;
			button2.OnPressed += delegate(BaseButton.ButtonEventArgs a)
			{
				WiresMenu.HelpPopup helpPopup = new WiresMenu.HelpPopup();
				base.UserInterfaceManager.ModalRoot.AddChild(helpPopup);
				helpPopup.Open(new UIBox2?(UIBox2.FromDimensions(a.Event.PointerLocation.Position, new ValueTuple<float, float>(400f, 200f))), null);
			};
			PanelContainer panelContainer3 = new PanelContainer();
			panelContainer3.PanelOverride = new StyleBoxFlat
			{
				BackgroundColor = Color.FromHex("#202020", null)
			};
			Control.OrderedChildCollection children6 = panelContainer3.Children;
			BoxContainer boxContainer9 = new BoxContainer();
			boxContainer9.Orientation = 0;
			Control.OrderedChildCollection children7 = boxContainer9.Children;
			GridContainer gridContainer = new GridContainer();
			gridContainer.Margin = new Thickness(8f, 4f);
			gridContainer.Columns = 3;
			control = gridContainer;
			this._statusContainer = gridContainer;
			children7.Add(control);
			children6.Add(boxContainer9);
			PanelContainer panelContainer4 = panelContainer3;
			this._topContainer.AddChild(boxContainer8);
			this._topContainer.AddChild(new PanelContainer
			{
				MinSize = new ValueTuple<float, float>(0f, 2f),
				PanelOverride = new StyleBoxFlat
				{
					BackgroundColor = Color.FromHex("#525252ff", null)
				}
			});
			this._topContainer.AddChild(panelContainer4);
			this._topContainer.AddChild(new PanelContainer
			{
				MinSize = new ValueTuple<float, float>(0f, 2f),
				PanelOverride = new StyleBoxFlat
				{
					BackgroundColor = Color.FromHex("#525252ff", null)
				}
			});
			this.CloseButton.OnPressed += delegate(BaseButton.ButtonEventArgs _)
			{
				this.Close();
			};
			base.SetSize = new ValueTuple<float, float>(320f, 200f);
		}

		// Token: 0x06000047 RID: 71 RVA: 0x00003ACC File Offset: 0x00001CCC
		public void Populate(WiresBoundUserInterfaceState state)
		{
			this._nameLabel.Text = state.BoardName;
			this._serialLabel.Text = state.SerialNumber;
			this._wiresHBox.RemoveAllChildren();
			Random random = new Random(state.WireSeed);
			ClientWire[] wiresList = state.WiresList;
			for (int i = 0; i < wiresList.Length; i++)
			{
				ClientWire wire = wiresList[i];
				bool mirror = random.Next(2) == 0;
				bool flip = random.Next(2) == 0;
				int type = random.Next(2);
				WiresMenu.WireControl wireControl = new WiresMenu.WireControl(wire.Color, wire.Letter, wire.IsCut, flip, mirror, type, this._resourceCache)
				{
					VerticalAlignment = 3
				};
				this._wiresHBox.AddChild(wireControl);
				wireControl.WireClicked += delegate()
				{
					this.Owner.PerformAction(wire.Id, wire.IsCut ? WiresAction.Mend : WiresAction.Cut);
				};
				wireControl.ContactsClicked += delegate()
				{
					this.Owner.PerformAction(wire.Id, WiresAction.Pulse);
				};
			}
			this._statusContainer.RemoveAllChildren();
			foreach (StatusEntry statusEntry in state.Statuses)
			{
				object value = statusEntry.Value;
				if (value is StatusLightData)
				{
					StatusLightData data = (StatusLightData)value;
					this._statusContainer.AddChild(new WiresMenu.StatusLight(data, this._resourceCache));
				}
				else
				{
					this._statusContainer.AddChild(new Label
					{
						Text = statusEntry.ToString()
					});
				}
			}
		}

		// Token: 0x06000048 RID: 72 RVA: 0x00003C56 File Offset: 0x00001E56
		protected override BaseWindow.DragMode GetDragModeFor(Vector2 relativeMousePos)
		{
			return 1;
		}

		// Token: 0x06000049 RID: 73 RVA: 0x00003C59 File Offset: 0x00001E59
		protected override bool HasPoint(Vector2 point)
		{
			return false;
		}

		// Token: 0x0400001A RID: 26
		[Dependency]
		private readonly IResourceCache _resourceCache;

		// Token: 0x0400001C RID: 28
		private readonly Control _wiresHBox;

		// Token: 0x0400001D RID: 29
		private readonly Control _topContainer;

		// Token: 0x0400001E RID: 30
		private readonly Control _statusContainer;

		// Token: 0x0400001F RID: 31
		private readonly Label _nameLabel;

		// Token: 0x04000020 RID: 32
		private readonly Label _serialLabel;

		// Token: 0x02000017 RID: 23
		[NullableContext(2)]
		[Nullable(0)]
		private sealed class WireControl : Control
		{
			// Token: 0x14000006 RID: 6
			// (add) Token: 0x0600004C RID: 76 RVA: 0x00003CC0 File Offset: 0x00001EC0
			// (remove) Token: 0x0600004D RID: 77 RVA: 0x00003CF8 File Offset: 0x00001EF8
			public event Action WireClicked;

			// Token: 0x14000007 RID: 7
			// (add) Token: 0x0600004E RID: 78 RVA: 0x00003D30 File Offset: 0x00001F30
			// (remove) Token: 0x0600004F RID: 79 RVA: 0x00003D68 File Offset: 0x00001F68
			public event Action ContactsClicked;

			// Token: 0x06000050 RID: 80 RVA: 0x00003DA0 File Offset: 0x00001FA0
			[NullableContext(1)]
			public WireControl(WireColor color, WireLetter letter, bool isCut, bool flip, bool mirror, int type, IResourceCache resourceCache)
			{
				this._resourceCache = resourceCache;
				base.HorizontalAlignment = 2;
				base.MouseFilter = 0;
				LayoutContainer layoutContainer = new LayoutContainer();
				base.AddChild(layoutContainer);
				Label label = new Label
				{
					Text = letter.Letter().ToString(),
					VerticalAlignment = 3,
					HorizontalAlignment = 2,
					Align = 1,
					FontOverride = this._resourceCache.GetFont("/Fonts/NotoSansDisplay/NotoSansDisplay-Bold.ttf", 12),
					FontColorOverride = new Color?(Color.Gray),
					ToolTip = letter.Name(),
					MouseFilter = 0
				};
				layoutContainer.AddChild(label);
				LayoutContainer.SetAnchorPreset(label, 12, false);
				LayoutContainer.SetGrowVertical(label, 1);
				LayoutContainer.SetGrowHorizontal(label, 2);
				Texture texture = this._resourceCache.GetTexture("/Textures/Interface/WireHacking/contact.svg.96dpi.png");
				TextureRect textureRect = new TextureRect
				{
					Texture = texture,
					Modulate = Color.FromHex("#E1CA76", null)
				};
				layoutContainer.AddChild(textureRect);
				LayoutContainer.SetPosition(textureRect, new ValueTuple<float, float>(0f, 0f));
				TextureRect textureRect2 = new TextureRect
				{
					Texture = texture,
					Modulate = Color.FromHex("#E1CA76", null)
				};
				layoutContainer.AddChild(textureRect2);
				LayoutContainer.SetPosition(textureRect2, new ValueTuple<float, float>(0f, 60f));
				WiresMenu.WireControl.WireRender wireRender = new WiresMenu.WireControl.WireRender(color, isCut, flip, mirror, type, this._resourceCache);
				layoutContainer.AddChild(wireRender);
				LayoutContainer.SetPosition(wireRender, new ValueTuple<float, float>(2f, 16f));
				base.ToolTip = color.Name();
				base.MinSize = new ValueTuple<float, float>(20f, 102f);
			}

			// Token: 0x06000051 RID: 81 RVA: 0x00003F6C File Offset: 0x0000216C
			[NullableContext(1)]
			protected override void KeyBindDown(GUIBoundKeyEventArgs args)
			{
				base.KeyBindDown(args);
				if (args.Function != EngineKeyFunctions.UIClick)
				{
					return;
				}
				if (args.RelativePosition.Y > 20f && args.RelativePosition.Y < 60f)
				{
					Action wireClicked = this.WireClicked;
					if (wireClicked == null)
					{
						return;
					}
					wireClicked();
					return;
				}
				else
				{
					Action contactsClicked = this.ContactsClicked;
					if (contactsClicked == null)
					{
						return;
					}
					contactsClicked();
					return;
				}
			}

			// Token: 0x06000052 RID: 82 RVA: 0x00003FD8 File Offset: 0x000021D8
			protected override bool HasPoint(Vector2 point)
			{
				return base.HasPoint(point) && point.Y <= 80f;
			}

			// Token: 0x04000022 RID: 34
			[Nullable(1)]
			private IResourceCache _resourceCache;

			// Token: 0x04000023 RID: 35
			[Nullable(1)]
			private const string TextureContact = "/Textures/Interface/WireHacking/contact.svg.96dpi.png";

			// Token: 0x02000018 RID: 24
			[NullableContext(1)]
			[Nullable(0)]
			private sealed class WireRender : Control
			{
				// Token: 0x06000053 RID: 83 RVA: 0x00003FF8 File Offset: 0x000021F8
				public WireRender(WireColor color, bool isCut, bool flip, bool mirror, int type, IResourceCache resourceCache)
				{
					this._resourceCache = resourceCache;
					this._color = color;
					this._isCut = isCut;
					this._flip = flip;
					this._mirror = mirror;
					this._type = type;
					base.SetSize = new ValueTuple<float, float>(16f, 50f);
				}

				// Token: 0x06000054 RID: 84 RVA: 0x00004054 File Offset: 0x00002254
				protected override void Draw(DrawingHandleScreen handle)
				{
					Color value = this._color.ColorValue();
					Texture texture = this._resourceCache.GetTexture(this._isCut ? WiresMenu.WireControl.WireRender.TextureCut[this._type] : WiresMenu.WireControl.WireRender.TextureNormal[this._type]);
					float num = 0f;
					float num2 = (float)texture.Width + num;
					float num3 = 0f;
					float num4 = (float)texture.Height + num3;
					if (this._flip)
					{
						float num5 = num4;
						num4 = num3;
						num3 = num5;
					}
					if (this._mirror)
					{
						float num6 = num2;
						num2 = num;
						num = num6;
					}
					num *= this.UIScale;
					num2 *= this.UIScale;
					num3 *= this.UIScale;
					num4 *= this.UIScale;
					UIBox2 uibox;
					uibox..ctor(num, num3, num2, num4);
					if (this._isCut)
					{
						Color orange = Color.Orange;
						Texture texture2 = this._resourceCache.GetTexture(WiresMenu.WireControl.WireRender.TextureCopper[this._type]);
						handle.DrawTextureRect(texture2, uibox, new Color?(orange));
					}
					handle.DrawTextureRect(texture, uibox, new Color?(value));
				}

				// Token: 0x04000026 RID: 38
				private readonly WireColor _color;

				// Token: 0x04000027 RID: 39
				private readonly bool _isCut;

				// Token: 0x04000028 RID: 40
				private readonly bool _flip;

				// Token: 0x04000029 RID: 41
				private readonly bool _mirror;

				// Token: 0x0400002A RID: 42
				private readonly int _type;

				// Token: 0x0400002B RID: 43
				private static readonly string[] TextureNormal = new string[]
				{
					"/Textures/Interface/WireHacking/wire_1.svg.96dpi.png",
					"/Textures/Interface/WireHacking/wire_2.svg.96dpi.png"
				};

				// Token: 0x0400002C RID: 44
				private static readonly string[] TextureCut = new string[]
				{
					"/Textures/Interface/WireHacking/wire_1_cut.svg.96dpi.png",
					"/Textures/Interface/WireHacking/wire_2_cut.svg.96dpi.png"
				};

				// Token: 0x0400002D RID: 45
				private static readonly string[] TextureCopper = new string[]
				{
					"/Textures/Interface/WireHacking/wire_1_copper.svg.96dpi.png",
					"/Textures/Interface/WireHacking/wire_2_copper.svg.96dpi.png"
				};

				// Token: 0x0400002E RID: 46
				private readonly IResourceCache _resourceCache;
			}
		}

		// Token: 0x02000019 RID: 25
		[Nullable(0)]
		private sealed class StatusLight : Control
		{
			// Token: 0x06000056 RID: 86 RVA: 0x000041B8 File Offset: 0x000023B8
			public StatusLight(StatusLightData data, IResourceCache resourceCache)
			{
				WiresMenu.StatusLight.<>c__DisplayClass2_0 CS$<>8__locals1 = new WiresMenu.StatusLight.<>c__DisplayClass2_0();
				Vector4 vector = Color.ToHsv(data.Color);
				vector.Z /= 2f;
				Color value = Color.FromHsv(vector);
				Control control = new Control();
				control.SetSize = new ValueTuple<float, float>(20f, 20f);
				control.Children.Add(new TextureRect
				{
					Texture = resourceCache.GetTexture("/Textures/Interface/WireHacking/light_off_base.svg.96dpi.png"),
					Stretch = 4,
					ModulateSelfOverride = new Color?(value)
				});
				Control.OrderedChildCollection children = control.Children;
				WiresMenu.StatusLight.<>c__DisplayClass2_0 CS$<>8__locals2 = CS$<>8__locals1;
				TextureRect textureRect = new TextureRect();
				textureRect.ModulateSelfOverride = new Color?(data.Color.WithAlpha(0.4f));
				textureRect.Stretch = 4;
				textureRect.Texture = resourceCache.GetTexture("/Textures/Interface/WireHacking/light_on_base.svg.96dpi.png");
				TextureRect textureRect2 = textureRect;
				CS$<>8__locals2.activeLight = textureRect;
				children.Add(textureRect2);
				Control control2 = control;
				CS$<>8__locals1.animation = null;
				switch (data.State)
				{
				case StatusLightState.Off:
					CS$<>8__locals1.activeLight.Visible = false;
					break;
				case StatusLightState.On:
					break;
				case StatusLightState.BlinkingFast:
					CS$<>8__locals1.animation = WiresMenu.StatusLight._blinkingFast;
					break;
				case StatusLightState.BlinkingSlow:
					CS$<>8__locals1.animation = WiresMenu.StatusLight._blinkingSlow;
					break;
				default:
					throw new ArgumentOutOfRangeException();
				}
				if (CS$<>8__locals1.animation != null)
				{
					CS$<>8__locals1.activeLight.PlayAnimation(CS$<>8__locals1.animation, "blink");
					TextureRect activeLight = CS$<>8__locals1.activeLight;
					activeLight.AnimationCompleted = (Action<string>)Delegate.Combine(activeLight.AnimationCompleted, new Action<string>(delegate(string s)
					{
						if (s == "blink")
						{
							CS$<>8__locals1.activeLight.PlayAnimation(CS$<>8__locals1.animation, s);
						}
					}));
				}
				Font font = resourceCache.GetFont("/Fonts/Boxfont-round/Boxfont Round.ttf", 12);
				BoxContainer boxContainer = new BoxContainer
				{
					Orientation = 0,
					SeparationOverride = new int?(4)
				};
				boxContainer.AddChild(new Label
				{
					Text = data.Text,
					FontOverride = font,
					FontColorOverride = new Color?(Color.FromHex("#A1A6AE", null)),
					VerticalAlignment = 2
				});
				boxContainer.AddChild(control2);
				boxContainer.AddChild(new Control
				{
					MinSize = new ValueTuple<float, float>(6f, 0f)
				});
				base.AddChild(boxContainer);
			}

			// Token: 0x0400002F RID: 47
			private static readonly Animation _blinkingFast = new Animation
			{
				Length = TimeSpan.FromSeconds(0.2),
				AnimationTracks = 
				{
					new AnimationTrackControlProperty
					{
						Property = "Modulate",
						InterpolationMode = 0,
						KeyFrames = 
						{
							new AnimationTrackProperty.KeyFrame(Color.White, 0f),
							new AnimationTrackProperty.KeyFrame(Color.Transparent, 0.1f),
							new AnimationTrackProperty.KeyFrame(Color.White, 0.1f)
						}
					}
				}
			};

			// Token: 0x04000030 RID: 48
			private static readonly Animation _blinkingSlow = new Animation
			{
				Length = TimeSpan.FromSeconds(0.8),
				AnimationTracks = 
				{
					new AnimationTrackControlProperty
					{
						Property = "Modulate",
						InterpolationMode = 0,
						KeyFrames = 
						{
							new AnimationTrackProperty.KeyFrame(Color.White, 0f),
							new AnimationTrackProperty.KeyFrame(Color.White, 0.3f),
							new AnimationTrackProperty.KeyFrame(Color.Transparent, 0.1f),
							new AnimationTrackProperty.KeyFrame(Color.Transparent, 0.3f),
							new AnimationTrackProperty.KeyFrame(Color.White, 0.1f)
						}
					}
				}
			};
		}

		// Token: 0x0200001B RID: 27
		[NullableContext(0)]
		private sealed class HelpPopup : Popup
		{
			// Token: 0x0600005A RID: 90 RVA: 0x00004594 File Offset: 0x00002794
			public HelpPopup()
			{
				RichTextLabel richTextLabel = new RichTextLabel();
				richTextLabel.SetMessage("Click on the gold contacts with a multitool in hand to pulse their wire.\nClick on the wires with a pair of wirecutters in hand to cut/mend them.\n\nThe lights at the top show the state of the machine, messing with wires will probably do stuff to them.\nWire layouts are different each round, but consistent between machines of the same type.");
				PanelContainer panelContainer = new PanelContainer();
				panelContainer.StyleClasses.Add("entity-tooltip");
				panelContainer.Children.Add(richTextLabel);
				base.AddChild(panelContainer);
			}

			// Token: 0x04000033 RID: 51
			[Nullable(1)]
			private const string Text = "Click on the gold contacts with a multitool in hand to pulse their wire.\nClick on the wires with a pair of wirecutters in hand to cut/mend them.\n\nThe lights at the top show the state of the machine, messing with wires will probably do stuff to them.\nWire layouts are different each round, but consistent between machines of the same type.";
		}
	}
}
