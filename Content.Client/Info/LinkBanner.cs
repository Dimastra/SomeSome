using System;
using System.Runtime.CompilerServices;
using Content.Client.Changelog;
using Content.Client.UserInterface.Systems.EscapeMenu;
using Content.Client.White.Stalin;
using Content.Shared.CCVar;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.Controls;
using Robust.Shared.Collections;
using Robust.Shared.Configuration;
using Robust.Shared.IoC;
using Robust.Shared.Localization;

namespace Content.Client.Info
{
	// Token: 0x020002BB RID: 699
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class LinkBanner : BoxContainer
	{
		// Token: 0x060011A0 RID: 4512 RVA: 0x00068CA4 File Offset: 0x00066EA4
		public LinkBanner()
		{
			LinkBanner.<>c__DisplayClass3_0 CS$<>8__locals1 = new LinkBanner.<>c__DisplayClass3_0();
			CS$<>8__locals1.<>4__this = this;
			IoCManager.InjectDependencies<LinkBanner>(this);
			CS$<>8__locals1.buttons = new BoxContainer
			{
				Orientation = 0
			};
			base.AddChild(CS$<>8__locals1.buttons);
			CS$<>8__locals1.uriOpener = IoCManager.Resolve<IUriOpener>();
			this._cfg = IoCManager.Resolve<IConfigurationManager>();
			Button button = new Button
			{
				Text = Loc.GetString("server-info-rules-button")
			};
			button.OnPressed += delegate(BaseButton.ButtonEventArgs args)
			{
				new RulesAndInfoWindow().Open();
			};
			CS$<>8__locals1.buttons.AddChild(button);
			CS$<>8__locals1.<.ctor>g__AddInfoButton|2("server-info-discord-button", CCVars.InfoLinksDiscord);
			CS$<>8__locals1.<.ctor>g__AddInfoButton|2("server-info-website-button", CCVars.InfoLinksWebsite);
			CS$<>8__locals1.<.ctor>g__AddInfoButton|2("server-info-wiki-button", CCVars.InfoLinksWiki);
			CS$<>8__locals1.<.ctor>g__AddInfoButton|2("server-info-forum-button", CCVars.InfoLinksForum);
			ChangelogButton changelogButton = new ChangelogButton();
			changelogButton.OnPressed += delegate(BaseButton.ButtonEventArgs args)
			{
				base.UserInterfaceManager.GetUIController<ChangelogUIController>().ToggleWindow();
			};
			CS$<>8__locals1.buttons.AddChild(changelogButton);
			Button button2 = new Button
			{
				Text = "Привязать дискорд"
			};
			button2.StyleClasses.Add("ButtonColorRed");
			button2.OnPressed += delegate(BaseButton.ButtonEventArgs _)
			{
				this._stalinManager.RequestUri();
			};
			CS$<>8__locals1.buttons.AddChild(button2);
		}

		// Token: 0x060011A1 RID: 4513 RVA: 0x00068DEC File Offset: 0x00066FEC
		protected override void EnteredTree()
		{
			base.EnteredTree();
			foreach (ValueTuple<CVarDef<string>, Button> valueTuple in this._infoLinks)
			{
				CVarDef<string> item = valueTuple.Item1;
				valueTuple.Item2.Visible = (this._cfg.GetCVar<string>(item) != "");
			}
		}

		// Token: 0x040008A6 RID: 2214
		[Dependency]
		private readonly StalinManager _stalinManager;

		// Token: 0x040008A7 RID: 2215
		private readonly IConfigurationManager _cfg;

		// Token: 0x040008A8 RID: 2216
		[TupleElementNames(new string[]
		{
			"cVar",
			"button"
		})]
		[Nullable(new byte[]
		{
			0,
			0,
			1,
			1,
			1
		})]
		private ValueList<ValueTuple<CVarDef<string>, Button>> _infoLinks;
	}
}
