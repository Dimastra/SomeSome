using System;
using System.Runtime.CompilerServices;
using Content.Server.Administration.Logs;
using Content.Server.Popups;
using Content.Server.UserInterface;
using Content.Shared.Administration.Logs;
using Content.Shared.Database;
using Content.Shared.Examine;
using Content.Shared.IdentityManagement;
using Content.Shared.Interaction;
using Content.Shared.Paper;
using Content.Shared.Popups;
using Content.Shared.Tag;
using Robust.Server.GameObjects;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Localization;
using Robust.Shared.Player;
using Robust.Shared.Utility;

namespace Content.Server.Paper
{
	// Token: 0x020002EF RID: 751
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class PaperSystem : EntitySystem
	{
		// Token: 0x06000F6D RID: 3949 RVA: 0x0004F374 File Offset: 0x0004D574
		public override void Initialize()
		{
			base.Initialize();
			base.SubscribeLocalEvent<PaperComponent, ComponentInit>(new ComponentEventHandler<PaperComponent, ComponentInit>(this.OnInit), null, null);
			base.SubscribeLocalEvent<PaperComponent, BeforeActivatableUIOpenEvent>(new ComponentEventHandler<PaperComponent, BeforeActivatableUIOpenEvent>(this.BeforeUIOpen), null, null);
			base.SubscribeLocalEvent<PaperComponent, ExaminedEvent>(new ComponentEventHandler<PaperComponent, ExaminedEvent>(this.OnExamined), null, null);
			base.SubscribeLocalEvent<PaperComponent, InteractUsingEvent>(new ComponentEventHandler<PaperComponent, InteractUsingEvent>(this.OnInteractUsing), null, null);
			base.SubscribeLocalEvent<PaperComponent, SharedPaperComponent.PaperInputTextMessage>(new ComponentEventHandler<PaperComponent, SharedPaperComponent.PaperInputTextMessage>(this.OnInputTextMessage), null, null);
		}

		// Token: 0x06000F6E RID: 3950 RVA: 0x0004F3EC File Offset: 0x0004D5EC
		private void OnInit(EntityUid uid, PaperComponent paperComp, ComponentInit args)
		{
			paperComp.Mode = SharedPaperComponent.PaperAction.Read;
			this.UpdateUserInterface(uid, paperComp);
			AppearanceComponent appearance;
			if (base.TryComp<AppearanceComponent>(uid, ref appearance))
			{
				if (paperComp.Content != "")
				{
					this._appearance.SetData(uid, SharedPaperComponent.PaperVisuals.Status, SharedPaperComponent.PaperStatus.Written, appearance);
				}
				if (paperComp.StampState != null)
				{
					this._appearance.SetData(uid, SharedPaperComponent.PaperVisuals.Stamp, paperComp.StampState, appearance);
				}
			}
		}

		// Token: 0x06000F6F RID: 3951 RVA: 0x0004F45F File Offset: 0x0004D65F
		private void BeforeUIOpen(EntityUid uid, PaperComponent paperComp, BeforeActivatableUIOpenEvent args)
		{
			paperComp.Mode = SharedPaperComponent.PaperAction.Read;
			this.UpdateUserInterface(uid, paperComp);
		}

		// Token: 0x06000F70 RID: 3952 RVA: 0x0004F470 File Offset: 0x0004D670
		private void OnExamined(EntityUid uid, PaperComponent paperComp, ExaminedEvent args)
		{
			if (!args.IsInDetailsRange)
			{
				return;
			}
			if (paperComp.Content != "")
			{
				args.PushMarkup(Loc.GetString("paper-component-examine-detail-has-words", new ValueTuple<string, object>[]
				{
					new ValueTuple<string, object>("paper", uid)
				}));
			}
			if (paperComp.StampedBy.Count > 0)
			{
				string commaSeparated = string.Join(", ", paperComp.StampedBy);
				args.PushMarkup(Loc.GetString("paper-component-examine-detail-stamped-by", new ValueTuple<string, object>[]
				{
					new ValueTuple<string, object>("paper", uid),
					new ValueTuple<string, object>("stamps", commaSeparated)
				}));
			}
		}

		// Token: 0x06000F71 RID: 3953 RVA: 0x0004F524 File Offset: 0x0004D724
		private void OnInteractUsing(EntityUid uid, PaperComponent paperComp, InteractUsingEvent args)
		{
			if (!this._tagSystem.HasTag(args.Used, "Write"))
			{
				StampComponent stampComp;
				if (base.TryComp<StampComponent>(args.Used, ref stampComp) && this.TryStamp(uid, stampComp.StampedName, stampComp.StampState, paperComp))
				{
					string stampPaperOtherMessage = Loc.GetString("paper-component-action-stamp-paper-other", new ValueTuple<string, object>[]
					{
						new ValueTuple<string, object>("user", Identity.Entity(args.User, this.EntityManager)),
						new ValueTuple<string, object>("target", Identity.Entity(args.Target, this.EntityManager)),
						new ValueTuple<string, object>("stamp", args.Used)
					});
					this._popupSystem.PopupEntity(stampPaperOtherMessage, args.User, Filter.PvsExcept(args.User, 2f, this.EntityManager), true, PopupType.Small);
					string stampPaperSelfMessage = Loc.GetString("paper-component-action-stamp-paper-self", new ValueTuple<string, object>[]
					{
						new ValueTuple<string, object>("target", Identity.Entity(args.Target, this.EntityManager)),
						new ValueTuple<string, object>("stamp", args.Used)
					});
					this._popupSystem.PopupEntity(stampPaperSelfMessage, args.User, args.User, PopupType.Small);
				}
				return;
			}
			ActorComponent actor;
			if (!base.TryComp<ActorComponent>(args.User, ref actor))
			{
				return;
			}
			paperComp.Mode = SharedPaperComponent.PaperAction.Write;
			this.UpdateUserInterface(uid, paperComp);
			BoundUserInterface uiOrNull = this._uiSystem.GetUiOrNull(uid, SharedPaperComponent.PaperUiKey.Key, null);
			if (uiOrNull == null)
			{
				return;
			}
			uiOrNull.Open(actor.PlayerSession);
		}

		// Token: 0x06000F72 RID: 3954 RVA: 0x0004F6CC File Offset: 0x0004D8CC
		private void OnInputTextMessage(EntityUid uid, PaperComponent paperComp, SharedPaperComponent.PaperInputTextMessage args)
		{
			if (string.IsNullOrEmpty(args.Text))
			{
				return;
			}
			string text = FormattedMessage.EscapeText(args.Text);
			if (text.Length + paperComp.Content.Length <= paperComp.ContentSize)
			{
				paperComp.Content = paperComp.Content + text + "\n";
			}
			AppearanceComponent appearance;
			if (base.TryComp<AppearanceComponent>(uid, ref appearance))
			{
				this._appearance.SetData(uid, SharedPaperComponent.PaperVisuals.Status, SharedPaperComponent.PaperStatus.Written, appearance);
			}
			MetaDataComponent meta;
			if (base.TryComp<MetaDataComponent>(uid, ref meta))
			{
				meta.EntityDescription = "";
			}
			if (args.Session.AttachedEntity != null)
			{
				ISharedAdminLogManager adminLogger = this._adminLogger;
				LogType type = LogType.Chat;
				LogImpact impact = LogImpact.Low;
				LogStringHandler logStringHandler = new LogStringHandler(37, 3);
				logStringHandler.AppendFormatted<EntityStringRepresentation>(base.ToPrettyString(args.Session.AttachedEntity.Value), "player", "ToPrettyString(args.Session.AttachedEntity.Value)");
				logStringHandler.AppendLiteral(" has written on ");
				logStringHandler.AppendFormatted<EntityStringRepresentation>(base.ToPrettyString(uid), "entity", "ToPrettyString(uid)");
				logStringHandler.AppendLiteral(" the following text: ");
				logStringHandler.AppendFormatted(args.Text);
				adminLogger.Add(type, impact, ref logStringHandler);
			}
			this.UpdateUserInterface(uid, paperComp);
		}

		// Token: 0x06000F73 RID: 3955 RVA: 0x0004F800 File Offset: 0x0004DA00
		public bool TryStamp(EntityUid uid, string stampName, string stampState, [Nullable(2)] PaperComponent paperComp = null)
		{
			if (!base.Resolve<PaperComponent>(uid, ref paperComp, true))
			{
				return false;
			}
			if (!paperComp.StampedBy.Contains(Loc.GetString(stampName)))
			{
				paperComp.StampedBy.Add(Loc.GetString(stampName));
				AppearanceComponent appearance;
				if (paperComp.StampState == null && base.TryComp<AppearanceComponent>(uid, ref appearance))
				{
					paperComp.StampState = stampState;
					this._appearance.SetData(uid, SharedPaperComponent.PaperVisuals.Stamp, paperComp.StampState, appearance);
				}
			}
			return true;
		}

		// Token: 0x06000F74 RID: 3956 RVA: 0x0004F878 File Offset: 0x0004DA78
		public void SetContent(EntityUid uid, string content, [Nullable(2)] PaperComponent paperComp = null)
		{
			if (!base.Resolve<PaperComponent>(uid, ref paperComp, true))
			{
				return;
			}
			paperComp.Content = content + "\n";
			this.UpdateUserInterface(uid, paperComp);
			AppearanceComponent appearance;
			if (!base.TryComp<AppearanceComponent>(uid, ref appearance))
			{
				return;
			}
			SharedPaperComponent.PaperStatus status = string.IsNullOrWhiteSpace(content) ? SharedPaperComponent.PaperStatus.Blank : SharedPaperComponent.PaperStatus.Written;
			this._appearance.SetData(uid, SharedPaperComponent.PaperVisuals.Status, status, appearance);
		}

		// Token: 0x06000F75 RID: 3957 RVA: 0x0004F8E0 File Offset: 0x0004DAE0
		[NullableContext(2)]
		public void UpdateUserInterface(EntityUid uid, PaperComponent paperComp = null)
		{
			if (!base.Resolve<PaperComponent>(uid, ref paperComp, true))
			{
				return;
			}
			BoundUserInterface uiOrNull = this._uiSystem.GetUiOrNull(uid, SharedPaperComponent.PaperUiKey.Key, null);
			if (uiOrNull == null)
			{
				return;
			}
			uiOrNull.SetState(new SharedPaperComponent.PaperBoundUserInterfaceState(paperComp.Content, paperComp.StampedBy, paperComp.Mode), null, true);
		}

		// Token: 0x0400090F RID: 2319
		[Dependency]
		private readonly TagSystem _tagSystem;

		// Token: 0x04000910 RID: 2320
		[Dependency]
		private readonly PopupSystem _popupSystem;

		// Token: 0x04000911 RID: 2321
		[Dependency]
		private readonly UserInterfaceSystem _uiSystem;

		// Token: 0x04000912 RID: 2322
		[Dependency]
		private readonly IAdminLogManager _adminLogger;

		// Token: 0x04000913 RID: 2323
		[Dependency]
		private readonly SharedAppearanceSystem _appearance;
	}
}
