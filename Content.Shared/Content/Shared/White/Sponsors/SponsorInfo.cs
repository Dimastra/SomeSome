using System;
using System.Runtime.CompilerServices;
using System.Text.Json.Serialization;
using Robust.Shared.Serialization;

namespace Content.Shared.White.Sponsors
{
	// Token: 0x02000031 RID: 49
	[NullableContext(1)]
	[Nullable(0)]
	[NetSerializable]
	[Serializable]
	public sealed class SponsorInfo
	{
		// Token: 0x17000014 RID: 20
		// (get) Token: 0x06000049 RID: 73 RVA: 0x00002BF2 File Offset: 0x00000DF2
		// (set) Token: 0x0600004A RID: 74 RVA: 0x00002BFA File Offset: 0x00000DFA
		[JsonPropertyName("tier")]
		public int? Tier { get; set; }

		// Token: 0x17000015 RID: 21
		// (get) Token: 0x0600004B RID: 75 RVA: 0x00002C03 File Offset: 0x00000E03
		// (set) Token: 0x0600004C RID: 76 RVA: 0x00002C0B File Offset: 0x00000E0B
		[Nullable(2)]
		[JsonPropertyName("oocColor")]
		public string OOCColor { [NullableContext(2)] get; [NullableContext(2)] set; }

		// Token: 0x17000016 RID: 22
		// (get) Token: 0x0600004D RID: 77 RVA: 0x00002C14 File Offset: 0x00000E14
		// (set) Token: 0x0600004E RID: 78 RVA: 0x00002C1C File Offset: 0x00000E1C
		[JsonPropertyName("priorityJoin")]
		public bool HavePriorityJoin { get; set; }

		// Token: 0x17000017 RID: 23
		// (get) Token: 0x0600004F RID: 79 RVA: 0x00002C25 File Offset: 0x00000E25
		// (set) Token: 0x06000050 RID: 80 RVA: 0x00002C2D File Offset: 0x00000E2D
		[JsonPropertyName("allowedMarkings")]
		public string[] AllowedMarkings { get; set; } = Array.Empty<string>();

		// Token: 0x17000018 RID: 24
		// (get) Token: 0x06000051 RID: 81 RVA: 0x00002C36 File Offset: 0x00000E36
		// (set) Token: 0x06000052 RID: 82 RVA: 0x00002C3E File Offset: 0x00000E3E
		[JsonPropertyName("extraSlots")]
		public int ExtraSlots { get; set; }

		// Token: 0x17000019 RID: 25
		// (get) Token: 0x06000053 RID: 83 RVA: 0x00002C47 File Offset: 0x00000E47
		// (set) Token: 0x06000054 RID: 84 RVA: 0x00002C4F File Offset: 0x00000E4F
		[JsonPropertyName("MeatyOreCoin")]
		public int MeatyOreCoin { get; set; }
	}
}
