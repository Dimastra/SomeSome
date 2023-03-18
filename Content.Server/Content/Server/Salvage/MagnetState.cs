using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Server.Salvage
{
	// Token: 0x0200021A RID: 538
	[CopyByRef]
	public struct MagnetState : IEquatable<MagnetState>
	{
		// Token: 0x06000AAA RID: 2730 RVA: 0x00037FD9 File Offset: 0x000361D9
		public MagnetState(MagnetStateType StateType, TimeSpan Until)
		{
			this.StateType = StateType;
			this.Until = Until;
		}

		// Token: 0x1700019C RID: 412
		// (get) Token: 0x06000AAB RID: 2731 RVA: 0x00037FE9 File Offset: 0x000361E9
		// (set) Token: 0x06000AAC RID: 2732 RVA: 0x00037FF1 File Offset: 0x000361F1
		public MagnetStateType StateType { readonly get; set; }

		// Token: 0x1700019D RID: 413
		// (get) Token: 0x06000AAD RID: 2733 RVA: 0x00037FFA File Offset: 0x000361FA
		// (set) Token: 0x06000AAE RID: 2734 RVA: 0x00038002 File Offset: 0x00036202
		public TimeSpan Until { readonly get; set; }

		// Token: 0x06000AAF RID: 2735 RVA: 0x0003800C File Offset: 0x0003620C
		[CompilerGenerated]
		public override readonly string ToString()
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append("MagnetState");
			stringBuilder.Append(" { ");
			if (this.PrintMembers(stringBuilder))
			{
				stringBuilder.Append(' ');
			}
			stringBuilder.Append('}');
			return stringBuilder.ToString();
		}

		// Token: 0x06000AB0 RID: 2736 RVA: 0x00038058 File Offset: 0x00036258
		[CompilerGenerated]
		private readonly bool PrintMembers(StringBuilder builder)
		{
			builder.Append("StateType = ");
			builder.Append(this.StateType.ToString());
			builder.Append(", Until = ");
			builder.Append(this.Until.ToString());
			return true;
		}

		// Token: 0x06000AB1 RID: 2737 RVA: 0x000380B4 File Offset: 0x000362B4
		[CompilerGenerated]
		public static bool operator !=(MagnetState left, MagnetState right)
		{
			return !(left == right);
		}

		// Token: 0x06000AB2 RID: 2738 RVA: 0x000380C0 File Offset: 0x000362C0
		[CompilerGenerated]
		public static bool operator ==(MagnetState left, MagnetState right)
		{
			return left.Equals(right);
		}

		// Token: 0x06000AB3 RID: 2739 RVA: 0x000380CA File Offset: 0x000362CA
		[CompilerGenerated]
		public override readonly int GetHashCode()
		{
			return EqualityComparer<MagnetStateType>.Default.GetHashCode(this.<StateType>k__BackingField) * -1521134295 + EqualityComparer<TimeSpan>.Default.GetHashCode(this.<Until>k__BackingField);
		}

		// Token: 0x06000AB4 RID: 2740 RVA: 0x000380F3 File Offset: 0x000362F3
		[CompilerGenerated]
		public override readonly bool Equals(object obj)
		{
			return obj is MagnetState && this.Equals((MagnetState)obj);
		}

		// Token: 0x06000AB5 RID: 2741 RVA: 0x0003810B File Offset: 0x0003630B
		[CompilerGenerated]
		public readonly bool Equals(MagnetState other)
		{
			return EqualityComparer<MagnetStateType>.Default.Equals(this.<StateType>k__BackingField, other.<StateType>k__BackingField) && EqualityComparer<TimeSpan>.Default.Equals(this.<Until>k__BackingField, other.<Until>k__BackingField);
		}

		// Token: 0x06000AB6 RID: 2742 RVA: 0x0003813D File Offset: 0x0003633D
		[CompilerGenerated]
		public readonly void Deconstruct(out MagnetStateType StateType, out TimeSpan Until)
		{
			StateType = this.StateType;
			Until = this.Until;
		}

		// Token: 0x04000692 RID: 1682
		public static readonly MagnetState Inactive = new MagnetState(MagnetStateType.Inactive, TimeSpan.Zero);
	}
}
