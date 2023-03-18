﻿using System;
using System.Runtime.CompilerServices;
using CompiledRobustXaml;
using Content.Client.UserInterface.Controls;
using Robust.Client.AutoGenerated;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.Controls;
using Robust.Shared.Maths;

namespace Content.Client.Borgs
{
	// Token: 0x0200041B RID: 1051
	[GenerateTypedNameReferences]
	public sealed class LawUIContainer : PanelContainer
	{
		// Token: 0x060019C3 RID: 6595 RVA: 0x00093CD1 File Offset: 0x00091ED1
		public LawUIContainer()
		{
			LawUIContainer.!XamlIlPopulateTrampoline(this);
		}

		// Token: 0x060019C4 RID: 6596 RVA: 0x00093CDF File Offset: 0x00091EDF
		[NullableContext(1)]
		public void SetHeading(string desc)
		{
			this.Title.Text = desc;
		}

		// Token: 0x060019C5 RID: 6597 RVA: 0x00093CED File Offset: 0x00091EED
		[NullableContext(1)]
		public void SetDescription(string desc)
		{
			this.Description.SetMessage(desc);
		}

		// Token: 0x17000552 RID: 1362
		// (get) Token: 0x060019C6 RID: 6598 RVA: 0x00093CFB File Offset: 0x00091EFB
		private NanoHeading Title
		{
			get
			{
				return base.FindControl<NanoHeading>("Title");
			}
		}

		// Token: 0x17000553 RID: 1363
		// (get) Token: 0x060019C7 RID: 6599 RVA: 0x00014240 File Offset: 0x00012440
		private RichTextLabel Description
		{
			get
			{
				return base.FindControl<RichTextLabel>("Description");
			}
		}

		// Token: 0x060019C8 RID: 6600 RVA: 0x00093D08 File Offset: 0x00091F08
		static void xaml(IServiceProvider A_0, PanelContainer A_1)
		{
			XamlIlContext.Context<PanelContainer> context = new XamlIlContext.Context<PanelContainer>(A_0, null, "resm:Content.Client.Borgs.LawUIContainer.xaml?assembly=Content.Client");
			context.RootObject = A_1;
			context.IntermediateRoot = A_1;
			A_1.Margin = new Thickness(12f, 12f, 12f, 0f);
			string item = "PanelBackgroundAngledDark";
			A_1.StyleClasses.Add(item);
			A_1.VerticalExpand = true;
			BoxContainer boxContainer = new BoxContainer();
			boxContainer.Orientation = 1;
			boxContainer.HorizontalExpand = true;
			NanoHeading nanoHeading = new NanoHeading();
			nanoHeading.Name = "Title";
			Control control = nanoHeading;
			context.RobustNameScope.Register("Title", control);
			control = nanoHeading;
			boxContainer.XamlChildren.Add(control);
			RichTextLabel richTextLabel = new RichTextLabel();
			richTextLabel.Name = "Description";
			control = richTextLabel;
			context.RobustNameScope.Register("Description", control);
			richTextLabel.Margin = new Thickness(12f, 6f, 6f, 6f);
			control = richTextLabel;
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

		// Token: 0x060019C9 RID: 6601 RVA: 0x00093E8D File Offset: 0x0009208D
		private static void !XamlIlPopulateTrampoline(LawUIContainer A_0)
		{
			LawUIContainer.Populate:Content.Client.Borgs.LawUIContainer.xaml(null, A_0);
		}
	}
}
