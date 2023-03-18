using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Content.Shared.Actions;
using Content.Shared.Actions.ActionTypes;
using Content.Shared.Interaction;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Random;
using Robust.Shared.Timing;

namespace Content.Server.Actions
{
	// Token: 0x02000874 RID: 2164
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class ActionOnInteractSystem : EntitySystem
	{
		// Token: 0x06002F4B RID: 12107 RVA: 0x000F494F File Offset: 0x000F2B4F
		public override void Initialize()
		{
			base.Initialize();
			base.SubscribeLocalEvent<ActionOnInteractComponent, ActivateInWorldEvent>(new ComponentEventHandler<ActionOnInteractComponent, ActivateInWorldEvent>(this.OnActivate), null, null);
			base.SubscribeLocalEvent<ActionOnInteractComponent, AfterInteractEvent>(new ComponentEventHandler<ActionOnInteractComponent, AfterInteractEvent>(this.OnAfterInteract), null, null);
		}

		// Token: 0x06002F4C RID: 12108 RVA: 0x000F4980 File Offset: 0x000F2B80
		private void OnActivate(EntityUid uid, ActionOnInteractComponent component, ActivateInWorldEvent args)
		{
			if (args.Handled || component.ActivateActions == null)
			{
				return;
			}
			List<InstantAction> options = new List<InstantAction>();
			foreach (InstantAction action in component.ActivateActions)
			{
				if (this.ValidAction(action, true))
				{
					options.Add(action);
				}
			}
			if (options.Count == 0)
			{
				return;
			}
			InstantAction act = RandomExtensions.Pick<InstantAction>(this._random, options);
			if (act.Event != null)
			{
				act.Event.Performer = args.User;
			}
			act.Provider = new EntityUid?(uid);
			this._actions.PerformAction(args.User, null, act, act.Event, this._timing.CurTime, false);
			args.Handled = true;
		}

		// Token: 0x06002F4D RID: 12109 RVA: 0x000F4A5C File Offset: 0x000F2C5C
		private void OnAfterInteract(EntityUid uid, ActionOnInteractComponent component, AfterInteractEvent args)
		{
			if (args.Handled)
			{
				return;
			}
			if (args.Target != null && component.EntityActions != null)
			{
				List<EntityTargetAction> entOptions = new List<EntityTargetAction>();
				foreach (EntityTargetAction action in component.EntityActions)
				{
					if (this.ValidAction(action, args.CanReach) && this._actions.ValidateEntityTarget(args.User, args.Target.Value, action))
					{
						entOptions.Add(action);
					}
				}
				if (entOptions.Count > 0)
				{
					EntityTargetAction entAct = RandomExtensions.Pick<EntityTargetAction>(this._random, entOptions);
					if (entAct.Event != null)
					{
						entAct.Event.Performer = args.User;
						entAct.Event.Target = args.Target.Value;
					}
					entAct.Provider = new EntityUid?(uid);
					this._actions.PerformAction(args.User, null, entAct, entAct.Event, this._timing.CurTime, false);
					args.Handled = true;
					return;
				}
			}
			if (component.WorldActions == null)
			{
				return;
			}
			List<WorldTargetAction> options = new List<WorldTargetAction>();
			foreach (WorldTargetAction action2 in component.WorldActions)
			{
				if (this.ValidAction(action2, args.CanReach) && this._actions.ValidateWorldTarget(args.User, args.ClickLocation, action2))
				{
					options.Add(action2);
				}
			}
			if (options.Count == 0)
			{
				return;
			}
			WorldTargetAction act = RandomExtensions.Pick<WorldTargetAction>(this._random, options);
			if (act.Event != null)
			{
				act.Event.Performer = args.User;
				act.Event.Target = args.ClickLocation;
			}
			act.Provider = new EntityUid?(uid);
			this._actions.PerformAction(args.User, null, act, act.Event, this._timing.CurTime, false);
			args.Handled = true;
		}

		// Token: 0x06002F4E RID: 12110 RVA: 0x000F4C94 File Offset: 0x000F2E94
		private bool ValidAction(ActionType act, bool canReach = true)
		{
			if (!act.Enabled)
			{
				return false;
			}
			if (act.Charges != null)
			{
				int? charges = act.Charges;
				int num = 0;
				if (charges.GetValueOrDefault() <= num & charges != null)
				{
					return false;
				}
			}
			TimeSpan curTime = this._timing.CurTime;
			if (act.Cooldown != null && act.Cooldown.Value.Item2 > curTime)
			{
				return false;
			}
			if (!canReach)
			{
				TargetedAction targetedAction = act as TargetedAction;
				return targetedAction != null && !targetedAction.CheckCanAccess;
			}
			return true;
		}

		// Token: 0x04001C71 RID: 7281
		[Dependency]
		private readonly IRobustRandom _random;

		// Token: 0x04001C72 RID: 7282
		[Dependency]
		private readonly IGameTiming _timing;

		// Token: 0x04001C73 RID: 7283
		[Dependency]
		private readonly SharedActionsSystem _actions;
	}
}
