﻿using System;
using System.Runtime.CompilerServices;
using CompiledRobustXaml;
using Content.Client.Administration.UI.CustomControls;
using Robust.Client.AutoGenerated;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.Controls;
using Robust.Shared.Input;
using Robust.Shared.Localization;

namespace Content.Client.Administration.UI.Tabs.PlayerTab
{
	// Token: 0x02000498 RID: 1176
	[GenerateTypedNameReferences]
	public sealed class PlayerTabHeader : ContainerButton
	{
		// Token: 0x140000B4 RID: 180
		// (add) Token: 0x06001CF2 RID: 7410 RVA: 0x000A86A0 File Offset: 0x000A68A0
		// (remove) Token: 0x06001CF3 RID: 7411 RVA: 0x000A86D8 File Offset: 0x000A68D8
		[Nullable(2)]
		[method: NullableContext(2)]
		[Nullable(2)]
		public event Action<PlayerTabHeader.Header> OnHeaderClicked;

		// Token: 0x06001CF4 RID: 7412 RVA: 0x000A8710 File Offset: 0x000A6910
		public PlayerTabHeader()
		{
			PlayerTabHeader.!XamlIlPopulateTrampoline(this);
			this.UsernameLabel.OnKeyBindDown += this.UsernameClicked;
			this.CharacterLabel.OnKeyBindDown += this.CharacterClicked;
			this.JobLabel.OnKeyBindDown += this.JobClicked;
			this.AntagonistLabel.OnKeyBindDown += this.AntagonistClicked;
		}

		// Token: 0x06001CF5 RID: 7413 RVA: 0x000A8788 File Offset: 0x000A6988
		[NullableContext(1)]
		public Label GetHeader(PlayerTabHeader.Header header)
		{
			Label result;
			switch (header)
			{
			case PlayerTabHeader.Header.Username:
				result = this.UsernameLabel;
				break;
			case PlayerTabHeader.Header.Character:
				result = this.CharacterLabel;
				break;
			case PlayerTabHeader.Header.Job:
				result = this.JobLabel;
				break;
			case PlayerTabHeader.Header.Antagonist:
				result = this.AntagonistLabel;
				break;
			default:
				throw new ArgumentOutOfRangeException("header", header, null);
			}
			return result;
		}

		// Token: 0x06001CF6 RID: 7414 RVA: 0x000A87E4 File Offset: 0x000A69E4
		public void ResetHeaderText()
		{
			this.UsernameLabel.Text = Loc.GetString("player-tab-username");
			this.CharacterLabel.Text = Loc.GetString("player-tab-character");
			this.JobLabel.Text = Loc.GetString("player-tab-job");
			this.AntagonistLabel.Text = Loc.GetString("player-tab-antagonist");
		}

		// Token: 0x06001CF7 RID: 7415 RVA: 0x000A8845 File Offset: 0x000A6A45
		[NullableContext(1)]
		private void HeaderClicked(GUIBoundKeyEventArgs args, PlayerTabHeader.Header header)
		{
			if (args.Function != EngineKeyFunctions.UIClick)
			{
				return;
			}
			Action<PlayerTabHeader.Header> onHeaderClicked = this.OnHeaderClicked;
			if (onHeaderClicked != null)
			{
				onHeaderClicked(header);
			}
			args.Handle();
		}

		// Token: 0x06001CF8 RID: 7416 RVA: 0x000A8872 File Offset: 0x000A6A72
		[NullableContext(1)]
		private void UsernameClicked(GUIBoundKeyEventArgs args)
		{
			this.HeaderClicked(args, PlayerTabHeader.Header.Username);
		}

		// Token: 0x06001CF9 RID: 7417 RVA: 0x000A887C File Offset: 0x000A6A7C
		[NullableContext(1)]
		private void CharacterClicked(GUIBoundKeyEventArgs args)
		{
			this.HeaderClicked(args, PlayerTabHeader.Header.Character);
		}

		// Token: 0x06001CFA RID: 7418 RVA: 0x000A8886 File Offset: 0x000A6A86
		[NullableContext(1)]
		private void JobClicked(GUIBoundKeyEventArgs args)
		{
			this.HeaderClicked(args, PlayerTabHeader.Header.Job);
		}

		// Token: 0x06001CFB RID: 7419 RVA: 0x000A8890 File Offset: 0x000A6A90
		[NullableContext(1)]
		private void AntagonistClicked(GUIBoundKeyEventArgs args)
		{
			this.HeaderClicked(args, PlayerTabHeader.Header.Antagonist);
		}

		// Token: 0x06001CFC RID: 7420 RVA: 0x000A889C File Offset: 0x000A6A9C
		protected override void Dispose(bool disposing)
		{
			base.Dispose(disposing);
			if (disposing)
			{
				this.UsernameLabel.OnKeyBindDown += this.UsernameClicked;
				this.CharacterLabel.OnKeyBindDown += this.CharacterClicked;
				this.JobLabel.OnKeyBindDown += this.JobClicked;
				this.AntagonistLabel.OnKeyBindDown += this.AntagonistClicked;
			}
		}

		// Token: 0x17000602 RID: 1538
		// (get) Token: 0x06001CFD RID: 7421 RVA: 0x000A8387 File Offset: 0x000A6587
		public PanelContainer BackgroundColorPanel
		{
			get
			{
				return base.FindControl<PanelContainer>("BackgroundColorPanel");
			}
		}

		// Token: 0x17000603 RID: 1539
		// (get) Token: 0x06001CFE RID: 7422 RVA: 0x000A8394 File Offset: 0x000A6594
		private Label UsernameLabel
		{
			get
			{
				return base.FindControl<Label>("UsernameLabel");
			}
		}

		// Token: 0x17000604 RID: 1540
		// (get) Token: 0x06001CFF RID: 7423 RVA: 0x000A83A1 File Offset: 0x000A65A1
		private Label CharacterLabel
		{
			get
			{
				return base.FindControl<Label>("CharacterLabel");
			}
		}

		// Token: 0x17000605 RID: 1541
		// (get) Token: 0x06001D00 RID: 7424 RVA: 0x000A83AE File Offset: 0x000A65AE
		private Label JobLabel
		{
			get
			{
				return base.FindControl<Label>("JobLabel");
			}
		}

		// Token: 0x17000606 RID: 1542
		// (get) Token: 0x06001D01 RID: 7425 RVA: 0x000A83BB File Offset: 0x000A65BB
		private Label AntagonistLabel
		{
			get
			{
				return base.FindControl<Label>("AntagonistLabel");
			}
		}

		// Token: 0x06001D02 RID: 7426 RVA: 0x000A8910 File Offset: 0x000A6B10
		static void xaml(IServiceProvider A_0, ContainerButton A_1)
		{
			XamlIlContext.Context<ContainerButton> context = new XamlIlContext.Context<ContainerButton>(A_0, null, "resm:Content.Client.Administration.UI.Tabs.PlayerTab.PlayerTabHeader.xaml?assembly=Content.Client");
			context.RootObject = A_1;
			context.IntermediateRoot = A_1;
			A_1.EnableAllKeybinds = true;
			PanelContainer panelContainer = new PanelContainer();
			panelContainer.Name = "BackgroundColorPanel";
			Control control = panelContainer;
			context.RobustNameScope.Register("BackgroundColorPanel", control);
			panelContainer.Access = new AccessLevel?(0);
			control = panelContainer;
			A_1.XamlChildren.Add(control);
			BoxContainer boxContainer = new BoxContainer();
			boxContainer.Orientation = 0;
			boxContainer.HorizontalExpand = true;
			boxContainer.SeparationOverride = new int?(4);
			Label label = new Label();
			label.Name = "UsernameLabel";
			control = label;
			context.RobustNameScope.Register("UsernameLabel", control);
			label.SizeFlagsStretchRatio = 3f;
			label.HorizontalExpand = true;
			label.ClipText = true;
			label.Text = (string)new LocExtension("player-tab-username").ProvideValue();
			label.MouseFilter = 1;
			control = label;
			boxContainer.XamlChildren.Add(control);
			control = new VSeparator();
			boxContainer.XamlChildren.Add(control);
			Label label2 = new Label();
			label2.Name = "CharacterLabel";
			control = label2;
			context.RobustNameScope.Register("CharacterLabel", control);
			label2.SizeFlagsStretchRatio = 3f;
			label2.HorizontalExpand = true;
			label2.ClipText = true;
			label2.Text = (string)new LocExtension("player-tab-character").ProvideValue();
			label2.MouseFilter = 1;
			control = label2;
			boxContainer.XamlChildren.Add(control);
			control = new VSeparator();
			boxContainer.XamlChildren.Add(control);
			Label label3 = new Label();
			label3.Name = "JobLabel";
			control = label3;
			context.RobustNameScope.Register("JobLabel", control);
			label3.SizeFlagsStretchRatio = 3f;
			label3.HorizontalExpand = true;
			label3.ClipText = true;
			label3.Text = (string)new LocExtension("player-tab-job").ProvideValue();
			label3.MouseFilter = 1;
			control = label3;
			boxContainer.XamlChildren.Add(control);
			control = new VSeparator();
			boxContainer.XamlChildren.Add(control);
			Label label4 = new Label();
			label4.Name = "AntagonistLabel";
			control = label4;
			context.RobustNameScope.Register("AntagonistLabel", control);
			label4.SizeFlagsStretchRatio = 2f;
			label4.HorizontalExpand = true;
			label4.ClipText = true;
			label4.Text = (string)new LocExtension("player-tab-antagonist").ProvideValue();
			label4.MouseFilter = 1;
			control = label4;
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

		// Token: 0x06001D03 RID: 7427 RVA: 0x000A8C8A File Offset: 0x000A6E8A
		private static void !XamlIlPopulateTrampoline(PlayerTabHeader A_0)
		{
			PlayerTabHeader.Populate:Content.Client.Administration.UI.Tabs.PlayerTab.PlayerTabHeader.xaml(null, A_0);
		}

		// Token: 0x02000499 RID: 1177
		public enum Header
		{
			// Token: 0x04000E7E RID: 3710
			Username,
			// Token: 0x04000E7F RID: 3711
			Character,
			// Token: 0x04000E80 RID: 3712
			Job,
			// Token: 0x04000E81 RID: 3713
			Antagonist
		}
	}
}
