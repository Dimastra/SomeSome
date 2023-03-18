using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using Robust.Shared.GameObjects;

namespace Content.Shared.Materials
{
	// Token: 0x02000333 RID: 819
	[ByRefEvent]
	public readonly struct MaterialEntityInsertedEvent : IEquatable<MaterialEntityInsertedEvent>
	{
		// Token: 0x06000966 RID: 2406 RVA: 0x0001F788 File Offset: 0x0001D988
		[NullableContext(1)]
		public MaterialEntityInsertedEvent(MaterialComponent MaterialComp)
		{
			this.MaterialComp = MaterialComp;
		}

		// Token: 0x06000967 RID: 2407 RVA: 0x0001F794 File Offset: 0x0001D994
		[CompilerGenerated]
		public override string ToString()
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append("MaterialEntityInsertedEvent");
			stringBuilder.Append(" { ");
			if (this.PrintMembers(stringBuilder))
			{
				stringBuilder.Append(' ');
			}
			stringBuilder.Append('}');
			return stringBuilder.ToString();
		}

		// Token: 0x06000968 RID: 2408 RVA: 0x0001F7E0 File Offset: 0x0001D9E0
		[CompilerGenerated]
		private bool PrintMembers(StringBuilder builder)
		{
			builder.Append("MaterialComp = ");
			builder.Append(this.MaterialComp);
			return true;
		}

		// Token: 0x06000969 RID: 2409 RVA: 0x0001F7FC File Offset: 0x0001D9FC
		[CompilerGenerated]
		public static bool operator !=(MaterialEntityInsertedEvent left, MaterialEntityInsertedEvent right)
		{
			return !(left == right);
		}

		// Token: 0x0600096A RID: 2410 RVA: 0x0001F808 File Offset: 0x0001DA08
		[CompilerGenerated]
		public static bool operator ==(MaterialEntityInsertedEvent left, MaterialEntityInsertedEvent right)
		{
			return left.Equals(right);
		}

		// Token: 0x0600096B RID: 2411 RVA: 0x0001F812 File Offset: 0x0001DA12
		[CompilerGenerated]
		public override int GetHashCode()
		{
			return EqualityComparer<MaterialComponent>.Default.GetHashCode(this.MaterialComp);
		}

		// Token: 0x0600096C RID: 2412 RVA: 0x0001F824 File Offset: 0x0001DA24
		[CompilerGenerated]
		public override bool Equals(object obj)
		{
			return obj is MaterialEntityInsertedEvent && this.Equals((MaterialEntityInsertedEvent)obj);
		}

		// Token: 0x0600096D RID: 2413 RVA: 0x0001F83C File Offset: 0x0001DA3C
		[CompilerGenerated]
		public bool Equals(MaterialEntityInsertedEvent other)
		{
			return EqualityComparer<MaterialComponent>.Default.Equals(this.MaterialComp, other.MaterialComp);
		}

		// Token: 0x0600096E RID: 2414 RVA: 0x0001F854 File Offset: 0x0001DA54
		[NullableContext(1)]
		[CompilerGenerated]
		public void Deconstruct(out MaterialComponent MaterialComp)
		{
			MaterialComp = this.MaterialComp;
		}

		// Token: 0x0400095A RID: 2394
		[Nullable(1)]
		public readonly MaterialComponent MaterialComp;
	}
}
