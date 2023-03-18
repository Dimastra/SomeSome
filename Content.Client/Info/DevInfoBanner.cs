using System;
using Content.Client.Credits;
using Content.Shared.CCVar;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.Controls;
using Robust.Shared.Configuration;
using Robust.Shared.IoC;
using Robust.Shared.Localization;

namespace Content.Client.Info
{
	// Token: 0x020002B4 RID: 692
	public sealed class DevInfoBanner : BoxContainer
	{
		// Token: 0x06001189 RID: 4489 RVA: 0x00068698 File Offset: 0x00066898
		public DevInfoBanner()
		{
			BoxContainer boxContainer = new BoxContainer
			{
				Orientation = 0
			};
			base.AddChild(boxContainer);
			IUriOpener uriOpener = IoCManager.Resolve<IUriOpener>();
			IConfigurationManager configurationManager = IoCManager.Resolve<IConfigurationManager>();
			string bugReport = configurationManager.GetCVar<string>(CCVars.InfoLinksBugReport);
			if (bugReport != "")
			{
				Button button = new Button
				{
					Text = Loc.GetString("server-info-report-button")
				};
				button.OnPressed += delegate(BaseButton.ButtonEventArgs args)
				{
					uriOpener.OpenUri(bugReport);
				};
				boxContainer.AddChild(button);
			}
			Button button2 = new Button
			{
				Text = Loc.GetString("server-info-credits-button")
			};
			button2.OnPressed += delegate(BaseButton.ButtonEventArgs args)
			{
				new CreditsWindow().Open();
			};
			boxContainer.AddChild(button2);
		}
	}
}
