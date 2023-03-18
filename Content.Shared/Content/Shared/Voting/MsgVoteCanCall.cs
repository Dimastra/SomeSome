using System;
using System.Runtime.CompilerServices;
using Lidgren.Network;
using Robust.Shared.Network;
using Robust.Shared.Serialization;

namespace Content.Shared.Voting
{
	// Token: 0x0200007E RID: 126
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class MsgVoteCanCall : NetMessage
	{
		// Token: 0x17000036 RID: 54
		// (get) Token: 0x0600017C RID: 380 RVA: 0x00008AD5 File Offset: 0x00006CD5
		public override MsgGroups MsgGroup
		{
			get
			{
				return 4;
			}
		}

		// Token: 0x0600017D RID: 381 RVA: 0x00008AD8 File Offset: 0x00006CD8
		public override void ReadFromBuffer(NetIncomingMessage buffer, IRobustSerializer serializer)
		{
			this.CanCall = buffer.ReadBoolean();
			buffer.ReadPadBits();
			this.WhenCanCallVote = TimeSpan.FromTicks(buffer.ReadInt64());
			byte lenVotes = buffer.ReadByte();
			this.VotesUnavailable = new ValueTuple<StandardVoteType, TimeSpan>[(int)lenVotes];
			for (int i = 0; i < (int)lenVotes; i++)
			{
				StandardVoteType type = (StandardVoteType)buffer.ReadByte();
				TimeSpan timeOut = TimeSpan.FromTicks(buffer.ReadInt64());
				this.VotesUnavailable[i] = new ValueTuple<StandardVoteType, TimeSpan>(type, timeOut);
			}
		}

		// Token: 0x0600017E RID: 382 RVA: 0x00008B50 File Offset: 0x00006D50
		public override void WriteToBuffer(NetOutgoingMessage buffer, IRobustSerializer serializer)
		{
			buffer.Write(this.CanCall);
			buffer.WritePadBits();
			buffer.Write(this.WhenCanCallVote.Ticks);
			buffer.Write((byte)this.VotesUnavailable.Length);
			foreach (ValueTuple<StandardVoteType, TimeSpan> valueTuple in this.VotesUnavailable)
			{
				StandardVoteType type = valueTuple.Item1;
				TimeSpan timeout = valueTuple.Item2;
				buffer.Write((byte)type);
				buffer.Write(timeout.Ticks);
			}
		}

		// Token: 0x040001A3 RID: 419
		public bool CanCall;

		// Token: 0x040001A4 RID: 420
		public TimeSpan WhenCanCallVote;

		// Token: 0x040001A5 RID: 421
		[TupleElementNames(new string[]
		{
			"type",
			"whenAvailable"
		})]
		[Nullable(new byte[]
		{
			1,
			0
		})]
		public ValueTuple<StandardVoteType, TimeSpan>[] VotesUnavailable;
	}
}
