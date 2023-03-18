using System;
using System.Runtime.CompilerServices;
using Content.Server.Arcade.Components;
using Content.Server.Wires;
using Content.Shared.Arcade;
using Content.Shared.Wires;
using Robust.Shared.GameObjects;
using Robust.Shared.Maths;

namespace Content.Server.Arcade
{
	// Token: 0x020007C0 RID: 1984
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class ArcadeOverflowWireAction : BaseToggleWireAction
	{
		// Token: 0x170006AB RID: 1707
		// (get) Token: 0x06002B09 RID: 11017 RVA: 0x000E1394 File Offset: 0x000DF594
		// (set) Token: 0x06002B0A RID: 11018 RVA: 0x000E139C File Offset: 0x000DF59C
		public override Color Color { get; set; } = Color.Red;

		// Token: 0x170006AC RID: 1708
		// (get) Token: 0x06002B0B RID: 11019 RVA: 0x000E13A5 File Offset: 0x000DF5A5
		// (set) Token: 0x06002B0C RID: 11020 RVA: 0x000E13AD File Offset: 0x000DF5AD
		public override string Name { get; set; } = "wire-name-arcade-overflow";

		// Token: 0x170006AD RID: 1709
		// (get) Token: 0x06002B0D RID: 11021 RVA: 0x000E13B6 File Offset: 0x000DF5B6
		[Nullable(2)]
		public override object StatusKey { [NullableContext(2)] get; } = SharedSpaceVillainArcadeComponent.Indicators.HealthLimiter;

		// Token: 0x06002B0E RID: 11022 RVA: 0x000E13C0 File Offset: 0x000DF5C0
		public override void ToggleValue(EntityUid owner, bool setting)
		{
			SpaceVillainArcadeComponent arcade;
			if (this.EntityManager.TryGetComponent<SpaceVillainArcadeComponent>(owner, ref arcade))
			{
				arcade.OverflowFlag = !setting;
			}
		}

		// Token: 0x06002B0F RID: 11023 RVA: 0x000E13E8 File Offset: 0x000DF5E8
		public override bool GetValue(EntityUid owner)
		{
			SpaceVillainArcadeComponent arcade;
			return this.EntityManager.TryGetComponent<SpaceVillainArcadeComponent>(owner, ref arcade) && !arcade.OverflowFlag;
		}

		// Token: 0x06002B10 RID: 11024 RVA: 0x000E1410 File Offset: 0x000DF610
		public override StatusLightState? GetLightState(Wire wire)
		{
			if (this.EntityManager.HasComponent<SpaceVillainArcadeComponent>(wire.Owner))
			{
				return new StatusLightState?((!this.GetValue(wire.Owner)) ? StatusLightState.BlinkingSlow : StatusLightState.On);
			}
			return new StatusLightState?(StatusLightState.Off);
		}
	}
}
