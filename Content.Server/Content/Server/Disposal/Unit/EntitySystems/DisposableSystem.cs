using System;
using System.Linq;
using System.Runtime.CompilerServices;
using Content.Server.Atmos;
using Content.Server.Atmos.EntitySystems;
using Content.Server.Disposal.Tube;
using Content.Server.Disposal.Tube.Components;
using Content.Server.Disposal.Unit.Components;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Log;
using Robust.Shared.Map;
using Robust.Shared.Map.Components;
using Robust.Shared.Maths;
using Robust.Shared.Physics.Components;
using Robust.Shared.Physics.Systems;

namespace Content.Server.Disposal.Unit.EntitySystems
{
	// Token: 0x0200054D RID: 1357
	[NullableContext(1)]
	[Nullable(0)]
	internal sealed class DisposableSystem : EntitySystem
	{
		// Token: 0x06001C77 RID: 7287 RVA: 0x00097A44 File Offset: 0x00095C44
		[NullableContext(2)]
		public void ExitDisposals(EntityUid uid, DisposalHolderComponent holder = null, TransformComponent holderTransform = null)
		{
			if (base.Terminating(uid, null))
			{
				return;
			}
			if (!base.Resolve<DisposalHolderComponent, TransformComponent>(uid, ref holder, ref holderTransform, true))
			{
				return;
			}
			if (holder.IsExitingDisposals)
			{
				Logger.ErrorS("c.s.disposal.holder", "Tried exiting disposals twice. This should never happen.");
				return;
			}
			holder.IsExitingDisposals = true;
			EntityUid? disposalId = null;
			DisposalUnitComponent duc = null;
			MapGridComponent grid;
			if (this._mapManager.TryGetGrid(holderTransform.GridUid, ref grid))
			{
				foreach (EntityUid contentUid in grid.GetLocal(holderTransform.Coordinates))
				{
					if (this.EntityManager.TryGetComponent<DisposalUnitComponent>(contentUid, ref duc))
					{
						disposalId = new EntityUid?(contentUid);
						break;
					}
				}
			}
			foreach (EntityUid entity in holder.Container.ContainedEntities.ToArray<EntityUid>())
			{
				base.RemComp<BeingDisposedComponent>(entity);
				MetaDataComponent meta = base.MetaData(entity);
				holder.Container.Remove(entity, this.EntityManager, null, meta, false, true, null, null);
				TransformComponent xform = base.Transform(entity);
				if (!(xform.ParentUid != uid))
				{
					if (duc != null)
					{
						duc.Container.Insert(entity, this.EntityManager, xform, null, meta, null);
					}
					else
					{
						xform.AttachToGridOrMap();
					}
					PhysicsComponent physics;
					if (this.EntityManager.TryGetComponent<PhysicsComponent>(entity, ref physics))
					{
						this._physicsSystem.WakeBody(entity, false, null, physics);
					}
				}
			}
			if (disposalId != null && duc != null)
			{
				this._disposalUnitSystem.TryEjectContents(disposalId.Value, duc);
			}
			GasMixture environment = this._atmosphereSystem.GetContainingMixture(uid, false, true, null);
			if (environment != null)
			{
				this._atmosphereSystem.Merge(environment, holder.Air);
				holder.Air.Clear();
			}
			this.EntityManager.DeleteEntity(uid);
		}

		// Token: 0x06001C78 RID: 7288 RVA: 0x00097C3C File Offset: 0x00095E3C
		[NullableContext(2)]
		public bool EnterTube(EntityUid holderUid, EntityUid toUid, DisposalHolderComponent holder = null, TransformComponent holderTransform = null, IDisposalTubeComponent to = null, TransformComponent toTransform = null)
		{
			if (!base.Resolve<DisposalHolderComponent, TransformComponent>(holderUid, ref holder, ref holderTransform, true))
			{
				return false;
			}
			if (holder.IsExitingDisposals)
			{
				Logger.ErrorS("c.s.disposal.holder", "Tried entering tube after exiting disposals. This should never happen.");
				return false;
			}
			if (!base.Resolve<IDisposalTubeComponent, TransformComponent>(toUid, ref to, ref toTransform, true))
			{
				this.ExitDisposals(holderUid, holder, holderTransform);
				return false;
			}
			foreach (EntityUid ent in holder.Container.ContainedEntities)
			{
				base.EnsureComp<BeingDisposedComponent>(ent).Holder = holder.Owner;
			}
			if (!to.Contents.Insert(holder.Owner, null, null, null, null, null))
			{
				this.ExitDisposals(holderUid, holder, holderTransform);
				return false;
			}
			if (holder.CurrentTube != null)
			{
				holder.PreviousTube = holder.CurrentTube;
				holder.PreviousDirection = holder.CurrentDirection;
			}
			holder.CurrentTube = to;
			holder.CurrentDirection = to.NextDirection(holder);
			holder.StartingTime = 0.1f;
			holder.TimeLeft = 0.1f;
			if (holder.CurrentDirection == -1)
			{
				this.ExitDisposals(holderUid, holder, holderTransform);
				return false;
			}
			return true;
		}

		// Token: 0x06001C79 RID: 7289 RVA: 0x00097D64 File Offset: 0x00095F64
		public override void Update(float frameTime)
		{
			foreach (DisposalHolderComponent comp in this.EntityManager.EntityQuery<DisposalHolderComponent>(false))
			{
				this.UpdateComp(comp, frameTime);
			}
		}

		// Token: 0x06001C7A RID: 7290 RVA: 0x00097DB8 File Offset: 0x00095FB8
		private void UpdateComp(DisposalHolderComponent holder, float frameTime)
		{
			while (frameTime > 0f)
			{
				float time = frameTime;
				if (time > holder.TimeLeft)
				{
					time = holder.TimeLeft;
				}
				holder.TimeLeft -= time;
				frameTime -= time;
				IDisposalTubeComponent currentTube = holder.CurrentTube;
				if (currentTube == null || currentTube.Deleted)
				{
					this.ExitDisposals(holder.Owner, null, null);
					return;
				}
				if (holder.TimeLeft > 0f)
				{
					float progress = 1f - holder.TimeLeft / holder.StartingTime;
					EntityCoordinates origin = this.EntityManager.GetComponent<TransformComponent>(currentTube.Owner).Coordinates;
					Vector2 newPosition = DirectionExtensions.ToVec(holder.CurrentDirection) * progress;
					this.EntityManager.GetComponent<TransformComponent>(holder.Owner).Coordinates = origin.Offset(newPosition).WithEntityId(currentTube.Owner, null);
				}
				else
				{
					currentTube.Contents.Remove(holder.Owner, null, null, null, false, true, null, null);
					IDisposalTubeComponent nextTube = this._disposalTubeSystem.NextTubeFor(currentTube.Owner, holder.CurrentDirection, null);
					if (nextTube == null || nextTube.Deleted)
					{
						this.ExitDisposals(holder.Owner, null, null);
						return;
					}
					if (!this.EnterTube(holder.Owner, nextTube.Owner, holder, null, nextTube, null))
					{
						break;
					}
				}
			}
		}

		// Token: 0x04001245 RID: 4677
		[Dependency]
		private readonly IMapManager _mapManager;

		// Token: 0x04001246 RID: 4678
		[Dependency]
		private readonly DisposalUnitSystem _disposalUnitSystem;

		// Token: 0x04001247 RID: 4679
		[Dependency]
		private readonly DisposalTubeSystem _disposalTubeSystem;

		// Token: 0x04001248 RID: 4680
		[Dependency]
		private readonly AtmosphereSystem _atmosphereSystem;

		// Token: 0x04001249 RID: 4681
		[Dependency]
		private readonly SharedPhysicsSystem _physicsSystem;
	}
}
