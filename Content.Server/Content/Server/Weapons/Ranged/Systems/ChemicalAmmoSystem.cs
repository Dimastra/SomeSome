using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Content.Server.Chemistry.EntitySystems;
using Content.Server.Weapons.Ranged.Components;
using Content.Shared.Chemistry.Components;
using Content.Shared.FixedPoint;
using Content.Shared.Weapons.Ranged.Events;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;

namespace Content.Server.Weapons.Ranged.Systems
{
	// Token: 0x020000AE RID: 174
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class ChemicalAmmoSystem : EntitySystem
	{
		// Token: 0x060002B5 RID: 693 RVA: 0x0000E05C File Offset: 0x0000C25C
		public override void Initialize()
		{
			base.SubscribeLocalEvent<ChemicalAmmoComponent, AmmoShotEvent>(new ComponentEventHandler<ChemicalAmmoComponent, AmmoShotEvent>(this.OnFire), null, null);
		}

		// Token: 0x060002B6 RID: 694 RVA: 0x0000E074 File Offset: 0x0000C274
		private void OnFire(EntityUid uid, ChemicalAmmoComponent component, AmmoShotEvent args)
		{
			Solution ammoSolution;
			if (!this._solutionSystem.TryGetSolution(uid, component.SolutionName, out ammoSolution, null))
			{
				return;
			}
			List<EntityUid> firedProjectiles = args.FiredProjectiles;
			List<ValueTuple<EntityUid, Solution>> projectileSolutionContainers = new List<ValueTuple<EntityUid, Solution>>();
			foreach (EntityUid projectile in firedProjectiles)
			{
				Solution projectileSolutionContainer;
				if (this._solutionSystem.TryGetSolution(projectile, component.SolutionName, out projectileSolutionContainer, null))
				{
					projectileSolutionContainers.Add(new ValueTuple<EntityUid, Solution>(uid, projectileSolutionContainer));
				}
			}
			if (!projectileSolutionContainers.Any<ValueTuple<EntityUid, Solution>>())
			{
				return;
			}
			FixedPoint2 solutionPerProjectile = ammoSolution.Volume * (1 / projectileSolutionContainers.Count);
			foreach (ValueTuple<EntityUid, Solution> valueTuple in projectileSolutionContainers)
			{
				EntityUid projectileUid = valueTuple.Item1;
				Solution projectileSolution = valueTuple.Item2;
				Solution solutionToTransfer = this._solutionSystem.SplitSolution(uid, ammoSolution, solutionPerProjectile);
				this._solutionSystem.TryAddSolution(projectileUid, projectileSolution, solutionToTransfer);
			}
			this._solutionSystem.RemoveAllSolution(uid, ammoSolution);
		}

		// Token: 0x040001E0 RID: 480
		[Dependency]
		private readonly SolutionContainerSystem _solutionSystem;
	}
}
