using System;
using System.Runtime.CompilerServices;
using System.Threading;
using Content.Server.AlertLevel;
using Content.Server.Audio;
using Content.Server.Chat.Systems;
using Content.Server.DoAfter;
using Content.Server.Explosion.EntitySystems;
using Content.Server.Popups;
using Content.Server.Station.Systems;
using Content.Shared.Audio;
using Content.Shared.Construction.Components;
using Content.Shared.Containers.ItemSlots;
using Content.Shared.DoAfter;
using Content.Shared.Nuke;
using Content.Shared.Popups;
using Robust.Server.GameObjects;
using Robust.Shared.Audio;
using Robust.Shared.Containers;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Localization;
using Robust.Shared.Map;
using Robust.Shared.Maths;
using Robust.Shared.Player;
using Robust.Shared.Random;

namespace Content.Server.Nuke
{
	// Token: 0x02000329 RID: 809
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class NukeSystem : EntitySystem
	{
		// Token: 0x060010A5 RID: 4261 RVA: 0x000559A4 File Offset: 0x00053BA4
		public override void Initialize()
		{
			base.Initialize();
			base.SubscribeLocalEvent<NukeComponent, ComponentInit>(new ComponentEventHandler<NukeComponent, ComponentInit>(this.OnInit), null, null);
			base.SubscribeLocalEvent<NukeComponent, ComponentRemove>(new ComponentEventHandler<NukeComponent, ComponentRemove>(this.OnRemove), null, null);
			base.SubscribeLocalEvent<NukeComponent, MapInitEvent>(new ComponentEventHandler<NukeComponent, MapInitEvent>(this.OnMapInit), null, null);
			base.SubscribeLocalEvent<NukeComponent, EntInsertedIntoContainerMessage>(new ComponentEventHandler<NukeComponent, EntInsertedIntoContainerMessage>(this.OnItemSlotChanged), null, null);
			base.SubscribeLocalEvent<NukeComponent, EntRemovedFromContainerMessage>(new ComponentEventHandler<NukeComponent, EntRemovedFromContainerMessage>(this.OnItemSlotChanged), null, null);
			base.SubscribeLocalEvent<NukeComponent, AnchorAttemptEvent>(new ComponentEventHandler<NukeComponent, AnchorAttemptEvent>(this.OnAnchorAttempt), null, null);
			base.SubscribeLocalEvent<NukeComponent, UnanchorAttemptEvent>(new ComponentEventHandler<NukeComponent, UnanchorAttemptEvent>(this.OnUnanchorAttempt), null, null);
			base.SubscribeLocalEvent<NukeComponent, AnchorStateChangedEvent>(new ComponentEventRefHandler<NukeComponent, AnchorStateChangedEvent>(this.OnAnchorChanged), null, null);
			base.SubscribeLocalEvent<NukeComponent, NukeAnchorMessage>(new ComponentEventHandler<NukeComponent, NukeAnchorMessage>(this.OnAnchorButtonPressed), null, null);
			base.SubscribeLocalEvent<NukeComponent, NukeArmedMessage>(new ComponentEventHandler<NukeComponent, NukeArmedMessage>(this.OnArmButtonPressed), null, null);
			base.SubscribeLocalEvent<NukeComponent, NukeKeypadMessage>(new ComponentEventHandler<NukeComponent, NukeKeypadMessage>(this.OnKeypadButtonPressed), null, null);
			base.SubscribeLocalEvent<NukeComponent, NukeKeypadClearMessage>(new ComponentEventHandler<NukeComponent, NukeKeypadClearMessage>(this.OnClearButtonPressed), null, null);
			base.SubscribeLocalEvent<NukeComponent, NukeKeypadEnterMessage>(new ComponentEventHandler<NukeComponent, NukeKeypadEnterMessage>(this.OnEnterButtonPressed), null, null);
			base.SubscribeLocalEvent<NukeComponent, DoAfterEvent>(new ComponentEventHandler<NukeComponent, DoAfterEvent>(this.OnDoAfter), null, null);
		}

		// Token: 0x060010A6 RID: 4262 RVA: 0x00055ACF File Offset: 0x00053CCF
		private void OnInit(EntityUid uid, NukeComponent component, ComponentInit args)
		{
			component.RemainingTime = (float)component.Timer;
			this._itemSlots.AddItemSlot(uid, "Nuke", component.DiskSlot, null);
			this.UpdateStatus(uid, component);
			this.UpdateUserInterface(uid, component);
		}

		// Token: 0x060010A7 RID: 4263 RVA: 0x00055B08 File Offset: 0x00053D08
		public override void Update(float frameTime)
		{
			base.Update(frameTime);
			foreach (NukeComponent nuke in base.EntityQuery<NukeComponent>(false))
			{
				NukeStatus status = nuke.Status;
				if (status != NukeStatus.ARMED)
				{
					if (status == NukeStatus.COOLDOWN)
					{
						this.TickCooldown(nuke.Owner, frameTime, nuke);
					}
				}
				else
				{
					this.TickTimer(nuke.Owner, frameTime, nuke);
				}
			}
		}

		// Token: 0x060010A8 RID: 4264 RVA: 0x00055B88 File Offset: 0x00053D88
		private void OnMapInit(EntityUid uid, NukeComponent nuke, MapInitEvent args)
		{
			EntityUid? originStation = this._stationSystem.GetOwningStation(uid, null);
			if (originStation != null)
			{
				nuke.OriginStation = originStation;
			}
			else
			{
				TransformComponent transform = base.Transform(uid);
				nuke.OriginMapGrid = new ValueTuple<MapId, EntityUid?>?(new ValueTuple<MapId, EntityUid?>(transform.MapID, transform.GridUid));
			}
			nuke.Code = this.GenerateRandomNumberString(nuke.CodeLength);
		}

		// Token: 0x060010A9 RID: 4265 RVA: 0x00055BEB File Offset: 0x00053DEB
		private void OnRemove(EntityUid uid, NukeComponent component, ComponentRemove args)
		{
			this._itemSlots.RemoveItemSlot(uid, component.DiskSlot, null);
		}

		// Token: 0x060010AA RID: 4266 RVA: 0x00055C00 File Offset: 0x00053E00
		private void OnItemSlotChanged(EntityUid uid, NukeComponent component, ContainerModifiedMessage args)
		{
			if (!component.Initialized)
			{
				return;
			}
			if (args.Container.ID != component.DiskSlot.ID)
			{
				return;
			}
			this.UpdateStatus(uid, component);
			this.UpdateUserInterface(uid, component);
		}

		// Token: 0x060010AB RID: 4267 RVA: 0x00055C39 File Offset: 0x00053E39
		private void OnAnchorAttempt(EntityUid uid, NukeComponent component, AnchorAttemptEvent args)
		{
			this.CheckAnchorAttempt(uid, component, args);
		}

		// Token: 0x060010AC RID: 4268 RVA: 0x00055C44 File Offset: 0x00053E44
		private void OnUnanchorAttempt(EntityUid uid, NukeComponent component, UnanchorAttemptEvent args)
		{
			this.CheckAnchorAttempt(uid, component, args);
		}

		// Token: 0x060010AD RID: 4269 RVA: 0x00055C50 File Offset: 0x00053E50
		private void CheckAnchorAttempt(EntityUid uid, NukeComponent component, BaseAnchoredAttemptEvent args)
		{
			if (component.Status == NukeStatus.ARMED)
			{
				string msg = Loc.GetString("nuke-component-cant-anchor");
				this._popups.PopupEntity(msg, uid, args.User, PopupType.Small);
				args.Cancel();
			}
		}

		// Token: 0x060010AE RID: 4270 RVA: 0x00055C8B File Offset: 0x00053E8B
		private void OnAnchorChanged(EntityUid uid, NukeComponent component, ref AnchorStateChangedEvent args)
		{
			this.UpdateUserInterface(uid, component);
			if (!args.Anchored && component.Status == NukeStatus.ARMED && component.RemainingTime > component.DisarmDoafterLength)
			{
				this.DisarmBomb(uid, component);
			}
		}

		// Token: 0x060010AF RID: 4271 RVA: 0x00055CBC File Offset: 0x00053EBC
		private void OnAnchorButtonPressed(EntityUid uid, NukeComponent component, NukeAnchorMessage args)
		{
			NukeSystem.<OnAnchorButtonPressed>d__23 <OnAnchorButtonPressed>d__;
			<OnAnchorButtonPressed>d__.<>t__builder = AsyncVoidMethodBuilder.Create();
			<OnAnchorButtonPressed>d__.<>4__this = this;
			<OnAnchorButtonPressed>d__.uid = uid;
			<OnAnchorButtonPressed>d__.component = component;
			<OnAnchorButtonPressed>d__.<>1__state = -1;
			<OnAnchorButtonPressed>d__.<>t__builder.Start<NukeSystem.<OnAnchorButtonPressed>d__23>(ref <OnAnchorButtonPressed>d__);
		}

		// Token: 0x060010B0 RID: 4272 RVA: 0x00055D03 File Offset: 0x00053F03
		private void OnEnterButtonPressed(EntityUid uid, NukeComponent component, NukeKeypadEnterMessage args)
		{
			if (component.Status != NukeStatus.AWAIT_CODE)
			{
				return;
			}
			this.UpdateStatus(uid, component);
			this.UpdateUserInterface(uid, component);
		}

		// Token: 0x060010B1 RID: 4273 RVA: 0x00055D20 File Offset: 0x00053F20
		private void OnKeypadButtonPressed(EntityUid uid, NukeComponent component, NukeKeypadMessage args)
		{
			this.PlayNukeKeypadSound(uid, args.Value, component);
			if (component.Status != NukeStatus.AWAIT_CODE)
			{
				return;
			}
			if (component.EnteredCode.Length >= component.CodeLength)
			{
				return;
			}
			component.EnteredCode += args.Value.ToString();
			this.UpdateUserInterface(uid, component);
		}

		// Token: 0x060010B2 RID: 4274 RVA: 0x00055D80 File Offset: 0x00053F80
		private void OnClearButtonPressed(EntityUid uid, NukeComponent component, NukeKeypadClearMessage args)
		{
			this._audio.Play(component.KeypadPressSound, Filter.Pvs(uid, 2f, null, null, null), uid, true, null);
			if (component.Status != NukeStatus.AWAIT_CODE)
			{
				return;
			}
			component.EnteredCode = "";
			this.UpdateUserInterface(uid, component);
		}

		// Token: 0x060010B3 RID: 4275 RVA: 0x00055DD8 File Offset: 0x00053FD8
		private void OnArmButtonPressed(EntityUid uid, NukeComponent component, NukeArmedMessage args)
		{
			if (!component.DiskSlot.HasItem)
			{
				return;
			}
			if (component.Status == NukeStatus.AWAIT_ARM && base.Transform(uid).Anchored)
			{
				this.ArmBomb(uid, component);
				return;
			}
			EntityUid? attachedEntity = args.Session.AttachedEntity;
			if (attachedEntity != null)
			{
				EntityUid user = attachedEntity.GetValueOrDefault();
				this.DisarmBombDoafter(uid, user, component);
				return;
			}
		}

		// Token: 0x060010B4 RID: 4276 RVA: 0x00055E3C File Offset: 0x0005403C
		private void OnDoAfter(EntityUid uid, NukeComponent component, DoAfterEvent args)
		{
			if (args.Handled || args.Cancelled)
			{
				return;
			}
			this.DisarmBomb(uid, component);
			NukeDisarmSuccessEvent ev = new NukeDisarmSuccessEvent();
			base.RaiseLocalEvent<NukeDisarmSuccessEvent>(ev);
			args.Handled = true;
		}

		// Token: 0x060010B5 RID: 4277 RVA: 0x00055E78 File Offset: 0x00054078
		[NullableContext(2)]
		private void TickCooldown(EntityUid uid, float frameTime, NukeComponent nuke = null)
		{
			if (!base.Resolve<NukeComponent>(uid, ref nuke, true))
			{
				return;
			}
			nuke.CooldownTime -= frameTime;
			if (nuke.CooldownTime <= 0f)
			{
				nuke.CooldownTime = 0f;
				nuke.Status = NukeStatus.AWAIT_ARM;
				this.UpdateStatus(uid, nuke);
			}
			this.UpdateUserInterface(uid, nuke);
		}

		// Token: 0x060010B6 RID: 4278 RVA: 0x00055ED0 File Offset: 0x000540D0
		[NullableContext(2)]
		private void TickTimer(EntityUid uid, float frameTime, NukeComponent nuke = null)
		{
			if (!base.Resolve<NukeComponent>(uid, ref nuke, true))
			{
				return;
			}
			nuke.RemainingTime -= frameTime;
			if (nuke.RemainingTime <= 111.6f + nuke.AlertSoundTime + 1.5f && !nuke.PlayedNukeSong)
			{
				this._soundSystem.DispatchStationEventMusic(uid, nuke.ArmMusic, StationEventMusicType.Nuke);
				nuke.PlayedNukeSong = true;
			}
			if (nuke.RemainingTime <= nuke.AlertSoundTime && !nuke.PlayedAlertSound)
			{
				nuke.AlertAudioStream = this._audio.Play(nuke.AlertSound, Filter.Broadcast(), uid, true, null);
				this._soundSystem.StopStationEventMusic(uid, StationEventMusicType.Nuke);
				nuke.PlayedAlertSound = true;
			}
			if (nuke.RemainingTime <= 0f)
			{
				nuke.RemainingTime = 0f;
				this.ActivateBomb(uid, nuke, null);
				return;
			}
			this.UpdateUserInterface(uid, nuke);
		}

		// Token: 0x060010B7 RID: 4279 RVA: 0x00055FB0 File Offset: 0x000541B0
		[NullableContext(2)]
		private void UpdateStatus(EntityUid uid, NukeComponent component = null)
		{
			if (!base.Resolve<NukeComponent>(uid, ref component, true))
			{
				return;
			}
			switch (component.Status)
			{
			case NukeStatus.AWAIT_DISK:
				if (component.DiskSlot.HasItem)
				{
					component.Status = NukeStatus.AWAIT_CODE;
					return;
				}
				break;
			case NukeStatus.AWAIT_CODE:
				if (!component.DiskSlot.HasItem)
				{
					component.Status = NukeStatus.AWAIT_DISK;
					component.EnteredCode = "";
					return;
				}
				if (component.EnteredCode == component.Code)
				{
					component.Status = NukeStatus.AWAIT_ARM;
					component.RemainingTime = (float)component.Timer;
					this._audio.Play(component.AccessGrantedSound, Filter.Pvs(uid, 2f, null, null, null), uid, true, null);
					return;
				}
				component.EnteredCode = "";
				this._audio.Play(component.AccessDeniedSound, Filter.Pvs(uid, 2f, null, null, null), uid, true, null);
				break;
			case NukeStatus.AWAIT_ARM:
			case NukeStatus.ARMED:
				break;
			default:
				return;
			}
		}

		// Token: 0x060010B8 RID: 4280 RVA: 0x000560AC File Offset: 0x000542AC
		[NullableContext(2)]
		private void UpdateUserInterface(EntityUid uid, NukeComponent component = null)
		{
			if (!base.Resolve<NukeComponent>(uid, ref component, true))
			{
				return;
			}
			BoundUserInterface ui = this._ui.GetUiOrNull(uid, NukeUiKey.Key, null);
			if (ui == null)
			{
				return;
			}
			bool anchored = false;
			TransformComponent transform;
			if (this.EntityManager.TryGetComponent<TransformComponent>(uid, ref transform))
			{
				anchored = transform.Anchored;
			}
			bool allowArm = component.DiskSlot.HasItem && (component.Status == NukeStatus.AWAIT_ARM || component.Status == NukeStatus.ARMED);
			NukeUiState state = new NukeUiState
			{
				Status = component.Status,
				RemainingTime = (int)component.RemainingTime,
				DiskInserted = component.DiskSlot.HasItem,
				IsAnchored = anchored,
				AllowArm = allowArm,
				EnteredCodeLength = component.EnteredCode.Length,
				MaxCodeLength = component.CodeLength,
				CooldownTime = (int)component.CooldownTime
			};
			this._ui.SetUiState(ui, state, null, true);
		}

		// Token: 0x060010B9 RID: 4281 RVA: 0x00056198 File Offset: 0x00054398
		[NullableContext(2)]
		private void PlayNukeKeypadSound(EntityUid uid, int number, NukeComponent component = null)
		{
			if (!base.Resolve<NukeComponent>(uid, ref component, true))
			{
				return;
			}
			int num;
			switch (number)
			{
			case 0:
				num = component.LastPlayedKeypadSemitones + 12;
				break;
			case 1:
				num = 0;
				break;
			case 2:
				num = 2;
				break;
			case 3:
				num = 3;
				break;
			case 4:
				num = 4;
				break;
			case 5:
				num = 5;
				break;
			case 6:
				num = 6;
				break;
			case 7:
				num = 7;
				break;
			case 8:
				num = 9;
				break;
			case 9:
				num = 10;
				break;
			default:
				num = 0;
				break;
			}
			int semitoneShift = num;
			component.LastPlayedKeypadSemitones = ((number == 0) ? component.LastPlayedKeypadSemitones : semitoneShift);
			this._audio.Play(component.KeypadPressSound, Filter.Pvs(uid, 2f, null, null, null), uid, true, new AudioParams?(AudioHelpers.ShiftSemitone(semitoneShift).WithVolume(-5f)));
		}

		// Token: 0x060010BA RID: 4282 RVA: 0x00056264 File Offset: 0x00054464
		public string GenerateRandomNumberString(int length)
		{
			string ret = "";
			for (int i = 0; i < length; i++)
			{
				ret += ((char)this._random.Next(48, 58)).ToString();
			}
			return ret;
		}

		// Token: 0x060010BB RID: 4283 RVA: 0x000562A4 File Offset: 0x000544A4
		[NullableContext(2)]
		public void ArmBomb(EntityUid uid, NukeComponent component = null)
		{
			if (!base.Resolve<NukeComponent>(uid, ref component, true))
			{
				return;
			}
			if (component.Status == NukeStatus.ARMED)
			{
				return;
			}
			EntityUid? stationUid = this._stationSystem.GetOwningStation(uid, null);
			if (stationUid != null)
			{
				this._alertLevel.SetLevel(stationUid.Value, component.AlertLevelOnActivate, true, true, true, true, null, null);
			}
			TransformComponent nukeXform = base.Transform(uid);
			MapCoordinates pos = nukeXform.MapPosition;
			int x = (int)pos.X;
			int y = (int)pos.Y;
			DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(4, 2);
			defaultInterpolatedStringHandler.AppendLiteral("(");
			defaultInterpolatedStringHandler.AppendFormatted<int>(x);
			defaultInterpolatedStringHandler.AppendLiteral(", ");
			defaultInterpolatedStringHandler.AppendFormatted<int>(y);
			defaultInterpolatedStringHandler.AppendLiteral(")");
			string posText = defaultInterpolatedStringHandler.ToStringAndClear();
			string announcement = Loc.GetString("nuke-component-announcement-armed", new ValueTuple<string, object>[]
			{
				new ValueTuple<string, object>("time", (int)component.RemainingTime),
				new ValueTuple<string, object>("position", posText)
			});
			string sender = Loc.GetString("nuke-component-announcement-sender");
			this._chatSystem.DispatchStationAnnouncement(uid, announcement, sender, false, null, new Color?(Color.Red));
			this._soundSystem.PlayGlobalOnStation(uid, this._audio.GetSound(component.ArmSound), null);
			this._itemSlots.SetLock(uid, component.DiskSlot, true, null);
			nukeXform.Anchored = true;
			component.Status = NukeStatus.ARMED;
			this.UpdateUserInterface(uid, component);
		}

		// Token: 0x060010BC RID: 4284 RVA: 0x00056424 File Offset: 0x00054624
		[NullableContext(2)]
		public void DisarmBomb(EntityUid uid, NukeComponent component = null)
		{
			if (!base.Resolve<NukeComponent>(uid, ref component, true))
			{
				return;
			}
			if (component.Status != NukeStatus.ARMED)
			{
				return;
			}
			EntityUid? stationUid = this._stationSystem.GetOwningStation(uid, null);
			if (stationUid != null)
			{
				this._alertLevel.SetLevel(stationUid.Value, component.AlertLevelOnDeactivate, true, true, true, false, null, null);
			}
			string announcement = Loc.GetString("nuke-component-announcement-unarmed");
			string sender = Loc.GetString("nuke-component-announcement-sender");
			this._chatSystem.DispatchStationAnnouncement(uid, announcement, sender, false, null, null);
			component.PlayedNukeSong = false;
			this._soundSystem.PlayGlobalOnStation(uid, this._audio.GetSound(component.DisarmSound), null);
			this._soundSystem.StopStationEventMusic(uid, StationEventMusicType.Nuke);
			component.PlayedAlertSound = false;
			IPlayingAudioStream alertAudioStream = component.AlertAudioStream;
			if (alertAudioStream != null)
			{
				alertAudioStream.Stop();
			}
			this._itemSlots.SetLock(uid, component.DiskSlot, false, null);
			component.Status = NukeStatus.COOLDOWN;
			component.CooldownTime = (float)component.Cooldown;
			this.UpdateUserInterface(uid, component);
		}

		// Token: 0x060010BD RID: 4285 RVA: 0x0005652F File Offset: 0x0005472F
		[NullableContext(2)]
		public void ToggleBomb(EntityUid uid, NukeComponent component = null)
		{
			if (!base.Resolve<NukeComponent>(uid, ref component, true))
			{
				return;
			}
			if (component.Status == NukeStatus.ARMED)
			{
				this.DisarmBomb(uid, component);
				return;
			}
			this.ArmBomb(uid, component);
		}

		// Token: 0x060010BE RID: 4286 RVA: 0x00056558 File Offset: 0x00054758
		[NullableContext(2)]
		public void ActivateBomb(EntityUid uid, NukeComponent component = null, TransformComponent transform = null)
		{
			if (!base.Resolve<NukeComponent, TransformComponent>(uid, ref component, ref transform, true))
			{
				return;
			}
			if (component.Exploded)
			{
				return;
			}
			component.Exploded = true;
			this._explosions.QueueExplosion(uid, component.ExplosionType, component.TotalIntensity, component.IntensitySlope, component.MaxIntensity, 1f, int.MaxValue, true, false, null, true);
			base.RaiseLocalEvent<NukeExplodedEvent>(new NukeExplodedEvent
			{
				OwningStation = transform.GridUid
			});
			this._soundSystem.StopStationEventMusic(uid, StationEventMusicType.Nuke);
			this.EntityManager.DeleteEntity(uid);
		}

		// Token: 0x060010BF RID: 4287 RVA: 0x000565EE File Offset: 0x000547EE
		[NullableContext(2)]
		public void SetRemainingTime(EntityUid uid, float timer, NukeComponent component = null)
		{
			if (!base.Resolve<NukeComponent>(uid, ref component, true))
			{
				return;
			}
			component.RemainingTime = timer;
			this.UpdateUserInterface(uid, component);
		}

		// Token: 0x060010C0 RID: 4288 RVA: 0x0005660C File Offset: 0x0005480C
		private void DisarmBombDoafter(EntityUid uid, EntityUid user, NukeComponent nuke)
		{
			float disarmDoafterLength = nuke.DisarmDoafterLength;
			EntityUid? target = new EntityUid?(uid);
			DoAfterEventArgs doafter = new DoAfterEventArgs(user, disarmDoafterLength, default(CancellationToken), target, null)
			{
				BreakOnDamage = true,
				BreakOnStun = true,
				BreakOnTargetMove = true,
				BreakOnUserMove = true,
				NeedHand = true
			};
			this._doAfterSystem.DoAfter(doafter);
			this._popups.PopupEntity(Loc.GetString("nuke-component-doafter-warning"), user, user, PopupType.LargeCaution);
		}

		// Token: 0x040009E7 RID: 2535
		[Dependency]
		private readonly ItemSlotsSystem _itemSlots;

		// Token: 0x040009E8 RID: 2536
		[Dependency]
		private readonly PopupSystem _popups;

		// Token: 0x040009E9 RID: 2537
		[Dependency]
		private readonly ExplosionSystem _explosions;

		// Token: 0x040009EA RID: 2538
		[Dependency]
		private readonly AlertLevelSystem _alertLevel;

		// Token: 0x040009EB RID: 2539
		[Dependency]
		private readonly StationSystem _stationSystem;

		// Token: 0x040009EC RID: 2540
		[Dependency]
		private readonly ServerGlobalSoundSystem _soundSystem;

		// Token: 0x040009ED RID: 2541
		[Dependency]
		private readonly ChatSystem _chatSystem;

		// Token: 0x040009EE RID: 2542
		[Dependency]
		private readonly DoAfterSystem _doAfterSystem;

		// Token: 0x040009EF RID: 2543
		[Dependency]
		private readonly IRobustRandom _random;

		// Token: 0x040009F0 RID: 2544
		[Dependency]
		private readonly SharedAudioSystem _audio;

		// Token: 0x040009F1 RID: 2545
		[Dependency]
		private readonly UserInterfaceSystem _ui;

		// Token: 0x040009F2 RID: 2546
		private const float NukeSongLength = 111.6f;

		// Token: 0x040009F3 RID: 2547
		private const float NukeSongBuffer = 1.5f;
	}
}
