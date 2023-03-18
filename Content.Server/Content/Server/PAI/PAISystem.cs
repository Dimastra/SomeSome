using System;
using System.Runtime.CompilerServices;
using Content.Server.Ghost.Roles.Components;
using Content.Server.Instruments;
using Content.Server.Mind.Components;
using Content.Server.Popups;
using Content.Shared.Examine;
using Content.Shared.Interaction.Events;
using Content.Shared.PAI;
using Content.Shared.Popups;
using Content.Shared.Verbs;
using Robust.Server.GameObjects;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Localization;
using Robust.Shared.Prototypes;

namespace Content.Server.PAI
{
	// Token: 0x020002F0 RID: 752
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class PAISystem : SharedPAISystem
	{
		// Token: 0x06000F77 RID: 3959 RVA: 0x0004F938 File Offset: 0x0004DB38
		public override void Initialize()
		{
			base.Initialize();
			base.SubscribeLocalEvent<PAIComponent, ExaminedEvent>(new ComponentEventHandler<PAIComponent, ExaminedEvent>(this.OnExamined), null, null);
			base.SubscribeLocalEvent<PAIComponent, UseInHandEvent>(new ComponentEventHandler<PAIComponent, UseInHandEvent>(this.OnUseInHand), null, null);
			base.SubscribeLocalEvent<PAIComponent, MindAddedMessage>(new ComponentEventHandler<PAIComponent, MindAddedMessage>(this.OnMindAdded), null, null);
			base.SubscribeLocalEvent<PAIComponent, MindRemovedMessage>(new ComponentEventHandler<PAIComponent, MindRemovedMessage>(this.OnMindRemoved), null, null);
			base.SubscribeLocalEvent<PAIComponent, GetVerbsEvent<ActivationVerb>>(new ComponentEventHandler<PAIComponent, GetVerbsEvent<ActivationVerb>>(this.AddWipeVerb), null, null);
		}

		// Token: 0x06000F78 RID: 3960 RVA: 0x0004F9B0 File Offset: 0x0004DBB0
		private void OnExamined(EntityUid uid, PAIComponent component, ExaminedEvent args)
		{
			if (args.IsInDetailsRange)
			{
				MindComponent mind;
				if (this.EntityManager.TryGetComponent<MindComponent>(uid, ref mind) && mind.HasMind)
				{
					args.PushMarkup(Loc.GetString("pai-system-pai-installed"));
					return;
				}
				if (this.EntityManager.HasComponent<GhostTakeoverAvailableComponent>(uid))
				{
					args.PushMarkup(Loc.GetString("pai-system-still-searching"));
					return;
				}
				args.PushMarkup(Loc.GetString("pai-system-off"));
			}
		}

		// Token: 0x06000F79 RID: 3961 RVA: 0x0004FA20 File Offset: 0x0004DC20
		private void OnUseInHand(EntityUid uid, PAIComponent component, UseInHandEvent args)
		{
			if (args.Handled)
			{
				return;
			}
			args.Handled = true;
			MindComponent mind;
			if (this.EntityManager.TryGetComponent<MindComponent>(uid, ref mind) && mind.HasMind)
			{
				this._popupSystem.PopupEntity(Loc.GetString("pai-system-pai-installed"), uid, args.User, PopupType.Large);
				return;
			}
			if (this.EntityManager.HasComponent<GhostTakeoverAvailableComponent>(uid))
			{
				this._popupSystem.PopupEntity(Loc.GetString("pai-system-still-searching"), uid, args.User, PopupType.Small);
				return;
			}
			string val = Loc.GetString("pai-system-pai-name", new ValueTuple<string, object>[]
			{
				new ValueTuple<string, object>("owner", args.User)
			});
			this.EntityManager.GetComponent<MetaDataComponent>(component.Owner).EntityName = val;
			GhostTakeoverAvailableComponent ghostTakeoverAvailableComponent = this.EntityManager.EnsureComponent<GhostTakeoverAvailableComponent>(uid);
			ghostTakeoverAvailableComponent.RoleName = Loc.GetString("pai-system-role-name");
			ghostTakeoverAvailableComponent.RoleDescription = Loc.GetString("pai-system-role-description");
			this._popupSystem.PopupEntity(Loc.GetString("pai-system-searching"), uid, args.User, PopupType.Small);
			this.UpdatePAIAppearance(uid, PAIStatus.Searching);
		}

		// Token: 0x06000F7A RID: 3962 RVA: 0x0004FB33 File Offset: 0x0004DD33
		private void OnMindRemoved(EntityUid uid, PAIComponent component, MindRemovedMessage args)
		{
			this.PAITurningOff(uid);
		}

		// Token: 0x06000F7B RID: 3963 RVA: 0x0004FB3C File Offset: 0x0004DD3C
		private void OnMindAdded(EntityUid uid, PAIComponent pai, MindAddedMessage args)
		{
			if (this.EntityManager.HasComponent<GhostTakeoverAvailableComponent>(uid))
			{
				this.EntityManager.RemoveComponent<GhostTakeoverAvailableComponent>(uid);
			}
			this.UpdatePAIAppearance(uid, PAIStatus.On);
		}

		// Token: 0x06000F7C RID: 3964 RVA: 0x0004FB64 File Offset: 0x0004DD64
		private void PAITurningOff(EntityUid uid)
		{
			this.UpdatePAIAppearance(uid, PAIStatus.Off);
			ActorComponent actor;
			if (base.HasComp<ActiveInstrumentComponent>(uid) && base.TryComp<ActorComponent>(uid, ref actor))
			{
				this._instrumentSystem.ToggleInstrumentUi(uid, actor.PlayerSession, null);
			}
			InstrumentComponent instrument;
			if (this.EntityManager.TryGetComponent<InstrumentComponent>(uid, ref instrument))
			{
				this._instrumentSystem.Clean(uid, instrument);
			}
			MetaDataComponent metadata;
			if (this.EntityManager.TryGetComponent<MetaDataComponent>(uid, ref metadata))
			{
				EntityPrototype proto = metadata.EntityPrototype;
				if (proto != null)
				{
					metadata.EntityName = proto.Name;
				}
			}
		}

		// Token: 0x06000F7D RID: 3965 RVA: 0x0004FBE4 File Offset: 0x0004DDE4
		private void UpdatePAIAppearance(EntityUid uid, PAIStatus status)
		{
			AppearanceComponent appearance;
			if (this.EntityManager.TryGetComponent<AppearanceComponent>(uid, ref appearance))
			{
				this._appearance.SetData(uid, PAIVisuals.Status, status, appearance);
			}
		}

		// Token: 0x06000F7E RID: 3966 RVA: 0x0004FC1C File Offset: 0x0004DE1C
		private void AddWipeVerb(EntityUid uid, PAIComponent pai, GetVerbsEvent<ActivationVerb> args)
		{
			if (!args.CanAccess || !args.CanInteract)
			{
				return;
			}
			MindComponent mind;
			if (this.EntityManager.TryGetComponent<MindComponent>(uid, ref mind) && mind.HasMind)
			{
				ActivationVerb verb = new ActivationVerb();
				verb.Text = Loc.GetString("pai-system-wipe-device-verb-text");
				verb.Act = delegate()
				{
					if (pai.Deleted)
					{
						return;
					}
					if (this.EntityManager.HasComponent<MindComponent>(uid))
					{
						this.EntityManager.RemoveComponent<MindComponent>(uid);
						this._popupSystem.PopupEntity(Loc.GetString("pai-system-wiped-device"), uid, args.User, PopupType.Large);
						this.PAITurningOff(uid);
					}
				};
				args.Verbs.Add(verb);
				return;
			}
			if (this.EntityManager.HasComponent<GhostTakeoverAvailableComponent>(uid))
			{
				ActivationVerb verb2 = new ActivationVerb();
				verb2.Text = Loc.GetString("pai-system-stop-searching-verb-text");
				verb2.Act = delegate()
				{
					if (pai.Deleted)
					{
						return;
					}
					if (this.EntityManager.HasComponent<GhostTakeoverAvailableComponent>(uid))
					{
						this.EntityManager.RemoveComponent<GhostTakeoverAvailableComponent>(uid);
						this._popupSystem.PopupEntity(Loc.GetString("pai-system-stopped-searching"), uid, args.User, PopupType.Small);
						this.PAITurningOff(uid);
					}
				};
				args.Verbs.Add(verb2);
			}
		}

		// Token: 0x04000914 RID: 2324
		[Dependency]
		private readonly PopupSystem _popupSystem;

		// Token: 0x04000915 RID: 2325
		[Dependency]
		private readonly InstrumentSystem _instrumentSystem;

		// Token: 0x04000916 RID: 2326
		[Dependency]
		private readonly SharedAppearanceSystem _appearance;
	}
}
