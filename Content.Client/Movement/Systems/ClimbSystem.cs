using System;
using System.Runtime.CompilerServices;
using Content.Client.Interactable;
using Content.Shared.Climbing;
using Content.Shared.DragDrop;
using Content.Shared.Interaction;
using Content.Shared.Physics;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.IoC;

namespace Content.Client.Movement.Systems
{
	// Token: 0x02000227 RID: 551
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class ClimbSystem : SharedClimbSystem
	{
		// Token: 0x06000E51 RID: 3665 RVA: 0x000569AA File Offset: 0x00054BAA
		public override void Initialize()
		{
			base.Initialize();
			ComponentEventRefHandler<ClimbingComponent, ComponentHandleState> componentEventRefHandler;
			if ((componentEventRefHandler = ClimbSystem.<>O.<0>__OnClimbingState) == null)
			{
				componentEventRefHandler = (ClimbSystem.<>O.<0>__OnClimbingState = new ComponentEventRefHandler<ClimbingComponent, ComponentHandleState>(ClimbSystem.OnClimbingState));
			}
			base.SubscribeLocalEvent<ClimbingComponent, ComponentHandleState>(componentEventRefHandler, null, null);
			base.SubscribeLocalEvent<ClimbableComponent, CanDropTargetEvent>(new ComponentEventRefHandler<ClimbableComponent, CanDropTargetEvent>(this.OnCanDragDropOn), null, null);
		}

		// Token: 0x06000E52 RID: 3666 RVA: 0x000569EC File Offset: 0x00054BEC
		private static void OnClimbingState(EntityUid uid, ClimbingComponent component, ref ComponentHandleState args)
		{
			ClimbingComponent.ClimbModeComponentState climbModeComponentState = args.Current as ClimbingComponent.ClimbModeComponentState;
			if (climbModeComponentState == null)
			{
				return;
			}
			component.IsClimbing = climbModeComponentState.Climbing;
			component.OwnerIsTransitioning = climbModeComponentState.IsTransitioning;
		}

		// Token: 0x06000E53 RID: 3667 RVA: 0x00056A24 File Offset: 0x00054C24
		protected override void OnCanDragDropOn(EntityUid uid, ClimbableComponent component, ref CanDropTargetEvent args)
		{
			ClimbSystem.<>c__DisplayClass3_0 CS$<>8__locals1 = new ClimbSystem.<>c__DisplayClass3_0();
			base.OnCanDragDropOn(uid, component, ref args);
			if (!args.CanDrop)
			{
				return;
			}
			CS$<>8__locals1.user = args.User;
			CS$<>8__locals1.target = uid;
			CS$<>8__locals1.dragged = args.Dragged;
			args.CanDrop = (this._interactionSystem.InRangeUnobstructed(CS$<>8__locals1.user, CS$<>8__locals1.target, component.Range, CollisionGroup.Impassable | CollisionGroup.InteractImpassable, new SharedInteractionSystem.Ignored(CS$<>8__locals1.<OnCanDragDropOn>g__Ignored|0), false) && this._interactionSystem.InRangeUnobstructed(CS$<>8__locals1.user, CS$<>8__locals1.dragged, component.Range, CollisionGroup.Impassable | CollisionGroup.InteractImpassable, new SharedInteractionSystem.Ignored(CS$<>8__locals1.<OnCanDragDropOn>g__Ignored|0), false));
			args.Handled = true;
		}

		// Token: 0x04000711 RID: 1809
		[Dependency]
		private readonly InteractionSystem _interactionSystem;

		// Token: 0x02000228 RID: 552
		[CompilerGenerated]
		private static class <>O
		{
			// Token: 0x04000712 RID: 1810
			[Nullable(new byte[]
			{
				0,
				1
			})]
			public static ComponentEventRefHandler<ClimbingComponent, ComponentHandleState> <0>__OnClimbingState;
		}
	}
}
