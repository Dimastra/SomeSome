using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using Robust.Shared.GameObjects;

namespace Content.Shared.Lock
{
	// Token: 0x0200035B RID: 859
	[ByRefEvent]
	public readonly struct LockToggledEvent : IEquatable<LockToggledEvent>
	{
		// Token: 0x06000A08 RID: 2568 RVA: 0x00020A53 File Offset: 0x0001EC53
		public LockToggledEvent(bool Locked)
		{
			this.Locked = Locked;
		}

		// Token: 0x170001EF RID: 495
		// (get) Token: 0x06000A09 RID: 2569 RVA: 0x00020A5C File Offset: 0x0001EC5C
		// (set) Token: 0x06000A0A RID: 2570 RVA: 0x00020A64 File Offset: 0x0001EC64
		public bool Locked { get; set; }

		// Token: 0x06000A0B RID: 2571 RVA: 0x00020A70 File Offset: 0x0001EC70
		[CompilerGenerated]
		public override string ToString()
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append("LockToggledEvent");
			stringBuilder.Append(" { ");
			if (this.PrintMembers(stringBuilder))
			{
				stringBuilder.Append(' ');
			}
			stringBuilder.Append('}');
			return stringBuilder.ToString();
		}

		// Token: 0x06000A0C RID: 2572 RVA: 0x00020ABC File Offset: 0x0001ECBC
		[CompilerGenerated]
		private bool PrintMembers(StringBuilder builder)
		{
			builder.Append("Locked = ");
			builder.Append(this.Locked.ToString());
			return true;
		}

		// Token: 0x06000A0D RID: 2573 RVA: 0x00020AF1 File Offset: 0x0001ECF1
		[CompilerGenerated]
		public static bool operator !=(LockToggledEvent left, LockToggledEvent right)
		{
			return !(left == right);
		}

		// Token: 0x06000A0E RID: 2574 RVA: 0x00020AFD File Offset: 0x0001ECFD
		[CompilerGenerated]
		public static bool operator ==(LockToggledEvent left, LockToggledEvent right)
		{
			return left.Equals(right);
		}

		// Token: 0x06000A0F RID: 2575 RVA: 0x00020B07 File Offset: 0x0001ED07
		[CompilerGenerated]
		public override int GetHashCode()
		{
			return EqualityComparer<bool>.Default.GetHashCode(this.<Locked>k__BackingField);
		}

		// Token: 0x06000A10 RID: 2576 RVA: 0x00020B19 File Offset: 0x0001ED19
		[CompilerGenerated]
		public override bool Equals(object obj)
		{
			return obj is LockToggledEvent && this.Equals((LockToggledEvent)obj);
		}

		// Token: 0x06000A11 RID: 2577 RVA: 0x00020B31 File Offset: 0x0001ED31
		[CompilerGenerated]
		public bool Equals(LockToggledEvent other)
		{
			return EqualityComparer<bool>.Default.Equals(this.<Locked>k__BackingField, other.<Locked>k__BackingField);
		}

		// Token: 0x06000A12 RID: 2578 RVA: 0x00020B49 File Offset: 0x0001ED49
		[CompilerGenerated]
		public void Deconstruct(out bool Locked)
		{
			Locked = this.Locked;
		}
	}
}
