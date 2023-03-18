using System;
using System.Runtime.CompilerServices;
using Content.Shared.Body.Components;
using Content.Shared.Disposal.Components;
using Content.Shared.DragDrop;
using Content.Shared.Item;
using Content.Shared.Mobs.Components;
using Content.Shared.Mobs.Systems;
using Content.Shared.Throwing;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Physics.Components;
using Robust.Shared.Physics.Events;
using Robust.Shared.Timing;

namespace Content.Shared.Disposal
{
	// Token: 0x020004FD RID: 1277
	[NullableContext(1)]
	[Nullable(0)]
	public abstract class SharedDisposalUnitSystem : EntitySystem
	{
		// Token: 0x06000F75 RID: 3957 RVA: 0x0003244F File Offset: 0x0003064F
		public override void Initialize()
		{
			base.Initialize();
			base.SubscribeLocalEvent<SharedDisposalUnitComponent, PreventCollideEvent>(new ComponentEventRefHandler<SharedDisposalUnitComponent, PreventCollideEvent>(this.OnPreventCollide), null, null);
			base.SubscribeLocalEvent<SharedDisposalUnitComponent, CanDropTargetEvent>(new ComponentEventRefHandler<SharedDisposalUnitComponent, CanDropTargetEvent>(this.OnCanDragDropOn), null, null);
		}

		// Token: 0x06000F76 RID: 3958 RVA: 0x00032480 File Offset: 0x00030680
		private void OnPreventCollide(EntityUid uid, SharedDisposalUnitComponent component, ref PreventCollideEvent args)
		{
			EntityUid otherBody = args.BodyB.Owner;
			if (this.EntityManager.HasComponent<ItemComponent>(otherBody) && !this.EntityManager.HasComponent<ThrownItemComponent>(otherBody))
			{
				args.Cancelled = true;
				return;
			}
			if (component.RecentlyEjected.Contains(otherBody))
			{
				args.Cancelled = true;
			}
		}

		// Token: 0x06000F77 RID: 3959 RVA: 0x000324D2 File Offset: 0x000306D2
		private void OnCanDragDropOn(EntityUid uid, SharedDisposalUnitComponent component, ref CanDropTargetEvent args)
		{
			if (args.Handled)
			{
				return;
			}
			args.CanDrop = this.CanInsert(component, args.Dragged);
			args.Handled = true;
		}

		// Token: 0x06000F78 RID: 3960 RVA: 0x000324F8 File Offset: 0x000306F8
		public virtual bool CanInsert(SharedDisposalUnitComponent component, EntityUid entity)
		{
			ItemComponent storable;
			MobStateComponent damageState;
			PhysicsComponent physics;
			return this.EntityManager.GetComponent<TransformComponent>(component.Owner).Anchored && (this.EntityManager.TryGetComponent<ItemComponent>(entity, ref storable) || this.EntityManager.HasComponent<BodyComponent>(entity)) && (!base.TryComp<MobStateComponent>(entity, ref damageState) || component.MobsCanEnter) && ((this.EntityManager.TryGetComponent<PhysicsComponent>(entity, ref physics) && (physics.CanCollide || storable != null)) || (damageState != null && (!component.MobsCanEnter || this._mobState.IsDead(entity, damageState))));
		}

		// Token: 0x04000EBE RID: 3774
		[Dependency]
		protected readonly IGameTiming GameTiming;

		// Token: 0x04000EBF RID: 3775
		[Dependency]
		private readonly MobStateSystem _mobState;

		// Token: 0x04000EC0 RID: 3776
		protected static TimeSpan ExitAttemptDelay = TimeSpan.FromSeconds(0.5);

		// Token: 0x04000EC1 RID: 3777
		public const float PressurePerSecond = 0.05f;
	}
}
