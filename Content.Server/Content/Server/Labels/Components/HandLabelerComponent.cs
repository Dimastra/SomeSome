using System;
using System.Runtime.CompilerServices;
using Content.Shared.Whitelist;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.ViewVariables;

namespace Content.Server.Labels.Components
{
	// Token: 0x02000427 RID: 1063
	[NullableContext(1)]
	[Nullable(0)]
	[RegisterComponent]
	public sealed class HandLabelerComponent : Component
	{
		// Token: 0x170002E9 RID: 745
		// (get) Token: 0x06001587 RID: 5511 RVA: 0x0007112C File Offset: 0x0006F32C
		// (set) Token: 0x06001588 RID: 5512 RVA: 0x00071134 File Offset: 0x0006F334
		[ViewVariables]
		[DataField("assignedLabel", false, 1, false, false, null)]
		public string AssignedLabel { get; set; } = string.Empty;

		// Token: 0x170002EA RID: 746
		// (get) Token: 0x06001589 RID: 5513 RVA: 0x0007113D File Offset: 0x0006F33D
		// (set) Token: 0x0600158A RID: 5514 RVA: 0x00071145 File Offset: 0x0006F345
		[ViewVariables]
		[DataField("maxLabelChars", false, 1, false, false, null)]
		public int MaxLabelChars { get; set; } = 50;

		// Token: 0x04000D64 RID: 3428
		[DataField("whitelist", false, 1, false, false, null)]
		public EntityWhitelist Whitelist = new EntityWhitelist();
	}
}
