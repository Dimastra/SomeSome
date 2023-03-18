using System;
using System.Runtime.CompilerServices;
using Content.Server.IdentityManagement;
using Content.Shared.Clothing.Components;
using Content.Shared.Clothing.EntitySystems;
using Content.Shared.IdentityManagement.Components;
using Content.Shared.Prototypes;
using Content.Shared.Verbs;
using Robust.Server.GameObjects;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.IoC;
using Robust.Shared.Localization;
using Robust.Shared.Prototypes;
using Robust.Shared.Utility;

namespace Content.Server.Clothing.Systems
{
	// Token: 0x02000639 RID: 1593
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class ChameleonClothingSystem : SharedChameleonClothingSystem
	{
		// Token: 0x060021E2 RID: 8674 RVA: 0x000B0B78 File Offset: 0x000AED78
		public override void Initialize()
		{
			base.Initialize();
			base.SubscribeLocalEvent<ChameleonClothingComponent, ComponentInit>(new ComponentEventHandler<ChameleonClothingComponent, ComponentInit>(this.OnInit), null, null);
			base.SubscribeLocalEvent<ChameleonClothingComponent, ComponentGetState>(new ComponentEventRefHandler<ChameleonClothingComponent, ComponentGetState>(this.GetState), null, null);
			base.SubscribeLocalEvent<ChameleonClothingComponent, GetVerbsEvent<InteractionVerb>>(new ComponentEventHandler<ChameleonClothingComponent, GetVerbsEvent<InteractionVerb>>(this.OnVerb), null, null);
			base.SubscribeLocalEvent<ChameleonClothingComponent, ChameleonPrototypeSelectedMessage>(new ComponentEventHandler<ChameleonClothingComponent, ChameleonPrototypeSelectedMessage>(this.OnSelected), null, null);
		}

		// Token: 0x060021E3 RID: 8675 RVA: 0x000B0BDB File Offset: 0x000AEDDB
		private void OnInit(EntityUid uid, ChameleonClothingComponent component, ComponentInit args)
		{
			this.SetSelectedPrototype(uid, component.SelectedId, true, component);
		}

		// Token: 0x060021E4 RID: 8676 RVA: 0x000B0BEC File Offset: 0x000AEDEC
		private void GetState(EntityUid uid, ChameleonClothingComponent component, ref ComponentGetState args)
		{
			args.State = new ChameleonClothingComponentState
			{
				SelectedId = component.SelectedId
			};
		}

		// Token: 0x060021E5 RID: 8677 RVA: 0x000B0C08 File Offset: 0x000AEE08
		private void OnVerb(EntityUid uid, ChameleonClothingComponent component, GetVerbsEvent<InteractionVerb> args)
		{
			if (!args.CanAccess || !args.CanInteract)
			{
				return;
			}
			args.Verbs.Add(new InteractionVerb
			{
				Text = Loc.GetString("chameleon-component-verb-text"),
				Icon = new SpriteSpecifier.Texture(new ResourcePath("/Textures/Interface/VerbIcons/settings.svg.192dpi.png", "/")),
				Act = delegate()
				{
					this.TryOpenUi(uid, args.User, component);
				}
			});
		}

		// Token: 0x060021E6 RID: 8678 RVA: 0x000B0CA4 File Offset: 0x000AEEA4
		private void OnSelected(EntityUid uid, ChameleonClothingComponent component, ChameleonPrototypeSelectedMessage args)
		{
			this.SetSelectedPrototype(uid, args.SelectedId, false, component);
		}

		// Token: 0x060021E7 RID: 8679 RVA: 0x000B0CB8 File Offset: 0x000AEEB8
		[NullableContext(2)]
		private void TryOpenUi(EntityUid uid, EntityUid user, ChameleonClothingComponent component = null)
		{
			if (!base.Resolve<ChameleonClothingComponent>(uid, ref component, true))
			{
				return;
			}
			ActorComponent actor;
			if (!base.TryComp<ActorComponent>(user, ref actor))
			{
				return;
			}
			this._uiSystem.TryToggleUi(uid, ChameleonUiKey.Key, actor.PlayerSession, null);
		}

		// Token: 0x060021E8 RID: 8680 RVA: 0x000B0CF8 File Offset: 0x000AEEF8
		[NullableContext(2)]
		private void UpdateUi(EntityUid uid, ChameleonClothingComponent component = null)
		{
			if (!base.Resolve<ChameleonClothingComponent>(uid, ref component, true))
			{
				return;
			}
			ChameleonBoundUserInterfaceState state = new ChameleonBoundUserInterfaceState(component.Slot, component.SelectedId);
			this._uiSystem.TrySetUiState(uid, ChameleonUiKey.Key, state, null, null, true);
		}

		// Token: 0x060021E9 RID: 8681 RVA: 0x000B0D3C File Offset: 0x000AEF3C
		[NullableContext(2)]
		public void SetSelectedPrototype(EntityUid uid, string protoId, bool forceUpdate = false, ChameleonClothingComponent component = null)
		{
			if (!base.Resolve<ChameleonClothingComponent>(uid, ref component, false))
			{
				return;
			}
			if (component.SelectedId == protoId && !forceUpdate)
			{
				return;
			}
			EntityPrototype proto;
			if (string.IsNullOrEmpty(protoId) || !this._proto.TryIndex<EntityPrototype>(protoId, ref proto))
			{
				return;
			}
			if (!base.IsValidTarget(proto, component.Slot))
			{
				return;
			}
			component.SelectedId = protoId;
			this.UpdateIdentityBlocker(uid, component, proto);
			base.UpdateVisuals(uid, component);
			this.UpdateUi(uid, component);
			base.Dirty(component, null);
		}

		// Token: 0x060021EA RID: 8682 RVA: 0x000B0DC0 File Offset: 0x000AEFC0
		private void UpdateIdentityBlocker(EntityUid uid, ChameleonClothingComponent component, EntityPrototype proto)
		{
			if (proto.HasComponent(this._factory))
			{
				base.EnsureComp<IdentityBlockerComponent>(uid);
			}
			else
			{
				base.RemComp<IdentityBlockerComponent>(uid);
			}
			if (component.User != null)
			{
				this._identity.QueueIdentityUpdate(component.User.Value);
			}
		}

		// Token: 0x040014C6 RID: 5318
		[Dependency]
		private readonly IPrototypeManager _proto;

		// Token: 0x040014C7 RID: 5319
		[Dependency]
		private readonly UserInterfaceSystem _uiSystem;

		// Token: 0x040014C8 RID: 5320
		[Dependency]
		private readonly IComponentFactory _factory;

		// Token: 0x040014C9 RID: 5321
		[Dependency]
		private readonly IdentitySystem _identity;
	}
}
