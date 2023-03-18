using System;
using System.Runtime.CompilerServices;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom.Prototype;
using Robust.Shared.ViewVariables;

namespace Content.Shared.Stacks
{
	// Token: 0x0200016C RID: 364
	[NullableContext(2)]
	[Nullable(0)]
	[RegisterComponent]
	[NetworkedComponent]
	public sealed class StackComponent : Component
	{
		// Token: 0x170000CA RID: 202
		// (get) Token: 0x06000467 RID: 1127 RVA: 0x00011CAC File Offset: 0x0000FEAC
		// (set) Token: 0x06000468 RID: 1128 RVA: 0x00011CB4 File Offset: 0x0000FEB4
		[ViewVariables]
		[DataField("stackType", false, 1, true, false, typeof(PrototypeIdSerializer<StackPrototype>))]
		public string StackTypeId { get; private set; }

		// Token: 0x170000CB RID: 203
		// (get) Token: 0x06000469 RID: 1129 RVA: 0x00011CBD File Offset: 0x0000FEBD
		// (set) Token: 0x0600046A RID: 1130 RVA: 0x00011CC5 File Offset: 0x0000FEC5
		[DataField("count", false, 1, false, false, null)]
		public int Count { get; set; } = 30;

		// Token: 0x170000CC RID: 204
		// (get) Token: 0x0600046B RID: 1131 RVA: 0x00011CCE File Offset: 0x0000FECE
		// (set) Token: 0x0600046C RID: 1132 RVA: 0x00011CD6 File Offset: 0x0000FED6
		[ViewVariables]
		[DataField("maxCountOverride", false, 1, false, false, null)]
		public int? MaxCountOverride { get; set; }

		// Token: 0x170000CD RID: 205
		// (get) Token: 0x0600046D RID: 1133 RVA: 0x00011CDF File Offset: 0x0000FEDF
		// (set) Token: 0x0600046E RID: 1134 RVA: 0x00011CE7 File Offset: 0x0000FEE7
		[DataField("unlimited", false, 1, false, false, null)]
		[ViewVariables]
		public bool Unlimited { get; set; }

		// Token: 0x170000CE RID: 206
		// (get) Token: 0x0600046F RID: 1135 RVA: 0x00011CF0 File Offset: 0x0000FEF0
		// (set) Token: 0x06000470 RID: 1136 RVA: 0x00011CF8 File Offset: 0x0000FEF8
		[ViewVariables]
		public bool ThrowIndividually { get; set; }

		// Token: 0x170000CF RID: 207
		// (get) Token: 0x06000471 RID: 1137 RVA: 0x00011D01 File Offset: 0x0000FF01
		// (set) Token: 0x06000472 RID: 1138 RVA: 0x00011D09 File Offset: 0x0000FF09
		[ViewVariables]
		public bool UiUpdateNeeded { get; set; }
	}
}
