﻿using System;
using System.Runtime.CompilerServices;
using CompiledRobustXaml;
using Content.Client.UserInterface.Systems.Actions.Controls;
using Content.Shared.Input;
using Robust.Client.AutoGenerated;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.Controls;
using Robust.Shared.Input;

namespace Content.Client.UserInterface.Systems.Actions.Widgets
{
	// Token: 0x020000C7 RID: 199
	[GenerateTypedNameReferences]
	public sealed class ActionsBar : UIWidget
	{
		// Token: 0x06000584 RID: 1412 RVA: 0x0001E528 File Offset: 0x0001C728
		public ActionsBar()
		{
			ActionsBar.!XamlIlPopulateTrampoline(this);
			ActionsBar.<>c__DisplayClass0_0 CS$<>8__locals1;
			CS$<>8__locals1.keys = ContentKeyFunctions.GetHotbarBoundKeys();
			for (int i = 1; i < CS$<>8__locals1.keys.Length; i++)
			{
				this.ActionsContainer.Children.Add(ActionsBar.<.ctor>g__MakeButton|0_0(i, ref CS$<>8__locals1));
			}
			this.ActionsContainer.Children.Add(ActionsBar.<.ctor>g__MakeButton|0_0(0, ref CS$<>8__locals1));
		}

		// Token: 0x170000D9 RID: 217
		// (get) Token: 0x06000585 RID: 1413 RVA: 0x0001E590 File Offset: 0x0001C790
		public ActionButtonContainer ActionsContainer
		{
			get
			{
				return base.FindControl<ActionButtonContainer>("ActionsContainer");
			}
		}

		// Token: 0x170000DA RID: 218
		// (get) Token: 0x06000586 RID: 1414 RVA: 0x0001E59D File Offset: 0x0001C79D
		public ActionPageButtons PageButtons
		{
			get
			{
				return base.FindControl<ActionPageButtons>("PageButtons");
			}
		}

		// Token: 0x06000587 RID: 1415 RVA: 0x0001E5AC File Offset: 0x0001C7AC
		[NullableContext(1)]
		[CompilerGenerated]
		internal static ActionButton <.ctor>g__MakeButton|0_0(int index, ref ActionsBar.<>c__DisplayClass0_0 A_1)
		{
			BoundKeyFunction value = A_1.keys[index];
			return new ActionButton
			{
				KeyBind = new BoundKeyFunction?(value),
				Label = 
				{
					Text = index.ToString()
				}
			};
		}

		// Token: 0x06000588 RID: 1416 RVA: 0x0001E5EC File Offset: 0x0001C7EC
		static void xaml(IServiceProvider A_0, ActionsBar A_1)
		{
			XamlIlContext.Context<ActionsBar> context = new XamlIlContext.Context<ActionsBar>(A_0, null, "resm:Content.Client.UserInterface.Systems.Actions.Widgets.ActionsBar.xaml?assembly=Content.Client");
			context.RootObject = A_1;
			context.IntermediateRoot = A_1;
			A_1.VerticalExpand = false;
			A_1.Orientation = 0;
			A_1.HorizontalExpand = false;
			BoxContainer boxContainer = new BoxContainer();
			boxContainer.Orientation = 1;
			ActionButtonContainer actionButtonContainer = new ActionButtonContainer();
			actionButtonContainer.HorizontalAlignment = 2;
			actionButtonContainer.VerticalAlignment = 2;
			actionButtonContainer.Name = "ActionsContainer";
			Control control = actionButtonContainer;
			context.RobustNameScope.Register("ActionsContainer", control);
			actionButtonContainer.Access = new AccessLevel?(0);
			control = actionButtonContainer;
			boxContainer.XamlChildren.Add(control);
			ActionPageButtons actionPageButtons = new ActionPageButtons();
			actionPageButtons.Name = "PageButtons";
			control = actionPageButtons;
			context.RobustNameScope.Register("PageButtons", control);
			actionPageButtons.Access = new AccessLevel?(0);
			control = actionPageButtons;
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

		// Token: 0x06000589 RID: 1417 RVA: 0x0001E75C File Offset: 0x0001C95C
		private static void !XamlIlPopulateTrampoline(ActionsBar A_0)
		{
			ActionsBar.Populate:Content.Client.UserInterface.Systems.Actions.Widgets.ActionsBar.xaml(null, A_0);
		}
	}
}
