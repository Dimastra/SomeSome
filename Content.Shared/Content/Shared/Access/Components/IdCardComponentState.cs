using System;
using System.Runtime.CompilerServices;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared.Access.Components
{
	// Token: 0x0200077D RID: 1917
	[NullableContext(2)]
	[Nullable(0)]
	[NetSerializable]
	[Serializable]
	public sealed class IdCardComponentState : ComponentState
	{
		// Token: 0x060017AB RID: 6059 RVA: 0x0004CE32 File Offset: 0x0004B032
		public IdCardComponentState(string fullName, string jobTitle)
		{
			this.FullName = fullName;
			this.JobTitle = jobTitle;
		}

		// Token: 0x0400175E RID: 5982
		public string FullName;

		// Token: 0x0400175F RID: 5983
		public string JobTitle;
	}
}
