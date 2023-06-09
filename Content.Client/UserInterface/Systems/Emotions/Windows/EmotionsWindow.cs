﻿using System;
using CompiledRobustXaml;
using Robust.Client.AutoGenerated;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.Controls;
using Robust.Client.UserInterface.CustomControls;

namespace Content.Client.UserInterface.Systems.Emotions.Windows
{
	// Token: 0x0200009B RID: 155
	[GenerateTypedNameReferences]
	public sealed class EmotionsWindow : DefaultWindow
	{
		// Token: 0x060003AE RID: 942 RVA: 0x00015BB4 File Offset: 0x00013DB4
		public EmotionsWindow()
		{
			EmotionsWindow.!XamlIlPopulateTrampoline(this);
		}

		// Token: 0x1700009E RID: 158
		// (get) Token: 0x060003AF RID: 943 RVA: 0x00015BC2 File Offset: 0x00013DC2
		public GridContainer EmotionsContainer
		{
			get
			{
				return base.FindControl<GridContainer>("EmotionsContainer");
			}
		}

		// Token: 0x060003B0 RID: 944 RVA: 0x00015BD0 File Offset: 0x00013DD0
		static void xaml(IServiceProvider A_0, EmotionsWindow A_1)
		{
			XamlIlContext.Context<EmotionsWindow> context = new XamlIlContext.Context<EmotionsWindow>(A_0, null, "resm:Content.Client.UserInterface.Systems.Emotions.Windows.EmotionsWindow.xaml?assembly=Content.Client");
			context.RootObject = A_1;
			context.IntermediateRoot = A_1;
			A_1.Title = "Emotions Menu";
			A_1.VerticalExpand = true;
			A_1.HorizontalExpand = true;
			A_1.MinHeight = 250f;
			A_1.MinWidth = 300f;
			GridContainer gridContainer = new GridContainer();
			gridContainer.Name = "EmotionsContainer";
			Control control = gridContainer;
			context.RobustNameScope.Register("EmotionsContainer", control);
			gridContainer.Access = new AccessLevel?(0);
			gridContainer.Columns = 3;
			control = gridContainer;
			A_1.XamlChildren.Add(control);
			if ((control = (A_1 as Control)) != null)
			{
				context.RobustNameScope.Absorb(control.NameScope);
				control.NameScope = context.RobustNameScope;
			}
			context.RobustNameScope.Complete();
		}

		// Token: 0x060003B1 RID: 945 RVA: 0x00015CD9 File Offset: 0x00013ED9
		private static void !XamlIlPopulateTrampoline(EmotionsWindow A_0)
		{
			EmotionsWindow.Populate:Content.Client.UserInterface.Systems.Emotions.Windows.EmotionsWindow.xaml(null, A_0);
		}
	}
}
