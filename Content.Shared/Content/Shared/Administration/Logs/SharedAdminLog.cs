using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using Content.Shared.Database;
using Robust.Shared.Serialization;

namespace Content.Shared.Administration.Logs
{
	// Token: 0x02000750 RID: 1872
	[NullableContext(1)]
	[Nullable(0)]
	[NetSerializable]
	[Serializable]
	public readonly struct SharedAdminLog : IEquatable<SharedAdminLog>
	{
		// Token: 0x060016D4 RID: 5844 RVA: 0x0004A2E6 File Offset: 0x000484E6
		public SharedAdminLog(int Id, LogType Type, LogImpact Impact, DateTime Date, string Message, Guid[] Players)
		{
			this.Id = Id;
			this.Type = Type;
			this.Impact = Impact;
			this.Date = Date;
			this.Message = Message;
			this.Players = Players;
		}

		// Token: 0x170004C9 RID: 1225
		// (get) Token: 0x060016D5 RID: 5845 RVA: 0x0004A315 File Offset: 0x00048515
		// (set) Token: 0x060016D6 RID: 5846 RVA: 0x0004A31D File Offset: 0x0004851D
		public int Id { get; set; }

		// Token: 0x170004CA RID: 1226
		// (get) Token: 0x060016D7 RID: 5847 RVA: 0x0004A326 File Offset: 0x00048526
		// (set) Token: 0x060016D8 RID: 5848 RVA: 0x0004A32E File Offset: 0x0004852E
		public LogType Type { get; set; }

		// Token: 0x170004CB RID: 1227
		// (get) Token: 0x060016D9 RID: 5849 RVA: 0x0004A337 File Offset: 0x00048537
		// (set) Token: 0x060016DA RID: 5850 RVA: 0x0004A33F File Offset: 0x0004853F
		public LogImpact Impact { get; set; }

		// Token: 0x170004CC RID: 1228
		// (get) Token: 0x060016DB RID: 5851 RVA: 0x0004A348 File Offset: 0x00048548
		// (set) Token: 0x060016DC RID: 5852 RVA: 0x0004A350 File Offset: 0x00048550
		public DateTime Date { get; set; }

		// Token: 0x170004CD RID: 1229
		// (get) Token: 0x060016DD RID: 5853 RVA: 0x0004A359 File Offset: 0x00048559
		// (set) Token: 0x060016DE RID: 5854 RVA: 0x0004A361 File Offset: 0x00048561
		public string Message { get; set; }

		// Token: 0x170004CE RID: 1230
		// (get) Token: 0x060016DF RID: 5855 RVA: 0x0004A36A File Offset: 0x0004856A
		// (set) Token: 0x060016E0 RID: 5856 RVA: 0x0004A372 File Offset: 0x00048572
		public Guid[] Players { get; set; }

		// Token: 0x060016E1 RID: 5857 RVA: 0x0004A37C File Offset: 0x0004857C
		[NullableContext(0)]
		[CompilerGenerated]
		public override string ToString()
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append("SharedAdminLog");
			stringBuilder.Append(" { ");
			if (this.PrintMembers(stringBuilder))
			{
				stringBuilder.Append(' ');
			}
			stringBuilder.Append('}');
			return stringBuilder.ToString();
		}

		// Token: 0x060016E2 RID: 5858 RVA: 0x0004A3C8 File Offset: 0x000485C8
		[NullableContext(0)]
		[CompilerGenerated]
		private bool PrintMembers(StringBuilder builder)
		{
			builder.Append("Id = ");
			builder.Append(this.Id.ToString());
			builder.Append(", Type = ");
			builder.Append(this.Type.ToString());
			builder.Append(", Impact = ");
			builder.Append(this.Impact.ToString());
			builder.Append(", Date = ");
			builder.Append(this.Date.ToString());
			builder.Append(", Message = ");
			builder.Append(this.Message);
			builder.Append(", Players = ");
			builder.Append(this.Players);
			return true;
		}

		// Token: 0x060016E3 RID: 5859 RVA: 0x0004A4A4 File Offset: 0x000486A4
		[CompilerGenerated]
		public static bool operator !=(SharedAdminLog left, SharedAdminLog right)
		{
			return !(left == right);
		}

		// Token: 0x060016E4 RID: 5860 RVA: 0x0004A4B0 File Offset: 0x000486B0
		[CompilerGenerated]
		public static bool operator ==(SharedAdminLog left, SharedAdminLog right)
		{
			return left.Equals(right);
		}

		// Token: 0x060016E5 RID: 5861 RVA: 0x0004A4BC File Offset: 0x000486BC
		[CompilerGenerated]
		public override int GetHashCode()
		{
			return ((((EqualityComparer<int>.Default.GetHashCode(this.<Id>k__BackingField) * -1521134295 + EqualityComparer<LogType>.Default.GetHashCode(this.<Type>k__BackingField)) * -1521134295 + EqualityComparer<LogImpact>.Default.GetHashCode(this.<Impact>k__BackingField)) * -1521134295 + EqualityComparer<DateTime>.Default.GetHashCode(this.<Date>k__BackingField)) * -1521134295 + EqualityComparer<string>.Default.GetHashCode(this.<Message>k__BackingField)) * -1521134295 + EqualityComparer<Guid[]>.Default.GetHashCode(this.<Players>k__BackingField);
		}

		// Token: 0x060016E6 RID: 5862 RVA: 0x0004A54C File Offset: 0x0004874C
		[NullableContext(0)]
		[CompilerGenerated]
		public override bool Equals(object obj)
		{
			return obj is SharedAdminLog && this.Equals((SharedAdminLog)obj);
		}

		// Token: 0x060016E7 RID: 5863 RVA: 0x0004A564 File Offset: 0x00048764
		[CompilerGenerated]
		public bool Equals(SharedAdminLog other)
		{
			return EqualityComparer<int>.Default.Equals(this.<Id>k__BackingField, other.<Id>k__BackingField) && EqualityComparer<LogType>.Default.Equals(this.<Type>k__BackingField, other.<Type>k__BackingField) && EqualityComparer<LogImpact>.Default.Equals(this.<Impact>k__BackingField, other.<Impact>k__BackingField) && EqualityComparer<DateTime>.Default.Equals(this.<Date>k__BackingField, other.<Date>k__BackingField) && EqualityComparer<string>.Default.Equals(this.<Message>k__BackingField, other.<Message>k__BackingField) && EqualityComparer<Guid[]>.Default.Equals(this.<Players>k__BackingField, other.<Players>k__BackingField);
		}

		// Token: 0x060016E8 RID: 5864 RVA: 0x0004A601 File Offset: 0x00048801
		[CompilerGenerated]
		public void Deconstruct(out int Id, out LogType Type, out LogImpact Impact, out DateTime Date, out string Message, out Guid[] Players)
		{
			Id = this.Id;
			Type = this.Type;
			Impact = this.Impact;
			Date = this.Date;
			Message = this.Message;
			Players = this.Players;
		}
	}
}
