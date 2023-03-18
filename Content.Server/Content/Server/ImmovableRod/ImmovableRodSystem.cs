using System;
using System.Runtime.CompilerServices;
using Content.Server.Body.Systems;
using Content.Server.Popups;
using Content.Shared.Body.Components;
using Content.Shared.Examine;
using Content.Shared.Popups;
using Robust.Shared.Audio;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Localization;
using Robust.Shared.Map;
using Robust.Shared.Map.Components;
using Robust.Shared.Maths;
using Robust.Shared.Physics.Components;
using Robust.Shared.Physics.Events;
using Robust.Shared.Physics.Systems;
using Robust.Shared.Player;
using Robust.Shared.Random;

namespace Content.Server.ImmovableRod
{
	// Token: 0x02000454 RID: 1108
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class ImmovableRodSystem : EntitySystem
	{
		// Token: 0x0600165A RID: 5722 RVA: 0x00075EEC File Offset: 0x000740EC
		public override void Update(float frameTime)
		{
			base.Update(frameTime);
			foreach (ValueTuple<ImmovableRodComponent, TransformComponent> valueTuple in this.EntityManager.EntityQuery<ImmovableRodComponent, TransformComponent>(true))
			{
				ImmovableRodComponent rod = valueTuple.Item1;
				TransformComponent trans = valueTuple.Item2;
				MapGridComponent grid;
				if (rod.DestroyTiles && this._map.TryGetGrid(trans.GridUid, ref grid))
				{
					grid.SetTile(trans.Coordinates, Tile.Empty);
				}
			}
		}

		// Token: 0x0600165B RID: 5723 RVA: 0x00075F7C File Offset: 0x0007417C
		public override void Initialize()
		{
			base.Initialize();
			base.SubscribeLocalEvent<ImmovableRodComponent, StartCollideEvent>(new ComponentEventRefHandler<ImmovableRodComponent, StartCollideEvent>(this.OnCollide), null, null);
			base.SubscribeLocalEvent<ImmovableRodComponent, ComponentInit>(new ComponentEventHandler<ImmovableRodComponent, ComponentInit>(this.OnComponentInit), null, null);
			base.SubscribeLocalEvent<ImmovableRodComponent, ExaminedEvent>(new ComponentEventHandler<ImmovableRodComponent, ExaminedEvent>(this.OnExamined), null, null);
		}

		// Token: 0x0600165C RID: 5724 RVA: 0x00075FCC File Offset: 0x000741CC
		private void OnComponentInit(EntityUid uid, ImmovableRodComponent component, ComponentInit args)
		{
			PhysicsComponent phys;
			if (this.EntityManager.TryGetComponent<PhysicsComponent>(uid, ref phys))
			{
				this._physics.SetLinearDamping(phys, 0f, true);
				this._physics.SetFriction(phys, 0f, true);
				this._physics.SetBodyStatus(phys, 1, true);
				if (!component.RandomizeVelocity)
				{
					return;
				}
				TransformComponent xform = base.Transform(uid);
				Vector2 vector;
				if (component.DirectionOverride.Degrees == 0.0)
				{
					vector = this._random.NextVector2(component.MinSpeed, component.MaxSpeed);
				}
				else
				{
					Angle worldRotation = xform.WorldRotation;
					Vector2 vector2 = component.DirectionOverride.ToVec();
					vector = worldRotation.RotateVec(ref vector2) * this._random.NextFloat(component.MinSpeed, component.MaxSpeed);
				}
				Vector2 vel = vector;
				this._physics.ApplyLinearImpulse(uid, vel, null, phys);
				xform.LocalRotation = DirectionExtensions.ToWorldAngle(vel - xform.WorldPosition) + 1.5707964f;
			}
		}

		// Token: 0x0600165D RID: 5725 RVA: 0x000760D4 File Offset: 0x000742D4
		private void OnCollide(EntityUid uid, ImmovableRodComponent component, ref StartCollideEvent args)
		{
			EntityUid ent = args.OtherFixture.Body.Owner;
			if (RandomExtensions.Prob(this._random, component.HitSoundProbability))
			{
				SoundSystem.Play(component.Sound.GetSound(null, null), Filter.Pvs(uid, 2f, null, null, null), uid, new AudioParams?(component.Sound.Params));
			}
			if (base.HasComp<ImmovableRodComponent>(ent))
			{
				EntityCoordinates coords = base.Transform(uid).Coordinates;
				this._popup.PopupCoordinates(Loc.GetString("immovable-rod-collided-rod-not-good"), coords, PopupType.LargeCaution);
				base.Del(uid);
				base.Del(ent);
				base.Spawn("Singularity", coords);
				return;
			}
			BodyComponent body;
			if (base.TryComp<BodyComponent>(ent, ref body))
			{
				component.MobCount++;
				this._popup.PopupEntity(Loc.GetString("immovable-rod-penetrated-mob", new ValueTuple<string, object>[]
				{
					new ValueTuple<string, object>("rod", uid),
					new ValueTuple<string, object>("mob", ent)
				}), uid, PopupType.LargeCaution);
				this._bodySystem.GibBody(new EntityUid?(ent), false, body, false);
			}
			base.QueueDel(ent);
		}

		// Token: 0x0600165E RID: 5726 RVA: 0x00076200 File Offset: 0x00074400
		private void OnExamined(EntityUid uid, ImmovableRodComponent component, ExaminedEvent args)
		{
			if (component.MobCount == 0)
			{
				args.PushText(Loc.GetString("immovable-rod-consumed-none", new ValueTuple<string, object>[]
				{
					new ValueTuple<string, object>("rod", uid)
				}));
				return;
			}
			args.PushText(Loc.GetString("immovable-rod-consumed-souls", new ValueTuple<string, object>[]
			{
				new ValueTuple<string, object>("rod", uid),
				new ValueTuple<string, object>("amount", component.MobCount)
			}));
		}

		// Token: 0x04000DFE RID: 3582
		[Dependency]
		private readonly IRobustRandom _random;

		// Token: 0x04000DFF RID: 3583
		[Dependency]
		private readonly IMapManager _map;

		// Token: 0x04000E00 RID: 3584
		[Dependency]
		private readonly BodySystem _bodySystem;

		// Token: 0x04000E01 RID: 3585
		[Dependency]
		private readonly PopupSystem _popup;

		// Token: 0x04000E02 RID: 3586
		[Dependency]
		private readonly SharedPhysicsSystem _physics;
	}
}
