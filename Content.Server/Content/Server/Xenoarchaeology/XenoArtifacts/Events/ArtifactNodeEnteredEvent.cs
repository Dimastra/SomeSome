using System;
using Robust.Shared.GameObjects;

namespace Content.Server.Xenoarchaeology.XenoArtifacts.Events
{
	// Token: 0x0200003E RID: 62
	public sealed class ArtifactNodeEnteredEvent : EntityEventArgs
	{
		// Token: 0x060000C1 RID: 193 RVA: 0x00005C5F File Offset: 0x00003E5F
		public ArtifactNodeEnteredEvent(int randomSeed)
		{
			this.RandomSeed = randomSeed;
		}

		// Token: 0x0400008C RID: 140
		public readonly int RandomSeed;
	}
}
