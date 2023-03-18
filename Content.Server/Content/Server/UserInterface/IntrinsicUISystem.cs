using System;
using System.Runtime.CompilerServices;
using Content.Server.Actions;
using Content.Shared.Actions;
using Robust.Server.GameObjects;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Log;

namespace Content.Server.UserInterface
{
	// Token: 0x020000FB RID: 251
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class IntrinsicUISystem : EntitySystem
	{
		// Token: 0x06000497 RID: 1175 RVA: 0x00015FD8 File Offset: 0x000141D8
		public override void Initialize()
		{
			base.SubscribeLocalEvent<IntrinsicUIComponent, ComponentStartup>(new ComponentEventHandler<IntrinsicUIComponent, ComponentStartup>(this.OnGetActions), null, null);
			base.SubscribeLocalEvent<IntrinsicUIComponent, ToggleIntrinsicUIEvent>(new ComponentEventHandler<IntrinsicUIComponent, ToggleIntrinsicUIEvent>(this.OnActionToggle), null, null);
		}

		// Token: 0x06000498 RID: 1176 RVA: 0x00016002 File Offset: 0x00014202
		private void OnActionToggle(EntityUid uid, IntrinsicUIComponent component, ToggleIntrinsicUIEvent args)
		{
			args.Handled = this.InteractUI(uid, args.Key, component, null);
		}

		// Token: 0x06000499 RID: 1177 RVA: 0x0001601C File Offset: 0x0001421C
		private void OnGetActions(EntityUid uid, IntrinsicUIComponent component, ComponentStartup args)
		{
			ActionsComponent actions;
			if (!base.TryComp<ActionsComponent>(uid, ref actions))
			{
				return;
			}
			foreach (IntrinsicUIEntry entry in component.UIs)
			{
				this._actionsSystem.AddAction(uid, entry.ToggleAction, null, actions, true);
			}
		}

		// Token: 0x0600049A RID: 1178 RVA: 0x00016094 File Offset: 0x00014294
		[NullableContext(2)]
		public bool InteractUI(EntityUid uid, Enum key, IntrinsicUIComponent iui = null, ActorComponent actor = null)
		{
			if (!base.Resolve<IntrinsicUIComponent, ActorComponent>(uid, ref iui, ref actor, true))
			{
				return false;
			}
			if (key == null)
			{
				string text = "bui";
				DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(36, 1);
				defaultInterpolatedStringHandler.AppendLiteral("Entity ");
				defaultInterpolatedStringHandler.AppendFormatted<EntityStringRepresentation>(base.ToPrettyString(uid));
				defaultInterpolatedStringHandler.AppendLiteral(" has an invalid intrinsic UI.");
				Logger.ErrorS(text, defaultInterpolatedStringHandler.ToStringAndClear());
			}
			BoundUserInterface ui = this.GetUIOrNull(uid, key, iui);
			if (ui == null)
			{
				string text2 = "bui";
				DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(20, 2);
				defaultInterpolatedStringHandler.AppendLiteral("Couldn't get UI ");
				defaultInterpolatedStringHandler.AppendFormatted<Enum>(key);
				defaultInterpolatedStringHandler.AppendLiteral(" on ");
				defaultInterpolatedStringHandler.AppendFormatted<EntityStringRepresentation>(base.ToPrettyString(uid));
				Logger.ErrorS(text2, defaultInterpolatedStringHandler.ToStringAndClear());
				return false;
			}
			IntrinsicUIOpenAttemptEvent attempt = new IntrinsicUIOpenAttemptEvent(uid, key);
			base.RaiseLocalEvent<IntrinsicUIOpenAttemptEvent>(uid, attempt, false);
			if (attempt.Cancelled)
			{
				return false;
			}
			ui.Toggle(actor.PlayerSession);
			return true;
		}

		// Token: 0x0600049B RID: 1179 RVA: 0x00016176 File Offset: 0x00014376
		[NullableContext(2)]
		private BoundUserInterface GetUIOrNull(EntityUid uid, Enum key, IntrinsicUIComponent component = null)
		{
			if (!base.Resolve<IntrinsicUIComponent>(uid, ref component, true))
			{
				return null;
			}
			if (key != null)
			{
				return uid.GetUIOrNull(key);
			}
			return null;
		}

		// Token: 0x040002B2 RID: 690
		[Dependency]
		private readonly ActionsSystem _actionsSystem;
	}
}
