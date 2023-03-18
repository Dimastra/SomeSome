using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using Robust.Shared.GameObjects;

namespace Content.Shared.Research.Components
{
	// Token: 0x02000205 RID: 517
	[ByRefEvent]
	public readonly struct ResearchRegistrationChangedEvent : IEquatable<ResearchRegistrationChangedEvent>
	{
		// Token: 0x060005C0 RID: 1472 RVA: 0x00014B08 File Offset: 0x00012D08
		public ResearchRegistrationChangedEvent(EntityUid? Server)
		{
			this.Server = Server;
		}

		// Token: 0x1700011F RID: 287
		// (get) Token: 0x060005C1 RID: 1473 RVA: 0x00014B11 File Offset: 0x00012D11
		// (set) Token: 0x060005C2 RID: 1474 RVA: 0x00014B19 File Offset: 0x00012D19
		public EntityUid? Server { get; set; }

		// Token: 0x060005C3 RID: 1475 RVA: 0x00014B24 File Offset: 0x00012D24
		[CompilerGenerated]
		public override string ToString()
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append("ResearchRegistrationChangedEvent");
			stringBuilder.Append(" { ");
			if (this.PrintMembers(stringBuilder))
			{
				stringBuilder.Append(' ');
			}
			stringBuilder.Append('}');
			return stringBuilder.ToString();
		}

		// Token: 0x060005C4 RID: 1476 RVA: 0x00014B70 File Offset: 0x00012D70
		[CompilerGenerated]
		private bool PrintMembers(StringBuilder builder)
		{
			builder.Append("Server = ");
			builder.Append(this.Server.ToString());
			return true;
		}

		// Token: 0x060005C5 RID: 1477 RVA: 0x00014BA5 File Offset: 0x00012DA5
		[CompilerGenerated]
		public static bool operator !=(ResearchRegistrationChangedEvent left, ResearchRegistrationChangedEvent right)
		{
			return !(left == right);
		}

		// Token: 0x060005C6 RID: 1478 RVA: 0x00014BB1 File Offset: 0x00012DB1
		[CompilerGenerated]
		public static bool operator ==(ResearchRegistrationChangedEvent left, ResearchRegistrationChangedEvent right)
		{
			return left.Equals(right);
		}

		// Token: 0x060005C7 RID: 1479 RVA: 0x00014BBB File Offset: 0x00012DBB
		[CompilerGenerated]
		public override int GetHashCode()
		{
			return EqualityComparer<EntityUid?>.Default.GetHashCode(this.<Server>k__BackingField);
		}

		// Token: 0x060005C8 RID: 1480 RVA: 0x00014BCD File Offset: 0x00012DCD
		[CompilerGenerated]
		public override bool Equals(object obj)
		{
			return obj is ResearchRegistrationChangedEvent && this.Equals((ResearchRegistrationChangedEvent)obj);
		}

		// Token: 0x060005C9 RID: 1481 RVA: 0x00014BE5 File Offset: 0x00012DE5
		[CompilerGenerated]
		public bool Equals(ResearchRegistrationChangedEvent other)
		{
			return EqualityComparer<EntityUid?>.Default.Equals(this.<Server>k__BackingField, other.<Server>k__BackingField);
		}

		// Token: 0x060005CA RID: 1482 RVA: 0x00014BFD File Offset: 0x00012DFD
		[CompilerGenerated]
		public void Deconstruct(out EntityUid? Server)
		{
			Server = this.Server;
		}
	}
}
