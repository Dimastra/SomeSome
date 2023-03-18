using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using Content.Server.Body.Systems;
using Content.Server.Chat.Systems;
using Content.Server.DoAfter;
using Content.Server.GameTicking;
using Content.Server.GameTicking.Rules;
using Content.Server.NPC.Systems;
using Content.Server.Popups;
using Content.Shared.Actions;
using Content.Shared.Chemistry.Components;
using Content.Shared.Damage;
using Content.Shared.DoAfter;
using Content.Shared.Dragon;
using Content.Shared.Examine;
using Content.Shared.Humanoid;
using Content.Shared.Maps;
using Content.Shared.Mobs;
using Content.Shared.Mobs.Components;
using Content.Shared.Movement.Systems;
using Content.Shared.Popups;
using Content.Shared.Whitelist;
using Robust.Server.GameObjects;
using Robust.Shared.Audio;
using Robust.Shared.Containers;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.IoC;
using Robust.Shared.Localization;
using Robust.Shared.Map;
using Robust.Shared.Map.Components;
using Robust.Shared.Maths;
using Robust.Shared.Player;
using Robust.Shared.Random;

namespace Content.Server.Dragon
{
	// Token: 0x0200053E RID: 1342
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class DragonSystem : GameRuleSystem
	{
		// Token: 0x06001BF5 RID: 7157 RVA: 0x000950E0 File Offset: 0x000932E0
		public override void Initialize()
		{
			base.Initialize();
			base.SubscribeLocalEvent<DragonComponent, ComponentStartup>(new ComponentEventHandler<DragonComponent, ComponentStartup>(this.OnStartup), null, null);
			base.SubscribeLocalEvent<DragonComponent, ComponentShutdown>(new ComponentEventHandler<DragonComponent, ComponentShutdown>(this.OnShutdown), null, null);
			base.SubscribeLocalEvent<DragonComponent, DragonDevourActionEvent>(new ComponentEventHandler<DragonComponent, DragonDevourActionEvent>(this.OnDevourAction), null, null);
			base.SubscribeLocalEvent<DragonComponent, DragonSpawnRiftActionEvent>(new ComponentEventHandler<DragonComponent, DragonSpawnRiftActionEvent>(this.OnDragonRift), null, null);
			base.SubscribeLocalEvent<DragonComponent, RefreshMovementSpeedModifiersEvent>(new ComponentEventHandler<DragonComponent, RefreshMovementSpeedModifiersEvent>(this.OnDragonMove), null, null);
			base.SubscribeLocalEvent<DragonComponent, DoAfterEvent>(new ComponentEventHandler<DragonComponent, DoAfterEvent>(this.OnDoAfter), null, null);
			base.SubscribeLocalEvent<DragonComponent, MobStateChangedEvent>(new ComponentEventHandler<DragonComponent, MobStateChangedEvent>(this.OnMobStateChanged), null, null);
			base.SubscribeLocalEvent<DragonRiftComponent, ComponentShutdown>(new ComponentEventHandler<DragonRiftComponent, ComponentShutdown>(this.OnRiftShutdown), null, null);
			base.SubscribeLocalEvent<DragonRiftComponent, ComponentGetState>(new ComponentEventRefHandler<DragonRiftComponent, ComponentGetState>(this.OnRiftGetState), null, null);
			base.SubscribeLocalEvent<DragonRiftComponent, AnchorStateChangedEvent>(new ComponentEventRefHandler<DragonRiftComponent, AnchorStateChangedEvent>(this.OnAnchorChange), null, null);
			base.SubscribeLocalEvent<DragonRiftComponent, ExaminedEvent>(new ComponentEventHandler<DragonRiftComponent, ExaminedEvent>(this.OnRiftExamined), null, null);
			base.SubscribeLocalEvent<RoundEndTextAppendEvent>(new EntityEventHandler<RoundEndTextAppendEvent>(this.OnRiftRoundEnd), null, null);
		}

		// Token: 0x06001BF6 RID: 7158 RVA: 0x000951E4 File Offset: 0x000933E4
		private void OnDoAfter(EntityUid uid, DragonComponent component, DoAfterEvent args)
		{
			if (args.Handled || args.Cancelled)
			{
				return;
			}
			Solution ichorInjection = new Solution(component.DevourChem, component.DevourHealRate);
			if (base.HasComp<HumanoidAppearanceComponent>(args.Args.Target))
			{
				ichorInjection.ScaleSolution(0.5f);
				component.DragonStomach.Insert(args.Args.Target.Value, null, null, null, null, null);
				this._bloodstreamSystem.TryAddToChemicals(uid, ichorInjection, null);
			}
			else if (args.Args.Target != null)
			{
				this.EntityManager.QueueDeleteEntity(args.Args.Target.Value);
			}
			if (component.SoundDevour != null)
			{
				this._audioSystem.PlayPvs(component.SoundDevour, uid, new AudioParams?(component.SoundDevour.Params));
			}
		}

		// Token: 0x06001BF7 RID: 7159 RVA: 0x000952C4 File Offset: 0x000934C4
		public override void Update(float frameTime)
		{
			base.Update(frameTime);
			foreach (DragonComponent comp in base.EntityQuery<DragonComponent>(false))
			{
				if (comp.WeakenedAccumulator > 0f)
				{
					comp.WeakenedAccumulator -= frameTime;
					if (comp.WeakenedAccumulator < 0f)
					{
						comp.WeakenedAccumulator = 0f;
						this._movement.RefreshMovementSpeedModifiers(comp.Owner, null);
					}
				}
				if (comp.Rifts.Count < 3)
				{
					if (comp.Rifts.Count > 0)
					{
						List<EntityUid> rifts = comp.Rifts;
						EntityUid lastRift = rifts[rifts.Count - 1];
						DragonRiftComponent rift;
						if (base.TryComp<DragonRiftComponent>(lastRift, ref rift) && rift.State != DragonRiftState.Finished)
						{
							comp.RiftAccumulator = 0f;
							continue;
						}
					}
					comp.RiftAccumulator += frameTime;
					if (comp.RiftAccumulator >= comp.RiftMaxAccumulator)
					{
						this.Roar(comp);
						base.QueueDel(comp.Owner);
					}
				}
			}
			foreach (DragonRiftComponent comp2 in base.EntityQuery<DragonRiftComponent>(false))
			{
				if (comp2.State != DragonRiftState.Finished && comp2.Accumulator >= comp2.MaxAccumulator)
				{
					comp2.Accumulator = comp2.MaxAccumulator;
					base.RemComp<DamageableComponent>(comp2.Owner);
					comp2.State = DragonRiftState.Finished;
					base.Dirty(comp2, null);
				}
				else if (comp2.State != DragonRiftState.Finished)
				{
					comp2.Accumulator += frameTime;
				}
				comp2.SpawnAccumulator += frameTime;
				if (comp2.State < DragonRiftState.AlmostFinished && comp2.Accumulator > comp2.MaxAccumulator / 2f)
				{
					comp2.State = DragonRiftState.AlmostFinished;
					base.Dirty(comp2, null);
					Vector2 location = base.Transform(comp2.Owner).LocalPosition;
					this._chat.DispatchGlobalAnnouncement(Loc.GetString("carp-rift-warning", new ValueTuple<string, object>[]
					{
						new ValueTuple<string, object>("location", location)
					}), "Central Command", false, null, new Color?(Color.Red));
					this._audioSystem.PlayGlobal("/Audio/Misc/notice1.ogg", Filter.Broadcast(), true, null);
				}
				if (comp2.SpawnAccumulator > comp2.SpawnCooldown)
				{
					comp2.SpawnAccumulator -= comp2.SpawnCooldown;
					EntityUid ent = base.Spawn(comp2.SpawnPrototype, base.Transform(comp2.Owner).MapPosition);
					this._npc.SetBlackboard(ent, "FollowTarget", new EntityCoordinates(comp2.Owner, Vector2.Zero), null);
				}
			}
		}

		// Token: 0x06001BF8 RID: 7160 RVA: 0x000955C8 File Offset: 0x000937C8
		private void OnRiftExamined(EntityUid uid, DragonRiftComponent component, ExaminedEvent args)
		{
			args.PushMarkup(Loc.GetString("carp-rift-examine", new ValueTuple<string, object>[]
			{
				new ValueTuple<string, object>("percentage", MathF.Round(component.Accumulator / component.MaxAccumulator * 100f))
			}));
		}

		// Token: 0x06001BF9 RID: 7161 RVA: 0x00095619 File Offset: 0x00093819
		private void OnAnchorChange(EntityUid uid, DragonRiftComponent component, ref AnchorStateChangedEvent args)
		{
			if (!args.Anchored && component.State == DragonRiftState.Charging)
			{
				base.QueueDel(uid);
			}
		}

		// Token: 0x06001BFA RID: 7162 RVA: 0x00095634 File Offset: 0x00093834
		private void OnRiftShutdown(EntityUid uid, DragonRiftComponent component, ComponentShutdown args)
		{
			DragonComponent dragon;
			if (base.TryComp<DragonComponent>(component.Dragon, ref dragon) && !dragon.Weakened)
			{
				foreach (EntityUid rift in dragon.Rifts)
				{
					base.QueueDel(rift);
				}
				dragon.Rifts.Clear();
				dragon.WeakenedAccumulator = dragon.WeakenedDuration;
				this._movement.RefreshMovementSpeedModifiers(component.Dragon, null);
				this._popupSystem.PopupEntity(Loc.GetString("carp-rift-destroyed"), component.Dragon, component.Dragon, PopupType.Small);
			}
		}

		// Token: 0x06001BFB RID: 7163 RVA: 0x000956F0 File Offset: 0x000938F0
		private void OnRiftGetState(EntityUid uid, DragonRiftComponent component, ref ComponentGetState args)
		{
			args.State = new DragonRiftComponentState
			{
				State = component.State
			};
		}

		// Token: 0x06001BFC RID: 7164 RVA: 0x00095709 File Offset: 0x00093909
		private void OnDragonMove(EntityUid uid, DragonComponent component, RefreshMovementSpeedModifiersEvent args)
		{
			if (component.Weakened)
			{
				args.ModifySpeed(0.5f, 0.5f);
			}
		}

		// Token: 0x06001BFD RID: 7165 RVA: 0x00095724 File Offset: 0x00093924
		private void OnDragonRift(EntityUid uid, DragonComponent component, DragonSpawnRiftActionEvent args)
		{
			if (component.Weakened)
			{
				this._popupSystem.PopupEntity(Loc.GetString("carp-rift-weakened"), uid, uid, PopupType.Small);
				return;
			}
			if (component.Rifts.Count >= 3)
			{
				this._popupSystem.PopupEntity(Loc.GetString("carp-rift-max"), uid, uid, PopupType.Small);
				return;
			}
			if (component.Rifts.Count > 0)
			{
				List<EntityUid> rifts = component.Rifts;
				DragonRiftComponent rift;
				if (base.TryComp<DragonRiftComponent>(rifts[rifts.Count - 1], ref rift) && rift.State != DragonRiftState.Finished)
				{
					this._popupSystem.PopupEntity(Loc.GetString("carp-rift-duplicate"), uid, uid, PopupType.Small);
					return;
				}
			}
			TransformComponent xform = base.Transform(uid);
			MapGridComponent grid;
			if (!this._mapManager.TryGetGrid(xform.GridUid, ref grid))
			{
				this._popupSystem.PopupEntity(Loc.GetString("carp-rift-anchor"), uid, uid, PopupType.Small);
				return;
			}
			using (IEnumerator<ValueTuple<DragonRiftComponent, TransformComponent>> enumerator = base.EntityQuery<DragonRiftComponent, TransformComponent>(true).GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					if (enumerator.Current.Item2.Coordinates.InRange(this.EntityManager, xform.Coordinates, 15f))
					{
						this._popupSystem.PopupEntity(Loc.GetString("carp-rift-proximity", new ValueTuple<string, object>[]
						{
							new ValueTuple<string, object>("proximity", 15)
						}), uid, uid, PopupType.Small);
						return;
					}
				}
			}
			using (IEnumerator<TileRef> enumerator2 = grid.GetTilesIntersecting(new Circle(xform.WorldPosition, 2f), false, null).GetEnumerator())
			{
				while (enumerator2.MoveNext())
				{
					if (enumerator2.Current.IsSpace(this._tileDef))
					{
						this._popupSystem.PopupEntity(Loc.GetString("carp-rift-space-proximity", new ValueTuple<string, object>[]
						{
							new ValueTuple<string, object>("proximity", 2)
						}), uid, uid, PopupType.Small);
						return;
					}
				}
			}
			EntityUid carpUid = base.Spawn(component.RiftPrototype, xform.MapPosition);
			component.Rifts.Add(carpUid);
			base.Comp<DragonRiftComponent>(carpUid).Dragon = uid;
			this._audioSystem.PlayPvs("/Audio/Weapons/Guns/Gunshots/rocket_launcher.ogg", carpUid, null);
		}

		// Token: 0x06001BFE RID: 7166 RVA: 0x00095974 File Offset: 0x00093B74
		private void OnShutdown(EntityUid uid, DragonComponent component, ComponentShutdown args)
		{
			foreach (EntityUid rift in component.Rifts)
			{
				base.QueueDel(rift);
			}
		}

		// Token: 0x06001BFF RID: 7167 RVA: 0x000959C8 File Offset: 0x00093BC8
		private void OnMobStateChanged(EntityUid uid, DragonComponent component, MobStateChangedEvent args)
		{
			if (args.NewMobState == MobState.Dead)
			{
				if (component.SoundDeath != null)
				{
					this._audioSystem.PlayPvs(component.SoundDeath, uid, new AudioParams?(component.SoundDeath.Params));
				}
				ContainerHelpers.EmptyContainer(component.DragonStomach, false, null, false, null);
				foreach (EntityUid rift in component.Rifts)
				{
					base.QueueDel(rift);
				}
				component.Rifts.Clear();
			}
		}

		// Token: 0x06001C00 RID: 7168 RVA: 0x00095A78 File Offset: 0x00093C78
		private void Roar(DragonComponent component)
		{
			if (component.SoundRoar != null)
			{
				this._audioSystem.Play(component.SoundRoar, Filter.Pvs(component.Owner, 4f, this.EntityManager, null, null), component.Owner, true, new AudioParams?(component.SoundRoar.Params));
			}
		}

		// Token: 0x06001C01 RID: 7169 RVA: 0x00095AD0 File Offset: 0x00093CD0
		private void OnStartup(EntityUid uid, DragonComponent component, ComponentStartup args)
		{
			component.DragonStomach = this._containerSystem.EnsureContainer<Container>(uid, "dragon_stomach", null);
			if (component.DevourAction != null)
			{
				this._actionsSystem.AddAction(uid, component.DevourAction, null, null, true);
			}
			if (component.SpawnRiftAction != null)
			{
				this._actionsSystem.AddAction(uid, component.SpawnRiftAction, null, null, true);
			}
			this.Roar(component);
		}

		// Token: 0x06001C02 RID: 7170 RVA: 0x00095B48 File Offset: 0x00093D48
		private void OnDevourAction(EntityUid uid, DragonComponent component, DragonDevourActionEvent args)
		{
			if (!args.Handled)
			{
				EntityWhitelist devourWhitelist = component.DevourWhitelist;
				if (devourWhitelist != null && devourWhitelist.IsValid(args.Target, this.EntityManager))
				{
					args.Handled = true;
					EntityUid target = args.Target;
					MobStateComponent targetState;
					if (!this.EntityManager.TryGetComponent<MobStateComponent>(target, ref targetState))
					{
						this._popupSystem.PopupEntity(Loc.GetString("devour-action-popup-message-structure"), uid, uid, PopupType.Small);
						if (component.SoundStructureDevour != null)
						{
							this._audioSystem.PlayPvs(component.SoundStructureDevour, uid, new AudioParams?(component.SoundStructureDevour.Params));
						}
						SharedDoAfterSystem doAfterSystem = this._doAfterSystem;
						float structureDevourTime = component.StructureDevourTime;
						EntityUid? target2 = new EntityUid?(target);
						doAfterSystem.DoAfter(new DoAfterEventArgs(uid, structureDevourTime, default(CancellationToken), target2, null)
						{
							BreakOnTargetMove = true,
							BreakOnUserMove = true,
							BreakOnStun = true
						});
						return;
					}
					MobState currentState = targetState.CurrentState;
					if (currentState - MobState.Critical <= 1)
					{
						SharedDoAfterSystem doAfterSystem2 = this._doAfterSystem;
						float devourTime = component.DevourTime;
						EntityUid? target2 = new EntityUid?(target);
						doAfterSystem2.DoAfter(new DoAfterEventArgs(uid, devourTime, default(CancellationToken), target2, null)
						{
							BreakOnTargetMove = true,
							BreakOnUserMove = true,
							BreakOnStun = true
						});
						return;
					}
					this._popupSystem.PopupEntity(Loc.GetString("devour-action-popup-message-fail-target-alive"), uid, uid, PopupType.Small);
					return;
				}
			}
		}

		// Token: 0x17000430 RID: 1072
		// (get) Token: 0x06001C03 RID: 7171 RVA: 0x00095C9D File Offset: 0x00093E9D
		public override string Prototype
		{
			get
			{
				return "Dragon";
			}
		}

		// Token: 0x06001C04 RID: 7172 RVA: 0x00095CA4 File Offset: 0x00093EA4
		private int RiftsMet(DragonComponent component)
		{
			int finished = 0;
			foreach (EntityUid rift in component.Rifts)
			{
				DragonRiftComponent drift;
				if (base.TryComp<DragonRiftComponent>(rift, ref drift) && drift.State == DragonRiftState.Finished)
				{
					finished++;
				}
			}
			return finished;
		}

		// Token: 0x06001C05 RID: 7173 RVA: 0x00095D0C File Offset: 0x00093F0C
		public override void Started()
		{
			List<ValueTuple<MapGridComponent, TransformComponent>> spawnLocations = this.EntityManager.EntityQuery<MapGridComponent, TransformComponent>(false).ToList<ValueTuple<MapGridComponent, TransformComponent>>();
			if (spawnLocations.Count == 0)
			{
				return;
			}
			ValueTuple<MapGridComponent, TransformComponent> location = RandomExtensions.Pick<ValueTuple<MapGridComponent, TransformComponent>>(this._random, spawnLocations);
			base.Spawn("MobDragon", location.Item2.MapPosition);
		}

		// Token: 0x06001C06 RID: 7174 RVA: 0x00095D58 File Offset: 0x00093F58
		public override void Ended()
		{
		}

		// Token: 0x06001C07 RID: 7175 RVA: 0x00095D5C File Offset: 0x00093F5C
		private void OnRiftRoundEnd(RoundEndTextAppendEvent args)
		{
			if (!base.RuleAdded)
			{
				return;
			}
			if (base.EntityQuery<DragonComponent>(true).ToList<DragonComponent>().Count == 0)
			{
				return;
			}
			args.AddLine(Loc.GetString("dragon-round-end-summary"));
			foreach (DragonComponent dragon in base.EntityQuery<DragonComponent>(true))
			{
				int met = this.RiftsMet(dragon);
				ActorComponent actor;
				if (base.TryComp<ActorComponent>(dragon.Owner, ref actor))
				{
					args.AddLine(Loc.GetString("dragon-round-end-dragon-player", new ValueTuple<string, object>[]
					{
						new ValueTuple<string, object>("name", dragon.Owner),
						new ValueTuple<string, object>("count", met),
						new ValueTuple<string, object>("player", actor.PlayerSession)
					}));
				}
				else
				{
					args.AddLine(Loc.GetString("dragon-round-end-dragon", new ValueTuple<string, object>[]
					{
						new ValueTuple<string, object>("name", dragon.Owner),
						new ValueTuple<string, object>("count", met)
					}));
				}
			}
		}

		// Token: 0x0400120F RID: 4623
		[Dependency]
		private readonly IMapManager _mapManager;

		// Token: 0x04001210 RID: 4624
		[Dependency]
		private readonly IRobustRandom _random;

		// Token: 0x04001211 RID: 4625
		[Dependency]
		private readonly ITileDefinitionManager _tileDef;

		// Token: 0x04001212 RID: 4626
		[Dependency]
		private readonly ChatSystem _chat;

		// Token: 0x04001213 RID: 4627
		[Dependency]
		private readonly SharedActionsSystem _actionsSystem;

		// Token: 0x04001214 RID: 4628
		[Dependency]
		private readonly DoAfterSystem _doAfterSystem;

		// Token: 0x04001215 RID: 4629
		[Dependency]
		private readonly PopupSystem _popupSystem;

		// Token: 0x04001216 RID: 4630
		[Dependency]
		private readonly BloodstreamSystem _bloodstreamSystem;

		// Token: 0x04001217 RID: 4631
		[Dependency]
		private readonly MovementSpeedModifierSystem _movement;

		// Token: 0x04001218 RID: 4632
		[Dependency]
		private readonly SharedContainerSystem _containerSystem;

		// Token: 0x04001219 RID: 4633
		[Dependency]
		private readonly SharedAudioSystem _audioSystem;

		// Token: 0x0400121A RID: 4634
		[Dependency]
		private readonly NPCSystem _npc;

		// Token: 0x0400121B RID: 4635
		private const int RiftRange = 15;

		// Token: 0x0400121C RID: 4636
		private const int RiftTileRadius = 2;

		// Token: 0x0400121D RID: 4637
		private const int RiftsAllowed = 3;
	}
}
