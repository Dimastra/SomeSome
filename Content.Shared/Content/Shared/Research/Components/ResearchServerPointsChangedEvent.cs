using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using Robust.Shared.GameObjects;

namespace Content.Shared.Research.Components
{
	// Token: 0x0200020D RID: 525
	[ByRefEvent]
	public readonly struct ResearchServerPointsChangedEvent : IEquatable<ResearchServerPointsChangedEvent>
	{
		// Token: 0x060005D1 RID: 1489 RVA: 0x00014CA9 File Offset: 0x00012EA9
		public ResearchServerPointsChangedEvent(EntityUid Server, int Total, int Delta)
		{
			this.Server = Server;
			this.Total = Total;
			this.Delta = Delta;
		}

		// Token: 0x17000120 RID: 288
		// (get) Token: 0x060005D2 RID: 1490 RVA: 0x00014CC0 File Offset: 0x00012EC0
		// (set) Token: 0x060005D3 RID: 1491 RVA: 0x00014CC8 File Offset: 0x00012EC8
		public EntityUid Server { get; set; }

		// Token: 0x17000121 RID: 289
		// (get) Token: 0x060005D4 RID: 1492 RVA: 0x00014CD1 File Offset: 0x00012ED1
		// (set) Token: 0x060005D5 RID: 1493 RVA: 0x00014CD9 File Offset: 0x00012ED9
		public int Total { get; set; }

		// Token: 0x17000122 RID: 290
		// (get) Token: 0x060005D6 RID: 1494 RVA: 0x00014CE2 File Offset: 0x00012EE2
		// (set) Token: 0x060005D7 RID: 1495 RVA: 0x00014CEA File Offset: 0x00012EEA
		public int Delta { get; set; }

		// Token: 0x060005D8 RID: 1496 RVA: 0x00014CF4 File Offset: 0x00012EF4
		[CompilerGenerated]
		public override string ToString()
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append("ResearchServerPointsChangedEvent");
			stringBuilder.Append(" { ");
			if (this.PrintMembers(stringBuilder))
			{
				stringBuilder.Append(' ');
			}
			stringBuilder.Append('}');
			return stringBuilder.ToString();
		}

		// Token: 0x060005D9 RID: 1497 RVA: 0x00014D40 File Offset: 0x00012F40
		[CompilerGenerated]
		private bool PrintMembers(StringBuilder builder)
		{
			builder.Append("Server = ");
			builder.Append(this.Server.ToString());
			builder.Append(", Total = ");
			builder.Append(this.Total.ToString());
			builder.Append(", Delta = ");
			builder.Append(this.Delta.ToString());
			return true;
		}

		// Token: 0x060005DA RID: 1498 RVA: 0x00014DC3 File Offset: 0x00012FC3
		[CompilerGenerated]
		public static bool operator !=(ResearchServerPointsChangedEvent left, ResearchServerPointsChangedEvent right)
		{
			return !(left == right);
		}

		// Token: 0x060005DB RID: 1499 RVA: 0x00014DCF File Offset: 0x00012FCF
		[CompilerGenerated]
		public static bool operator ==(ResearchServerPointsChangedEvent left, ResearchServerPointsChangedEvent right)
		{
			return left.Equals(right);
		}

		// Token: 0x060005DC RID: 1500 RVA: 0x00014DD9 File Offset: 0x00012FD9
		[CompilerGenerated]
		public override int GetHashCode()
		{
			return (EqualityComparer<EntityUid>.Default.GetHashCode(this.<Server>k__BackingField) * -1521134295 + EqualityComparer<int>.Default.GetHashCode(this.<Total>k__BackingField)) * -1521134295 + EqualityComparer<int>.Default.GetHashCode(this.<Delta>k__BackingField);
		}

		// Token: 0x060005DD RID: 1501 RVA: 0x00014E19 File Offset: 0x00013019
		[CompilerGenerated]
		public override bool Equals(object obj)
		{
			return obj is ResearchServerPointsChangedEvent && this.Equals((ResearchServerPointsChangedEvent)obj);
		}

		// Token: 0x060005DE RID: 1502 RVA: 0x00014E34 File Offset: 0x00013034
		[CompilerGenerated]
		public bool Equals(ResearchServerPointsChangedEvent other)
		{
			return EqualityComparer<EntityUid>.Default.Equals(this.<Server>k__BackingField, other.<Server>k__BackingField) && EqualityComparer<int>.Default.Equals(this.<Total>k__BackingField, other.<Total>k__BackingField) && EqualityComparer<int>.Default.Equals(this.<Delta>k__BackingField, other.<Delta>k__BackingField);
		}

		// Token: 0x060005DF RID: 1503 RVA: 0x00014E89 File Offset: 0x00013089
		[CompilerGenerated]
		public void Deconstruct(out EntityUid Server, out int Total, out int Delta)
		{
			Server = this.Server;
			Total = this.Total;
			Delta = this.Delta;
		}
	}
}
