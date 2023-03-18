using System;
using System.Runtime.CompilerServices;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;

namespace Content.Shared.Audio
{
	// Token: 0x02000683 RID: 1667
	public abstract class SharedContentAudioSystem : EntitySystem
	{
		// Token: 0x0600147B RID: 5243 RVA: 0x00044398 File Offset: 0x00042598
		public override void Initialize()
		{
			base.Initialize();
			this._audio.OcclusionCollisionMask = 2;
		}

		// Token: 0x0400140B RID: 5131
		[Nullable(1)]
		[Dependency]
		private readonly SharedAudioSystem _audio;
	}
}
