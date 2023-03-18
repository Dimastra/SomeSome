using System;
using System.Runtime.CompilerServices;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.ViewVariables;

namespace Content.Shared.Buckle.Components
{
	// Token: 0x02000641 RID: 1601
	[NullableContext(2)]
	[Nullable(0)]
	[RegisterComponent]
	[NetworkedComponent]
	public sealed class BuckleComponent : Component
	{
		// Token: 0x170003E0 RID: 992
		// (get) Token: 0x06001357 RID: 4951 RVA: 0x000407E5 File Offset: 0x0003E9E5
		// (set) Token: 0x06001358 RID: 4952 RVA: 0x000407ED File Offset: 0x0003E9ED
		public bool Buckled { get; set; }

		// Token: 0x170003E1 RID: 993
		// (get) Token: 0x06001359 RID: 4953 RVA: 0x000407F6 File Offset: 0x0003E9F6
		// (set) Token: 0x0600135A RID: 4954 RVA: 0x000407FE File Offset: 0x0003E9FE
		public EntityUid? LastEntityBuckledTo { get; set; }

		// Token: 0x170003E2 RID: 994
		// (get) Token: 0x0600135B RID: 4955 RVA: 0x00040807 File Offset: 0x0003EA07
		// (set) Token: 0x0600135C RID: 4956 RVA: 0x0004080F File Offset: 0x0003EA0F
		public bool DontCollide { get; set; }

		// Token: 0x170003E3 RID: 995
		// (get) Token: 0x0600135D RID: 4957 RVA: 0x00040818 File Offset: 0x0003EA18
		// (set) Token: 0x0600135E RID: 4958 RVA: 0x00040820 File Offset: 0x0003EA20
		[ViewVariables]
		public StrapComponent BuckledTo { get; set; }

		// Token: 0x170003E4 RID: 996
		// (get) Token: 0x0600135F RID: 4959 RVA: 0x00040829 File Offset: 0x0003EA29
		// (set) Token: 0x06001360 RID: 4960 RVA: 0x00040831 File Offset: 0x0003EA31
		public int? OriginalDrawDepth { get; set; }

		// Token: 0x04001345 RID: 4933
		[DataField("range", false, 1, false, false, null)]
		public float Range = 1.0714285f;

		// Token: 0x04001349 RID: 4937
		[DataField("delay", false, 1, false, false, null)]
		public TimeSpan UnbuckleDelay = TimeSpan.FromSeconds(0.25);

		// Token: 0x0400134A RID: 4938
		[ViewVariables]
		public TimeSpan BuckleTime;

		// Token: 0x0400134C RID: 4940
		[DataField("size", false, 1, false, false, null)]
		public int Size = 100;
	}
}
