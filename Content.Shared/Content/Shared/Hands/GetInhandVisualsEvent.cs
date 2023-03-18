using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Content.Shared.Hands.Components;
using Robust.Shared.GameObjects;

namespace Content.Shared.Hands
{
	// Token: 0x02000428 RID: 1064
	public sealed class GetInhandVisualsEvent : EntityEventArgs
	{
		// Token: 0x06000CCB RID: 3275 RVA: 0x0002A425 File Offset: 0x00028625
		public GetInhandVisualsEvent(EntityUid user, HandLocation location)
		{
			this.User = user;
			this.Location = location;
		}

		// Token: 0x04000C98 RID: 3224
		public readonly EntityUid User;

		// Token: 0x04000C99 RID: 3225
		public readonly HandLocation Location;

		// Token: 0x04000C9A RID: 3226
		[Nullable(new byte[]
		{
			1,
			0,
			1,
			1
		})]
		public List<ValueTuple<string, SharedSpriteComponent.PrototypeLayerData>> Layers = new List<ValueTuple<string, SharedSpriteComponent.PrototypeLayerData>>();
	}
}
