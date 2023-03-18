using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading;
using Content.Server.Administration.Logs;
using Content.Server.DoAfter;
using Content.Server.Kitchen.Components;
using Content.Server.Popups;
using Content.Shared.Administration.Logs;
using Content.Shared.Database;
using Content.Shared.DoAfter;
using Content.Shared.DragDrop;
using Content.Shared.IdentityManagement;
using Content.Shared.Interaction;
using Content.Shared.Interaction.Events;
using Content.Shared.Kitchen;
using Content.Shared.Kitchen.Components;
using Content.Shared.Mobs.Components;
using Content.Shared.Mobs.Systems;
using Content.Shared.Nutrition.Components;
using Content.Shared.Popups;
using Content.Shared.Storage;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Localization;
using Robust.Shared.Player;
using Robust.Shared.Random;

namespace Content.Server.Kitchen.EntitySystems
{
	// Token: 0x0200042E RID: 1070
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class KitchenSpikeSystem : SharedKitchenSpikeSystem
	{
		// Token: 0x060015A1 RID: 5537 RVA: 0x000716A8 File Offset: 0x0006F8A8
		public override void Initialize()
		{
			base.Initialize();
			base.SubscribeLocalEvent<KitchenSpikeComponent, InteractUsingEvent>(new ComponentEventHandler<KitchenSpikeComponent, InteractUsingEvent>(this.OnInteractUsing), null, null);
			base.SubscribeLocalEvent<KitchenSpikeComponent, InteractHandEvent>(new ComponentEventHandler<KitchenSpikeComponent, InteractHandEvent>(this.OnInteractHand), null, null);
			base.SubscribeLocalEvent<KitchenSpikeComponent, DragDropTargetEvent>(new ComponentEventRefHandler<KitchenSpikeComponent, DragDropTargetEvent>(this.OnDragDrop), null, null);
			base.SubscribeLocalEvent<KitchenSpikeComponent, DoAfterEvent>(new ComponentEventHandler<KitchenSpikeComponent, DoAfterEvent>(this.OnDoAfter), null, null);
			base.SubscribeLocalEvent<KitchenSpikeComponent, SuicideEvent>(new ComponentEventHandler<KitchenSpikeComponent, SuicideEvent>(this.OnSuicide), null, null);
			base.SubscribeLocalEvent<ButcherableComponent, CanDropDraggedEvent>(new ComponentEventRefHandler<ButcherableComponent, CanDropDraggedEvent>(this.OnButcherableCanDrop), null, null);
		}

		// Token: 0x060015A2 RID: 5538 RVA: 0x00071733 File Offset: 0x0006F933
		private void OnButcherableCanDrop(EntityUid uid, ButcherableComponent component, ref CanDropDraggedEvent args)
		{
			args.Handled = true;
			args.CanDrop |= (component.Type > ButcheringType.Knife);
		}

		// Token: 0x060015A3 RID: 5539 RVA: 0x00071750 File Offset: 0x0006F950
		private void OnSuicide(EntityUid uid, KitchenSpikeComponent component, SuicideEvent args)
		{
			if (args.Handled)
			{
				return;
			}
			args.SetHandled(SuicideKind.Piercing);
			EntityUid victim = args.Victim;
			string othersMessage = Loc.GetString("comp-kitchen-spike-suicide-other", new ValueTuple<string, object>[]
			{
				new ValueTuple<string, object>("victim", victim)
			});
			this._popupSystem.PopupEntity(othersMessage, victim, PopupType.Small);
			string selfMessage = Loc.GetString("comp-kitchen-spike-suicide-self");
			this._popupSystem.PopupEntity(selfMessage, victim, victim, PopupType.Small);
		}

		// Token: 0x060015A4 RID: 5540 RVA: 0x000717C4 File Offset: 0x0006F9C4
		private void OnDoAfter(EntityUid uid, KitchenSpikeComponent component, DoAfterEvent args)
		{
			if (args.Args.Target == null)
			{
				return;
			}
			ButcherableComponent butcherable;
			if (base.TryComp<ButcherableComponent>(args.Args.Target.Value, ref butcherable))
			{
				butcherable.BeingButchered = false;
			}
			if (args.Handled || args.Cancelled)
			{
				return;
			}
			if (this.Spikeable(uid, args.Args.User, args.Args.Target.Value, component, butcherable))
			{
				this.Spike(uid, args.Args.User, args.Args.Target.Value, component, null);
			}
			args.Handled = true;
		}

		// Token: 0x060015A5 RID: 5541 RVA: 0x00071867 File Offset: 0x0006FA67
		private void OnDragDrop(EntityUid uid, KitchenSpikeComponent component, ref DragDropTargetEvent args)
		{
			if (args.Handled)
			{
				return;
			}
			args.Handled = true;
			if (this.Spikeable(uid, args.User, args.Dragged, component, null))
			{
				this.TrySpike(uid, args.User, args.Dragged, component, null, null);
			}
		}

		// Token: 0x060015A6 RID: 5542 RVA: 0x000718A8 File Offset: 0x0006FAA8
		private void OnInteractHand(EntityUid uid, KitchenSpikeComponent component, InteractHandEvent args)
		{
			if (args.Handled)
			{
				return;
			}
			List<string> prototypesToSpawn = component.PrototypesToSpawn;
			if (prototypesToSpawn != null && prototypesToSpawn.Count > 0)
			{
				this._popupSystem.PopupEntity(Loc.GetString("comp-kitchen-spike-knife-needed"), uid, args.User, PopupType.Small);
				args.Handled = true;
			}
		}

		// Token: 0x060015A7 RID: 5543 RVA: 0x000718F9 File Offset: 0x0006FAF9
		private void OnInteractUsing(EntityUid uid, KitchenSpikeComponent component, InteractUsingEvent args)
		{
			if (args.Handled)
			{
				return;
			}
			if (this.TryGetPiece(uid, args.User, args.Used, null, null))
			{
				args.Handled = true;
			}
		}

		// Token: 0x060015A8 RID: 5544 RVA: 0x00071924 File Offset: 0x0006FB24
		[NullableContext(2)]
		private void Spike(EntityUid uid, EntityUid userUid, EntityUid victimUid, KitchenSpikeComponent component = null, ButcherableComponent butcherable = null)
		{
			if (!base.Resolve<KitchenSpikeComponent>(uid, ref component, true) || !base.Resolve<ButcherableComponent>(victimUid, ref butcherable, true))
			{
				return;
			}
			ISharedAdminLogManager logger = this._logger;
			LogType type = LogType.Gib;
			LogImpact impact = LogImpact.Extreme;
			LogStringHandler logStringHandler = new LogStringHandler(16, 2);
			logStringHandler.AppendFormatted<EntityStringRepresentation>(base.ToPrettyString(userUid), "user", "ToPrettyString(userUid)");
			logStringHandler.AppendLiteral(" kitchen spiked ");
			logStringHandler.AppendFormatted<EntityStringRepresentation>(base.ToPrettyString(victimUid), "target", "ToPrettyString(victimUid)");
			logger.Add(type, impact, ref logStringHandler);
			component.PrototypesToSpawn = EntitySpawnCollection.GetSpawns(butcherable.SpawnedEntities, this._random);
			component.MeatSource1p = Loc.GetString("comp-kitchen-spike-remove-meat", new ValueTuple<string, object>[]
			{
				new ValueTuple<string, object>("victim", victimUid)
			});
			component.MeatSource0 = Loc.GetString("comp-kitchen-spike-remove-meat-last", new ValueTuple<string, object>[]
			{
				new ValueTuple<string, object>("victim", victimUid)
			});
			component.Victim = base.Name(victimUid, null);
			this.UpdateAppearance(uid, null, component);
			this._popupSystem.PopupEntity(Loc.GetString("comp-kitchen-spike-kill", new ValueTuple<string, object>[]
			{
				new ValueTuple<string, object>("user", Identity.Entity(userUid, this.EntityManager)),
				new ValueTuple<string, object>("victim", victimUid)
			}), uid, PopupType.LargeCaution);
			this.EntityManager.QueueDeleteEntity(victimUid);
			this._audio.Play(component.SpikeSound, Filter.Pvs(uid, 2f, null, null, null), uid, true, null);
		}

		// Token: 0x060015A9 RID: 5545 RVA: 0x00071ABC File Offset: 0x0006FCBC
		[NullableContext(2)]
		private bool TryGetPiece(EntityUid uid, EntityUid user, EntityUid used, KitchenSpikeComponent component = null, SharpComponent sharp = null)
		{
			if (!base.Resolve<KitchenSpikeComponent>(uid, ref component, true) || component.PrototypesToSpawn == null || component.PrototypesToSpawn.Count == 0)
			{
				return false;
			}
			if (!base.Resolve<SharpComponent>(used, ref sharp, false))
			{
				return false;
			}
			string item = RandomExtensions.PickAndTake<string>(this._random, component.PrototypesToSpawn);
			EntityUid ent = base.Spawn(item, base.Transform(uid).Coordinates);
			base.MetaData(ent).EntityName = Loc.GetString("comp-kitchen-spike-meat-name", new ValueTuple<string, object>[]
			{
				new ValueTuple<string, object>("name", base.Name(ent, null)),
				new ValueTuple<string, object>("victim", component.Victim)
			});
			if (component.PrototypesToSpawn.Count != 0)
			{
				this._popupSystem.PopupEntity(component.MeatSource1p, uid, user, PopupType.MediumCaution);
			}
			else
			{
				this.UpdateAppearance(uid, null, component);
				this._popupSystem.PopupEntity(component.MeatSource0, uid, user, PopupType.MediumCaution);
			}
			return true;
		}

		// Token: 0x060015AA RID: 5546 RVA: 0x00071BB8 File Offset: 0x0006FDB8
		[NullableContext(2)]
		private void UpdateAppearance(EntityUid uid, AppearanceComponent appearance = null, KitchenSpikeComponent component = null)
		{
			if (!base.Resolve<KitchenSpikeComponent, AppearanceComponent>(uid, ref component, ref appearance, false))
			{
				return;
			}
			SharedAppearanceSystem appearance2 = this._appearance;
			Enum @enum = SharedKitchenSpikeComponent.KitchenSpikeVisuals.Status;
			List<string> prototypesToSpawn = component.PrototypesToSpawn;
			appearance2.SetData(uid, @enum, (prototypesToSpawn != null && prototypesToSpawn.Count > 0) ? SharedKitchenSpikeComponent.KitchenSpikeStatus.Bloody : SharedKitchenSpikeComponent.KitchenSpikeStatus.Empty, appearance);
		}

		// Token: 0x060015AB RID: 5547 RVA: 0x00071C08 File Offset: 0x0006FE08
		[NullableContext(2)]
		private bool Spikeable(EntityUid uid, EntityUid userUid, EntityUid victimUid, KitchenSpikeComponent component = null, ButcherableComponent butcherable = null)
		{
			if (!base.Resolve<KitchenSpikeComponent>(uid, ref component, true))
			{
				return false;
			}
			List<string> prototypesToSpawn = component.PrototypesToSpawn;
			if (prototypesToSpawn != null && prototypesToSpawn.Count > 0)
			{
				this._popupSystem.PopupEntity(Loc.GetString("comp-kitchen-spike-deny-collect", new ValueTuple<string, object>[]
				{
					new ValueTuple<string, object>("this", uid)
				}), uid, userUid, PopupType.Small);
				return false;
			}
			if (!base.Resolve<ButcherableComponent>(victimUid, ref butcherable, false))
			{
				this._popupSystem.PopupEntity(Loc.GetString("comp-kitchen-spike-deny-butcher", new ValueTuple<string, object>[]
				{
					new ValueTuple<string, object>("victim", Identity.Entity(victimUid, this.EntityManager)),
					new ValueTuple<string, object>("this", uid)
				}), victimUid, userUid, PopupType.Small);
				return false;
			}
			ButcheringType type = butcherable.Type;
			if (type == ButcheringType.Knife)
			{
				this._popupSystem.PopupEntity(Loc.GetString("comp-kitchen-spike-deny-butcher-knife", new ValueTuple<string, object>[]
				{
					new ValueTuple<string, object>("victim", Identity.Entity(victimUid, this.EntityManager)),
					new ValueTuple<string, object>("this", uid)
				}), victimUid, userUid, PopupType.Small);
				return false;
			}
			if (type == ButcheringType.Spike)
			{
				return true;
			}
			this._popupSystem.PopupEntity(Loc.GetString("comp-kitchen-spike-deny-butcher", new ValueTuple<string, object>[]
			{
				new ValueTuple<string, object>("victim", Identity.Entity(victimUid, this.EntityManager)),
				new ValueTuple<string, object>("this", uid)
			}), victimUid, userUid, PopupType.Small);
			return false;
		}

		// Token: 0x060015AC RID: 5548 RVA: 0x00071D9C File Offset: 0x0006FF9C
		[NullableContext(2)]
		public bool TrySpike(EntityUid uid, EntityUid userUid, EntityUid victimUid, KitchenSpikeComponent component = null, ButcherableComponent butcherable = null, MobStateComponent mobState = null)
		{
			if (!base.Resolve<KitchenSpikeComponent>(uid, ref component, true) || component.InUse || !base.Resolve<ButcherableComponent>(victimUid, ref butcherable, true) || butcherable.BeingButchered)
			{
				return false;
			}
			if (base.Resolve<MobStateComponent>(victimUid, ref mobState, false) && this._mobStateSystem.IsAlive(victimUid, mobState))
			{
				this._popupSystem.PopupEntity(Loc.GetString("comp-kitchen-spike-deny-not-dead", new ValueTuple<string, object>[]
				{
					new ValueTuple<string, object>("victim", Identity.Entity(victimUid, this.EntityManager))
				}), victimUid, userUid, PopupType.Small);
				return true;
			}
			if (userUid != victimUid)
			{
				this._popupSystem.PopupEntity(Loc.GetString("comp-kitchen-spike-begin-hook-victim", new ValueTuple<string, object>[]
				{
					new ValueTuple<string, object>("user", Identity.Entity(userUid, this.EntityManager)),
					new ValueTuple<string, object>("this", uid)
				}), victimUid, victimUid, PopupType.LargeCaution);
			}
			butcherable.BeingButchered = true;
			component.InUse = true;
			float delay = component.SpikeDelay + butcherable.ButcherDelay;
			EntityUid? target = new EntityUid?(victimUid);
			EntityUid? used = new EntityUid?(uid);
			DoAfterEventArgs doAfterArgs = new DoAfterEventArgs(userUid, delay, default(CancellationToken), target, used)
			{
				BreakOnTargetMove = true,
				BreakOnUserMove = true,
				BreakOnDamage = true,
				BreakOnStun = true,
				NeedHand = true
			};
			this._doAfter.DoAfter(doAfterArgs);
			return true;
		}

		// Token: 0x04000D75 RID: 3445
		[Dependency]
		private readonly PopupSystem _popupSystem;

		// Token: 0x04000D76 RID: 3446
		[Dependency]
		private readonly DoAfterSystem _doAfter;

		// Token: 0x04000D77 RID: 3447
		[Dependency]
		private readonly IAdminLogManager _logger;

		// Token: 0x04000D78 RID: 3448
		[Dependency]
		private readonly MobStateSystem _mobStateSystem;

		// Token: 0x04000D79 RID: 3449
		[Dependency]
		private readonly IRobustRandom _random;

		// Token: 0x04000D7A RID: 3450
		[Dependency]
		private readonly SharedAppearanceSystem _appearance;

		// Token: 0x04000D7B RID: 3451
		[Dependency]
		private readonly SharedAudioSystem _audio;
	}
}
