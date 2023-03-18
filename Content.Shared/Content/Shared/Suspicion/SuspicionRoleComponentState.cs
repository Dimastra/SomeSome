using System;
using System.Runtime.CompilerServices;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared.Suspicion
{
	// Token: 0x020000F5 RID: 245
	[NullableContext(2)]
	[Nullable(0)]
	[NetSerializable]
	[Serializable]
	public sealed class SuspicionRoleComponentState : ComponentState
	{
		// Token: 0x060002BC RID: 700 RVA: 0x0000CB9F File Offset: 0x0000AD9F
		public SuspicionRoleComponentState(string role, bool? antagonist, [TupleElementNames(new string[]
		{
			"name",
			null
		})] [Nullable(new byte[]
		{
			1,
			0,
			1
		})] ValueTuple<string, EntityUid>[] allies)
		{
			this.Role = role;
			this.Antagonist = antagonist;
			this.Allies = allies;
		}

		// Token: 0x04000309 RID: 777
		public readonly string Role;

		// Token: 0x0400030A RID: 778
		public readonly bool? Antagonist;

		// Token: 0x0400030B RID: 779
		[TupleElementNames(new string[]
		{
			"name",
			null
		})]
		[Nullable(new byte[]
		{
			1,
			0,
			1
		})]
		public readonly ValueTuple<string, EntityUid>[] Allies;
	}
}
