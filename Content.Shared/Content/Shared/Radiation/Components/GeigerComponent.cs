using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Content.Shared.Radiation.Systems;
using Robust.Shared.Analyzers;
using Robust.Shared.Audio;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.ViewVariables;

namespace Content.Shared.Radiation.Components
{
	// Token: 0x0200022D RID: 557
	[RegisterComponent]
	[NetworkedComponent]
	[Access(new Type[]
	{
		typeof(SharedGeigerSystem)
	})]
	public sealed class GeigerComponent : Component
	{
		// Token: 0x04000631 RID: 1585
		[DataField("attachedToSuit", false, 1, false, false, null)]
		public bool AttachedToSuit;

		// Token: 0x04000632 RID: 1586
		[DataField("isEnabled", false, 1, false, false, null)]
		public bool IsEnabled;

		// Token: 0x04000633 RID: 1587
		[ViewVariables]
		[DataField("showExamine", false, 1, false, false, null)]
		public bool ShowExamine;

		// Token: 0x04000634 RID: 1588
		[ViewVariables]
		[DataField("showControl", false, 1, false, false, null)]
		public bool ShowControl;

		// Token: 0x04000635 RID: 1589
		[Nullable(1)]
		[DataField("sounds", false, 1, false, false, null)]
		public Dictionary<GeigerDangerLevel, SoundSpecifier> Sounds = new Dictionary<GeigerDangerLevel, SoundSpecifier>
		{
			{
				GeigerDangerLevel.Low,
				new SoundPathSpecifier("/Audio/Items/Geiger/low.ogg", null)
			},
			{
				GeigerDangerLevel.Med,
				new SoundPathSpecifier("/Audio/Items/Geiger/med.ogg", null)
			},
			{
				GeigerDangerLevel.High,
				new SoundPathSpecifier("/Audio/Items/Geiger/high.ogg", null)
			},
			{
				GeigerDangerLevel.Extreme,
				new SoundPathSpecifier("/Audio/Items/Geiger/ext.ogg", null)
			}
		};

		// Token: 0x04000636 RID: 1590
		[ViewVariables]
		public float CurrentRadiation;

		// Token: 0x04000637 RID: 1591
		[ViewVariables]
		public GeigerDangerLevel DangerLevel;

		// Token: 0x04000638 RID: 1592
		[ViewVariables]
		public EntityUid? User;

		// Token: 0x04000639 RID: 1593
		[Access]
		public bool UiUpdateNeeded;

		// Token: 0x0400063A RID: 1594
		[Nullable(2)]
		public IPlayingAudioStream Stream;
	}
}
