using System;
using System.Runtime.CompilerServices;
using Content.Shared.Movement.Components;
using Content.Shared.Movement.Events;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;

namespace Content.Shared.Movement.Systems
{
	// Token: 0x020002D7 RID: 727
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class MovementIgnoreGravitySystem : EntitySystem
	{
		// Token: 0x060007EF RID: 2031 RVA: 0x0001A584 File Offset: 0x00018784
		public override void Initialize()
		{
			base.SubscribeLocalEvent<MovementIgnoreGravityComponent, ComponentGetState>(new ComponentEventRefHandler<MovementIgnoreGravityComponent, ComponentGetState>(this.GetState), null, null);
			base.SubscribeLocalEvent<MovementIgnoreGravityComponent, ComponentHandleState>(new ComponentEventRefHandler<MovementIgnoreGravityComponent, ComponentHandleState>(this.HandleState), null, null);
			base.SubscribeLocalEvent<MovementAlwaysTouchingComponent, CanWeightlessMoveEvent>(new ComponentEventRefHandler<MovementAlwaysTouchingComponent, CanWeightlessMoveEvent>(this.OnWeightless), null, null);
		}

		// Token: 0x060007F0 RID: 2032 RVA: 0x0001A5C2 File Offset: 0x000187C2
		private void OnWeightless(EntityUid uid, MovementAlwaysTouchingComponent component, ref CanWeightlessMoveEvent args)
		{
			args.CanMove = true;
		}

		// Token: 0x060007F1 RID: 2033 RVA: 0x0001A5CB File Offset: 0x000187CB
		private void HandleState(EntityUid uid, MovementIgnoreGravityComponent component, ref ComponentHandleState args)
		{
			if (args.Next == null)
			{
				return;
			}
			component.Weightless = ((MovementIgnoreGravityComponentState)args.Next).Weightless;
		}

		// Token: 0x060007F2 RID: 2034 RVA: 0x0001A5EC File Offset: 0x000187EC
		private void GetState(EntityUid uid, MovementIgnoreGravityComponent component, ref ComponentGetState args)
		{
			args.State = new MovementIgnoreGravityComponentState(component);
		}
	}
}
