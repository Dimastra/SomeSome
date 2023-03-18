using System;
using System.Runtime.CompilerServices;
using Robust.Shared.GameObjects;
using Robust.Shared.Map;
using Robust.Shared.Serialization;

namespace Content.Shared.Decals
{
	// Token: 0x0200052A RID: 1322
	[NullableContext(1)]
	[Nullable(0)]
	[NetSerializable]
	[Serializable]
	public sealed class RequestDecalPlacementEvent : EntityEventArgs
	{
		// Token: 0x06001006 RID: 4102 RVA: 0x00033A43 File Offset: 0x00031C43
		public RequestDecalPlacementEvent(Decal decal, EntityCoordinates coordinates)
		{
			this.Decal = decal;
			this.Coordinates = coordinates;
		}

		// Token: 0x04000F2B RID: 3883
		public Decal Decal;

		// Token: 0x04000F2C RID: 3884
		public EntityCoordinates Coordinates;
	}
}
