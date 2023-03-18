using System;
using System.Runtime.CompilerServices;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.ViewVariables;

namespace Content.Server.Labels.Components
{
	// Token: 0x02000428 RID: 1064
	[NullableContext(2)]
	[Nullable(0)]
	[RegisterComponent]
	public sealed class LabelComponent : Component
	{
		// Token: 0x170002EB RID: 747
		// (get) Token: 0x0600158C RID: 5516 RVA: 0x00071174 File Offset: 0x0006F374
		// (set) Token: 0x0600158D RID: 5517 RVA: 0x0007117C File Offset: 0x0006F37C
		[ViewVariables]
		[DataField("currentLabel", false, 1, false, false, null)]
		public string CurrentLabel { get; set; }

		// Token: 0x170002EC RID: 748
		// (get) Token: 0x0600158E RID: 5518 RVA: 0x00071185 File Offset: 0x0006F385
		// (set) Token: 0x0600158F RID: 5519 RVA: 0x0007118D File Offset: 0x0006F38D
		public string OriginalName { get; set; }
	}
}
