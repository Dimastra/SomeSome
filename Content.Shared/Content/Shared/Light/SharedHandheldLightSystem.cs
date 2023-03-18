using System;
using System.Runtime.CompilerServices;
using Content.Shared.Actions;
using Content.Shared.Clothing.EntitySystems;
using Content.Shared.Item;
using Content.Shared.Toggleable;
using Robust.Shared.Audio;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.IoC;

namespace Content.Shared.Light
{
	// Token: 0x02000365 RID: 869
	[NullableContext(1)]
	[Nullable(0)]
	public abstract class SharedHandheldLightSystem : EntitySystem
	{
		// Token: 0x06000A38 RID: 2616 RVA: 0x00022046 File Offset: 0x00020246
		public override void Initialize()
		{
			base.Initialize();
			base.SubscribeLocalEvent<HandheldLightComponent, ComponentInit>(new ComponentEventHandler<HandheldLightComponent, ComponentInit>(this.OnInit), null, null);
			base.SubscribeLocalEvent<HandheldLightComponent, ComponentHandleState>(new ComponentEventRefHandler<HandheldLightComponent, ComponentHandleState>(this.OnHandleState), null, null);
		}

		// Token: 0x06000A39 RID: 2617 RVA: 0x00022076 File Offset: 0x00020276
		private void OnInit(EntityUid uid, HandheldLightComponent component, ComponentInit args)
		{
			this.UpdateVisuals(uid, component, null);
			base.Dirty(component, null);
		}

		// Token: 0x06000A3A RID: 2618 RVA: 0x0002208C File Offset: 0x0002028C
		private void OnHandleState(EntityUid uid, HandheldLightComponent component, ref ComponentHandleState args)
		{
			HandheldLightComponent.HandheldLightComponentState state = args.Current as HandheldLightComponent.HandheldLightComponentState;
			if (state == null)
			{
				return;
			}
			component.Level = state.Charge;
			this.SetActivated(uid, state.Activated, component, false);
		}

		// Token: 0x06000A3B RID: 2619 RVA: 0x000220C4 File Offset: 0x000202C4
		[NullableContext(2)]
		public void SetActivated(EntityUid uid, bool activated, HandheldLightComponent component = null, bool makeNoise = true)
		{
			if (!base.Resolve<HandheldLightComponent>(uid, ref component, true))
			{
				return;
			}
			if (component.Activated == activated)
			{
				return;
			}
			component.Activated = activated;
			if (makeNoise)
			{
				SoundSpecifier sound = component.Activated ? component.TurnOnSound : component.TurnOffSound;
				this._audio.PlayPvs(sound, component.Owner, null);
			}
			base.Dirty(component, null);
			this.UpdateVisuals(uid, component, null);
		}

		// Token: 0x06000A3C RID: 2620 RVA: 0x00022138 File Offset: 0x00020338
		[NullableContext(2)]
		public void UpdateVisuals(EntityUid uid, HandheldLightComponent component = null, AppearanceComponent appearance = null)
		{
			if (!base.Resolve<HandheldLightComponent, AppearanceComponent>(uid, ref component, ref appearance, false))
			{
				return;
			}
			if (component.AddPrefix)
			{
				string prefix = component.Activated ? "on" : "off";
				this._itemSys.SetHeldPrefix(uid, prefix, null);
				this._clothingSys.SetEquippedPrefix(uid, prefix, null);
			}
			if (component.ToggleAction != null)
			{
				this._actionSystem.SetToggled(component.ToggleAction, component.Activated);
			}
			this._appearance.SetData(uid, ToggleableLightVisuals.Enabled, component.Activated, appearance);
		}

		// Token: 0x040009F3 RID: 2547
		[Dependency]
		private readonly SharedItemSystem _itemSys;

		// Token: 0x040009F4 RID: 2548
		[Dependency]
		private readonly ClothingSystem _clothingSys;

		// Token: 0x040009F5 RID: 2549
		[Dependency]
		private readonly SharedActionsSystem _actionSystem;

		// Token: 0x040009F6 RID: 2550
		[Dependency]
		private readonly SharedAppearanceSystem _appearance;

		// Token: 0x040009F7 RID: 2551
		[Dependency]
		private readonly SharedAudioSystem _audio;
	}
}
