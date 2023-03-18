using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using Robust.Shared.GameObjects;

namespace Content.Shared.Body.Part
{
	// Token: 0x0200065A RID: 1626
	[NullableContext(1)]
	[Nullable(0)]
	[ByRefEvent]
	public readonly struct BodyPartAddedEvent : IEquatable<BodyPartAddedEvent>
	{
		// Token: 0x060013D1 RID: 5073 RVA: 0x00042B53 File Offset: 0x00040D53
		public BodyPartAddedEvent(string Slot, BodyPartComponent Part)
		{
			this.Slot = Slot;
			this.Part = Part;
		}

		// Token: 0x170003F8 RID: 1016
		// (get) Token: 0x060013D2 RID: 5074 RVA: 0x00042B63 File Offset: 0x00040D63
		// (set) Token: 0x060013D3 RID: 5075 RVA: 0x00042B6B File Offset: 0x00040D6B
		public string Slot { get; set; }

		// Token: 0x170003F9 RID: 1017
		// (get) Token: 0x060013D4 RID: 5076 RVA: 0x00042B74 File Offset: 0x00040D74
		// (set) Token: 0x060013D5 RID: 5077 RVA: 0x00042B7C File Offset: 0x00040D7C
		public BodyPartComponent Part { get; set; }

		// Token: 0x060013D6 RID: 5078 RVA: 0x00042B88 File Offset: 0x00040D88
		[NullableContext(0)]
		[CompilerGenerated]
		public override string ToString()
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append("BodyPartAddedEvent");
			stringBuilder.Append(" { ");
			if (this.PrintMembers(stringBuilder))
			{
				stringBuilder.Append(' ');
			}
			stringBuilder.Append('}');
			return stringBuilder.ToString();
		}

		// Token: 0x060013D7 RID: 5079 RVA: 0x00042BD4 File Offset: 0x00040DD4
		[NullableContext(0)]
		[CompilerGenerated]
		private bool PrintMembers(StringBuilder builder)
		{
			builder.Append("Slot = ");
			builder.Append(this.Slot);
			builder.Append(", Part = ");
			builder.Append(this.Part);
			return true;
		}

		// Token: 0x060013D8 RID: 5080 RVA: 0x00042C09 File Offset: 0x00040E09
		[CompilerGenerated]
		public static bool operator !=(BodyPartAddedEvent left, BodyPartAddedEvent right)
		{
			return !(left == right);
		}

		// Token: 0x060013D9 RID: 5081 RVA: 0x00042C15 File Offset: 0x00040E15
		[CompilerGenerated]
		public static bool operator ==(BodyPartAddedEvent left, BodyPartAddedEvent right)
		{
			return left.Equals(right);
		}

		// Token: 0x060013DA RID: 5082 RVA: 0x00042C1F File Offset: 0x00040E1F
		[CompilerGenerated]
		public override int GetHashCode()
		{
			return EqualityComparer<string>.Default.GetHashCode(this.<Slot>k__BackingField) * -1521134295 + EqualityComparer<BodyPartComponent>.Default.GetHashCode(this.<Part>k__BackingField);
		}

		// Token: 0x060013DB RID: 5083 RVA: 0x00042C48 File Offset: 0x00040E48
		[NullableContext(0)]
		[CompilerGenerated]
		public override bool Equals(object obj)
		{
			return obj is BodyPartAddedEvent && this.Equals((BodyPartAddedEvent)obj);
		}

		// Token: 0x060013DC RID: 5084 RVA: 0x00042C60 File Offset: 0x00040E60
		[CompilerGenerated]
		public bool Equals(BodyPartAddedEvent other)
		{
			return EqualityComparer<string>.Default.Equals(this.<Slot>k__BackingField, other.<Slot>k__BackingField) && EqualityComparer<BodyPartComponent>.Default.Equals(this.<Part>k__BackingField, other.<Part>k__BackingField);
		}

		// Token: 0x060013DD RID: 5085 RVA: 0x00042C92 File Offset: 0x00040E92
		[CompilerGenerated]
		public void Deconstruct(out string Slot, out BodyPartComponent Part)
		{
			Slot = this.Slot;
			Part = this.Part;
		}
	}
}
