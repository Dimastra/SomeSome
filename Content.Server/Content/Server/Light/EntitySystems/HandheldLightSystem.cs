using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Content.Server.Popups;
using Content.Server.Power.Components;
using Content.Server.PowerCell;
using Content.Shared.Actions;
using Content.Shared.Actions.ActionTypes;
using Content.Shared.Examine;
using Content.Shared.Interaction;
using Content.Shared.Light;
using Content.Shared.Popups;
using Content.Shared.Rounding;
using Content.Shared.Toggleable;
using Content.Shared.Verbs;
using Robust.Server.GameObjects;
using Robust.Shared.Containers;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.IoC;
using Robust.Shared.Localization;
using Robust.Shared.Maths;
using Robust.Shared.Prototypes;
using Robust.Shared.Utility;

namespace Content.Server.Light.EntitySystems
{
	// Token: 0x0200040C RID: 1036
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class HandheldLightSystem : SharedHandheldLightSystem
	{
		// Token: 0x060014FA RID: 5370 RVA: 0x0006DF64 File Offset: 0x0006C164
		public override void Initialize()
		{
			base.Initialize();
			base.SubscribeLocalEvent<HandheldLightComponent, ComponentRemove>(new ComponentEventHandler<HandheldLightComponent, ComponentRemove>(this.OnRemove), null, null);
			base.SubscribeLocalEvent<HandheldLightComponent, ComponentGetState>(new ComponentEventRefHandler<HandheldLightComponent, ComponentGetState>(this.OnGetState), null, null);
			base.SubscribeLocalEvent<HandheldLightComponent, ExaminedEvent>(new ComponentEventHandler<HandheldLightComponent, ExaminedEvent>(this.OnExamine), null, null);
			base.SubscribeLocalEvent<HandheldLightComponent, GetVerbsEvent<ActivationVerb>>(new ComponentEventHandler<HandheldLightComponent, GetVerbsEvent<ActivationVerb>>(this.AddToggleLightVerb), null, null);
			base.SubscribeLocalEvent<HandheldLightComponent, ActivateInWorldEvent>(new ComponentEventHandler<HandheldLightComponent, ActivateInWorldEvent>(this.OnActivate), null, null);
			base.SubscribeLocalEvent<HandheldLightComponent, GetItemActionsEvent>(new ComponentEventHandler<HandheldLightComponent, GetItemActionsEvent>(this.OnGetActions), null, null);
			base.SubscribeLocalEvent<HandheldLightComponent, ToggleActionEvent>(new ComponentEventHandler<HandheldLightComponent, ToggleActionEvent>(this.OnToggleAction), null, null);
			base.SubscribeLocalEvent<HandheldLightComponent, EntInsertedIntoContainerMessage>(new ComponentEventHandler<HandheldLightComponent, EntInsertedIntoContainerMessage>(this.OnEntInserted), null, null);
			base.SubscribeLocalEvent<HandheldLightComponent, EntRemovedFromContainerMessage>(new ComponentEventHandler<HandheldLightComponent, EntRemovedFromContainerMessage>(this.OnEntRemoved), null, null);
		}

		// Token: 0x060014FB RID: 5371 RVA: 0x0006E02B File Offset: 0x0006C22B
		private void OnEntInserted(EntityUid uid, HandheldLightComponent component, EntInsertedIntoContainerMessage args)
		{
			this.UpdateLevel(uid, component);
		}

		// Token: 0x060014FC RID: 5372 RVA: 0x0006E035 File Offset: 0x0006C235
		private void OnEntRemoved(EntityUid uid, HandheldLightComponent component, EntRemovedFromContainerMessage args)
		{
			this.UpdateLevel(uid, component);
		}

		// Token: 0x060014FD RID: 5373 RVA: 0x0006E040 File Offset: 0x0006C240
		private void OnGetActions(EntityUid uid, HandheldLightComponent component, GetItemActionsEvent args)
		{
			InstantActionPrototype act;
			if (component.ToggleAction == null && this._proto.TryIndex<InstantActionPrototype>(component.ToggleActionId, ref act))
			{
				component.ToggleAction = new InstantAction(act);
			}
			if (component.ToggleAction != null)
			{
				args.Actions.Add(component.ToggleAction);
			}
		}

		// Token: 0x060014FE RID: 5374 RVA: 0x0006E090 File Offset: 0x0006C290
		private void OnToggleAction(EntityUid uid, HandheldLightComponent component, ToggleActionEvent args)
		{
			if (args.Handled)
			{
				return;
			}
			if (component.Activated)
			{
				this.TurnOff(uid, component, true);
			}
			else
			{
				this.TurnOn(args.Performer, uid, component);
			}
			args.Handled = true;
		}

		// Token: 0x060014FF RID: 5375 RVA: 0x0006E0C5 File Offset: 0x0006C2C5
		private void OnGetState(EntityUid uid, HandheldLightComponent component, ref ComponentGetState args)
		{
			args.State = new HandheldLightComponent.HandheldLightComponentState(component.Activated, this.GetLevel(uid, component));
		}

		// Token: 0x06001500 RID: 5376 RVA: 0x0006E0E0 File Offset: 0x0006C2E0
		private byte? GetLevel(EntityUid uid, HandheldLightComponent component)
		{
			BatteryComponent battery;
			if (!this._powerCell.TryGetBatteryFromSlot(uid, out battery, null))
			{
				return null;
			}
			if (MathHelper.CloseToPercent(battery.CurrentCharge, 0f, 1E-05) || component.Wattage > battery.CurrentCharge)
			{
				return new byte?(0);
			}
			return new byte?((byte)ContentHelpers.RoundToNearestLevels((double)(battery.CurrentCharge / battery.MaxCharge * 255f), 255.0, 6));
		}

		// Token: 0x06001501 RID: 5377 RVA: 0x0006E161 File Offset: 0x0006C361
		private void OnRemove(EntityUid uid, HandheldLightComponent component, ComponentRemove args)
		{
			this._activeLights.Remove(component);
		}

		// Token: 0x06001502 RID: 5378 RVA: 0x0006E170 File Offset: 0x0006C370
		private void OnActivate(EntityUid uid, HandheldLightComponent component, ActivateInWorldEvent args)
		{
			if (args.Handled)
			{
				return;
			}
			if (this.ToggleStatus(args.User, uid, component))
			{
				args.Handled = true;
			}
		}

		// Token: 0x06001503 RID: 5379 RVA: 0x0006E192 File Offset: 0x0006C392
		public bool ToggleStatus(EntityUid user, EntityUid uid, HandheldLightComponent component)
		{
			if (!component.Activated)
			{
				return this.TurnOn(user, uid, component);
			}
			return this.TurnOff(uid, component, true);
		}

		// Token: 0x06001504 RID: 5380 RVA: 0x0006E1AF File Offset: 0x0006C3AF
		private void OnExamine(EntityUid uid, HandheldLightComponent component, ExaminedEvent args)
		{
			args.PushMarkup(component.Activated ? Loc.GetString("handheld-light-component-on-examine-is-on-message") : Loc.GetString("handheld-light-component-on-examine-is-off-message"));
		}

		// Token: 0x06001505 RID: 5381 RVA: 0x0006E1D5 File Offset: 0x0006C3D5
		public override void Shutdown()
		{
			base.Shutdown();
			this._activeLights.Clear();
		}

		// Token: 0x06001506 RID: 5382 RVA: 0x0006E1E8 File Offset: 0x0006C3E8
		public override void Update(float frameTime)
		{
			RemQueue<HandheldLightComponent> toRemove = default(RemQueue<HandheldLightComponent>);
			foreach (HandheldLightComponent handheld in this._activeLights)
			{
				EntityUid uid = handheld.Owner;
				if (handheld.Deleted)
				{
					toRemove.Add(handheld);
				}
				else if (!base.Paused(uid, null))
				{
					this.TryUpdate(uid, handheld, frameTime);
				}
			}
			foreach (HandheldLightComponent light in toRemove)
			{
				this._activeLights.Remove(light);
			}
		}

		// Token: 0x06001507 RID: 5383 RVA: 0x0006E2B0 File Offset: 0x0006C4B0
		private void AddToggleLightVerb(EntityUid uid, HandheldLightComponent component, GetVerbsEvent<ActivationVerb> args)
		{
			if (!args.CanAccess || !args.CanInteract)
			{
				return;
			}
			ActivationVerb verb = new ActivationVerb
			{
				Text = Loc.GetString("verb-common-toggle-light"),
				Icon = new SpriteSpecifier.Texture(new ResourcePath("/Textures/Interface/VerbIcons/light.svg.192dpi.png", "/")),
				Act = (component.Activated ? delegate()
				{
					this.TurnOff(uid, component, true);
				} : delegate()
				{
					this.TurnOn(args.User, uid, component);
				})
			};
			args.Verbs.Add(verb);
		}

		// Token: 0x06001508 RID: 5384 RVA: 0x0006E36C File Offset: 0x0006C56C
		public bool TurnOff(EntityUid uid, HandheldLightComponent component, bool makeNoise = true)
		{
			PointLightComponent pointLightComponent;
			if (!component.Activated || !base.TryComp<PointLightComponent>(uid, ref pointLightComponent))
			{
				return false;
			}
			pointLightComponent.Enabled = false;
			base.SetActivated(uid, false, component, makeNoise);
			component.Level = null;
			this._activeLights.Remove(component);
			return true;
		}

		// Token: 0x06001509 RID: 5385 RVA: 0x0006E3BC File Offset: 0x0006C5BC
		public bool TurnOn(EntityUid user, EntityUid uid, HandheldLightComponent component)
		{
			PointLightComponent pointLightComponent;
			if (component.Activated || !base.TryComp<PointLightComponent>(uid, ref pointLightComponent))
			{
				return false;
			}
			BatteryComponent battery;
			if (!this._powerCell.TryGetBatteryFromSlot(uid, out battery, null) && !base.TryComp<BatteryComponent>(uid, ref battery))
			{
				this._audio.PlayPvs(this._audio.GetSound(component.TurnOnFailSound), uid, null);
				this._popup.PopupEntity(Loc.GetString("handheld-light-component-cell-missing-message"), uid, user, PopupType.Small);
				return false;
			}
			if (component.Wattage > battery.CurrentCharge)
			{
				this._audio.PlayPvs(this._audio.GetSound(component.TurnOnFailSound), uid, null);
				this._popup.PopupEntity(Loc.GetString("handheld-light-component-cell-dead-message"), uid, user, PopupType.Small);
				return false;
			}
			pointLightComponent.Enabled = true;
			base.SetActivated(uid, true, component, true);
			this._activeLights.Add(component);
			return true;
		}

		// Token: 0x0600150A RID: 5386 RVA: 0x0006E4AC File Offset: 0x0006C6AC
		public void TryUpdate(EntityUid uid, HandheldLightComponent component, float frameTime)
		{
			BatteryComponent battery;
			if (!this._powerCell.TryGetBatteryFromSlot(uid, out battery, null) && !base.TryComp<BatteryComponent>(uid, ref battery))
			{
				this.TurnOff(uid, component, false);
				return;
			}
			AppearanceComponent appearanceComponent = EntityManagerExt.GetComponentOrNull<AppearanceComponent>(this.EntityManager, uid);
			float fraction = battery.CurrentCharge / battery.MaxCharge;
			if ((double)fraction >= 0.3)
			{
				this._appearance.SetData(uid, HandheldLightVisuals.Power, HandheldLightPowerStates.FullPower, appearanceComponent);
			}
			else if ((double)fraction >= 0.1)
			{
				this._appearance.SetData(uid, HandheldLightVisuals.Power, HandheldLightPowerStates.LowPower, appearanceComponent);
			}
			else
			{
				this._appearance.SetData(uid, HandheldLightVisuals.Power, HandheldLightPowerStates.Dying, appearanceComponent);
			}
			if (component.Activated && !battery.TryUseCharge(component.Wattage * frameTime))
			{
				this.TurnOff(uid, component, false);
			}
			this.UpdateLevel(uid, component);
		}

		// Token: 0x0600150B RID: 5387 RVA: 0x0006E590 File Offset: 0x0006C790
		private void UpdateLevel(EntityUid uid, HandheldLightComponent comp)
		{
			byte? level = this.GetLevel(uid, comp);
			byte? b = level;
			int? num = (b != null) ? new int?((int)b.GetValueOrDefault()) : null;
			b = comp.Level;
			int? num2 = (b != null) ? new int?((int)b.GetValueOrDefault()) : null;
			if (num.GetValueOrDefault() == num2.GetValueOrDefault() & num != null == (num2 != null))
			{
				return;
			}
			comp.Level = level;
			base.Dirty(comp, null);
		}

		// Token: 0x04000D07 RID: 3335
		[Dependency]
		private readonly PopupSystem _popup;

		// Token: 0x04000D08 RID: 3336
		[Dependency]
		private readonly PowerCellSystem _powerCell;

		// Token: 0x04000D09 RID: 3337
		[Dependency]
		private readonly IPrototypeManager _proto;

		// Token: 0x04000D0A RID: 3338
		[Dependency]
		private readonly SharedAudioSystem _audio;

		// Token: 0x04000D0B RID: 3339
		[Dependency]
		private readonly SharedAppearanceSystem _appearance;

		// Token: 0x04000D0C RID: 3340
		private readonly HashSet<HandheldLightComponent> _activeLights = new HashSet<HandheldLightComponent>();
	}
}
