using System;
using Robust.Shared.GameObjects;
using Robust.Shared.Map;
using Robust.Shared.Serialization;

namespace Content.Shared.Decals
{
	// Token: 0x0200052B RID: 1323
	[NetSerializable]
	[Serializable]
	public sealed class RequestDecalRemovalEvent : EntityEventArgs
	{
		// Token: 0x06001007 RID: 4103 RVA: 0x00033A59 File Offset: 0x00031C59
		public RequestDecalRemovalEvent(EntityCoordinates coordinates)
		{
			this.Coordinates = coordinates;
		}

		// Token: 0x04000F2D RID: 3885
		public EntityCoordinates Coordinates;
	}
}
