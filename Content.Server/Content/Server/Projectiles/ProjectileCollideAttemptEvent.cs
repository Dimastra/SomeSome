using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using Robust.Shared.GameObjects;

namespace Content.Server.Projectiles
{
	// Token: 0x0200026C RID: 620
	[ByRefEvent]
	public struct ProjectileCollideAttemptEvent : IEquatable<ProjectileCollideAttemptEvent>
	{
		// Token: 0x06000C53 RID: 3155 RVA: 0x00040C2C File Offset: 0x0003EE2C
		public ProjectileCollideAttemptEvent(EntityUid Target, bool Cancelled)
		{
			this.Target = Target;
			this.Cancelled = Cancelled;
		}

		// Token: 0x170001C9 RID: 457
		// (get) Token: 0x06000C54 RID: 3156 RVA: 0x00040C3C File Offset: 0x0003EE3C
		// (set) Token: 0x06000C55 RID: 3157 RVA: 0x00040C44 File Offset: 0x0003EE44
		public EntityUid Target { readonly get; set; }

		// Token: 0x170001CA RID: 458
		// (get) Token: 0x06000C56 RID: 3158 RVA: 0x00040C4D File Offset: 0x0003EE4D
		// (set) Token: 0x06000C57 RID: 3159 RVA: 0x00040C55 File Offset: 0x0003EE55
		public bool Cancelled { readonly get; set; }

		// Token: 0x06000C58 RID: 3160 RVA: 0x00040C60 File Offset: 0x0003EE60
		[CompilerGenerated]
		public override readonly string ToString()
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append("ProjectileCollideAttemptEvent");
			stringBuilder.Append(" { ");
			if (this.PrintMembers(stringBuilder))
			{
				stringBuilder.Append(' ');
			}
			stringBuilder.Append('}');
			return stringBuilder.ToString();
		}

		// Token: 0x06000C59 RID: 3161 RVA: 0x00040CAC File Offset: 0x0003EEAC
		[CompilerGenerated]
		private readonly bool PrintMembers(StringBuilder builder)
		{
			builder.Append("Target = ");
			builder.Append(this.Target.ToString());
			builder.Append(", Cancelled = ");
			builder.Append(this.Cancelled.ToString());
			return true;
		}

		// Token: 0x06000C5A RID: 3162 RVA: 0x00040D08 File Offset: 0x0003EF08
		[CompilerGenerated]
		public static bool operator !=(ProjectileCollideAttemptEvent left, ProjectileCollideAttemptEvent right)
		{
			return !(left == right);
		}

		// Token: 0x06000C5B RID: 3163 RVA: 0x00040D14 File Offset: 0x0003EF14
		[CompilerGenerated]
		public static bool operator ==(ProjectileCollideAttemptEvent left, ProjectileCollideAttemptEvent right)
		{
			return left.Equals(right);
		}

		// Token: 0x06000C5C RID: 3164 RVA: 0x00040D1E File Offset: 0x0003EF1E
		[CompilerGenerated]
		public override readonly int GetHashCode()
		{
			return EqualityComparer<EntityUid>.Default.GetHashCode(this.<Target>k__BackingField) * -1521134295 + EqualityComparer<bool>.Default.GetHashCode(this.<Cancelled>k__BackingField);
		}

		// Token: 0x06000C5D RID: 3165 RVA: 0x00040D47 File Offset: 0x0003EF47
		[CompilerGenerated]
		public override readonly bool Equals(object obj)
		{
			return obj is ProjectileCollideAttemptEvent && this.Equals((ProjectileCollideAttemptEvent)obj);
		}

		// Token: 0x06000C5E RID: 3166 RVA: 0x00040D5F File Offset: 0x0003EF5F
		[CompilerGenerated]
		public readonly bool Equals(ProjectileCollideAttemptEvent other)
		{
			return EqualityComparer<EntityUid>.Default.Equals(this.<Target>k__BackingField, other.<Target>k__BackingField) && EqualityComparer<bool>.Default.Equals(this.<Cancelled>k__BackingField, other.<Cancelled>k__BackingField);
		}

		// Token: 0x06000C5F RID: 3167 RVA: 0x00040D91 File Offset: 0x0003EF91
		[CompilerGenerated]
		public readonly void Deconstruct(out EntityUid Target, out bool Cancelled)
		{
			Target = this.Target;
			Cancelled = this.Cancelled;
		}
	}
}
