using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Maths;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.ViewVariables;

namespace Content.Shared.Sprite
{
	// Token: 0x02000170 RID: 368
	[RegisterComponent]
	[NetworkedComponent]
	public sealed class RandomSpriteComponent : Component
	{
		// Token: 0x04000439 RID: 1081
		[TupleElementNames(new string[]
		{
			"State",
			"Color"
		})]
		[Nullable(new byte[]
		{
			1,
			1,
			1,
			0,
			1,
			2
		})]
		[ViewVariables]
		[DataField("available", false, 1, false, false, null)]
		public List<Dictionary<string, ValueTuple<string, string>>> Available = new List<Dictionary<string, ValueTuple<string, string>>>();

		// Token: 0x0400043A RID: 1082
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
		[ViewVariables]
		[DataField("selected", false, 1, false, false, null)]
		public Dictionary<string, ValueTuple<string, Color?>> Selected = new Dictionary<string, ValueTuple<string, Color?>>();
	}
}
