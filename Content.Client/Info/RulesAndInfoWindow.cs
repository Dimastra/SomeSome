using System;
using System.Runtime.CompilerServices;
using Content.Client.UserInterface.Systems.EscapeMenu;
using Robust.Client.ResourceManagement;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.Controls;
using Robust.Client.UserInterface.CustomControls;
using Robust.Shared.ContentPack;
using Robust.Shared.IoC;
using Robust.Shared.Localization;

namespace Content.Client.Info
{
	// Token: 0x020002BF RID: 703
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class RulesAndInfoWindow : DefaultWindow
	{
		// Token: 0x060011AB RID: 4523 RVA: 0x00068F2C File Offset: 0x0006712C
		public RulesAndInfoWindow()
		{
			IoCManager.InjectDependencies<RulesAndInfoWindow>(this);
			base.Title = Loc.GetString("ui-info-title");
			TabContainer tabContainer = new TabContainer();
			Info info = new Info();
			Info info2 = new Info();
			tabContainer.AddChild(info);
			tabContainer.AddChild(info2);
			TabContainer.SetTabTitle(info, Loc.GetString("ui-info-tab-rules"));
			TabContainer.SetTabTitle(info2, Loc.GetString("ui-info-tab-tutorial"));
			RulesAndInfoWindow.AddSection(info, this._rules.RulesSection());
			this.PopulateTutorial(info2);
			base.Contents.AddChild(tabContainer);
			base.SetSize = new ValueTuple<float, float>(650f, 650f);
		}

		// Token: 0x060011AC RID: 4524 RVA: 0x00068FD4 File Offset: 0x000671D4
		private void PopulateTutorial(Info tutorialList)
		{
			this.AddSection(tutorialList, Loc.GetString("ui-info-header-intro"), "Intro.txt", false);
			InfoControlsSection infoControlsSection = new InfoControlsSection();
			tutorialList.InfoContainer.AddChild(infoControlsSection);
			this.AddSection(tutorialList, Loc.GetString("ui-info-header-gameplay"), "Gameplay.txt", true);
			this.AddSection(tutorialList, Loc.GetString("ui-info-header-sandbox"), "Sandbox.txt", true);
			infoControlsSection.ControlsButton.OnPressed += delegate(BaseButton.ButtonEventArgs _)
			{
				base.UserInterfaceManager.GetUIController<OptionsUIController>().OpenWindow();
			};
		}

		// Token: 0x060011AD RID: 4525 RVA: 0x0006904F File Offset: 0x0006724F
		private static void AddSection(Info info, Control control)
		{
			info.InfoContainer.AddChild(control);
		}

		// Token: 0x060011AE RID: 4526 RVA: 0x0006905D File Offset: 0x0006725D
		private void AddSection(Info info, string title, string path, bool markup = false)
		{
			RulesAndInfoWindow.AddSection(info, RulesAndInfoWindow.MakeSection(title, path, markup, this._resourceManager));
		}

		// Token: 0x060011AF RID: 4527 RVA: 0x00069074 File Offset: 0x00067274
		private static Control MakeSection(string title, string path, bool markup, IResourceManager res)
		{
			return new InfoSection(title, res.ContentFileReadAllText("/Server Info/" + path), markup);
		}

		// Token: 0x040008B0 RID: 2224
		[Dependency]
		private readonly IResourceCache _resourceManager;

		// Token: 0x040008B1 RID: 2225
		[Dependency]
		private readonly RulesManager _rules;
	}
}
