using System;
using System.Runtime.CompilerServices;
using Robust.Shared.Serialization;

namespace Content.Shared.Administration
{
	// Token: 0x02000739 RID: 1849
	[NetSerializable]
	[Serializable]
	public sealed class ReplayPrototypeUploadMsg
	{
		// Token: 0x040016AE RID: 5806
		[Nullable(1)]
		public string PrototypeData;
	}
}
