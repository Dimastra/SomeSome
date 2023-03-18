using System;
using System.Runtime.CompilerServices;
using Content.Server.Chat.Systems;
using Content.Shared.Speech;
using Robust.Shared.Audio;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Localization;
using Robust.Shared.Prototypes;
using Robust.Shared.Random;
using Robust.Shared.Timing;

namespace Content.Server.SurveillanceCamera
{
	// Token: 0x02000145 RID: 325
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class SurveillanceCameraSpeakerSystem : EntitySystem
	{
		// Token: 0x06000629 RID: 1577 RVA: 0x0001D8E4 File Offset: 0x0001BAE4
		public override void Initialize()
		{
			base.SubscribeLocalEvent<SurveillanceCameraSpeakerComponent, SurveillanceCameraSpeechSendEvent>(new ComponentEventHandler<SurveillanceCameraSpeakerComponent, SurveillanceCameraSpeechSendEvent>(this.OnSpeechSent), null, null);
		}

		// Token: 0x0600062A RID: 1578 RVA: 0x0001D8FC File Offset: 0x0001BAFC
		private void OnSpeechSent(EntityUid uid, SurveillanceCameraSpeakerComponent component, SurveillanceCameraSpeechSendEvent args)
		{
			if (!component.SpeechEnabled)
			{
				return;
			}
			TimeSpan time = this._gameTiming.CurTime;
			TimeSpan cd = TimeSpan.FromSeconds((double)component.SpeechSoundCooldown);
			SpeechComponent speech;
			SpeechSoundsPrototype speechProto;
			if (time - component.LastSoundPlayed < cd && base.TryComp<SpeechComponent>(args.Speaker, ref speech) && speech.SpeechSounds != null && this._prototypeManager.TryIndex<SpeechSoundsPrototype>(speech.SpeechSounds, ref speechProto))
			{
				string message = args.Message;
				char c = message[message.Length - 1];
				SoundSpecifier soundSpecifier;
				if (c != '!')
				{
					if (c == '?')
					{
						soundSpecifier = speechProto.AskSound;
					}
					else
					{
						soundSpecifier = speechProto.SaySound;
					}
				}
				else
				{
					soundSpecifier = speechProto.ExclaimSound;
				}
				SoundSpecifier sound = soundSpecifier;
				int uppercase = 0;
				for (int i = 0; i < args.Message.Length; i++)
				{
					if (char.IsUpper(args.Message[i]))
					{
						uppercase++;
					}
				}
				if (uppercase > args.Message.Length / 2)
				{
					sound = speechProto.ExclaimSound;
				}
				float scale = (float)RandomExtensions.NextGaussian(this._random, 1.0, (double)speechProto.Variation);
				AudioParams param = speech.AudioParams.WithPitchScale(scale);
				this._audioSystem.PlayPvs(sound, uid, new AudioParams?(param));
				component.LastSoundPlayed = time;
			}
			TransformSpeakerNameEvent nameEv = new TransformSpeakerNameEvent(args.Speaker, base.Name(args.Speaker, null));
			base.RaiseLocalEvent<TransformSpeakerNameEvent>(args.Speaker, nameEv, false);
			string name = Loc.GetString("speech-name-relay", new ValueTuple<string, object>[]
			{
				new ValueTuple<string, object>("speaker", base.Name(uid, null)),
				new ValueTuple<string, object>("originalName", nameEv.Name)
			});
			bool hideGlobalGhostChat = true;
			this._chatSystem.TrySendInGameICMessage(uid, args.Message, InGameICChatType.Speak, false, hideGlobalGhostChat, null, null, name, true, false);
		}

		// Token: 0x04000392 RID: 914
		[Dependency]
		private readonly SharedAudioSystem _audioSystem;

		// Token: 0x04000393 RID: 915
		[Dependency]
		private readonly ChatSystem _chatSystem;

		// Token: 0x04000394 RID: 916
		[Dependency]
		private readonly IGameTiming _gameTiming;

		// Token: 0x04000395 RID: 917
		[Dependency]
		private readonly IPrototypeManager _prototypeManager;

		// Token: 0x04000396 RID: 918
		[Dependency]
		private readonly IRobustRandom _random;
	}
}
