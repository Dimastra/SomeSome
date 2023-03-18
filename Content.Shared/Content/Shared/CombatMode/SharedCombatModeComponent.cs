using System;
using System.Runtime.CompilerServices;
using Content.Shared.Actions;
using Content.Shared.Actions.ActionTypes;
using Content.Shared.Targeting;
using Robust.Shared.Audio;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom.Prototype;
using Robust.Shared.ViewVariables;

namespace Content.Shared.CombatMode
{
	// Token: 0x0200059C RID: 1436
	[NullableContext(1)]
	[Nullable(0)]
	[NetworkedComponent]
	public abstract class SharedCombatModeComponent : Component
	{
		// Token: 0x17000388 RID: 904
		// (get) Token: 0x06001184 RID: 4484 RVA: 0x000392E2 File Offset: 0x000374E2
		// (set) Token: 0x06001185 RID: 4485 RVA: 0x000392EA File Offset: 0x000374EA
		[ViewVariables]
		public virtual bool IsInCombatMode
		{
			get
			{
				return this._isInCombatMode;
			}
			set
			{
				if (this._isInCombatMode == value)
				{
					return;
				}
				this._isInCombatMode = value;
				if (this.CombatToggleAction != null)
				{
					EntitySystem.Get<SharedActionsSystem>().SetToggled(this.CombatToggleAction, this._isInCombatMode);
				}
				base.Dirty(null);
			}
		}

		// Token: 0x17000389 RID: 905
		// (get) Token: 0x06001186 RID: 4486 RVA: 0x00039322 File Offset: 0x00037522
		// (set) Token: 0x06001187 RID: 4487 RVA: 0x0003932A File Offset: 0x0003752A
		[ViewVariables]
		public virtual TargetingZone ActiveZone
		{
			get
			{
				return this._activeZone;
			}
			set
			{
				if (this._activeZone == value)
				{
					return;
				}
				this._activeZone = value;
				base.Dirty(null);
			}
		}

		// Token: 0x0400102E RID: 4142
		[ViewVariables]
		[DataField("canDisarm", false, 1, false, false, null)]
		public bool? CanDisarm;

		// Token: 0x0400102F RID: 4143
		[DataField("disarmSuccessSound", false, 1, false, false, null)]
		public readonly SoundSpecifier DisarmSuccessSound = new SoundPathSpecifier("/Audio/Effects/thudswoosh.ogg", null);

		// Token: 0x04001030 RID: 4144
		[DataField("disarmFailChance", false, 1, false, false, null)]
		public readonly float BaseDisarmFailChance = 0.75f;

		// Token: 0x04001031 RID: 4145
		private bool _isInCombatMode;

		// Token: 0x04001032 RID: 4146
		private TargetingZone _activeZone;

		// Token: 0x04001033 RID: 4147
		[DataField("combatToggleActionId", false, 1, false, false, typeof(PrototypeIdSerializer<InstantActionPrototype>))]
		public readonly string CombatToggleActionId = "CombatModeToggle";

		// Token: 0x04001034 RID: 4148
		[Nullable(2)]
		[DataField("combatToggleAction", false, 1, false, false, null)]
		public InstantAction CombatToggleAction;
	}
}
