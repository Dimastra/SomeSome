using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Lidgren.Network;
using Robust.Shared.Network;
using Robust.Shared.Serialization;

namespace Content.Shared.Players.PlayTimeTracking
{
	// Token: 0x0200026D RID: 621
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class MsgPlayTime : NetMessage
	{
		// Token: 0x17000164 RID: 356
		// (get) Token: 0x06000719 RID: 1817 RVA: 0x000185E0 File Offset: 0x000167E0
		public override MsgGroups MsgGroup
		{
			get
			{
				return 5;
			}
		}

		// Token: 0x0600071A RID: 1818 RVA: 0x000185E4 File Offset: 0x000167E4
		public override void ReadFromBuffer(NetIncomingMessage buffer, IRobustSerializer serializer)
		{
			int count = buffer.ReadVariableInt32();
			for (int i = 0; i < count; i++)
			{
				this.Trackers.Add(buffer.ReadString(), NetMessageExt.ReadTimeSpan(buffer));
			}
		}

		// Token: 0x0600071B RID: 1819 RVA: 0x0001861C File Offset: 0x0001681C
		public override void WriteToBuffer(NetOutgoingMessage buffer, IRobustSerializer serializer)
		{
			buffer.WriteVariableInt32(this.Trackers.Count);
			foreach (KeyValuePair<string, TimeSpan> keyValuePair in this.Trackers)
			{
				string text;
				TimeSpan timeSpan;
				keyValuePair.Deconstruct(out text, out timeSpan);
				string role = text;
				TimeSpan time = timeSpan;
				buffer.Write(role);
				NetMessageExt.Write(buffer, time);
			}
		}

		// Token: 0x04000701 RID: 1793
		public Dictionary<string, TimeSpan> Trackers = new Dictionary<string, TimeSpan>();
	}
}
