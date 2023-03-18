using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Content.Shared.Chemistry.Components;
using Content.Shared.Movement.Components;
using Content.Shared.Movement.Systems;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.IoC;
using Robust.Shared.Timing;

namespace Content.Shared.Chemistry
{
	// Token: 0x020005C8 RID: 1480
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class MetabolismMovespeedModifierSystem : EntitySystem
	{
		// Token: 0x060011F2 RID: 4594 RVA: 0x0003AC60 File Offset: 0x00038E60
		public override void Initialize()
		{
			base.Initialize();
			base.UpdatesOutsidePrediction = true;
			base.SubscribeLocalEvent<MovespeedModifierMetabolismComponent, ComponentHandleState>(new ComponentEventRefHandler<MovespeedModifierMetabolismComponent, ComponentHandleState>(this.OnMovespeedHandleState), null, null);
			base.SubscribeLocalEvent<MovespeedModifierMetabolismComponent, ComponentStartup>(new ComponentEventHandler<MovespeedModifierMetabolismComponent, ComponentStartup>(this.AddComponent), null, null);
			base.SubscribeLocalEvent<MovespeedModifierMetabolismComponent, RefreshMovementSpeedModifiersEvent>(new ComponentEventHandler<MovespeedModifierMetabolismComponent, RefreshMovementSpeedModifiersEvent>(this.OnRefreshMovespeed), null, null);
		}

		// Token: 0x060011F3 RID: 4595 RVA: 0x0003ACB8 File Offset: 0x00038EB8
		private void OnMovespeedHandleState(EntityUid uid, MovespeedModifierMetabolismComponent component, ref ComponentHandleState args)
		{
			MovespeedModifierMetabolismComponent.MovespeedModifierMetabolismComponentState cast = args.Current as MovespeedModifierMetabolismComponent.MovespeedModifierMetabolismComponentState;
			if (cast == null)
			{
				return;
			}
			MovementSpeedModifierComponent modifier;
			if (this.EntityManager.TryGetComponent<MovementSpeedModifierComponent>(uid, ref modifier) && (!component.WalkSpeedModifier.Equals(cast.WalkSpeedModifier) || !component.SprintSpeedModifier.Equals(cast.SprintSpeedModifier)))
			{
				this._movespeed.RefreshMovementSpeedModifiers(uid, null);
			}
			component.WalkSpeedModifier = cast.WalkSpeedModifier;
			component.SprintSpeedModifier = cast.SprintSpeedModifier;
			component.ModifierTimer = cast.ModifierTimer;
		}

		// Token: 0x060011F4 RID: 4596 RVA: 0x0003AD42 File Offset: 0x00038F42
		private void OnRefreshMovespeed(EntityUid uid, MovespeedModifierMetabolismComponent component, RefreshMovementSpeedModifiersEvent args)
		{
			args.ModifySpeed(component.WalkSpeedModifier, component.SprintSpeedModifier);
		}

		// Token: 0x060011F5 RID: 4597 RVA: 0x0003AD56 File Offset: 0x00038F56
		private void AddComponent(EntityUid uid, MovespeedModifierMetabolismComponent component, ComponentStartup args)
		{
			this._components.Add(component);
		}

		// Token: 0x060011F6 RID: 4598 RVA: 0x0003AD64 File Offset: 0x00038F64
		public override void Update(float frameTime)
		{
			base.Update(frameTime);
			TimeSpan currentTime = this._gameTiming.CurTime;
			for (int i = this._components.Count - 1; i >= 0; i--)
			{
				MovespeedModifierMetabolismComponent component = this._components[i];
				if (component.Deleted)
				{
					this._components.RemoveAt(i);
				}
				else if (!(component.ModifierTimer > currentTime))
				{
					this._components.RemoveAt(i);
					this.EntityManager.RemoveComponent<MovespeedModifierMetabolismComponent>(component.Owner);
					this._movespeed.RefreshMovementSpeedModifiers(component.Owner, null);
				}
			}
		}

		// Token: 0x040010B6 RID: 4278
		[Dependency]
		private readonly IGameTiming _gameTiming;

		// Token: 0x040010B7 RID: 4279
		[Dependency]
		private readonly MovementSpeedModifierSystem _movespeed;

		// Token: 0x040010B8 RID: 4280
		private readonly List<MovespeedModifierMetabolismComponent> _components = new List<MovespeedModifierMetabolismComponent>();
	}
}
