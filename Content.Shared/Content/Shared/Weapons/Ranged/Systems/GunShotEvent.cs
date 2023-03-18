using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using Robust.Shared.GameObjects;

namespace Content.Shared.Weapons.Ranged.Systems
{
	// Token: 0x0200004A RID: 74
	[ByRefEvent]
	public struct GunShotEvent : IEquatable<GunShotEvent>
	{
		// Token: 0x06000118 RID: 280 RVA: 0x00006BCB File Offset: 0x00004DCB
		public GunShotEvent(EntityUid User)
		{
			this.User = User;
		}

		// Token: 0x1700002B RID: 43
		// (get) Token: 0x06000119 RID: 281 RVA: 0x00006BD4 File Offset: 0x00004DD4
		// (set) Token: 0x0600011A RID: 282 RVA: 0x00006BDC File Offset: 0x00004DDC
		public EntityUid User { readonly get; set; }

		// Token: 0x0600011B RID: 283 RVA: 0x00006BE8 File Offset: 0x00004DE8
		[CompilerGenerated]
		public override readonly string ToString()
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append("GunShotEvent");
			stringBuilder.Append(" { ");
			if (this.PrintMembers(stringBuilder))
			{
				stringBuilder.Append(' ');
			}
			stringBuilder.Append('}');
			return stringBuilder.ToString();
		}

		// Token: 0x0600011C RID: 284 RVA: 0x00006C34 File Offset: 0x00004E34
		[CompilerGenerated]
		private readonly bool PrintMembers(StringBuilder builder)
		{
			builder.Append("User = ");
			builder.Append(this.User.ToString());
			return true;
		}

		// Token: 0x0600011D RID: 285 RVA: 0x00006C69 File Offset: 0x00004E69
		[CompilerGenerated]
		public static bool operator !=(GunShotEvent left, GunShotEvent right)
		{
			return !(left == right);
		}

		// Token: 0x0600011E RID: 286 RVA: 0x00006C75 File Offset: 0x00004E75
		[CompilerGenerated]
		public static bool operator ==(GunShotEvent left, GunShotEvent right)
		{
			return left.Equals(right);
		}

		// Token: 0x0600011F RID: 287 RVA: 0x00006C7F File Offset: 0x00004E7F
		[CompilerGenerated]
		public override readonly int GetHashCode()
		{
			return EqualityComparer<EntityUid>.Default.GetHashCode(this.<User>k__BackingField);
		}

		// Token: 0x06000120 RID: 288 RVA: 0x00006C91 File Offset: 0x00004E91
		[CompilerGenerated]
		public override readonly bool Equals(object obj)
		{
			return obj is GunShotEvent && this.Equals((GunShotEvent)obj);
		}

		// Token: 0x06000121 RID: 289 RVA: 0x00006CA9 File Offset: 0x00004EA9
		[CompilerGenerated]
		public readonly bool Equals(GunShotEvent other)
		{
			return EqualityComparer<EntityUid>.Default.Equals(this.<User>k__BackingField, other.<User>k__BackingField);
		}

		// Token: 0x06000122 RID: 290 RVA: 0x00006CC1 File Offset: 0x00004EC1
		[CompilerGenerated]
		public readonly void Deconstruct(out EntityUid User)
		{
			User = this.User;
		}
	}
}
