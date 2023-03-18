using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using Robust.Shared.GameObjects;

namespace Content.Shared.Throwing
{
	// Token: 0x020000CF RID: 207
	[ByRefEvent]
	public readonly struct LandEvent : IEquatable<LandEvent>
	{
		// Token: 0x0600023D RID: 573 RVA: 0x0000B110 File Offset: 0x00009310
		public LandEvent(EntityUid? User)
		{
			this.User = User;
		}

		// Token: 0x1700006F RID: 111
		// (get) Token: 0x0600023E RID: 574 RVA: 0x0000B119 File Offset: 0x00009319
		// (set) Token: 0x0600023F RID: 575 RVA: 0x0000B121 File Offset: 0x00009321
		public EntityUid? User { get; set; }

		// Token: 0x06000240 RID: 576 RVA: 0x0000B12C File Offset: 0x0000932C
		[CompilerGenerated]
		public override string ToString()
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append("LandEvent");
			stringBuilder.Append(" { ");
			if (this.PrintMembers(stringBuilder))
			{
				stringBuilder.Append(' ');
			}
			stringBuilder.Append('}');
			return stringBuilder.ToString();
		}

		// Token: 0x06000241 RID: 577 RVA: 0x0000B178 File Offset: 0x00009378
		[CompilerGenerated]
		private bool PrintMembers(StringBuilder builder)
		{
			builder.Append("User = ");
			builder.Append(this.User.ToString());
			return true;
		}

		// Token: 0x06000242 RID: 578 RVA: 0x0000B1AD File Offset: 0x000093AD
		[CompilerGenerated]
		public static bool operator !=(LandEvent left, LandEvent right)
		{
			return !(left == right);
		}

		// Token: 0x06000243 RID: 579 RVA: 0x0000B1B9 File Offset: 0x000093B9
		[CompilerGenerated]
		public static bool operator ==(LandEvent left, LandEvent right)
		{
			return left.Equals(right);
		}

		// Token: 0x06000244 RID: 580 RVA: 0x0000B1C3 File Offset: 0x000093C3
		[CompilerGenerated]
		public override int GetHashCode()
		{
			return EqualityComparer<EntityUid?>.Default.GetHashCode(this.<User>k__BackingField);
		}

		// Token: 0x06000245 RID: 581 RVA: 0x0000B1D5 File Offset: 0x000093D5
		[CompilerGenerated]
		public override bool Equals(object obj)
		{
			return obj is LandEvent && this.Equals((LandEvent)obj);
		}

		// Token: 0x06000246 RID: 582 RVA: 0x0000B1ED File Offset: 0x000093ED
		[CompilerGenerated]
		public bool Equals(LandEvent other)
		{
			return EqualityComparer<EntityUid?>.Default.Equals(this.<User>k__BackingField, other.<User>k__BackingField);
		}

		// Token: 0x06000247 RID: 583 RVA: 0x0000B205 File Offset: 0x00009405
		[CompilerGenerated]
		public void Deconstruct(out EntityUid? User)
		{
			User = this.User;
		}
	}
}
