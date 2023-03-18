using System;
using System.Runtime.CompilerServices;
using Content.Server.Chat.Systems;
using Content.Server.Emoting.Components;
using Content.Server.Hands.Components;
using Content.Shared.Chat.Prototypes;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Prototypes;

namespace Content.Server.Emoting.Systems
{
	// Token: 0x0200052F RID: 1327
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class BodyEmotesSystem : EntitySystem
	{
		// Token: 0x06001B9E RID: 7070 RVA: 0x00093B26 File Offset: 0x00091D26
		public override void Initialize()
		{
			base.Initialize();
			base.SubscribeLocalEvent<BodyEmotesComponent, ComponentStartup>(new ComponentEventHandler<BodyEmotesComponent, ComponentStartup>(this.OnStartup), null, null);
			base.SubscribeLocalEvent<BodyEmotesComponent, EmoteEvent>(new ComponentEventRefHandler<BodyEmotesComponent, EmoteEvent>(this.OnEmote), null, null);
		}

		// Token: 0x06001B9F RID: 7071 RVA: 0x00093B56 File Offset: 0x00091D56
		private void OnStartup(EntityUid uid, BodyEmotesComponent component, ComponentStartup args)
		{
			if (component.SoundsId == null)
			{
				return;
			}
			this._proto.TryIndex<EmoteSoundsPrototype>(component.SoundsId, ref component.Sounds);
		}

		// Token: 0x06001BA0 RID: 7072 RVA: 0x00093B7C File Offset: 0x00091D7C
		private void OnEmote(EntityUid uid, BodyEmotesComponent component, ref EmoteEvent args)
		{
			if (args.Handled)
			{
				return;
			}
			EmoteCategory cat = args.Emote.Category;
			if (cat.HasFlag(EmoteCategory.Gesture))
			{
				args.Handled = this.TryEmoteHands(uid, args.Emote, component);
			}
		}

		// Token: 0x06001BA1 RID: 7073 RVA: 0x00093BC8 File Offset: 0x00091DC8
		private bool TryEmoteHands(EntityUid uid, EmotePrototype emote, BodyEmotesComponent component)
		{
			HandsComponent hands;
			return base.TryComp<HandsComponent>(uid, ref hands) && hands.Count > 0 && this._chat.TryPlayEmoteSound(uid, component.Sounds, emote);
		}

		// Token: 0x040011B6 RID: 4534
		[Dependency]
		private readonly IPrototypeManager _proto;

		// Token: 0x040011B7 RID: 4535
		[Dependency]
		private readonly ChatSystem _chat;
	}
}
