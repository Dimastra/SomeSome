using System;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared.CrewManifest
{
	// Token: 0x0200054C RID: 1356
	[NetSerializable]
	[Serializable]
	public sealed class RequestCrewManifestMessage : EntityEventArgs
	{
		// Token: 0x17000342 RID: 834
		// (get) Token: 0x06001086 RID: 4230 RVA: 0x00036182 File Offset: 0x00034382
		public EntityUid Id { get; }

		// Token: 0x06001087 RID: 4231 RVA: 0x0003618A File Offset: 0x0003438A
		public RequestCrewManifestMessage(EntityUid id)
		{
			this.Id = id;
		}
	}
}
