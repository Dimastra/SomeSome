using System;
using System.Runtime.CompilerServices;
using Content.Shared.CCVar;
using Content.Shared.Info;
using Robust.Shared.Configuration;
using Robust.Shared.ContentPack;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Localization;
using Robust.Shared.Log;

namespace Content.Server.Info
{
	// Token: 0x0200044E RID: 1102
	[NullableContext(1)]
	[Nullable(0)]
	public class InfoSystem : EntitySystem
	{
		// Token: 0x06001639 RID: 5689 RVA: 0x000755F8 File Offset: 0x000737F8
		public override void Initialize()
		{
			base.Initialize();
			base.SubscribeNetworkEvent<RequestRulesMessage>(new EntitySessionEventHandler<RequestRulesMessage>(this.OnRequestRules), null, null);
		}

		// Token: 0x0600163A RID: 5690 RVA: 0x00075614 File Offset: 0x00073814
		protected void OnRequestRules(RequestRulesMessage message, EntitySessionEventArgs eventArgs)
		{
			Logger.DebugS("info", "Client requested rules.");
			string title = Loc.GetString(this._cfg.GetCVar<string>(CCVars.RulesHeader));
			string path = this._cfg.GetCVar<string>(CCVars.RulesFile);
			string rules = "Server could not read its rules.";
			try
			{
				rules = this._res.ContentFileReadAllText("/Server Info/" + path);
			}
			catch (Exception)
			{
				Logger.ErrorS("info", "Could not read server rules file.");
			}
			RulesMessage response = new RulesMessage(title, rules);
			base.RaiseNetworkEvent(response, eventArgs.SenderSession.ConnectedClient);
		}

		// Token: 0x04000DED RID: 3565
		[Dependency]
		private readonly IResourceManager _res;

		// Token: 0x04000DEE RID: 3566
		[Dependency]
		private readonly IConfigurationManager _cfg;
	}
}
