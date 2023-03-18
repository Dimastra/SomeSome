using System;
using System.Runtime.CompilerServices;
using Content.Server.Holiday.Interfaces;
using Robust.Shared.Localization;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Server.Holiday.Greet
{
	// Token: 0x0200046D RID: 1133
	[DataDefinition]
	public sealed class Custom : IHolidayGreet
	{
		// Token: 0x060016BF RID: 5823 RVA: 0x00077C29 File Offset: 0x00075E29
		[NullableContext(1)]
		public string Greet(HolidayPrototype holiday)
		{
			return Loc.GetString(this._greet);
		}

		// Token: 0x04000E3C RID: 3644
		[Nullable(1)]
		[DataField("text", false, 1, false, false, null)]
		private string _greet = string.Empty;
	}
}
