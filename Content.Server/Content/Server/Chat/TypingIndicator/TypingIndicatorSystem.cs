using System;
using System.Runtime.CompilerServices;
using Content.Shared.ActionBlocker;
using Content.Shared.Chat.TypingIndicator;
using Robust.Server.GameObjects;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Log;
using Robust.Shared.Players;

namespace Content.Server.Chat.TypingIndicator
{
	// Token: 0x020006BD RID: 1725
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class TypingIndicatorSystem : SharedTypingIndicatorSystem
	{
		// Token: 0x060023DE RID: 9182 RVA: 0x000BAC04 File Offset: 0x000B8E04
		public override void Initialize()
		{
			base.Initialize();
			base.SubscribeLocalEvent<PlayerAttachedEvent>(new EntityEventHandler<PlayerAttachedEvent>(this.OnPlayerAttached), null, null);
			base.SubscribeLocalEvent<TypingIndicatorComponent, PlayerDetachedEvent>(new ComponentEventHandler<TypingIndicatorComponent, PlayerDetachedEvent>(this.OnPlayerDetached), null, null);
			base.SubscribeNetworkEvent<TypingChangedEvent>(new EntitySessionEventHandler<TypingChangedEvent>(this.OnClientTypingChanged), null, null);
		}

		// Token: 0x060023DF RID: 9183 RVA: 0x000BAC53 File Offset: 0x000B8E53
		private void OnPlayerAttached(PlayerAttachedEvent ev)
		{
			base.EnsureComp<TypingIndicatorComponent>(ev.Entity);
			base.EnsureComp<ServerAppearanceComponent>(ev.Entity);
		}

		// Token: 0x060023E0 RID: 9184 RVA: 0x000BAC6F File Offset: 0x000B8E6F
		private void OnPlayerDetached(EntityUid uid, TypingIndicatorComponent component, PlayerDetachedEvent args)
		{
			this.SetTypingIndicatorState(uid, TypingIndicatorState.None, null);
		}

		// Token: 0x060023E1 RID: 9185 RVA: 0x000BAC7C File Offset: 0x000B8E7C
		private void OnClientTypingChanged(TypingChangedEvent ev, EntitySessionEventArgs args)
		{
			EntityUid? uid = args.SenderSession.AttachedEntity;
			if (!base.Exists(uid))
			{
				DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(59, 1);
				defaultInterpolatedStringHandler.AppendLiteral("Client ");
				defaultInterpolatedStringHandler.AppendFormatted<ICommonSession>(args.SenderSession);
				defaultInterpolatedStringHandler.AppendLiteral(" sent TypingChangedEvent without an attached entity.");
				Logger.Warning(defaultInterpolatedStringHandler.ToStringAndClear());
				return;
			}
			if (!this._actionBlocker.CanEmote(uid.Value) && !this._actionBlocker.CanSpeak(uid.Value))
			{
				this.SetTypingIndicatorState(uid.Value, TypingIndicatorState.None, null);
				return;
			}
			this.SetTypingIndicatorState(uid.Value, ev.State, null);
		}

		// Token: 0x060023E2 RID: 9186 RVA: 0x000BAD28 File Offset: 0x000B8F28
		[NullableContext(2)]
		private void SetTypingIndicatorState(EntityUid uid, TypingIndicatorState state, AppearanceComponent appearance = null)
		{
			if (!base.Resolve<AppearanceComponent>(uid, ref appearance, false))
			{
				return;
			}
			appearance.SetData(TypingIndicatorVisuals.State, state);
		}

		// Token: 0x04001633 RID: 5683
		[Dependency]
		private readonly ActionBlockerSystem _actionBlocker;

		// Token: 0x04001634 RID: 5684
		[Dependency]
		private readonly SharedAppearanceSystem _appearance;
	}
}
