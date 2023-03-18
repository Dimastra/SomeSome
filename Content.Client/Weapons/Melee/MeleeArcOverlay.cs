using System;
using System.Runtime.CompilerServices;
using Content.Shared.CombatMode;
using Content.Shared.Weapons.Melee;
using Robust.Client.Graphics;
using Robust.Client.Input;
using Robust.Client.Player;
using Robust.Shared.Enums;
using Robust.Shared.GameObjects;
using Robust.Shared.Map;
using Robust.Shared.Maths;

namespace Content.Client.Weapons.Melee
{
	// Token: 0x0200003B RID: 59
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class MeleeArcOverlay : Overlay
	{
		// Token: 0x1700001C RID: 28
		// (get) Token: 0x060000FE RID: 254 RVA: 0x00005516 File Offset: 0x00003716
		public override OverlaySpace Space
		{
			get
			{
				return 8;
			}
		}

		// Token: 0x060000FF RID: 255 RVA: 0x00008EAF File Offset: 0x000070AF
		public MeleeArcOverlay(IEntityManager entManager, IEyeManager eyeManager, IInputManager inputManager, IPlayerManager playerManager, MeleeWeaponSystem melee, SharedCombatModeSystem combatMode)
		{
			this._entManager = entManager;
			this._eyeManager = eyeManager;
			this._inputManager = inputManager;
			this._playerManager = playerManager;
			this._melee = melee;
			this._combatMode = combatMode;
		}

		// Token: 0x06000100 RID: 256 RVA: 0x00008EE4 File Offset: 0x000070E4
		protected override void Draw(in OverlayDrawArgs args)
		{
			LocalPlayer localPlayer = this._playerManager.LocalPlayer;
			EntityUid? entityUid = (localPlayer != null) ? localPlayer.ControlledEntity : null;
			TransformComponent transformComponent;
			if (!this._entManager.TryGetComponent<TransformComponent>(entityUid, ref transformComponent) || !this._combatMode.IsInCombatMode(entityUid, null))
			{
				return;
			}
			MeleeWeaponComponent weapon = this._melee.GetWeapon(entityUid.Value);
			if (weapon == null)
			{
				return;
			}
			ScreenCoordinates mouseScreenPosition = this._inputManager.MouseScreenPosition;
			MapCoordinates mapCoordinates = this._eyeManager.ScreenToMap(mouseScreenPosition);
			if (mapCoordinates.MapId != args.MapId)
			{
				return;
			}
			MapCoordinates mapPosition = transformComponent.MapPosition;
			if (mapCoordinates.MapId != mapPosition.MapId)
			{
				return;
			}
			Vector2 vector = mapCoordinates.Position - mapPosition.Position;
			if (vector.Equals(Vector2.Zero))
			{
				return;
			}
			vector = vector.Normalized * Math.Min(weapon.Range, vector.Length);
			args.WorldHandle.DrawLine(mapPosition.Position, mapPosition.Position + vector, Color.Aqua);
			if (weapon.Angle.Theta == 0.0)
			{
				return;
			}
			args.WorldHandle.DrawLine(mapPosition.Position, mapPosition.Position + new Angle(-weapon.Angle / 2.0).RotateVec(ref vector), Color.Orange);
			args.WorldHandle.DrawLine(mapPosition.Position, mapPosition.Position + new Angle(weapon.Angle / 2.0).RotateVec(ref vector), Color.Orange);
		}

		// Token: 0x040000AA RID: 170
		private readonly IEntityManager _entManager;

		// Token: 0x040000AB RID: 171
		private readonly IEyeManager _eyeManager;

		// Token: 0x040000AC RID: 172
		private readonly IInputManager _inputManager;

		// Token: 0x040000AD RID: 173
		private readonly IPlayerManager _playerManager;

		// Token: 0x040000AE RID: 174
		private readonly MeleeWeaponSystem _melee;

		// Token: 0x040000AF RID: 175
		private readonly SharedCombatModeSystem _combatMode;
	}
}
