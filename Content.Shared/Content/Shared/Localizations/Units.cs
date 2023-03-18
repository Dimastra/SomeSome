using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Runtime.CompilerServices;
using Robust.Shared.Localization;

namespace Content.Shared.Localizations
{
	// Token: 0x0200035E RID: 862
	[NullableContext(1)]
	[Nullable(0)]
	public static class Units
	{
		// Token: 0x06000A2C RID: 2604 RVA: 0x00021610 File Offset: 0x0001F810
		// Note: this type is marked as 'beforefieldinit'.
		static Units()
		{
			Dictionary<string, Units.TypeTable> dictionary = new Dictionary<string, Units.TypeTable>();
			dictionary["generic"] = Units.Generic;
			dictionary["pressure"] = Units.Pressure;
			dictionary["power"] = Units.Power;
			dictionary["energy"] = Units.Energy;
			dictionary["temperature"] = Units.Temperature;
			Units.Types = dictionary;
		}

		// Token: 0x040009D3 RID: 2515
		public static readonly Units.TypeTable Generic = new Units.TypeTable(new Units.TypeTable.Entry[]
		{
			new Units.TypeTable.Entry(new ValueTuple<double?, double?>(null, new double?(1E-24)), 1E+24, "si--y"),
			new Units.TypeTable.Entry(new ValueTuple<double?, double?>(new double?(1E-24), new double?(1E-21)), 1E+21, "si--z"),
			new Units.TypeTable.Entry(new ValueTuple<double?, double?>(new double?(1E-21), new double?(1E-18)), 1E+18, "si--a"),
			new Units.TypeTable.Entry(new ValueTuple<double?, double?>(new double?(1E-18), new double?(1E-15)), 1000000000000000.0, "si--f"),
			new Units.TypeTable.Entry(new ValueTuple<double?, double?>(new double?(1E-15), new double?(1E-12)), 1000000000000.0, "si--p"),
			new Units.TypeTable.Entry(new ValueTuple<double?, double?>(new double?(1E-12), new double?(1E-09)), 1000000000.0, "si--n"),
			new Units.TypeTable.Entry(new ValueTuple<double?, double?>(new double?(1E-09), new double?(0.001)), 1000000.0, "si--u"),
			new Units.TypeTable.Entry(new ValueTuple<double?, double?>(new double?(0.001), new double?((double)1)), 1000.0, "si--m"),
			new Units.TypeTable.Entry(new ValueTuple<double?, double?>(new double?((double)1), new double?((double)1000)), 1.0, "si"),
			new Units.TypeTable.Entry(new ValueTuple<double?, double?>(new double?((double)1000), new double?(1000000.0)), 0.0001, "si-k"),
			new Units.TypeTable.Entry(new ValueTuple<double?, double?>(new double?(1000000.0), new double?(1000000000.0)), 1E-06, "si-m"),
			new Units.TypeTable.Entry(new ValueTuple<double?, double?>(new double?(1000000000.0), new double?(1000000000000.0)), 1E-09, "si-g"),
			new Units.TypeTable.Entry(new ValueTuple<double?, double?>(new double?(1000000000000.0), new double?(1000000000000000.0)), 1E-12, "si-t"),
			new Units.TypeTable.Entry(new ValueTuple<double?, double?>(new double?(1000000000000000.0), new double?(1E+18)), 1E-15, "si-p"),
			new Units.TypeTable.Entry(new ValueTuple<double?, double?>(new double?(1E+18), new double?(1E+21)), 1E-18, "si-e"),
			new Units.TypeTable.Entry(new ValueTuple<double?, double?>(new double?(1E+21), new double?(1E+24)), 1E-21, "si-z"),
			new Units.TypeTable.Entry(new ValueTuple<double?, double?>(new double?(1E+24), null), 1E-24, "si-y")
		});

		// Token: 0x040009D4 RID: 2516
		public static readonly Units.TypeTable Pressure = new Units.TypeTable(new Units.TypeTable.Entry[]
		{
			new Units.TypeTable.Entry(new ValueTuple<double?, double?>(null, new double?(1E-06)), 1000000000.0, "u--pascal"),
			new Units.TypeTable.Entry(new ValueTuple<double?, double?>(new double?(1E-06), new double?(0.001)), 1000000.0, "m--pascal"),
			new Units.TypeTable.Entry(new ValueTuple<double?, double?>(new double?(0.001), new double?((double)1)), 1000.0, "pascal"),
			new Units.TypeTable.Entry(new ValueTuple<double?, double?>(new double?((double)1), new double?((double)1000)), 1.0, "k-pascal"),
			new Units.TypeTable.Entry(new ValueTuple<double?, double?>(new double?((double)1000), new double?(1000000.0)), 0.0001, "m-pascal"),
			new Units.TypeTable.Entry(new ValueTuple<double?, double?>(new double?(1000000.0), null), 1E-06, "g-pascal")
		});

		// Token: 0x040009D5 RID: 2517
		public static readonly Units.TypeTable Power = new Units.TypeTable(new Units.TypeTable.Entry[]
		{
			new Units.TypeTable.Entry(new ValueTuple<double?, double?>(null, new double?(0.001)), 1000000.0, "u--watt"),
			new Units.TypeTable.Entry(new ValueTuple<double?, double?>(new double?(0.001), new double?((double)1)), 1000.0, "m--watt"),
			new Units.TypeTable.Entry(new ValueTuple<double?, double?>(new double?((double)1), new double?((double)1000)), 1.0, "watt"),
			new Units.TypeTable.Entry(new ValueTuple<double?, double?>(new double?((double)1000), new double?(1000000.0)), 0.0001, "k-watt"),
			new Units.TypeTable.Entry(new ValueTuple<double?, double?>(new double?(1000000.0), new double?(1000000000.0)), 1E-06, "m-watt"),
			new Units.TypeTable.Entry(new ValueTuple<double?, double?>(new double?(1000000000.0), null), 1E-09, "g-watt")
		});

		// Token: 0x040009D6 RID: 2518
		public static readonly Units.TypeTable Energy = new Units.TypeTable(new Units.TypeTable.Entry[]
		{
			new Units.TypeTable.Entry(new ValueTuple<double?, double?>(null, new double?(0.001)), 1000000.0, "u--joule"),
			new Units.TypeTable.Entry(new ValueTuple<double?, double?>(new double?(0.001), new double?((double)1)), 1000.0, "m--joule"),
			new Units.TypeTable.Entry(new ValueTuple<double?, double?>(new double?((double)1), new double?((double)1000)), 1.0, "joule"),
			new Units.TypeTable.Entry(new ValueTuple<double?, double?>(new double?((double)1000), new double?(1000000.0)), 0.0001, "k-joule"),
			new Units.TypeTable.Entry(new ValueTuple<double?, double?>(new double?(1000000.0), new double?(1000000000.0)), 1E-06, "m-joule"),
			new Units.TypeTable.Entry(new ValueTuple<double?, double?>(new double?(1000000000.0), null), 1E-09, "g-joule")
		});

		// Token: 0x040009D7 RID: 2519
		public static readonly Units.TypeTable Temperature = new Units.TypeTable(new Units.TypeTable.Entry[]
		{
			new Units.TypeTable.Entry(new ValueTuple<double?, double?>(null, new double?(0.001)), 1000000.0, "u--kelvin"),
			new Units.TypeTable.Entry(new ValueTuple<double?, double?>(new double?(0.001), new double?((double)1)), 1000.0, "m--kelvin"),
			new Units.TypeTable.Entry(new ValueTuple<double?, double?>(new double?((double)1), new double?(1000.0)), 1.0, "kelvin"),
			new Units.TypeTable.Entry(new ValueTuple<double?, double?>(new double?(1000.0), new double?(1000000.0)), 0.001, "k-kelvin"),
			new Units.TypeTable.Entry(new ValueTuple<double?, double?>(new double?(1000000.0), new double?(1000000000.0)), 1E-06, "m-kelvin"),
			new Units.TypeTable.Entry(new ValueTuple<double?, double?>(new double?(1000000000.0), null), 1E-09, "g-kelvin")
		});

		// Token: 0x040009D8 RID: 2520
		public static readonly Dictionary<string, Units.TypeTable> Types;

		// Token: 0x020007E2 RID: 2018
		[Nullable(0)]
		public sealed class TypeTable
		{
			// Token: 0x0600185D RID: 6237 RVA: 0x0004DEA3 File Offset: 0x0004C0A3
			public TypeTable(params Units.TypeTable.Entry[] e)
			{
				this.E = e;
			}

			// Token: 0x0600185E RID: 6238 RVA: 0x0004DEB4 File Offset: 0x0004C0B4
			[NullableContext(2)]
			public bool TryGetUnit(double val, [NotNullWhen(true)] out Units.TypeTable.Entry winner)
			{
				Units.TypeTable.Entry w = null;
				Units.TypeTable.Entry[] e2 = this.E;
				int i = 0;
				while (i < e2.Length)
				{
					Units.TypeTable.Entry e = e2[i];
					if (e.Range.Item1 == null)
					{
						goto IL_4B;
					}
					double? num = e.Range.Item1;
					if (num.GetValueOrDefault() <= val & num != null)
					{
						goto IL_4B;
					}
					IL_80:
					i++;
					continue;
					IL_4B:
					if (e.Range.Item2 != null)
					{
						num = e.Range.Item2;
						if (!(val < num.GetValueOrDefault() & num != null))
						{
							goto IL_80;
						}
					}
					w = e;
					goto IL_80;
				}
				winner = w;
				return w != null;
			}

			// Token: 0x0600185F RID: 6239 RVA: 0x0004DF54 File Offset: 0x0004C154
			public string Format(double val)
			{
				Units.TypeTable.Entry w;
				if (this.TryGetUnit(val, out w))
				{
					return (val * w.Factor).ToString() + " " + Loc.GetString("units-" + w.Unit);
				}
				return val.ToString(CultureInfo.InvariantCulture);
			}

			// Token: 0x06001860 RID: 6240 RVA: 0x0004DFA8 File Offset: 0x0004C1A8
			public string Format(double val, string fmt)
			{
				Units.TypeTable.Entry w;
				if (this.TryGetUnit(val, out w))
				{
					return (val * w.Factor).ToString(fmt) + " " + Loc.GetString("units-" + w.Unit);
				}
				return val.ToString(fmt);
			}

			// Token: 0x04001849 RID: 6217
			public readonly Units.TypeTable.Entry[] E;

			// Token: 0x020008B1 RID: 2225
			[NullableContext(0)]
			public sealed class Entry
			{
				// Token: 0x06001A4B RID: 6731 RVA: 0x0005205C File Offset: 0x0005025C
				public Entry(ValueTuple<double?, double?> range, double factor, [Nullable(1)] string unit)
				{
					this.Range = range;
					this.Factor = factor;
					this.Unit = unit;
				}

				// Token: 0x04001ABF RID: 6847
				[TupleElementNames(new string[]
				{
					"Min",
					"Max"
				})]
				public readonly ValueTuple<double?, double?> Range;

				// Token: 0x04001AC0 RID: 6848
				public readonly double Factor;

				// Token: 0x04001AC1 RID: 6849
				[Nullable(1)]
				public readonly string Unit;
			}
		}
	}
}
