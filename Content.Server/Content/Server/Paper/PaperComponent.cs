using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Content.Shared.Paper;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Server.Paper
{
	// Token: 0x020002EE RID: 750
	[NullableContext(1)]
	[Nullable(0)]
	[NetworkedComponent]
	[RegisterComponent]
	public sealed class PaperComponent : SharedPaperComponent
	{
		// Token: 0x17000217 RID: 535
		// (get) Token: 0x06000F64 RID: 3940 RVA: 0x0004F304 File Offset: 0x0004D504
		// (set) Token: 0x06000F65 RID: 3941 RVA: 0x0004F30C File Offset: 0x0004D50C
		[DataField("content", false, 1, false, false, null)]
		public string Content { get; set; } = "";

		// Token: 0x17000218 RID: 536
		// (get) Token: 0x06000F66 RID: 3942 RVA: 0x0004F315 File Offset: 0x0004D515
		// (set) Token: 0x06000F67 RID: 3943 RVA: 0x0004F31D File Offset: 0x0004D51D
		[DataField("contentSize", false, 1, false, false, null)]
		public int ContentSize { get; set; } = 1000;

		// Token: 0x17000219 RID: 537
		// (get) Token: 0x06000F68 RID: 3944 RVA: 0x0004F326 File Offset: 0x0004D526
		// (set) Token: 0x06000F69 RID: 3945 RVA: 0x0004F32E File Offset: 0x0004D52E
		[DataField("stampedBy", false, 1, false, false, null)]
		public List<string> StampedBy { get; set; } = new List<string>();

		// Token: 0x1700021A RID: 538
		// (get) Token: 0x06000F6A RID: 3946 RVA: 0x0004F337 File Offset: 0x0004D537
		// (set) Token: 0x06000F6B RID: 3947 RVA: 0x0004F33F File Offset: 0x0004D53F
		[Nullable(2)]
		[DataField("stampState", false, 1, false, false, null)]
		public string StampState { [NullableContext(2)] get; [NullableContext(2)] set; }

		// Token: 0x0400090A RID: 2314
		public SharedPaperComponent.PaperAction Mode;
	}
}
