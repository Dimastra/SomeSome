﻿using System;
using System.Runtime.CompilerServices;
using CompiledRobustXaml;
using Content.Client.Administration.UI.CustomControls;
using Content.Shared.Administration.BanList;
using Robust.Client.AutoGenerated;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.Controls;
using Robust.Shared.Localization;

namespace Content.Client.Administration.UI.BanList
{
	// Token: 0x020004DA RID: 1242
	[GenerateTypedNameReferences]
	public sealed class BanListLine : BoxContainer
	{
		// Token: 0x140000BE RID: 190
		// (add) Token: 0x06001FA2 RID: 8098 RVA: 0x000B8D00 File Offset: 0x000B6F00
		// (remove) Token: 0x06001FA3 RID: 8099 RVA: 0x000B8D38 File Offset: 0x000B6F38
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
		public event Func<BanListLine, bool> OnIdsClicked;

		// Token: 0x06001FA4 RID: 8100 RVA: 0x000B8D70 File Offset: 0x000B6F70
		[NullableContext(1)]
		public BanListLine(SharedServerBan ban)
		{
			BanListLine.!XamlIlPopulateTrampoline(this);
			this.Ban = ban;
			this.IdsHidden.OnPressed += this.IdsPressed;
			this.Reason.Text = ban.Reason;
			this.BanTime.Text = BanListLine.FormatDate(ban.BanTime);
			this.Expires.Text = ((ban.ExpirationTime == null) ? Loc.GetString("ban-list-permanent") : BanListLine.FormatDate(ban.ExpirationTime.Value));
			SharedServerUnban unban = ban.Unban;
			if (unban != null)
			{
				string @string = Loc.GetString("ban-list-unbanned", new ValueTuple<string, object>[]
				{
					new ValueTuple<string, object>("date", BanListLine.FormatDate(unban.UnbanTime))
				});
				string str = (unban.UnbanningAdmin == null) ? string.Empty : ("\n" + Loc.GetString("ban-list-unbanned-by", new ValueTuple<string, object>[]
				{
					new ValueTuple<string, object>("unbanner", unban.UnbanningAdmin)
				}));
				Label expires = this.Expires;
				expires.Text = expires.Text + "\n" + @string + str;
			}
			this.BanningAdmin.Text = ban.BanningAdminName;
		}

		// Token: 0x06001FA5 RID: 8101 RVA: 0x000B8EC0 File Offset: 0x000B70C0
		[NullableContext(1)]
		private static string FormatDate(DateTimeOffset date)
		{
			return date.ToString("MM/dd/yyyy h:mm tt");
		}

		// Token: 0x06001FA6 RID: 8102 RVA: 0x000B8ECE File Offset: 0x000B70CE
		[NullableContext(1)]
		private void IdsPressed(BaseButton.ButtonEventArgs buttonEventArgs)
		{
			Func<BanListLine, bool> onIdsClicked = this.OnIdsClicked;
			if (onIdsClicked == null)
			{
				return;
			}
			onIdsClicked(this);
		}

		// Token: 0x06001FA7 RID: 8103 RVA: 0x000B8EE2 File Offset: 0x000B70E2
		protected override void Dispose(bool disposing)
		{
			base.Dispose(disposing);
			this.IdsHidden.OnPressed -= this.IdsPressed;
			this.OnIdsClicked = null;
		}

		// Token: 0x170006EA RID: 1770
		// (get) Token: 0x06001FA8 RID: 8104 RVA: 0x000B8F09 File Offset: 0x000B7109
		private Button IdsHidden
		{
			get
			{
				return base.FindControl<Button>("IdsHidden");
			}
		}

		// Token: 0x170006EB RID: 1771
		// (get) Token: 0x06001FA9 RID: 8105 RVA: 0x000B8F16 File Offset: 0x000B7116
		private Label Reason
		{
			get
			{
				return base.FindControl<Label>("Reason");
			}
		}

		// Token: 0x170006EC RID: 1772
		// (get) Token: 0x06001FAA RID: 8106 RVA: 0x000B8F23 File Offset: 0x000B7123
		private Label BanTime
		{
			get
			{
				return base.FindControl<Label>("BanTime");
			}
		}

		// Token: 0x170006ED RID: 1773
		// (get) Token: 0x06001FAB RID: 8107 RVA: 0x000B8F30 File Offset: 0x000B7130
		private Label Expires
		{
			get
			{
				return base.FindControl<Label>("Expires");
			}
		}

		// Token: 0x170006EE RID: 1774
		// (get) Token: 0x06001FAC RID: 8108 RVA: 0x000B8F3D File Offset: 0x000B713D
		private Label BanningAdmin
		{
			get
			{
				return base.FindControl<Label>("BanningAdmin");
			}
		}

		// Token: 0x06001FAD RID: 8109 RVA: 0x000B8F4C File Offset: 0x000B714C
		static void xaml(IServiceProvider A_0, BoxContainer A_1)
		{
			XamlIlContext.Context<BoxContainer> context = new XamlIlContext.Context<BoxContainer>(A_0, null, "resm:Content.Client.Administration.UI.BanList.BanListLine.xaml?assembly=Content.Client");
			context.RootObject = A_1;
			context.IntermediateRoot = A_1;
			A_1.Orientation = 0;
			A_1.HorizontalExpand = true;
			A_1.SeparationOverride = new int?(4);
			BoxContainer boxContainer = new BoxContainer();
			boxContainer.Orientation = 1;
			boxContainer.SizeFlagsStretchRatio = 1f;
			boxContainer.HorizontalExpand = true;
			boxContainer.RectClipContent = true;
			Button button = new Button();
			button.Name = "IdsHidden";
			Control control = button;
			context.RobustNameScope.Register("IdsHidden", control);
			button.Text = (string)new LocExtension("ban-list-view").ProvideValue();
			button.HorizontalExpand = true;
			button.VerticalExpand = true;
			button.MouseFilter = 1;
			control = button;
			boxContainer.XamlChildren.Add(control);
			control = boxContainer;
			A_1.XamlChildren.Add(control);
			control = new VSeparator();
			A_1.XamlChildren.Add(control);
			Label label = new Label();
			label.Name = "Reason";
			control = label;
			context.RobustNameScope.Register("Reason", control);
			label.SizeFlagsStretchRatio = 6f;
			label.HorizontalExpand = true;
			label.VerticalExpand = true;
			control = label;
			A_1.XamlChildren.Add(control);
			control = new VSeparator();
			A_1.XamlChildren.Add(control);
			Label label2 = new Label();
			label2.Name = "BanTime";
			control = label2;
			context.RobustNameScope.Register("BanTime", control);
			label2.SizeFlagsStretchRatio = 2f;
			label2.HorizontalExpand = true;
			label2.ClipText = true;
			control = label2;
			A_1.XamlChildren.Add(control);
			control = new VSeparator();
			A_1.XamlChildren.Add(control);
			Label label3 = new Label();
			label3.Name = "Expires";
			control = label3;
			context.RobustNameScope.Register("Expires", control);
			label3.SizeFlagsStretchRatio = 4f;
			label3.HorizontalExpand = true;
			label3.ClipText = true;
			control = label3;
			A_1.XamlChildren.Add(control);
			control = new VSeparator();
			A_1.XamlChildren.Add(control);
			Label label4 = new Label();
			label4.Name = "BanningAdmin";
			control = label4;
			context.RobustNameScope.Register("BanningAdmin", control);
			label4.SizeFlagsStretchRatio = 2f;
			label4.HorizontalExpand = true;
			label4.ClipText = true;
			control = label4;
			A_1.XamlChildren.Add(control);
			control = new VSeparator();
			A_1.XamlChildren.Add(control);
			if ((control = (A_1 as Control)) != null)
			{
				context.RobustNameScope.Absorb(control.NameScope);
				control.NameScope = context.RobustNameScope;
			}
			context.RobustNameScope.Complete();
		}

		// Token: 0x06001FAE RID: 8110 RVA: 0x000B92AC File Offset: 0x000B74AC
		private static void !XamlIlPopulateTrampoline(BanListLine A_0)
		{
			BanListLine.Populate:Content.Client.Administration.UI.BanList.BanListLine.xaml(null, A_0);
		}

		// Token: 0x04000F21 RID: 3873
		[Nullable(1)]
		public readonly SharedServerBan Ban;
	}
}
