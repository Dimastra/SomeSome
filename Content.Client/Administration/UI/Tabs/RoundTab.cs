﻿using System;
using CompiledRobustXaml;
using Content.Client.Administration.UI.CustomControls;
using Robust.Client.AutoGenerated;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.Controls;
using Robust.Shared.Maths;

namespace Content.Client.Administration.UI.Tabs
{
	// Token: 0x02000494 RID: 1172
	[GenerateTypedNameReferences]
	public sealed class RoundTab : Control
	{
		// Token: 0x06001CC9 RID: 7369 RVA: 0x000A74E1 File Offset: 0x000A56E1
		public RoundTab()
		{
			RoundTab.!XamlIlPopulateTrampoline(this);
		}

		// Token: 0x06001CCA RID: 7370 RVA: 0x000A74F0 File Offset: 0x000A56F0
		static void xaml(IServiceProvider A_0, Control A_1)
		{
			XamlIlContext.Context<Control> context = new XamlIlContext.Context<Control>(A_0, null, "resm:Content.Client.Administration.UI.Tabs.RoundTab.xaml?assembly=Content.Client");
			context.RootObject = A_1;
			context.IntermediateRoot = A_1;
			A_1.Margin = new Thickness(4f, 4f, 4f, 4f);
			A_1.MinSize = new Vector2(50f, 50f);
			GridContainer gridContainer = new GridContainer();
			gridContainer.Columns = 3;
			Control control = new CommandButton
			{
				Command = "startround",
				Text = (string)new LocExtension("Start Round").ProvideValue()
			};
			gridContainer.XamlChildren.Add(control);
			control = new CommandButton
			{
				Command = "endround",
				Text = (string)new LocExtension("End Round").ProvideValue()
			};
			gridContainer.XamlChildren.Add(control);
			control = new CommandButton
			{
				Command = "restartround",
				Text = (string)new LocExtension("Restart Round").ProvideValue()
			};
			gridContainer.XamlChildren.Add(control);
			control = new CommandButton
			{
				Command = "restartroundnow",
				Text = (string)new LocExtension("administration-ui-round-tab-restart-round-now").ProvideValue()
			};
			gridContainer.XamlChildren.Add(control);
			control = gridContainer;
			A_1.XamlChildren.Add(control);
			if ((control = (A_1 as Control)) != null)
			{
				context.RobustNameScope.Absorb(control.NameScope);
				control.NameScope = context.RobustNameScope;
			}
			context.RobustNameScope.Complete();
		}

		// Token: 0x06001CCB RID: 7371 RVA: 0x000A76BB File Offset: 0x000A58BB
		private static void !XamlIlPopulateTrampoline(RoundTab A_0)
		{
			RoundTab.Populate:Content.Client.Administration.UI.Tabs.RoundTab.xaml(null, A_0);
		}
	}
}