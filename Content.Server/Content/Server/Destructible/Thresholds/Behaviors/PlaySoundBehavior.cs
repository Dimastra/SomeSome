using System;
using System.Runtime.CompilerServices;
using Content.Shared.Audio;
using Robust.Shared.Audio;
using Robust.Shared.GameObjects;
using Robust.Shared.Map;
using Robust.Shared.Player;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Server.Destructible.Thresholds.Behaviors
{
	// Token: 0x020005AB RID: 1451
	[NullableContext(1)]
	[Nullable(0)]
	[DataDefinition]
	[Serializable]
	public sealed class PlaySoundBehavior : IThresholdBehavior
	{
		// Token: 0x17000472 RID: 1138
		// (get) Token: 0x06001E1B RID: 7707 RVA: 0x0009F41E File Offset: 0x0009D61E
		// (set) Token: 0x06001E1C RID: 7708 RVA: 0x0009F426 File Offset: 0x0009D626
		[DataField("sound", false, 1, true, false, null)]
		public SoundSpecifier Sound { get; set; }

		// Token: 0x06001E1D RID: 7709 RVA: 0x0009F430 File Offset: 0x0009D630
		public void Execute(EntityUid owner, DestructibleSystem system, EntityUid? cause = null)
		{
			EntityCoordinates pos = system.EntityManager.GetComponent<TransformComponent>(owner).Coordinates;
			SoundSystem.Play(this.Sound.GetSound(null, null), Filter.Pvs(pos, 2f, null, null), pos, new AudioParams?(AudioHelpers.WithVariation(0.125f)));
		}
	}
}
