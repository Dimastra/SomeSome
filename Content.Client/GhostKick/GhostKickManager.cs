using System;
using System.Runtime.CompilerServices;
using Content.Shared.GhostKick;
using Robust.Client;
using Robust.Shared;
using Robust.Shared.Configuration;
using Robust.Shared.IoC;
using Robust.Shared.Network;

namespace Content.Client.GhostKick
{
	// Token: 0x02000300 RID: 768
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class GhostKickManager
	{
		// Token: 0x0600133C RID: 4924 RVA: 0x00072879 File Offset: 0x00070A79
		public void Initialize()
		{
			this._netManager.RegisterNetMessage<MsgGhostKick>(new ProcessMessage<MsgGhostKick>(this.RxCallback), 3);
			this._baseClient.RunLevelChanged += this.BaseClientOnRunLevelChanged;
		}

		// Token: 0x0600133D RID: 4925 RVA: 0x000728AA File Offset: 0x00070AAA
		private void BaseClientOnRunLevelChanged([Nullable(2)] object sender, RunLevelChangedEventArgs e)
		{
			if (this._fakeLossEnabled && e.OldLevel == 4)
			{
				this._cfg.SetCVar<float>(CVars.NetFakeLoss, 0f, false);
				this._fakeLossEnabled = false;
			}
		}

		// Token: 0x0600133E RID: 4926 RVA: 0x000728DA File Offset: 0x00070ADA
		private void RxCallback(MsgGhostKick message)
		{
			this._fakeLossEnabled = true;
			this._cfg.SetCVar<float>(CVars.NetFakeLoss, 1f, false);
		}

		// Token: 0x0400099E RID: 2462
		private bool _fakeLossEnabled;

		// Token: 0x0400099F RID: 2463
		[Dependency]
		private readonly IBaseClient _baseClient;

		// Token: 0x040009A0 RID: 2464
		[Dependency]
		private readonly IClientNetManager _netManager;

		// Token: 0x040009A1 RID: 2465
		[Dependency]
		private readonly IConfigurationManager _cfg;
	}
}
