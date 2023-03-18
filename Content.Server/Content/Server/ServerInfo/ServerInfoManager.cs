using System;
using System.Runtime.CompilerServices;
using System.Text.Json.Nodes;
using Content.Shared.CCVar;
using Robust.Server.ServerStatus;
using Robust.Shared.Configuration;
using Robust.Shared.IoC;
using Robust.Shared.Localization;

namespace Content.Server.ServerInfo
{
	// Token: 0x02000213 RID: 531
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class ServerInfoManager
	{
		// Token: 0x06000A89 RID: 2697 RVA: 0x00037507 File Offset: 0x00035707
		public void Initialize()
		{
			this._statusHost.OnInfoRequest += this.OnInfoRequest;
		}

		// Token: 0x06000A8A RID: 2698 RVA: 0x00037520 File Offset: 0x00035720
		private void OnInfoRequest(JsonNode json)
		{
			foreach (ValueTuple<CVarDef<string>, string, string> valueTuple in ServerInfoManager.Vars)
			{
				CVarDef<string> cVar = valueTuple.Item1;
				string icon = valueTuple.Item2;
				string name = valueTuple.Item3;
				string url = this._cfg.GetCVar<string>(cVar);
				if (!string.IsNullOrEmpty(url))
				{
					StatusHostHelpers.AddLink(json, this._loc.GetString(name), url, icon);
				}
			}
		}

		// Token: 0x04000671 RID: 1649
		[TupleElementNames(new string[]
		{
			"cVar",
			"icon",
			"name"
		})]
		[Nullable(new byte[]
		{
			1,
			0,
			1,
			1,
			1,
			1
		})]
		private static readonly ValueTuple<CVarDef<string>, string, string>[] Vars = new ValueTuple<CVarDef<string>, string, string>[]
		{
			new ValueTuple<CVarDef<string>, string, string>(CCVars.InfoLinksDiscord, "discord", "info-link-discord"),
			new ValueTuple<CVarDef<string>, string, string>(CCVars.InfoLinksForum, "forum", "info-link-forum"),
			new ValueTuple<CVarDef<string>, string, string>(CCVars.InfoLinksGithub, "github", "info-link-github"),
			new ValueTuple<CVarDef<string>, string, string>(CCVars.InfoLinksWebsite, "web", "info-link-website"),
			new ValueTuple<CVarDef<string>, string, string>(CCVars.InfoLinksWiki, "wiki", "info-link-wiki")
		};

		// Token: 0x04000672 RID: 1650
		[Dependency]
		private readonly IStatusHost _statusHost;

		// Token: 0x04000673 RID: 1651
		[Dependency]
		private readonly IConfigurationManager _cfg;

		// Token: 0x04000674 RID: 1652
		[Dependency]
		private readonly ILocalizationManager _loc;
	}
}
