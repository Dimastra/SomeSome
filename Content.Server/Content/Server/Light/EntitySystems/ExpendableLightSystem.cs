using System;
using System.Runtime.CompilerServices;
using Content.Server.Light.Components;
using Content.Shared.Clothing.Components;
using Content.Shared.Clothing.EntitySystems;
using Content.Shared.Interaction.Events;
using Content.Shared.Item;
using Content.Shared.Light.Component;
using Content.Shared.Tag;
using Content.Shared.Temperature;
using Content.Shared.Verbs;
using Robust.Server.GameObjects;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Localization;
using Robust.Shared.Utility;

namespace Content.Server.Light.EntitySystems
{
	// Token: 0x0200040B RID: 1035
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class ExpendableLightSystem : EntitySystem
	{
		// Token: 0x060014F0 RID: 5360 RVA: 0x0006DAFC File Offset: 0x0006BCFC
		public override void Initialize()
		{
			base.Initialize();
			base.SubscribeLocalEvent<ExpendableLightComponent, ComponentInit>(new ComponentEventHandler<ExpendableLightComponent, ComponentInit>(this.OnExpLightInit), null, null);
			base.SubscribeLocalEvent<ExpendableLightComponent, UseInHandEvent>(new ComponentEventHandler<ExpendableLightComponent, UseInHandEvent>(this.OnExpLightUse), null, null);
			base.SubscribeLocalEvent<ExpendableLightComponent, GetVerbsEvent<ActivationVerb>>(new ComponentEventHandler<ExpendableLightComponent, GetVerbsEvent<ActivationVerb>>(this.AddIgniteVerb), null, null);
		}

		// Token: 0x060014F1 RID: 5361 RVA: 0x0006DB4C File Offset: 0x0006BD4C
		public override void Update(float frameTime)
		{
			foreach (ExpendableLightComponent light in this.EntityManager.EntityQuery<ExpendableLightComponent>(false))
			{
				this.UpdateLight(light, frameTime);
			}
		}

		// Token: 0x060014F2 RID: 5362 RVA: 0x0006DBA0 File Offset: 0x0006BDA0
		private void UpdateLight(ExpendableLightComponent component, float frameTime)
		{
			if (!component.Activated)
			{
				return;
			}
			component.StateExpiryTime -= frameTime;
			if (component.StateExpiryTime <= 0f)
			{
				ExpendableLightState currentState = component.CurrentState;
				if (currentState == ExpendableLightState.Lit)
				{
					component.CurrentState = ExpendableLightState.Fading;
					component.StateExpiryTime = component.FadeOutDuration;
					this.UpdateVisualizer(component, null);
					return;
				}
				if (currentState != ExpendableLightState.Fading)
				{
				}
				component.CurrentState = ExpendableLightState.Dead;
				MetaDataComponent metaDataComponent = base.MetaData(component.Owner);
				metaDataComponent.EntityName = Loc.GetString(component.SpentName);
				metaDataComponent.EntityDescription = Loc.GetString(component.SpentDesc);
				this._tagSystem.AddTag(component.Owner, "Trash");
				this.UpdateSounds(component);
				this.UpdateVisualizer(component, null);
				ItemComponent item;
				if (base.TryComp<ItemComponent>(component.Owner, ref item))
				{
					this._item.SetHeldPrefix(component.Owner, "unlit", item);
				}
			}
		}

		// Token: 0x060014F3 RID: 5363 RVA: 0x0006DC84 File Offset: 0x0006BE84
		public bool TryActivate(ExpendableLightComponent component)
		{
			if (!component.Activated && component.CurrentState == ExpendableLightState.BrandNew)
			{
				ItemComponent item;
				if (base.TryComp<ItemComponent>(component.Owner, ref item))
				{
					this._item.SetHeldPrefix(component.Owner, "lit", item);
				}
				component.CurrentState = ExpendableLightState.Lit;
				component.StateExpiryTime = component.GlowDuration;
				this.UpdateSounds(component);
				this.UpdateVisualizer(component, null);
				return true;
			}
			return false;
		}

		// Token: 0x060014F4 RID: 5364 RVA: 0x0006DCF0 File Offset: 0x0006BEF0
		private void UpdateVisualizer(ExpendableLightComponent component, [Nullable(2)] AppearanceComponent appearance = null)
		{
			if (!base.Resolve<AppearanceComponent>(component.Owner, ref appearance, false))
			{
				return;
			}
			this._appearance.SetData(appearance.Owner, ExpendableLightVisuals.State, component.CurrentState, appearance);
			switch (component.CurrentState)
			{
			case ExpendableLightState.Lit:
				this._appearance.SetData(appearance.Owner, ExpendableLightVisuals.Behavior, component.TurnOnBehaviourID, appearance);
				return;
			case ExpendableLightState.Fading:
				this._appearance.SetData(appearance.Owner, ExpendableLightVisuals.Behavior, component.FadeOutBehaviourID, appearance);
				return;
			case ExpendableLightState.Dead:
			{
				this._appearance.SetData(appearance.Owner, ExpendableLightVisuals.Behavior, string.Empty, appearance);
				IsHotEvent isHotEvent = new IsHotEvent
				{
					IsHot = true
				};
				base.RaiseLocalEvent<IsHotEvent>(component.Owner, isHotEvent, false);
				return;
			}
			default:
				return;
			}
		}

		// Token: 0x060014F5 RID: 5365 RVA: 0x0006DDC4 File Offset: 0x0006BFC4
		private void UpdateSounds(ExpendableLightComponent component)
		{
			EntityUid uid = component.Owner;
			ExpendableLightState currentState = component.CurrentState;
			if (currentState != ExpendableLightState.Lit)
			{
				if (currentState != ExpendableLightState.Fading)
				{
					this._audio.PlayPvs(component.DieSound, uid, null);
				}
			}
			else
			{
				this._audio.PlayPvs(component.LitSound, uid, null);
			}
			ClothingComponent clothing;
			if (base.TryComp<ClothingComponent>(uid, ref clothing))
			{
				this._clothing.SetEquippedPrefix(uid, component.Activated ? "Activated" : string.Empty, clothing);
			}
		}

		// Token: 0x060014F6 RID: 5366 RVA: 0x0006DE50 File Offset: 0x0006C050
		private void OnExpLightInit(EntityUid uid, ExpendableLightComponent component, ComponentInit args)
		{
			ItemComponent item;
			if (base.TryComp<ItemComponent>(uid, ref item))
			{
				this._item.SetHeldPrefix(uid, "unlit", item);
			}
			component.CurrentState = ExpendableLightState.BrandNew;
			this.EntityManager.EnsureComponent<PointLightComponent>(uid);
		}

		// Token: 0x060014F7 RID: 5367 RVA: 0x0006DE90 File Offset: 0x0006C090
		private void OnExpLightUse(EntityUid uid, ExpendableLightComponent component, UseInHandEvent args)
		{
			if (args.Handled)
			{
				return;
			}
			IsHotEvent isHotEvent = new IsHotEvent
			{
				IsHot = true
			};
			base.RaiseLocalEvent<IsHotEvent>(uid, isHotEvent, false);
			if (this.TryActivate(component))
			{
				args.Handled = true;
			}
		}

		// Token: 0x060014F8 RID: 5368 RVA: 0x0006DECC File Offset: 0x0006C0CC
		private void AddIgniteVerb(EntityUid uid, ExpendableLightComponent component, GetVerbsEvent<ActivationVerb> args)
		{
			if (!args.CanAccess || !args.CanInteract)
			{
				return;
			}
			if (component.CurrentState != ExpendableLightState.BrandNew)
			{
				return;
			}
			ActivationVerb verb = new ActivationVerb
			{
				Text = Loc.GetString("expendable-light-start-verb"),
				Icon = new SpriteSpecifier.Texture(new ResourcePath("/Textures/Interface/VerbIcons/light.svg.192dpi.png", "/")),
				Act = delegate()
				{
					this.TryActivate(component);
				}
			};
			args.Verbs.Add(verb);
		}

		// Token: 0x04000D02 RID: 3330
		[Dependency]
		private readonly SharedItemSystem _item;

		// Token: 0x04000D03 RID: 3331
		[Dependency]
		private readonly ClothingSystem _clothing;

		// Token: 0x04000D04 RID: 3332
		[Dependency]
		private readonly TagSystem _tagSystem;

		// Token: 0x04000D05 RID: 3333
		[Dependency]
		private readonly SharedAudioSystem _audio;

		// Token: 0x04000D06 RID: 3334
		[Dependency]
		private readonly SharedAppearanceSystem _appearance;
	}
}
