using System;
using System.Runtime.CompilerServices;
using Content.Shared.Actions.ActionTypes;
using Robust.Shared.Audio;
using Robust.Shared.GameObjects;
using Robust.Shared.Maths;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Utility;
using Robust.Shared.ViewVariables;

namespace Content.Shared.Vehicle.Components
{
	// Token: 0x020000A6 RID: 166
	[NullableContext(2)]
	[Nullable(0)]
	[RegisterComponent]
	public sealed class VehicleComponent : Component
	{
		// Token: 0x17000052 RID: 82
		// (get) Token: 0x060001DA RID: 474 RVA: 0x0000A0C5 File Offset: 0x000082C5
		public bool HasRider
		{
			get
			{
				return this.Rider != null;
			}
		}

		// Token: 0x04000246 RID: 582
		[ViewVariables]
		public EntityUid? Rider;

		// Token: 0x04000247 RID: 583
		public Vector2 BaseBuckleOffset = Vector2.Zero;

		// Token: 0x04000248 RID: 584
		[DataField("hornSound", false, 1, false, false, null)]
		public SoundSpecifier HornSound = new SoundPathSpecifier("/Audio/Effects/Vehicle/carhorn.ogg", null)
		{
			Params = AudioParams.Default.WithVolume(-3f)
		};

		// Token: 0x04000249 RID: 585
		public IPlayingAudioStream HonkPlayingStream;

		// Token: 0x0400024A RID: 586
		[Nullable(1)]
		[DataField("hornAction", false, 1, false, false, null)]
		public InstantAction HornAction = new InstantAction
		{
			UseDelay = new TimeSpan?(TimeSpan.FromSeconds(3.4)),
			Icon = new SpriteSpecifier.Texture(new ResourcePath("Objects/Fun/bikehorn.rsi/icon.png", "/")),
			DisplayName = "action-name-honk",
			Description = "action-desc-honk",
			Event = new HonkActionEvent()
		};

		// Token: 0x0400024B RID: 587
		[ViewVariables]
		public bool HasKey;

		// Token: 0x0400024C RID: 588
		[DataField("northOnly", false, 1, false, false, null)]
		public bool NorthOnly;

		// Token: 0x0400024D RID: 589
		[DataField("northOverride", false, 1, false, false, null)]
		public float NorthOverride;

		// Token: 0x0400024E RID: 590
		[DataField("southOverride", false, 1, false, false, null)]
		public float SouthOverride;
	}
}
