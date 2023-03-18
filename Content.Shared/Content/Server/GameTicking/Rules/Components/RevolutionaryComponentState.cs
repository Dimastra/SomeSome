using System;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Server.GameTicking.Rules.Components
{
	// Token: 0x0200000C RID: 12
	[NetSerializable]
	[Serializable]
	public sealed class RevolutionaryComponentState : ComponentState
	{
		// Token: 0x0600000D RID: 13 RVA: 0x000020F2 File Offset: 0x000002F2
		public RevolutionaryComponentState(bool headrev)
		{
			this.HeadRevolutionary = headrev;
		}

		// Token: 0x0400000A RID: 10
		public bool HeadRevolutionary;
	}
}
