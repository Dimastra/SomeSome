using System;
using System.Runtime.CompilerServices;
using Content.Server.Light.Components;
using Content.Server.Light.Events;
using Content.Server.Mind.Components;
using Content.Shared.Actions;
using Content.Shared.Light;
using Content.Shared.Toggleable;
using Content.Shared.Verbs;
using Robust.Server.GameObjects;
using Robust.Shared.Audio;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Localization;
using Robust.Shared.Player;
using Robust.Shared.Utility;

namespace Content.Server.Light.EntitySystems
{
	// Token: 0x02000414 RID: 1044
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class UnpoweredFlashlightSystem : EntitySystem
	{
		// Token: 0x06001545 RID: 5445 RVA: 0x0006FCF0 File Offset: 0x0006DEF0
		public override void Initialize()
		{
			base.Initialize();
			base.SubscribeLocalEvent<UnpoweredFlashlightComponent, GetVerbsEvent<ActivationVerb>>(new ComponentEventHandler<UnpoweredFlashlightComponent, GetVerbsEvent<ActivationVerb>>(this.AddToggleLightVerbs), null, null);
			base.SubscribeLocalEvent<UnpoweredFlashlightComponent, GetItemActionsEvent>(new ComponentEventHandler<UnpoweredFlashlightComponent, GetItemActionsEvent>(this.OnGetActions), null, null);
			base.SubscribeLocalEvent<UnpoweredFlashlightComponent, ToggleActionEvent>(new ComponentEventHandler<UnpoweredFlashlightComponent, ToggleActionEvent>(this.OnToggleAction), null, null);
			base.SubscribeLocalEvent<UnpoweredFlashlightComponent, MindAddedMessage>(new ComponentEventHandler<UnpoweredFlashlightComponent, MindAddedMessage>(this.OnMindAdded), null, null);
		}

		// Token: 0x06001546 RID: 5446 RVA: 0x0006FD53 File Offset: 0x0006DF53
		private void OnToggleAction(EntityUid uid, UnpoweredFlashlightComponent component, ToggleActionEvent args)
		{
			if (args.Handled)
			{
				return;
			}
			this.ToggleLight(uid, component);
			args.Handled = true;
		}

		// Token: 0x06001547 RID: 5447 RVA: 0x0006FD6D File Offset: 0x0006DF6D
		private void OnGetActions(EntityUid uid, UnpoweredFlashlightComponent component, GetItemActionsEvent args)
		{
			args.Actions.Add(component.ToggleAction);
		}

		// Token: 0x06001548 RID: 5448 RVA: 0x0006FD84 File Offset: 0x0006DF84
		private void AddToggleLightVerbs(EntityUid uid, UnpoweredFlashlightComponent component, GetVerbsEvent<ActivationVerb> args)
		{
			if (!args.CanAccess || !args.CanInteract)
			{
				return;
			}
			ActivationVerb verb = new ActivationVerb();
			verb.Text = Loc.GetString("toggle-flashlight-verb-get-data-text");
			verb.Icon = new SpriteSpecifier.Texture(new ResourcePath("/Textures/Interface/VerbIcons/light.svg.192dpi.png", "/"));
			verb.Act = delegate()
			{
				this.ToggleLight(uid, component);
			};
			verb.Priority = -1;
			args.Verbs.Add(verb);
		}

		// Token: 0x06001549 RID: 5449 RVA: 0x0006FE14 File Offset: 0x0006E014
		private void OnMindAdded(EntityUid uid, UnpoweredFlashlightComponent component, MindAddedMessage args)
		{
			this._actionsSystem.AddAction(uid, component.ToggleAction, null, null, true);
		}

		// Token: 0x0600154A RID: 5450 RVA: 0x0006FE40 File Offset: 0x0006E040
		public void ToggleLight(EntityUid uid, UnpoweredFlashlightComponent flashlight)
		{
			PointLightComponent light;
			if (!this.EntityManager.TryGetComponent<PointLightComponent>(flashlight.Owner, ref light))
			{
				return;
			}
			flashlight.LightOn = !flashlight.LightOn;
			light.Enabled = flashlight.LightOn;
			AppearanceComponent appearance;
			if (this.EntityManager.TryGetComponent<AppearanceComponent>(flashlight.Owner, ref appearance))
			{
				this._appearance.SetData(uid, UnpoweredFlashlightVisuals.LightOn, flashlight.LightOn, appearance);
			}
			SoundSystem.Play(flashlight.ToggleSound.GetSound(null, null), Filter.Pvs(light.Owner, 2f, null, null, null), flashlight.Owner, null);
			base.RaiseLocalEvent<LightToggleEvent>(flashlight.Owner, new LightToggleEvent(flashlight.LightOn), true);
			this._actionsSystem.SetToggled(flashlight.ToggleAction, flashlight.LightOn);
		}

		// Token: 0x04000D24 RID: 3364
		[Dependency]
		private readonly SharedActionsSystem _actionsSystem;

		// Token: 0x04000D25 RID: 3365
		[Dependency]
		private readonly SharedAppearanceSystem _appearance;
	}
}
