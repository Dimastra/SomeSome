using System;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared.Audio
{
	// Token: 0x02000680 RID: 1664
	[NetSerializable]
	[Serializable]
	public sealed class AmbientSoundComponentState : ComponentState
	{
		// Token: 0x17000414 RID: 1044
		// (get) Token: 0x06001467 RID: 5223 RVA: 0x0004415D File Offset: 0x0004235D
		// (set) Token: 0x06001468 RID: 5224 RVA: 0x00044165 File Offset: 0x00042365
		public bool Enabled { get; set; }

		// Token: 0x17000415 RID: 1045
		// (get) Token: 0x06001469 RID: 5225 RVA: 0x0004416E File Offset: 0x0004236E
		// (set) Token: 0x0600146A RID: 5226 RVA: 0x00044176 File Offset: 0x00042376
		public float Range { get; set; }

		// Token: 0x17000416 RID: 1046
		// (get) Token: 0x0600146B RID: 5227 RVA: 0x0004417F File Offset: 0x0004237F
		// (set) Token: 0x0600146C RID: 5228 RVA: 0x00044187 File Offset: 0x00042387
		public float Volume { get; set; }
	}
}
