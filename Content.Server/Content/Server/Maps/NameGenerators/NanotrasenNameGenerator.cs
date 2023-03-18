using System;
using System.Runtime.CompilerServices;
using Robust.Shared.IoC;
using Robust.Shared.Random;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Server.Maps.NameGenerators
{
	// Token: 0x020003DB RID: 987
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class NanotrasenNameGenerator : StationNameGenerator
	{
		// Token: 0x170002DF RID: 735
		// (get) Token: 0x06001450 RID: 5200 RVA: 0x00069240 File Offset: 0x00067440
		private string Prefix
		{
			get
			{
				return "NT";
			}
		}

		// Token: 0x170002E0 RID: 736
		// (get) Token: 0x06001451 RID: 5201 RVA: 0x00069247 File Offset: 0x00067447
		private string[] SuffixCodes
		{
			get
			{
				return new string[]
				{
					"LV",
					"NX",
					"EV",
					"QT",
					"PR"
				};
			}
		}

		// Token: 0x06001452 RID: 5202 RVA: 0x00069278 File Offset: 0x00067478
		public override string FormatName(string input)
		{
			IRobustRandom random = IoCManager.Resolve<IRobustRandom>();
			object arg = this.Prefix + this.PrefixCreator;
			DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(1, 2);
			defaultInterpolatedStringHandler.AppendFormatted(RandomExtensions.Pick<string>(random, this.SuffixCodes));
			defaultInterpolatedStringHandler.AppendLiteral("-");
			defaultInterpolatedStringHandler.AppendFormatted<int>(random.Next(0, 999), "D3");
			return string.Format(input, arg, defaultInterpolatedStringHandler.ToStringAndClear());
		}

		// Token: 0x04000C8C RID: 3212
		[DataField("prefixCreator", false, 1, false, false, null)]
		public string PrefixCreator;
	}
}
