using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using Robust.Shared.GameObjects;

namespace Content.Shared.Research.Components
{
	// Token: 0x0200020E RID: 526
	[ByRefEvent]
	public struct ResearchServerGetPointsPerSecondEvent : IEquatable<ResearchServerGetPointsPerSecondEvent>
	{
		// Token: 0x060005E0 RID: 1504 RVA: 0x00014EA7 File Offset: 0x000130A7
		public ResearchServerGetPointsPerSecondEvent(EntityUid Server, int Points)
		{
			this.Server = Server;
			this.Points = Points;
		}

		// Token: 0x17000123 RID: 291
		// (get) Token: 0x060005E1 RID: 1505 RVA: 0x00014EB7 File Offset: 0x000130B7
		// (set) Token: 0x060005E2 RID: 1506 RVA: 0x00014EBF File Offset: 0x000130BF
		public EntityUid Server { readonly get; set; }

		// Token: 0x17000124 RID: 292
		// (get) Token: 0x060005E3 RID: 1507 RVA: 0x00014EC8 File Offset: 0x000130C8
		// (set) Token: 0x060005E4 RID: 1508 RVA: 0x00014ED0 File Offset: 0x000130D0
		public int Points { readonly get; set; }

		// Token: 0x060005E5 RID: 1509 RVA: 0x00014EDC File Offset: 0x000130DC
		[CompilerGenerated]
		public override readonly string ToString()
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append("ResearchServerGetPointsPerSecondEvent");
			stringBuilder.Append(" { ");
			if (this.PrintMembers(stringBuilder))
			{
				stringBuilder.Append(' ');
			}
			stringBuilder.Append('}');
			return stringBuilder.ToString();
		}

		// Token: 0x060005E6 RID: 1510 RVA: 0x00014F28 File Offset: 0x00013128
		[CompilerGenerated]
		private readonly bool PrintMembers(StringBuilder builder)
		{
			builder.Append("Server = ");
			builder.Append(this.Server.ToString());
			builder.Append(", Points = ");
			builder.Append(this.Points.ToString());
			return true;
		}

		// Token: 0x060005E7 RID: 1511 RVA: 0x00014F84 File Offset: 0x00013184
		[CompilerGenerated]
		public static bool operator !=(ResearchServerGetPointsPerSecondEvent left, ResearchServerGetPointsPerSecondEvent right)
		{
			return !(left == right);
		}

		// Token: 0x060005E8 RID: 1512 RVA: 0x00014F90 File Offset: 0x00013190
		[CompilerGenerated]
		public static bool operator ==(ResearchServerGetPointsPerSecondEvent left, ResearchServerGetPointsPerSecondEvent right)
		{
			return left.Equals(right);
		}

		// Token: 0x060005E9 RID: 1513 RVA: 0x00014F9A File Offset: 0x0001319A
		[CompilerGenerated]
		public override readonly int GetHashCode()
		{
			return EqualityComparer<EntityUid>.Default.GetHashCode(this.<Server>k__BackingField) * -1521134295 + EqualityComparer<int>.Default.GetHashCode(this.<Points>k__BackingField);
		}

		// Token: 0x060005EA RID: 1514 RVA: 0x00014FC3 File Offset: 0x000131C3
		[CompilerGenerated]
		public override readonly bool Equals(object obj)
		{
			return obj is ResearchServerGetPointsPerSecondEvent && this.Equals((ResearchServerGetPointsPerSecondEvent)obj);
		}

		// Token: 0x060005EB RID: 1515 RVA: 0x00014FDB File Offset: 0x000131DB
		[CompilerGenerated]
		public readonly bool Equals(ResearchServerGetPointsPerSecondEvent other)
		{
			return EqualityComparer<EntityUid>.Default.Equals(this.<Server>k__BackingField, other.<Server>k__BackingField) && EqualityComparer<int>.Default.Equals(this.<Points>k__BackingField, other.<Points>k__BackingField);
		}

		// Token: 0x060005EC RID: 1516 RVA: 0x0001500D File Offset: 0x0001320D
		[CompilerGenerated]
		public readonly void Deconstruct(out EntityUid Server, out int Points)
		{
			Server = this.Server;
			Points = this.Points;
		}
	}
}
