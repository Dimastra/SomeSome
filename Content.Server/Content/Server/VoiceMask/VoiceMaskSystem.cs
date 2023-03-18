using System;
using System.Runtime.CompilerServices;
using Content.Server.Actions;
using Content.Server.Administration.Logs;
using Content.Server.Chat.Systems;
using Content.Server.Popups;
using Content.Server.White.TTS;
using Content.Shared.Actions.ActionTypes;
using Content.Shared.Administration.Logs;
using Content.Shared.Database;
using Content.Shared.Inventory;
using Content.Shared.Inventory.Events;
using Content.Shared.Popups;
using Content.Shared.VoiceMask;
using Robust.Server.GameObjects;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Localization;
using Robust.Shared.Prototypes;

namespace Content.Server.VoiceMask
{
	// Token: 0x020000CC RID: 204
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class VoiceMaskSystem : EntitySystem
	{
		// Token: 0x0600038A RID: 906 RVA: 0x00012A40 File Offset: 0x00010C40
		public override void Initialize()
		{
			base.SubscribeLocalEvent<VoiceMaskComponent, TransformSpeakerNameEvent>(new ComponentEventHandler<VoiceMaskComponent, TransformSpeakerNameEvent>(this.OnSpeakerNameTransform), null, null);
			base.SubscribeLocalEvent<VoiceMaskComponent, VoiceMaskChangeNameMessage>(new ComponentEventHandler<VoiceMaskComponent, VoiceMaskChangeNameMessage>(this.OnChangeName), null, null);
			base.SubscribeLocalEvent<VoiceMaskerComponent, GotEquippedEvent>(new ComponentEventHandler<VoiceMaskerComponent, GotEquippedEvent>(this.OnEquip), null, null);
			base.SubscribeLocalEvent<VoiceMaskerComponent, GotUnequippedEvent>(new ComponentEventHandler<VoiceMaskerComponent, GotUnequippedEvent>(this.OnUnequip), null, null);
			base.SubscribeLocalEvent<VoiceMaskSetNameEvent>(new EntityEventHandler<VoiceMaskSetNameEvent>(this.OnSetName), null, null);
			this.InitializeTTS();
		}

		// Token: 0x0600038B RID: 907 RVA: 0x00012AB7 File Offset: 0x00010CB7
		private void OnSetName(VoiceMaskSetNameEvent ev)
		{
			this.OpenUI(ev.Performer, null);
		}

		// Token: 0x0600038C RID: 908 RVA: 0x00012AC8 File Offset: 0x00010CC8
		private void OnChangeName(EntityUid uid, VoiceMaskComponent component, VoiceMaskChangeNameMessage message)
		{
			if (message.Name.Length > 32 || message.Name.Length <= 0)
			{
				this._popupSystem.PopupCursor(Loc.GetString("voice-mask-popup-failure"), message.Session, PopupType.Small);
				return;
			}
			component.VoiceName = message.Name;
			if (message.Session.AttachedEntity != null)
			{
				ISharedAdminLogManager adminLogger = this._adminLogger;
				LogType type = LogType.Action;
				LogImpact impact = LogImpact.Medium;
				LogStringHandler logStringHandler = new LogStringHandler(16, 3);
				logStringHandler.AppendFormatted<EntityStringRepresentation>(base.ToPrettyString(message.Session.AttachedEntity.Value), "player", "ToPrettyString(message.Session.AttachedEntity.Value)");
				logStringHandler.AppendLiteral(" set voice of ");
				logStringHandler.AppendFormatted<EntityStringRepresentation>(base.ToPrettyString(uid), "mask", "ToPrettyString(uid)");
				logStringHandler.AppendLiteral(": ");
				logStringHandler.AppendFormatted(component.VoiceName);
				adminLogger.Add(type, impact, ref logStringHandler);
			}
			else
			{
				ISharedAdminLogManager adminLogger2 = this._adminLogger;
				LogType type2 = LogType.Action;
				LogImpact impact2 = LogImpact.Medium;
				LogStringHandler logStringHandler = new LogStringHandler(15, 2);
				logStringHandler.AppendLiteral("Voice of ");
				logStringHandler.AppendFormatted<EntityStringRepresentation>(base.ToPrettyString(uid), "mask", "ToPrettyString(uid)");
				logStringHandler.AppendLiteral(" set: ");
				logStringHandler.AppendFormatted(component.VoiceName);
				adminLogger2.Add(type2, impact2, ref logStringHandler);
			}
			this._popupSystem.PopupCursor(Loc.GetString("voice-mask-popup-success"), message.Session, PopupType.Small);
			this.TrySetLastKnownName(uid, message.Name);
			this.UpdateUI(uid, component);
		}

		// Token: 0x0600038D RID: 909 RVA: 0x00012C3E File Offset: 0x00010E3E
		private void OnSpeakerNameTransform(EntityUid uid, VoiceMaskComponent component, TransformSpeakerNameEvent args)
		{
			if (component.Enabled)
			{
				args.Name = component.VoiceName;
			}
		}

		// Token: 0x0600038E RID: 910 RVA: 0x00012C54 File Offset: 0x00010E54
		[NullableContext(2)]
		private void OpenUI(EntityUid player, ActorComponent actor = null)
		{
			if (!base.Resolve<ActorComponent>(player, ref actor, true))
			{
				return;
			}
			BoundUserInterface uiOrNull = this._uiSystem.GetUiOrNull(player, VoiceMaskUIKey.Key, null);
			if (uiOrNull != null)
			{
				uiOrNull.Open(actor.PlayerSession);
			}
			this.UpdateUI(player, null);
		}

		// Token: 0x0600038F RID: 911 RVA: 0x00012C90 File Offset: 0x00010E90
		[NullableContext(2)]
		private void UpdateUI(EntityUid owner, VoiceMaskComponent component = null)
		{
			if (!base.Resolve<VoiceMaskComponent>(owner, ref component, true))
			{
				return;
			}
			BoundUserInterface uiOrNull = this._uiSystem.GetUiOrNull(owner, VoiceMaskUIKey.Key, null);
			if (uiOrNull == null)
			{
				return;
			}
			uiOrNull.SetState(new VoiceMaskBuiState(component.VoiceName, component.VoiceId), null, true);
		}

		// Token: 0x06000390 RID: 912 RVA: 0x00012CD0 File Offset: 0x00010ED0
		private void OnEquip(EntityUid uid, VoiceMaskerComponent component, GotEquippedEvent args)
		{
			VoiceMaskComponent comp = base.EnsureComp<VoiceMaskComponent>(args.Equipee);
			comp.VoiceName = component.LastSetName;
			if (component.LastSetVoice != null)
			{
				comp.VoiceId = component.LastSetVoice;
			}
			InstantActionPrototype action;
			if (!this._prototypeManager.TryIndex<InstantActionPrototype>(component.Action, ref action))
			{
				throw new ArgumentException("Could not get voice masking prototype.");
			}
			this._actions.AddAction(args.Equipee, (InstantAction)action.Clone(), new EntityUid?(uid), null, true);
		}

		// Token: 0x06000391 RID: 913 RVA: 0x00012D4E File Offset: 0x00010F4E
		private void OnUnequip(EntityUid uid, VoiceMaskerComponent compnent, GotUnequippedEvent args)
		{
			base.RemComp<VoiceMaskComponent>(args.Equipee);
		}

		// Token: 0x06000392 RID: 914 RVA: 0x00012D60 File Offset: 0x00010F60
		private void TrySetLastKnownName(EntityUid maskWearer, string lastName)
		{
			EntityUid? maskEntity;
			VoiceMaskerComponent maskComp;
			if (!base.HasComp<VoiceMaskComponent>(maskWearer) || !this._inventory.TryGetSlotEntity(maskWearer, "mask", out maskEntity, null, null) || !base.TryComp<VoiceMaskerComponent>(maskEntity, ref maskComp))
			{
				return;
			}
			maskComp.LastSetName = lastName;
		}

		// Token: 0x06000393 RID: 915 RVA: 0x00012DA0 File Offset: 0x00010FA0
		private void InitializeTTS()
		{
			base.SubscribeLocalEvent<VoiceMaskComponent, TransformSpeakerVoiceEvent>(new ComponentEventHandler<VoiceMaskComponent, TransformSpeakerVoiceEvent>(this.OnSpeakerVoiceTransform), null, null);
			base.SubscribeLocalEvent<VoiceMaskComponent, VoiceMaskChangeVoiceMessage>(new ComponentEventHandler<VoiceMaskComponent, VoiceMaskChangeVoiceMessage>(this.OnChangeVoice), null, null);
		}

		// Token: 0x06000394 RID: 916 RVA: 0x00012DCA File Offset: 0x00010FCA
		private void OnSpeakerVoiceTransform(EntityUid uid, VoiceMaskComponent component, TransformSpeakerVoiceEvent args)
		{
			if (component.Enabled)
			{
				args.VoiceId = component.VoiceId;
			}
		}

		// Token: 0x06000395 RID: 917 RVA: 0x00012DE0 File Offset: 0x00010FE0
		private void OnChangeVoice(EntityUid uid, VoiceMaskComponent component, VoiceMaskChangeVoiceMessage message)
		{
			component.VoiceId = message.Voice;
			this._popupSystem.PopupCursor(Loc.GetString("voice-mask-voice-popup-success"), message.Session, PopupType.Small);
			this.TrySetLastKnownVoice(uid, message.Voice);
			this.UpdateUI(uid, component);
		}

		// Token: 0x06000396 RID: 918 RVA: 0x00012E20 File Offset: 0x00011020
		[NullableContext(2)]
		private void TrySetLastKnownVoice(EntityUid maskWearer, string voiceId)
		{
			EntityUid? maskEntity;
			VoiceMaskerComponent maskComp;
			if (!base.HasComp<VoiceMaskComponent>(maskWearer) || !this._inventory.TryGetSlotEntity(maskWearer, "mask", out maskEntity, null, null) || !base.TryComp<VoiceMaskerComponent>(maskEntity, ref maskComp))
			{
				return;
			}
			maskComp.LastSetVoice = voiceId;
		}

		// Token: 0x0400023D RID: 573
		[Dependency]
		private readonly UserInterfaceSystem _uiSystem;

		// Token: 0x0400023E RID: 574
		[Dependency]
		private readonly PopupSystem _popupSystem;

		// Token: 0x0400023F RID: 575
		[Dependency]
		private readonly IAdminLogManager _adminLogger;

		// Token: 0x04000240 RID: 576
		[Dependency]
		private readonly InventorySystem _inventory;

		// Token: 0x04000241 RID: 577
		[Dependency]
		private readonly ActionsSystem _actions;

		// Token: 0x04000242 RID: 578
		[Dependency]
		private readonly IPrototypeManager _prototypeManager;

		// Token: 0x04000243 RID: 579
		private const string MaskSlot = "mask";
	}
}
