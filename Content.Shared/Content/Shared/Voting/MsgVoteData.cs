using System;
using System.Runtime.CompilerServices;
using Lidgren.Network;
using Robust.Shared.Network;
using Robust.Shared.Serialization;

namespace Content.Shared.Voting
{
	// Token: 0x0200007F RID: 127
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class MsgVoteData : NetMessage
	{
		// Token: 0x17000037 RID: 55
		// (get) Token: 0x06000180 RID: 384 RVA: 0x00008BD4 File Offset: 0x00006DD4
		public override MsgGroups MsgGroup
		{
			get
			{
				return 4;
			}
		}

		// Token: 0x06000181 RID: 385 RVA: 0x00008BD8 File Offset: 0x00006DD8
		public override void ReadFromBuffer(NetIncomingMessage buffer, IRobustSerializer serializer)
		{
			this.VoteId = buffer.ReadVariableInt32();
			this.VoteActive = buffer.ReadBoolean();
			buffer.ReadPadBits();
			if (!this.VoteActive)
			{
				return;
			}
			this.VoteTitle = buffer.ReadString();
			this.VoteInitiator = buffer.ReadString();
			this.StartTime = TimeSpan.FromTicks(buffer.ReadInt64());
			this.EndTime = TimeSpan.FromTicks(buffer.ReadInt64());
			this.Options = new ValueTuple<ushort, string>[(int)buffer.ReadByte()];
			for (int i = 0; i < this.Options.Length; i++)
			{
				this.Options[i] = new ValueTuple<ushort, string>(buffer.ReadUInt16(), buffer.ReadString());
			}
			this.IsYourVoteDirty = buffer.ReadBoolean();
			if (this.IsYourVoteDirty)
			{
				this.YourVote = (buffer.ReadBoolean() ? new byte?(buffer.ReadByte()) : null);
			}
		}

		// Token: 0x06000182 RID: 386 RVA: 0x00008CC0 File Offset: 0x00006EC0
		public override void WriteToBuffer(NetOutgoingMessage buffer, IRobustSerializer serializer)
		{
			buffer.WriteVariableInt32(this.VoteId);
			buffer.Write(this.VoteActive);
			buffer.WritePadBits();
			if (!this.VoteActive)
			{
				return;
			}
			buffer.Write(this.VoteTitle);
			buffer.Write(this.VoteInitiator);
			buffer.Write(this.StartTime.Ticks);
			buffer.Write(this.EndTime.Ticks);
			buffer.Write((byte)this.Options.Length);
			foreach (ValueTuple<ushort, string> valueTuple in this.Options)
			{
				ushort votes = valueTuple.Item1;
				string name = valueTuple.Item2;
				buffer.Write(votes);
				buffer.Write(name);
			}
			buffer.Write(this.IsYourVoteDirty);
			if (this.IsYourVoteDirty)
			{
				buffer.Write(this.YourVote != null);
				if (this.YourVote != null)
				{
					buffer.Write(this.YourVote.Value);
				}
			}
		}

		// Token: 0x17000038 RID: 56
		// (get) Token: 0x06000183 RID: 387 RVA: 0x00008DB8 File Offset: 0x00006FB8
		public override NetDeliveryMethod DeliveryMethod
		{
			get
			{
				return 67;
			}
		}

		// Token: 0x040001A6 RID: 422
		public int VoteId;

		// Token: 0x040001A7 RID: 423
		public bool VoteActive;

		// Token: 0x040001A8 RID: 424
		public string VoteTitle = string.Empty;

		// Token: 0x040001A9 RID: 425
		public string VoteInitiator = string.Empty;

		// Token: 0x040001AA RID: 426
		public TimeSpan StartTime;

		// Token: 0x040001AB RID: 427
		public TimeSpan EndTime;

		// Token: 0x040001AC RID: 428
		[TupleElementNames(new string[]
		{
			"votes",
			"name"
		})]
		[Nullable(new byte[]
		{
			1,
			0,
			1
		})]
		public ValueTuple<ushort, string>[] Options;

		// Token: 0x040001AD RID: 429
		public bool IsYourVoteDirty;

		// Token: 0x040001AE RID: 430
		public byte? YourVote;
	}
}
