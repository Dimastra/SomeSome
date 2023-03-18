using System;
using System.Runtime.CompilerServices;
using Content.Shared.Actions;
using Content.Shared.Interaction.Events;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;

namespace Content.Shared.CombatMode.Pacification
{
	// Token: 0x0200059F RID: 1439
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class PacificationSystem : EntitySystem
	{
		// Token: 0x06001190 RID: 4496 RVA: 0x000394A8 File Offset: 0x000376A8
		public override void Initialize()
		{
			base.Initialize();
			base.SubscribeLocalEvent<PacifiedComponent, ComponentStartup>(new ComponentEventHandler<PacifiedComponent, ComponentStartup>(this.OnStartup), null, null);
			base.SubscribeLocalEvent<PacifiedComponent, ComponentShutdown>(new ComponentEventHandler<PacifiedComponent, ComponentShutdown>(this.OnShutdown), null, null);
			base.SubscribeLocalEvent<PacifiedComponent, AttackAttemptEvent>(new ComponentEventHandler<PacifiedComponent, AttackAttemptEvent>(this.OnAttackAttempt), null, null);
		}

		// Token: 0x06001191 RID: 4497 RVA: 0x000394F7 File Offset: 0x000376F7
		private void OnAttackAttempt(EntityUid uid, PacifiedComponent component, AttackAttemptEvent args)
		{
			args.Cancel();
		}

		// Token: 0x06001192 RID: 4498 RVA: 0x00039500 File Offset: 0x00037700
		private void OnStartup(EntityUid uid, PacifiedComponent component, ComponentStartup args)
		{
			SharedCombatModeComponent combatMode;
			if (!base.TryComp<SharedCombatModeComponent>(uid, ref combatMode))
			{
				return;
			}
			if (combatMode.CanDisarm != null)
			{
				combatMode.CanDisarm = new bool?(false);
			}
			combatMode.IsInCombatMode = false;
			if (combatMode.CombatToggleAction != null)
			{
				this._actionsSystem.SetEnabled(combatMode.CombatToggleAction, false);
			}
		}

		// Token: 0x06001193 RID: 4499 RVA: 0x00039554 File Offset: 0x00037754
		private void OnShutdown(EntityUid uid, PacifiedComponent component, ComponentShutdown args)
		{
			SharedCombatModeComponent combatMode;
			if (!base.TryComp<SharedCombatModeComponent>(uid, ref combatMode))
			{
				return;
			}
			if (combatMode.CanDisarm != null)
			{
				combatMode.CanDisarm = new bool?(true);
			}
			if (combatMode.CombatToggleAction != null)
			{
				this._actionsSystem.SetEnabled(combatMode.CombatToggleAction, true);
			}
		}

		// Token: 0x04001037 RID: 4151
		[Dependency]
		private readonly SharedActionsSystem _actionsSystem;
	}
}
