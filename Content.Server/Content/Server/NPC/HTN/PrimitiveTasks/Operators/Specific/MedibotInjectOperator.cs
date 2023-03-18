using System;
using System.Runtime.CompilerServices;
using Content.Server.Chat.Systems;
using Content.Server.Chemistry.EntitySystems;
using Content.Server.NPC.Components;
using Content.Server.Silicons.Bots;
using Content.Shared.Chemistry.Components;
using Content.Shared.Damage;
using Content.Shared.FixedPoint;
using Content.Shared.Interaction;
using Content.Shared.Physics;
using Content.Shared.Popups;
using Robust.Shared.Audio;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Localization;
using Robust.Shared.Player;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Server.NPC.HTN.PrimitiveTasks.Operators.Specific
{
	// Token: 0x02000359 RID: 857
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class MedibotInjectOperator : HTNOperator
	{
		// Token: 0x060011D3 RID: 4563 RVA: 0x0005DDCB File Offset: 0x0005BFCB
		public override void Initialize(IEntitySystemManager sysManager)
		{
			base.Initialize(sysManager);
			this._chat = sysManager.GetEntitySystem<ChatSystem>();
			this._interactionSystem = sysManager.GetEntitySystem<SharedInteractionSystem>();
			this._popupSystem = sysManager.GetEntitySystem<SharedPopupSystem>();
			this._solutionSystem = sysManager.GetEntitySystem<SolutionContainerSystem>();
		}

		// Token: 0x060011D4 RID: 4564 RVA: 0x0005DE04 File Offset: 0x0005C004
		public override void Shutdown(NPCBlackboard blackboard, HTNOperatorStatus status)
		{
			base.Shutdown(blackboard, status);
			blackboard.Remove<EntityUid>(this.TargetKey);
		}

		// Token: 0x060011D5 RID: 4565 RVA: 0x0005DE1C File Offset: 0x0005C01C
		public override HTNOperatorStatus Update(NPCBlackboard blackboard, float frameTime)
		{
			EntityUid owner = blackboard.GetValue<EntityUid>("Owner");
			EntityUid target;
			if (!blackboard.TryGetValue<EntityUid>(this.TargetKey, out target, this._entManager) || this._entManager.Deleted(target))
			{
				return HTNOperatorStatus.Failed;
			}
			MedibotComponent botComp;
			if (!this._entManager.TryGetComponent<MedibotComponent>(owner, ref botComp))
			{
				return HTNOperatorStatus.Failed;
			}
			this._entManager.EnsureComponent<NPCRecentlyInjectedComponent>(target);
			DamageableComponent damage;
			if (!this._entManager.TryGetComponent<DamageableComponent>(target, ref damage))
			{
				return HTNOperatorStatus.Failed;
			}
			Solution injectable;
			if (!this._solutionSystem.TryGetInjectableSolution(target, out injectable, null, null))
			{
				return HTNOperatorStatus.Failed;
			}
			if (!this._interactionSystem.InRangeUnobstructed(owner, target, 1.5f, CollisionGroup.Impassable | CollisionGroup.InteractImpassable, null, false))
			{
				return HTNOperatorStatus.Failed;
			}
			if (damage.TotalDamage == 0)
			{
				return HTNOperatorStatus.Failed;
			}
			if (damage.TotalDamage >= 100f)
			{
				FixedPoint2 accepted;
				this._solutionSystem.TryAddReagent(target, injectable, botComp.EmergencyMed, botComp.EmergencyMedInjectAmount, out accepted, null);
				this._popupSystem.PopupEntity(Loc.GetString("hypospray-component-feel-prick-message"), target, target, PopupType.Small);
				SoundSystem.Play("/Audio/Items/hypospray.ogg", Filter.Pvs(target, 2f, null, null, null), target, null);
				this._chat.TrySendInGameICMessage(owner, Loc.GetString("medibot-finish-inject"), InGameICChatType.Speak, false, true, null, null, null, true, false);
				return HTNOperatorStatus.Finished;
			}
			if (damage.TotalDamage >= 50f)
			{
				FixedPoint2 accepted2;
				this._solutionSystem.TryAddReagent(target, injectable, botComp.StandardMed, botComp.StandardMedInjectAmount, out accepted2, null);
				this._popupSystem.PopupEntity(Loc.GetString("hypospray-component-feel-prick-message"), target, target, PopupType.Small);
				SoundSystem.Play("/Audio/Items/hypospray.ogg", Filter.Pvs(target, 2f, null, null, null), target, null);
				this._chat.TrySendInGameICMessage(owner, Loc.GetString("medibot-finish-inject"), InGameICChatType.Speak, false, true, null, null, null, true, false);
				return HTNOperatorStatus.Finished;
			}
			return HTNOperatorStatus.Failed;
		}

		// Token: 0x04000AD2 RID: 2770
		[Dependency]
		private readonly IEntityManager _entManager;

		// Token: 0x04000AD3 RID: 2771
		private ChatSystem _chat;

		// Token: 0x04000AD4 RID: 2772
		private SharedInteractionSystem _interactionSystem;

		// Token: 0x04000AD5 RID: 2773
		private SharedPopupSystem _popupSystem;

		// Token: 0x04000AD6 RID: 2774
		private SolutionContainerSystem _solutionSystem;

		// Token: 0x04000AD7 RID: 2775
		[DataField("targetKey", false, 1, true, false, null)]
		public string TargetKey = string.Empty;
	}
}
