using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using Content.Shared.White.Sponsors;
using Robust.Shared.IoC;
using Robust.Shared.Network;

namespace Content.Client.White.Sponsors
{
	// Token: 0x02000021 RID: 33
	[NullableContext(2)]
	[Nullable(0)]
	public sealed class SponsorsManager
	{
		// Token: 0x06000076 RID: 118 RVA: 0x00004BFA File Offset: 0x00002DFA
		public void Initialize()
		{
			this._netMgr.RegisterNetMessage<MsgSponsorInfo>(delegate(MsgSponsorInfo msg)
			{
				this._info = msg.Info;
			}, 3);
		}

		// Token: 0x06000077 RID: 119 RVA: 0x00004C14 File Offset: 0x00002E14
		public bool TryGetInfo([NotNullWhen(true)] out SponsorInfo sponsor)
		{
			sponsor = this._info;
			return this._info != null;
		}

		// Token: 0x04000044 RID: 68
		[Nullable(1)]
		[Dependency]
		private readonly IClientNetManager _netMgr;

		// Token: 0x04000045 RID: 69
		private SponsorInfo _info;
	}
}
