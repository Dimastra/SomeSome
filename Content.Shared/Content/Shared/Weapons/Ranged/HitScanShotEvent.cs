using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using Robust.Shared.GameObjects;

namespace Content.Shared.Weapons.Ranged
{
	// Token: 0x02000045 RID: 69
	[ByRefEvent]
	public struct HitScanShotEvent : IEquatable<HitScanShotEvent>
	{
		// Token: 0x0600008A RID: 138 RVA: 0x00003256 File Offset: 0x00001456
		public HitScanShotEvent(EntityUid? User, EntityUid Target)
		{
			this.User = User;
			this.Target = Target;
		}

		// Token: 0x17000027 RID: 39
		// (get) Token: 0x0600008B RID: 139 RVA: 0x00003266 File Offset: 0x00001466
		// (set) Token: 0x0600008C RID: 140 RVA: 0x0000326E File Offset: 0x0000146E
		public EntityUid? User { readonly get; set; }

		// Token: 0x17000028 RID: 40
		// (get) Token: 0x0600008D RID: 141 RVA: 0x00003277 File Offset: 0x00001477
		// (set) Token: 0x0600008E RID: 142 RVA: 0x0000327F File Offset: 0x0000147F
		public EntityUid Target { readonly get; set; }

		// Token: 0x0600008F RID: 143 RVA: 0x00003288 File Offset: 0x00001488
		[CompilerGenerated]
		public override readonly string ToString()
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append("HitScanShotEvent");
			stringBuilder.Append(" { ");
			if (this.PrintMembers(stringBuilder))
			{
				stringBuilder.Append(' ');
			}
			stringBuilder.Append('}');
			return stringBuilder.ToString();
		}

		// Token: 0x06000090 RID: 144 RVA: 0x000032D4 File Offset: 0x000014D4
		[CompilerGenerated]
		private readonly bool PrintMembers(StringBuilder builder)
		{
			builder.Append("User = ");
			builder.Append(this.User.ToString());
			builder.Append(", Target = ");
			builder.Append(this.Target.ToString());
			return true;
		}

		// Token: 0x06000091 RID: 145 RVA: 0x00003330 File Offset: 0x00001530
		[CompilerGenerated]
		public static bool operator !=(HitScanShotEvent left, HitScanShotEvent right)
		{
			return !(left == right);
		}

		// Token: 0x06000092 RID: 146 RVA: 0x0000333C File Offset: 0x0000153C
		[CompilerGenerated]
		public static bool operator ==(HitScanShotEvent left, HitScanShotEvent right)
		{
			return left.Equals(right);
		}

		// Token: 0x06000093 RID: 147 RVA: 0x00003346 File Offset: 0x00001546
		[CompilerGenerated]
		public override readonly int GetHashCode()
		{
			return EqualityComparer<EntityUid?>.Default.GetHashCode(this.<User>k__BackingField) * -1521134295 + EqualityComparer<EntityUid>.Default.GetHashCode(this.<Target>k__BackingField);
		}

		// Token: 0x06000094 RID: 148 RVA: 0x0000336F File Offset: 0x0000156F
		[CompilerGenerated]
		public override readonly bool Equals(object obj)
		{
			return obj is HitScanShotEvent && this.Equals((HitScanShotEvent)obj);
		}

		// Token: 0x06000095 RID: 149 RVA: 0x00003387 File Offset: 0x00001587
		[CompilerGenerated]
		public readonly bool Equals(HitScanShotEvent other)
		{
			return EqualityComparer<EntityUid?>.Default.Equals(this.<User>k__BackingField, other.<User>k__BackingField) && EqualityComparer<EntityUid>.Default.Equals(this.<Target>k__BackingField, other.<Target>k__BackingField);
		}

		// Token: 0x06000096 RID: 150 RVA: 0x000033B9 File Offset: 0x000015B9
		[CompilerGenerated]
		public readonly void Deconstruct(out EntityUid? User, out EntityUid Target)
		{
			User = this.User;
			Target = this.Target;
		}
	}
}
