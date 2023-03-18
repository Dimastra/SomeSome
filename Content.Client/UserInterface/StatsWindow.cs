﻿using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using CompiledRobustXaml;
using Robust.Client.AutoGenerated;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.Controls;
using Robust.Client.UserInterface.CustomControls;
using Robust.Shared.IoC;
using Robust.Shared.Maths;

namespace Content.Client.UserInterface
{
	// Token: 0x0200006C RID: 108
	[GenerateTypedNameReferences]
	public sealed class StatsWindow : DefaultWindow
	{
		// Token: 0x060001F9 RID: 505 RVA: 0x0000E1DC File Offset: 0x0000C3DC
		public StatsWindow()
		{
			StatsWindow.!XamlIlPopulateTrampoline(this);
			IoCManager.InjectDependencies<StatsWindow>(this);
		}

		// Token: 0x060001FA RID: 506 RVA: 0x0000E1F4 File Offset: 0x0000C3F4
		[NullableContext(1)]
		public void UpdateValues(List<string> headers, List<string[]> values)
		{
			this.Values.DisposeAllChildren();
			this.Values.Columns = headers.Count;
			for (int i = 0; i < headers.Count; i++)
			{
				string text = headers[i];
				this.Values.AddChild(new Label
				{
					Text = text
				});
			}
			values.Sort((string[] x, string[] y) => string.Compare(x[0], y[0], StringComparison.CurrentCultureIgnoreCase));
			for (int j = 0; j < values.Count; j++)
			{
				string[] array = values[j];
				for (int k = 0; k < array.Length; k++)
				{
					this.Values.AddChild(new Label
					{
						Text = array[k]
					});
				}
			}
		}

		// Token: 0x17000045 RID: 69
		// (get) Token: 0x060001FB RID: 507 RVA: 0x0000E2B7 File Offset: 0x0000C4B7
		private GridContainer Values
		{
			get
			{
				return base.FindControl<GridContainer>("Values");
			}
		}

		// Token: 0x060001FC RID: 508 RVA: 0x0000E2C4 File Offset: 0x0000C4C4
		static void xaml(IServiceProvider A_0, DefaultWindow A_1)
		{
			XamlIlContext.Context<DefaultWindow> context = new XamlIlContext.Context<DefaultWindow>(A_0, null, "resm:Content.Client.UserInterface.StatsWindow.xaml?assembly=Content.Client");
			context.RootObject = A_1;
			context.IntermediateRoot = A_1;
			A_1.Title = "Stats window";
			A_1.MinSize = new Vector2(600f, 400f);
			BoxContainer boxContainer = new BoxContainer();
			boxContainer.Orientation = 1;
			ScrollContainer scrollContainer = new ScrollContainer();
			scrollContainer.HorizontalExpand = true;
			scrollContainer.VerticalExpand = true;
			scrollContainer.SizeFlagsStretchRatio = 6f;
			GridContainer gridContainer = new GridContainer();
			gridContainer.Name = "Values";
			Control control = gridContainer;
			context.RobustNameScope.Register("Values", control);
			control = gridContainer;
			scrollContainer.XamlChildren.Add(control);
			control = scrollContainer;
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

		// Token: 0x060001FD RID: 509 RVA: 0x0000E3F7 File Offset: 0x0000C5F7
		private static void !XamlIlPopulateTrampoline(StatsWindow A_0)
		{
			StatsWindow.Populate:Content.Client.UserInterface.StatsWindow.xaml(null, A_0);
		}
	}
}