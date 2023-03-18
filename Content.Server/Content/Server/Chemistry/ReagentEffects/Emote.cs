using System;
using System.Runtime.CompilerServices;
using Content.Server.Chat.Systems;
using Content.Shared.Chat.Prototypes;
using Content.Shared.Chemistry.Reagent;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom.Prototype;

namespace Content.Server.Chemistry.ReagentEffects
{
	// Token: 0x02000662 RID: 1634
	public sealed class Emote : ReagentEffect
	{
		// Token: 0x06002265 RID: 8805 RVA: 0x000B3D58 File Offset: 0x000B1F58
		public override void Effect(ReagentEffectArgs args)
		{
			if (this.EmoteId == null)
			{
				return;
			}
			ChatSystem chatSys = args.EntityManager.System<ChatSystem>();
			if (this.ShowInChat)
			{
				chatSys.TryEmoteWithChat(args.SolutionEntity, this.EmoteId, false, true, null);
				return;
			}
			chatSys.TryEmoteWithoutChat(args.SolutionEntity, this.EmoteId);
		}

		// Token: 0x04001547 RID: 5447
		[Nullable(2)]
		[DataField("emote", false, 1, false, false, typeof(PrototypeIdSerializer<EmotePrototype>))]
		public string EmoteId;

		// Token: 0x04001548 RID: 5448
		[DataField("showInChat", false, 1, false, false, null)]
		public bool ShowInChat;
	}
}
