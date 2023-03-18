using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Content.Server.Chat.Systems;
using Content.Server.Ghost.Components;
using Content.Shared.Humanoid;
using Content.Shared.Mobs;
using Robust.Shared.Audio;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Localization;
using Robust.Shared.Random;

namespace Content.Server.White.Other
{
	// Token: 0x02000093 RID: 147
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class OnDeath : EntitySystem
	{
		// Token: 0x06000241 RID: 577 RVA: 0x0000C66D File Offset: 0x0000A86D
		public override void Initialize()
		{
			base.SubscribeLocalEvent<HumanoidAppearanceComponent, MobStateChangedEvent>(new ComponentEventHandler<HumanoidAppearanceComponent, MobStateChangedEvent>(this.HandleDeathEvent), null, null);
			base.SubscribeLocalEvent<GhostComponent, ComponentInit>(new ComponentEventHandler<GhostComponent, ComponentInit>(this.OnGhosted), null, null);
		}

		// Token: 0x06000242 RID: 578 RVA: 0x0000C698 File Offset: 0x0000A898
		private void HandleDeathEvent(EntityUid uid, HumanoidAppearanceComponent component, MobStateChangedEvent args)
		{
			switch (args.NewMobState)
			{
			case MobState.Invalid:
				this.StopPlayingStream(uid);
				return;
			case MobState.Alive:
				this.StopPlayingStream(uid);
				return;
			case MobState.Critical:
				this.PlayPlayingStream(uid);
				return;
			case MobState.Dead:
			{
				this.StopPlayingStream(uid);
				string deathGaspMessage = this.SelectRandomDeathGaspMessage();
				string localizedMessage = this.LocalizeDeathGaspMessage(deathGaspMessage);
				this.SendDeathGaspMessage(uid, localizedMessage);
				this.PlayDeathSound(uid);
				return;
			}
			default:
				return;
			}
		}

		// Token: 0x06000243 RID: 579 RVA: 0x0000C704 File Offset: 0x0000A904
		private void PlayPlayingStream(EntityUid uid)
		{
			IPlayingAudioStream currentStream;
			if (this._playingStreams.TryGetValue(uid, out currentStream))
			{
				currentStream.Stop();
			}
			IPlayingAudioStream newStream = this._audio.PlayEntity(OnDeath.HeartSounds, uid, uid, new AudioParams?(AudioParams.Default.WithLoop(true)));
			if (newStream != null)
			{
				this._playingStreams[uid] = newStream;
			}
		}

		// Token: 0x06000244 RID: 580 RVA: 0x0000C75C File Offset: 0x0000A95C
		private void StopPlayingStream(EntityUid uid)
		{
			IPlayingAudioStream currentStream;
			if (this._playingStreams.TryGetValue(uid, out currentStream))
			{
				currentStream.Stop();
				this._playingStreams.Remove(uid);
			}
		}

		// Token: 0x06000245 RID: 581 RVA: 0x0000C78C File Offset: 0x0000A98C
		private string SelectRandomDeathGaspMessage()
		{
			return OnDeath.DeathGaspMessages[this._random.Next(OnDeath.DeathGaspMessages.Length)];
		}

		// Token: 0x06000246 RID: 582 RVA: 0x0000C7A6 File Offset: 0x0000A9A6
		private string LocalizeDeathGaspMessage(string message)
		{
			return Loc.GetString(message);
		}

		// Token: 0x06000247 RID: 583 RVA: 0x0000C7B0 File Offset: 0x0000A9B0
		private void SendDeathGaspMessage(EntityUid uid, string message)
		{
			this._chat.TrySendInGameICMessage(uid, message, InGameICChatType.Emote, false, false, null, null, null, true, true);
		}

		// Token: 0x06000248 RID: 584 RVA: 0x0000C7D2 File Offset: 0x0000A9D2
		private void PlayDeathSound(EntityUid uid)
		{
			this._audio.PlayEntity(OnDeath.DeathSounds, uid, uid, new AudioParams?(AudioParams.Default));
		}

		// Token: 0x06000249 RID: 585 RVA: 0x0000C7F1 File Offset: 0x0000A9F1
		private void OnGhosted(EntityUid uid, GhostComponent component, ComponentInit args)
		{
			this.StopPlayingStream(uid);
		}

		// Token: 0x04000199 RID: 409
		[Dependency]
		private readonly ChatSystem _chat;

		// Token: 0x0400019A RID: 410
		[Dependency]
		private readonly IRobustRandom _random;

		// Token: 0x0400019B RID: 411
		[Dependency]
		private readonly SharedAudioSystem _audio;

		// Token: 0x0400019C RID: 412
		private readonly Dictionary<EntityUid, IPlayingAudioStream> _playingStreams = new Dictionary<EntityUid, IPlayingAudioStream>();

		// Token: 0x0400019D RID: 413
		private static readonly SoundSpecifier DeathSounds = new SoundCollectionSpecifier("deathSounds", null);

		// Token: 0x0400019E RID: 414
		private static readonly SoundSpecifier HeartSounds = new SoundCollectionSpecifier("heartSounds", null);

		// Token: 0x0400019F RID: 415
		private static readonly string[] DeathGaspMessages = new string[]
		{
			"death-gasp-high",
			"death-gasp-medium",
			"death-gasp-normal"
		};
	}
}
