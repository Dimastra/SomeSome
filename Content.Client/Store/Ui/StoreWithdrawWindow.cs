﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using CompiledRobustXaml;
using Content.Shared.FixedPoint;
using Content.Shared.Store;
using Robust.Client.AutoGenerated;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.Controls;
using Robust.Client.UserInterface.CustomControls;
using Robust.Shared.IoC;
using Robust.Shared.Localization;
using Robust.Shared.Maths;
using Robust.Shared.Prototypes;

namespace Content.Client.Store.Ui
{
	// Token: 0x02000120 RID: 288
	[GenerateTypedNameReferences]
	public sealed class StoreWithdrawWindow : DefaultWindow
	{
		// Token: 0x1400003E RID: 62
		// (add) Token: 0x060007F2 RID: 2034 RVA: 0x0002E368 File Offset: 0x0002C568
		// (remove) Token: 0x060007F3 RID: 2035 RVA: 0x0002E3A0 File Offset: 0x0002C5A0
		[Nullable(new byte[]
		{
			2,
			1,
			1
		})]
		[Nullable(new byte[]
		{
			2,
			1,
			1
		})]
		public event Action<BaseButton.ButtonEventArgs, string, int> OnWithdrawAttempt;

		// Token: 0x060007F4 RID: 2036 RVA: 0x0002E3D5 File Offset: 0x0002C5D5
		public StoreWithdrawWindow()
		{
			StoreWithdrawWindow.!XamlIlPopulateTrampoline(this);
			IoCManager.InjectDependencies<StoreWithdrawWindow>(this);
		}

		// Token: 0x060007F5 RID: 2037 RVA: 0x0002E400 File Offset: 0x0002C600
		[NullableContext(1)]
		public void CreateCurrencyButtons(Dictionary<string, FixedPoint2> balance)
		{
			this._validCurrencies.Clear();
			foreach (KeyValuePair<string, FixedPoint2> keyValuePair in balance)
			{
				CurrencyPrototype value;
				if (this._prototypeManager.TryIndex<CurrencyPrototype>(keyValuePair.Key, ref value))
				{
					this._validCurrencies.Add(keyValuePair.Value, value);
				}
			}
			if (this._validCurrencies.Count < 1)
			{
				return;
			}
			this.ButtonContainer.Children.Clear();
			this._buttons.Clear();
			foreach (KeyValuePair<FixedPoint2, CurrencyPrototype> keyValuePair2 in this._validCurrencies)
			{
				StoreWithdrawWindow.CurrencyWithdrawButton button = new StoreWithdrawWindow.CurrencyWithdrawButton
				{
					Id = keyValuePair2.Value.ID,
					Amount = keyValuePair2.Key,
					MinHeight = 20f,
					Text = Loc.GetString("store-withdraw-button-ui", new ValueTuple<string, object>[]
					{
						new ValueTuple<string, object>("currency", Loc.GetString(keyValuePair2.Value.DisplayName, new ValueTuple<string, object>[]
						{
							new ValueTuple<string, object>("amount", keyValuePair2.Key)
						}))
					})
				};
				button.Disabled = false;
				button.OnPressed += delegate(BaseButton.ButtonEventArgs args)
				{
					Action<BaseButton.ButtonEventArgs, string, int> onWithdrawAttempt = this.OnWithdrawAttempt;
					if (onWithdrawAttempt != null)
					{
						onWithdrawAttempt(args, button.Id, this.WithdrawSlider.Value);
					}
					this.Close();
				};
				this._buttons.Add(button);
				this.ButtonContainer.AddChild(button);
			}
			int maxValue = this._validCurrencies.Keys.Max<FixedPoint2>().Int();
			this.WithdrawSlider.MinValue = 1;
			this.WithdrawSlider.MaxValue = maxValue;
			this.WithdrawSlider.OnValueChanged += this.OnValueChanged;
			this.OnValueChanged(this.WithdrawSlider.Value);
		}

		// Token: 0x060007F6 RID: 2038 RVA: 0x0002E650 File Offset: 0x0002C850
		public void OnValueChanged(int i)
		{
			foreach (StoreWithdrawWindow.CurrencyWithdrawButton currencyWithdrawButton in this._buttons)
			{
				currencyWithdrawButton.Disabled = (currencyWithdrawButton.Amount < this.WithdrawSlider.Value);
			}
		}

		// Token: 0x1700016D RID: 365
		// (get) Token: 0x060007F7 RID: 2039 RVA: 0x0002E6B8 File Offset: 0x0002C8B8
		private SliderIntInput WithdrawSlider
		{
			get
			{
				return base.FindControl<SliderIntInput>("WithdrawSlider");
			}
		}

		// Token: 0x1700016E RID: 366
		// (get) Token: 0x060007F8 RID: 2040 RVA: 0x00013974 File Offset: 0x00011B74
		private BoxContainer ButtonContainer
		{
			get
			{
				return base.FindControl<BoxContainer>("ButtonContainer");
			}
		}

		// Token: 0x060007F9 RID: 2041 RVA: 0x0002E6C8 File Offset: 0x0002C8C8
		static void xaml(IServiceProvider A_0, DefaultWindow A_1)
		{
			XamlIlContext.Context<DefaultWindow> context = new XamlIlContext.Context<DefaultWindow>(A_0, null, "resm:Content.Client.Store.Ui.StoreWithdrawWindow.xaml?assembly=Content.Client");
			context.RootObject = A_1;
			context.IntermediateRoot = A_1;
			A_1.Title = (string)new LocExtension("store-ui-default-withdraw-text").ProvideValue();
			A_1.MinSize = new Vector2(256f, 128f);
			BoxContainer boxContainer = new BoxContainer();
			boxContainer.HorizontalExpand = true;
			boxContainer.Orientation = 1;
			boxContainer.VerticalExpand = true;
			SliderIntInput sliderIntInput = new SliderIntInput();
			sliderIntInput.Name = "WithdrawSlider";
			Control control = sliderIntInput;
			context.RobustNameScope.Register("WithdrawSlider", control);
			sliderIntInput.HorizontalExpand = true;
			control = sliderIntInput;
			boxContainer.XamlChildren.Add(control);
			BoxContainer boxContainer2 = new BoxContainer();
			boxContainer2.Name = "ButtonContainer";
			control = boxContainer2;
			context.RobustNameScope.Register("ButtonContainer", control);
			boxContainer2.VerticalAlignment = 3;
			boxContainer2.Orientation = 1;
			boxContainer2.VerticalExpand = true;
			control = boxContainer2;
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

		// Token: 0x060007FA RID: 2042 RVA: 0x0002E854 File Offset: 0x0002CA54
		private static void !XamlIlPopulateTrampoline(StoreWithdrawWindow A_0)
		{
			StoreWithdrawWindow.Populate:Content.Client.Store.Ui.StoreWithdrawWindow.xaml(null, A_0);
		}

		// Token: 0x04000406 RID: 1030
		[Nullable(1)]
		[Dependency]
		private readonly IPrototypeManager _prototypeManager;

		// Token: 0x04000407 RID: 1031
		[Nullable(1)]
		private Dictionary<FixedPoint2, CurrencyPrototype> _validCurrencies = new Dictionary<FixedPoint2, CurrencyPrototype>();

		// Token: 0x04000408 RID: 1032
		[Nullable(1)]
		private HashSet<StoreWithdrawWindow.CurrencyWithdrawButton> _buttons = new HashSet<StoreWithdrawWindow.CurrencyWithdrawButton>();

		// Token: 0x02000121 RID: 289
		private sealed class CurrencyWithdrawButton : Button
		{
			// Token: 0x0400040A RID: 1034
			[Nullable(2)]
			public string Id;

			// Token: 0x0400040B RID: 1035
			public FixedPoint2 Amount = FixedPoint2.Zero;
		}
	}
}