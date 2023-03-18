using System;
using System.Runtime.CompilerServices;
using Content.Shared.Access.Components;
using Content.Shared.Access.Systems;
using Content.Shared.Emag.Systems;
using Content.Shared.Examine;
using Content.Shared.Hands.Components;
using Content.Shared.IdentityManagement;
using Content.Shared.Interaction;
using Content.Shared.Popups;
using Content.Shared.Storage;
using Content.Shared.Storage.Components;
using Content.Shared.Verbs;
using Robust.Shared.Audio;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.IoC;
using Robust.Shared.Localization;
using Robust.Shared.Network;
using Robust.Shared.Timing;
using Robust.Shared.Utility;

namespace Content.Shared.Lock
{
	// Token: 0x0200035C RID: 860
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class LockSystem : EntitySystem
	{
		// Token: 0x06000A13 RID: 2579 RVA: 0x00020B54 File Offset: 0x0001ED54
		public override void Initialize()
		{
			base.Initialize();
			base.SubscribeLocalEvent<LockComponent, ComponentGetState>(new ComponentEventRefHandler<LockComponent, ComponentGetState>(this.OnGetState), null, null);
			base.SubscribeLocalEvent<LockComponent, ComponentHandleState>(new ComponentEventRefHandler<LockComponent, ComponentHandleState>(this.OnHandleState), null, null);
			base.SubscribeLocalEvent<LockComponent, ComponentStartup>(new ComponentEventHandler<LockComponent, ComponentStartup>(this.OnStartup), null, null);
			base.SubscribeLocalEvent<LockComponent, ActivateInWorldEvent>(new ComponentEventHandler<LockComponent, ActivateInWorldEvent>(this.OnActivated), null, null);
			base.SubscribeLocalEvent<LockComponent, StorageOpenAttemptEvent>(new ComponentEventRefHandler<LockComponent, StorageOpenAttemptEvent>(this.OnStorageOpenAttempt), null, null);
			base.SubscribeLocalEvent<LockComponent, ExaminedEvent>(new ComponentEventHandler<LockComponent, ExaminedEvent>(this.OnExamined), null, null);
			base.SubscribeLocalEvent<LockComponent, GetVerbsEvent<AlternativeVerb>>(new ComponentEventHandler<LockComponent, GetVerbsEvent<AlternativeVerb>>(this.AddToggleLockVerb), null, null);
			base.SubscribeLocalEvent<LockComponent, GotEmaggedEvent>(new ComponentEventRefHandler<LockComponent, GotEmaggedEvent>(this.OnEmagged), null, null);
		}

		// Token: 0x06000A14 RID: 2580 RVA: 0x00020C07 File Offset: 0x0001EE07
		private void OnGetState(EntityUid uid, LockComponent component, ref ComponentGetState args)
		{
			args.State = new LockComponentState(component.Locked, component.LockOnClick);
		}

		// Token: 0x06000A15 RID: 2581 RVA: 0x00020C20 File Offset: 0x0001EE20
		private void OnHandleState(EntityUid uid, LockComponent component, ref ComponentHandleState args)
		{
			LockComponentState state = args.Current as LockComponentState;
			if (state == null)
			{
				return;
			}
			component.Locked = state.Locked;
			component.LockOnClick = state.LockOnClick;
		}

		// Token: 0x06000A16 RID: 2582 RVA: 0x00020C55 File Offset: 0x0001EE55
		private void OnStartup(EntityUid uid, LockComponent lockComp, ComponentStartup args)
		{
			this._appearanceSystem.SetData(uid, StorageVisuals.CanLock, true, null);
		}

		// Token: 0x06000A17 RID: 2583 RVA: 0x00020C70 File Offset: 0x0001EE70
		private void OnActivated(EntityUid uid, LockComponent lockComp, ActivateInWorldEvent args)
		{
			if (args.Handled)
			{
				return;
			}
			if (lockComp.Locked)
			{
				this.TryUnlock(uid, args.User, lockComp);
				args.Handled = true;
				return;
			}
			if (lockComp.LockOnClick)
			{
				this.TryLock(uid, args.User, lockComp);
				args.Handled = true;
			}
		}

		// Token: 0x06000A18 RID: 2584 RVA: 0x00020CC3 File Offset: 0x0001EEC3
		private void OnStorageOpenAttempt(EntityUid uid, LockComponent component, ref StorageOpenAttemptEvent args)
		{
			if (!component.Locked)
			{
				return;
			}
			if (!args.Silent)
			{
				this._sharedPopupSystem.PopupEntity(Loc.GetString("entity-storage-component-locked-message"), uid, PopupType.Small);
			}
			args.Cancelled = true;
		}

		// Token: 0x06000A19 RID: 2585 RVA: 0x00020CF4 File Offset: 0x0001EEF4
		private void OnExamined(EntityUid uid, LockComponent lockComp, ExaminedEvent args)
		{
			args.PushText(Loc.GetString(lockComp.Locked ? "lock-comp-on-examined-is-locked" : "lock-comp-on-examined-is-unlocked", new ValueTuple<string, object>[]
			{
				new ValueTuple<string, object>("entityName", Identity.Name(uid, this.EntityManager, null))
			}));
		}

		// Token: 0x06000A1A RID: 2586 RVA: 0x00020D4C File Offset: 0x0001EF4C
		[NullableContext(2)]
		public bool TryLock(EntityUid uid, EntityUid user, LockComponent lockComp = null)
		{
			if (!base.Resolve<LockComponent>(uid, ref lockComp, true))
			{
				return false;
			}
			if (!this.CanToggleLock(uid, user, false))
			{
				return false;
			}
			if (!this.HasUserAccess(uid, user, null, false))
			{
				return false;
			}
			if (this._net.IsClient && this._timing.IsFirstTimePredicted)
			{
				this._sharedPopupSystem.PopupEntity(Loc.GetString("lock-comp-do-lock-success", new ValueTuple<string, object>[]
				{
					new ValueTuple<string, object>("entityName", Identity.Name(uid, this.EntityManager, null))
				}), uid, user, PopupType.Small);
				this._audio.PlayPvs(this._audio.GetSound(lockComp.LockSound), uid, new AudioParams?(AudioParams.Default.WithVolume(-5f)));
			}
			lockComp.Locked = true;
			this._appearanceSystem.SetData(uid, StorageVisuals.Locked, true, null);
			base.Dirty(lockComp, null);
			LockToggledEvent ev = new LockToggledEvent(true);
			base.RaiseLocalEvent<LockToggledEvent>(uid, ref ev, true);
			return true;
		}

		// Token: 0x06000A1B RID: 2587 RVA: 0x00020E50 File Offset: 0x0001F050
		[NullableContext(2)]
		public void Unlock(EntityUid uid, EntityUid? user, LockComponent lockComp = null)
		{
			if (!base.Resolve<LockComponent>(uid, ref lockComp, true))
			{
				return;
			}
			if (this._net.IsClient && this._timing.IsFirstTimePredicted)
			{
				if (user != null && user.GetValueOrDefault().Valid)
				{
					this._sharedPopupSystem.PopupEntity(Loc.GetString("lock-comp-do-unlock-success", new ValueTuple<string, object>[]
					{
						new ValueTuple<string, object>("entityName", Identity.Name(uid, this.EntityManager, null))
					}), uid, user.Value, PopupType.Small);
				}
				this._audio.PlayPvs(this._audio.GetSound(lockComp.UnlockSound), uid, new AudioParams?(AudioParams.Default.WithVolume(-5f)));
			}
			lockComp.Locked = false;
			this._appearanceSystem.SetData(uid, StorageVisuals.Locked, false, null);
			base.Dirty(lockComp, null);
			LockToggledEvent ev = new LockToggledEvent(false);
			base.RaiseLocalEvent<LockToggledEvent>(uid, ref ev, true);
		}

		// Token: 0x06000A1C RID: 2588 RVA: 0x00020F5A File Offset: 0x0001F15A
		[NullableContext(2)]
		public bool TryUnlock(EntityUid uid, EntityUid user, LockComponent lockComp = null)
		{
			if (!base.Resolve<LockComponent>(uid, ref lockComp, true))
			{
				return false;
			}
			if (!this.CanToggleLock(uid, user, false))
			{
				return false;
			}
			if (!this.HasUserAccess(uid, user, null, false))
			{
				return false;
			}
			this.Unlock(uid, new EntityUid?(user), lockComp);
			return true;
		}

		// Token: 0x06000A1D RID: 2589 RVA: 0x00020F94 File Offset: 0x0001F194
		public bool CanToggleLock(EntityUid uid, EntityUid user, bool quiet = true)
		{
			if (!base.HasComp<SharedHandsComponent>(user))
			{
				return false;
			}
			LockToggleAttemptEvent ev = new LockToggleAttemptEvent(user, quiet, false);
			base.RaiseLocalEvent<LockToggleAttemptEvent>(uid, ref ev, true);
			return !ev.Cancelled;
		}

		// Token: 0x06000A1E RID: 2590 RVA: 0x00020FCC File Offset: 0x0001F1CC
		[NullableContext(2)]
		private bool HasUserAccess(EntityUid uid, EntityUid user, AccessReaderComponent reader = null, bool quiet = true)
		{
			if (!base.Resolve<AccessReaderComponent>(uid, ref reader, true))
			{
				return true;
			}
			if (this._accessReader.IsAllowed(user, reader))
			{
				return true;
			}
			if (!quiet && this._net.IsClient && this._timing.IsFirstTimePredicted)
			{
				this._sharedPopupSystem.PopupEntity(Loc.GetString("lock-comp-has-user-access-fail"), uid, user, PopupType.Small);
			}
			return false;
		}

		// Token: 0x06000A1F RID: 2591 RVA: 0x00021030 File Offset: 0x0001F230
		private void AddToggleLockVerb(EntityUid uid, LockComponent component, GetVerbsEvent<AlternativeVerb> args)
		{
			if (!args.CanAccess || !args.CanInteract || !this.CanToggleLock(uid, args.User, true))
			{
				return;
			}
			AlternativeVerb verb = new AlternativeVerb
			{
				Act = (component.Locked ? delegate()
				{
					this.TryUnlock(uid, args.User, component);
				} : delegate()
				{
					this.TryLock(uid, args.User, component);
				}),
				Text = Loc.GetString(component.Locked ? "toggle-lock-verb-unlock" : "toggle-lock-verb-lock"),
				Icon = (component.Locked ? new SpriteSpecifier.Texture(new ResourcePath("/Textures/Interface/VerbIcons/unlock.svg.192dpi.png", "/")) : new SpriteSpecifier.Texture(new ResourcePath("/Textures/Interface/VerbIcons/lock.svg.192dpi.png", "/")))
			};
			args.Verbs.Add(verb);
		}

		// Token: 0x06000A20 RID: 2592 RVA: 0x0002113C File Offset: 0x0001F33C
		private void OnEmagged(EntityUid uid, LockComponent component, ref GotEmaggedEvent args)
		{
			if (!component.Locked)
			{
				return;
			}
			if (this._net.IsClient && this._timing.IsFirstTimePredicted)
			{
				this._audio.PlayPvs(this._audio.GetSound(component.UnlockSound), uid, new AudioParams?(AudioParams.Default.WithVolume(-5f)));
			}
			this._appearanceSystem.SetData(uid, StorageVisuals.Locked, false, null);
			base.RemComp<LockComponent>(uid);
			args.Handled = true;
		}

		// Token: 0x040009CA RID: 2506
		[Dependency]
		private readonly IGameTiming _timing;

		// Token: 0x040009CB RID: 2507
		[Dependency]
		private readonly INetManager _net;

		// Token: 0x040009CC RID: 2508
		[Dependency]
		private readonly AccessReaderSystem _accessReader;

		// Token: 0x040009CD RID: 2509
		[Dependency]
		private readonly SharedAppearanceSystem _appearanceSystem;

		// Token: 0x040009CE RID: 2510
		[Dependency]
		private readonly SharedAudioSystem _audio;

		// Token: 0x040009CF RID: 2511
		[Dependency]
		private readonly SharedPopupSystem _sharedPopupSystem;
	}
}
