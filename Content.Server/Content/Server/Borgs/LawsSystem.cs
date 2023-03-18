using System;
using System.Runtime.CompilerServices;
using Content.Server.Chat.Systems;
using Content.Shared.Borgs;
using Robust.Server.GameObjects;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Timing;

namespace Content.Server.Borgs
{
	// Token: 0x020000A6 RID: 166
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class LawsSystem : EntitySystem
	{
		// Token: 0x0600029A RID: 666 RVA: 0x0000DA9F File Offset: 0x0000BC9F
		public override void Initialize()
		{
			base.Initialize();
			base.SubscribeLocalEvent<LawsComponent, StateLawsMessage>(new ComponentEventHandler<LawsComponent, StateLawsMessage>(this.OnStateLaws), null, null);
			base.SubscribeLocalEvent<LawsComponent, PlayerAttachedEvent>(new ComponentEventHandler<LawsComponent, PlayerAttachedEvent>(this.OnPlayerAttached), null, null);
		}

		// Token: 0x0600029B RID: 667 RVA: 0x0000DACF File Offset: 0x0000BCCF
		private void OnStateLaws(EntityUid uid, LawsComponent component, StateLawsMessage args)
		{
			this.StateLaws(uid, component);
		}

		// Token: 0x0600029C RID: 668 RVA: 0x0000DADC File Offset: 0x0000BCDC
		private void OnPlayerAttached(EntityUid uid, LawsComponent component, PlayerAttachedEvent args)
		{
			BoundUserInterface ui;
			if (!this._uiSystem.TryGetUi(uid, LawsUiKey.Key, ref ui, null))
			{
				return;
			}
			this._uiSystem.TryOpen(uid, LawsUiKey.Key, args.Player, null);
		}

		// Token: 0x0600029D RID: 669 RVA: 0x0000DB1C File Offset: 0x0000BD1C
		[NullableContext(2)]
		public void StateLaws(EntityUid uid, LawsComponent component = null)
		{
			if (!base.Resolve<LawsComponent>(uid, ref component, true))
			{
				return;
			}
			if (!component.CanState)
			{
				return;
			}
			if (component.StateTime != null && this._timing.CurTime < component.StateTime)
			{
				return;
			}
			component.StateTime = new TimeSpan?(this._timing.CurTime + component.StateCD);
			foreach (string law in component.Laws)
			{
				this._chat.TrySendInGameICMessage(uid, law, InGameICChatType.Speak, false, false, null, null, null, true, false);
			}
		}

		// Token: 0x0600029E RID: 670 RVA: 0x0000DBF0 File Offset: 0x0000BDF0
		[NullableContext(2)]
		public void ClearLaws(EntityUid uid, LawsComponent component = null)
		{
			if (!base.Resolve<LawsComponent>(uid, ref component, false))
			{
				return;
			}
			component.Laws.Clear();
			base.Dirty(component, null);
		}

		// Token: 0x0600029F RID: 671 RVA: 0x0000DC14 File Offset: 0x0000BE14
		public void AddLaw(EntityUid uid, string law, int? index = null, [Nullable(2)] LawsComponent component = null)
		{
			if (!base.Resolve<LawsComponent>(uid, ref component, false))
			{
				return;
			}
			if (index == null)
			{
				index = new int?(component.Laws.Count);
			}
			index = new int?(Math.Clamp(index.Value, 0, component.Laws.Count));
			component.Laws.Insert(index.Value, law);
			base.Dirty(component, null);
		}

		// Token: 0x060002A0 RID: 672 RVA: 0x0000DC88 File Offset: 0x0000BE88
		[NullableContext(2)]
		public void RemoveLaw(EntityUid uid, int? index = null, LawsComponent component = null)
		{
			if (!base.Resolve<LawsComponent>(uid, ref component, false))
			{
				return;
			}
			if (index == null)
			{
				index = new int?(component.Laws.Count);
			}
			if (component.Laws.Count == 0)
			{
				return;
			}
			index = new int?(Math.Clamp(index.Value, 0, component.Laws.Count - 1));
			int? num = index;
			int num2 = 0;
			if (num.GetValueOrDefault() < num2 & num != null)
			{
				return;
			}
			component.Laws.RemoveAt(index.Value);
			base.Dirty(component, null);
		}

		// Token: 0x040001D6 RID: 470
		[Dependency]
		private readonly ChatSystem _chat;

		// Token: 0x040001D7 RID: 471
		[Dependency]
		private readonly IGameTiming _timing;

		// Token: 0x040001D8 RID: 472
		[Dependency]
		private readonly UserInterfaceSystem _uiSystem;
	}
}
