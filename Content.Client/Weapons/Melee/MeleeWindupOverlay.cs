using System;
using System.Runtime.CompilerServices;
using Content.Shared.Weapons.Melee;
using Robust.Client.GameObjects;
using Robust.Client.Graphics;
using Robust.Client.Player;
using Robust.Shared.Enums;
using Robust.Shared.GameObjects;
using Robust.Shared.Maths;
using Robust.Shared.Prototypes;
using Robust.Shared.Timing;
using Robust.Shared.Utility;

namespace Content.Client.Weapons.Melee
{
	// Token: 0x0200003E RID: 62
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class MeleeWindupOverlay : Overlay
	{
		// Token: 0x17000020 RID: 32
		// (get) Token: 0x06000118 RID: 280 RVA: 0x00005516 File Offset: 0x00003716
		public override OverlaySpace Space
		{
			get
			{
				return 8;
			}
		}

		// Token: 0x06000119 RID: 281 RVA: 0x00009ECC File Offset: 0x000080CC
		public MeleeWindupOverlay(IEntityManager entManager, IGameTiming timing, IPlayerManager playerManager, IPrototypeManager protoManager)
		{
			this._entManager = entManager;
			this._timing = timing;
			this._player = playerManager;
			this._melee = this._entManager.EntitySysManager.GetEntitySystem<SharedMeleeWeaponSystem>();
			this._transform = this._entManager.EntitySysManager.GetEntitySystem<SharedTransformSystem>();
			SpriteSpecifier.Rsi rsi = new SpriteSpecifier.Rsi(new ResourcePath("/Textures/Interface/Misc/progress_bar.rsi", "/"), "icon");
			this._texture = this._entManager.EntitySysManager.GetEntitySystem<SpriteSystem>().Frame0(rsi);
			this._shader = protoManager.Index<ShaderPrototype>("unshaded").Instance();
		}

		// Token: 0x0600011A RID: 282 RVA: 0x00009F70 File Offset: 0x00008170
		protected override void Draw(in OverlayDrawArgs args)
		{
			LocalPlayer localPlayer = this._player.LocalPlayer;
			EntityUid? entityUid = (localPlayer != null) ? localPlayer.ControlledEntity : null;
			TransformComponent transformComponent;
			if (!this._entManager.TryGetComponent<TransformComponent>(entityUid, ref transformComponent) || transformComponent.MapID != args.MapId)
			{
				return;
			}
			MeleeWeaponComponent weapon = this._melee.GetWeapon(entityUid.Value);
			if (weapon == null)
			{
				return;
			}
			DrawingHandleWorld worldHandle = args.WorldHandle;
			IEye eye = args.Viewport.Eye;
			Angle angle = (eye != null) ? eye.Rotation : Angle.Zero;
			EntityQuery<SpriteComponent> entityQuery = this._entManager.GetEntityQuery<SpriteComponent>();
			EntityQuery<TransformComponent> entityQuery2 = this._entManager.GetEntityQuery<TransformComponent>();
			Vector2 vector = new Vector2(1f, 1f);
			Matrix3 matrix = Matrix3.CreateScale(ref vector);
			Matrix3 matrix2 = Matrix3.CreateRotation(-angle);
			worldHandle.UseShader(this._shader);
			TimeSpan curTime = this._timing.CurTime;
			if (weapon.WindUpStart == null || weapon.Attacking)
			{
				return;
			}
			TransformComponent transformComponent2;
			if (!entityQuery2.TryGetComponent(weapon.Owner, ref transformComponent2) || transformComponent2.MapID != args.MapId)
			{
				return;
			}
			Matrix3 matrix3 = Matrix3.CreateTranslation(this._transform.GetWorldPosition(transformComponent2));
			Matrix3 matrix4;
			Matrix3.Multiply(ref matrix, ref matrix3, ref matrix4);
			Matrix3 matrix5;
			Matrix3.Multiply(ref matrix2, ref matrix4, ref matrix5);
			worldHandle.SetTransform(ref matrix5);
			float num = (float)(-(float)this._texture.Height) / 1f;
			SpriteComponent spriteComponent;
			float num2;
			if (entityQuery.TryGetComponent(weapon.Owner, ref spriteComponent))
			{
				num2 = -spriteComponent.Bounds.Height / 2f - 0.05f;
			}
			else
			{
				num2 = -0.5f;
			}
			Vector2 vector2;
			vector2..ctor((float)(-(float)this._texture.Width) / 2f / 32f, num2 / 1f + num / 32f * 1f);
			worldHandle.DrawTexture(this._texture, vector2, null);
			float num3 = 0.1f / (float)weapon.WindupTime.TotalSeconds * 32f;
			float num4 = 12f;
			Box2 box;
			box..ctor(new Vector2(num4 - num3 / 2f, 3f) / 32f, new Vector2(num4 + num3 / 2f, 4f) / 32f);
			box = box.Translated(vector2);
			worldHandle.DrawRect(box, Color.LimeGreen, true);
			double num5 = weapon.WindupTime.TotalSeconds * 2.0;
			double num6 = (curTime - weapon.WindUpStart.Value).TotalSeconds % (2.0 * num5) / num5;
			if (num6 > 1.0)
			{
				num6 = 2.0 - num6;
			}
			float num7 = (float)num6;
			float num8 = 20f * num7 + 2f + 1f;
			Box2 box2;
			box2..ctor(new Vector2(Math.Max(2f, num8 - 2f), 3f) / 32f, new Vector2(Math.Min(22f, num8), 4f) / 32f);
			box2 = box2.Translated(vector2);
			worldHandle.DrawRect(box2, Color.White, true);
			worldHandle.UseShader(null);
			worldHandle.SetTransform(ref Matrix3.Identity);
		}

		// Token: 0x040000C0 RID: 192
		private readonly IEntityManager _entManager;

		// Token: 0x040000C1 RID: 193
		private readonly IGameTiming _timing;

		// Token: 0x040000C2 RID: 194
		private readonly IPlayerManager _player;

		// Token: 0x040000C3 RID: 195
		private readonly SharedMeleeWeaponSystem _melee;

		// Token: 0x040000C4 RID: 196
		private readonly SharedTransformSystem _transform;

		// Token: 0x040000C5 RID: 197
		private readonly Texture _texture;

		// Token: 0x040000C6 RID: 198
		private readonly ShaderInstance _shader;
	}
}
