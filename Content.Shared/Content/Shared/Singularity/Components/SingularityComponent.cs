using System;
using System.Runtime.CompilerServices;
using Robust.Shared.Analyzers;
using Robust.Shared.Audio;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Utility;
using Robust.Shared.ViewVariables;

namespace Content.Shared.Singularity.Components
{
	// Token: 0x020001B6 RID: 438
	[NullableContext(2)]
	[Nullable(0)]
	[RegisterComponent]
	[NetworkedComponent]
	public sealed class SingularityComponent : Component
	{
		// Token: 0x170000E6 RID: 230
		// (get) Token: 0x0600050C RID: 1292 RVA: 0x0001356E File Offset: 0x0001176E
		[DataField("layer", false, 1, false, false, null)]
		public int Layer { get; }

		// Token: 0x04000501 RID: 1281
		[DataField("level", false, 1, false, false, null)]
		[Access]
		public byte Level = 1;

		// Token: 0x04000502 RID: 1282
		[DataField("radsPerLevel", false, 1, false, false, null)]
		[Access]
		[ViewVariables]
		public float RadsPerLevel = 2f;

		// Token: 0x04000503 RID: 1283
		[DataField("energy", false, 1, false, false, null)]
		public float Energy = 180f;

		// Token: 0x04000504 RID: 1284
		[DataField("energyLoss", false, 1, false, false, null)]
		[ViewVariables]
		public float EnergyDrain;

		// Token: 0x04000505 RID: 1285
		[DataField("ambientSound", false, 1, false, false, null)]
		[ViewVariables]
		public SoundSpecifier AmbientSound = new SoundPathSpecifier("/Audio/Effects/singularity_form.ogg", new AudioParams?(AudioParams.Default.WithVolume(5f).WithLoop(true).WithMaxDistance(20f)));

		// Token: 0x04000506 RID: 1286
		[ViewVariables]
		public IPlayingAudioStream AmbientSoundStream;

		// Token: 0x04000507 RID: 1287
		[DataField("formationSound", false, 1, false, false, null)]
		[ViewVariables]
		public SoundSpecifier FormationSound;

		// Token: 0x04000508 RID: 1288
		[DataField("dissipationSound", false, 1, false, false, null)]
		[ViewVariables]
		public SoundSpecifier DissipationSound = new SoundPathSpecifier("/Audio/Effects/singularity_collapse.ogg", new AudioParams?(AudioParams.Default));

		// Token: 0x0400050A RID: 1290
		[Nullable(1)]
		[DataField("baseSprite", false, 1, false, false, null)]
		public SpriteSpecifier.Rsi BaseSprite = new SpriteSpecifier.Rsi(new ResourcePath("Structures/Power/Generation/Singularity/singularity", "/"), "singularity");

		// Token: 0x0400050B RID: 1291
		[DataField("updatePeriod", false, 1, false, false, null)]
		[ViewVariables]
		public TimeSpan TargetUpdatePeriod = TimeSpan.FromSeconds(1.0);

		// Token: 0x0400050C RID: 1292
		[ViewVariables]
		public TimeSpan NextUpdateTime;

		// Token: 0x0400050D RID: 1293
		[ViewVariables]
		public TimeSpan LastUpdateTime;
	}
}
