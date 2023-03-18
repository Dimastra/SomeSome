using System;
using System.Runtime.CompilerServices;
using Content.Server.Administration.Logs;
using Content.Server.Tools.Components;
using Content.Shared.Administration.Logs;
using Content.Shared.Database;
using Content.Shared.Examine;
using Content.Shared.Interaction;
using Content.Shared.Tools;
using Content.Shared.Tools.Components;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Localization;
using Robust.Shared.Map;

namespace Content.Server.Tools.Systems
{
	// Token: 0x02000113 RID: 275
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class WeldableSystem : EntitySystem
	{
		// Token: 0x06000500 RID: 1280 RVA: 0x0001855C File Offset: 0x0001675C
		public override void Initialize()
		{
			base.Initialize();
			base.SubscribeLocalEvent<WeldableComponent, InteractUsingEvent>(new ComponentEventHandler<WeldableComponent, InteractUsingEvent>(this.OnInteractUsing), null, null);
			base.SubscribeLocalEvent<WeldableComponent, WeldableSystem.WeldFinishedEvent>(new ComponentEventHandler<WeldableComponent, WeldableSystem.WeldFinishedEvent>(this.OnWeldFinished), null, null);
			base.SubscribeLocalEvent<WeldableComponent, WeldableSystem.WeldCancelledEvent>(new ComponentEventHandler<WeldableComponent, WeldableSystem.WeldCancelledEvent>(this.OnWeldCanceled), null, null);
			base.SubscribeLocalEvent<WeldableComponent, ExaminedEvent>(new ComponentEventHandler<WeldableComponent, ExaminedEvent>(this.OnExamine), null, null);
		}

		// Token: 0x06000501 RID: 1281 RVA: 0x000185BF File Offset: 0x000167BF
		private void OnExamine(EntityUid uid, WeldableComponent component, ExaminedEvent args)
		{
			if (component.IsWelded && component.WeldedExamineMessage != null)
			{
				args.PushText(Loc.GetString(component.WeldedExamineMessage));
			}
		}

		// Token: 0x06000502 RID: 1282 RVA: 0x000185E2 File Offset: 0x000167E2
		private void OnInteractUsing(EntityUid uid, WeldableComponent component, InteractUsingEvent args)
		{
			if (args.Handled)
			{
				return;
			}
			args.Handled = this.TryWeld(uid, args.Used, args.User, component);
		}

		// Token: 0x06000503 RID: 1283 RVA: 0x00018608 File Offset: 0x00016808
		[NullableContext(2)]
		private bool CanWeld(EntityUid uid, EntityUid tool, EntityUid user, WeldableComponent component = null)
		{
			if (!base.Resolve<WeldableComponent>(uid, ref component, true))
			{
				return false;
			}
			if (!component.Weldable || component.BeingWelded)
			{
				return false;
			}
			if (!this._toolSystem.HasQuality(tool, component.WeldingQuality, null))
			{
				return false;
			}
			WeldableAttemptEvent attempt = new WeldableAttemptEvent(user, tool);
			base.RaiseLocalEvent<WeldableAttemptEvent>(uid, attempt, true);
			return !attempt.Cancelled;
		}

		// Token: 0x06000504 RID: 1284 RVA: 0x0001866C File Offset: 0x0001686C
		[NullableContext(2)]
		private bool TryWeld(EntityUid uid, EntityUid tool, EntityUid user, WeldableComponent component = null)
		{
			if (!base.Resolve<WeldableComponent>(uid, ref component, true))
			{
				return false;
			}
			if (!this.CanWeld(uid, tool, user, component))
			{
				return false;
			}
			ToolEventData toolEvData = new ToolEventData(new WeldableSystem.WeldFinishedEvent(user, tool), 0f, null, new EntityUid?(uid));
			component.BeingWelded = this._toolSystem.UseTool(tool, user, new EntityUid?(uid), (float)component.WeldingTime.Seconds, new string[]
			{
				component.WeldingQuality
			}, toolEvData, component.FuelConsumption, null, null, null);
			ISharedAdminLogManager adminLogger = this._adminLogger;
			LogType type = LogType.Action;
			LogImpact impact = LogImpact.Low;
			LogStringHandler logStringHandler = new LogStringHandler(16, 4);
			logStringHandler.AppendFormatted<EntityStringRepresentation>(base.ToPrettyString(user), "user", "ToPrettyString(user)");
			logStringHandler.AppendLiteral(" is ");
			logStringHandler.AppendFormatted(component.IsWelded ? "un" : "");
			logStringHandler.AppendLiteral("welding ");
			logStringHandler.AppendFormatted<EntityStringRepresentation>(base.ToPrettyString(uid), "target", "ToPrettyString(uid)");
			logStringHandler.AppendLiteral(" at ");
			logStringHandler.AppendFormatted<EntityCoordinates>(base.Transform(uid).Coordinates, "targetlocation", "Transform(uid).Coordinates");
			adminLogger.Add(type, impact, ref logStringHandler);
			return true;
		}

		// Token: 0x06000505 RID: 1285 RVA: 0x0001879C File Offset: 0x0001699C
		private void OnWeldFinished(EntityUid uid, WeldableComponent component, WeldableSystem.WeldFinishedEvent args)
		{
			component.BeingWelded = false;
			if (!this.CanWeld(uid, args.Tool, args.User, component))
			{
				return;
			}
			component.IsWelded = !component.IsWelded;
			base.RaiseLocalEvent<WeldableChangedEvent>(uid, new WeldableChangedEvent(component.IsWelded), true);
			this.UpdateAppearance(uid, component);
			ISharedAdminLogManager adminLogger = this._adminLogger;
			LogType type = LogType.Action;
			LogImpact impact = LogImpact.Low;
			LogStringHandler logStringHandler = new LogStringHandler(8, 3);
			logStringHandler.AppendFormatted<EntityStringRepresentation>(base.ToPrettyString(args.User), "user", "ToPrettyString(args.User)");
			logStringHandler.AppendLiteral(" ");
			logStringHandler.AppendFormatted((!component.IsWelded) ? "un" : "");
			logStringHandler.AppendLiteral("welded ");
			logStringHandler.AppendFormatted<EntityStringRepresentation>(base.ToPrettyString(uid), "target", "ToPrettyString(uid)");
			adminLogger.Add(type, impact, ref logStringHandler);
		}

		// Token: 0x06000506 RID: 1286 RVA: 0x00018872 File Offset: 0x00016A72
		private void OnWeldCanceled(EntityUid uid, WeldableComponent component, WeldableSystem.WeldCancelledEvent args)
		{
			component.BeingWelded = false;
		}

		// Token: 0x06000507 RID: 1287 RVA: 0x0001887C File Offset: 0x00016A7C
		[NullableContext(2)]
		private void UpdateAppearance(EntityUid uid, WeldableComponent component = null)
		{
			if (!base.Resolve<WeldableComponent>(uid, ref component, true))
			{
				return;
			}
			AppearanceComponent appearance;
			if (!base.TryComp<AppearanceComponent>(uid, ref appearance))
			{
				return;
			}
			this._appearance.SetData(uid, WeldableVisuals.IsWelded, component.IsWelded, appearance);
		}

		// Token: 0x06000508 RID: 1288 RVA: 0x000188C0 File Offset: 0x00016AC0
		[NullableContext(2)]
		public void ForceWeldedState(EntityUid uid, bool state, WeldableComponent component = null)
		{
			if (!base.Resolve<WeldableComponent>(uid, ref component, true))
			{
				return;
			}
			component.IsWelded = state;
			base.RaiseLocalEvent<WeldableChangedEvent>(uid, new WeldableChangedEvent(component.IsWelded), false);
			this.UpdateAppearance(uid, component);
		}

		// Token: 0x06000509 RID: 1289 RVA: 0x000188F1 File Offset: 0x00016AF1
		[NullableContext(2)]
		public void SetWeldingTime(EntityUid uid, TimeSpan time, WeldableComponent component = null)
		{
			if (!base.Resolve<WeldableComponent>(uid, ref component, true))
			{
				return;
			}
			component.WeldingTime = time;
		}

		// Token: 0x040002E9 RID: 745
		[Dependency]
		private readonly IAdminLogManager _adminLogger;

		// Token: 0x040002EA RID: 746
		[Dependency]
		private readonly SharedToolSystem _toolSystem;

		// Token: 0x040002EB RID: 747
		[Dependency]
		private readonly SharedAppearanceSystem _appearance;

		// Token: 0x020008D8 RID: 2264
		[NullableContext(0)]
		private sealed class WeldFinishedEvent : EntityEventArgs
		{
			// Token: 0x0600308F RID: 12431 RVA: 0x000FA8E2 File Offset: 0x000F8AE2
			public WeldFinishedEvent(EntityUid user, EntityUid tool)
			{
				this.User = user;
				this.Tool = tool;
			}

			// Token: 0x04001DD5 RID: 7637
			public readonly EntityUid User;

			// Token: 0x04001DD6 RID: 7638
			public readonly EntityUid Tool;
		}

		// Token: 0x020008D9 RID: 2265
		[NullableContext(0)]
		private sealed class WeldCancelledEvent : EntityEventArgs
		{
		}
	}
}
