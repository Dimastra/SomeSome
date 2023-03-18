using System;
using System.Runtime.CompilerServices;
using Content.Shared.Clothing.Components;
using Content.Shared.Clothing.EntitySystems;
using Content.Shared.Movement.Components;
using Content.Shared.Movement.Systems;
using Robust.Client.GameObjects;
using Robust.Shared.Containers;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Map;
using Robust.Shared.Map.Components;
using Robust.Shared.Physics.Components;
using Robust.Shared.Timing;

namespace Content.Client.Movement.Systems
{
	// Token: 0x0200022A RID: 554
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class JetpackSystem : SharedJetpackSystem
	{
		// Token: 0x06000E57 RID: 3671 RVA: 0x00056B0C File Offset: 0x00054D0C
		public override void Initialize()
		{
			base.Initialize();
			base.SubscribeLocalEvent<JetpackComponent, AppearanceChangeEvent>(new ComponentEventRefHandler<JetpackComponent, AppearanceChangeEvent>(this.OnJetpackAppearance), null, null);
		}

		// Token: 0x06000E58 RID: 3672 RVA: 0x00003C59 File Offset: 0x00001E59
		protected override bool CanEnable(JetpackComponent component)
		{
			return false;
		}

		// Token: 0x06000E59 RID: 3673 RVA: 0x00056B28 File Offset: 0x00054D28
		private void OnJetpackAppearance(EntityUid uid, JetpackComponent component, ref AppearanceChangeEvent args)
		{
			bool flag;
			this._appearance.TryGetData<bool>(uid, JetpackVisuals.Enabled, ref flag, args.Component);
			string text = "icon" + (flag ? "-on" : "");
			SpriteComponent sprite = args.Sprite;
			if (sprite != null)
			{
				sprite.LayerSetState(0, text);
			}
			ClothingComponent clothing;
			if (base.TryComp<ClothingComponent>(uid, ref clothing))
			{
				this._clothing.SetEquippedPrefix(uid, flag ? "on" : null, clothing);
			}
		}

		// Token: 0x06000E5A RID: 3674 RVA: 0x00056BA8 File Offset: 0x00054DA8
		public override void Update(float frameTime)
		{
			base.Update(frameTime);
			if (!this._timing.IsFirstTimePredicted)
			{
				return;
			}
			foreach (ActiveJetpackComponent activeJetpackComponent in base.EntityQuery<ActiveJetpackComponent>(false))
			{
				if (!(this._timing.CurTime < activeJetpackComponent.TargetTime))
				{
					activeJetpackComponent.TargetTime = this._timing.CurTime + TimeSpan.FromSeconds((double)activeJetpackComponent.EffectCooldown);
					this.CreateParticles(activeJetpackComponent.Owner);
				}
			}
		}

		// Token: 0x06000E5B RID: 3675 RVA: 0x00056C4C File Offset: 0x00054E4C
		private void CreateParticles(EntityUid uid)
		{
			IContainer container;
			PhysicsComponent physicsComponent;
			if (this.Container.TryGetContainingContainer(uid, ref container, null, null) && base.TryComp<PhysicsComponent>(container.Owner, ref physicsComponent) && physicsComponent.LinearVelocity.LengthSquared < 1f)
			{
				return;
			}
			TransformComponent transformComponent = base.Transform(uid);
			EntityCoordinates coordinates = transformComponent.Coordinates;
			EntityUid? gridUid = coordinates.GetGridUid(this.EntityManager);
			MapGridComponent mapGridComponent;
			if (this._mapManager.TryGetGrid(gridUid, ref mapGridComponent))
			{
				coordinates..ctor(mapGridComponent.Owner, mapGridComponent.WorldToLocal(coordinates.ToMapPos(this.EntityManager)));
			}
			else
			{
				if (transformComponent.MapUid == null)
				{
					return;
				}
				coordinates..ctor(transformComponent.MapUid.Value, transformComponent.WorldPosition);
			}
			EntityUid entityUid = base.Spawn("JetpackEffect", coordinates);
			base.Transform(entityUid).Coordinates = coordinates;
		}

		// Token: 0x04000716 RID: 1814
		[Dependency]
		private readonly IGameTiming _timing;

		// Token: 0x04000717 RID: 1815
		[Dependency]
		private readonly IMapManager _mapManager;

		// Token: 0x04000718 RID: 1816
		[Dependency]
		private readonly ClothingSystem _clothing;

		// Token: 0x04000719 RID: 1817
		[Dependency]
		private readonly SharedAppearanceSystem _appearance;
	}
}
