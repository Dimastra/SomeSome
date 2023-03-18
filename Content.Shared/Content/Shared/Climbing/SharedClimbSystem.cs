using System;
using System.Runtime.CompilerServices;
using Content.Shared.DragDrop;
using Content.Shared.Movement.Events;
using Robust.Shared.GameObjects;

namespace Content.Shared.Climbing
{
	// Token: 0x020005C6 RID: 1478
	[NullableContext(1)]
	[Nullable(0)]
	public abstract class SharedClimbSystem : EntitySystem
	{
		// Token: 0x060011E7 RID: 4583 RVA: 0x0003AB78 File Offset: 0x00038D78
		public override void Initialize()
		{
			base.Initialize();
			ComponentEventHandler<ClimbingComponent, UpdateCanMoveEvent> componentEventHandler;
			if ((componentEventHandler = SharedClimbSystem.<>O.<0>__HandleMoveAttempt) == null)
			{
				componentEventHandler = (SharedClimbSystem.<>O.<0>__HandleMoveAttempt = new ComponentEventHandler<ClimbingComponent, UpdateCanMoveEvent>(SharedClimbSystem.HandleMoveAttempt));
			}
			base.SubscribeLocalEvent<ClimbingComponent, UpdateCanMoveEvent>(componentEventHandler, null, null);
		}

		// Token: 0x060011E8 RID: 4584 RVA: 0x0003ABA3 File Offset: 0x00038DA3
		private static void HandleMoveAttempt(EntityUid uid, ClimbingComponent component, UpdateCanMoveEvent args)
		{
			if (component.LifeStage > 6)
			{
				return;
			}
			if (component.OwnerIsTransitioning)
			{
				args.Cancel();
			}
		}

		// Token: 0x060011E9 RID: 4585 RVA: 0x0003ABBD File Offset: 0x00038DBD
		protected virtual void OnCanDragDropOn(EntityUid uid, ClimbableComponent component, ref CanDropTargetEvent args)
		{
			args.CanDrop = base.HasComp<ClimbingComponent>(args.Dragged);
		}

		// Token: 0x02000852 RID: 2130
		[CompilerGenerated]
		private static class <>O
		{
			// Token: 0x0400196B RID: 6507
			[Nullable(new byte[]
			{
				0,
				1,
				1
			})]
			public static ComponentEventHandler<ClimbingComponent, UpdateCanMoveEvent> <0>__HandleMoveAttempt;
		}
	}
}
