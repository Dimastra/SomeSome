using System;
using System.Runtime.CompilerServices;
using System.Text.Json.Serialization;

namespace Content.Server.UtkaIntegration
{
	// Token: 0x020000F0 RID: 240
	[NullableContext(2)]
	[Nullable(0)]
	public sealed class UtkaChatMeEvent : UtkaBaseMessage
	{
		// Token: 0x170000C1 RID: 193
		// (get) Token: 0x06000461 RID: 1121 RVA: 0x0001587E File Offset: 0x00013A7E
		[JsonPropertyName("command")]
		public override string Command
		{
			get
			{
				return "me";
			}
		}

		// Token: 0x170000C2 RID: 194
		// (get) Token: 0x06000462 RID: 1122 RVA: 0x00015885 File Offset: 0x00013A85
		// (set) Token: 0x06000463 RID: 1123 RVA: 0x0001588D File Offset: 0x00013A8D
		[JsonPropertyName("ckey")]
		public string Ckey { get; set; }

		// Token: 0x170000C3 RID: 195
		// (get) Token: 0x06000464 RID: 1124 RVA: 0x00015896 File Offset: 0x00013A96
		// (set) Token: 0x06000465 RID: 1125 RVA: 0x0001589E File Offset: 0x00013A9E
		[JsonPropertyName("message")]
		public string Message { get; set; }

		// Token: 0x170000C4 RID: 196
		// (get) Token: 0x06000466 RID: 1126 RVA: 0x000158A7 File Offset: 0x00013AA7
		// (set) Token: 0x06000467 RID: 1127 RVA: 0x000158AF File Offset: 0x00013AAF
		[JsonPropertyName("character_name")]
		public string CharacterName { get; set; }
	}
}
