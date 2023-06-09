﻿using System;
using System.Runtime.CompilerServices;
using CompiledRobustXaml;
using Content.Client.Stylesheets;
using Content.Client.UserInterface.Controls;
using Content.Shared.Voting;
using Robust.Client.AutoGenerated;
using Robust.Client.Console;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.Controls;
using Robust.Client.UserInterface.CustomControls;
using Robust.Shared.Console;
using Robust.Shared.IoC;
using Robust.Shared.Localization;
using Robust.Shared.Maths;
using Robust.Shared.Network;
using Robust.Shared.Timing;

namespace Content.Client.Voting.UI
{
	// Token: 0x02000048 RID: 72
	[GenerateTypedNameReferences]
	public sealed class VoteCallMenu : BaseWindow
	{
		// Token: 0x06000140 RID: 320 RVA: 0x0000A9BC File Offset: 0x00008BBC
		public VoteCallMenu()
		{
			IoCManager.InjectDependencies<VoteCallMenu>(this);
			VoteCallMenu.!XamlIlPopulateTrampoline(this);
			base.Stylesheet = IoCManager.Resolve<IStylesheetManager>().SheetSpace;
			this.CloseButton.OnPressed += delegate(BaseButton.ButtonEventArgs _)
			{
				this.Close();
			};
			for (int i = 0; i < VoteCallMenu.AvailableVoteTypes.Length; i++)
			{
				string item = VoteCallMenu.AvailableVoteTypes[i].Item1;
				this.VoteTypeButton.AddItem(Loc.GetString(item), new int?(i));
			}
			this.VoteTypeButton.OnItemSelected += this.VoteTypeSelected;
			OptionButton voteSecondButton = this.VoteSecondButton;
			Action<OptionButton.ItemSelectedEventArgs> action;
			if ((action = VoteCallMenu.<>O.<0>__VoteSecondSelected) == null)
			{
				action = (VoteCallMenu.<>O.<0>__VoteSecondSelected = new Action<OptionButton.ItemSelectedEventArgs>(VoteCallMenu.VoteSecondSelected));
			}
			voteSecondButton.OnItemSelected += action;
			this.CreateButton.OnPressed += this.CreatePressed;
		}

		// Token: 0x06000141 RID: 321 RVA: 0x0000AA91 File Offset: 0x00008C91
		protected override void Opened()
		{
			base.Opened();
			this._netManager.ClientSendMessage(new MsgVoteMenu());
			this._voteManager.CanCallVoteChanged += this.CanCallVoteChanged;
		}

		// Token: 0x06000142 RID: 322 RVA: 0x0000AAC0 File Offset: 0x00008CC0
		public override void Close()
		{
			base.Close();
			this._voteManager.CanCallVoteChanged -= this.CanCallVoteChanged;
		}

		// Token: 0x06000143 RID: 323 RVA: 0x0000AADF File Offset: 0x00008CDF
		protected override void FrameUpdate(FrameEventArgs args)
		{
			base.FrameUpdate(args);
			this.UpdateVoteTimeout();
		}

		// Token: 0x06000144 RID: 324 RVA: 0x0000AAEE File Offset: 0x00008CEE
		private void CanCallVoteChanged(bool obj)
		{
			if (!obj)
			{
				this.Close();
			}
		}

		// Token: 0x06000145 RID: 325 RVA: 0x0000AAFC File Offset: 0x00008CFC
		[NullableContext(1)]
		private void CreatePressed(BaseButton.ButtonEventArgs obj)
		{
			int selectedId = this.VoteTypeButton.SelectedId;
			ValueTuple<string, StandardVoteType, ValueTuple<string, string>[]> valueTuple = VoteCallMenu.AvailableVoteTypes[selectedId];
			StandardVoteType item = valueTuple.Item2;
			ValueTuple<string, string>[] item2 = valueTuple.Item3;
			if (item2 != null)
			{
				int selectedId2 = this.VoteSecondButton.SelectedId;
				string item3 = item2[selectedId2].Item2;
				IConsoleShell localShell = this._consoleHost.LocalShell;
				DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(12, 2);
				defaultInterpolatedStringHandler.AppendLiteral("createvote ");
				defaultInterpolatedStringHandler.AppendFormatted<StandardVoteType>(item);
				defaultInterpolatedStringHandler.AppendLiteral(" ");
				defaultInterpolatedStringHandler.AppendFormatted(item3);
				localShell.RemoteExecuteCommand(defaultInterpolatedStringHandler.ToStringAndClear());
			}
			else
			{
				IConsoleShell localShell2 = this._consoleHost.LocalShell;
				DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(11, 1);
				defaultInterpolatedStringHandler.AppendLiteral("createvote ");
				defaultInterpolatedStringHandler.AppendFormatted<StandardVoteType>(item);
				localShell2.RemoteExecuteCommand(defaultInterpolatedStringHandler.ToStringAndClear());
			}
			this.Close();
		}

		// Token: 0x06000146 RID: 326 RVA: 0x0000ABD4 File Offset: 0x00008DD4
		private void UpdateVoteTimeout()
		{
			StandardVoteType item = VoteCallMenu.AvailableVoteTypes[this.VoteTypeButton.SelectedId].Item2;
			TimeSpan t;
			bool flag = this._voteManager.CanCallStandardVote(item, out t);
			this.CreateButton.Disabled = !flag;
			this.VoteTypeTimeoutLabel.Visible = !flag;
			if (!flag)
			{
				if (t == TimeSpan.Zero)
				{
					this.VoteTypeTimeoutLabel.Text = Loc.GetString("ui-vote-type-not-available");
					return;
				}
				TimeSpan timeSpan = t - this._gameTiming.RealTime;
				this.VoteTypeTimeoutLabel.Text = Loc.GetString("ui-vote-type-timeout", new ValueTuple<string, object>[]
				{
					new ValueTuple<string, object>("remaining", timeSpan.ToString("mm\\:ss"))
				});
			}
		}

		// Token: 0x06000147 RID: 327 RVA: 0x0000AC99 File Offset: 0x00008E99
		[NullableContext(1)]
		private static void VoteSecondSelected(OptionButton.ItemSelectedEventArgs obj)
		{
			obj.Button.SelectId(obj.Id);
		}

		// Token: 0x06000148 RID: 328 RVA: 0x0000ACAC File Offset: 0x00008EAC
		[NullableContext(1)]
		private void VoteTypeSelected(OptionButton.ItemSelectedEventArgs obj)
		{
			this.VoteTypeButton.SelectId(obj.Id);
			ValueTuple<string, string>[] item = VoteCallMenu.AvailableVoteTypes[obj.Id].Item3;
			if (item == null)
			{
				this.VoteSecondButton.Visible = false;
				return;
			}
			this.VoteSecondButton.Visible = true;
			this.VoteSecondButton.Clear();
			for (int i = 0; i < item.Length; i++)
			{
				string item2 = item[i].Item1;
				this.VoteSecondButton.AddItem(Loc.GetString(item2), new int?(i));
			}
		}

		// Token: 0x06000149 RID: 329 RVA: 0x00003C56 File Offset: 0x00001E56
		protected override BaseWindow.DragMode GetDragModeFor(Vector2 relativeMousePos)
		{
			return 1;
		}

		// Token: 0x17000025 RID: 37
		// (get) Token: 0x0600014A RID: 330 RVA: 0x0000AD39 File Offset: 0x00008F39
		private TextureButton CloseButton
		{
			get
			{
				return base.FindControl<TextureButton>("CloseButton");
			}
		}

		// Token: 0x17000026 RID: 38
		// (get) Token: 0x0600014B RID: 331 RVA: 0x0000AD46 File Offset: 0x00008F46
		private OptionButton VoteTypeButton
		{
			get
			{
				return base.FindControl<OptionButton>("VoteTypeButton");
			}
		}

		// Token: 0x17000027 RID: 39
		// (get) Token: 0x0600014C RID: 332 RVA: 0x0000AD53 File Offset: 0x00008F53
		private OptionButton VoteSecondButton
		{
			get
			{
				return base.FindControl<OptionButton>("VoteSecondButton");
			}
		}

		// Token: 0x17000028 RID: 40
		// (get) Token: 0x0600014D RID: 333 RVA: 0x0000AD60 File Offset: 0x00008F60
		private Label VoteTypeTimeoutLabel
		{
			get
			{
				return base.FindControl<Label>("VoteTypeTimeoutLabel");
			}
		}

		// Token: 0x17000029 RID: 41
		// (get) Token: 0x0600014E RID: 334 RVA: 0x0000AD6D File Offset: 0x00008F6D
		private Button CreateButton
		{
			get
			{
				return base.FindControl<Button>("CreateButton");
			}
		}

		// Token: 0x06000151 RID: 337 RVA: 0x0000ADD0 File Offset: 0x00008FD0
		static void xaml(IServiceProvider A_0, VoteCallMenu A_1)
		{
			XamlIlContext.Context<VoteCallMenu> context = new XamlIlContext.Context<VoteCallMenu>(A_0, null, "resm:Content.Client.Voting.UI.VoteCallMenu.xaml?assembly=Content.Client");
			context.RootObject = A_1;
			context.IntermediateRoot = A_1;
			A_1.MouseFilter = 0;
			A_1.MinSize = new Vector2(350f, 150f);
			PanelContainer panelContainer = new PanelContainer();
			string item = "AngleRect";
			panelContainer.StyleClasses.Add(item);
			Control control = panelContainer;
			A_1.XamlChildren.Add(control);
			BoxContainer boxContainer = new BoxContainer();
			boxContainer.Orientation = 1;
			BoxContainer boxContainer2 = new BoxContainer();
			boxContainer2.Margin = new Thickness(8f, 0f, 8f, 0f);
			boxContainer2.Orientation = 0;
			Label label = new Label();
			label.Text = (string)new LocExtension("ui-vote-create-title").ProvideValue();
			label.HorizontalExpand = true;
			label.VAlign = 1;
			item = "LabelHeading";
			label.StyleClasses.Add(item);
			control = label;
			boxContainer2.XamlChildren.Add(control);
			TextureButton textureButton = new TextureButton();
			textureButton.Name = "CloseButton";
			control = textureButton;
			context.RobustNameScope.Register("CloseButton", control);
			item = "windowCloseButton";
			textureButton.StyleClasses.Add(item);
			textureButton.VerticalAlignment = 2;
			control = textureButton;
			boxContainer2.XamlChildren.Add(control);
			control = boxContainer2;
			boxContainer.XamlChildren.Add(control);
			control = new HighDivider();
			boxContainer.XamlChildren.Add(control);
			BoxContainer boxContainer3 = new BoxContainer();
			boxContainer3.Orientation = 1;
			boxContainer3.Margin = new Thickness(8f, 2f, 8f, 0f);
			boxContainer3.VerticalExpand = true;
			boxContainer3.VerticalAlignment = 1;
			BoxContainer boxContainer4 = new BoxContainer();
			boxContainer4.Orientation = 0;
			OptionButton optionButton = new OptionButton();
			optionButton.Name = "VoteTypeButton";
			control = optionButton;
			context.RobustNameScope.Register("VoteTypeButton", control);
			optionButton.HorizontalExpand = true;
			control = optionButton;
			boxContainer4.XamlChildren.Add(control);
			Control control2 = new Control();
			control2.HorizontalExpand = true;
			OptionButton optionButton2 = new OptionButton();
			optionButton2.Name = "VoteSecondButton";
			control = optionButton2;
			context.RobustNameScope.Register("VoteSecondButton", control);
			optionButton2.Visible = false;
			control = optionButton2;
			control2.XamlChildren.Add(control);
			control = control2;
			boxContainer4.XamlChildren.Add(control);
			control = boxContainer4;
			boxContainer3.XamlChildren.Add(control);
			Label label2 = new Label();
			label2.Name = "VoteTypeTimeoutLabel";
			control = label2;
			context.RobustNameScope.Register("VoteTypeTimeoutLabel", control);
			label2.Visible = false;
			control = label2;
			boxContainer3.XamlChildren.Add(control);
			control = boxContainer3;
			boxContainer.XamlChildren.Add(control);
			Button button = new Button();
			button.Margin = new Thickness(8f, 2f, 8f, 2f);
			button.Name = "CreateButton";
			control = button;
			context.RobustNameScope.Register("CreateButton", control);
			button.Text = (string)new LocExtension("ui-vote-create-button").ProvideValue();
			control = button;
			boxContainer.XamlChildren.Add(control);
			PanelContainer panelContainer2 = new PanelContainer();
			item = "LowDivider";
			panelContainer2.StyleClasses.Add(item);
			control = panelContainer2;
			boxContainer.XamlChildren.Add(control);
			Label label3 = new Label();
			label3.Margin = new Thickness(12f, 0f, 0f, 0f);
			item = "LabelSubText";
			label3.StyleClasses.Add(item);
			label3.Text = (string)new LocExtension("ui-vote-fluff").ProvideValue();
			control = label3;
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

		// Token: 0x06000152 RID: 338 RVA: 0x0000B26C File Offset: 0x0000946C
		private static void !XamlIlPopulateTrampoline(VoteCallMenu A_0)
		{
			VoteCallMenu.Populate:Content.Client.Voting.UI.VoteCallMenu.xaml(null, A_0);
		}

		// Token: 0x040000E6 RID: 230
		[Nullable(1)]
		[Dependency]
		private readonly IClientConsoleHost _consoleHost;

		// Token: 0x040000E7 RID: 231
		[Nullable(1)]
		[Dependency]
		private readonly IVoteManager _voteManager;

		// Token: 0x040000E8 RID: 232
		[Nullable(1)]
		[Dependency]
		private readonly IGameTiming _gameTiming;

		// Token: 0x040000E9 RID: 233
		[Nullable(1)]
		[Dependency]
		private readonly IClientNetManager _netManager;

		// Token: 0x040000EA RID: 234
		[TupleElementNames(new string[]
		{
			"name",
			"type",
			"secondaries",
			"name",
			"id"
		})]
		[Nullable(new byte[]
		{
			1,
			0,
			1,
			2,
			0,
			1,
			1
		})]
		public static readonly ValueTuple<string, StandardVoteType, ValueTuple<string, string>[]>[] AvailableVoteTypes = new ValueTuple<string, StandardVoteType, ValueTuple<string, string>[]>[]
		{
			new ValueTuple<string, StandardVoteType, ValueTuple<string, string>[]>("ui-vote-type-restart", StandardVoteType.Restart, null),
			new ValueTuple<string, StandardVoteType, ValueTuple<string, string>[]>("ui-vote-type-gamemode", StandardVoteType.Preset, null),
			new ValueTuple<string, StandardVoteType, ValueTuple<string, string>[]>("ui-vote-type-map", StandardVoteType.Map, null)
		};

		// Token: 0x02000049 RID: 73
		[CompilerGenerated]
		private static class <>O
		{
			// Token: 0x040000EB RID: 235
			[Nullable(new byte[]
			{
				0,
				1
			})]
			public static Action<OptionButton.ItemSelectedEventArgs> <0>__VoteSecondSelected;
		}
	}
}
