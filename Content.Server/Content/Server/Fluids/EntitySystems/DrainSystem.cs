using System;
using System.Runtime.CompilerServices;
using Content.Server.Chemistry.Components.SolutionManager;
using Content.Server.Chemistry.EntitySystems;
using Content.Server.Fluids.Components;
using Content.Shared.Audio;
using Content.Shared.Chemistry.Components;
using Content.Shared.FixedPoint;
using Robust.Shared.Collections;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;

namespace Content.Server.Fluids.EntitySystems
{
	// Token: 0x020004EB RID: 1259
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class DrainSystem : EntitySystem
	{
		// Token: 0x060019EA RID: 6634 RVA: 0x00087C8C File Offset: 0x00085E8C
		public override void Update(float frameTime)
		{
			base.Update(frameTime);
			EntityQuery<SolutionContainerManagerComponent> managerQuery = base.GetEntityQuery<SolutionContainerManagerComponent>();
			EntityQuery<TransformComponent> xformQuery = base.GetEntityQuery<TransformComponent>();
			EntityQuery<PuddleComponent> puddleQuery = base.GetEntityQuery<PuddleComponent>();
			ValueList<ValueTuple<EntityUid, string>> puddles = default(ValueList<ValueTuple<EntityUid, string>>);
			foreach (DrainComponent drain in base.EntityQuery<DrainComponent>(false))
			{
				drain.Accumulator += frameTime;
				if (drain.Accumulator >= drain.DrainFrequency)
				{
					drain.Accumulator -= drain.DrainFrequency;
					SolutionContainerManagerComponent manager;
					if (managerQuery.TryGetComponent(drain.Owner, ref manager))
					{
						Solution drainSolution;
						this._solutionSystem.TryGetSolution(drain.Owner, "drainBuffer", out drainSolution, manager);
						if (drainSolution != null)
						{
							this._solutionSystem.SplitSolution(drain.Owner, drainSolution, drain.UnitsDestroyedPerSecond * drain.DrainFrequency);
							float amount = drain.UnitsPerSecond * drain.DrainFrequency;
							TransformComponent xform;
							if (xformQuery.TryGetComponent(drain.Owner, ref xform))
							{
								puddles.Clear();
								foreach (EntityUid entity in this._lookup.GetEntitiesInRange(xform.MapPosition, drain.Range, 46))
								{
									PuddleComponent puddle;
									if (puddleQuery.TryGetComponent(entity, ref puddle))
									{
										puddles.Add(new ValueTuple<EntityUid, string>(entity, puddle.SolutionName));
									}
								}
								if (puddles.Count == 0)
								{
									this._ambientSoundSystem.SetAmbience(drain.Owner, false, null);
								}
								else
								{
									this._ambientSoundSystem.SetAmbience(drain.Owner, true, null);
									amount /= (float)puddles.Count;
									foreach (ValueTuple<EntityUid, string> valueTuple in puddles)
									{
										EntityUid puddle2 = valueTuple.Item1;
										string solution = valueTuple.Item2;
										Solution puddleSolution;
										if (!this._solutionSystem.TryGetSolution(puddle2, solution, out puddleSolution, null))
										{
											this.EntityManager.QueueDeleteEntity(puddle2);
										}
										else
										{
											Solution transferSolution = this._solutionSystem.SplitSolution(puddle2, puddleSolution, FixedPoint2.Min(new FixedPoint2[]
											{
												FixedPoint2.New(amount),
												puddleSolution.Volume,
												drainSolution.AvailableVolume
											}));
											this._solutionSystem.TryAddSolution(drain.Owner, drainSolution, transferSolution);
											if (puddleSolution.Volume <= 0)
											{
												base.QueueDel(puddle2);
											}
										}
									}
								}
							}
						}
					}
				}
			}
		}

		// Token: 0x0400104A RID: 4170
		[Dependency]
		private readonly EntityLookupSystem _lookup;

		// Token: 0x0400104B RID: 4171
		[Dependency]
		private readonly SolutionContainerSystem _solutionSystem;

		// Token: 0x0400104C RID: 4172
		[Dependency]
		private readonly SharedAmbientSoundSystem _ambientSoundSystem;
	}
}
