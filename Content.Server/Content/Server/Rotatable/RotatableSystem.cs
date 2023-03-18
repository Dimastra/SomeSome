using System;
using System.Runtime.CompilerServices;
using Content.Server.Popups;
using Content.Shared.Popups;
using Content.Shared.Rotatable;
using Content.Shared.Verbs;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Localization;
using Robust.Shared.Maths;
using Robust.Shared.Physics.Components;
using Robust.Shared.Utility;

namespace Content.Server.Rotatable
{
	// Token: 0x02000227 RID: 551
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class RotatableSystem : EntitySystem
	{
		// Token: 0x06000B00 RID: 2816 RVA: 0x00039C7D File Offset: 0x00037E7D
		public override void Initialize()
		{
			base.SubscribeLocalEvent<FlippableComponent, GetVerbsEvent<Verb>>(new ComponentEventHandler<FlippableComponent, GetVerbsEvent<Verb>>(this.AddFlipVerb), null, null);
			base.SubscribeLocalEvent<RotatableComponent, GetVerbsEvent<Verb>>(new ComponentEventHandler<RotatableComponent, GetVerbsEvent<Verb>>(this.AddRotateVerbs), null, null);
		}

		// Token: 0x06000B01 RID: 2817 RVA: 0x00039CA8 File Offset: 0x00037EA8
		private void AddFlipVerb(EntityUid uid, FlippableComponent component, GetVerbsEvent<Verb> args)
		{
			if (!args.CanAccess || !args.CanInteract)
			{
				return;
			}
			Verb verb = new Verb
			{
				Act = delegate()
				{
					this.TryFlip(uid, component, args.User);
				},
				Text = Loc.GetString("flippable-verb-get-data-text"),
				DoContactInteraction = new bool?(true)
			};
			args.Verbs.Add(verb);
		}

		// Token: 0x06000B02 RID: 2818 RVA: 0x00039D38 File Offset: 0x00037F38
		private void AddRotateVerbs(EntityUid uid, RotatableComponent component, GetVerbsEvent<Verb> args)
		{
			if (!args.CanAccess || !args.CanInteract || base.Transform(uid).NoLocalRotation)
			{
				return;
			}
			PhysicsComponent physics;
			if (!component.RotateWhileAnchored && this.EntityManager.TryGetComponent<PhysicsComponent>(uid, ref physics) && physics.BodyType == 4)
			{
				return;
			}
			Verb resetRotation = new Verb
			{
				DoContactInteraction = new bool?(true),
				Act = delegate()
				{
					this.EntityManager.GetComponent<TransformComponent>(uid).LocalRotation = Angle.Zero;
				},
				Category = VerbCategory.Rotate,
				Icon = new SpriteSpecifier.Texture(new ResourcePath("/Textures/Interface/VerbIcons/refresh.svg.192dpi.png", "/")),
				Text = "Reset",
				Priority = -2,
				CloseMenu = new bool?(false)
			};
			args.Verbs.Add(resetRotation);
			Verb rotateCW = new Verb
			{
				Act = delegate()
				{
					this.EntityManager.GetComponent<TransformComponent>(uid).LocalRotation -= component.Increment;
				},
				Category = VerbCategory.Rotate,
				Icon = new SpriteSpecifier.Texture(new ResourcePath("/Textures/Interface/VerbIcons/rotate_cw.svg.192dpi.png", "/")),
				Priority = -1,
				CloseMenu = new bool?(false)
			};
			args.Verbs.Add(rotateCW);
			Verb rotateCCW = new Verb
			{
				Act = delegate()
				{
					this.EntityManager.GetComponent<TransformComponent>(uid).LocalRotation += component.Increment;
				},
				Category = VerbCategory.Rotate,
				Icon = new SpriteSpecifier.Texture(new ResourcePath("/Textures/Interface/VerbIcons/rotate_ccw.svg.192dpi.png", "/")),
				Priority = 0,
				CloseMenu = new bool?(false)
			};
			args.Verbs.Add(rotateCCW);
		}

		// Token: 0x06000B03 RID: 2819 RVA: 0x00039EE4 File Offset: 0x000380E4
		public void TryFlip(EntityUid uid, FlippableComponent component, EntityUid user)
		{
			PhysicsComponent physics;
			if (this.EntityManager.TryGetComponent<PhysicsComponent>(uid, ref physics) && physics.BodyType == 4)
			{
				this._popup.PopupEntity(Loc.GetString("flippable-component-try-flip-is-stuck"), uid, user, PopupType.Small);
				return;
			}
			TransformComponent oldTransform = this.EntityManager.GetComponent<TransformComponent>(uid);
			EntityUid entity = this.EntityManager.SpawnEntity(component.MirrorEntity, oldTransform.Coordinates);
			TransformComponent component2 = this.EntityManager.GetComponent<TransformComponent>(entity);
			component2.LocalRotation = oldTransform.LocalRotation;
			component2.Anchored = false;
			this.EntityManager.DeleteEntity(uid);
		}

		// Token: 0x040006C7 RID: 1735
		[Dependency]
		private readonly PopupSystem _popup;
	}
}
