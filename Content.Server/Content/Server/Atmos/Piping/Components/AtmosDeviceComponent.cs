using System;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.ViewVariables;

namespace Content.Server.Atmos.Piping.Components
{
	// Token: 0x02000762 RID: 1890
	[RegisterComponent]
	public sealed class AtmosDeviceComponent : Component
	{
		// Token: 0x17000614 RID: 1556
		// (get) Token: 0x0600280B RID: 10251 RVA: 0x000D1E88 File Offset: 0x000D0088
		// (set) Token: 0x0600280C RID: 10252 RVA: 0x000D1E90 File Offset: 0x000D0090
		[ViewVariables]
		[DataField("requireAnchored", false, 1, false, false, null)]
		public bool RequireAnchored { get; private set; } = true;

		// Token: 0x17000615 RID: 1557
		// (get) Token: 0x0600280D RID: 10253 RVA: 0x000D1E99 File Offset: 0x000D0099
		[DataField("joinSystem", false, 1, false, false, null)]
		public bool JoinSystem { get; }

		// Token: 0x17000616 RID: 1558
		// (get) Token: 0x0600280E RID: 10254 RVA: 0x000D1EA1 File Offset: 0x000D00A1
		// (set) Token: 0x0600280F RID: 10255 RVA: 0x000D1EA9 File Offset: 0x000D00A9
		[ViewVariables]
		public bool JoinedSystem { get; set; }

		// Token: 0x17000617 RID: 1559
		// (get) Token: 0x06002810 RID: 10256 RVA: 0x000D1EB2 File Offset: 0x000D00B2
		// (set) Token: 0x06002811 RID: 10257 RVA: 0x000D1EBA File Offset: 0x000D00BA
		[ViewVariables]
		public TimeSpan LastProcess { get; set; } = TimeSpan.Zero;

		// Token: 0x17000618 RID: 1560
		// (get) Token: 0x06002812 RID: 10258 RVA: 0x000D1EC3 File Offset: 0x000D00C3
		// (set) Token: 0x06002813 RID: 10259 RVA: 0x000D1ECB File Offset: 0x000D00CB
		public EntityUid? JoinedGrid { get; set; }
	}
}
