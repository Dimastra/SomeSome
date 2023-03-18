using System;
using System.Runtime.CompilerServices;
using Content.Server.Arcade.Components;
using Content.Server.Wires;
using Content.Shared.Wires;
using Robust.Shared.GameObjects;
using Robust.Shared.Maths;

namespace Content.Server.Arcade
{
	// Token: 0x020007BE RID: 1982
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class ArcadeEnemyInvincibleWireAction : BaseToggleWireAction
	{
		// Token: 0x170006A8 RID: 1704
		// (get) Token: 0x06002B00 RID: 11008 RVA: 0x000E12E4 File Offset: 0x000DF4E4
		// (set) Token: 0x06002B01 RID: 11009 RVA: 0x000E12EC File Offset: 0x000DF4EC
		public override string Name { get; set; } = "wire-name-player-invincible";

		// Token: 0x170006A9 RID: 1705
		// (get) Token: 0x06002B02 RID: 11010 RVA: 0x000E12F5 File Offset: 0x000DF4F5
		// (set) Token: 0x06002B03 RID: 11011 RVA: 0x000E12FD File Offset: 0x000DF4FD
		public override Color Color { get; set; } = Color.Purple;

		// Token: 0x170006AA RID: 1706
		// (get) Token: 0x06002B04 RID: 11012 RVA: 0x000E1306 File Offset: 0x000DF506
		[Nullable(2)]
		public override object StatusKey { [NullableContext(2)] get; }

		// Token: 0x06002B05 RID: 11013 RVA: 0x000E1310 File Offset: 0x000DF510
		public override void ToggleValue(EntityUid owner, bool setting)
		{
			SpaceVillainArcadeComponent arcade;
			if (this.EntityManager.TryGetComponent<SpaceVillainArcadeComponent>(owner, ref arcade))
			{
				arcade.PlayerInvincibilityFlag = !setting;
			}
		}

		// Token: 0x06002B06 RID: 11014 RVA: 0x000E1338 File Offset: 0x000DF538
		public override bool GetValue(EntityUid owner)
		{
			SpaceVillainArcadeComponent arcade;
			return this.EntityManager.TryGetComponent<SpaceVillainArcadeComponent>(owner, ref arcade) && !arcade.PlayerInvincibilityFlag;
		}

		// Token: 0x06002B07 RID: 11015 RVA: 0x000E1360 File Offset: 0x000DF560
		public override StatusLightData? GetStatusLightData(Wire wire)
		{
			return null;
		}
	}
}
