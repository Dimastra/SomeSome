﻿using System;
using System.Runtime.CompilerServices;
using System.Text;
using CompiledRobustXaml;
using Content.Shared.Forensics;
using Robust.Client.AutoGenerated;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.Controls;
using Robust.Client.UserInterface.CustomControls;
using Robust.Shared.IoC;
using Robust.Shared.Localization;
using Robust.Shared.Maths;
using Robust.Shared.Timing;

namespace Content.Client.Forensics
{
	// Token: 0x0200030D RID: 781
	[GenerateTypedNameReferences]
	public sealed class ForensicScannerMenu : DefaultWindow
	{
		// Token: 0x060013BC RID: 5052 RVA: 0x00074147 File Offset: 0x00072347
		public ForensicScannerMenu()
		{
			ForensicScannerMenu.!XamlIlPopulateTrampoline(this);
			IoCManager.InjectDependencies<ForensicScannerMenu>(this);
		}

		// Token: 0x060013BD RID: 5053 RVA: 0x0007415C File Offset: 0x0007235C
		public void UpdatePrinterState(bool disabled)
		{
			this.Print.Disabled = disabled;
		}

		// Token: 0x060013BE RID: 5054 RVA: 0x0007416C File Offset: 0x0007236C
		[NullableContext(1)]
		public void UpdateState(ForensicScannerBoundUserInterfaceState msg)
		{
			if (string.IsNullOrEmpty(msg.LastScannedName))
			{
				this.Print.Disabled = true;
				this.Clear.Disabled = true;
				this.Name.Text = string.Empty;
				this.Diagnostics.Text = string.Empty;
				return;
			}
			this.Print.Disabled = (msg.PrintReadyAt > this._gameTiming.CurTime);
			this.Clear.Disabled = false;
			this.Name.Text = msg.LastScannedName;
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.AppendLine(Loc.GetString("forensic-scanner-interface-fingerprints"));
			foreach (string value in msg.Fingerprints)
			{
				stringBuilder.AppendLine(value);
			}
			stringBuilder.AppendLine();
			stringBuilder.AppendLine(Loc.GetString("forensic-scanner-interface-fibers"));
			foreach (string value2 in msg.Fibers)
			{
				stringBuilder.AppendLine(value2);
			}
			this.Diagnostics.Text = stringBuilder.ToString();
		}

		// Token: 0x17000419 RID: 1049
		// (get) Token: 0x060013BF RID: 5055 RVA: 0x000742C8 File Offset: 0x000724C8
		public Button Print
		{
			get
			{
				return base.FindControl<Button>("Print");
			}
		}

		// Token: 0x1700041A RID: 1050
		// (get) Token: 0x060013C0 RID: 5056 RVA: 0x000559E6 File Offset: 0x00053BE6
		public Button Clear
		{
			get
			{
				return base.FindControl<Button>("Clear");
			}
		}

		// Token: 0x1700041B RID: 1051
		// (get) Token: 0x060013C1 RID: 5057 RVA: 0x000742D5 File Offset: 0x000724D5
		private Label Name
		{
			get
			{
				return base.FindControl<Label>("Name");
			}
		}

		// Token: 0x1700041C RID: 1052
		// (get) Token: 0x060013C2 RID: 5058 RVA: 0x0006E238 File Offset: 0x0006C438
		private Label Diagnostics
		{
			get
			{
				return base.FindControl<Label>("Diagnostics");
			}
		}

		// Token: 0x060013C3 RID: 5059 RVA: 0x000742E4 File Offset: 0x000724E4
		static void xaml(IServiceProvider A_0, DefaultWindow A_1)
		{
			XamlIlContext.Context<DefaultWindow> context = new XamlIlContext.Context<DefaultWindow>(A_0, null, "resm:Content.Client.Forensics.ForensicScannerMenu.xaml?assembly=Content.Client");
			context.RootObject = A_1;
			context.IntermediateRoot = A_1;
			A_1.Title = (string)new LocExtension("forensic-scanner-interface-title").ProvideValue();
			A_1.MinSize = new Vector2(350f, 200f);
			A_1.SetSize = new Vector2(350f, 500f);
			BoxContainer boxContainer = new BoxContainer();
			boxContainer.Orientation = 1;
			BoxContainer boxContainer2 = new BoxContainer();
			boxContainer2.Orientation = 0;
			Button button = new Button();
			button.Name = "Print";
			Control control = button;
			context.RobustNameScope.Register("Print", control);
			button.TextAlign = 1;
			button.HorizontalExpand = true;
			button.Access = new AccessLevel?(0);
			button.Disabled = true;
			button.Text = (string)new LocExtension("forensic-scanner-interface-print").ProvideValue();
			control = button;
			boxContainer2.XamlChildren.Add(control);
			Button button2 = new Button();
			button2.Name = "Clear";
			control = button2;
			context.RobustNameScope.Register("Clear", control);
			button2.TextAlign = 1;
			button2.HorizontalExpand = true;
			button2.Access = new AccessLevel?(0);
			button2.Disabled = true;
			button2.Text = (string)new LocExtension("forensic-scanner-interface-clear").ProvideValue();
			control = button2;
			boxContainer2.XamlChildren.Add(control);
			control = boxContainer2;
			boxContainer.XamlChildren.Add(control);
			Label label = new Label();
			label.Name = "Name";
			control = label;
			context.RobustNameScope.Register("Name", control);
			label.Align = 1;
			control = label;
			boxContainer.XamlChildren.Add(control);
			Label label2 = new Label();
			label2.Name = "Diagnostics";
			control = label2;
			context.RobustNameScope.Register("Diagnostics", control);
			label2.Text = (string)new LocExtension("forensic-scanner-interface-no-data").ProvideValue();
			control = label2;
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

		// Token: 0x060013C4 RID: 5060 RVA: 0x000745A5 File Offset: 0x000727A5
		private static void !XamlIlPopulateTrampoline(ForensicScannerMenu A_0)
		{
			ForensicScannerMenu.Populate:Content.Client.Forensics.ForensicScannerMenu.xaml(null, A_0);
		}

		// Token: 0x040009E6 RID: 2534
		[Nullable(1)]
		[Dependency]
		private readonly IGameTiming _gameTiming;
	}
}
