using System;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Shared.FixedPoint
{
	// Token: 0x0200048C RID: 1164
	[NullableContext(1)]
	[Nullable(0)]
	[CopyByRef]
	[Serializable]
	public struct FixedPoint2 : ISelfSerialize, IComparable<FixedPoint2>, IEquatable<FixedPoint2>, IFormattable
	{
		// Token: 0x170002F3 RID: 755
		// (get) Token: 0x06000DFC RID: 3580 RVA: 0x0002D769 File Offset: 0x0002B969
		// (set) Token: 0x06000DFD RID: 3581 RVA: 0x0002D771 File Offset: 0x0002B971
		public int Value { readonly get; private set; }

		// Token: 0x170002F4 RID: 756
		// (get) Token: 0x06000DFE RID: 3582 RVA: 0x0002D77A File Offset: 0x0002B97A
		public static FixedPoint2 MaxValue { get; } = new FixedPoint2(int.MaxValue);

		// Token: 0x170002F5 RID: 757
		// (get) Token: 0x06000DFF RID: 3583 RVA: 0x0002D781 File Offset: 0x0002B981
		public static FixedPoint2 Epsilon { get; } = new FixedPoint2(1);

		// Token: 0x170002F6 RID: 758
		// (get) Token: 0x06000E00 RID: 3584 RVA: 0x0002D788 File Offset: 0x0002B988
		public static FixedPoint2 Zero { get; } = new FixedPoint2(0);

		// Token: 0x06000E01 RID: 3585 RVA: 0x0002D78F File Offset: 0x0002B98F
		private readonly double ShiftDown()
		{
			return (double)this.Value / Math.Pow(10.0, 2.0);
		}

		// Token: 0x06000E02 RID: 3586 RVA: 0x0002D7B0 File Offset: 0x0002B9B0
		private FixedPoint2(int value)
		{
			this.Value = value;
		}

		// Token: 0x06000E03 RID: 3587 RVA: 0x0002D7B9 File Offset: 0x0002B9B9
		public static FixedPoint2 New(int value)
		{
			return new FixedPoint2(value * (int)Math.Pow(10.0, 2.0));
		}

		// Token: 0x06000E04 RID: 3588 RVA: 0x0002D7DA File Offset: 0x0002B9DA
		public static FixedPoint2 FromCents(int value)
		{
			return new FixedPoint2(value);
		}

		// Token: 0x06000E05 RID: 3589 RVA: 0x0002D7E2 File Offset: 0x0002B9E2
		public static FixedPoint2 New(float value)
		{
			return new FixedPoint2(FixedPoint2.FromFloat(value));
		}

		// Token: 0x06000E06 RID: 3590 RVA: 0x0002D7EF File Offset: 0x0002B9EF
		private static int FromFloat(float value)
		{
			return (int)MathF.Round(value * MathF.Pow(10f, 2f), MidpointRounding.AwayFromZero);
		}

		// Token: 0x06000E07 RID: 3591 RVA: 0x0002D809 File Offset: 0x0002BA09
		public static FixedPoint2 New(double value)
		{
			return new FixedPoint2((int)Math.Round(value * Math.Pow(10.0, 2.0), MidpointRounding.AwayFromZero));
		}

		// Token: 0x06000E08 RID: 3592 RVA: 0x0002D830 File Offset: 0x0002BA30
		public static FixedPoint2 New(string value)
		{
			return FixedPoint2.New(FixedPoint2.FloatFromString(value));
		}

		// Token: 0x06000E09 RID: 3593 RVA: 0x0002D83D File Offset: 0x0002BA3D
		private static float FloatFromString(string value)
		{
			return float.Parse(value, CultureInfo.InvariantCulture);
		}

		// Token: 0x06000E0A RID: 3594 RVA: 0x0002D84A File Offset: 0x0002BA4A
		public static FixedPoint2 operator +(FixedPoint2 a)
		{
			return a;
		}

		// Token: 0x06000E0B RID: 3595 RVA: 0x0002D84D File Offset: 0x0002BA4D
		public static FixedPoint2 operator -(FixedPoint2 a)
		{
			return new FixedPoint2(-a.Value);
		}

		// Token: 0x06000E0C RID: 3596 RVA: 0x0002D85C File Offset: 0x0002BA5C
		public static FixedPoint2 operator +(FixedPoint2 a, FixedPoint2 b)
		{
			return new FixedPoint2(a.Value + b.Value);
		}

		// Token: 0x06000E0D RID: 3597 RVA: 0x0002D872 File Offset: 0x0002BA72
		public static FixedPoint2 operator -(FixedPoint2 a, FixedPoint2 b)
		{
			return new FixedPoint2(a.Value - b.Value);
		}

		// Token: 0x06000E0E RID: 3598 RVA: 0x0002D888 File Offset: 0x0002BA88
		public static FixedPoint2 operator *(FixedPoint2 a, FixedPoint2 b)
		{
			return new FixedPoint2((int)MathF.Round((float)(b.Value * a.Value) / MathF.Pow(10f, 2f), MidpointRounding.AwayFromZero));
		}

		// Token: 0x06000E0F RID: 3599 RVA: 0x0002D8B6 File Offset: 0x0002BAB6
		public static FixedPoint2 operator *(FixedPoint2 a, float b)
		{
			return new FixedPoint2((int)MathF.Round((float)a.Value * b, MidpointRounding.AwayFromZero));
		}

		// Token: 0x06000E10 RID: 3600 RVA: 0x0002D8CE File Offset: 0x0002BACE
		public static FixedPoint2 operator *(FixedPoint2 a, double b)
		{
			return new FixedPoint2((int)Math.Round((double)a.Value * b, MidpointRounding.AwayFromZero));
		}

		// Token: 0x06000E11 RID: 3601 RVA: 0x0002D8E6 File Offset: 0x0002BAE6
		public static FixedPoint2 operator *(FixedPoint2 a, int b)
		{
			return new FixedPoint2(a.Value * b);
		}

		// Token: 0x06000E12 RID: 3602 RVA: 0x0002D8F6 File Offset: 0x0002BAF6
		public static FixedPoint2 operator /(FixedPoint2 a, FixedPoint2 b)
		{
			return new FixedPoint2((int)MathF.Round(MathF.Pow(10f, 2f) * (float)a.Value / (float)b.Value, MidpointRounding.AwayFromZero));
		}

		// Token: 0x06000E13 RID: 3603 RVA: 0x0002D925 File Offset: 0x0002BB25
		public static FixedPoint2 operator /(FixedPoint2 a, float b)
		{
			return new FixedPoint2((int)MathF.Round((float)a.Value / b, MidpointRounding.AwayFromZero));
		}

		// Token: 0x06000E14 RID: 3604 RVA: 0x0002D93D File Offset: 0x0002BB3D
		public static bool operator <=(FixedPoint2 a, int b)
		{
			return a <= FixedPoint2.New(b);
		}

		// Token: 0x06000E15 RID: 3605 RVA: 0x0002D94B File Offset: 0x0002BB4B
		public static bool operator >=(FixedPoint2 a, int b)
		{
			return a >= FixedPoint2.New(b);
		}

		// Token: 0x06000E16 RID: 3606 RVA: 0x0002D959 File Offset: 0x0002BB59
		public static bool operator <(FixedPoint2 a, int b)
		{
			return a < FixedPoint2.New(b);
		}

		// Token: 0x06000E17 RID: 3607 RVA: 0x0002D967 File Offset: 0x0002BB67
		public static bool operator >(FixedPoint2 a, int b)
		{
			return a > FixedPoint2.New(b);
		}

		// Token: 0x06000E18 RID: 3608 RVA: 0x0002D975 File Offset: 0x0002BB75
		public static bool operator ==(FixedPoint2 a, int b)
		{
			return a == FixedPoint2.New(b);
		}

		// Token: 0x06000E19 RID: 3609 RVA: 0x0002D983 File Offset: 0x0002BB83
		public static bool operator !=(FixedPoint2 a, int b)
		{
			return a != FixedPoint2.New(b);
		}

		// Token: 0x06000E1A RID: 3610 RVA: 0x0002D991 File Offset: 0x0002BB91
		public static bool operator ==(FixedPoint2 a, FixedPoint2 b)
		{
			return a.Equals(b);
		}

		// Token: 0x06000E1B RID: 3611 RVA: 0x0002D99B File Offset: 0x0002BB9B
		public static bool operator !=(FixedPoint2 a, FixedPoint2 b)
		{
			return !a.Equals(b);
		}

		// Token: 0x06000E1C RID: 3612 RVA: 0x0002D9A8 File Offset: 0x0002BBA8
		public static bool operator <=(FixedPoint2 a, FixedPoint2 b)
		{
			return a.Value <= b.Value;
		}

		// Token: 0x06000E1D RID: 3613 RVA: 0x0002D9BD File Offset: 0x0002BBBD
		public static bool operator >=(FixedPoint2 a, FixedPoint2 b)
		{
			return a.Value >= b.Value;
		}

		// Token: 0x06000E1E RID: 3614 RVA: 0x0002D9D2 File Offset: 0x0002BBD2
		public static bool operator <(FixedPoint2 a, FixedPoint2 b)
		{
			return a.Value < b.Value;
		}

		// Token: 0x06000E1F RID: 3615 RVA: 0x0002D9E4 File Offset: 0x0002BBE4
		public static bool operator >(FixedPoint2 a, FixedPoint2 b)
		{
			return a.Value > b.Value;
		}

		// Token: 0x06000E20 RID: 3616 RVA: 0x0002D9F6 File Offset: 0x0002BBF6
		public readonly float Float()
		{
			return (float)this.ShiftDown();
		}

		// Token: 0x06000E21 RID: 3617 RVA: 0x0002D9FF File Offset: 0x0002BBFF
		public readonly double Double()
		{
			return this.ShiftDown();
		}

		// Token: 0x06000E22 RID: 3618 RVA: 0x0002DA07 File Offset: 0x0002BC07
		public readonly int Int()
		{
			return (int)this.ShiftDown();
		}

		// Token: 0x06000E23 RID: 3619 RVA: 0x0002DA10 File Offset: 0x0002BC10
		public static implicit operator FixedPoint2(float n)
		{
			return FixedPoint2.New(n);
		}

		// Token: 0x06000E24 RID: 3620 RVA: 0x0002DA18 File Offset: 0x0002BC18
		public static implicit operator FixedPoint2(double n)
		{
			return FixedPoint2.New(n);
		}

		// Token: 0x06000E25 RID: 3621 RVA: 0x0002DA20 File Offset: 0x0002BC20
		public static implicit operator FixedPoint2(int n)
		{
			return FixedPoint2.New(n);
		}

		// Token: 0x06000E26 RID: 3622 RVA: 0x0002DA28 File Offset: 0x0002BC28
		public static explicit operator float(FixedPoint2 n)
		{
			return n.Float();
		}

		// Token: 0x06000E27 RID: 3623 RVA: 0x0002DA31 File Offset: 0x0002BC31
		public static explicit operator double(FixedPoint2 n)
		{
			return n.Double();
		}

		// Token: 0x06000E28 RID: 3624 RVA: 0x0002DA3A File Offset: 0x0002BC3A
		public static explicit operator int(FixedPoint2 n)
		{
			return n.Int();
		}

		// Token: 0x06000E29 RID: 3625 RVA: 0x0002DA43 File Offset: 0x0002BC43
		public static FixedPoint2 Min(params FixedPoint2[] fixedPoints)
		{
			return fixedPoints.Min<FixedPoint2>();
		}

		// Token: 0x06000E2A RID: 3626 RVA: 0x0002DA4B File Offset: 0x0002BC4B
		public static FixedPoint2 Min(FixedPoint2 a, FixedPoint2 b)
		{
			if (!(a < b))
			{
				return b;
			}
			return a;
		}

		// Token: 0x06000E2B RID: 3627 RVA: 0x0002DA59 File Offset: 0x0002BC59
		public static FixedPoint2 Max(FixedPoint2 a, FixedPoint2 b)
		{
			if (!(a > b))
			{
				return b;
			}
			return a;
		}

		// Token: 0x06000E2C RID: 3628 RVA: 0x0002DA67 File Offset: 0x0002BC67
		public static FixedPoint2 Abs(FixedPoint2 a)
		{
			return FixedPoint2.New(Math.Abs(a.Value));
		}

		// Token: 0x06000E2D RID: 3629 RVA: 0x0002DA7A File Offset: 0x0002BC7A
		public static FixedPoint2 Dist(FixedPoint2 a, FixedPoint2 b)
		{
			return FixedPoint2.Abs(a - b);
		}

		// Token: 0x06000E2E RID: 3630 RVA: 0x0002DA88 File Offset: 0x0002BC88
		public static FixedPoint2 Clamp(FixedPoint2 reagent, FixedPoint2 min, FixedPoint2 max)
		{
			if (min > max)
			{
				DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(25, 4);
				defaultInterpolatedStringHandler.AppendFormatted("min");
				defaultInterpolatedStringHandler.AppendLiteral(" ");
				defaultInterpolatedStringHandler.AppendFormatted<FixedPoint2>(min);
				defaultInterpolatedStringHandler.AppendLiteral(" cannot be larger than ");
				defaultInterpolatedStringHandler.AppendFormatted("max");
				defaultInterpolatedStringHandler.AppendLiteral(" ");
				defaultInterpolatedStringHandler.AppendFormatted<FixedPoint2>(max);
				throw new ArgumentException(defaultInterpolatedStringHandler.ToStringAndClear());
			}
			if (reagent < min)
			{
				return min;
			}
			if (!(reagent > max))
			{
				return reagent;
			}
			return max;
		}

		// Token: 0x06000E2F RID: 3631 RVA: 0x0002DB18 File Offset: 0x0002BD18
		[NullableContext(2)]
		public override readonly bool Equals(object obj)
		{
			if (obj is FixedPoint2)
			{
				FixedPoint2 unit = (FixedPoint2)obj;
				return this.Value == unit.Value;
			}
			return false;
		}

		// Token: 0x06000E30 RID: 3632 RVA: 0x0002DB45 File Offset: 0x0002BD45
		public override readonly int GetHashCode()
		{
			return HashCode.Combine<int>(this.Value);
		}

		// Token: 0x06000E31 RID: 3633 RVA: 0x0002DB52 File Offset: 0x0002BD52
		public void Deserialize(string value)
		{
			this.Value = FixedPoint2.FromFloat(FixedPoint2.FloatFromString(value));
		}

		// Token: 0x06000E32 RID: 3634 RVA: 0x0002DB68 File Offset: 0x0002BD68
		public override readonly string ToString()
		{
			return this.ShiftDown().ToString(CultureInfo.InvariantCulture) ?? "";
		}

		// Token: 0x06000E33 RID: 3635 RVA: 0x0002DB91 File Offset: 0x0002BD91
		[NullableContext(2)]
		[return: Nullable(1)]
		public string ToString(string format, IFormatProvider formatProvider)
		{
			return this.ToString();
		}

		// Token: 0x06000E34 RID: 3636 RVA: 0x0002DB9F File Offset: 0x0002BD9F
		public readonly string Serialize()
		{
			return this.ToString();
		}

		// Token: 0x06000E35 RID: 3637 RVA: 0x0002DBAD File Offset: 0x0002BDAD
		public readonly bool Equals(FixedPoint2 other)
		{
			return this.Value == other.Value;
		}

		// Token: 0x06000E36 RID: 3638 RVA: 0x0002DBBE File Offset: 0x0002BDBE
		public readonly int CompareTo(FixedPoint2 other)
		{
			if (other.Value > this.Value)
			{
				return -1;
			}
			if (other.Value < this.Value)
			{
				return 1;
			}
			return 0;
		}

		// Token: 0x04000D52 RID: 3410
		private const int Shift = 2;
	}
}
