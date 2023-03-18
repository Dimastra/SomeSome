using System;
using System.Runtime.CompilerServices;
using Content.Client.Gravity;
using Content.Client.Pointing.Components;
using Content.Shared.Mobs.Systems;
using Content.Shared.Pointing;
using Content.Shared.Verbs;
using Robust.Client.GameObjects;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Localization;
using Robust.Shared.Utility;

namespace Content.Client.Pointing
{
	// Token: 0x020001B1 RID: 433
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class PointingSystem : SharedPointingSystem
	{
		// Token: 0x06000B42 RID: 2882 RVA: 0x00041854 File Offset: 0x0003FA54
		public override void Initialize()
		{
			base.Initialize();
			base.SubscribeLocalEvent<GetVerbsEvent<Verb>>(new EntityEventHandler<GetVerbsEvent<Verb>>(this.AddPointingVerb), null, null);
			base.SubscribeLocalEvent<PointingArrowComponent, ComponentStartup>(new ComponentEventHandler<PointingArrowComponent, ComponentStartup>(this.OnArrowStartup), null, null);
			base.SubscribeLocalEvent<PointingArrowComponent, AnimationCompletedEvent>(new ComponentEventHandler<PointingArrowComponent, AnimationCompletedEvent>(this.OnArrowAnimation), null, null);
			base.SubscribeLocalEvent<RoguePointingArrowComponent, ComponentStartup>(new ComponentEventHandler<RoguePointingArrowComponent, ComponentStartup>(this.OnRogueArrowStartup), null, null);
		}

		// Token: 0x06000B43 RID: 2883 RVA: 0x000418B7 File Offset: 0x0003FAB7
		private void OnArrowAnimation(EntityUid uid, PointingArrowComponent component, AnimationCompletedEvent args)
		{
			this._floatingSystem.FloatAnimation(uid, component.Offset, component.AnimationKey, component.AnimationTime, false);
		}

		// Token: 0x06000B44 RID: 2884 RVA: 0x000418D8 File Offset: 0x0003FAD8
		private void AddPointingVerb(GetVerbsEvent<Verb> args)
		{
			if (args.Target.IsClientSide())
			{
				return;
			}
			if (base.HasComp<PointingArrowComponent>(args.Target))
			{
				return;
			}
			if (this._mobState.IsIncapacitated(args.User, null))
			{
				return;
			}
			Verb item = new Verb
			{
				Text = Loc.GetString("pointing-verb-get-data-text"),
				Icon = new SpriteSpecifier.Texture(new ResourcePath("/Textures/Interface/VerbIcons/point.svg.192dpi.png", "/")),
				ClientExclusive = true,
				Act = delegate()
				{
					this.RaiseNetworkEvent(new PointingAttemptEvent(args.Target));
				}
			};
			args.Verbs.Add(item);
		}

		// Token: 0x06000B45 RID: 2885 RVA: 0x00041998 File Offset: 0x0003FB98
		private void OnArrowStartup(EntityUid uid, PointingArrowComponent component, ComponentStartup args)
		{
			SpriteComponent spriteComponent;
			if (base.TryComp<SpriteComponent>(uid, ref spriteComponent))
			{
				spriteComponent.DrawDepth = 9;
			}
			this._floatingSystem.FloatAnimation(uid, component.Offset, component.AnimationKey, component.AnimationTime, false);
		}

		// Token: 0x06000B46 RID: 2886 RVA: 0x000419D8 File Offset: 0x0003FBD8
		private void OnRogueArrowStartup(EntityUid uid, RoguePointingArrowComponent arrow, ComponentStartup args)
		{
			SpriteComponent spriteComponent;
			if (base.TryComp<SpriteComponent>(uid, ref spriteComponent))
			{
				spriteComponent.DrawDepth = 9;
				spriteComponent.NoRotation = false;
			}
		}

		// Token: 0x04000580 RID: 1408
		[Dependency]
		private readonly MobStateSystem _mobState;

		// Token: 0x04000581 RID: 1409
		[Dependency]
		private readonly FloatingVisualizerSystem _floatingSystem;
	}
}
