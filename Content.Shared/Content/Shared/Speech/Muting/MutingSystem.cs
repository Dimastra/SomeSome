using System;
using System.Runtime.CompilerServices;
using Content.Shared.Popups;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Localization;

namespace Content.Shared.Speech.Muting
{
	// Token: 0x0200017F RID: 383
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class MutingSystem : EntitySystem
	{
		// Token: 0x0600049F RID: 1183 RVA: 0x000120BF File Offset: 0x000102BF
		public override void Initialize()
		{
			base.Initialize();
			base.SubscribeLocalEvent<MutedComponent, SpeakAttemptEvent>(new ComponentEventHandler<MutedComponent, SpeakAttemptEvent>(this.OnSpeakAttempt), null, null);
		}

		// Token: 0x060004A0 RID: 1184 RVA: 0x000120DB File Offset: 0x000102DB
		private void OnSpeakAttempt(EntityUid uid, MutedComponent component, SpeakAttemptEvent args)
		{
			this._popupSystem.PopupEntity(Loc.GetString("speech-muted"), uid, uid, PopupType.Small);
			args.Cancel();
		}

		// Token: 0x0400044F RID: 1103
		[Dependency]
		private readonly SharedPopupSystem _popupSystem;
	}
}
