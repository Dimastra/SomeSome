﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using CompiledRobustXaml;
using Content.Shared.Fax;
using Robust.Client.AutoGenerated;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.Controls;
using Robust.Client.UserInterface.CustomControls;
using Robust.Shared.Localization;
using Robust.Shared.Maths;

namespace Content.Client.Fax.UI
{
	// Token: 0x02000319 RID: 793
	[GenerateTypedNameReferences]
	public sealed class FaxWindow : DefaultWindow
	{
		// Token: 0x1400007A RID: 122
		// (add) Token: 0x060013F9 RID: 5113 RVA: 0x00075508 File Offset: 0x00073708
		// (remove) Token: 0x060013FA RID: 5114 RVA: 0x00075540 File Offset: 0x00073740
		[Nullable(2)]
		[method: NullableContext(2)]
		[Nullable(2)]
		public event Action SendButtonPressed;

		// Token: 0x1400007B RID: 123
		// (add) Token: 0x060013FB RID: 5115 RVA: 0x00075578 File Offset: 0x00073778
		// (remove) Token: 0x060013FC RID: 5116 RVA: 0x000755B0 File Offset: 0x000737B0
		[Nullable(2)]
		[method: NullableContext(2)]
		[Nullable(2)]
		public event Action RefreshButtonPressed;

		// Token: 0x1400007C RID: 124
		// (add) Token: 0x060013FD RID: 5117 RVA: 0x000755E8 File Offset: 0x000737E8
		// (remove) Token: 0x060013FE RID: 5118 RVA: 0x00075620 File Offset: 0x00073820
		[Nullable(new byte[]
		{
			2,
			1
		})]
		[Nullable(new byte[]
		{
			2,
			1
		})]
		public event Action<string> PeerSelected;

		// Token: 0x060013FF RID: 5119 RVA: 0x00075658 File Offset: 0x00073858
		public FaxWindow()
		{
			FaxWindow.!XamlIlPopulateTrampoline(this);
			this.SendButton.OnPressed += delegate(BaseButton.ButtonEventArgs _)
			{
				Action sendButtonPressed = this.SendButtonPressed;
				if (sendButtonPressed == null)
				{
					return;
				}
				sendButtonPressed();
			};
			this.RefreshButton.OnPressed += delegate(BaseButton.ButtonEventArgs _)
			{
				Action refreshButtonPressed = this.RefreshButtonPressed;
				if (refreshButtonPressed == null)
				{
					return;
				}
				refreshButtonPressed();
			};
			this.PeerSelector.OnItemSelected += delegate(OptionButton.ItemSelectedEventArgs args)
			{
				Action<string> peerSelected = this.PeerSelected;
				if (peerSelected == null)
				{
					return;
				}
				peerSelected((string)args.Button.GetItemMetadata(args.Id));
			};
		}

		// Token: 0x06001400 RID: 5120 RVA: 0x000756B8 File Offset: 0x000738B8
		[NullableContext(1)]
		public void UpdateState(FaxUiState state)
		{
			this.SendButton.Disabled = !state.CanSend;
			this.FromLabel.Text = state.DeviceName;
			if (state.IsPaperInserted)
			{
				this.PaperStatusLabel.FontColorOverride = new Color?(Color.Green);
				this.PaperStatusLabel.Text = Loc.GetString("fax-machine-ui-paper-inserted");
			}
			else
			{
				this.PaperStatusLabel.FontColorOverride = new Color?(Color.Red);
				this.PaperStatusLabel.Text = Loc.GetString("fax-machine-ui-paper-not-inserted");
			}
			if (state.AvailablePeers.Count == 0)
			{
				this.PeerSelector.AddItem(Loc.GetString("fax-machine-ui-no-peers"), null);
				this.PeerSelector.Disabled = true;
			}
			if (this.PeerSelector.Disabled && state.AvailablePeers.Count != 0)
			{
				this.PeerSelector.Clear();
				this.PeerSelector.Disabled = false;
			}
			if (!string.IsNullOrEmpty(state.DestinationAddress) || state.AvailablePeers.Count == 0)
			{
				if (state.AvailablePeers.Count != 0)
				{
					this.PeerSelector.Clear();
					foreach (KeyValuePair<string, string> keyValuePair in state.AvailablePeers)
					{
						string text;
						string text2;
						keyValuePair.Deconstruct(out text, out text2);
						string text3 = text;
						string name = text2;
						int num = this.AddPeerSelect(name, text3);
						if (text3 == state.DestinationAddress)
						{
							this.PeerSelector.Select(num);
						}
					}
				}
				return;
			}
			Action<string> peerSelected = this.PeerSelected;
			if (peerSelected == null)
			{
				return;
			}
			peerSelected(state.AvailablePeers.First<KeyValuePair<string, string>>().Key);
		}

		// Token: 0x06001401 RID: 5121 RVA: 0x00075880 File Offset: 0x00073A80
		[NullableContext(1)]
		private int AddPeerSelect(string name, string address)
		{
			this.PeerSelector.AddItem(name, null);
			this.PeerSelector.SetItemMetadata(this.PeerSelector.ItemCount - 1, address);
			return this.PeerSelector.ItemCount - 1;
		}

		// Token: 0x17000422 RID: 1058
		// (get) Token: 0x06001402 RID: 5122 RVA: 0x000758C8 File Offset: 0x00073AC8
		private Label PaperStatusLabel
		{
			get
			{
				return base.FindControl<Label>("PaperStatusLabel");
			}
		}

		// Token: 0x17000423 RID: 1059
		// (get) Token: 0x06001403 RID: 5123 RVA: 0x000758D5 File Offset: 0x00073AD5
		private Label FromLabel
		{
			get
			{
				return base.FindControl<Label>("FromLabel");
			}
		}

		// Token: 0x17000424 RID: 1060
		// (get) Token: 0x06001404 RID: 5124 RVA: 0x000758E2 File Offset: 0x00073AE2
		private OptionButton PeerSelector
		{
			get
			{
				return base.FindControl<OptionButton>("PeerSelector");
			}
		}

		// Token: 0x17000425 RID: 1061
		// (get) Token: 0x06001405 RID: 5125 RVA: 0x000758EF File Offset: 0x00073AEF
		private Button SendButton
		{
			get
			{
				return base.FindControl<Button>("SendButton");
			}
		}

		// Token: 0x17000426 RID: 1062
		// (get) Token: 0x06001406 RID: 5126 RVA: 0x000758FC File Offset: 0x00073AFC
		private Button RefreshButton
		{
			get
			{
				return base.FindControl<Button>("RefreshButton");
			}
		}

		// Token: 0x0600140A RID: 5130 RVA: 0x00075958 File Offset: 0x00073B58
		static void xaml(IServiceProvider A_0, DefaultWindow A_1)
		{
			XamlIlContext.Context<DefaultWindow> context = new XamlIlContext.Context<DefaultWindow>(A_0, null, "resm:Content.Client.Fax.UI.FaxWindow.xaml?assembly=Content.Client");
			context.RootObject = A_1;
			context.IntermediateRoot = A_1;
			A_1.Title = (string)new LocExtension("fax-machine-ui-window").ProvideValue();
			A_1.MinWidth = 250f;
			BoxContainer boxContainer = new BoxContainer();
			boxContainer.Orientation = 1;
			boxContainer.VerticalExpand = true;
			BoxContainer boxContainer2 = new BoxContainer();
			boxContainer2.Orientation = 0;
			boxContainer2.HorizontalExpand = true;
			Control control = new Label
			{
				Text = (string)new LocExtension("fax-machine-ui-paper").ProvideValue()
			};
			boxContainer2.XamlChildren.Add(control);
			control = new Control
			{
				MinWidth = 4f
			};
			boxContainer2.XamlChildren.Add(control);
			Label label = new Label();
			label.Name = "PaperStatusLabel";
			control = label;
			context.RobustNameScope.Register("PaperStatusLabel", control);
			control = label;
			boxContainer2.XamlChildren.Add(control);
			control = boxContainer2;
			boxContainer.XamlChildren.Add(control);
			control = new Control
			{
				HorizontalExpand = true,
				MinHeight = 20f
			};
			boxContainer.XamlChildren.Add(control);
			BoxContainer boxContainer3 = new BoxContainer();
			boxContainer3.Orientation = 0;
			boxContainer3.HorizontalExpand = true;
			control = new Label
			{
				Text = (string)new LocExtension("fax-machine-ui-from").ProvideValue()
			};
			boxContainer3.XamlChildren.Add(control);
			control = new Control
			{
				MinWidth = 4f
			};
			boxContainer3.XamlChildren.Add(control);
			Label label2 = new Label();
			label2.Name = "FromLabel";
			control = label2;
			context.RobustNameScope.Register("FromLabel", control);
			control = label2;
			boxContainer3.XamlChildren.Add(control);
			control = boxContainer3;
			boxContainer.XamlChildren.Add(control);
			BoxContainer boxContainer4 = new BoxContainer();
			boxContainer4.Orientation = 0;
			boxContainer4.HorizontalExpand = true;
			control = new Label
			{
				Text = (string)new LocExtension("fax-machine-ui-to").ProvideValue()
			};
			boxContainer4.XamlChildren.Add(control);
			control = new Control
			{
				MinWidth = 4f
			};
			boxContainer4.XamlChildren.Add(control);
			OptionButton optionButton = new OptionButton();
			optionButton.Name = "PeerSelector";
			control = optionButton;
			context.RobustNameScope.Register("PeerSelector", control);
			optionButton.HorizontalExpand = true;
			control = optionButton;
			boxContainer4.XamlChildren.Add(control);
			control = boxContainer4;
			boxContainer.XamlChildren.Add(control);
			control = new Control
			{
				HorizontalExpand = true,
				MinHeight = 20f
			};
			boxContainer.XamlChildren.Add(control);
			BoxContainer boxContainer5 = new BoxContainer();
			boxContainer5.Orientation = 0;
			boxContainer5.HorizontalExpand = true;
			Button button = new Button();
			button.Name = "SendButton";
			control = button;
			context.RobustNameScope.Register("SendButton", control);
			button.Text = (string)new LocExtension("fax-machine-ui-send-button").ProvideValue();
			button.HorizontalExpand = true;
			button.Disabled = true;
			control = button;
			boxContainer5.XamlChildren.Add(control);
			Button button2 = new Button();
			button2.Name = "RefreshButton";
			control = button2;
			context.RobustNameScope.Register("RefreshButton", control);
			button2.Text = (string)new LocExtension("fax-machine-ui-refresh-button").ProvideValue();
			control = button2;
			boxContainer5.XamlChildren.Add(control);
			control = boxContainer5;
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

		// Token: 0x0600140B RID: 5131 RVA: 0x00075DD2 File Offset: 0x00073FD2
		private static void !XamlIlPopulateTrampoline(FaxWindow A_0)
		{
			FaxWindow.Populate:Content.Client.Fax.UI.FaxWindow.xaml(null, A_0);
		}
	}
}
