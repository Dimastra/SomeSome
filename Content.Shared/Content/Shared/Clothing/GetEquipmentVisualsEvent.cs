using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Robust.Shared.GameObjects;

namespace Content.Shared.Clothing
{
	// Token: 0x020005A1 RID: 1441
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class GetEquipmentVisualsEvent : EntityEventArgs
	{
		// Token: 0x06001196 RID: 4502 RVA: 0x000395B0 File Offset: 0x000377B0
		public GetEquipmentVisualsEvent(EntityUid equipee, string slot)
		{
			this.Equipee = equipee;
			this.Slot = slot;
		}

		// Token: 0x04001038 RID: 4152
		public readonly EntityUid Equipee;

		// Token: 0x04001039 RID: 4153
		public readonly string Slot;

		// Token: 0x0400103A RID: 4154
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
