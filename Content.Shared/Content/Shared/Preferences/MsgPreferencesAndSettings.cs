using System;
using System.IO;
using System.Runtime.CompilerServices;
using Lidgren.Network;
using Robust.Shared.Network;
using Robust.Shared.Serialization;

namespace Content.Shared.Preferences
{
	// Token: 0x0200024A RID: 586
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class MsgPreferencesAndSettings : NetMessage
	{
		// Token: 0x17000154 RID: 340
		// (get) Token: 0x060006D1 RID: 1745 RVA: 0x00017F42 File Offset: 0x00016142
		public override MsgGroups MsgGroup
		{
			get
			{
				return 4;
			}
		}

		// Token: 0x060006D2 RID: 1746 RVA: 0x00017F48 File Offset: 0x00016148
		public override void ReadFromBuffer(NetIncomingMessage buffer, IRobustSerializer serializer)
		{
			int length = buffer.ReadVariableInt32();
			using (MemoryStream stream = NetMessageExt.ReadAlignedMemory(buffer, length))
			{
				serializer.DeserializeDirect<PlayerPreferences>(stream, ref this.Preferences);
			}
			length = buffer.ReadVariableInt32();
			using (MemoryStream stream2 = NetMessageExt.ReadAlignedMemory(buffer, length))
			{
				serializer.DeserializeDirect<GameSettings>(stream2, ref this.Settings);
			}
		}

		// Token: 0x060006D3 RID: 1747 RVA: 0x00017FC4 File Offset: 0x000161C4
		public override void WriteToBuffer(NetOutgoingMessage buffer, IRobustSerializer serializer)
		{
			using (MemoryStream stream = new MemoryStream())
			{
				serializer.SerializeDirect<PlayerPreferences>(stream, this.Preferences);
				buffer.WriteVariableInt32((int)stream.Length);
				ArraySegment<byte> segment;
				stream.TryGetBuffer(out segment);
				buffer.Write(segment);
			}
			using (MemoryStream stream2 = new MemoryStream())
			{
				serializer.SerializeDirect<GameSettings>(stream2, this.Settings);
				buffer.WriteVariableInt32((int)stream2.Length);
				ArraySegment<byte> segment2;
				stream2.TryGetBuffer(out segment2);
				buffer.Write(segment2);
			}
		}

		// Token: 0x0400069B RID: 1691
		public PlayerPreferences Preferences;

		// Token: 0x0400069C RID: 1692
		public GameSettings Settings;
	}
}
