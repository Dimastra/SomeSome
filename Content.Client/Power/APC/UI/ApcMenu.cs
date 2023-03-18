﻿using System;
using System.Runtime.CompilerServices;
using CompiledRobustXaml;
using Content.Shared.APC;
using Robust.Client.AutoGenerated;
using Robust.Client.Graphics;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.Controls;
using Robust.Client.UserInterface.CustomControls;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Localization;
using Robust.Shared.Maths;

namespace Content.Client.Power.APC.UI
{
	// Token: 0x020001A8 RID: 424
	[GenerateTypedNameReferences]
	public sealed class ApcMenu : DefaultWindow
	{
		// Token: 0x06000B0F RID: 2831 RVA: 0x00040820 File Offset: 0x0003EA20
		[NullableContext(1)]
		public ApcMenu(ApcBoundUserInterface owner)
		{
			IoCManager.InjectDependencies<ApcMenu>(this);
			ApcMenu.!XamlIlPopulateTrampoline(this);
			this.BreakerButton.OnPressed += delegate(BaseButton.ButtonEventArgs _)
			{
				owner.BreakerPressed();
			};
		}

		// Token: 0x06000B10 RID: 2832 RVA: 0x00040864 File Offset: 0x0003EA64
		[NullableContext(1)]
		public void UpdateState(BoundUserInterfaceState state)
		{
			ApcBoundInterfaceState apcBoundInterfaceState = (ApcBoundInterfaceState)state;
			if (this.BreakerButton != null)
			{
				this.BreakerButton.Pressed = apcBoundInterfaceState.MainBreaker;
			}
			if (this.PowerLabel != null)
			{
				this.PowerLabel.Text = Loc.GetString("apc-menu-power-label", new ValueTuple<string, object>[]
				{
					new ValueTuple<string, object>("power", apcBoundInterfaceState.Power)
				});
			}
			if (this.ExternalPowerStateLabel != null)
			{
				switch (apcBoundInterfaceState.ApcExternalPower)
				{
				case ApcExternalPowerState.None:
					this.ExternalPowerStateLabel.Text = Loc.GetString("apc-menu-power-state-none");
					this.ExternalPowerStateLabel.SetOnlyStyleClass("PowerStateNone");
					break;
				case ApcExternalPowerState.Low:
					this.ExternalPowerStateLabel.Text = Loc.GetString("apc-menu-power-state-low");
					this.ExternalPowerStateLabel.SetOnlyStyleClass("PowerStateLow");
					break;
				case ApcExternalPowerState.Good:
					this.ExternalPowerStateLabel.Text = Loc.GetString("apc-menu-power-state-good");
					this.ExternalPowerStateLabel.SetOnlyStyleClass("PowerStateGood");
					break;
				default:
					throw new ArgumentOutOfRangeException();
				}
			}
			if (this.ChargeBar != null)
			{
				this.ChargeBar.Value = apcBoundInterfaceState.Charge;
				this.UpdateChargeBarColor(apcBoundInterfaceState.Charge);
				if (this.APCMenu != null)
				{
					float num = apcBoundInterfaceState.Charge / this.ChargeBar.MaxValue * 100f;
					this.ChargePercentage.Text = " " + num.ToString("0.00") + "%";
				}
			}
		}

		// Token: 0x06000B11 RID: 2833 RVA: 0x000409E0 File Offset: 0x0003EBE0
		private void UpdateChargeBarColor(float charge)
		{
			if (this.ChargeBar == null)
			{
				return;
			}
			float num = charge / this.ChargeBar.MaxValue;
			float num2;
			if (num <= 0.5f)
			{
				num /= 0.5f;
				num2 = MathHelper.Lerp(0f, 0.066f, num);
			}
			else
			{
				num = (num - 0.5f) / 0.5f;
				num2 = MathHelper.Lerp(0.066f, 0.33f, num);
			}
			ProgressBar chargeBar = this.ChargeBar;
			if (chargeBar.ForegroundStyleBoxOverride == null)
			{
				chargeBar.ForegroundStyleBoxOverride = new StyleBoxFlat();
			}
			((StyleBoxFlat)this.ChargeBar.ForegroundStyleBoxOverride).BackgroundColor = Color.FromHsv(new Vector4(num2, 1f, 0.8f, 1f));
		}

		// Token: 0x1700021D RID: 541
		// (get) Token: 0x06000B12 RID: 2834 RVA: 0x00040A91 File Offset: 0x0003EC91
		private DefaultWindow APCMenu
		{
			get
			{
				return base.FindControl<DefaultWindow>("APCMenu");
			}
		}

		// Token: 0x1700021E RID: 542
		// (get) Token: 0x06000B13 RID: 2835 RVA: 0x00040A9E File Offset: 0x0003EC9E
		private Button BreakerButton
		{
			get
			{
				return base.FindControl<Button>("BreakerButton");
			}
		}

		// Token: 0x1700021F RID: 543
		// (get) Token: 0x06000B14 RID: 2836 RVA: 0x00040AAB File Offset: 0x0003ECAB
		private Label PowerLabel
		{
			get
			{
				return base.FindControl<Label>("PowerLabel");
			}
		}

		// Token: 0x17000220 RID: 544
		// (get) Token: 0x06000B15 RID: 2837 RVA: 0x00040AB8 File Offset: 0x0003ECB8
		private Label ExternalPowerStateLabel
		{
			get
			{
				return base.FindControl<Label>("ExternalPowerStateLabel");
			}
		}

		// Token: 0x17000221 RID: 545
		// (get) Token: 0x06000B16 RID: 2838 RVA: 0x00040AC5 File Offset: 0x0003ECC5
		private ProgressBar ChargeBar
		{
			get
			{
				return base.FindControl<ProgressBar>("ChargeBar");
			}
		}

		// Token: 0x17000222 RID: 546
		// (get) Token: 0x06000B17 RID: 2839 RVA: 0x00040AD2 File Offset: 0x0003ECD2
		private Label ChargePercentage
		{
			get
			{
				return base.FindControl<Label>("ChargePercentage");
			}
		}

		// Token: 0x06000B18 RID: 2840 RVA: 0x00040AE0 File Offset: 0x0003ECE0
		static void xaml(IServiceProvider A_0, DefaultWindow A_1)
		{
			XamlIlContext.Context<DefaultWindow> context = new XamlIlContext.Context<DefaultWindow>(A_0, null, "resm:Content.Client.Power.APC.UI.ApcMenu.xaml?assembly=Content.Client");
			context.RootObject = A_1;
			context.IntermediateRoot = A_1;
			A_1.Name = "APCMenu";
			context.RobustNameScope.Register("APCMenu", A_1);
			A_1.Title = (string)new LocExtension("apc-menu-title").ProvideValue();
			A_1.Resizable = false;
			BoxContainer boxContainer = new BoxContainer();
			boxContainer.Orientation = 1;
			boxContainer.SeparationOverride = new int?(4);
			BoxContainer boxContainer2 = new BoxContainer();
			boxContainer2.Orientation = 0;
			Control control = new Label
			{
				Text = (string)new LocExtension("apc-menu-breaker-label").ProvideValue()
			};
			boxContainer2.XamlChildren.Add(control);
			Button button = new Button();
			button.Name = "BreakerButton";
			control = button;
			context.RobustNameScope.Register("BreakerButton", control);
			button.Text = (string)new LocExtension("apc-menu-breaker-button").ProvideValue();
			control = button;
			boxContainer2.XamlChildren.Add(control);
			control = boxContainer2;
			boxContainer.XamlChildren.Add(control);
			Label label = new Label();
			label.Name = "PowerLabel";
			control = label;
			context.RobustNameScope.Register("PowerLabel", control);
			control = label;
			boxContainer.XamlChildren.Add(control);
			BoxContainer boxContainer3 = new BoxContainer();
			boxContainer3.Orientation = 0;
			control = new Label
			{
				Text = (string)new LocExtension("apc-menu-external-label").ProvideValue()
			};
			boxContainer3.XamlChildren.Add(control);
			Label label2 = new Label();
			label2.Name = "ExternalPowerStateLabel";
			control = label2;
			context.RobustNameScope.Register("ExternalPowerStateLabel", control);
			label2.Text = (string)new LocExtension("apc-menu-power-state-good").ProvideValue();
			control = label2;
			boxContainer3.XamlChildren.Add(control);
			control = boxContainer3;
			boxContainer.XamlChildren.Add(control);
			BoxContainer boxContainer4 = new BoxContainer();
			boxContainer4.Orientation = 0;
			control = new Label
			{
				Text = (string)new LocExtension("apc-menu-charge-label").ProvideValue()
			};
			boxContainer4.XamlChildren.Add(control);
			ProgressBar progressBar = new ProgressBar();
			progressBar.Name = "ChargeBar";
			control = progressBar;
			context.RobustNameScope.Register("ChargeBar", control);
			progressBar.HorizontalExpand = true;
			progressBar.MinValue = 0f;
			progressBar.MaxValue = 1f;
			progressBar.Page = 0f;
			progressBar.Value = 0.5f;
			control = progressBar;
			boxContainer4.XamlChildren.Add(control);
			Label label3 = new Label();
			label3.Name = "ChargePercentage";
			control = label3;
			context.RobustNameScope.Register("ChargePercentage", control);
			control = label3;
			boxContainer4.XamlChildren.Add(control);
			control = boxContainer4;
			boxContainer.XamlChildren.Add(control);
			control = boxContainer;
			A_1.XamlChildren.Add(control);
			if ((control = (A_1 as Control)) != null)
			{
				context.RobustNameScope.Absorb(control.NameScope);
				control.NameScope = context.RobustNameScope;
			}
			context.RobustNameScope.Complete();
		}

		// Token: 0x06000B19 RID: 2841 RVA: 0x00040E88 File Offset: 0x0003F088
		private static void !XamlIlPopulateTrampoline(ApcMenu A_0)
		{
			ApcMenu.Populate:Content.Client.Power.APC.UI.ApcMenu.xaml(null, A_0);
		}
	}
}
