using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Content.Shared.Administration.Logs;
using Content.Shared.Chemistry.Components;
using Content.Shared.Chemistry.Reaction;
using Content.Shared.Chemistry.Reagent;
using Content.Shared.Database;
using Content.Shared.FixedPoint;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Map;
using Robust.Shared.Prototypes;
using Robust.Shared.Random;

namespace Content.Shared.Chemistry
{
	// Token: 0x020005C9 RID: 1481
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class ReactiveSystem : EntitySystem
	{
		// Token: 0x060011F8 RID: 4600 RVA: 0x0003AE10 File Offset: 0x00039010
		public void ReactionEntity(EntityUid uid, ReactionMethod method, Solution solution)
		{
			foreach (Solution.ReagentQuantity reagentQuantity in solution)
			{
				string text;
				FixedPoint2 fixedPoint;
				reagentQuantity.Deconstruct(out text, out fixedPoint);
				string id = text;
				FixedPoint2 quantity = fixedPoint;
				this.ReactionEntity(uid, method, id, quantity, solution);
			}
		}

		// Token: 0x060011F9 RID: 4601 RVA: 0x0003AE70 File Offset: 0x00039070
		public void DoEntityReaction(EntityUid uid, Solution solution, ReactionMethod method)
		{
			foreach (Solution.ReagentQuantity reagentQuantity in solution.Contents.ToArray())
			{
				string text;
				FixedPoint2 fixedPoint;
				reagentQuantity.Deconstruct(out text, out fixedPoint);
				string reagentId = text;
				FixedPoint2 quantity = fixedPoint;
				this.ReactionEntity(uid, method, reagentId, quantity, solution);
			}
		}

		// Token: 0x060011FA RID: 4602 RVA: 0x0003AEBC File Offset: 0x000390BC
		public void ReactionEntity(EntityUid uid, ReactionMethod method, string reagentId, FixedPoint2 reactVolume, [Nullable(2)] Solution source)
		{
			this.ReactionEntity(uid, method, this._prototypeManager.Index<ReagentPrototype>(reagentId), reactVolume, source);
		}

		// Token: 0x060011FB RID: 4603 RVA: 0x0003AED8 File Offset: 0x000390D8
		public void ReactionEntity(EntityUid uid, ReactionMethod method, ReagentPrototype reagent, FixedPoint2 reactVolume, [Nullable(2)] Solution source)
		{
			ReactiveComponent reactive;
			if (!this.EntityManager.TryGetComponent<ReactiveComponent>(uid, ref reactive))
			{
				return;
			}
			ReagentEffectArgs args = new ReagentEffectArgs(uid, null, source, reagent, (source != null) ? source.GetReagentQuantity(reagent.ID) : reactVolume, this.EntityManager, new ReactionMethod?(method), 1f);
			if (reagent.ReactiveEffects != null && reactive.ReactiveGroups != null)
			{
				foreach (KeyValuePair<string, Content.Shared.Chemistry.Reagent.ReactiveReagentEffectEntry> keyValuePair in reagent.ReactiveEffects)
				{
					string text;
					Content.Shared.Chemistry.Reagent.ReactiveReagentEffectEntry reactiveReagentEffectEntry;
					keyValuePair.Deconstruct(out text, out reactiveReagentEffectEntry);
					string key = text;
					Content.Shared.Chemistry.Reagent.ReactiveReagentEffectEntry val = reactiveReagentEffectEntry;
					if (val.Methods.Contains(method) && reactive.ReactiveGroups.ContainsKey(key) && reactive.ReactiveGroups[key].Contains(method))
					{
						foreach (ReagentEffect effect in val.Effects)
						{
							if (effect.ShouldApply(args, this._robustRandom))
							{
								if (effect.ShouldLog)
								{
									EntityUid entity = args.SolutionEntity;
									ISharedAdminLogManager adminLogger = this._adminLogger;
									LogType type = LogType.ReagentEffect;
									LogImpact logImpact = effect.LogImpact;
									LogStringHandler logStringHandler = new LogStringHandler(64, 5);
									logStringHandler.AppendLiteral("Reactive effect ");
									logStringHandler.AppendFormatted(effect.GetType().Name, 0, "effect");
									logStringHandler.AppendLiteral(" of reagent ");
									logStringHandler.AppendFormatted(reagent.ID, 0, "reagent");
									logStringHandler.AppendLiteral(" with method ");
									logStringHandler.AppendFormatted<ReactionMethod>(method, "method");
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
				}
			}
			if (reactive.Reactions != null)
			{
				foreach (Content.Shared.Chemistry.Reaction.ReactiveReagentEffectEntry entry in reactive.Reactions)
				{
					if (entry.Methods.Contains(method) && (entry.Reagents == null || entry.Reagents.Contains(reagent.ID)))
					{
						foreach (ReagentEffect effect2 in entry.Effects)
						{
							if (effect2.ShouldApply(args, this._robustRandom))
							{
								if (effect2.ShouldLog)
								{
									EntityUid entity2 = args.SolutionEntity;
									ISharedAdminLogManager adminLogger2 = this._adminLogger;
									LogType type2 = LogType.ReagentEffect;
									LogImpact logImpact2 = effect2.LogImpact;
									LogStringHandler logStringHandler = new LogStringHandler(52, 5);
									logStringHandler.AppendLiteral("Reactive effect ");
									logStringHandler.AppendFormatted(effect2.GetType().Name, 0, "effect");
									logStringHandler.AppendLiteral(" of ");
									logStringHandler.AppendFormatted<EntityStringRepresentation>(base.ToPrettyString(entity2), "entity", "ToPrettyString(entity)");
									logStringHandler.AppendLiteral(" using reagent ");
									logStringHandler.AppendFormatted(reagent.ID, 0, "reagent");
									logStringHandler.AppendLiteral(" with method ");
									logStringHandler.AppendFormatted<ReactionMethod>(method, "method");
									logStringHandler.AppendLiteral(" at ");
									logStringHandler.AppendFormatted<EntityCoordinates>(base.Transform(entity2).Coordinates, "coordinates", "Transform(entity).Coordinates");
									adminLogger2.Add(type2, logImpact2, ref logStringHandler);
								}
								effect2.Effect(args);
							}
						}
					}
				}
			}
		}

		// Token: 0x040010B9 RID: 4281
		[Dependency]
		private readonly IPrototypeManager _prototypeManager;

		// Token: 0x040010BA RID: 4282
		[Dependency]
		private readonly IRobustRandom _robustRandom;

		// Token: 0x040010BB RID: 4283
		[Dependency]
		private readonly ISharedAdminLogManager _adminLogger;
	}
}
