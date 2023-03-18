using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using Content.Client.UserInterface.Controls;
using Content.Shared.Administration;

namespace Content.Client.Administration.UI.CustomControls
{
	// Token: 0x020004CC RID: 1228
	[NullableContext(1)]
	[Nullable(0)]
	public class PlayerListData : ListData, IEquatable<PlayerListData>
	{
		// Token: 0x06001F3C RID: 7996 RVA: 0x000B6D53 File Offset: 0x000B4F53
		public PlayerListData(PlayerInfo Info)
		{
			this.Info = Info;
			base..ctor();
		}

		// Token: 0x170006CF RID: 1743
		// (get) Token: 0x06001F3D RID: 7997 RVA: 0x000B6D62 File Offset: 0x000B4F62
		[CompilerGenerated]
		protected override Type EqualityContract
		{
			[CompilerGenerated]
			get
			{
				return typeof(PlayerListData);
			}
		}

		// Token: 0x170006D0 RID: 1744
		// (get) Token: 0x06001F3E RID: 7998 RVA: 0x000B6D6E File Offset: 0x000B4F6E
		// (set) Token: 0x06001F3F RID: 7999 RVA: 0x000B6D76 File Offset: 0x000B4F76
		public PlayerInfo Info { get; set; }

		// Token: 0x06001F40 RID: 8000 RVA: 0x000B6D80 File Offset: 0x000B4F80
		[CompilerGenerated]
		public override string ToString()
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append("PlayerListData");
			stringBuilder.Append(" { ");
			if (this.PrintMembers(stringBuilder))
			{
				stringBuilder.Append(' ');
			}
			stringBuilder.Append('}');
			return stringBuilder.ToString();
		}

		// Token: 0x06001F41 RID: 8001 RVA: 0x000B6DCC File Offset: 0x000B4FCC
		[CompilerGenerated]
		protected override bool PrintMembers(StringBuilder builder)
		{
			if (base.PrintMembers(builder))
			{
				builder.Append(", ");
			}
			builder.Append("Info = ");
			builder.Append(this.Info);
			return true;
		}

		// Token: 0x06001F42 RID: 8002 RVA: 0x000B6DFD File Offset: 0x000B4FFD
		[NullableContext(2)]
		[CompilerGenerated]
		public static bool operator !=(PlayerListData left, PlayerListData right)
		{
			return !(left == right);
		}

		// Token: 0x06001F43 RID: 8003 RVA: 0x000B6E09 File Offset: 0x000B5009
		[NullableContext(2)]
		[CompilerGenerated]
		public static bool operator ==(PlayerListData left, PlayerListData right)
		{
			return left == right || (left != null && left.Equals(right));
		}

		// Token: 0x06001F44 RID: 8004 RVA: 0x000B6E1D File Offset: 0x000B501D
		[CompilerGenerated]
		public override int GetHashCode()
		{
			return base.GetHashCode() * -1521134295 + EqualityComparer<PlayerInfo>.Default.GetHashCode(this.<Info>k__BackingField);
		}

		// Token: 0x06001F45 RID: 8005 RVA: 0x000B6E3C File Offset: 0x000B503C
		[NullableContext(2)]
		[CompilerGenerated]
		public override bool Equals(object obj)
		{
			return this.Equals(obj as PlayerListData);
		}

		// Token: 0x06001F46 RID: 8006 RVA: 0x0002163F File Offset: 0x0001F83F
		[NullableContext(2)]
		[CompilerGenerated]
		public sealed override bool Equals(ListData other)
		{
			return this.Equals(other);
		}

		// Token: 0x06001F47 RID: 8007 RVA: 0x000B6E4A File Offset: 0x000B504A
		[NullableContext(2)]
		[CompilerGenerated]
		public virtual bool Equals(PlayerListData other)
		{
			return this == other || (base.Equals(other) && EqualityComparer<PlayerInfo>.Default.Equals(this.<Info>k__BackingField, other.<Info>k__BackingField));
		}

		// Token: 0x06001F49 RID: 8009 RVA: 0x000B6E7B File Offset: 0x000B507B
		[CompilerGenerated]
		protected PlayerListData(PlayerListData original) : base(original)
		{
			this.Info = original.<Info>k__BackingField;
		}

		// Token: 0x06001F4A RID: 8010 RVA: 0x000B6E90 File Offset: 0x000B5090
		[CompilerGenerated]
		public void Deconstruct(out PlayerInfo Info)
		{
			Info = this.Info;
		}
	}
}
