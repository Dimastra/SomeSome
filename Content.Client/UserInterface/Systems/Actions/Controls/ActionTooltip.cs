﻿using System;
using CompiledRobustXaml;
using Robust.Client.AutoGenerated;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.Controls;

namespace Content.Client.UserInterface.Systems.Actions.Controls
{
	// Token: 0x020000CD RID: 205
	[GenerateTypedNameReferences]
	public sealed class ActionTooltip : PanelContainer
	{
		// Token: 0x060005C7 RID: 1479 RVA: 0x0001F9B1 File Offset: 0x0001DBB1
		public ActionTooltip()
		{
			ActionTooltip.!XamlIlPopulateTrampoline(this);
		}

		// Token: 0x060005C8 RID: 1480 RVA: 0x0001F9C0 File Offset: 0x0001DBC0
		static void xaml(IServiceProvider A_0, ActionTooltip A_1)
		{
			XamlIlContext.Context<ActionTooltip> context = new XamlIlContext.Context<ActionTooltip>(A_0, null, "resm:Content.Client.UserInterface.Systems.Actions.Controls.ActionTooltip.xaml?assembly=Content.Client");
			context.RootObject = A_1;
			context.IntermediateRoot = A_1;
			string item = "StyleClassTooltipPanel";
			A_1.StyleClasses.Add(item);
			BoxContainer boxContainer = new BoxContainer();
			boxContainer.Orientation = 1;
			boxContainer.RectClipContent = true;
			RichTextLabel richTextLabel = new RichTextLabel();
			richTextLabel.MaxWidth = 350f;
			item = "StyleClassTooltipActionTitle";
			richTextLabel.StyleClasses.Add(item);
			Control control = richTextLabel;
			boxContainer.XamlChildren.Add(control);
			RichTextLabel richTextLabel2 = new RichTextLabel();
			richTextLabel2.MaxWidth = 350f;
			item = "StyleClassTooltipActionDescription";
			richTextLabel2.StyleClasses.Add(item);
			control = richTextLabel2;
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

		// Token: 0x060005C9 RID: 1481 RVA: 0x0001FAF4 File Offset: 0x0001DCF4
		private static void !XamlIlPopulateTrampoline(ActionTooltip A_0)
		{
			ActionTooltip.Populate:Content.Client.UserInterface.Systems.Actions.Controls.ActionTooltip.xaml(null, A_0);
		}
	}
}