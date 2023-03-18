using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using Robust.Shared.GameObjects;

namespace Content.Shared.Body.Part
{
	// Token: 0x0200065B RID: 1627
	[NullableContext(1)]
	[Nullable(0)]
	[ByRefEvent]
	public readonly struct BodyPartRemovedEvent : IEquatable<BodyPartRemovedEvent>
	{
		// Token: 0x060013DE RID: 5086 RVA: 0x00042CA4 File Offset: 0x00040EA4
		public BodyPartRemovedEvent(string Slot, BodyPartComponent Part)
		{
			this.Slot = Slot;
			this.Part = Part;
		}

		// Token: 0x170003FA RID: 1018
		// (get) Token: 0x060013DF RID: 5087 RVA: 0x00042CB4 File Offset: 0x00040EB4
		// (set) Token: 0x060013E0 RID: 5088 RVA: 0x00042CBC File Offset: 0x00040EBC
		public string Slot { get; set; }

		// Token: 0x170003FB RID: 1019
		// (get) Token: 0x060013E1 RID: 5089 RVA: 0x00042CC5 File Offset: 0x00040EC5
		// (set) Token: 0x060013E2 RID: 5090 RVA: 0x00042CCD File Offset: 0x00040ECD
		public BodyPartComponent Part { get; set; }

		// Token: 0x060013E3 RID: 5091 RVA: 0x00042CD8 File Offset: 0x00040ED8
		[NullableContext(0)]
		[CompilerGenerated]
		public override string ToString()
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append("BodyPartRemovedEvent");
			stringBuilder.Append(" { ");
			if (this.PrintMembers(stringBuilder))
			{
				stringBuilder.Append(' ');
			}
			stringBuilder.Append('}');
			return stringBuilder.ToString();
		}

		// Token: 0x060013E4 RID: 5092 RVA: 0x00042D24 File Offset: 0x00040F24
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

		// Token: 0x060013E5 RID: 5093 RVA: 0x00042D59 File Offset: 0x00040F59
		[CompilerGenerated]
		public static bool operator !=(BodyPartRemovedEvent left, BodyPartRemovedEvent right)
		{
			return !(left == right);
		}

		// Token: 0x060013E6 RID: 5094 RVA: 0x00042D65 File Offset: 0x00040F65
		[CompilerGenerated]
		public static bool operator ==(BodyPartRemovedEvent left, BodyPartRemovedEvent right)
		{
			return left.Equals(right);
		}

		// Token: 0x060013E7 RID: 5095 RVA: 0x00042D6F File Offset: 0x00040F6F
		[CompilerGenerated]
		public override int GetHashCode()
		{
			return EqualityComparer<string>.Default.GetHashCode(this.<Slot>k__BackingField) * -1521134295 + EqualityComparer<BodyPartComponent>.Default.GetHashCode(this.<Part>k__BackingField);
		}

		// Token: 0x060013E8 RID: 5096 RVA: 0x00042D98 File Offset: 0x00040F98
		[NullableContext(0)]
		[CompilerGenerated]
		public override bool Equals(object obj)
		{
			return obj is BodyPartRemovedEvent && this.Equals((BodyPartRemovedEvent)obj);
		}

		// Token: 0x060013E9 RID: 5097 RVA: 0x00042DB0 File Offset: 0x00040FB0
		[CompilerGenerated]
		public bool Equals(BodyPartRemovedEvent other)
		{
			return EqualityComparer<string>.Default.Equals(this.<Slot>k__BackingField, other.<Slot>k__BackingField) && EqualityComparer<BodyPartComponent>.Default.Equals(this.<Part>k__BackingField, other.<Part>k__BackingField);
		}

		// Token: 0x060013EA RID: 5098 RVA: 0x00042DE2 File Offset: 0x00040FE2
		[CompilerGenerated]
		public void Deconstruct(out string Slot, out BodyPartComponent Part)
		{
			Slot = this.Slot;
			Part = this.Part;
		}
	}
}
