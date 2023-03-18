using System;
using System.Runtime.CompilerServices;
using Content.Server.Chat.Systems;
using Content.Server.VoiceMask;
using Content.Shared.Interaction.Components;
using Robust.Shared.GameObjects;

namespace Content.Server.Chat
{
	// Token: 0x020006BA RID: 1722
	public sealed class FunnyFontsChatSystem : EntitySystem
	{
		// Token: 0x060023CE RID: 9166 RVA: 0x000BA6DB File Offset: 0x000B88DB
		public override void Initialize()
		{
			base.Initialize();
			base.SubscribeLocalEvent<SpeechTransformedEvent>(new EntityEventHandler<SpeechTransformedEvent>(this.OnEntitySpeak), null, null);
		}

		// Token: 0x060023CF RID: 9167 RVA: 0x000BA6F8 File Offset: 0x000B88F8
		[NullableContext(1)]
		private void OnEntitySpeak(SpeechTransformedEvent ev)
		{
			VoiceMaskComponent mask;
			if (base.TryComp<VoiceMaskComponent>(ev.Sender, ref mask) && mask.Enabled)
			{
				return;
			}
			ClumsyComponent clumsyComponent;
			if (base.TryComp<ClumsyComponent>(ev.Sender, ref clumsyComponent))
			{
				ev.Message = "[font=\"ComicSansMS\"]" + ev.Message + "[/font]";
			}
		}
	}
}
