using System;
using System.Runtime.CompilerServices;
using Content.Shared.Construction;
using Robust.Shared.Audio;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Random;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.ViewVariables;

namespace Content.Server.Construction.Completions
{
	// Token: 0x0200061D RID: 1565
	[NullableContext(1)]
	[Nullable(0)]
	[DataDefinition]
	public sealed class PlaySound : IGraphAction
	{
		// Token: 0x17000508 RID: 1288
		// (get) Token: 0x06002160 RID: 8544 RVA: 0x000AE8F5 File Offset: 0x000ACAF5
		// (set) Token: 0x06002161 RID: 8545 RVA: 0x000AE8FD File Offset: 0x000ACAFD
		[DataField("sound", false, 1, true, false, null)]
		public SoundSpecifier Sound { get; private set; }

		// Token: 0x06002162 RID: 8546 RVA: 0x000AE908 File Offset: 0x000ACB08
		public void PerformAction(EntityUid uid, EntityUid? userUid, IEntityManager entityManager)
		{
			float scale = (float)RandomExtensions.NextGaussian(IoCManager.Resolve<IRobustRandom>(), 1.0, (double)this.Variation);
			entityManager.EntitySysManager.GetEntitySystem<SharedAudioSystem>().PlayPvs(this.Sound, uid, new AudioParams?(this.AudioParams.WithPitchScale(scale)));
		}

		// Token: 0x04001484 RID: 5252
		[DataField("AudioParams", false, 1, false, false, null)]
		public AudioParams AudioParams = AudioParams.Default;

		// Token: 0x04001485 RID: 5253
		[ViewVariables]
		[DataField("variation", false, 1, false, false, null)]
		public float Variation = 0.125f;
	}
}
