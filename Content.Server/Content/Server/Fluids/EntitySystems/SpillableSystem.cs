using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using Content.Server.Administration.Logs;
using Content.Server.Chemistry.EntitySystems;
using Content.Server.DoAfter;
using Content.Server.Fluids.Components;
using Content.Server.Nutrition.Components;
using Content.Shared.Administration.Logs;
using Content.Shared.Chemistry.Components;
using Content.Shared.Chemistry.Reagent;
using Content.Shared.Clothing.Components;
using Content.Shared.Database;
using Content.Shared.DoAfter;
using Content.Shared.FixedPoint;
using Content.Shared.Inventory.Events;
using Content.Shared.Throwing;
using Content.Shared.Verbs;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Localization;
using Robust.Shared.Map;
using Robust.Shared.Map.Components;
using Robust.Shared.Prototypes;

namespace Content.Server.Fluids.EntitySystems
{
	// Token: 0x020004F1 RID: 1265
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class SpillableSystem : EntitySystem
	{
		// Token: 0x06001A14 RID: 6676 RVA: 0x000895D0 File Offset: 0x000877D0
		public override void Initialize()
		{
			base.Initialize();
			base.SubscribeLocalEvent<SpillableComponent, LandEvent>(new ComponentEventRefHandler<SpillableComponent, LandEvent>(this.SpillOnLand), null, null);
			base.SubscribeLocalEvent<SpillableComponent, GetVerbsEvent<Verb>>(new ComponentEventHandler<SpillableComponent, GetVerbsEvent<Verb>>(this.AddSpillVerb), null, null);
			base.SubscribeLocalEvent<SpillableComponent, GotEquippedEvent>(new ComponentEventHandler<SpillableComponent, GotEquippedEvent>(this.OnGotEquipped), null, null);
			base.SubscribeLocalEvent<SpillableComponent, SolutionSpikeOverflowEvent>(new ComponentEventHandler<SpillableComponent, SolutionSpikeOverflowEvent>(this.OnSpikeOverflow), null, null);
			base.SubscribeLocalEvent<SpillableComponent, DoAfterEvent>(new ComponentEventHandler<SpillableComponent, DoAfterEvent>(this.OnDoAfter), null, null);
		}

		// Token: 0x06001A15 RID: 6677 RVA: 0x00089647 File Offset: 0x00087847
		private void OnSpikeOverflow(EntityUid uid, SpillableComponent component, SolutionSpikeOverflowEvent args)
		{
			if (!args.Handled)
			{
				this.SpillAt(args.Overflow, base.Transform(uid).Coordinates, "PuddleSmear", true, true, true);
			}
			args.Handled = true;
		}

		// Token: 0x06001A16 RID: 6678 RVA: 0x0008967C File Offset: 0x0008787C
		private void OnGotEquipped(EntityUid uid, SpillableComponent component, GotEquippedEvent args)
		{
			if (!component.SpillWorn)
			{
				return;
			}
			ClothingComponent clothing;
			if (!base.TryComp<ClothingComponent>(uid, ref clothing))
			{
				return;
			}
			if (!clothing.Slots.HasFlag(args.SlotFlags))
			{
				return;
			}
			Solution solution;
			if (!this._solutionContainerSystem.TryGetSolution(uid, component.SolutionName, out solution, null))
			{
				return;
			}
			if (solution.Volume == 0)
			{
				return;
			}
			Solution drainedSolution = this._solutionContainerSystem.Drain(uid, solution, solution.Volume, null);
			this.SpillAt(args.Equipee, drainedSolution, "PuddleSmear", true, true, null);
		}

		// Token: 0x06001A17 RID: 6679 RVA: 0x0008970E File Offset: 0x0008790E
		[return: Nullable(2)]
		public PuddleComponent SpillAt(EntityUid uid, Solution solution, string prototype, bool sound = true, bool combine = true, [Nullable(2)] TransformComponent transformComponent = null)
		{
			if (base.Resolve<TransformComponent>(uid, ref transformComponent, false))
			{
				return this.SpillAt(solution, transformComponent.Coordinates, prototype, true, sound, combine);
			}
			return null;
		}

		// Token: 0x06001A18 RID: 6680 RVA: 0x00089734 File Offset: 0x00087934
		private void SpillOnLand(EntityUid uid, SpillableComponent component, ref LandEvent args)
		{
			Solution solution;
			if (!this._solutionContainerSystem.TryGetSolution(uid, component.SolutionName, out solution, null))
			{
				return;
			}
			DrinkComponent drink;
			if (base.TryComp<DrinkComponent>(uid, ref drink) && !drink.Opened)
			{
				return;
			}
			if (args.User != null)
			{
				ISharedAdminLogManager adminLogger = this._adminLogger;
				LogType type = LogType.Landed;
				LogStringHandler logStringHandler = new LogStringHandler(31, 2);
				logStringHandler.AppendFormatted<EntityStringRepresentation>(base.ToPrettyString(uid), "entity", "ToPrettyString(uid)");
				logStringHandler.AppendLiteral(" spilled a solution ");
				logStringHandler.AppendFormatted(SolutionContainerSystem.ToPrettyString(solution), 0, "solution");
				logStringHandler.AppendLiteral(" on landing");
				adminLogger.Add(type, ref logStringHandler);
			}
			Solution drainedSolution = this._solutionContainerSystem.Drain(uid, solution, solution.Volume, null);
			this.SpillAt(drainedSolution, this.EntityManager.GetComponent<TransformComponent>(uid).Coordinates, "PuddleSmear", true, true, true);
		}

		// Token: 0x06001A19 RID: 6681 RVA: 0x00089810 File Offset: 0x00087A10
		private void AddSpillVerb(EntityUid uid, SpillableComponent component, GetVerbsEvent<Verb> args)
		{
			if (!args.CanAccess || !args.CanInteract)
			{
				return;
			}
			Solution solution;
			if (!this._solutionContainerSystem.TryGetDrainableSolution(args.Target, out solution, null, null))
			{
				return;
			}
			DrinkComponent drink;
			if (base.TryComp<DrinkComponent>(args.Target, ref drink) && !drink.Opened)
			{
				return;
			}
			if (solution.Volume == FixedPoint2.Zero)
			{
				return;
			}
			Verb verb = new Verb();
			verb.Text = Loc.GetString("spill-target-verb-get-data-text");
			if (component.SpillDelay == null)
			{
				verb.Act = delegate()
				{
					Solution puddleSolution = this._solutionContainerSystem.SplitSolution(args.Target, solution, solution.Volume);
					this.SpillAt(puddleSolution, this.Transform(args.Target).Coordinates, "PuddleSmear", true, true, true);
				};
			}
			else
			{
				verb.Act = delegate()
				{
					SharedDoAfterSystem doAfterSystem = this._doAfterSystem;
					EntityUid user = args.User;
					float value = component.SpillDelay.Value;
					EntityUid? target = new EntityUid?(uid);
					doAfterSystem.DoAfter(new DoAfterEventArgs(user, value, default(CancellationToken), target, null)
					{
						BreakOnTargetMove = true,
						BreakOnUserMove = true,
						BreakOnDamage = true,
						BreakOnStun = true,
						NeedHand = true
					});
				};
			}
			verb.Impact = LogImpact.Medium;
			verb.DoContactInteraction = new bool?(true);
			args.Verbs.Add(verb);
		}

		// Token: 0x06001A1A RID: 6682 RVA: 0x00089924 File Offset: 0x00087B24
		[return: Nullable(2)]
		public PuddleComponent SpillAt(Solution solution, EntityCoordinates coordinates, string prototype, bool overflow = true, bool sound = true, bool combine = true)
		{
			if (solution.Volume == 0)
			{
				return null;
			}
			MapGridComponent mapGrid;
			if (!this._mapManager.TryGetGrid(coordinates.GetGridUid(this.EntityManager), ref mapGrid))
			{
				return null;
			}
			return this.SpillAt(mapGrid.GetTileRef(coordinates), solution, prototype, overflow, sound, false, combine);
		}

		// Token: 0x06001A1B RID: 6683 RVA: 0x00089978 File Offset: 0x00087B78
		[NullableContext(2)]
		public bool TryGetPuddle(TileRef tileRef, [NotNullWhen(true)] out PuddleComponent puddle)
		{
			foreach (EntityUid entity in this._entityLookup.GetEntitiesIntersecting(tileRef, 46))
			{
				PuddleComponent p;
				if (this.EntityManager.TryGetComponent<PuddleComponent>(entity, ref p))
				{
					puddle = p;
					return true;
				}
			}
			puddle = null;
			return false;
		}

		// Token: 0x06001A1C RID: 6684 RVA: 0x000899E4 File Offset: 0x00087BE4
		[return: Nullable(2)]
		public PuddleComponent SpillAt(TileRef tileRef, Solution solution, string prototype, bool overflow = true, bool sound = true, bool noTileReact = false, bool combine = true)
		{
			if (solution.Volume <= 0)
			{
				return null;
			}
			if (tileRef.Tile.IsEmpty)
			{
				return null;
			}
			EntityUid gridId = tileRef.GridUid;
			MapGridComponent mapGrid;
			if (!this._mapManager.TryGetGrid(new EntityUid?(gridId), ref mapGrid))
			{
				return null;
			}
			if (!noTileReact)
			{
				for (int i = 0; i < solution.Contents.Count; i++)
				{
					string text;
					FixedPoint2 fixedPoint;
					solution.Contents[i].Deconstruct(out text, out fixedPoint);
					string reagentId = text;
					FixedPoint2 quantity = fixedPoint;
					FixedPoint2 removed = this._prototypeManager.Index<ReagentPrototype>(reagentId).ReactionTile(tileRef, quantity);
					if (!(removed <= FixedPoint2.Zero))
					{
						solution.RemoveReagent(reagentId, removed);
					}
				}
			}
			if (solution.Volume == FixedPoint2.Zero)
			{
				return null;
			}
			EntityCoordinates spillGridCoords = mapGrid.GridTileToLocal(tileRef.GridIndices);
			EntityUid startEntity = EntityUid.Invalid;
			PuddleComponent puddleComponent = null;
			if (combine)
			{
				foreach (EntityUid spillEntity in this._entityLookup.GetEntitiesIntersecting(tileRef, 46).ToArray<EntityUid>())
				{
					if (this.EntityManager.TryGetComponent<PuddleComponent>(spillEntity, ref puddleComponent))
					{
						if (!overflow && this._puddleSystem.WouldOverflow(puddleComponent.Owner, solution, puddleComponent))
						{
							return null;
						}
						if (this._puddleSystem.TryAddSolution(puddleComponent.Owner, solution, sound, overflow, null))
						{
							startEntity = puddleComponent.Owner;
							break;
						}
					}
				}
			}
			if (startEntity != EntityUid.Invalid)
			{
				return puddleComponent;
			}
			startEntity = this.EntityManager.SpawnEntity(prototype, spillGridCoords);
			puddleComponent = this.EntityManager.EnsureComponent<PuddleComponent>(startEntity);
			this._puddleSystem.TryAddSolution(startEntity, solution, sound, overflow, null);
			return puddleComponent;
		}

		// Token: 0x06001A1D RID: 6685 RVA: 0x00089B98 File Offset: 0x00087D98
		private void OnDoAfter(EntityUid uid, SpillableComponent component, DoAfterEvent args)
		{
			if (args.Handled || args.Cancelled || args.Args.Target == null)
			{
				return;
			}
			Solution solution;
			if (!this._solutionContainerSystem.TryGetDrainableSolution(uid, out solution, null, null) || solution.Volume == 0)
			{
				return;
			}
			Solution puddleSolution = this._solutionContainerSystem.SplitSolution(uid, solution, solution.Volume);
			this.SpillAt(puddleSolution, base.Transform(uid).Coordinates, "PuddleSmear", true, true, true);
			args.Handled = true;
		}

		// Token: 0x04001064 RID: 4196
		[Dependency]
		private readonly SolutionContainerSystem _solutionContainerSystem;

		// Token: 0x04001065 RID: 4197
		[Dependency]
		private readonly PuddleSystem _puddleSystem;

		// Token: 0x04001066 RID: 4198
		[Dependency]
		private readonly IMapManager _mapManager;

		// Token: 0x04001067 RID: 4199
		[Dependency]
		private readonly IPrototypeManager _prototypeManager;

		// Token: 0x04001068 RID: 4200
		[Dependency]
		private readonly EntityLookupSystem _entityLookup;

		// Token: 0x04001069 RID: 4201
		[Dependency]
		private readonly IAdminLogManager _adminLogger;

		// Token: 0x0400106A RID: 4202
		[Dependency]
		private readonly DoAfterSystem _doAfterSystem;
	}
}
