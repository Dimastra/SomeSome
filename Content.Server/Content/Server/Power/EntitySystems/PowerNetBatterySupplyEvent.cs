using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using Robust.Shared.GameObjects;

namespace Content.Server.Power.EntitySystems
{
	// Token: 0x02000296 RID: 662
	[ByRefEvent]
	public readonly struct PowerNetBatterySupplyEvent : IEquatable<PowerNetBatterySupplyEvent>
	{
		// Token: 0x06000D79 RID: 3449 RVA: 0x00046A01 File Offset: 0x00044C01
		public PowerNetBatterySupplyEvent(bool Supply)
		{
			this.Supply = Supply;
		}

		// Token: 0x06000D7A RID: 3450 RVA: 0x00046A0C File Offset: 0x00044C0C
		[CompilerGenerated]
		public override string ToString()
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append("PowerNetBatterySupplyEvent");
			stringBuilder.Append(" { ");
			if (this.PrintMembers(stringBuilder))
			{
				stringBuilder.Append(' ');
			}
			stringBuilder.Append('}');
			return stringBuilder.ToString();
		}

		// Token: 0x06000D7B RID: 3451 RVA: 0x00046A58 File Offset: 0x00044C58
		[CompilerGenerated]
		private bool PrintMembers(StringBuilder builder)
		{
			builder.Append("Supply = ");
			builder.Append(this.Supply.ToString());
			return true;
		}

		// Token: 0x06000D7C RID: 3452 RVA: 0x00046A7F File Offset: 0x00044C7F
		[CompilerGenerated]
		public static bool operator !=(PowerNetBatterySupplyEvent left, PowerNetBatterySupplyEvent right)
		{
			return !(left == right);
		}

		// Token: 0x06000D7D RID: 3453 RVA: 0x00046A8B File Offset: 0x00044C8B
		[CompilerGenerated]
		public static bool operator ==(PowerNetBatterySupplyEvent left, PowerNetBatterySupplyEvent right)
		{
			return left.Equals(right);
		}

		// Token: 0x06000D7E RID: 3454 RVA: 0x00046A95 File Offset: 0x00044C95
		[CompilerGenerated]
		public override int GetHashCode()
		{
			return EqualityComparer<bool>.Default.GetHashCode(this.Supply);
		}

		// Token: 0x06000D7F RID: 3455 RVA: 0x00046AA7 File Offset: 0x00044CA7
		[CompilerGenerated]
		public override bool Equals(object obj)
		{
			return obj is PowerNetBatterySupplyEvent && this.Equals((PowerNetBatterySupplyEvent)obj);
		}

		// Token: 0x06000D80 RID: 3456 RVA: 0x00046ABF File Offset: 0x00044CBF
		[CompilerGenerated]
		public bool Equals(PowerNetBatterySupplyEvent other)
		{
			return EqualityComparer<bool>.Default.Equals(this.Supply, other.Supply);
		}

		// Token: 0x06000D81 RID: 3457 RVA: 0x00046AD7 File Offset: 0x00044CD7
		[CompilerGenerated]
		public void Deconstruct(out bool Supply)
		{
			Supply = this.Supply;
		}

		// Token: 0x04000800 RID: 2048
		public readonly bool Supply;
	}
}
