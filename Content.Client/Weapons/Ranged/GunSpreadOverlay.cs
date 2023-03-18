using System;
using System.Runtime.CompilerServices;
using Content.Client.Weapons.Ranged.Systems;
using Content.Shared.Weapons.Ranged.Components;
using Robust.Client.Graphics;
using Robust.Client.Input;
using Robust.Client.Player;
using Robust.Shared.Enums;
using Robust.Shared.GameObjects;
using Robust.Shared.Map;
using Robust.Shared.Maths;
using Robust.Shared.Timing;

namespace Content.Client.Weapons.Ranged
{
	// Token: 0x0200002C RID: 44
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class GunSpreadOverlay : Overlay
	{
		// Token: 0x17000019 RID: 25
		// (get) Token: 0x060000B6 RID: 182 RVA: 0x0000689B File Offset: 0x00004A9B
		public override OverlaySpace Space
		{
			get
			{
				return 4;
			}
		}

		// Token: 0x060000B7 RID: 183 RVA: 0x0000689E File Offset: 0x00004A9E
		public GunSpreadOverlay(IEntityManager entManager, IEyeManager eyeManager, IGameTiming timing, IInputManager input, IPlayerManager player, GunSystem system)
		{
			this._entManager = entManager;
			this._eye = eyeManager;
			this._input = input;
			this._timing = timing;
			this._player = player;
			this._guns = system;
		}

		// Token: 0x060000B8 RID: 184 RVA: 0x000068D4 File Offset: 0x00004AD4
		protected override void Draw(in OverlayDrawArgs args)
		{
			DrawingHandleWorld worldHandle = args.WorldHandle;
			LocalPlayer localPlayer = this._player.LocalPlayer;
			EntityUid? entityUid = (localPlayer != null) ? localPlayer.ControlledEntity : null;
			TransformComponent transformComponent;
			if (entityUid == null || !this._entManager.TryGetComponent<TransformComponent>(entityUid, ref transformComponent))
			{
				return;
			}
			MapCoordinates mapPosition = transformComponent.MapPosition;
			if (mapPosition.MapId == MapId.Nullspace)
			{
				return;
			}
			GunComponent gun = this._guns.GetGun(entityUid.Value);
			if (gun == null)
			{
				return;
			}
			ScreenCoordinates mouseScreenPosition = this._input.MouseScreenPosition;
			MapCoordinates mapCoordinates = this._eye.ScreenToMap(mouseScreenPosition);
			if (mapPosition.MapId != mapCoordinates.MapId)
			{
				return;
			}
			Angle maxAngle = gun.MaxAngle;
			Angle minAngle = gun.MinAngle;
			double totalSeconds = (this._timing.CurTime - gun.NextFire).TotalSeconds;
			Angle angle;
			angle..ctor(MathHelper.Clamp(gun.CurrentAngle.Theta - gun.AngleDecay.Theta * totalSeconds, gun.MinAngle.Theta, gun.MaxAngle.Theta));
			Vector2 vector = mapCoordinates.Position - mapPosition.Position;
			worldHandle.DrawLine(mapPosition.Position, mapCoordinates.Position + vector, Color.Orange);
			worldHandle.DrawLine(mapPosition.Position, mapCoordinates.Position + maxAngle.RotateVec(ref vector), Color.Red);
			worldHandle.DrawLine(mapPosition.Position, mapCoordinates.Position + (-maxAngle).RotateVec(ref vector), Color.Red);
			worldHandle.DrawLine(mapPosition.Position, mapCoordinates.Position + minAngle.RotateVec(ref vector), Color.Green);
			worldHandle.DrawLine(mapPosition.Position, mapCoordinates.Position + (-minAngle).RotateVec(ref vector), Color.Green);
			worldHandle.DrawLine(mapPosition.Position, mapCoordinates.Position + angle.RotateVec(ref vector), Color.Yellow);
			worldHandle.DrawLine(mapPosition.Position, mapCoordinates.Position + (-angle).RotateVec(ref vector), Color.Yellow);
		}

		// Token: 0x04000078 RID: 120
		private IEntityManager _entManager;

		// Token: 0x04000079 RID: 121
		private readonly IEyeManager _eye;

		// Token: 0x0400007A RID: 122
		private readonly IGameTiming _timing;

		// Token: 0x0400007B RID: 123
		private readonly IInputManager _input;

		// Token: 0x0400007C RID: 124
		private readonly IPlayerManager _player;

		// Token: 0x0400007D RID: 125
		private readonly GunSystem _guns;
	}
}
