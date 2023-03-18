using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using Robust.Shared.GameObjects;

namespace Content.Server.Power.EntitySystems
{
	// Token: 0x02000295 RID: 661
	[ByRefEvent]
	public readonly struct PowerConsumerReceivedChanged : IEquatable<PowerConsumerReceivedChanged>
	{
		// Token: 0x06000D70 RID: 3440 RVA: 0x000468B3 File Offset: 0x00044AB3
		public PowerConsumerReceivedChanged(float ReceivedPower, float DrawRate)
		{
			this.ReceivedPower = ReceivedPower;
			this.DrawRate = DrawRate;
		}

		// Token: 0x06000D71 RID: 3441 RVA: 0x000468C4 File Offset: 0x00044AC4
		[CompilerGenerated]
		public override string ToString()
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append("PowerConsumerReceivedChanged");
			stringBuilder.Append(" { ");
			if (this.PrintMembers(stringBuilder))
			{
				stringBuilder.Append(' ');
			}
			stringBuilder.Append('}');
			return stringBuilder.ToString();
		}

		// Token: 0x06000D72 RID: 3442 RVA: 0x00046910 File Offset: 0x00044B10
		[CompilerGenerated]
		private bool PrintMembers(StringBuilder builder)
		{
			builder.Append("ReceivedPower = ");
			builder.Append(this.ReceivedPower.ToString());
			builder.Append(", DrawRate = ");
			builder.Append(this.DrawRate.ToString());
			return true;
		}

		// Token: 0x06000D73 RID: 3443 RVA: 0x00046966 File Offset: 0x00044B66
		[CompilerGenerated]
		public static bool operator !=(PowerConsumerReceivedChanged left, PowerConsumerReceivedChanged right)
		{
			return !(left == right);
		}

		// Token: 0x06000D74 RID: 3444 RVA: 0x00046972 File Offset: 0x00044B72
		[CompilerGenerated]
		public static bool operator ==(PowerConsumerReceivedChanged left, PowerConsumerReceivedChanged right)
		{
			return left.Equals(right);
		}

		// Token: 0x06000D75 RID: 3445 RVA: 0x0004697C File Offset: 0x00044B7C
		[CompilerGenerated]
		public override int GetHashCode()
		{
			return EqualityComparer<float>.Default.GetHashCode(this.ReceivedPower) * -1521134295 + EqualityComparer<float>.Default.GetHashCode(this.DrawRate);
		}

		// Token: 0x06000D76 RID: 3446 RVA: 0x000469A5 File Offset: 0x00044BA5
		[CompilerGenerated]
		public override bool Equals(object obj)
		{
			return obj is PowerConsumerReceivedChanged && this.Equals((PowerConsumerReceivedChanged)obj);
		}

		// Token: 0x06000D77 RID: 3447 RVA: 0x000469BD File Offset: 0x00044BBD
		[CompilerGenerated]
		public bool Equals(PowerConsumerReceivedChanged other)
		{
			return EqualityComparer<float>.Default.Equals(this.ReceivedPower, other.ReceivedPower) && EqualityComparer<float>.Default.Equals(this.DrawRate, other.DrawRate);
		}

		// Token: 0x06000D78 RID: 3448 RVA: 0x000469EF File Offset: 0x00044BEF
		[CompilerGenerated]
		public void Deconstruct(out float ReceivedPower, out float DrawRate)
		{
			ReceivedPower = this.ReceivedPower;
			DrawRate = this.DrawRate;
		}

		// Token: 0x040007FE RID: 2046
		public readonly float ReceivedPower;

		// Token: 0x040007FF RID: 2047
		public readonly float DrawRate;
	}
}
