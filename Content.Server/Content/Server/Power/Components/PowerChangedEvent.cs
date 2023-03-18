using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using Robust.Shared.GameObjects;

namespace Content.Server.Power.Components
{
	// Token: 0x020002A3 RID: 675
	[ByRefEvent]
	public readonly struct PowerChangedEvent : IEquatable<PowerChangedEvent>
	{
		// Token: 0x06000DB6 RID: 3510 RVA: 0x00047651 File Offset: 0x00045851
		public PowerChangedEvent(bool Powered, float ReceivingPower)
		{
			this.Powered = Powered;
			this.ReceivingPower = ReceivingPower;
		}

		// Token: 0x06000DB7 RID: 3511 RVA: 0x00047664 File Offset: 0x00045864
		[CompilerGenerated]
		public override string ToString()
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append("PowerChangedEvent");
			stringBuilder.Append(" { ");
			if (this.PrintMembers(stringBuilder))
			{
				stringBuilder.Append(' ');
			}
			stringBuilder.Append('}');
			return stringBuilder.ToString();
		}

		// Token: 0x06000DB8 RID: 3512 RVA: 0x000476B0 File Offset: 0x000458B0
		[CompilerGenerated]
		private bool PrintMembers(StringBuilder builder)
		{
			builder.Append("Powered = ");
			builder.Append(this.Powered.ToString());
			builder.Append(", ReceivingPower = ");
			builder.Append(this.ReceivingPower.ToString());
			return true;
		}

		// Token: 0x06000DB9 RID: 3513 RVA: 0x00047706 File Offset: 0x00045906
		[CompilerGenerated]
		public static bool operator !=(PowerChangedEvent left, PowerChangedEvent right)
		{
			return !(left == right);
		}

		// Token: 0x06000DBA RID: 3514 RVA: 0x00047712 File Offset: 0x00045912
		[CompilerGenerated]
		public static bool operator ==(PowerChangedEvent left, PowerChangedEvent right)
		{
			return left.Equals(right);
		}

		// Token: 0x06000DBB RID: 3515 RVA: 0x0004771C File Offset: 0x0004591C
		[CompilerGenerated]
		public override int GetHashCode()
		{
			return EqualityComparer<bool>.Default.GetHashCode(this.Powered) * -1521134295 + EqualityComparer<float>.Default.GetHashCode(this.ReceivingPower);
		}

		// Token: 0x06000DBC RID: 3516 RVA: 0x00047745 File Offset: 0x00045945
		[CompilerGenerated]
		public override bool Equals(object obj)
		{
			return obj is PowerChangedEvent && this.Equals((PowerChangedEvent)obj);
		}

		// Token: 0x06000DBD RID: 3517 RVA: 0x0004775D File Offset: 0x0004595D
		[CompilerGenerated]
		public bool Equals(PowerChangedEvent other)
		{
			return EqualityComparer<bool>.Default.Equals(this.Powered, other.Powered) && EqualityComparer<float>.Default.Equals(this.ReceivingPower, other.ReceivingPower);
		}

		// Token: 0x06000DBE RID: 3518 RVA: 0x0004778F File Offset: 0x0004598F
		[CompilerGenerated]
		public void Deconstruct(out bool Powered, out float ReceivingPower)
		{
			Powered = this.Powered;
			ReceivingPower = this.ReceivingPower;
		}

		// Token: 0x04000821 RID: 2081
		public readonly bool Powered;

		// Token: 0x04000822 RID: 2082
		public readonly float ReceivingPower;
	}
}
