using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Robust.Shared.GameObjects;
using Robust.Shared.Maths;
using Robust.Shared.Serialization;

namespace Content.Shared.Sprite
{
	// Token: 0x02000172 RID: 370
	[NetSerializable]
	[Serializable]
	public sealed class RandomSpriteColorComponentState : ComponentState
	{
		// Token: 0x0400043B RID: 1083
		[TupleElementNames(new string[]
		{
			"State",
			"Color"
		})]
		[Nullable(new byte[]
		{
			1,
			1,
			0,
			1
		})]
		public Dictionary<string, ValueTuple<string, Color?>> Selected;
	}
}
