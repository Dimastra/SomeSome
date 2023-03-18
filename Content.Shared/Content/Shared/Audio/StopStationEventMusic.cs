using System;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared.Audio
{
	// Token: 0x0200068A RID: 1674
	[NetSerializable]
	[Serializable]
	public sealed class StopStationEventMusic : EntityEventArgs
	{
		// Token: 0x06001482 RID: 5250 RVA: 0x000443F7 File Offset: 0x000425F7
		public StopStationEventMusic(StationEventMusicType type)
		{
			this.Type = type;
		}

		// Token: 0x04001411 RID: 5137
		public StationEventMusicType Type;
	}
}
