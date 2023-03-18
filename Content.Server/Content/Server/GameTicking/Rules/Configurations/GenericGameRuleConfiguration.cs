using System;
using System.Runtime.CompilerServices;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Server.GameTicking.Rules.Configurations
{
	// Token: 0x020004C6 RID: 1222
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class GenericGameRuleConfiguration : GameRuleConfiguration
	{
		// Token: 0x1700039D RID: 925
		// (get) Token: 0x06001955 RID: 6485 RVA: 0x00085F3B File Offset: 0x0008413B
		public override string Id
		{
			get
			{
				return this._id;
			}
		}

		// Token: 0x04000FE8 RID: 4072
		[DataField("id", false, 1, true, false, null)]
		private string _id;
	}
}
