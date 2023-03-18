using System;
using System.Runtime.CompilerServices;
using Content.Server.Chat.Systems;
using Content.Shared.Actions;
using Content.Shared.Actions.ActionTypes;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Localization;

namespace Content.Server.Actions
{
	// Token: 0x02000875 RID: 2165
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class ActionsSystem : SharedActionsSystem
	{
		// Token: 0x06002F50 RID: 12112 RVA: 0x000F4D30 File Offset: 0x000F2F30
		protected override bool PerformBasicActions(EntityUid user, ActionType action, bool predicted)
		{
			bool result = base.PerformBasicActions(user, action, predicted);
			if (!string.IsNullOrWhiteSpace(action.Speech))
			{
				this._chat.TrySendInGameICMessage(user, Loc.GetString(action.Speech), InGameICChatType.Speak, false, false, null, null, null, true, false);
				result = true;
			}
			return result;
		}

		// Token: 0x04001C74 RID: 7284
		[Dependency]
		private readonly ChatSystem _chat;

		// Token: 0x04001C75 RID: 7285
		[Dependency]
		private readonly MetaDataSystem _metaSystem;
	}
}
