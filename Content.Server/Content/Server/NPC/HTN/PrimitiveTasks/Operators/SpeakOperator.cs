using System;
using System.Runtime.CompilerServices;
using Content.Server.Chat.Systems;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Localization;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Server.NPC.HTN.PrimitiveTasks.Operators
{
	// Token: 0x02000356 RID: 854
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class SpeakOperator : HTNOperator
	{
		// Token: 0x060011CB RID: 4555 RVA: 0x0005DC96 File Offset: 0x0005BE96
		public override void Initialize(IEntitySystemManager sysManager)
		{
			base.Initialize(sysManager);
			this._chat = IoCManager.Resolve<IEntitySystemManager>().GetEntitySystem<ChatSystem>();
		}

		// Token: 0x060011CC RID: 4556 RVA: 0x0005DCB0 File Offset: 0x0005BEB0
		public override HTNOperatorStatus Update(NPCBlackboard blackboard, float frameTime)
		{
			EntityUid speaker = blackboard.GetValue<EntityUid>("Owner");
			this._chat.TrySendInGameICMessage(speaker, Loc.GetString(this.Speech), InGameICChatType.Speak, false, false, null, null, null, true, false);
			return base.Update(blackboard, frameTime);
		}

		// Token: 0x04000ACC RID: 2764
		private ChatSystem _chat;

		// Token: 0x04000ACD RID: 2765
		[DataField("speech", false, 1, true, false, null)]
		public string Speech = string.Empty;
	}
}
