using System;
using System.Runtime.CompilerServices;
using Content.Server.Administration.Logs;
using Content.Server.DoAfter;
using Content.Server.Popups;
using Content.Server.RCD.Components;
using Content.Shared.Examine;
using Content.Shared.Interaction;
using Content.Shared.Interaction.Events;
using Content.Shared.Maps;
using Content.Shared.Physics;
using Content.Shared.Popups;
using Content.Shared.Tag;
using Robust.Shared.Audio;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Localization;
using Robust.Shared.Map;
using Robust.Shared.Map.Components;
using Robust.Shared.Player;
using Robust.Shared.Timing;

namespace Content.Server.RCD.Systems
{
	// Token: 0x0200024C RID: 588
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class RCDSystem : EntitySystem
	{
		// Token: 0x06000BC4 RID: 3012 RVA: 0x0003DF3C File Offset: 0x0003C13C
		public override void Initialize()
		{
			base.Initialize();
			base.SubscribeLocalEvent<RCDComponent, ExaminedEvent>(new ComponentEventHandler<RCDComponent, ExaminedEvent>(this.OnExamine), null, null);
			base.SubscribeLocalEvent<RCDComponent, UseInHandEvent>(new ComponentEventHandler<RCDComponent, UseInHandEvent>(this.OnUseInHand), null, null);
			base.SubscribeLocalEvent<RCDComponent, AfterInteractEvent>(new ComponentEventHandler<RCDComponent, AfterInteractEvent>(this.OnAfterInteract), null, null);
		}

		// Token: 0x06000BC5 RID: 3013 RVA: 0x0003DF8C File Offset: 0x0003C18C
		private void OnExamine(EntityUid uid, RCDComponent component, ExaminedEvent args)
		{
			if (component.AutoRecharge)
			{
				if (component.CurrentAmmo == component.MaxAmmo)
				{
					args.PushMarkup(Loc.GetString("emag-max-charges"));
					return;
				}
				double timeRemaining = Math.Round((component.NextChargeTime - this._gameTiming.CurTime).TotalSeconds);
				args.PushMarkup(Loc.GetString("emag-recharging", new ValueTuple<string, object>[]
				{
					new ValueTuple<string, object>("seconds", timeRemaining)
				}));
			}
			string msg = Loc.GetString("rcd-component-examine-detail-count", new ValueTuple<string, object>[]
			{
				new ValueTuple<string, object>("mode", component.Mode),
				new ValueTuple<string, object>("ammoCount", component.CurrentAmmo)
			});
			args.PushMarkup(msg);
		}

		// Token: 0x06000BC6 RID: 3014 RVA: 0x0003E061 File Offset: 0x0003C261
		private void OnUseInHand(EntityUid uid, RCDComponent component, UseInHandEvent args)
		{
			if (args.Handled)
			{
				return;
			}
			this.NextMode(uid, component, args.User);
			args.Handled = true;
		}

		// Token: 0x06000BC7 RID: 3015 RVA: 0x0003E084 File Offset: 0x0003C284
		private void OnAfterInteract(EntityUid uid, RCDComponent rcd, AfterInteractEvent args)
		{
			RCDSystem.<OnAfterInteract>d__12 <OnAfterInteract>d__;
			<OnAfterInteract>d__.<>t__builder = AsyncVoidMethodBuilder.Create();
			<OnAfterInteract>d__.<>4__this = this;
			<OnAfterInteract>d__.uid = uid;
			<OnAfterInteract>d__.rcd = rcd;
			<OnAfterInteract>d__.args = args;
			<OnAfterInteract>d__.<>1__state = -1;
			<OnAfterInteract>d__.<>t__builder.Start<RCDSystem.<OnAfterInteract>d__12>(ref <OnAfterInteract>d__);
		}

		// Token: 0x06000BC8 RID: 3016 RVA: 0x0003E0D4 File Offset: 0x0003C2D4
		private bool IsRCDStillValid(RCDComponent rcd, AfterInteractEvent eventArgs, MapGridComponent mapGrid, TileRef tile, RcdMode startingMode)
		{
			if (rcd.CurrentAmmo <= 0)
			{
				this._popup.PopupEntity(Loc.GetString("rcd-component-no-ammo-message"), rcd.Owner, eventArgs.User, PopupType.Small);
				return false;
			}
			if (rcd.Mode != startingMode)
			{
				return false;
			}
			if (!((eventArgs.Target == null) ? this._interactionSystem.InRangeUnobstructed(eventArgs.User, mapGrid.GridTileToWorld(tile.GridIndices), 1.5f, CollisionGroup.Impassable | CollisionGroup.InteractImpassable, null, true) : this._interactionSystem.InRangeUnobstructed(eventArgs.User, eventArgs.Target.Value, 1.5f, CollisionGroup.Impassable | CollisionGroup.InteractImpassable, null, true)))
			{
				return false;
			}
			switch (rcd.Mode)
			{
			case RcdMode.Floors:
				if (!tile.Tile.IsEmpty)
				{
					this._popup.PopupEntity(Loc.GetString("rcd-component-cannot-build-floor-tile-not-empty-message"), rcd.Owner, eventArgs.User, PopupType.Small);
					return false;
				}
				return true;
			case RcdMode.Walls:
				if (tile.Tile.IsEmpty)
				{
					this._popup.PopupEntity(Loc.GetString("rcd-component-cannot-build-wall-tile-not-empty-message"), rcd.Owner, eventArgs.User, PopupType.Small);
					return false;
				}
				if (tile.IsBlockedTurf(true, null, null))
				{
					this._popup.PopupEntity(Loc.GetString("rcd-component-tile-obstructed-message"), rcd.Owner, eventArgs.User, PopupType.Small);
					return false;
				}
				return true;
			case RcdMode.Airlock:
				if (tile.Tile.IsEmpty)
				{
					this._popup.PopupEntity(Loc.GetString("rcd-component-cannot-build-airlock-tile-not-empty-message"), rcd.Owner, eventArgs.User, PopupType.Small);
					return false;
				}
				if (tile.IsBlockedTurf(true, null, null))
				{
					this._popup.PopupEntity(Loc.GetString("rcd-component-tile-obstructed-message"), rcd.Owner, eventArgs.User, PopupType.Small);
					return false;
				}
				return true;
			case RcdMode.Deconstruct:
				if (tile.Tile.IsEmpty)
				{
					return false;
				}
				if (eventArgs.Target == null && tile.IsBlockedTurf(true, null, null))
				{
					this._popup.PopupEntity(Loc.GetString("rcd-component-tile-obstructed-message"), rcd.Owner, eventArgs.User, PopupType.Small);
					return false;
				}
				if (eventArgs.Target != null && !this._tagSystem.HasTag(eventArgs.Target.Value, "RCDDeconstructWhitelist"))
				{
					this._popup.PopupEntity(Loc.GetString("rcd-component-deconstruct-target-not-on-whitelist-message"), rcd.Owner, eventArgs.User, PopupType.Small);
					return false;
				}
				return true;
			default:
				return false;
			}
		}

		// Token: 0x06000BC9 RID: 3017 RVA: 0x0003E34C File Offset: 0x0003C54C
		public override void Update(float frameTime)
		{
			base.Update(frameTime);
			foreach (RCDComponent rcd in base.EntityQuery<RCDComponent>(false))
			{
				if (rcd.AutoRecharge && rcd.CurrentAmmo != rcd.MaxAmmo && !(this._gameTiming.CurTime < rcd.NextChargeTime))
				{
					this.ChangeRCDCharge(rcd.Owner, 1, true, rcd);
				}
			}
		}

		// Token: 0x06000BCA RID: 3018 RVA: 0x0003E3D8 File Offset: 0x0003C5D8
		[NullableContext(2)]
		private bool ChangeRCDCharge(EntityUid uid, int change, bool resetTimer, RCDComponent component = null)
		{
			if (!base.Resolve<RCDComponent>(uid, ref component, true))
			{
				return false;
			}
			if (component.CurrentAmmo + change <= 0 || component.CurrentAmmo + change > component.MaxAmmo)
			{
				return false;
			}
			if (resetTimer || component.CurrentAmmo == component.MaxAmmo)
			{
				component.NextChargeTime = this._gameTiming.CurTime + component.RechargeDuration;
			}
			component.CurrentAmmo += change;
			base.Dirty(component, null);
			return true;
		}

		// Token: 0x06000BCB RID: 3019 RVA: 0x0003E460 File Offset: 0x0003C660
		private void NextMode(EntityUid uid, RCDComponent rcd, EntityUid user)
		{
			SoundSystem.Play(rcd.SwapModeSound.GetSound(null, null), Filter.Pvs(uid, 2f, this.EntityManager, null, null), uid, null);
			int mode = (int)rcd.Mode;
			mode = (mode + 1) % this.RCDModeCount;
			rcd.Mode = (RcdMode)mode;
			RcdMode mode2 = rcd.Mode;
			string @string;
			switch (mode2)
			{
			case RcdMode.Floors:
				@string = Loc.GetString("rcd-component-change-mode", new ValueTuple<string, object>[]
				{
					new ValueTuple<string, object>("mode", "Плитки")
				});
				break;
			case RcdMode.Walls:
				@string = Loc.GetString("rcd-component-change-mode", new ValueTuple<string, object>[]
				{
					new ValueTuple<string, object>("mode", "Стены")
				});
				break;
			case RcdMode.Airlock:
				@string = Loc.GetString("rcd-component-change-mode", new ValueTuple<string, object>[]
				{
					new ValueTuple<string, object>("mode", "Шлюз")
				});
				break;
			case RcdMode.Deconstruct:
				@string = Loc.GetString("rcd-component-change-mode", new ValueTuple<string, object>[]
				{
					new ValueTuple<string, object>("mode", "Разобрка")
				});
				break;
			default:
				<PrivateImplementationDetails>.ThrowSwitchExpressionException(mode2);
				break;
			}
			string msg = @string;
			this._popup.PopupEntity(msg, rcd.Owner, user, PopupType.Small);
		}

		// Token: 0x0400073E RID: 1854
		[Dependency]
		private readonly ITileDefinitionManager _tileDefinitionManager;

		// Token: 0x0400073F RID: 1855
		[Dependency]
		private readonly IMapManager _mapManager;

		// Token: 0x04000740 RID: 1856
		[Dependency]
		private readonly IAdminLogManager _adminLogger;

		// Token: 0x04000741 RID: 1857
		[Dependency]
		private readonly DoAfterSystem _doAfterSystem;

		// Token: 0x04000742 RID: 1858
		[Dependency]
		private readonly SharedInteractionSystem _interactionSystem;

		// Token: 0x04000743 RID: 1859
		[Dependency]
		private readonly PopupSystem _popup;

		// Token: 0x04000744 RID: 1860
		[Dependency]
		private readonly TagSystem _tagSystem;

		// Token: 0x04000745 RID: 1861
		[Dependency]
		private readonly IGameTiming _gameTiming;

		// Token: 0x04000746 RID: 1862
		private readonly int RCDModeCount = Enum.GetValues(typeof(RcdMode)).Length;
	}
}
