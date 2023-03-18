﻿using System;
using System.Runtime.CompilerServices;
using CompiledRobustXaml;
using Robust.Client.AutoGenerated;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.Controls;
using Robust.Client.UserInterface.CustomControls;

namespace Content.Client.Access.UI
{
	// Token: 0x020004FA RID: 1274
	[GenerateTypedNameReferences]
	public sealed class AgentIDCardWindow : DefaultWindow
	{
		// Token: 0x140000CE RID: 206
		// (add) Token: 0x06002058 RID: 8280 RVA: 0x000BB91C File Offset: 0x000B9B1C
		// (remove) Token: 0x06002059 RID: 8281 RVA: 0x000BB954 File Offset: 0x000B9B54
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
		public event Action<string> OnNameEntered;

		// Token: 0x140000CF RID: 207
		// (add) Token: 0x0600205A RID: 8282 RVA: 0x000BB98C File Offset: 0x000B9B8C
		// (remove) Token: 0x0600205B RID: 8283 RVA: 0x000BB9C4 File Offset: 0x000B9BC4
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
		public event Action<string> OnJobEntered;

		// Token: 0x0600205C RID: 8284 RVA: 0x000BB9F9 File Offset: 0x000B9BF9
		public AgentIDCardWindow()
		{
			AgentIDCardWindow.!XamlIlPopulateTrampoline(this);
			this.NameLineEdit.OnTextEntered += delegate(LineEdit.LineEditEventArgs e)
			{
				Action<string> onNameEntered = this.OnNameEntered;
				if (onNameEntered == null)
				{
					return;
				}
				onNameEntered(e.Text);
			};
			this.JobLineEdit.OnTextEntered += delegate(LineEdit.LineEditEventArgs e)
			{
				Action<string> onJobEntered = this.OnJobEntered;
				if (onJobEntered == null)
				{
					return;
				}
				onJobEntered(e.Text);
			};
		}

		// Token: 0x0600205D RID: 8285 RVA: 0x000BBA35 File Offset: 0x000B9C35
		[NullableContext(1)]
		public void SetCurrentName(string name)
		{
			this.NameLineEdit.Text = name;
		}

		// Token: 0x0600205E RID: 8286 RVA: 0x000BBA43 File Offset: 0x000B9C43
		[NullableContext(1)]
		public void SetCurrentJob(string job)
		{
			this.JobLineEdit.Text = job;
		}

		// Token: 0x17000700 RID: 1792
		// (get) Token: 0x0600205F RID: 8287 RVA: 0x000BBA51 File Offset: 0x000B9C51
		private Label CurrentName
		{
			get
			{
				return base.FindControl<Label>("CurrentName");
			}
		}

		// Token: 0x17000701 RID: 1793
		// (get) Token: 0x06002060 RID: 8288 RVA: 0x000BBA5E File Offset: 0x000B9C5E
		private LineEdit NameLineEdit
		{
			get
			{
				return base.FindControl<LineEdit>("NameLineEdit");
			}
		}

		// Token: 0x17000702 RID: 1794
		// (get) Token: 0x06002061 RID: 8289 RVA: 0x000BBA6B File Offset: 0x000B9C6B
		private Label CurrentJob
		{
			get
			{
				return base.FindControl<Label>("CurrentJob");
			}
		}

		// Token: 0x17000703 RID: 1795
		// (get) Token: 0x06002062 RID: 8290 RVA: 0x000BBA78 File Offset: 0x000B9C78
		private LineEdit JobLineEdit
		{
			get
			{
				return base.FindControl<LineEdit>("JobLineEdit");
			}
		}

		// Token: 0x06002065 RID: 8293 RVA: 0x000BBAB8 File Offset: 0x000B9CB8
		static void xaml(IServiceProvider A_0, DefaultWindow A_1)
		{
			XamlIlContext.Context<DefaultWindow> context = new XamlIlContext.Context<DefaultWindow>(A_0, null, "resm:Content.Client.Access.UI.AgentIDCardWindow.xaml?assembly=Content.Client");
			context.RootObject = A_1;
			context.IntermediateRoot = A_1;
			A_1.Title = (string)new LocExtension("agent-id-menu-title").ProvideValue();
			BoxContainer boxContainer = new BoxContainer();
			boxContainer.Orientation = 1;
			boxContainer.SeparationOverride = new int?(4);
			boxContainer.MinWidth = 150f;
			Label label = new Label();
			label.Name = "CurrentName";
			Control control = label;
			context.RobustNameScope.Register("CurrentName", control);
			label.Text = (string)new LocExtension("agent-id-card-current-name").ProvideValue();
			control = label;
			boxContainer.XamlChildren.Add(control);
			LineEdit lineEdit = new LineEdit();
			lineEdit.Name = "NameLineEdit";
			control = lineEdit;
			context.RobustNameScope.Register("NameLineEdit", control);
			control = lineEdit;
			boxContainer.XamlChildren.Add(control);
			Label label2 = new Label();
			label2.Name = "CurrentJob";
			control = label2;
			context.RobustNameScope.Register("CurrentJob", control);
			label2.Text = (string)new LocExtension("agent-id-card-current-job").ProvideValue();
			control = label2;
			boxContainer.XamlChildren.Add(control);
			LineEdit lineEdit2 = new LineEdit();
			lineEdit2.Name = "JobLineEdit";
			control = lineEdit2;
			context.RobustNameScope.Register("JobLineEdit", control);
			control = lineEdit2;
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

		// Token: 0x06002066 RID: 8294 RVA: 0x000BBCBE File Offset: 0x000B9EBE
		private static void !XamlIlPopulateTrampoline(AgentIDCardWindow A_0)
		{
			AgentIDCardWindow.Populate:Content.Client.Access.UI.AgentIDCardWindow.xaml(null, A_0);
		}
	}
}
