using System;
using System.Globalization;
using System.Runtime.CompilerServices;
using Robust.Shared.IoC;
using Robust.Shared.Localization;

namespace Content.Shared.Localizations
{
	// Token: 0x0200035D RID: 861
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class ContentLocalizationManager
	{
		// Token: 0x06000A22 RID: 2594 RVA: 0x000211D0 File Offset: 0x0001F3D0
		public void Initialize()
		{
			CultureInfo culture = new CultureInfo("ru-RU");
			this._loc.LoadCulture(culture);
			ILocalizationManager loc = this._loc;
			CultureInfo culture6 = culture;
			string text = "PRESSURE";
			LocFunction locFunction;
			if ((locFunction = ContentLocalizationManager.<>O.<0>__FormatPressure) == null)
			{
				locFunction = (ContentLocalizationManager.<>O.<0>__FormatPressure = new LocFunction(ContentLocalizationManager.FormatPressure));
			}
			loc.AddFunction(culture6, text, locFunction);
			ILocalizationManager loc2 = this._loc;
			CultureInfo culture2 = culture;
			string text2 = "POWERWATTS";
			LocFunction locFunction2;
			if ((locFunction2 = ContentLocalizationManager.<>O.<1>__FormatPowerWatts) == null)
			{
				locFunction2 = (ContentLocalizationManager.<>O.<1>__FormatPowerWatts = new LocFunction(ContentLocalizationManager.FormatPowerWatts));
			}
			loc2.AddFunction(culture2, text2, locFunction2);
			ILocalizationManager loc3 = this._loc;
			CultureInfo culture3 = culture;
			string text3 = "POWERJOULES";
			LocFunction locFunction3;
			if ((locFunction3 = ContentLocalizationManager.<>O.<2>__FormatPowerJoules) == null)
			{
				locFunction3 = (ContentLocalizationManager.<>O.<2>__FormatPowerJoules = new LocFunction(ContentLocalizationManager.FormatPowerJoules));
			}
			loc3.AddFunction(culture3, text3, locFunction3);
			ILocalizationManager loc4 = this._loc;
			CultureInfo culture4 = culture;
			string text4 = "UNITS";
			LocFunction locFunction4;
			if ((locFunction4 = ContentLocalizationManager.<>O.<3>__FormatUnits) == null)
			{
				locFunction4 = (ContentLocalizationManager.<>O.<3>__FormatUnits = new LocFunction(ContentLocalizationManager.FormatUnits));
			}
			loc4.AddFunction(culture4, text4, locFunction4);
			this._loc.AddFunction(culture, "TOSTRING", (LocArgs args) => ContentLocalizationManager.FormatToString(culture, args));
			ILocalizationManager loc5 = this._loc;
			CultureInfo culture5 = culture;
			string text5 = "LOC";
			LocFunction locFunction5;
			if ((locFunction5 = ContentLocalizationManager.<>O.<4>__FormatLoc) == null)
			{
				locFunction5 = (ContentLocalizationManager.<>O.<4>__FormatLoc = new LocFunction(ContentLocalizationManager.FormatLoc));
			}
			loc5.AddFunction(culture5, text5, locFunction5);
		}

		// Token: 0x06000A23 RID: 2595 RVA: 0x0002131B File Offset: 0x0001F51B
		private static ILocValue FormatLoc(LocArgs args)
		{
			return new LocValueString(Loc.GetString(((LocValueString)args.Args[0]).Value));
		}

		// Token: 0x06000A24 RID: 2596 RVA: 0x00021340 File Offset: 0x0001F540
		private static ILocValue FormatToString(CultureInfo culture, LocArgs args)
		{
			ILocValue locValue = args.Args[0];
			string fmt = ((LocValueString)args.Args[1]).Value;
			object obj = locValue.Value;
			IFormattable formattable = obj as IFormattable;
			if (formattable != null)
			{
				return new LocValueString(formattable.ToString(fmt, culture));
			}
			return new LocValueString(((obj != null) ? obj.ToString() : null) ?? "");
		}

		// Token: 0x06000A25 RID: 2597 RVA: 0x000213AC File Offset: 0x0001F5AC
		private static ILocValue FormatUnitsGeneric(LocArgs args, string mode)
		{
			double pressure = ((LocValueNumber)args.Args[0]).Value;
			int places = 0;
			while (pressure > 1000.0 && places < 5)
			{
				pressure /= 1000.0;
				places++;
			}
			return new LocValueString(Loc.GetString(mode, new ValueTuple<string, object>[]
			{
				new ValueTuple<string, object>("divided", pressure),
				new ValueTuple<string, object>("places", places)
			}));
		}

		// Token: 0x06000A26 RID: 2598 RVA: 0x00021434 File Offset: 0x0001F634
		private static ILocValue FormatPressure(LocArgs args)
		{
			return ContentLocalizationManager.FormatUnitsGeneric(args, "zzzz-fmt-pressure");
		}

		// Token: 0x06000A27 RID: 2599 RVA: 0x00021441 File Offset: 0x0001F641
		private static ILocValue FormatPowerWatts(LocArgs args)
		{
			return ContentLocalizationManager.FormatUnitsGeneric(args, "zzzz-fmt-power-watts");
		}

		// Token: 0x06000A28 RID: 2600 RVA: 0x0002144E File Offset: 0x0001F64E
		private static ILocValue FormatPowerJoules(LocArgs args)
		{
			return ContentLocalizationManager.FormatUnitsGeneric(args, "zzzz-fmt-power-joules");
		}

		// Token: 0x06000A29 RID: 2601 RVA: 0x0002145C File Offset: 0x0001F65C
		private static ILocValue FormatUnits(LocArgs args)
		{
			Units.TypeTable ut;
			if (!Units.Types.TryGetValue(((LocValueString)args.Args[0]).Value, out ut))
			{
				throw new ArgumentException("Unknown unit type " + ((LocValueString)args.Args[0]).Value);
			}
			string fmtstr = ((LocValueString)args.Args[1]).Value;
			double max = double.NegativeInfinity;
			double[] iargs = new double[args.Args.Count - 1];
			for (int i = 2; i < args.Args.Count; i++)
			{
				double j = ((LocValueNumber)args.Args[i]).Value;
				if (j > max)
				{
					max = j;
				}
				iargs[i - 2] = j;
			}
			Units.TypeTable.Entry mu;
			if (!ut.TryGetUnit(max, out mu))
			{
				throw new ArgumentException("Unit out of range for type");
			}
			object[] fargs = new object[iargs.Length];
			for (int k = 0; k < iargs.Length; k++)
			{
				fargs[k] = iargs[k] * mu.Factor;
			}
			object[] array = fargs;
			array[array.Length - 1] = Loc.GetString("units-" + mu.Unit.ToLower());
			string text = fmtstr;
			string oldValue = "{UNIT";
			string str = "{";
			DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(0, 1);
			defaultInterpolatedStringHandler.AppendFormatted<int>(fargs.Length - 1);
			return new LocValueString(string.Format(text.Replace(oldValue, str + defaultInterpolatedStringHandler.ToStringAndClear()), fargs));
		}

		// Token: 0x040009D0 RID: 2512
		[Dependency]
		private readonly ILocalizationManager _loc;

		// Token: 0x040009D1 RID: 2513
		private const string Culture = "ru-RU";

		// Token: 0x040009D2 RID: 2514
		public static readonly string[] TimeSpanMinutesFormats = new string[]
		{
			"m\\:ss",
			"mm\\:ss",
			"%m",
			"mm"
		};

		// Token: 0x020007E0 RID: 2016
		[CompilerGenerated]
		private static class <>O
		{
			// Token: 0x04001843 RID: 6211
			[Nullable(0)]
			public static LocFunction <0>__FormatPressure;

			// Token: 0x04001844 RID: 6212
			[Nullable(0)]
			public static LocFunction <1>__FormatPowerWatts;

			// Token: 0x04001845 RID: 6213
			[Nullable(0)]
			public static LocFunction <2>__FormatPowerJoules;

			// Token: 0x04001846 RID: 6214
			[Nullable(0)]
			public static LocFunction <3>__FormatUnits;

			// Token: 0x04001847 RID: 6215
			[Nullable(0)]
			public static LocFunction <4>__FormatLoc;
		}
	}
}
