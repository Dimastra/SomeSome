﻿using System;
using System.Runtime.CompilerServices;
using CompiledRobustXaml;
using Robust.Client.AutoGenerated;
using Robust.Client.Graphics;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.Controls;
using Robust.Shared.Maths;

namespace Content.Client.Administration.UI.BanList
{
	// Token: 0x020004D9 RID: 1241
	[GenerateTypedNameReferences]
	public sealed class BanListIdsPopup : Popup
	{
		// Token: 0x06001F9B RID: 8091 RVA: 0x000B8A70 File Offset: 0x000B6C70
		[NullableContext(2)]
		public BanListIdsPopup(string id, string ip, string hwid, string guid)
		{
			BanListIdsPopup.!XamlIlPopulateTrampoline(this);
			this.ID.Text = id;
			this.IP.Text = ip;
			this.HWId.Text = hwid;
			this.GUID.Text = guid;
			base.UserInterfaceManager.ModalRoot.AddChild(this);
		}

		// Token: 0x170006E6 RID: 1766
		// (get) Token: 0x06001F9C RID: 8092 RVA: 0x000B8ACB File Offset: 0x000B6CCB
		private Label ID
		{
			get
			{
				return base.FindControl<Label>("ID");
			}
		}

		// Token: 0x170006E7 RID: 1767
		// (get) Token: 0x06001F9D RID: 8093 RVA: 0x000B8AD8 File Offset: 0x000B6CD8
		private Label IP
		{
			get
			{
				return base.FindControl<Label>("IP");
			}
		}

		// Token: 0x170006E8 RID: 1768
		// (get) Token: 0x06001F9E RID: 8094 RVA: 0x000B8AE5 File Offset: 0x000B6CE5
		private Label HWId
		{
			get
			{
				return base.FindControl<Label>("HWId");
			}
		}

		// Token: 0x170006E9 RID: 1769
		// (get) Token: 0x06001F9F RID: 8095 RVA: 0x000B8AF2 File Offset: 0x000B6CF2
		private Label GUID
		{
			get
			{
				return base.FindControl<Label>("GUID");
			}
		}

		// Token: 0x06001FA0 RID: 8096 RVA: 0x000B8B00 File Offset: 0x000B6D00
		static void xaml(IServiceProvider A_0, Popup A_1)
		{
			XamlIlContext.Context<Popup> context = new XamlIlContext.Context<Popup>(A_0, null, "resm:Content.Client.Administration.UI.BanList.BanListIdsPopup.xaml?assembly=Content.Client");
			context.RootObject = A_1;
			context.IntermediateRoot = A_1;
			PanelContainer panelContainer = new PanelContainer();
			panelContainer.PanelOverride = new StyleBoxFlat
			{
				BackgroundColor = Color.FromXaml("#252525")
			};
			BoxContainer boxContainer = new BoxContainer();
			boxContainer.Orientation = 1;
			Label label = new Label();
			label.Name = "ID";
			Control control = label;
			context.RobustNameScope.Register("ID", control);
			label.HorizontalExpand = true;
			control = label;
			boxContainer.XamlChildren.Add(control);
			Label label2 = new Label();
			label2.Name = "IP";
			control = label2;
			context.RobustNameScope.Register("IP", control);
			label2.HorizontalExpand = true;
			control = label2;
			boxContainer.XamlChildren.Add(control);
			Label label3 = new Label();
			label3.Name = "HWId";
			control = label3;
			context.RobustNameScope.Register("HWId", control);
			label3.HorizontalExpand = true;
			control = label3;
			boxContainer.XamlChildren.Add(control);
			Label label4 = new Label();
			label4.Name = "GUID";
			control = label4;
			context.RobustNameScope.Register("GUID", control);
			label4.HorizontalExpand = true;
			control = label4;
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

		// Token: 0x06001FA1 RID: 8097 RVA: 0x000B8CF6 File Offset: 0x000B6EF6
		private static void !XamlIlPopulateTrampoline(BanListIdsPopup A_0)
		{
			BanListIdsPopup.Populate:Content.Client.Administration.UI.BanList.BanListIdsPopup.xaml(null, A_0);
		}
	}
}