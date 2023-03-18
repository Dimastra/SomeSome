using System;
using System.Runtime.CompilerServices;

namespace Content.Server.Fax
{
	// Token: 0x020004FF RID: 1279
	[NullableContext(1)]
	[Nullable(0)]
	public static class FaxConstants
	{
		// Token: 0x040010B2 RID: 4274
		public const string FaxPingCommand = "fax_ping";

		// Token: 0x040010B3 RID: 4275
		public const string FaxPongCommand = "fax_pong";

		// Token: 0x040010B4 RID: 4276
		public const string FaxPrintCommand = "fax_print";

		// Token: 0x040010B5 RID: 4277
		public const string FaxNameData = "fax_data_name";

		// Token: 0x040010B6 RID: 4278
		public const string FaxPaperNameData = "fax_data_title";

		// Token: 0x040010B7 RID: 4279
		public const string FaxPaperPrototypeData = "fax_data_prototype";

		// Token: 0x040010B8 RID: 4280
		public const string FaxPaperContentData = "fax_data_content";

		// Token: 0x040010B9 RID: 4281
		public const string FaxPaperStampStateData = "fax_data_stamp_state";

		// Token: 0x040010BA RID: 4282
		public const string FaxPaperStampedByData = "fax_data_stamped_by";

		// Token: 0x040010BB RID: 4283
		public const string FaxSyndicateData = "fax_data_i_am_syndicate";
	}
}
