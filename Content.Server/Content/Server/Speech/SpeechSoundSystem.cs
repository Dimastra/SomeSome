using System;
using System.Runtime.CompilerServices;
using Content.Server.Chat.Systems;
using Content.Shared.Speech;
using Robust.Shared.Audio;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Player;
using Robust.Shared.Prototypes;
using Robust.Shared.Random;
using Robust.Shared.Timing;

namespace Content.Server.Speech
{
	// Token: 0x020001B1 RID: 433
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class SpeechSoundSystem : EntitySystem
	{
		// Token: 0x0600087F RID: 2175 RVA: 0x0002B50F File Offset: 0x0002970F
		public override void Initialize()
		{
			base.Initialize();
			base.SubscribeLocalEvent<SpeechComponent, EntitySpokeEvent>(new ComponentEventHandler<SpeechComponent, EntitySpokeEvent>(this.OnEntitySpoke), null, null);
		}

		// Token: 0x06000880 RID: 2176 RVA: 0x0002B52C File Offset: 0x0002972C
		private void OnEntitySpoke(EntityUid uid, SpeechComponent component, EntitySpokeEvent args)
		{
			if (component.SpeechSounds == null)
			{
				return;
			}
			TimeSpan currentTime = this._gameTiming.CurTime;
			TimeSpan cooldown = TimeSpan.FromSeconds((double)component.SoundCooldownTime);
			if (currentTime - component.LastTimeSoundPlayed < cooldown)
			{
				return;
			}
			SpeechSoundsPrototype prototype = this._protoManager.Index<SpeechSoundsPrototype>(component.SpeechSounds);
			string message = args.Message;
			string message2 = args.Message;
			char c = message2[message2.Length - 1];
			string contextSound;
			if (c != '!')
			{
				if (c == '?')
				{
					contextSound = prototype.AskSound.GetSound(null, null);
				}
				else
				{
					contextSound = prototype.SaySound.GetSound(null, null);
				}
			}
			else
			{
				contextSound = prototype.ExclaimSound.GetSound(null, null);
			}
			int uppercaseCount = 0;
			for (int i = 0; i < message.Length; i++)
			{
				if (char.IsUpper(message[i]))
				{
					uppercaseCount++;
				}
			}
			if (uppercaseCount > message.Length / 2)
			{
				contextSound = prototype.ExclaimSound.GetSound(null, null);
			}
			float scale = (float)RandomExtensions.NextGaussian(this._random, 1.0, (double)prototype.Variation);
			AudioParams pitchedAudioParams = component.AudioParams.WithPitchScale(scale);
			component.LastTimeSoundPlayed = currentTime;
			SoundSystem.Play(contextSound, Filter.Pvs(uid, 2f, this.EntityManager, null, null), uid, new AudioParams?(pitchedAudioParams));
		}

		// Token: 0x04000532 RID: 1330
		[Dependency]
		private readonly IGameTiming _gameTiming;

		// Token: 0x04000533 RID: 1331
		[Dependency]
		private readonly IPrototypeManager _protoManager;

		// Token: 0x04000534 RID: 1332
		[Dependency]
		private readonly IRobustRandom _random;
	}
}
