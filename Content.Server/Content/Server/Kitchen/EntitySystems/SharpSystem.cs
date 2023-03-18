using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading;
using Content.Server.Body.Systems;
using Content.Server.DoAfter;
using Content.Server.Kitchen.Components;
using Content.Shared.Body.Components;
using Content.Shared.Destructible;
using Content.Shared.DoAfter;
using Content.Shared.Interaction;
using Content.Shared.Mobs.Components;
using Content.Shared.Mobs.Systems;
using Content.Shared.Nutrition.Components;
using Content.Shared.Popups;
using Content.Shared.Storage;
using Content.Shared.Verbs;
using Robust.Server.Containers;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Localization;
using Robust.Shared.Map;
using Robust.Shared.Random;
using Robust.Shared.Utility;

namespace Content.Server.Kitchen.EntitySystems
{
	// Token: 0x02000431 RID: 1073
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class SharpSystem : EntitySystem
	{
		// Token: 0x060015D9 RID: 5593 RVA: 0x00073A78 File Offset: 0x00071C78
		public override void Initialize()
		{
			base.Initialize();
			base.SubscribeLocalEvent<SharpComponent, AfterInteractEvent>(new ComponentEventHandler<SharpComponent, AfterInteractEvent>(this.OnAfterInteract), null, null);
			base.SubscribeLocalEvent<SharpComponent, DoAfterEvent>(new ComponentEventHandler<SharpComponent, DoAfterEvent>(this.OnDoAfter), null, null);
			base.SubscribeLocalEvent<ButcherableComponent, GetVerbsEvent<InteractionVerb>>(new ComponentEventHandler<ButcherableComponent, GetVerbsEvent<InteractionVerb>>(this.OnGetInteractionVerbs), null, null);
		}

		// Token: 0x060015DA RID: 5594 RVA: 0x00073AC8 File Offset: 0x00071CC8
		private void OnAfterInteract(EntityUid uid, SharpComponent component, AfterInteractEvent args)
		{
			if (args.Target == null || !args.CanReach)
			{
				return;
			}
			this.TryStartButcherDoafter(uid, args.Target.Value, args.User);
		}

		// Token: 0x060015DB RID: 5595 RVA: 0x00073B0C File Offset: 0x00071D0C
		private void TryStartButcherDoafter(EntityUid knife, EntityUid target, EntityUid user)
		{
			ButcherableComponent butcher;
			if (!base.TryComp<ButcherableComponent>(target, ref butcher))
			{
				return;
			}
			SharpComponent sharp;
			if (!base.TryComp<SharpComponent>(knife, ref sharp))
			{
				return;
			}
			if (butcher.Type != ButcheringType.Knife)
			{
				return;
			}
			MobStateComponent mobState;
			if (base.TryComp<MobStateComponent>(target, ref mobState) && !this._mobStateSystem.IsDead(target, mobState))
			{
				return;
			}
			if (!sharp.Butchering.Add(target))
			{
				return;
			}
			float delay = sharp.ButcherDelayModifier * butcher.ButcherDelay;
			EntityUid? target2 = new EntityUid?(target);
			EntityUid? used = new EntityUid?(knife);
			DoAfterEventArgs doAfter = new DoAfterEventArgs(user, delay, default(CancellationToken), target2, used)
			{
				BreakOnTargetMove = true,
				BreakOnUserMove = true,
				BreakOnDamage = true,
				BreakOnStun = true,
				NeedHand = true
			};
			this._doAfterSystem.DoAfter(doAfter);
		}

		// Token: 0x060015DC RID: 5596 RVA: 0x00073BC8 File Offset: 0x00071DC8
		private void OnDoAfter(EntityUid uid, SharpComponent component, DoAfterEvent args)
		{
			ButcherableComponent butcher;
			if (args.Handled || args.Cancelled || !base.TryComp<ButcherableComponent>(args.Args.Target, ref butcher))
			{
				return;
			}
			component.Butchering.Remove(args.Args.Target.Value);
			if (this._containerSystem.IsEntityInContainer(args.Args.Target.Value, null))
			{
				args.Handled = true;
				return;
			}
			List<string> spawns = EntitySpawnCollection.GetSpawns(butcher.SpawnedEntities, this._robustRandom);
			MapCoordinates coords = base.Transform(args.Args.Target.Value).MapPosition;
			EntityUid popupEnt = default(EntityUid);
			foreach (string proto in spawns)
			{
				popupEnt = base.Spawn(proto, coords.Offset(this._robustRandom.NextVector2(0.25f)));
			}
			BodyComponent body;
			bool hasBody = base.TryComp<BodyComponent>(args.Args.Target.Value, ref body);
			PopupType popupType = PopupType.Small;
			if (hasBody)
			{
				popupType = PopupType.LargeCaution;
			}
			this._popupSystem.PopupEntity(Loc.GetString("butcherable-knife-butchered-success", new ValueTuple<string, object>[]
			{
				new ValueTuple<string, object>("target", args.Args.Target.Value),
				new ValueTuple<string, object>("knife", uid)
			}), popupEnt, args.Args.User, popupType);
			if (hasBody)
			{
				this._bodySystem.GibBody(new EntityUid?(args.Args.Target.Value), false, body, false);
			}
			this._destructibleSystem.DestroyEntity(args.Args.Target.Value);
			args.Handled = true;
		}

		// Token: 0x060015DD RID: 5597 RVA: 0x00073DA0 File Offset: 0x00071FA0
		private void OnGetInteractionVerbs(EntityUid uid, ButcherableComponent component, GetVerbsEvent<InteractionVerb> args)
		{
			if (component.Type != ButcheringType.Knife || args.Hands == null)
			{
				return;
			}
			bool disabled = false;
			string message = null;
			MobStateComponent state;
			if (args.Using == null || !base.HasComp<SharpComponent>(args.Using))
			{
				disabled = true;
				message = Loc.GetString("butcherable-need-knife", new ValueTuple<string, object>[]
				{
					new ValueTuple<string, object>("target", uid)
				});
			}
			else if (this._containerSystem.IsEntityInContainer(uid, null))
			{
				message = Loc.GetString("butcherable-not-in-container", new ValueTuple<string, object>[]
				{
					new ValueTuple<string, object>("target", uid)
				});
				disabled = true;
			}
			else if (base.TryComp<MobStateComponent>(uid, ref state) && !this._mobStateSystem.IsDead(uid, state))
			{
				disabled = true;
				message = Loc.GetString("butcherable-mob-isnt-dead");
			}
			InteractionVerb verb = new InteractionVerb
			{
				Act = delegate()
				{
					if (!disabled)
					{
						this.TryStartButcherDoafter(args.Using.Value, args.Target, args.User);
					}
				},
				Message = message,
				Disabled = disabled,
				Icon = new SpriteSpecifier.Texture(new ResourcePath("/Textures/Interface/VerbIcons/cutlery.svg.192dpi.png", "/")),
				Text = Loc.GetString("butcherable-verb-name")
			};
			args.Verbs.Add(verb);
		}

		// Token: 0x04000D91 RID: 3473
		[Dependency]
		private readonly BodySystem _bodySystem;

		// Token: 0x04000D92 RID: 3474
		[Dependency]
		private readonly SharedDestructibleSystem _destructibleSystem;

		// Token: 0x04000D93 RID: 3475
		[Dependency]
		private readonly DoAfterSystem _doAfterSystem;

		// Token: 0x04000D94 RID: 3476
		[Dependency]
		private readonly SharedPopupSystem _popupSystem;

		// Token: 0x04000D95 RID: 3477
		[Dependency]
		private readonly ContainerSystem _containerSystem;

		// Token: 0x04000D96 RID: 3478
		[Dependency]
		private readonly MobStateSystem _mobStateSystem;

		// Token: 0x04000D97 RID: 3479
		[Dependency]
		private readonly IRobustRandom _robustRandom;
	}
}
