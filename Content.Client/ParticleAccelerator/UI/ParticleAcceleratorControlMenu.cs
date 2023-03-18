using System;
using System.Runtime.CompilerServices;
using Content.Client.Resources;
using Content.Client.Stylesheets;
using Content.Client.UserInterface.Controls;
using Content.Shared.Singularity.Components;
using Robust.Client.Animations;
using Robust.Client.Graphics;
using Robust.Client.ResourceManagement;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.Controls;
using Robust.Client.UserInterface.CustomControls;
using Robust.Shared.IoC;
using Robust.Shared.Localization;
using Robust.Shared.Maths;
using Robust.Shared.Noise;
using Robust.Shared.Prototypes;
using Robust.Shared.Timing;

namespace Content.Client.ParticleAccelerator.UI
{
	// Token: 0x020001CC RID: 460
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class ParticleAcceleratorControlMenu : BaseWindow
	{
		// Token: 0x06000C1A RID: 3098 RVA: 0x00045E44 File Offset: 0x00044044
		public ParticleAcceleratorControlMenu(ParticleAcceleratorBoundUserInterface owner)
		{
			ParticleAcceleratorControlMenu.<>c__DisplayClass24_0 CS$<>8__locals1 = new ParticleAcceleratorControlMenu.<>c__DisplayClass24_0();
			CS$<>8__locals1.owner = owner;
			base..ctor();
			CS$<>8__locals1.<>4__this = this;
			base.SetSize = new ValueTuple<float, float>(400f, 320f);
			this._greyScaleShader = IoCManager.Resolve<IPrototypeManager>().Index<ShaderPrototype>("Greyscale").Instance();
			this.Owner = CS$<>8__locals1.owner;
			this._drawNoiseGenerator = new NoiseGenerator(0);
			this._drawNoiseGenerator.SetFrequency(0.5f);
			ParticleAcceleratorControlMenu.<>c__DisplayClass24_1 CS$<>8__locals2;
			CS$<>8__locals2.resourceCache = IoCManager.Resolve<IResourceCache>();
			Font font = CS$<>8__locals2.resourceCache.GetFont("/Fonts/Boxfont-round/Boxfont Round.ttf", 13);
			Texture texture = CS$<>8__locals2.resourceCache.GetTexture("/Textures/Interface/Nano/button.svg.96dpi.png");
			base.MouseFilter = 0;
			this._alarmControlAnimation = new Animation
			{
				Length = TimeSpan.FromSeconds(1.0),
				AnimationTracks = 
				{
					new AnimationTrackControlProperty
					{
						Property = "Visible",
						KeyFrames = 
						{
							new AnimationTrackProperty.KeyFrame(true, 0f),
							new AnimationTrackProperty.KeyFrame(false, 0.75f)
						}
					}
				}
			};
			StyleBoxTexture styleBoxTexture = new StyleBoxTexture
			{
				Texture = texture,
				Modulate = Color.FromHex("#252525", null)
			};
			styleBoxTexture.SetPatchMargin(15, 10f);
			StyleBoxTexture panelOverride = new StyleBoxTexture(styleBoxTexture)
			{
				Modulate = Color.FromHex("#202023", null)
			};
			base.AddChild(new PanelContainer
			{
				PanelOverride = styleBoxTexture,
				MouseFilter = 1
			});
			this._stateSpinBox = new SpinBox
			{
				Value = 0,
				IsValid = new Func<int, bool>(this.StrengthSpinBoxValid)
			};
			this._stateSpinBox.InitDefaultButtons();
			this._stateSpinBox.ValueChanged += this.PowerStateChanged;
			this._stateSpinBox.LineEditDisabled = true;
			this._offButton = new Button
			{
				ToggleMode = false,
				Text = Loc.GetString("particle-accelerator-control-menu-off-button"),
				StyleClasses = 
				{
					"OpenRight"
				}
			};
			this._offButton.OnPressed += delegate(BaseButton.ButtonEventArgs args)
			{
				CS$<>8__locals1.owner.SendEnableMessage(false);
			};
			this._onButton = new Button
			{
				ToggleMode = false,
				Text = Loc.GetString("particle-accelerator-control-menu-on-button"),
				StyleClasses = 
				{
					"OpenLeft"
				}
			};
			this._onButton.OnPressed += delegate(BaseButton.ButtonEventArgs args)
			{
				CS$<>8__locals1.owner.SendEnableMessage(true);
			};
			TextureButton textureButton = new TextureButton
			{
				StyleClasses = 
				{
					"windowCloseButton"
				},
				HorizontalAlignment = 3,
				Margin = new Thickness(0f, 0f, 8f, 0f)
			};
			textureButton.OnPressed += delegate(BaseButton.ButtonEventArgs args)
			{
				CS$<>8__locals1.<>4__this.Close();
			};
			Label label = new Label
			{
				HorizontalAlignment = 2,
				StyleClasses = 
				{
					"LabelSubText"
				},
				Text = Loc.GetString("particle-accelerator-control-menu-service-manual-reference")
			};
			this._drawLabel = new Label();
			Vector2 minSize;
			minSize..ctor(32f, 32f);
			BoxContainer boxContainer = new BoxContainer();
			boxContainer.Orientation = 1;
			Control.OrderedChildCollection children = boxContainer.Children;
			Control control = new Control();
			control.Margin = new Thickness(2f, 2f, 0f, 0f);
			control.Children.Add(new Label
			{
				Text = Loc.GetString("particle-accelerator-control-menu-device-version-label"),
				FontOverride = font,
				FontColorOverride = new Color?(StyleNano.NanoGold)
			});
			control.Children.Add(textureButton);
			children.Add(control);
			boxContainer.Children.Add(new PanelContainer
			{
				PanelOverride = new StyleBoxFlat
				{
					BackgroundColor = StyleNano.NanoGold
				},
				MinSize = new ValueTuple<float, float>(0f, 2f)
			});
			boxContainer.Children.Add(new Control
			{
				MinSize = new ValueTuple<float, float>(0f, 4f)
			});
			Control.OrderedChildCollection children2 = boxContainer.Children;
			BoxContainer boxContainer2 = new BoxContainer();
			boxContainer2.Orientation = 0;
			boxContainer2.VerticalExpand = true;
			Control.OrderedChildCollection children3 = boxContainer2.Children;
			BoxContainer boxContainer3 = new BoxContainer();
			boxContainer3.Orientation = 1;
			boxContainer3.Margin = new Thickness(4f, 0f, 0f, 0f);
			boxContainer3.HorizontalExpand = true;
			Control.OrderedChildCollection children4 = boxContainer3.Children;
			BoxContainer boxContainer4 = new BoxContainer();
			boxContainer4.Orientation = 0;
			boxContainer4.Children.Add(new Label
			{
				Text = Loc.GetString("particle-accelerator-control-menu-power-label") + " ",
				HorizontalExpand = true,
				HorizontalAlignment = 1
			});
			boxContainer4.Children.Add(this._offButton);
			boxContainer4.Children.Add(this._onButton);
			children4.Add(boxContainer4);
			Control.OrderedChildCollection children5 = boxContainer3.Children;
			BoxContainer boxContainer5 = new BoxContainer();
			boxContainer5.Orientation = 0;
			boxContainer5.Children.Add(new Label
			{
				Text = Loc.GetString("particle-accelerator-control-menu-strength-label") + " ",
				HorizontalExpand = true,
				HorizontalAlignment = 1
			});
			boxContainer5.Children.Add(this._stateSpinBox);
			children5.Add(boxContainer5);
			boxContainer3.Children.Add(new Control
			{
				MinSize = new ValueTuple<float, float>(0f, 10f)
			});
			boxContainer3.Children.Add(this._drawLabel);
			boxContainer3.Children.Add(new Control
			{
				VerticalExpand = true
			});
			Control.OrderedChildCollection children6 = boxContainer3.Children;
			BoxContainer boxContainer6 = new BoxContainer();
			boxContainer6.Orientation = 1;
			boxContainer6.Children.Add(new Label
			{
				Text = Loc.GetString("particle-accelerator-control-menu-alarm-control"),
				FontColorOverride = new Color?(Color.Red),
				Align = 1
			});
			boxContainer6.Children.Add(label);
			BoxContainer boxContainer7 = boxContainer6;
			this._alarmControl = boxContainer6;
			children6.Add(boxContainer7);
			children3.Add(boxContainer3);
			Control.OrderedChildCollection children7 = boxContainer2.Children;
			BoxContainer boxContainer8 = new BoxContainer();
			boxContainer8.Orientation = 1;
			boxContainer8.MinSize = new ValueTuple<float, float>(186f, 0f);
			Control.OrderedChildCollection children8 = boxContainer8.Children;
			Label label2 = new Label();
			label2.HorizontalAlignment = 2;
			Label label3 = label2;
			this._statusLabel = label2;
			children8.Add(label3);
			boxContainer8.Children.Add(new Control
			{
				MinSize = new ValueTuple<float, float>(0f, 20f)
			});
			Control.OrderedChildCollection children9 = boxContainer8.Children;
			PanelContainer panelContainer = new PanelContainer();
			panelContainer.HorizontalAlignment = 2;
			panelContainer.PanelOverride = panelOverride;
			Control.OrderedChildCollection children10 = panelContainer.Children;
			GridContainer gridContainer = new GridContainer();
			gridContainer.Columns = 3;
			gridContainer.VSeparationOverride = new int?(0);
			gridContainer.HSeparationOverride = new int?(0);
			gridContainer.Children.Add(new Control
			{
				MinSize = minSize
			});
			gridContainer.Children.Add(this._endCapTexture = CS$<>8__locals1.<.ctor>g__Segment|4("end_cap", "capc", ref CS$<>8__locals2));
			gridContainer.Children.Add(new Control
			{
				MinSize = minSize
			});
			gridContainer.Children.Add(this._controlBoxTexture = CS$<>8__locals1.<.ctor>g__Segment|4("control_box", "boxc", ref CS$<>8__locals2));
			gridContainer.Children.Add(this._fuelChamberTexture = CS$<>8__locals1.<.ctor>g__Segment|4("fuel_chamber", "chamberc", ref CS$<>8__locals2));
			gridContainer.Children.Add(new Control
			{
				MinSize = minSize
			});
			gridContainer.Children.Add(new Control
			{
				MinSize = minSize
			});
			gridContainer.Children.Add(this._powerBoxTexture = CS$<>8__locals1.<.ctor>g__Segment|4("power_box", "boxc", ref CS$<>8__locals2));
			gridContainer.Children.Add(new Control
			{
				MinSize = minSize
			});
			gridContainer.Children.Add(this._emitterLeftTexture = CS$<>8__locals1.<.ctor>g__Segment|4("emitter_left", "leftc", ref CS$<>8__locals2));
			gridContainer.Children.Add(this._emitterCenterTexture = CS$<>8__locals1.<.ctor>g__Segment|4("emitter_center", "centerc", ref CS$<>8__locals2));
			gridContainer.Children.Add(this._emitterRightTexture = CS$<>8__locals1.<.ctor>g__Segment|4("emitter_right", "rightc", ref CS$<>8__locals2));
			children10.Add(gridContainer);
			children9.Add(panelContainer);
			Control.OrderedChildCollection children11 = boxContainer8.Children;
			Button button = new Button();
			button.Text = Loc.GetString("particle-accelerator-control-menu-scan-parts-button");
			button.HorizontalAlignment = 2;
			Button button2 = button;
			this._scanButton = button;
			children11.Add(button2);
			children7.Add(boxContainer8);
			children2.Add(boxContainer2);
			Control.OrderedChildCollection children12 = boxContainer.Children;
			StripeBack stripeBack = new StripeBack();
			stripeBack.Children.Add(new Label
			{
				Margin = new Thickness(4f, 4f, 0f, 4f),
				Text = Loc.GetString("particle-accelerator-control-menu-check-containment-field-warning"),
				HorizontalAlignment = 2,
				StyleClasses = 
				{
					"LabelSubText"
				}
			});
			children12.Add(stripeBack);
			Control.OrderedChildCollection children13 = boxContainer.Children;
			BoxContainer boxContainer9 = new BoxContainer();
			boxContainer9.Orientation = 0;
			boxContainer9.Margin = new Thickness(12f, 0f, 0f, 0f);
			boxContainer9.Children.Add(new Label
			{
				Text = Loc.GetString("particle-accelerator-control-menu-foo-bar-baz"),
				StyleClasses = 
				{
					"LabelSubText"
				}
			});
			children13.Add(boxContainer9);
			base.AddChild(boxContainer);
			this._scanButton.OnPressed += delegate(BaseButton.ButtonEventArgs args)
			{
				CS$<>8__locals1.<>4__this.Owner.SendScanPartsMessage();
			};
			BoxContainer alarmControl = this._alarmControl;
			alarmControl.AnimationCompleted = (Action<string>)Delegate.Combine(alarmControl.AnimationCompleted, new Action<string>(delegate(string s)
			{
				if (CS$<>8__locals1.<>4__this._shouldContinueAnimating)
				{
					CS$<>8__locals1.<>4__this._alarmControl.PlayAnimation(CS$<>8__locals1.<>4__this._alarmControlAnimation, "warningAnim");
					return;
				}
				CS$<>8__locals1.<>4__this._alarmControl.Visible = false;
			}));
			this.UpdateUI(false, false, false, false);
		}

		// Token: 0x06000C1B RID: 3099 RVA: 0x000467DD File Offset: 0x000449DD
		private bool StrengthSpinBoxValid(int n)
		{
			return n >= 0 && n <= 3 && !this._blockSpinBox;
		}

		// Token: 0x06000C1C RID: 3100 RVA: 0x000467F4 File Offset: 0x000449F4
		private void PowerStateChanged([Nullable(2)] object sender, ValueChangedEventArgs e)
		{
			ParticleAcceleratorPowerState state;
			switch (e.Value)
			{
			case 0:
				state = ParticleAcceleratorPowerState.Standby;
				break;
			case 1:
				state = ParticleAcceleratorPowerState.Level0;
				break;
			case 2:
				state = ParticleAcceleratorPowerState.Level1;
				break;
			case 3:
				state = ParticleAcceleratorPowerState.Level2;
				break;
			default:
				return;
			}
			this._stateSpinBox.SetButtonDisabled(true);
			this.Owner.SendPowerStateMessage(state);
		}

		// Token: 0x06000C1D RID: 3101 RVA: 0x00003C56 File Offset: 0x00001E56
		protected override BaseWindow.DragMode GetDragModeFor(Vector2 relativeMousePos)
		{
			return 1;
		}

		// Token: 0x06000C1E RID: 3102 RVA: 0x00046848 File Offset: 0x00044A48
		public void DataUpdate(ParticleAcceleratorUIState uiState)
		{
			this._assembled = uiState.Assembled;
			this.UpdateUI(uiState.Assembled, uiState.InterfaceBlock, uiState.Enabled, uiState.WirePowerBlock);
			this._statusLabel.Text = Loc.GetString("particle-accelerator-control-menu-status-label", new ValueTuple<string, object>[]
			{
				new ValueTuple<string, object>("status", Loc.GetString(uiState.Assembled ? "particle-accelerator-control-menu-status-operational" : "particle-accelerator-control-menu-status-incomplete"))
			});
			this.UpdatePowerState(uiState.State, uiState.Enabled, uiState.Assembled, uiState.MaxLevel);
			this.UpdatePreview(uiState);
			this._lastDraw = uiState.PowerDraw;
			this._lastReceive = uiState.PowerReceive;
		}

		// Token: 0x06000C1F RID: 3103 RVA: 0x00046904 File Offset: 0x00044B04
		private void UpdatePowerState(ParticleAcceleratorPowerState state, bool enabled, bool assembled, ParticleAcceleratorPowerState maxState)
		{
			SpinBox stateSpinBox = this._stateSpinBox;
			int num;
			switch (state)
			{
			case ParticleAcceleratorPowerState.Standby:
				num = 0;
				break;
			case ParticleAcceleratorPowerState.Level0:
				num = 1;
				break;
			case ParticleAcceleratorPowerState.Level1:
				num = 2;
				break;
			case ParticleAcceleratorPowerState.Level2:
				num = 3;
				break;
			case ParticleAcceleratorPowerState.Level3:
				num = 4;
				break;
			default:
				num = 0;
				break;
			}
			stateSpinBox.OverrideValue(num);
			this._shouldContinueAnimating = false;
			this._alarmControl.StopAnimation("warningAnim");
			this._alarmControl.Visible = false;
			if (maxState == ParticleAcceleratorPowerState.Level3 && enabled && assembled)
			{
				this._shouldContinueAnimating = true;
				this._alarmControl.PlayAnimation(this._alarmControlAnimation, "warningAnim");
			}
		}

		// Token: 0x06000C20 RID: 3104 RVA: 0x000469A0 File Offset: 0x00044BA0
		private void UpdateUI(bool assembled, bool blocked, bool enabled, bool powerBlock)
		{
			this._onButton.Pressed = enabled;
			this._offButton.Pressed = !enabled;
			bool disabled = !assembled || blocked || powerBlock;
			this._onButton.Disabled = disabled;
			this._offButton.Disabled = disabled;
			this._scanButton.Disabled = blocked;
			bool flag = !assembled || blocked;
			this._stateSpinBox.SetButtonDisabled(flag);
			this._blockSpinBox = flag;
		}

		// Token: 0x06000C21 RID: 3105 RVA: 0x00046A10 File Offset: 0x00044C10
		private void UpdatePreview(ParticleAcceleratorUIState updateMessage)
		{
			this._endCapTexture.SetPowerState(updateMessage, updateMessage.EndCapExists);
			this._fuelChamberTexture.SetPowerState(updateMessage, updateMessage.FuelChamberExists);
			this._controlBoxTexture.SetPowerState(updateMessage, true);
			this._powerBoxTexture.SetPowerState(updateMessage, updateMessage.PowerBoxExists);
			this._emitterCenterTexture.SetPowerState(updateMessage, updateMessage.EmitterCenterExists);
			this._emitterLeftTexture.SetPowerState(updateMessage, updateMessage.EmitterLeftExists);
			this._emitterRightTexture.SetPowerState(updateMessage, updateMessage.EmitterRightExists);
		}

		// Token: 0x06000C22 RID: 3106 RVA: 0x00046A98 File Offset: 0x00044C98
		protected override void FrameUpdate(FrameEventArgs args)
		{
			base.FrameUpdate(args);
			if (!this._assembled)
			{
				this._drawLabel.Text = Loc.GetString("particle-accelerator-control-menu-draw-not-available");
				return;
			}
			this._time += args.DeltaSeconds;
			int value = 0;
			if (this._lastDraw != 0)
			{
				float noise = this._drawNoiseGenerator.GetNoise(this._time);
				value = (int)((float)this._lastDraw + noise * 5f);
			}
			Label drawLabel = this._drawLabel;
			string text = "particle-accelerator-control-menu-draw";
			ValueTuple<string, object>[] array = new ValueTuple<string, object>[2];
			int num = 0;
			string item = "watts";
			DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(0, 1);
			defaultInterpolatedStringHandler.AppendFormatted<int>(value, "##,##0");
			array[num] = new ValueTuple<string, object>(item, defaultInterpolatedStringHandler.ToStringAndClear());
			int num2 = 1;
			string item2 = "lastReceive";
			defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(0, 1);
			defaultInterpolatedStringHandler.AppendFormatted<int>(this._lastReceive, "##,##0");
			array[num2] = new ValueTuple<string, object>(item2, defaultInterpolatedStringHandler.ToStringAndClear());
			drawLabel.Text = Loc.GetString(text, array);
		}

		// Token: 0x040005C5 RID: 1477
		private readonly ShaderInstance _greyScaleShader;

		// Token: 0x040005C6 RID: 1478
		private readonly ParticleAcceleratorBoundUserInterface Owner;

		// Token: 0x040005C7 RID: 1479
		private readonly Label _drawLabel;

		// Token: 0x040005C8 RID: 1480
		private readonly NoiseGenerator _drawNoiseGenerator;

		// Token: 0x040005C9 RID: 1481
		private readonly Button _onButton;

		// Token: 0x040005CA RID: 1482
		private readonly Button _offButton;

		// Token: 0x040005CB RID: 1483
		private readonly Button _scanButton;

		// Token: 0x040005CC RID: 1484
		private readonly Label _statusLabel;

		// Token: 0x040005CD RID: 1485
		private readonly SpinBox _stateSpinBox;

		// Token: 0x040005CE RID: 1486
		private readonly BoxContainer _alarmControl;

		// Token: 0x040005CF RID: 1487
		private readonly Animation _alarmControlAnimation;

		// Token: 0x040005D0 RID: 1488
		private readonly ParticleAcceleratorControlMenu.PASegmentControl _endCapTexture;

		// Token: 0x040005D1 RID: 1489
		private readonly ParticleAcceleratorControlMenu.PASegmentControl _fuelChamberTexture;

		// Token: 0x040005D2 RID: 1490
		private readonly ParticleAcceleratorControlMenu.PASegmentControl _controlBoxTexture;

		// Token: 0x040005D3 RID: 1491
		private readonly ParticleAcceleratorControlMenu.PASegmentControl _powerBoxTexture;

		// Token: 0x040005D4 RID: 1492
		private readonly ParticleAcceleratorControlMenu.PASegmentControl _emitterCenterTexture;

		// Token: 0x040005D5 RID: 1493
		private readonly ParticleAcceleratorControlMenu.PASegmentControl _emitterRightTexture;

		// Token: 0x040005D6 RID: 1494
		private readonly ParticleAcceleratorControlMenu.PASegmentControl _emitterLeftTexture;

		// Token: 0x040005D7 RID: 1495
		private float _time;

		// Token: 0x040005D8 RID: 1496
		private int _lastDraw;

		// Token: 0x040005D9 RID: 1497
		private int _lastReceive;

		// Token: 0x040005DA RID: 1498
		private bool _blockSpinBox;

		// Token: 0x040005DB RID: 1499
		private bool _assembled;

		// Token: 0x040005DC RID: 1500
		private bool _shouldContinueAnimating;

		// Token: 0x020001CD RID: 461
		[Nullable(0)]
		private sealed class PASegmentControl : Control
		{
			// Token: 0x06000C23 RID: 3107 RVA: 0x00046B88 File Offset: 0x00044D88
			public PASegmentControl(ParticleAcceleratorControlMenu menu, IResourceCache cache, string name, string state)
			{
				this._menu = menu;
				this._baseState = name;
				this._rsi = cache.GetResource<RSIResource>("/Textures/Structures/Power/Generation/PA/" + name + ".rsi", true).RSI;
				TextureRect textureRect = new TextureRect();
				textureRect.Texture = this._rsi[state ?? ""].Frame0;
				TextureRect textureRect2 = textureRect;
				this._base = textureRect;
				base.AddChild(textureRect2);
				base.AddChild(this._unlit = new TextureRect());
				base.MinSize = this._rsi.Size;
			}

			// Token: 0x06000C24 RID: 3108 RVA: 0x00046C30 File Offset: 0x00044E30
			public void SetPowerState(ParticleAcceleratorUIState state, bool exists)
			{
				this._base.ShaderOverride = (exists ? null : this._menu._greyScaleShader);
				this._base.ModulateSelfOverride = (exists ? null : new Color?(new Color(127, 127, 127, byte.MaxValue)));
				if (!state.Enabled || !exists)
				{
					this._unlit.Visible = false;
					return;
				}
				this._unlit.Visible = true;
				string text;
				switch (state.State)
				{
				case ParticleAcceleratorPowerState.Standby:
					text = "_unlitp";
					break;
				case ParticleAcceleratorPowerState.Level0:
					text = "_unlitp0";
					break;
				case ParticleAcceleratorPowerState.Level1:
					text = "_unlitp1";
					break;
				case ParticleAcceleratorPowerState.Level2:
					text = "_unlitp2";
					break;
				case ParticleAcceleratorPowerState.Level3:
					text = "_unlitp3";
					break;
				default:
					text = "";
					break;
				}
				string str = text;
				RSI.State state2;
				if (!this._rsi.TryGetState(this._baseState + str, ref state2))
				{
					this._unlit.Visible = false;
					return;
				}
				this._unlit.Texture = state2.Frame0;
			}

			// Token: 0x040005DD RID: 1501
			private readonly ParticleAcceleratorControlMenu _menu;

			// Token: 0x040005DE RID: 1502
			private readonly string _baseState;

			// Token: 0x040005DF RID: 1503
			private readonly TextureRect _base;

			// Token: 0x040005E0 RID: 1504
			private readonly TextureRect _unlit;

			// Token: 0x040005E1 RID: 1505
			private readonly RSI _rsi;
		}
	}
}
