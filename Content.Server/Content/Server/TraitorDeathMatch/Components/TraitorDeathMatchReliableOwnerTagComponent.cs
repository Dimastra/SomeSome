using System;
using Robust.Shared.GameObjects;
using Robust.Shared.Network;
using Robust.Shared.ViewVariables;

namespace Content.Server.TraitorDeathMatch.Components
{
	// Token: 0x0200010B RID: 267
	[RegisterComponent]
	public sealed class TraitorDeathMatchReliableOwnerTagComponent : Component
	{
		// Token: 0x170000DD RID: 221
		// (get) Token: 0x060004C9 RID: 1225 RVA: 0x00016EDE File Offset: 0x000150DE
		// (set) Token: 0x060004CA RID: 1226 RVA: 0x00016EE6 File Offset: 0x000150E6
		[ViewVariables]
		public NetUserId? UserId { get; set; }
	}
}
