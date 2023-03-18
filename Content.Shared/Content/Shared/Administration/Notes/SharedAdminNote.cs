using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using Robust.Shared.Serialization;

namespace Content.Shared.Administration.Notes
{
	// Token: 0x0200074A RID: 1866
	[NullableContext(1)]
	[Nullable(0)]
	[NetSerializable]
	[Serializable]
	public sealed class SharedAdminNote : IEquatable<SharedAdminNote>
	{
		// Token: 0x060016A4 RID: 5796 RVA: 0x00049BEE File Offset: 0x00047DEE
		public SharedAdminNote(int Id, int? Round, string Message, string CreatedByName, string EditedByName, DateTime CreatedAt, DateTime LastEditedAt)
		{
			this.Id = Id;
			this.Round = Round;
			this.Message = Message;
			this.CreatedByName = CreatedByName;
			this.EditedByName = EditedByName;
			this.CreatedAt = CreatedAt;
			this.LastEditedAt = LastEditedAt;
			base..ctor();
		}

		// Token: 0x170004BE RID: 1214
		// (get) Token: 0x060016A5 RID: 5797 RVA: 0x00049C2B File Offset: 0x00047E2B
		[CompilerGenerated]
		private Type EqualityContract
		{
			[CompilerGenerated]
			get
			{
				return typeof(SharedAdminNote);
			}
		}

		// Token: 0x170004BF RID: 1215
		// (get) Token: 0x060016A6 RID: 5798 RVA: 0x00049C37 File Offset: 0x00047E37
		// (set) Token: 0x060016A7 RID: 5799 RVA: 0x00049C3F File Offset: 0x00047E3F
		public int Id { get; set; }

		// Token: 0x170004C0 RID: 1216
		// (get) Token: 0x060016A8 RID: 5800 RVA: 0x00049C48 File Offset: 0x00047E48
		// (set) Token: 0x060016A9 RID: 5801 RVA: 0x00049C50 File Offset: 0x00047E50
		public int? Round { get; set; }

		// Token: 0x170004C1 RID: 1217
		// (get) Token: 0x060016AA RID: 5802 RVA: 0x00049C59 File Offset: 0x00047E59
		// (set) Token: 0x060016AB RID: 5803 RVA: 0x00049C61 File Offset: 0x00047E61
		public string Message { get; set; }

		// Token: 0x170004C2 RID: 1218
		// (get) Token: 0x060016AC RID: 5804 RVA: 0x00049C6A File Offset: 0x00047E6A
		// (set) Token: 0x060016AD RID: 5805 RVA: 0x00049C72 File Offset: 0x00047E72
		public string CreatedByName { get; set; }

		// Token: 0x170004C3 RID: 1219
		// (get) Token: 0x060016AE RID: 5806 RVA: 0x00049C7B File Offset: 0x00047E7B
		// (set) Token: 0x060016AF RID: 5807 RVA: 0x00049C83 File Offset: 0x00047E83
		public string EditedByName { get; set; }

		// Token: 0x170004C4 RID: 1220
		// (get) Token: 0x060016B0 RID: 5808 RVA: 0x00049C8C File Offset: 0x00047E8C
		// (set) Token: 0x060016B1 RID: 5809 RVA: 0x00049C94 File Offset: 0x00047E94
		public DateTime CreatedAt { get; set; }

		// Token: 0x170004C5 RID: 1221
		// (get) Token: 0x060016B2 RID: 5810 RVA: 0x00049C9D File Offset: 0x00047E9D
		// (set) Token: 0x060016B3 RID: 5811 RVA: 0x00049CA5 File Offset: 0x00047EA5
		public DateTime LastEditedAt { get; set; }

		// Token: 0x060016B4 RID: 5812 RVA: 0x00049CB0 File Offset: 0x00047EB0
		[CompilerGenerated]
		public override string ToString()
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append("SharedAdminNote");
			stringBuilder.Append(" { ");
			if (this.PrintMembers(stringBuilder))
			{
				stringBuilder.Append(' ');
			}
			stringBuilder.Append('}');
			return stringBuilder.ToString();
		}

		// Token: 0x060016B5 RID: 5813 RVA: 0x00049CFC File Offset: 0x00047EFC
		[CompilerGenerated]
		private bool PrintMembers(StringBuilder builder)
		{
			RuntimeHelpers.EnsureSufficientExecutionStack();
			builder.Append("Id = ");
			builder.Append(this.Id.ToString());
			builder.Append(", Round = ");
			builder.Append(this.Round.ToString());
			builder.Append(", Message = ");
			builder.Append(this.Message);
			builder.Append(", CreatedByName = ");
			builder.Append(this.CreatedByName);
			builder.Append(", EditedByName = ");
			builder.Append(this.EditedByName);
			builder.Append(", CreatedAt = ");
			builder.Append(this.CreatedAt.ToString());
			builder.Append(", LastEditedAt = ");
			builder.Append(this.LastEditedAt.ToString());
			return true;
		}

		// Token: 0x060016B6 RID: 5814 RVA: 0x00049DF6 File Offset: 0x00047FF6
		[NullableContext(2)]
		[CompilerGenerated]
		public static bool operator !=(SharedAdminNote left, SharedAdminNote right)
		{
			return !(left == right);
		}

		// Token: 0x060016B7 RID: 5815 RVA: 0x00049E02 File Offset: 0x00048002
		[NullableContext(2)]
		[CompilerGenerated]
		public static bool operator ==(SharedAdminNote left, SharedAdminNote right)
		{
			return left == right || (left != null && left.Equals(right));
		}

		// Token: 0x060016B8 RID: 5816 RVA: 0x00049E18 File Offset: 0x00048018
		[CompilerGenerated]
		public override int GetHashCode()
		{
			return ((((((EqualityComparer<Type>.Default.GetHashCode(this.EqualityContract) * -1521134295 + EqualityComparer<int>.Default.GetHashCode(this.<Id>k__BackingField)) * -1521134295 + EqualityComparer<int?>.Default.GetHashCode(this.<Round>k__BackingField)) * -1521134295 + EqualityComparer<string>.Default.GetHashCode(this.<Message>k__BackingField)) * -1521134295 + EqualityComparer<string>.Default.GetHashCode(this.<CreatedByName>k__BackingField)) * -1521134295 + EqualityComparer<string>.Default.GetHashCode(this.<EditedByName>k__BackingField)) * -1521134295 + EqualityComparer<DateTime>.Default.GetHashCode(this.<CreatedAt>k__BackingField)) * -1521134295 + EqualityComparer<DateTime>.Default.GetHashCode(this.<LastEditedAt>k__BackingField);
		}

		// Token: 0x060016B9 RID: 5817 RVA: 0x00049ED6 File Offset: 0x000480D6
		[NullableContext(2)]
		[CompilerGenerated]
		public override bool Equals(object obj)
		{
			return this.Equals(obj as SharedAdminNote);
		}

		// Token: 0x060016BA RID: 5818 RVA: 0x00049EE4 File Offset: 0x000480E4
		[NullableContext(2)]
		[CompilerGenerated]
		public bool Equals(SharedAdminNote other)
		{
			return this == other || (other != null && this.EqualityContract == other.EqualityContract && EqualityComparer<int>.Default.Equals(this.<Id>k__BackingField, other.<Id>k__BackingField) && EqualityComparer<int?>.Default.Equals(this.<Round>k__BackingField, other.<Round>k__BackingField) && EqualityComparer<string>.Default.Equals(this.<Message>k__BackingField, other.<Message>k__BackingField) && EqualityComparer<string>.Default.Equals(this.<CreatedByName>k__BackingField, other.<CreatedByName>k__BackingField) && EqualityComparer<string>.Default.Equals(this.<EditedByName>k__BackingField, other.<EditedByName>k__BackingField) && EqualityComparer<DateTime>.Default.Equals(this.<CreatedAt>k__BackingField, other.<CreatedAt>k__BackingField) && EqualityComparer<DateTime>.Default.Equals(this.<LastEditedAt>k__BackingField, other.<LastEditedAt>k__BackingField));
		}

		// Token: 0x060016BC RID: 5820 RVA: 0x00049FCC File Offset: 0x000481CC
		[CompilerGenerated]
		private SharedAdminNote(SharedAdminNote original)
		{
			this.Id = original.<Id>k__BackingField;
			this.Round = original.<Round>k__BackingField;
			this.Message = original.<Message>k__BackingField;
			this.CreatedByName = original.<CreatedByName>k__BackingField;
			this.EditedByName = original.<EditedByName>k__BackingField;
			this.CreatedAt = original.<CreatedAt>k__BackingField;
			this.LastEditedAt = original.<LastEditedAt>k__BackingField;
		}

		// Token: 0x060016BD RID: 5821 RVA: 0x0004A034 File Offset: 0x00048234
		[CompilerGenerated]
		public void Deconstruct(out int Id, out int? Round, out string Message, out string CreatedByName, out string EditedByName, out DateTime CreatedAt, out DateTime LastEditedAt)
		{
			Id = this.Id;
			Round = this.Round;
			Message = this.Message;
			CreatedByName = this.CreatedByName;
			EditedByName = this.EditedByName;
			CreatedAt = this.CreatedAt;
			LastEditedAt = this.LastEditedAt;
		}
	}
}
