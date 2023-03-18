using System;
using System.Runtime.CompilerServices;
using Content.Server.Coordinates.Helpers;
using Content.Server.Popups;
using Content.Shared.Actions;
using Content.Shared.Alert;
using Content.Shared.Doors.Components;
using Content.Shared.Maps;
using Content.Shared.Mobs.Components;
using Content.Shared.Popups;
using Content.Shared.Speech;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Localization;
using Robust.Shared.Map;
using Robust.Shared.Maths;
using Robust.Shared.Physics.Components;
using Robust.Shared.Timing;

namespace Content.Server.Abilities.Mime
{
	// Token: 0x02000884 RID: 2180
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class MimePowersSystem : EntitySystem
	{
		// Token: 0x06002F86 RID: 12166 RVA: 0x000F6168 File Offset: 0x000F4368
		public override void Initialize()
		{
			base.Initialize();
			base.SubscribeLocalEvent<MimePowersComponent, ComponentInit>(new ComponentEventHandler<MimePowersComponent, ComponentInit>(this.OnComponentInit), null, null);
			base.SubscribeLocalEvent<MimePowersComponent, SpeakAttemptEvent>(new ComponentEventHandler<MimePowersComponent, SpeakAttemptEvent>(this.OnSpeakAttempt), null, null);
			base.SubscribeLocalEvent<MimePowersComponent, InvisibleWallActionEvent>(new ComponentEventHandler<MimePowersComponent, InvisibleWallActionEvent>(this.OnInvisibleWall), null, null);
		}

		// Token: 0x06002F87 RID: 12167 RVA: 0x000F61B8 File Offset: 0x000F43B8
		public override void Update(float frameTime)
		{
			base.Update(frameTime);
			foreach (MimePowersComponent mime in base.EntityQuery<MimePowersComponent>(false))
			{
				if (mime.VowBroken && !mime.ReadyToRepent && !(this._timing.CurTime < mime.VowRepentTime))
				{
					mime.ReadyToRepent = true;
					this._popupSystem.PopupEntity(Loc.GetString("mime-ready-to-repent"), mime.Owner, mime.Owner, PopupType.Small);
				}
			}
		}

		// Token: 0x06002F88 RID: 12168 RVA: 0x000F6258 File Offset: 0x000F4458
		private void OnComponentInit(EntityUid uid, MimePowersComponent component, ComponentInit args)
		{
			this._actionsSystem.AddAction(uid, component.InvisibleWallAction, new EntityUid?(uid), null, true);
			this._alertsSystem.ShowAlert(uid, AlertType.VowOfSilence, null, null);
		}

		// Token: 0x06002F89 RID: 12169 RVA: 0x000F629F File Offset: 0x000F449F
		private void OnSpeakAttempt(EntityUid uid, MimePowersComponent component, SpeakAttemptEvent args)
		{
			if (!component.Enabled)
			{
				return;
			}
			this._popupSystem.PopupEntity(Loc.GetString("mime-cant-speak"), uid, uid, PopupType.Small);
			args.Cancel();
		}

		// Token: 0x06002F8A RID: 12170 RVA: 0x000F62C8 File Offset: 0x000F44C8
		private void OnInvisibleWall(EntityUid uid, MimePowersComponent component, InvisibleWallActionEvent args)
		{
			if (!component.Enabled)
			{
				return;
			}
			TransformComponent transformComponent = base.Transform(uid);
			Vector2 offsetValue = transformComponent.LocalRotation.ToWorldVec().Normalized;
			EntityCoordinates coords = transformComponent.Coordinates.Offset(offsetValue).SnapToGrid(this.EntityManager, null);
			foreach (EntityUid entity in coords.GetEntitiesInTile(4, null))
			{
				PhysicsComponent physics = null;
				DoorComponent door;
				if ((base.HasComp<MobStateComponent>(entity) && entity != uid) || (base.Resolve<PhysicsComponent>(entity, ref physics, false) && (physics.CollisionLayer & 2) != 0 && (!base.TryComp<DoorComponent>(entity, ref door) || door.State == DoorState.Closed)))
				{
					this._popupSystem.PopupEntity(Loc.GetString("mime-invisible-wall-failed"), uid, uid, PopupType.Small);
					return;
				}
			}
			this._popupSystem.PopupEntity(Loc.GetString("mime-invisible-wall-popup", new ValueTuple<string, object>[]
			{
				new ValueTuple<string, object>("mime", uid)
			}), uid, PopupType.Small);
			base.Spawn(component.WallPrototype, coords);
			args.Handled = true;
		}

		// Token: 0x06002F8B RID: 12171 RVA: 0x000F6400 File Offset: 0x000F4600
		[NullableContext(2)]
		public void BreakVow(EntityUid uid, MimePowersComponent mimePowers = null)
		{
			if (!base.Resolve<MimePowersComponent>(uid, ref mimePowers, true))
			{
				return;
			}
			if (mimePowers.VowBroken)
			{
				return;
			}
			mimePowers.Enabled = false;
			mimePowers.VowBroken = true;
			mimePowers.VowRepentTime = this._timing.CurTime + mimePowers.VowCooldown;
			this._alertsSystem.ClearAlert(uid, AlertType.VowOfSilence);
			this._alertsSystem.ShowAlert(uid, AlertType.VowBroken, null, null);
			this._actionsSystem.RemoveAction(uid, mimePowers.InvisibleWallAction, null);
		}

		// Token: 0x06002F8C RID: 12172 RVA: 0x000F6490 File Offset: 0x000F4690
		[NullableContext(2)]
		public void RetakeVow(EntityUid uid, MimePowersComponent mimePowers = null)
		{
			if (!base.Resolve<MimePowersComponent>(uid, ref mimePowers, true))
			{
				return;
			}
			if (!mimePowers.ReadyToRepent)
			{
				this._popupSystem.PopupEntity(Loc.GetString("mime-not-ready-repent"), uid, uid, PopupType.Small);
				return;
			}
			mimePowers.Enabled = true;
			mimePowers.ReadyToRepent = false;
			mimePowers.VowBroken = false;
			this._alertsSystem.ClearAlert(uid, AlertType.VowBroken);
			this._alertsSystem.ShowAlert(uid, AlertType.VowOfSilence, null, null);
			this._actionsSystem.AddAction(uid, mimePowers.InvisibleWallAction, new EntityUid?(uid), null, true);
		}

		// Token: 0x04001C9C RID: 7324
		[Dependency]
		private readonly PopupSystem _popupSystem;

		// Token: 0x04001C9D RID: 7325
		[Dependency]
		private readonly SharedActionsSystem _actionsSystem;

		// Token: 0x04001C9E RID: 7326
		[Dependency]
		private readonly AlertsSystem _alertsSystem;

		// Token: 0x04001C9F RID: 7327
		[Dependency]
		private readonly IGameTiming _timing;
	}
}
