using System;
using System.Runtime.CompilerServices;
using Content.Shared.Info;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Log;

namespace Content.Client.Info
{
	// Token: 0x020002BA RID: 698
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class InfoSystem : EntitySystem
	{
		// Token: 0x0600119D RID: 4509 RVA: 0x00068C2E File Offset: 0x00066E2E
		public override void Initialize()
		{
			base.Initialize();
			base.SubscribeNetworkEvent<RulesMessage>(new EntitySessionEventHandler<RulesMessage>(this.OnRulesReceived), null, null);
			Logger.DebugS("info", "Requested server info.");
			base.RaiseNetworkEvent(new RequestRulesMessage());
		}

		// Token: 0x0600119E RID: 4510 RVA: 0x00068C64 File Offset: 0x00066E64
		private void OnRulesReceived(RulesMessage message, EntitySessionEventArgs eventArgs)
		{
			Logger.DebugS("info", "Received server rules.");
			this.Rules = message;
			this._rules.UpdateRules();
		}

		// Token: 0x040008A4 RID: 2212
		public RulesMessage Rules = new RulesMessage("Server Rules", "The server did not send any rules.");

		// Token: 0x040008A5 RID: 2213
		[Dependency]
		private readonly RulesManager _rules;
	}
}
