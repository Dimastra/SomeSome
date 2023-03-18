using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading;
using Content.Server.Body.Components;
using Content.Server.Body.Systems;
using Content.Server.Coordinates.Helpers;
using Content.Server.DoAfter;
using Content.Server.Doors.Systems;
using Content.Server.Magic.Events;
using Content.Server.Weapons.Ranged.Systems;
using Content.Shared.Actions;
using Content.Shared.Actions.ActionTypes;
using Content.Shared.Body.Components;
using Content.Shared.DoAfter;
using Content.Shared.Doors.Components;
using Content.Shared.Doors.Systems;
using Content.Shared.Interaction.Events;
using Content.Shared.Maps;
using Content.Shared.Physics;
using Content.Shared.Spawners.Components;
using Content.Shared.Storage;
using Robust.Server.GameObjects;
using Robust.Shared.Audio;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Map;
using Robust.Shared.Map.Components;
using Robust.Shared.Maths;
using Robust.Shared.Prototypes;
using Robust.Shared.Random;
using Robust.Shared.Serialization.Manager;

namespace Content.Server.Magic
{
	// Token: 0x020003E3 RID: 995
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class MagicSystem : EntitySystem
	{
		// Token: 0x06001473 RID: 5235 RVA: 0x0006A004 File Offset: 0x00068204
		public override void Initialize()
		{
			base.Initialize();
			base.SubscribeLocalEvent<SpellbookComponent, ComponentInit>(new ComponentEventHandler<SpellbookComponent, ComponentInit>(this.OnInit), null, null);
			base.SubscribeLocalEvent<SpellbookComponent, UseInHandEvent>(new ComponentEventHandler<SpellbookComponent, UseInHandEvent>(this.OnUse), null, null);
			base.SubscribeLocalEvent<SpellbookComponent, DoAfterEvent>(new ComponentEventHandler<SpellbookComponent, DoAfterEvent>(this.OnDoAfter), null, null);
			base.SubscribeLocalEvent<InstantSpawnSpellEvent>(new EntityEventHandler<InstantSpawnSpellEvent>(this.OnInstantSpawn), null, null);
			base.SubscribeLocalEvent<TeleportSpellEvent>(new EntityEventHandler<TeleportSpellEvent>(this.OnTeleportSpell), null, null);
			base.SubscribeLocalEvent<KnockSpellEvent>(new EntityEventHandler<KnockSpellEvent>(this.OnKnockSpell), null, null);
			base.SubscribeLocalEvent<SmiteSpellEvent>(new EntityEventHandler<SmiteSpellEvent>(this.OnSmiteSpell), null, null);
			base.SubscribeLocalEvent<WorldSpawnSpellEvent>(new EntityEventHandler<WorldSpawnSpellEvent>(this.OnWorldSpawn), null, null);
			base.SubscribeLocalEvent<ProjectileSpellEvent>(new EntityEventHandler<ProjectileSpellEvent>(this.OnProjectileSpell), null, null);
			base.SubscribeLocalEvent<ChangeComponentsSpellEvent>(new EntityEventHandler<ChangeComponentsSpellEvent>(this.OnChangeComponentsSpell), null, null);
		}

		// Token: 0x06001474 RID: 5236 RVA: 0x0006A0DF File Offset: 0x000682DF
		private void OnDoAfter(EntityUid uid, SpellbookComponent component, DoAfterEvent args)
		{
			if (args.Handled || args.Cancelled)
			{
				return;
			}
			this._actionsSystem.AddActions(args.Args.User, component.Spells, new EntityUid?(uid), null, true);
			args.Handled = true;
		}

		// Token: 0x06001475 RID: 5237 RVA: 0x0006A120 File Offset: 0x00068320
		private void OnInit(EntityUid uid, SpellbookComponent component, ComponentInit args)
		{
			foreach (KeyValuePair<string, int> keyValuePair in component.WorldSpells)
			{
				string text;
				int num;
				keyValuePair.Deconstruct(out text, out num);
				string id = text;
				int charges = num;
				WorldTargetAction spell = new WorldTargetAction(this._prototypeManager.Index<WorldTargetActionPrototype>(id));
				this._actionsSystem.SetCharges(spell, (charges < 0) ? null : new int?(charges));
				component.Spells.Add(spell);
			}
			foreach (KeyValuePair<string, int> keyValuePair in component.InstantSpells)
			{
				string text;
				int num;
				keyValuePair.Deconstruct(out text, out num);
				string id2 = text;
				int charges2 = num;
				InstantAction spell2 = new InstantAction(this._prototypeManager.Index<InstantActionPrototype>(id2));
				this._actionsSystem.SetCharges(spell2, (charges2 < 0) ? null : new int?(charges2));
				component.Spells.Add(spell2);
			}
			foreach (KeyValuePair<string, int> keyValuePair in component.EntitySpells)
			{
				string text;
				int num;
				keyValuePair.Deconstruct(out text, out num);
				string id3 = text;
				int charges3 = num;
				EntityTargetAction spell3 = new EntityTargetAction(this._prototypeManager.Index<EntityTargetActionPrototype>(id3));
				this._actionsSystem.SetCharges(spell3, (charges3 < 0) ? null : new int?(charges3));
				component.Spells.Add(spell3);
			}
		}

		// Token: 0x06001476 RID: 5238 RVA: 0x0006A2E8 File Offset: 0x000684E8
		private void OnUse(EntityUid uid, SpellbookComponent component, UseInHandEvent args)
		{
			if (args.Handled)
			{
				return;
			}
			this.AttemptLearn(uid, component, args);
			args.Handled = true;
		}

		// Token: 0x06001477 RID: 5239 RVA: 0x0006A304 File Offset: 0x00068504
		private void AttemptLearn(EntityUid uid, SpellbookComponent component, UseInHandEvent args)
		{
			EntityUid user = args.User;
			float learnTime = component.LearnTime;
			EntityUid? target = new EntityUid?(uid);
			DoAfterEventArgs doAfterEventArgs = new DoAfterEventArgs(user, learnTime, default(CancellationToken), target, null)
			{
				BreakOnTargetMove = true,
				BreakOnUserMove = true,
				BreakOnDamage = true,
				BreakOnStun = true,
				NeedHand = true
			};
			this._doAfter.DoAfter(doAfterEventArgs);
		}

		// Token: 0x06001478 RID: 5240 RVA: 0x0006A370 File Offset: 0x00068570
		private void OnInstantSpawn(InstantSpawnSpellEvent args)
		{
			if (args.Handled)
			{
				return;
			}
			TransformComponent transform = base.Transform(args.Performer);
			foreach (EntityCoordinates position in this.GetSpawnPositions(transform, args.Pos))
			{
				EntityUid ent = base.Spawn(args.Prototype, position.SnapToGrid(this.EntityManager, this._mapManager));
				if (args.PreventCollideWithCaster)
				{
					base.EnsureComp<PreventCollideComponent>(ent).Uid = args.Performer;
				}
			}
			args.Handled = true;
		}

		// Token: 0x06001479 RID: 5241 RVA: 0x0006A41C File Offset: 0x0006861C
		private void OnProjectileSpell(ProjectileSpellEvent ev)
		{
			if (ev.Handled)
			{
				return;
			}
			TransformComponent xform = base.Transform(ev.Performer);
			Vector2 userVelocity = this._physics.GetMapLinearVelocity(ev.Performer, null, null, null, null);
			foreach (EntityCoordinates pos in this.GetSpawnPositions(xform, ev.Pos))
			{
				MapCoordinates mapPos = pos.ToMap(this.EntityManager);
				MapGridComponent grid;
				EntityCoordinates spawnCoords = this._mapManager.TryFindGridAt(mapPos, ref grid) ? pos.WithEntityId(grid.Owner, this.EntityManager) : new EntityCoordinates(this._mapManager.GetMapEntityId(mapPos.MapId), mapPos.Position);
				EntityUid ent = base.Spawn(ev.Prototype, spawnCoords);
				this._gunSystem.ShootProjectile(ent, ev.Target.Position - mapPos.Position, userVelocity, new EntityUid?(ev.Performer), 20f);
			}
		}

		// Token: 0x0600147A RID: 5242 RVA: 0x0006A550 File Offset: 0x00068750
		private void OnChangeComponentsSpell(ChangeComponentsSpellEvent ev)
		{
			foreach (string toRemove in ev.ToRemove)
			{
				ComponentRegistration registration;
				if (this._compFact.TryGetRegistration(toRemove, ref registration, false))
				{
					base.RemComp(ev.Target, registration.Type);
				}
			}
			foreach (KeyValuePair<string, EntityPrototype.ComponentRegistryEntry> keyValuePair in ev.ToAdd)
			{
				string text;
				EntityPrototype.ComponentRegistryEntry componentRegistryEntry;
				keyValuePair.Deconstruct(out text, out componentRegistryEntry);
				string name = text;
				EntityPrototype.ComponentRegistryEntry data = componentRegistryEntry;
				if (!base.HasComp(ev.Target, data.Component.GetType()))
				{
					Component component = (Component)this._compFact.GetComponent(name, false);
					component.Owner = ev.Target;
					object temp = component;
					this._seriMan.CopyTo(data.Component, ref temp, null, false, false);
					this.EntityManager.AddComponent<Component>(ev.Target, (Component)temp, false);
				}
			}
		}

		// Token: 0x0600147B RID: 5243 RVA: 0x0006A680 File Offset: 0x00068880
		private List<EntityCoordinates> GetSpawnPositions(TransformComponent casterXform, MagicSpawnData data)
		{
			if (data is TargetCasterPos)
			{
				return new List<EntityCoordinates>(1)
				{
					casterXform.Coordinates
				};
			}
			if (!(data is TargetInFront))
			{
				throw new ArgumentOutOfRangeException();
			}
			EntityCoordinates directionPos = casterXform.Coordinates.Offset(casterXform.LocalRotation.ToWorldVec().Normalized);
			MapGridComponent mapGrid;
			if (!this._mapManager.TryGetGrid(casterXform.GridUid, ref mapGrid))
			{
				return new List<EntityCoordinates>();
			}
			TileRef? tileReference;
			if (!directionPos.TryGetTileRef(out tileReference, this.EntityManager, this._mapManager))
			{
				return new List<EntityCoordinates>();
			}
			Vector2i tileIndex = tileReference.Value.GridIndices;
			EntityCoordinates coords = mapGrid.GridTileToLocal(tileIndex);
			switch (casterXform.LocalRotation.GetCardinalDir())
			{
			case 0:
			case 4:
			{
				EntityCoordinates coordsPlus = mapGrid.GridTileToLocal(tileIndex + new ValueTuple<int, int>(1, 0));
				EntityCoordinates coordsMinus = mapGrid.GridTileToLocal(tileIndex + new ValueTuple<int, int>(-1, 0));
				return new List<EntityCoordinates>(3)
				{
					coords,
					coordsPlus,
					coordsMinus
				};
			}
			case 2:
			case 6:
			{
				EntityCoordinates coordsPlus = mapGrid.GridTileToLocal(tileIndex + new ValueTuple<int, int>(0, 1));
				EntityCoordinates coordsMinus = mapGrid.GridTileToLocal(tileIndex + new ValueTuple<int, int>(0, -1));
				return new List<EntityCoordinates>(3)
				{
					coords,
					coordsPlus,
					coordsMinus
				};
			}
			}
			return new List<EntityCoordinates>();
		}

		// Token: 0x0600147C RID: 5244 RVA: 0x0006A814 File Offset: 0x00068A14
		private void OnTeleportSpell(TeleportSpellEvent args)
		{
			if (args.Handled)
			{
				return;
			}
			TransformComponent transform = base.Transform(args.Performer);
			if (transform.MapID != args.Target.GetMapId(this.EntityManager))
			{
				return;
			}
			this._transformSystem.SetCoordinates(args.Performer, args.Target);
			transform.AttachToGridOrMap();
			this._audio.PlayPvs(args.BlinkSound, args.Performer, new AudioParams?(AudioParams.Default.WithVolume(args.BlinkVolume)));
			args.Handled = true;
		}

		// Token: 0x0600147D RID: 5245 RVA: 0x0006A8A8 File Offset: 0x00068AA8
		private void OnKnockSpell(KnockSpellEvent args)
		{
			if (args.Handled)
			{
				return;
			}
			EntityCoordinates coords = base.Transform(args.Performer).Coordinates;
			this._audio.PlayPvs(args.KnockSound, args.Performer, new AudioParams?(AudioParams.Default.WithVolume(args.KnockVolume)));
			foreach (EntityUid entity in this._lookup.GetEntitiesInRange(coords, args.Range, 46))
			{
				AirlockComponent airlock;
				if (base.TryComp<AirlockComponent>(entity, ref airlock))
				{
					this._airlock.SetBoltsDown(entity, airlock, false);
				}
				DoorComponent doorComp;
				if (base.TryComp<DoorComponent>(entity, ref doorComp) && doorComp.State != DoorState.Open)
				{
					this._doorSystem.StartOpening(doorComp.Owner, null, null, false);
				}
			}
			args.Handled = true;
		}

		// Token: 0x0600147E RID: 5246 RVA: 0x0006A9A0 File Offset: 0x00068BA0
		private void OnSmiteSpell(SmiteSpellEvent ev)
		{
			if (ev.Handled)
			{
				return;
			}
			Vector2 impulseVector = (base.Transform(ev.Target).MapPosition.Position - base.Transform(ev.Performer).MapPosition.Position) * 10000f;
			this._physics.ApplyLinearImpulse(ev.Target, impulseVector, null, null);
			BodyComponent body;
			if (!base.TryComp<BodyComponent>(ev.Target, ref body))
			{
				return;
			}
			HashSet<EntityUid> ents = this._bodySystem.GibBody(new EntityUid?(ev.Target), true, body, false);
			if (!ev.DeleteNonBrainParts)
			{
				return;
			}
			foreach (EntityUid part in ents)
			{
				if (base.HasComp<BodyComponent>(part) && !base.HasComp<BrainComponent>(part))
				{
					base.QueueDel(part);
				}
			}
		}

		// Token: 0x0600147F RID: 5247 RVA: 0x0006AA94 File Offset: 0x00068C94
		private void OnWorldSpawn(WorldSpawnSpellEvent args)
		{
			if (args.Handled)
			{
				return;
			}
			EntityCoordinates targetMapCoords = args.Target;
			this.SpawnSpellHelper(args.Contents, targetMapCoords, args.Lifetime, args.Offset);
			args.Handled = true;
		}

		// Token: 0x06001480 RID: 5248 RVA: 0x0006AAD4 File Offset: 0x00068CD4
		private void SpawnSpellHelper(List<EntitySpawnEntry> entityEntries, EntityCoordinates entityCoords, float? lifetime, Vector2 offsetVector2)
		{
			List<string> spawns = EntitySpawnCollection.GetSpawns(entityEntries, this._random);
			EntityCoordinates offsetCoords = entityCoords;
			foreach (string proto in spawns)
			{
				EntityUid entity = base.Spawn(proto, offsetCoords);
				offsetCoords = offsetCoords.Offset(offsetVector2);
				if (lifetime != null)
				{
					base.EnsureComp<TimedDespawnComponent>(entity).Lifetime = lifetime.Value;
				}
			}
		}

		// Token: 0x04000CA0 RID: 3232
		[Dependency]
		private readonly ISerializationManager _seriMan;

		// Token: 0x04000CA1 RID: 3233
		[Dependency]
		private readonly IComponentFactory _compFact;

		// Token: 0x04000CA2 RID: 3234
		[Dependency]
		private readonly IMapManager _mapManager;

		// Token: 0x04000CA3 RID: 3235
		[Dependency]
		private readonly IPrototypeManager _prototypeManager;

		// Token: 0x04000CA4 RID: 3236
		[Dependency]
		private readonly IRobustRandom _random;

		// Token: 0x04000CA5 RID: 3237
		[Dependency]
		private readonly AirlockSystem _airlock;

		// Token: 0x04000CA6 RID: 3238
		[Dependency]
		private readonly BodySystem _bodySystem;

		// Token: 0x04000CA7 RID: 3239
		[Dependency]
		private readonly EntityLookupSystem _lookup;

		// Token: 0x04000CA8 RID: 3240
		[Dependency]
		private readonly SharedDoorSystem _doorSystem;

		// Token: 0x04000CA9 RID: 3241
		[Dependency]
		private readonly SharedActionsSystem _actionsSystem;

		// Token: 0x04000CAA RID: 3242
		[Dependency]
		private readonly DoAfterSystem _doAfter;

		// Token: 0x04000CAB RID: 3243
		[Dependency]
		private readonly GunSystem _gunSystem;

		// Token: 0x04000CAC RID: 3244
		[Dependency]
		private readonly PhysicsSystem _physics;

		// Token: 0x04000CAD RID: 3245
		[Dependency]
		private readonly SharedTransformSystem _transformSystem;

		// Token: 0x04000CAE RID: 3246
		[Dependency]
		private readonly SharedAudioSystem _audio;
	}
}
