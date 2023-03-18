using System;
using System.Runtime.CompilerServices;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared.Tag
{
	// Token: 0x020000EA RID: 234
	[NullableContext(1)]
	[Nullable(0)]
	[NetSerializable]
	[Serializable]
	public sealed class TagComponentState : ComponentState
	{
		// Token: 0x06000288 RID: 648 RVA: 0x0000C29E File Offset: 0x0000A49E
		public TagComponentState(string[] tags)
		{
			this.Tags = tags;
		}

		// Token: 0x17000079 RID: 121
		// (get) Token: 0x06000289 RID: 649 RVA: 0x0000C2AD File Offset: 0x0000A4AD
		public string[] Tags { get; }
	}
}
