using System;
using System.Runtime.CompilerServices;
using Content.Shared.Damage;
using Content.Shared.FixedPoint;
using Content.Shared.Mobs;
using Content.Shared.Mobs.Components;
using Content.Shared.Mobs.Systems;
using Robust.Client.GameObjects;
using Robust.Client.Graphics;
using Robust.Shared.Enums;
using Robust.Shared.GameObjects;
using Robust.Shared.Maths;
using Robust.Shared.Prototypes;
using Robust.Shared.Utility;

namespace Content.Client.EntityHealthBar
{
	// Token: 0x02000026 RID: 38
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class EntityHealthBarOverlay : Overlay
	{
		// Token: 0x17000014 RID: 20
		// (get) Token: 0x06000096 RID: 150 RVA: 0x00005516 File Offset: 0x00003716
		public override OverlaySpace Space
		{
			get
			{
				return 8;
			}
		}

		// Token: 0x06000097 RID: 151 RVA: 0x0000551C File Offset: 0x0000371C
		public EntityHealthBarOverlay(IEntityManager entManager, IPrototypeManager protoManager)
		{
			this._entManager = entManager;
			this._transform = this._entManager.EntitySysManager.GetEntitySystem<SharedTransformSystem>();
			this._mobStateSystem = this._entManager.EntitySysManager.GetEntitySystem<MobStateSystem>();
			this._mobThresholdSystem = this._entManager.EntitySysManager.GetEntitySystem<MobThresholdSystem>();
			SpriteSpecifier.Rsi rsi = new SpriteSpecifier.Rsi(new ResourcePath("/Textures/Interface/Misc/health_bar.rsi", "/"), "icon");
			this._barTexture = this._entManager.EntitySysManager.GetEntitySystem<SpriteSystem>().Frame0(rsi);
			this._shader = protoManager.Index<ShaderPrototype>("shaded").Instance();
		}

		// Token: 0x06000098 RID: 152 RVA: 0x000055C4 File Offset: 0x000037C4
		protected override void Draw(in OverlayDrawArgs args)
		{
			DrawingHandleWorld worldHandle = args.WorldHandle;
			IEye eye = args.Viewport.Eye;
			Angle angle = (eye != null) ? eye.Rotation : Angle.Zero;
			EntityQuery<SpriteComponent> entityQuery = this._entManager.GetEntityQuery<SpriteComponent>();
			EntityQuery<TransformComponent> entityQuery2 = this._entManager.GetEntityQuery<TransformComponent>();
			Vector2 vector = new Vector2(1f, 1f);
			Matrix3 matrix = Matrix3.CreateScale(ref vector);
			Matrix3 matrix2 = Matrix3.CreateRotation(-angle);
			worldHandle.UseShader(this._shader);
			foreach (ValueTuple<MobStateComponent, DamageableComponent> valueTuple in this._entManager.EntityQuery<MobStateComponent, DamageableComponent>(true))
			{
				MobStateComponent item = valueTuple.Item1;
				DamageableComponent item2 = valueTuple.Item2;
				TransformComponent transformComponent;
				if (entityQuery2.TryGetComponent(item.Owner, ref transformComponent) && !(transformComponent.MapID != args.MapId) && (this.DamageContainer == null || !(item2.DamageContainerID != this.DamageContainer)))
				{
					Matrix3 matrix3 = Matrix3.CreateTranslation(this._transform.GetWorldPosition(transformComponent));
					Matrix3 matrix4;
					Matrix3.Multiply(ref matrix, ref matrix3, ref matrix4);
					Matrix3 matrix5;
					Matrix3.Multiply(ref matrix2, ref matrix4, ref matrix5);
					worldHandle.SetTransform(ref matrix5);
					SpriteComponent spriteComponent;
					float num;
					if (entityQuery.TryGetComponent(item.Owner, ref spriteComponent))
					{
						num = spriteComponent.Bounds.Height + 15f;
					}
					else
					{
						num = 1f;
					}
					Vector2 vector2;
					vector2..ctor((float)(-(float)this._barTexture.Width) / 2f / 32f, num / 32f);
					worldHandle.DrawTexture(this._barTexture, vector2, null);
					ValueTuple<float, bool> valueTuple2 = this.CalcProgress(item.Owner, item, item2);
					Color progressColor = EntityHealthBarOverlay.GetProgressColor(valueTuple2.Item1, valueTuple2.Item2);
					float num2 = 20f * valueTuple2.Item1 + 2f;
					Box2 box;
					box..ctor(new Vector2(2f, 3f) / 32f, new Vector2(num2, 4f) / 32f);
					box = box.Translated(vector2);
					worldHandle.DrawRect(box, progressColor, true);
				}
			}
			worldHandle.UseShader(null);
			worldHandle.SetTransform(ref Matrix3.Identity);
		}

		// Token: 0x06000099 RID: 153 RVA: 0x0000582C File Offset: 0x00003A2C
		[return: Nullable(0)]
		private ValueTuple<float, bool> CalcProgress(EntityUid uid, MobStateComponent component, DamageableComponent dmg)
		{
			if (this._mobStateSystem.IsAlive(uid, component))
			{
				FixedPoint2? fixedPoint;
				if (!this._mobThresholdSystem.TryGetThresholdForState(uid, MobState.Critical, out fixedPoint, null))
				{
					return new ValueTuple<float, bool>(1f, false);
				}
				return new ValueTuple<float, bool>(1f - (dmg.TotalDamage / fixedPoint).Value.Float(), false);
			}
			else
			{
				if (!this._mobStateSystem.IsCritical(uid, component))
				{
					return new ValueTuple<float, bool>(0f, true);
				}
				FixedPoint2? fixedPoint2;
				FixedPoint2? fixedPoint3;
				if (!this._mobThresholdSystem.TryGetThresholdForState(uid, MobState.Critical, out fixedPoint2, null) || !this._mobThresholdSystem.TryGetThresholdForState(uid, MobState.Dead, out fixedPoint3, null))
				{
					return new ValueTuple<float, bool>(1f, true);
				}
				return new ValueTuple<float, bool>(1f - ((dmg.TotalDamage - fixedPoint2) / (fixedPoint3 - fixedPoint2)).Value.Float(), true);
			}
		}

		// Token: 0x0600009A RID: 154 RVA: 0x000059C4 File Offset: 0x00003BC4
		public static Color GetProgressColor(float progress, bool crit)
		{
			if (progress >= 1f)
			{
				return new Color(0f, 1f, 0f, 1f);
			}
			if (!crit)
			{
				return Color.FromHsv(new ValueTuple<float, float, float, float>(0.2777778f * progress, 1f, 0.75f, 1f));
			}
			return Color.Red;
		}

		// Token: 0x04000054 RID: 84
		private readonly IEntityManager _entManager;

		// Token: 0x04000055 RID: 85
		private readonly SharedTransformSystem _transform;

		// Token: 0x04000056 RID: 86
		private readonly MobStateSystem _mobStateSystem;

		// Token: 0x04000057 RID: 87
		private readonly MobThresholdSystem _mobThresholdSystem;

		// Token: 0x04000058 RID: 88
		private readonly Texture _barTexture;

		// Token: 0x04000059 RID: 89
		private readonly ShaderInstance _shader;

		// Token: 0x0400005A RID: 90
		[Nullable(2)]
		public string DamageContainer;
	}
}
