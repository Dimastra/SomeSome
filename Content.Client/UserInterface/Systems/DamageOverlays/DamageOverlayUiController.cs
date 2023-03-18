using System;
using System.Runtime.CompilerServices;
using Content.Client.Alerts;
using Content.Client.Gameplay;
using Content.Client.UserInterface.Systems.DamageOverlays.Overlays;
using Content.Shared.Damage;
using Content.Shared.FixedPoint;
using Content.Shared.Mobs;
using Content.Shared.Mobs.Components;
using Content.Shared.Mobs.Systems;
using Robust.Client.GameObjects;
using Robust.Client.Graphics;
using Robust.Client.Player;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.Controllers;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;

namespace Content.Client.UserInterface.Systems.DamageOverlays
{
	// Token: 0x0200009D RID: 157
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class DamageOverlayUiController : UIController, IOnStateChanged<GameplayState>, IOnStateEntered<GameplayState>, IOnStateExited<GameplayState>
	{
		// Token: 0x060003BB RID: 955 RVA: 0x00015E44 File Offset: 0x00014044
		public override void Initialize()
		{
			this._overlay = new DamageOverlay();
			base.SubscribeLocalEvent<PlayerAttachedEvent>(new EntityEventHandler<PlayerAttachedEvent>(this.OnPlayerAttach), null, null);
			base.SubscribeLocalEvent<PlayerDetachedEvent>(new EntityEventHandler<PlayerDetachedEvent>(this.OnPlayerDetached), null, null);
			base.SubscribeLocalEvent<MobStateChangedEvent>(new EntityEventHandler<MobStateChangedEvent>(this.OnMobStateChanged), null, null);
			base.SubscribeLocalEvent<MobThresholdChecked>(new EntityEventRefHandler<MobThresholdChecked>(this.OnThresholdCheck), null, null);
		}

		// Token: 0x060003BC RID: 956 RVA: 0x00015EAC File Offset: 0x000140AC
		public void OnStateEntered(GameplayState state)
		{
			this._overlayManager.AddOverlay(this._overlay);
		}

		// Token: 0x060003BD RID: 957 RVA: 0x00015EC0 File Offset: 0x000140C0
		public void OnStateExited(GameplayState state)
		{
			this._overlayManager.RemoveOverlay(this._overlay);
		}

		// Token: 0x060003BE RID: 958 RVA: 0x00015ED4 File Offset: 0x000140D4
		private void OnPlayerAttach(PlayerAttachedEvent args)
		{
			this.ClearOverlay();
			MobStateComponent mobStateComponent;
			if (!this.EntityManager.TryGetComponent<MobStateComponent>(args.Entity, ref mobStateComponent))
			{
				return;
			}
			if (mobStateComponent.CurrentState != MobState.Dead)
			{
				this.UpdateOverlays(args.Entity, mobStateComponent, null, null);
			}
			this._overlayManager.AddOverlay(this._overlay);
		}

		// Token: 0x060003BF RID: 959 RVA: 0x00015F27 File Offset: 0x00014127
		private void OnPlayerDetached(PlayerDetachedEvent args)
		{
			this._overlayManager.RemoveOverlay(this._overlay);
			this.ClearOverlay();
		}

		// Token: 0x060003C0 RID: 960 RVA: 0x00015F44 File Offset: 0x00014144
		private void OnMobStateChanged(MobStateChangedEvent args)
		{
			EntityUid target = args.Target;
			LocalPlayer localPlayer = this._playerManager.LocalPlayer;
			if (target != ((localPlayer != null) ? localPlayer.ControlledEntity : null))
			{
				return;
			}
			this.UpdateOverlays(args.Target, args.Component, null, null);
		}

		// Token: 0x060003C1 RID: 961 RVA: 0x00015FAC File Offset: 0x000141AC
		private void OnThresholdCheck(ref MobThresholdChecked args)
		{
			EntityUid target = args.Target;
			LocalPlayer localPlayer = this._playerManager.LocalPlayer;
			if (target != ((localPlayer != null) ? localPlayer.ControlledEntity : null))
			{
				return;
			}
			this.UpdateOverlays(args.Target, args.MobState, args.Damageable, args.Threshold);
		}

		// Token: 0x060003C2 RID: 962 RVA: 0x0001601C File Offset: 0x0001421C
		private void ClearOverlay()
		{
			this._overlay.DeadLevel = 0f;
			this._overlay.CritLevel = 0f;
			this._overlay.BruteLevel = 0f;
			this._overlay.OxygenLevel = 0f;
		}

		// Token: 0x060003C3 RID: 963 RVA: 0x0001606C File Offset: 0x0001426C
		[NullableContext(2)]
		private void UpdateOverlays(EntityUid entity, MobStateComponent mobState, DamageableComponent damageable = null, MobThresholdsComponent thresholds = null)
		{
			if ((mobState == null && !this.EntityManager.TryGetComponent<MobStateComponent>(entity, ref mobState)) || (thresholds == null && !this.EntityManager.TryGetComponent<MobThresholdsComponent>(entity, ref thresholds)) || (damageable == null && !this.EntityManager.TryGetComponent<DamageableComponent>(entity, ref damageable)))
			{
				return;
			}
			FixedPoint2? fixedPoint;
			if (!this._mobThresholdSystem.TryGetIncapThreshold(entity, out fixedPoint, thresholds))
			{
				return;
			}
			FixedPoint2 value = fixedPoint.Value;
			this._overlay.State = mobState.CurrentState;
			switch (mobState.CurrentState)
			{
			case MobState.Alive:
			{
				FixedPoint2 a;
				if (damageable.DamagePerGroup.TryGetValue("Brute", out a))
				{
					this._overlay.BruteLevel = FixedPoint2.Min(1f, a / value).Float();
				}
				FixedPoint2 a2;
				if (damageable.DamagePerGroup.TryGetValue("Airloss", out a2))
				{
					this._overlay.OxygenLevel = FixedPoint2.Min(1f, a2 / value).Float();
				}
				if (this._overlay.BruteLevel < 0.05f)
				{
					this._overlay.BruteLevel = 0f;
				}
				this._overlay.CritLevel = 0f;
				this._overlay.DeadLevel = 0f;
				return;
			}
			case MobState.Critical:
			{
				FixedPoint2? fixedPoint2;
				if (!this._mobThresholdSystem.TryGetDeadPercentage(entity, FixedPoint2.Max(0.0, damageable.TotalDamage), out fixedPoint2, null))
				{
					return;
				}
				this._overlay.CritLevel = fixedPoint2.Value.Float();
				this._overlay.BruteLevel = 0f;
				this._overlay.DeadLevel = 0f;
				return;
			}
			case MobState.Dead:
				this._overlay.BruteLevel = 0f;
				this._overlay.CritLevel = 0f;
				return;
			default:
				return;
			}
		}

		// Token: 0x040001C2 RID: 450
		[Dependency]
		private readonly IOverlayManager _overlayManager;

		// Token: 0x040001C3 RID: 451
		[Dependency]
		private readonly IPlayerManager _playerManager;

		// Token: 0x040001C4 RID: 452
		[UISystemDependency]
		private readonly ClientAlertsSystem _alertsSystem;

		// Token: 0x040001C5 RID: 453
		[UISystemDependency]
		private readonly MobThresholdSystem _mobThresholdSystem;

		// Token: 0x040001C6 RID: 454
		private DamageOverlay _overlay;
	}
}
