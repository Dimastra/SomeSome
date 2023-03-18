using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Content.Shared.Administration.Logs;
using Content.Shared.Chemistry.Components;
using Content.Shared.Chemistry.Reagent;
using Content.Shared.Database;
using Content.Shared.FixedPoint;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Log;
using Robust.Shared.Map;
using Robust.Shared.Prototypes;
using Robust.Shared.Random;

namespace Content.Shared.Chemistry.Reaction
{
	// Token: 0x020005F4 RID: 1524
	[NullableContext(1)]
	[Nullable(0)]
	public abstract class SharedChemicalReactionSystem : EntitySystem
	{
		// Token: 0x0600127F RID: 4735 RVA: 0x0003C2DA File Offset: 0x0003A4DA
		public override void Initialize()
		{
			base.Initialize();
			this.InitializeReactionCache();
			this._prototypeManager.PrototypesReloaded += this.OnPrototypesReloaded;
		}

		// Token: 0x06001280 RID: 4736 RVA: 0x0003C2FF File Offset: 0x0003A4FF
		public override void Shutdown()
		{
			base.Shutdown();
			this._prototypeManager.PrototypesReloaded -= this.OnPrototypesReloaded;
		}

		// Token: 0x06001281 RID: 4737 RVA: 0x0003C320 File Offset: 0x0003A520
		private void InitializeReactionCache()
		{
			this._reactions = new Dictionary<string, List<ReactionPrototype>>();
			foreach (ReactionPrototype reaction in this._prototypeManager.EnumeratePrototypes<ReactionPrototype>())
			{
				this.CacheReaction(reaction);
			}
		}

		// Token: 0x06001282 RID: 4738 RVA: 0x0003C380 File Offset: 0x0003A580
		private void CacheReaction(ReactionPrototype reaction)
		{
			using (Dictionary<string, ReactantPrototype>.KeyCollection.Enumerator enumerator = reaction.Reactants.Keys.GetEnumerator())
			{
				if (enumerator.MoveNext())
				{
					string reagent = enumerator.Current;
					List<ReactionPrototype> cache;
					if (!this._reactions.TryGetValue(reagent, out cache))
					{
						cache = new List<ReactionPrototype>();
						this._reactions.Add(reagent, cache);
					}
					cache.Add(reaction);
				}
			}
		}

		// Token: 0x06001283 RID: 4739 RVA: 0x0003C400 File Offset: 0x0003A600
		private void OnPrototypesReloaded(PrototypesReloadedEventArgs eventArgs)
		{
			PrototypesReloadedEventArgs.PrototypeChangeSet set;
			if (!eventArgs.ByType.TryGetValue(typeof(ReactionPrototype), out set))
			{
				return;
			}
			Predicate<ReactionPrototype> <>9__0;
			foreach (KeyValuePair<string, List<ReactionPrototype>> keyValuePair in this._reactions)
			{
				string text;
				List<ReactionPrototype> list;
				keyValuePair.Deconstruct(out text, out list);
				string reactant = text;
				List<ReactionPrototype> list2 = list;
				Predicate<ReactionPrototype> match;
				if ((match = <>9__0) == null)
				{
					match = (<>9__0 = ((ReactionPrototype reaction) => set.Modified.ContainsKey(reaction.ID)));
				}
				list2.RemoveAll(match);
				if (list2.Count == 0)
				{
					this._reactions.Remove(reactant);
				}
			}
			foreach (IPrototype prototype in set.Modified.Values)
			{
				this.CacheReaction((ReactionPrototype)prototype);
			}
		}

		// Token: 0x06001284 RID: 4740 RVA: 0x0003C508 File Offset: 0x0003A708
		private bool CanReact(Solution solution, ReactionPrototype reaction, EntityUid owner, [Nullable(2)] ReactionMixerComponent mixerComponent, out FixedPoint2 lowestUnitReactions)
		{
			lowestUnitReactions = FixedPoint2.MaxValue;
			if (solution.Temperature < reaction.MinimumTemperature)
			{
				lowestUnitReactions = FixedPoint2.Zero;
				return false;
			}
			if (solution.Temperature > reaction.MaximumTemperature)
			{
				lowestUnitReactions = FixedPoint2.Zero;
				return false;
			}
			if ((mixerComponent == null && reaction.MixingCategories != null) || (mixerComponent != null && reaction.MixingCategories != null && reaction.MixingCategories.Except(mixerComponent.ReactionTypes).Any<string>()))
			{
				lowestUnitReactions = FixedPoint2.Zero;
				return false;
			}
			ReactionAttemptEvent attempt = new ReactionAttemptEvent(reaction, solution);
			base.RaiseLocalEvent<ReactionAttemptEvent>(owner, attempt, false);
			if (attempt.Cancelled)
			{
				lowestUnitReactions = FixedPoint2.Zero;
				return false;
			}
			foreach (KeyValuePair<string, ReactantPrototype> reactantData in reaction.Reactants)
			{
				string reactantName = reactantData.Key;
				FixedPoint2 reactantCoefficient = reactantData.Value.Amount;
				FixedPoint2 reactantQuantity;
				if (!solution.TryGetReagent(reactantName, out reactantQuantity))
				{
					return false;
				}
				if (reactantData.Value.Catalyst)
				{
					if (reactantQuantity == FixedPoint2.Zero || (reaction.Quantized && reactantQuantity < reactantCoefficient))
					{
						return false;
					}
				}
				else
				{
					FixedPoint2 unitReactions = reactantQuantity / reactantCoefficient;
					if (unitReactions < lowestUnitReactions)
					{
						lowestUnitReactions = unitReactions;
					}
				}
			}
			if (reaction.Quantized)
			{
				lowestUnitReactions = (int)lowestUnitReactions;
			}
			return lowestUnitReactions > 0;
		}

		// Token: 0x06001285 RID: 4741 RVA: 0x0003C6B8 File Offset: 0x0003A8B8
		private Solution PerformReaction(Solution solution, EntityUid owner, ReactionPrototype reaction, FixedPoint2 unitReactions)
		{
			ReagentPrototype randomReagent = this._prototypeManager.Index<ReagentPrototype>(RandomExtensions.Pick<KeyValuePair<string, ReactantPrototype>>(this._random, reaction.Reactants).Key);
			foreach (KeyValuePair<string, ReactantPrototype> reactant in reaction.Reactants)
			{
				if (!reactant.Value.Catalyst)
				{
					FixedPoint2 amountToRemove = unitReactions * reactant.Value.Amount;
					solution.RemoveReagent(reactant.Key, amountToRemove);
				}
			}
			Solution products = new Solution();
			foreach (KeyValuePair<string, FixedPoint2> product in reaction.Products)
			{
				products.AddReagent(product.Key, product.Value * unitReactions, true);
			}
			this.OnReaction(solution, reaction, randomReagent, owner, unitReactions);
			return products;
		}

		// Token: 0x06001286 RID: 4742 RVA: 0x0003C7CC File Offset: 0x0003A9CC
		protected virtual void OnReaction(Solution solution, ReactionPrototype reaction, ReagentPrototype randomReagent, EntityUid owner, FixedPoint2 unitReactions)
		{
			ReagentEffectArgs args = new ReagentEffectArgs(owner, null, solution, randomReagent, unitReactions, this.EntityManager, null, 1f);
			foreach (ReagentEffect effect in reaction.Effects)
			{
				if (effect.ShouldApply(args, null))
				{
					if (effect.ShouldLog)
					{
						EntityUid entity = args.SolutionEntity;
						ISharedAdminLogManager adminLogger = this.AdminLogger;
						LogType type = LogType.ReagentEffect;
						LogImpact logImpact = effect.LogImpact;
						LogStringHandler logStringHandler = new LogStringHandler(53, 4);
						logStringHandler.AppendLiteral("Reaction effect ");
						logStringHandler.AppendFormatted(effect.GetType().Name, 0, "effect");
						logStringHandler.AppendLiteral(" of reaction $");
						logStringHandler.AppendFormatted(reaction.ID, 0, "reaction");
						logStringHandler.AppendLiteral(" applied on entity ");
						logStringHandler.AppendFormatted<EntityStringRepresentation>(base.ToPrettyString(entity), "entity", "ToPrettyString(entity)");
						logStringHandler.AppendLiteral(" at ");
						logStringHandler.AppendFormatted<EntityCoordinates>(base.Transform(entity).Coordinates, "coordinates", "Transform(entity).Coordinates");
						adminLogger.Add(type, logImpact, ref logStringHandler);
					}
					effect.Effect(args);
				}
			}
		}

		// Token: 0x06001287 RID: 4743 RVA: 0x0003C928 File Offset: 0x0003AB28
		private bool ProcessReactions(Solution solution, EntityUid owner, FixedPoint2 maxVolume, SortedSet<ReactionPrototype> reactions, [Nullable(2)] ReactionMixerComponent mixerComponent)
		{
			HashSet<ReactionPrototype> toRemove = new HashSet<ReactionPrototype>();
			Solution products = null;
			foreach (ReactionPrototype reaction in reactions)
			{
				FixedPoint2 unitReactions;
				if (this.CanReact(solution, reaction, owner, mixerComponent, out unitReactions))
				{
					products = this.PerformReaction(solution, owner, reaction, unitReactions);
					break;
				}
				toRemove.Add(reaction);
			}
			if (products == null)
			{
				return false;
			}
			reactions.Except(toRemove);
			if (products.Volume <= 0)
			{
				return true;
			}
			FixedPoint2 excessVolume = solution.Volume + products.Volume - maxVolume;
			if (excessVolume > 0)
			{
				products.RemoveSolution(excessVolume);
			}
			foreach (Solution.ReagentQuantity reactant in products.Contents)
			{
				List<ReactionPrototype> reactantReactions;
				if (this._reactions.TryGetValue(reactant.ReagentId, out reactantReactions))
				{
					reactions.UnionWith(reactantReactions);
				}
			}
			solution.AddSolution(products, this._prototypeManager);
			return true;
		}

		// Token: 0x06001288 RID: 4744 RVA: 0x0003CA54 File Offset: 0x0003AC54
		public void FullyReactSolution(Solution solution, EntityUid owner)
		{
			this.FullyReactSolution(solution, owner, FixedPoint2.MaxValue, null);
		}

		// Token: 0x06001289 RID: 4745 RVA: 0x0003CA64 File Offset: 0x0003AC64
		public void FullyReactSolution(Solution solution, EntityUid owner, FixedPoint2 maxVolume, [Nullable(2)] ReactionMixerComponent mixerComponent)
		{
			SortedSet<ReactionPrototype> reactions = new SortedSet<ReactionPrototype>();
			foreach (Solution.ReagentQuantity reactant in solution.Contents)
			{
				List<ReactionPrototype> reactantReactions;
				if (this._reactions.TryGetValue(reactant.ReagentId, out reactantReactions))
				{
					reactions.UnionWith(reactantReactions);
				}
			}
			for (int i = 0; i < 20; i++)
			{
				if (!this.ProcessReactions(solution, owner, maxVolume, reactions, mixerComponent))
				{
					return;
				}
			}
			DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(44, 3);
			defaultInterpolatedStringHandler.AppendFormatted("Solution");
			defaultInterpolatedStringHandler.AppendLiteral(" ");
			defaultInterpolatedStringHandler.AppendFormatted<EntityUid>(owner);
			defaultInterpolatedStringHandler.AppendLiteral(" could not finish reacting in under ");
			defaultInterpolatedStringHandler.AppendFormatted<int>(20);
			defaultInterpolatedStringHandler.AppendLiteral(" loops.");
			Logger.Error(defaultInterpolatedStringHandler.ToStringAndClear());
		}

		// Token: 0x04001147 RID: 4423
		private const int MaxReactionIterations = 20;

		// Token: 0x04001148 RID: 4424
		[Dependency]
		private readonly IPrototypeManager _prototypeManager;

		// Token: 0x04001149 RID: 4425
		[Dependency]
		private readonly IRobustRandom _random;

		// Token: 0x0400114A RID: 4426
		[Dependency]
		protected readonly ISharedAdminLogManager AdminLogger;

		// Token: 0x0400114B RID: 4427
		private IDictionary<string, List<ReactionPrototype>> _reactions;
	}
}
