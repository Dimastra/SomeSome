﻿using System;
using CompiledRobustXaml;
using Content.Client.Administration.UI.CustomControls;
using Robust.Client.AutoGenerated;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.Controls;

namespace Content.Client.Administration.UI.BanList
{
	// Token: 0x020004D8 RID: 1240
	[GenerateTypedNameReferences]
	public sealed class BanListHeader : ContainerButton
	{
		// Token: 0x06001F97 RID: 8087 RVA: 0x000B875D File Offset: 0x000B695D
		public BanListHeader()
		{
			BanListHeader.!XamlIlPopulateTrampoline(this);
		}

		// Token: 0x170006E5 RID: 1765
		// (get) Token: 0x06001F98 RID: 8088 RVA: 0x0003A6BC File Offset: 0x000388BC
		public PanelContainer BackgroundPanel
		{
			get
			{
				return base.FindControl<PanelContainer>("BackgroundPanel");
			}
		}

		// Token: 0x06001F99 RID: 8089 RVA: 0x000B876C File Offset: 0x000B696C
		static void xaml(IServiceProvider A_0, ContainerButton A_1)
		{
			XamlIlContext.Context<ContainerButton> context = new XamlIlContext.Context<ContainerButton>(A_0, null, "resm:Content.Client.Administration.UI.BanList.BanListHeader.xaml?assembly=Content.Client");
			context.RootObject = A_1;
			context.IntermediateRoot = A_1;
			PanelContainer panelContainer = new PanelContainer();
			panelContainer.Name = "BackgroundPanel";
			Control control = panelContainer;
			context.RobustNameScope.Register("BackgroundPanel", control);
			panelContainer.Access = new AccessLevel?(0);
			BoxContainer boxContainer = new BoxContainer();
			boxContainer.Orientation = 0;
			boxContainer.HorizontalExpand = true;
			boxContainer.SeparationOverride = new int?(4);
			control = new Label
			{
				Text = (string)new LocExtension("ban-list-header-ids").ProvideValue(),
				SizeFlagsStretchRatio = 1f,
				HorizontalExpand = true
			};
			boxContainer.XamlChildren.Add(control);
			control = new VSeparator();
			boxContainer.XamlChildren.Add(control);
			control = new Label
			{
				Text = (string)new LocExtension("ban-list-header-reason").ProvideValue(),
				SizeFlagsStretchRatio = 6f,
				HorizontalExpand = true
			};
			boxContainer.XamlChildren.Add(control);
			control = new VSeparator();
			boxContainer.XamlChildren.Add(control);
			control = new Label
			{
				Text = (string)new LocExtension("ban-list-header-time").ProvideValue(),
				SizeFlagsStretchRatio = 2f,
				HorizontalExpand = true
			};
			boxContainer.XamlChildren.Add(control);
			control = new VSeparator();
			boxContainer.XamlChildren.Add(control);
			control = new Label
			{
				Text = (string)new LocExtension("ban-list-header-expires").ProvideValue(),
				SizeFlagsStretchRatio = 4f,
				HorizontalExpand = true
			};
			boxContainer.XamlChildren.Add(control);
			control = new VSeparator();
			boxContainer.XamlChildren.Add(control);
			control = new Label
			{
				Text = (string)new LocExtension("ban-list-header-banning-admin").ProvideValue(),
				SizeFlagsStretchRatio = 2f,
				HorizontalExpand = true
			};
			boxContainer.XamlChildren.Add(control);
			control = new VSeparator();
			boxContainer.XamlChildren.Add(control);
			control = boxContainer;
			panelContainer.XamlChildren.Add(control);
			control = panelContainer;
			A_1.XamlChildren.Add(control);
			if ((control = (A_1 as Control)) != null)
			{
				context.RobustNameScope.Absorb(control.NameScope);
				control.NameScope = context.RobustNameScope;
			}
			context.RobustNameScope.Complete();
		}

		// Token: 0x06001F9A RID: 8090 RVA: 0x000B8A65 File Offset: 0x000B6C65
		private static void !XamlIlPopulateTrampoline(BanListHeader A_0)
		{
			BanListHeader.Populate:Content.Client.Administration.UI.BanList.BanListHeader.xaml(null, A_0);
		}
	}
}
