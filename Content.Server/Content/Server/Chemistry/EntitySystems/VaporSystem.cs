using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Content.Server.Chemistry.Components;
using Content.Server.Chemistry.Components.SolutionManager;
using Content.Shared.Chemistry.Components;
using Content.Shared.Chemistry.Reagent;
using Content.Shared.FixedPoint;
using Content.Shared.Spawners.Components;
using Content.Shared.Throwing;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Map;
using Robust.Shared.Map.Components;
using Robust.Shared.Maths;
using Robust.Shared.Physics.Components;
using Robust.Shared.Physics.Events;
using Robust.Shared.Physics.Systems;
using Robust.Shared.Prototypes;

namespace Content.Server.Chemistry.EntitySystems
{
	// Token: 0x020006A2 RID: 1698
	[NullableContext(1)]
	[Nullable(0)]
	internal sealed class VaporSystem : EntitySystem
	{
		// Token: 0x0600236D RID: 9069 RVA: 0x000B92D0 File Offset: 0x000B74D0
		public override void Initialize()
		{
			base.Initialize();
			base.SubscribeLocalEvent<VaporComponent, StartCollideEvent>(new ComponentEventRefHandler<VaporComponent, StartCollideEvent>(this.HandleCollide), null, null);
		}

		// Token: 0x0600236E RID: 9070 RVA: 0x000B92EC File Offset: 0x000B74EC
		private void HandleCollide(EntityUid uid, VaporComponent component, ref StartCollideEvent args)
		{
			SolutionContainerManagerComponent contents;
			if (!this.EntityManager.TryGetComponent<SolutionContainerManagerComponent>(uid, ref contents))
			{
				return;
			}
			foreach (Solution solution in contents.Solutions.Values)
			{
				solution.DoEntityReaction(args.OtherFixture.Body.Owner, ReactionMethod.Touch);
			}
			if ((args.OtherFixture.CollisionLayer & 2) != 0 && args.OtherFixture.Hard)
			{
				this.EntityManager.QueueDeleteEntity(uid);
			}
		}

		// Token: 0x0600236F RID: 9071 RVA: 0x000B938C File Offset: 0x000B758C
		public void Start(VaporComponent vapor, TransformComponent vaporXform, Vector2 dir, float speed, MapCoordinates target, float aliveTime, EntityUid? user = null)
		{
			vapor.Active = true;
			TimedDespawnComponent despawn = base.EnsureComp<TimedDespawnComponent>(vapor.Owner);
			despawn.Lifetime = aliveTime;
			PhysicsComponent physics;
			if (this.EntityManager.TryGetComponent<PhysicsComponent>(vapor.Owner, ref physics))
			{
				this._physics.SetLinearDamping(physics, 0f, true);
				this._physics.SetAngularDamping(physics, 0f, true);
				this._throwing.TryThrow(vapor.Owner, dir * speed, 1f, user, 50f, null, null, null, null);
				float time = (target.Position - vaporXform.WorldPosition).Length / physics.LinearVelocity.Length;
				despawn.Lifetime = MathF.Min(aliveTime, time);
			}
		}

		// Token: 0x06002370 RID: 9072 RVA: 0x000B9464 File Offset: 0x000B7664
		internal bool TryAddSolution(VaporComponent vapor, Solution solution)
		{
			Solution vaporSolution;
			return !(solution.Volume == 0) && this._solutionContainerSystem.TryGetSolution(vapor.Owner, "vapor", out vaporSolution, null) && this._solutionContainerSystem.TryAddSolution(vapor.Owner, vaporSolution, solution);
		}

		// Token: 0x06002371 RID: 9073 RVA: 0x000B94B4 File Offset: 0x000B76B4
		public override void Update(float frameTime)
		{
			foreach (ValueTuple<VaporComponent, SolutionContainerManagerComponent, TransformComponent> valueTuple in this.EntityManager.EntityQuery<VaporComponent, SolutionContainerManagerComponent, TransformComponent>(false))
			{
				VaporComponent vaporComp = valueTuple.Item1;
				SolutionContainerManagerComponent solution = valueTuple.Item2;
				TransformComponent xform = valueTuple.Item3;
				foreach (KeyValuePair<string, Solution> keyValuePair in solution.Solutions)
				{
					string text;
					Solution solution2;
					keyValuePair.Deconstruct(out text, out solution2);
					Solution value = solution2;
					this.Update(frameTime, vaporComp, value, xform);
				}
			}
		}

		// Token: 0x06002372 RID: 9074 RVA: 0x000B956C File Offset: 0x000B776C
		private void Update(float frameTime, VaporComponent vapor, Solution contents, TransformComponent xform)
		{
			if (!vapor.Active)
			{
				return;
			}
			EntityUid entity = vapor.Owner;
			vapor.ReactTimer += frameTime;
			MapGridComponent gridComp;
			if (vapor.ReactTimer >= 0.125f && base.TryComp<MapGridComponent>(xform.GridUid, ref gridComp))
			{
				vapor.ReactTimer = 0f;
				TileRef tile = gridComp.GetTileRef(xform.Coordinates.ToVector2i(this.EntityManager, this._mapManager));
				foreach (Solution.ReagentQuantity reagentQuantity in contents.Contents.ToArray())
				{
					if (!(reagentQuantity.Quantity == FixedPoint2.Zero))
					{
						ReagentPrototype reagent = this._protoManager.Index<ReagentPrototype>(reagentQuantity.ReagentId);
						this._solutionContainerSystem.TryRemoveReagent(vapor.Owner, contents, reagentQuantity.ReagentId, reagent.ReactionTile(tile, reagentQuantity.Quantity / vapor.TransferAmount * 0.25f));
					}
				}
			}
			if (contents.Volume == 0)
			{
				this.EntityManager.QueueDeleteEntity(entity);
			}
		}

		// Token: 0x040015CF RID: 5583
		[Dependency]
		private readonly IMapManager _mapManager;

		// Token: 0x040015D0 RID: 5584
		[Dependency]
		private readonly IPrototypeManager _protoManager;

		// Token: 0x040015D1 RID: 5585
		[Dependency]
		private readonly SharedPhysicsSystem _physics;

		// Token: 0x040015D2 RID: 5586
		[Dependency]
		private readonly SolutionContainerSystem _solutionContainerSystem;

		// Token: 0x040015D3 RID: 5587
		[Dependency]
		private readonly ThrowingSystem _throwing;

		// Token: 0x040015D4 RID: 5588
		private const float ReactTime = 0.125f;
	}
}
