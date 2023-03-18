using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using Robust.Shared.GameObjects;
using Robust.Shared.Network;
using Robust.Shared.Serialization;

namespace Content.Shared.Administration
{
	// Token: 0x0200073E RID: 1854
	[NullableContext(1)]
	[Nullable(0)]
	[NetSerializable]
	[Serializable]
	public class PlayerInfo : IEquatable<PlayerInfo>
	{
		// Token: 0x06001671 RID: 5745 RVA: 0x000494E0 File Offset: 0x000476E0
		public PlayerInfo(string Username, string CharacterName, string IdentityName, string StartingJob, bool Antag, EntityUid? EntityUid, NetUserId SessionId, bool Connected, bool ActiveThisRound)
		{
			this.Username = Username;
			this.CharacterName = CharacterName;
			this.IdentityName = IdentityName;
			this.StartingJob = StartingJob;
			this.Antag = Antag;
			this.EntityUid = EntityUid;
			this.SessionId = SessionId;
			this.Connected = Connected;
			this.ActiveThisRound = ActiveThisRound;
			base..ctor();
		}

		// Token: 0x170004AE RID: 1198
		// (get) Token: 0x06001672 RID: 5746 RVA: 0x00049538 File Offset: 0x00047738
		[CompilerGenerated]
		protected virtual Type EqualityContract
		{
			[CompilerGenerated]
			get
			{
				return typeof(PlayerInfo);
			}
		}

		// Token: 0x170004AF RID: 1199
		// (get) Token: 0x06001673 RID: 5747 RVA: 0x00049544 File Offset: 0x00047744
		// (set) Token: 0x06001674 RID: 5748 RVA: 0x0004954C File Offset: 0x0004774C
		public string Username { get; set; }

		// Token: 0x170004B0 RID: 1200
		// (get) Token: 0x06001675 RID: 5749 RVA: 0x00049555 File Offset: 0x00047755
		// (set) Token: 0x06001676 RID: 5750 RVA: 0x0004955D File Offset: 0x0004775D
		public string CharacterName { get; set; }

		// Token: 0x170004B1 RID: 1201
		// (get) Token: 0x06001677 RID: 5751 RVA: 0x00049566 File Offset: 0x00047766
		// (set) Token: 0x06001678 RID: 5752 RVA: 0x0004956E File Offset: 0x0004776E
		public string IdentityName { get; set; }

		// Token: 0x170004B2 RID: 1202
		// (get) Token: 0x06001679 RID: 5753 RVA: 0x00049577 File Offset: 0x00047777
		// (set) Token: 0x0600167A RID: 5754 RVA: 0x0004957F File Offset: 0x0004777F
		public string StartingJob { get; set; }

		// Token: 0x170004B3 RID: 1203
		// (get) Token: 0x0600167B RID: 5755 RVA: 0x00049588 File Offset: 0x00047788
		// (set) Token: 0x0600167C RID: 5756 RVA: 0x00049590 File Offset: 0x00047790
		public bool Antag { get; set; }

		// Token: 0x170004B4 RID: 1204
		// (get) Token: 0x0600167D RID: 5757 RVA: 0x00049599 File Offset: 0x00047799
		// (set) Token: 0x0600167E RID: 5758 RVA: 0x000495A1 File Offset: 0x000477A1
		public EntityUid? EntityUid { get; set; }

		// Token: 0x170004B5 RID: 1205
		// (get) Token: 0x0600167F RID: 5759 RVA: 0x000495AA File Offset: 0x000477AA
		// (set) Token: 0x06001680 RID: 5760 RVA: 0x000495B2 File Offset: 0x000477B2
		public NetUserId SessionId { get; set; }

		// Token: 0x170004B6 RID: 1206
		// (get) Token: 0x06001681 RID: 5761 RVA: 0x000495BB File Offset: 0x000477BB
		// (set) Token: 0x06001682 RID: 5762 RVA: 0x000495C3 File Offset: 0x000477C3
		public bool Connected { get; set; }

		// Token: 0x170004B7 RID: 1207
		// (get) Token: 0x06001683 RID: 5763 RVA: 0x000495CC File Offset: 0x000477CC
		// (set) Token: 0x06001684 RID: 5764 RVA: 0x000495D4 File Offset: 0x000477D4
		public bool ActiveThisRound { get; set; }

		// Token: 0x06001685 RID: 5765 RVA: 0x000495E0 File Offset: 0x000477E0
		[CompilerGenerated]
		public override string ToString()
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append("PlayerInfo");
			stringBuilder.Append(" { ");
			if (this.PrintMembers(stringBuilder))
			{
				stringBuilder.Append(' ');
			}
			stringBuilder.Append('}');
			return stringBuilder.ToString();
		}

		// Token: 0x06001686 RID: 5766 RVA: 0x0004962C File Offset: 0x0004782C
		[CompilerGenerated]
		protected virtual bool PrintMembers(StringBuilder builder)
		{
			RuntimeHelpers.EnsureSufficientExecutionStack();
			builder.Append("Username = ");
			builder.Append(this.Username);
			builder.Append(", CharacterName = ");
			builder.Append(this.CharacterName);
			builder.Append(", IdentityName = ");
			builder.Append(this.IdentityName);
			builder.Append(", StartingJob = ");
			builder.Append(this.StartingJob);
			builder.Append(", Antag = ");
			builder.Append(this.Antag.ToString());
			builder.Append(", EntityUid = ");
			builder.Append(this.EntityUid.ToString());
			builder.Append(", SessionId = ");
			builder.Append(this.SessionId.ToString());
			builder.Append(", Connected = ");
			builder.Append(this.Connected.ToString());
			builder.Append(", ActiveThisRound = ");
			builder.Append(this.ActiveThisRound.ToString());
			return true;
		}

		// Token: 0x06001687 RID: 5767 RVA: 0x00049766 File Offset: 0x00047966
		[NullableContext(2)]
		[CompilerGenerated]
		public static bool operator !=(PlayerInfo left, PlayerInfo right)
		{
			return !(left == right);
		}

		// Token: 0x06001688 RID: 5768 RVA: 0x00049772 File Offset: 0x00047972
		[NullableContext(2)]
		[CompilerGenerated]
		public static bool operator ==(PlayerInfo left, PlayerInfo right)
		{
			return left == right || (left != null && left.Equals(right));
		}

		// Token: 0x06001689 RID: 5769 RVA: 0x00049788 File Offset: 0x00047988
		[CompilerGenerated]
		public override int GetHashCode()
		{
			return ((((((((EqualityComparer<Type>.Default.GetHashCode(this.EqualityContract) * -1521134295 + EqualityComparer<string>.Default.GetHashCode(this.<Username>k__BackingField)) * -1521134295 + EqualityComparer<string>.Default.GetHashCode(this.<CharacterName>k__BackingField)) * -1521134295 + EqualityComparer<string>.Default.GetHashCode(this.<IdentityName>k__BackingField)) * -1521134295 + EqualityComparer<string>.Default.GetHashCode(this.<StartingJob>k__BackingField)) * -1521134295 + EqualityComparer<bool>.Default.GetHashCode(this.<Antag>k__BackingField)) * -1521134295 + EqualityComparer<Robust.Shared.GameObjects.EntityUid?>.Default.GetHashCode(this.<EntityUid>k__BackingField)) * -1521134295 + EqualityComparer<NetUserId>.Default.GetHashCode(this.<SessionId>k__BackingField)) * -1521134295 + EqualityComparer<bool>.Default.GetHashCode(this.<Connected>k__BackingField)) * -1521134295 + EqualityComparer<bool>.Default.GetHashCode(this.<ActiveThisRound>k__BackingField);
		}

		// Token: 0x0600168A RID: 5770 RVA: 0x00049874 File Offset: 0x00047A74
		[NullableContext(2)]
		[CompilerGenerated]
		public override bool Equals(object obj)
		{
			return this.Equals(obj as PlayerInfo);
		}

		// Token: 0x0600168B RID: 5771 RVA: 0x00049884 File Offset: 0x00047A84
		[NullableContext(2)]
		[CompilerGenerated]
		public virtual bool Equals(PlayerInfo other)
		{
			return this == other || (other != null && this.EqualityContract == other.EqualityContract && EqualityComparer<string>.Default.Equals(this.<Username>k__BackingField, other.<Username>k__BackingField) && EqualityComparer<string>.Default.Equals(this.<CharacterName>k__BackingField, other.<CharacterName>k__BackingField) && EqualityComparer<string>.Default.Equals(this.<IdentityName>k__BackingField, other.<IdentityName>k__BackingField) && EqualityComparer<string>.Default.Equals(this.<StartingJob>k__BackingField, other.<StartingJob>k__BackingField) && EqualityComparer<bool>.Default.Equals(this.<Antag>k__BackingField, other.<Antag>k__BackingField) && EqualityComparer<Robust.Shared.GameObjects.EntityUid?>.Default.Equals(this.<EntityUid>k__BackingField, other.<EntityUid>k__BackingField) && EqualityComparer<NetUserId>.Default.Equals(this.<SessionId>k__BackingField, other.<SessionId>k__BackingField) && EqualityComparer<bool>.Default.Equals(this.<Connected>k__BackingField, other.<Connected>k__BackingField) && EqualityComparer<bool>.Default.Equals(this.<ActiveThisRound>k__BackingField, other.<ActiveThisRound>k__BackingField));
		}

		// Token: 0x0600168D RID: 5773 RVA: 0x000499A0 File Offset: 0x00047BA0
		[CompilerGenerated]
		protected PlayerInfo(PlayerInfo original)
		{
			this.Username = original.<Username>k__BackingField;
			this.CharacterName = original.<CharacterName>k__BackingField;
			this.IdentityName = original.<IdentityName>k__BackingField;
			this.StartingJob = original.<StartingJob>k__BackingField;
			this.Antag = original.<Antag>k__BackingField;
			this.EntityUid = original.<EntityUid>k__BackingField;
			this.SessionId = original.<SessionId>k__BackingField;
			this.Connected = original.<Connected>k__BackingField;
			this.ActiveThisRound = original.<ActiveThisRound>k__BackingField;
		}

		// Token: 0x0600168E RID: 5774 RVA: 0x00049A20 File Offset: 0x00047C20
		[CompilerGenerated]
		public void Deconstruct(out string Username, out string CharacterName, out string IdentityName, out string StartingJob, out bool Antag, out EntityUid? EntityUid, out NetUserId SessionId, out bool Connected, out bool ActiveThisRound)
		{
			Username = this.Username;
			CharacterName = this.CharacterName;
			IdentityName = this.IdentityName;
			StartingJob = this.StartingJob;
			Antag = this.Antag;
			EntityUid = this.EntityUid;
			SessionId = this.SessionId;
			Connected = this.Connected;
			ActiveThisRound = this.ActiveThisRound;
		}
	}
}
