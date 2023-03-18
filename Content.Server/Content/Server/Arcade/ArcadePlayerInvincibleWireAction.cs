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
	// Token: 0x020007BD RID: 1981
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class ArcadePlayerInvincibleWireAction : BaseToggleWireAction
	{
		// Token: 0x170006A5 RID: 1701
		// (get) Token: 0x06002AF7 RID: 10999 RVA: 0x000E11FE File Offset: 0x000DF3FE
		// (set) Token: 0x06002AF8 RID: 11000 RVA: 0x000E1206 File Offset: 0x000DF406
		public override string Name { get; set; } = "wire-name-arcade-invincible";

		// Token: 0x170006A6 RID: 1702
		// (get) Token: 0x06002AF9 RID: 11001 RVA: 0x000E120F File Offset: 0x000DF40F
		// (set) Token: 0x06002AFA RID: 11002 RVA: 0x000E1217 File Offset: 0x000DF417
		public override Color Color { get; set; } = Color.Purple;

		// Token: 0x170006A7 RID: 1703
		// (get) Token: 0x06002AFB RID: 11003 RVA: 0x000E1220 File Offset: 0x000DF420
		[Nullable(2)]
		public override object StatusKey { [NullableContext(2)] get; } = SharedSpaceVillainArcadeComponent.Indicators.HealthManager;

		// Token: 0x06002AFC RID: 11004 RVA: 0x000E1228 File Offset: 0x000DF428
		public override void ToggleValue(EntityUid owner, bool setting)
		{
			SpaceVillainArcadeComponent arcade;
			if (this.EntityManager.TryGetComponent<SpaceVillainArcadeComponent>(owner, ref arcade))
			{
				arcade.PlayerInvincibilityFlag = !setting;
			}
		}

		// Token: 0x06002AFD RID: 11005 RVA: 0x000E1250 File Offset: 0x000DF450
		public override bool GetValue(EntityUid owner)
		{
			SpaceVillainArcadeComponent arcade;
			return this.EntityManager.TryGetComponent<SpaceVillainArcadeComponent>(owner, ref arcade) && !arcade.PlayerInvincibilityFlag;
		}

		// Token: 0x06002AFE RID: 11006 RVA: 0x000E1278 File Offset: 0x000DF478
		public override StatusLightState? GetLightState(Wire wire)
		{
			SpaceVillainArcadeComponent arcade;
			if (this.EntityManager.TryGetComponent<SpaceVillainArcadeComponent>(wire.Owner, ref arcade))
			{
				return new StatusLightState?((arcade.PlayerInvincibilityFlag || arcade.EnemyInvincibilityFlag) ? StatusLightState.BlinkingSlow : StatusLightState.On);
			}
			return new StatusLightState?(StatusLightState.Off);
		}
	}
}
