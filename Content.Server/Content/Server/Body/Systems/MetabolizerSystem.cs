using System;
using System.Runtime.CompilerServices;
using Content.Server.Body.Components;
using Content.Server.Chemistry.Components.SolutionManager;
using Content.Server.Chemistry.EntitySystems;
using Content.Shared.Administration.Logs;
using Content.Shared.Body.Organ;
using Content.Shared.Chemistry.Components;
using Content.Shared.Chemistry.Reagent;
using Content.Shared.Database;
using Content.Shared.FixedPoint;
using Content.Shared.Mobs.Components;
using Content.Shared.Mobs.Systems;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Map;
using Robust.Shared.Prototypes;
using Robust.Shared.Random;

namespace Content.Server.Body.Systems
{
	// Token: 0x0200070A RID: 1802
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class MetabolizerSystem : EntitySystem
	{
		// Token: 0x060025FC RID: 9724 RVA: 0x000C8403 File Offset: 0x000C6603
		public override void Initialize()
		{
			base.Initialize();
			base.SubscribeLocalEvent<MetabolizerComponent, ComponentInit>(new ComponentEventHandler<MetabolizerComponent, ComponentInit>(this.OnMetabolizerInit), null, null);
			base.SubscribeLocalEvent<MetabolizerComponent, ApplyMetabolicMultiplierEvent>(new ComponentEventHandler<MetabolizerComponent, ApplyMetabolicMultiplierEvent>(this.OnApplyMetabolicMultiplier), null, null);
		}

		// Token: 0x060025FD RID: 9725 RVA: 0x000C8434 File Offset: 0x000C6634
		private void OnMetabolizerInit(EntityUid uid, MetabolizerComponent component, ComponentInit args)
		{
			if (!component.SolutionOnBody)
			{
				this._solutionContainerSystem.EnsureSolution(uid, component.SolutionName, null);
				return;
			}
			OrganComponent organComponent = base.CompOrNull<OrganComponent>(uid);
			EntityUid? entityUid = (organComponent != null) ? organComponent.Body : null;
			if (entityUid != null)
			{
				EntityUid body = entityUid.GetValueOrDefault();
				this._solutionContainerSystem.EnsureSolution(body, component.SolutionName, null);
			}
		}

		// Token: 0x060025FE RID: 9726 RVA: 0x000C84A0 File Offset: 0x000C66A0
		private void OnApplyMetabolicMultiplier(EntityUid uid, MetabolizerComponent component, ApplyMetabolicMultiplierEvent args)
		{
			if (args.Apply)
			{
				component.UpdateFrequency *= args.Multiplier;
				return;
			}
			component.UpdateFrequency /= args.Multiplier;
			if (component.AccumulatedFrametime >= component.UpdateFrequency)
			{
				component.AccumulatedFrametime = component.UpdateFrequency;
			}
		}

		// Token: 0x060025FF RID: 9727 RVA: 0x000C84F8 File Offset: 0x000C66F8
		public override void Update(float frameTime)
		{
			base.Update(frameTime);
			foreach (MetabolizerComponent metab in this.EntityManager.EntityQuery<MetabolizerComponent>(false))
			{
				metab.AccumulatedFrametime += frameTime;
				if (metab.AccumulatedFrametime >= metab.UpdateFrequency)
				{
					metab.AccumulatedFrametime -= metab.UpdateFrequency;
					this.TryMetabolize(metab.Owner, metab, null);
				}
			}
		}

		// Token: 0x06002600 RID: 9728 RVA: 0x000C8588 File Offset: 0x000C6788
		[NullableContext(2)]
		private void TryMetabolize(EntityUid uid, MetabolizerComponent meta = null, OrganComponent organ = null)
		{
			if (!base.Resolve<MetabolizerComponent>(uid, ref meta, true))
			{
				return;
			}
			base.Resolve<OrganComponent>(uid, ref organ, false);
			Solution solution = null;
			EntityUid? solutionEntityUid = null;
			SolutionContainerManagerComponent manager = null;
			if (meta.SolutionOnBody)
			{
				EntityUid? entityUid = (organ != null) ? organ.Body : null;
				if (entityUid != null)
				{
					EntityUid body = entityUid.GetValueOrDefault();
					if (!base.Resolve<SolutionContainerManagerComponent>(body, ref manager, false))
					{
						return;
					}
					this._solutionContainerSystem.TryGetSolution(body, meta.SolutionName, out solution, manager);
					solutionEntityUid = new EntityUid?(body);
				}
			}
			else
			{
				if (!base.Resolve<SolutionContainerManagerComponent>(uid, ref manager, false))
				{
					return;
				}
				this._solutionContainerSystem.TryGetSolution(uid, meta.SolutionName, out solution, manager);
				solutionEntityUid = new EntityUid?(uid);
			}
			if (solutionEntityUid == null || solution == null || solution.Contents.Count == 0)
			{
				return;
			}
			Solution.ReagentQuantity[] list = solution.Contents.ToArray();
			this._random.Shuffle<Solution.ReagentQuantity>(list);
			int reagents = 0;
			foreach (Solution.ReagentQuantity reagent in list)
			{
				ReagentPrototype proto;
				if (this._prototypeManager.TryIndex<ReagentPrototype>(reagent.ReagentId, ref proto))
				{
					FixedPoint2 mostToRemove = FixedPoint2.Zero;
					if (proto.Metabolisms == null)
					{
						if (meta.RemoveEmpty)
						{
							this._solutionContainerSystem.TryRemoveReagent(solutionEntityUid.Value, solution, reagent.ReagentId, FixedPoint2.New(1));
						}
					}
					else
					{
						if (reagents >= meta.MaxReagentsProcessable)
						{
							return;
						}
						reagents++;
						if (meta.MetabolismGroups != null)
						{
							foreach (MetabolismGroupEntry group in meta.MetabolismGroups)
							{
								if (proto.Metabolisms.ContainsKey(group.Id))
								{
									ReagentEffectsEntry entry = proto.Metabolisms[group.Id];
									if (entry.MetabolismRate > mostToRemove)
									{
										mostToRemove = entry.MetabolismRate;
									}
									mostToRemove *= group.MetabolismRateModifier;
									mostToRemove = FixedPoint2.Clamp(mostToRemove, 0, reagent.Quantity);
									float scale = (float)mostToRemove / (float)entry.MetabolismRate;
									MobStateComponent state;
									if (!this.EntityManager.TryGetComponent<MobStateComponent>(solutionEntityUid.Value, ref state) || !this._mobStateSystem.IsDead(solutionEntityUid.Value, state))
									{
										EntityUid actualEntity = ((organ != null) ? organ.Body : null) ?? solutionEntityUid.Value;
										ReagentEffectArgs args = new ReagentEffectArgs(actualEntity, new EntityUid?(meta.Owner), solution, proto, mostToRemove, this.EntityManager, null, scale);
										foreach (ReagentEffect effect in entry.Effects)
										{
											if (effect.ShouldApply(args, this._random))
											{
												if (effect.ShouldLog)
												{
													ISharedAdminLogManager adminLogger = this._adminLogger;
													LogType type = LogType.ReagentEffect;
													LogImpact logImpact = effect.LogImpact;
													LogStringHandler logStringHandler = new LogStringHandler(53, 4);
													logStringHandler.AppendLiteral("Metabolism effect ");
													logStringHandler.AppendFormatted(effect.GetType().Name, 0, "effect");
													logStringHandler.AppendLiteral(" of reagent ");
													logStringHandler.AppendFormatted(args.Reagent.LocalizedName, 0, "reagent");
													logStringHandler.AppendLiteral(" applied on entity ");
													logStringHandler.AppendFormatted<EntityUid>(actualEntity, "entity", "actualEntity");
													logStringHandler.AppendLiteral(" at ");
													logStringHandler.AppendFormatted<EntityCoordinates>(base.Transform(actualEntity).Coordinates, "coordinates", "Transform(actualEntity).Coordinates");
													adminLogger.Add(type, logImpact, ref logStringHandler);
												}
												effect.Effect(args);
											}
										}
									}
								}
							}
							if (mostToRemove > FixedPoint2.Zero)
							{
								this._solutionContainerSystem.TryRemoveReagent(solutionEntityUid.Value, solution, reagent.ReagentId, mostToRemove);
							}
						}
					}
				}
			}
		}

		// Token: 0x04001770 RID: 6000
		[Dependency]
		private readonly BodySystem _bodySystem;

		// Token: 0x04001771 RID: 6001
		[Dependency]
		private readonly SolutionContainerSystem _solutionContainerSystem;

		// Token: 0x04001772 RID: 6002
		[Dependency]
		private readonly IPrototypeManager _prototypeManager;

		// Token: 0x04001773 RID: 6003
		[Dependency]
		private readonly IRobustRandom _random;

		// Token: 0x04001774 RID: 6004
		[Dependency]
		private readonly MobStateSystem _mobStateSystem;

		// Token: 0x04001775 RID: 6005
		[Dependency]
		private readonly ISharedAdminLogManager _adminLogger;
	}
}
