using System;
using System.Runtime.CompilerServices;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Serialization;

namespace Content.Shared.Emoting
{
	// Token: 0x020004C4 RID: 1220
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class EmoteSystem : EntitySystem
	{
		// Token: 0x06000EB6 RID: 3766 RVA: 0x0002F50C File Offset: 0x0002D70C
		public override void Initialize()
		{
			base.Initialize();
			base.SubscribeLocalEvent<EmoteAttemptEvent>(new EntityEventHandler<EmoteAttemptEvent>(this.OnEmoteAttempt), null, null);
			base.SubscribeLocalEvent<EmotingComponent, ComponentGetState>(new ComponentEventRefHandler<EmotingComponent, ComponentGetState>(this.OnEmotingGetState), null, null);
			base.SubscribeLocalEvent<EmotingComponent, ComponentHandleState>(new ComponentEventRefHandler<EmotingComponent, ComponentHandleState>(this.OnEmotingHandleState), null, null);
		}

		// Token: 0x06000EB7 RID: 3767 RVA: 0x0002F55B File Offset: 0x0002D75B
		[NullableContext(2)]
		public void SetEmoting(EntityUid uid, bool value, EmotingComponent component = null)
		{
			if (value && !base.Resolve<EmotingComponent>(uid, ref component, true))
			{
				return;
			}
			component = base.EnsureComp<EmotingComponent>(uid);
			if (component.Enabled == value)
			{
				return;
			}
			base.Dirty(component, null);
		}

		// Token: 0x06000EB8 RID: 3768 RVA: 0x0002F588 File Offset: 0x0002D788
		private void OnEmotingHandleState(EntityUid uid, EmotingComponent component, ref ComponentHandleState args)
		{
			EmoteSystem.EmotingComponentState state = args.Current as EmoteSystem.EmotingComponentState;
			if (state == null)
			{
				return;
			}
			component.Enabled = state.Enabled;
		}

		// Token: 0x06000EB9 RID: 3769 RVA: 0x0002F5B1 File Offset: 0x0002D7B1
		private void OnEmotingGetState(EntityUid uid, EmotingComponent component, ref ComponentGetState args)
		{
			args.State = new EmoteSystem.EmotingComponentState(component.Enabled);
		}

		// Token: 0x06000EBA RID: 3770 RVA: 0x0002F5C4 File Offset: 0x0002D7C4
		private void OnEmoteAttempt(EmoteAttemptEvent args)
		{
			EmotingComponent emote;
			if (!base.TryComp<EmotingComponent>(args.Uid, ref emote) || !emote.Enabled)
			{
				args.Cancel();
			}
		}

		// Token: 0x02000819 RID: 2073
		[NullableContext(0)]
		[NetSerializable]
		[Serializable]
		private sealed class EmotingComponentState : ComponentState
		{
			// Token: 0x17000513 RID: 1299
			// (get) Token: 0x060018EF RID: 6383 RVA: 0x0004F1CB File Offset: 0x0004D3CB
			public bool Enabled { get; }

			// Token: 0x060018F0 RID: 6384 RVA: 0x0004F1D3 File Offset: 0x0004D3D3
			public EmotingComponentState(bool enabled)
			{
				this.Enabled = enabled;
			}
		}
	}
}
