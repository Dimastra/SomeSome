using System;
using System.Runtime.CompilerServices;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Serialization;

namespace Content.Shared.Speech
{
	// Token: 0x0200017D RID: 381
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class SpeechSystem : EntitySystem
	{
		// Token: 0x06000498 RID: 1176 RVA: 0x00011FCC File Offset: 0x000101CC
		public override void Initialize()
		{
			base.Initialize();
			base.SubscribeLocalEvent<SpeakAttemptEvent>(new EntityEventHandler<SpeakAttemptEvent>(this.OnSpeakAttempt), null, null);
			base.SubscribeLocalEvent<SpeechComponent, ComponentGetState>(new ComponentEventRefHandler<SpeechComponent, ComponentGetState>(this.OnSpeechGetState), null, null);
			base.SubscribeLocalEvent<SpeechComponent, ComponentHandleState>(new ComponentEventRefHandler<SpeechComponent, ComponentHandleState>(this.OnSpeechHandleState), null, null);
		}

		// Token: 0x06000499 RID: 1177 RVA: 0x0001201B File Offset: 0x0001021B
		[NullableContext(2)]
		public void SetSpeech(EntityUid uid, bool value, SpeechComponent component = null)
		{
			if (value && !base.Resolve<SpeechComponent>(uid, ref component, true))
			{
				return;
			}
			component = base.EnsureComp<SpeechComponent>(uid);
			if (component.Enabled == value)
			{
				return;
			}
			base.Dirty(component, null);
		}

		// Token: 0x0600049A RID: 1178 RVA: 0x00012048 File Offset: 0x00010248
		private void OnSpeechHandleState(EntityUid uid, SpeechComponent component, ref ComponentHandleState args)
		{
			SpeechSystem.SpeechComponentState state = args.Current as SpeechSystem.SpeechComponentState;
			if (state == null)
			{
				return;
			}
			component.Enabled = state.Enabled;
		}

		// Token: 0x0600049B RID: 1179 RVA: 0x00012071 File Offset: 0x00010271
		private void OnSpeechGetState(EntityUid uid, SpeechComponent component, ref ComponentGetState args)
		{
			args.State = new SpeechSystem.SpeechComponentState(component.Enabled);
		}

		// Token: 0x0600049C RID: 1180 RVA: 0x00012084 File Offset: 0x00010284
		private void OnSpeakAttempt(SpeakAttemptEvent args)
		{
			SpeechComponent speech;
			if (!base.TryComp<SpeechComponent>(args.Uid, ref speech) || !speech.Enabled)
			{
				args.Cancel();
			}
		}

		// Token: 0x020007A5 RID: 1957
		[NullableContext(0)]
		[NetSerializable]
		[Serializable]
		private sealed class SpeechComponentState : ComponentState
		{
			// Token: 0x060017EB RID: 6123 RVA: 0x0004D30C File Offset: 0x0004B50C
			public SpeechComponentState(bool enabled)
			{
				this.Enabled = enabled;
			}

			// Token: 0x040017C4 RID: 6084
			public readonly bool Enabled;
		}
	}
}
