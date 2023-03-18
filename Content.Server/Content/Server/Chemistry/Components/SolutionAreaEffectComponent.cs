using System;
using System.Linq;
using System.Runtime.CompilerServices;
using Content.Server.Atmos.Components;
using Content.Server.Chemistry.EntitySystems;
using Content.Shared.Chemistry;
using Content.Shared.Chemistry.Components;
using Content.Shared.Chemistry.Reagent;
using Content.Shared.FixedPoint;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Log;
using Robust.Shared.Map;
using Robust.Shared.Map.Components;
using Robust.Shared.Maths;
using Robust.Shared.Prototypes;

namespace Content.Server.Chemistry.Components
{
	// Token: 0x020006AE RID: 1710
	[NullableContext(1)]
	[Nullable(0)]
	public abstract class SolutionAreaEffectComponent : Component
	{
		// Token: 0x17000548 RID: 1352
		// (get) Token: 0x0600239A RID: 9114 RVA: 0x000B9D66 File Offset: 0x000B7F66
		// (set) Token: 0x0600239B RID: 9115 RVA: 0x000B9D6E File Offset: 0x000B7F6E
		public int Amount { get; set; }

		// Token: 0x17000549 RID: 1353
		// (get) Token: 0x0600239C RID: 9116 RVA: 0x000B9D77 File Offset: 0x000B7F77
		// (set) Token: 0x0600239D RID: 9117 RVA: 0x000B9D7F File Offset: 0x000B7F7F
		[Nullable(2)]
		public SolutionAreaEffectInceptionComponent Inception { [NullableContext(2)] get; [NullableContext(2)] set; }

		// Token: 0x0600239E RID: 9118 RVA: 0x000B9D88 File Offset: 0x000B7F88
		public void Start(int amount, float duration, float spreadDelay, float removeDelay)
		{
			if (this.Inception != null)
			{
				return;
			}
			if (this._entities.HasComponent<SolutionAreaEffectInceptionComponent>(base.Owner))
			{
				return;
			}
			this.Amount = amount;
			SolutionAreaEffectInceptionComponent solutionAreaEffectInceptionComponent = this._entities.AddComponent<SolutionAreaEffectInceptionComponent>(base.Owner);
			solutionAreaEffectInceptionComponent.Add(this);
			solutionAreaEffectInceptionComponent.Setup(amount, duration, spreadDelay, removeDelay);
		}

		// Token: 0x0600239F RID: 9119 RVA: 0x000B9DDC File Offset: 0x000B7FDC
		public void Spread()
		{
			SolutionAreaEffectComponent.<>c__DisplayClass15_0 CS$<>8__locals1;
			CS$<>8__locals1.<>4__this = this;
			CS$<>8__locals1.meta = this._entities.GetComponent<MetaDataComponent>(base.Owner);
			if (CS$<>8__locals1.meta.EntityPrototype == null)
			{
				Logger.Error("AreaEffectComponent needs its owner to be spawned by a prototype.");
				return;
			}
			TransformComponent xform = this._entities.GetComponent<TransformComponent>(base.Owner);
			CS$<>8__locals1.solSys = this._systems.GetEntitySystem<SolutionContainerSystem>();
			if (!this._entities.TryGetComponent<MapGridComponent>(xform.GridUid, ref CS$<>8__locals1.gridComp))
			{
				return;
			}
			CS$<>8__locals1.origin = CS$<>8__locals1.gridComp.TileIndicesFor(xform.Coordinates);
			this.<Spread>g__SpreadToDir|15_0(4, ref CS$<>8__locals1);
			this.<Spread>g__SpreadToDir|15_0(2, ref CS$<>8__locals1);
			this.<Spread>g__SpreadToDir|15_0(0, ref CS$<>8__locals1);
			this.<Spread>g__SpreadToDir|15_0(6, ref CS$<>8__locals1);
		}

		// Token: 0x060023A0 RID: 9120 RVA: 0x000B9E9C File Offset: 0x000B809C
		public void Kill()
		{
			SolutionAreaEffectInceptionComponent inception = this.Inception;
			if (inception != null)
			{
				inception.Remove(this);
			}
			this.OnKill();
		}

		// Token: 0x060023A1 RID: 9121
		protected abstract void OnKill();

		// Token: 0x060023A2 RID: 9122 RVA: 0x000B9EB8 File Offset: 0x000B80B8
		public void React(float averageExposures)
		{
			Solution solution;
			if (!this._entities.EntitySysManager.GetEntitySystem<SolutionContainerSystem>().TryGetSolution(base.Owner, "solutionArea", out solution, null) || solution.Contents.Count == 0)
			{
				return;
			}
			TransformComponent xform = this._entities.GetComponent<TransformComponent>(base.Owner);
			MapGridComponent mapGrid;
			if (!this.MapManager.TryGetGrid(xform.GridUid, ref mapGrid))
			{
				return;
			}
			TileRef tile = mapGrid.GetTileRef(xform.Coordinates.ToVector2i(this._entities, this.MapManager));
			ReactiveSystem chemistry = this._entities.EntitySysManager.GetEntitySystem<ReactiveSystem>();
			EntityLookupSystem entitySystem = this._entities.EntitySysManager.GetEntitySystem<EntityLookupSystem>();
			double solutionFraction = 1.0 / Math.Floor((double)averageExposures);
			EntityUid[] ents = entitySystem.GetEntitiesIntersecting(tile, 14).ToArray<EntityUid>();
			foreach (Solution.ReagentQuantity reagentQuantity in solution.Contents.ToArray())
			{
				if (!(reagentQuantity.Quantity == FixedPoint2.Zero))
				{
					ReagentPrototype reagent = this.PrototypeManager.Index<ReagentPrototype>(reagentQuantity.ReagentId);
					if (!this.ReactedTile)
					{
						reagent.ReactionTile(tile, reagentQuantity.Quantity);
						this.ReactedTile = true;
					}
					foreach (EntityUid entity in ents)
					{
						chemistry.ReactionEntity(entity, ReactionMethod.Touch, reagent, reagentQuantity.Quantity * solutionFraction, solution);
					}
				}
			}
			foreach (EntityUid entity2 in ents)
			{
				this.ReactWithEntity(entity2, solutionFraction);
			}
		}

		// Token: 0x060023A3 RID: 9123
		protected abstract void ReactWithEntity(EntityUid entity, double solutionFraction);

		// Token: 0x060023A4 RID: 9124 RVA: 0x000BA068 File Offset: 0x000B8268
		public void TryAddSolution(Solution solution)
		{
			if (solution.Volume == 0)
			{
				return;
			}
			Solution solutionArea;
			if (!EntitySystem.Get<SolutionContainerSystem>().TryGetSolution(base.Owner, "solutionArea", out solutionArea, null))
			{
				return;
			}
			Solution addSolution = solution.SplitSolution(FixedPoint2.Min(solution.Volume, solutionArea.AvailableVolume));
			EntitySystem.Get<SolutionContainerSystem>().TryAddSolution(base.Owner, solutionArea, addSolution);
			this.UpdateVisuals();
		}

		// Token: 0x060023A5 RID: 9125
		protected abstract void UpdateVisuals();

		// Token: 0x060023A6 RID: 9126 RVA: 0x000BA0D0 File Offset: 0x000B82D0
		protected override void OnRemove()
		{
			base.OnRemove();
			SolutionAreaEffectInceptionComponent inception = this.Inception;
			if (inception == null)
			{
				return;
			}
			inception.Remove(this);
		}

		// Token: 0x060023A8 RID: 9128 RVA: 0x000BA0F4 File Offset: 0x000B82F4
		[CompilerGenerated]
		private void <Spread>g__SpreadToDir|15_0(Direction dir, ref SolutionAreaEffectComponent.<>c__DisplayClass15_0 A_2)
		{
			Vector2i index = A_2.origin + DirectionExtensions.ToIntVec(dir);
			TileRef tile;
			if (!A_2.gridComp.TryGetTileRef(index, ref tile) || tile.Tile.IsEmpty)
			{
				return;
			}
			foreach (EntityUid neighbor in A_2.gridComp.GetAnchoredEntities(index))
			{
				SolutionAreaEffectComponent comp;
				if (this._entities.TryGetComponent<SolutionAreaEffectComponent>(neighbor, ref comp) && comp.Inception == this.Inception)
				{
					return;
				}
				AirtightComponent airtight;
				if (this._entities.TryGetComponent<AirtightComponent>(neighbor, ref airtight) && airtight.AirBlocked)
				{
					return;
				}
			}
			EntityUid newEffect = this._entities.SpawnEntity(A_2.meta.EntityPrototype.ID, A_2.gridComp.GridTileToLocal(index));
			SolutionAreaEffectComponent effectComponent;
			if (!this._entities.TryGetComponent<SolutionAreaEffectComponent>(newEffect, ref effectComponent))
			{
				this._entities.DeleteEntity(newEffect);
				return;
			}
			Solution solution;
			if (A_2.solSys.TryGetSolution(base.Owner, "solutionArea", out solution, null))
			{
				effectComponent.TryAddSolution(solution.Clone());
			}
			effectComponent.Amount = this.Amount - 1;
			SolutionAreaEffectInceptionComponent inception = this.Inception;
			if (inception == null)
			{
				return;
			}
			inception.Add(effectComponent);
		}

		// Token: 0x040015FD RID: 5629
		public const string SolutionName = "solutionArea";

		// Token: 0x040015FE RID: 5630
		[Dependency]
		protected readonly IMapManager MapManager;

		// Token: 0x040015FF RID: 5631
		[Dependency]
		protected readonly IPrototypeManager PrototypeManager;

		// Token: 0x04001600 RID: 5632
		[Dependency]
		private readonly IEntityManager _entities;

		// Token: 0x04001601 RID: 5633
		[Dependency]
		private readonly IEntitySystemManager _systems;

		// Token: 0x04001604 RID: 5636
		public bool ReactedTile;
	}
}
