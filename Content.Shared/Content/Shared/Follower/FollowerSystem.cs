using System;
using System.Runtime.CompilerServices;
using Content.Shared.Database;
using Content.Shared.Follower.Components;
using Content.Shared.Ghost;
using Content.Shared.Movement.Events;
using Content.Shared.Verbs;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Localization;
using Robust.Shared.Map;
using Robust.Shared.Maths;
using Robust.Shared.Utility;

namespace Content.Shared.Follower
{
	// Token: 0x02000474 RID: 1140
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class FollowerSystem : EntitySystem
	{
		// Token: 0x06000DCE RID: 3534 RVA: 0x0002D0E4 File Offset: 0x0002B2E4
		public override void Initialize()
		{
			base.Initialize();
			base.SubscribeLocalEvent<GetVerbsEvent<AlternativeVerb>>(new EntityEventHandler<GetVerbsEvent<AlternativeVerb>>(this.OnGetAlternativeVerbs), null, null);
			base.SubscribeLocalEvent<FollowerComponent, MoveInputEvent>(new ComponentEventRefHandler<FollowerComponent, MoveInputEvent>(this.OnFollowerMove), null, null);
			base.SubscribeLocalEvent<FollowedComponent, EntityTerminatingEvent>(new ComponentEventRefHandler<FollowedComponent, EntityTerminatingEvent>(this.OnFollowedTerminating), null, null);
		}

		// Token: 0x06000DCF RID: 3535 RVA: 0x0002D134 File Offset: 0x0002B334
		private void OnGetAlternativeVerbs(GetVerbsEvent<AlternativeVerb> ev)
		{
			if (!base.HasComp<SharedGhostComponent>(ev.User))
			{
				return;
			}
			if (ev.User == ev.Target || ev.Target.IsClientSide())
			{
				return;
			}
			AlternativeVerb verb = new AlternativeVerb
			{
				Priority = 10,
				Act = delegate()
				{
					this.StartFollowingEntity(ev.User, ev.Target);
				},
				Impact = LogImpact.Low,
				Text = Loc.GetString("verb-follow-text"),
				Icon = new SpriteSpecifier.Texture(new ResourcePath("/Textures/Interface/VerbIcons/open.svg.192dpi.png", "/"))
			};
			ev.Verbs.Add(verb);
		}

		// Token: 0x06000DD0 RID: 3536 RVA: 0x0002D1FC File Offset: 0x0002B3FC
		private void OnFollowerMove(EntityUid uid, FollowerComponent component, ref MoveInputEvent args)
		{
			this.StopFollowingEntity(uid, component.Following, null);
		}

		// Token: 0x06000DD1 RID: 3537 RVA: 0x0002D20C File Offset: 0x0002B40C
		private void OnFollowedTerminating(EntityUid uid, FollowedComponent component, ref EntityTerminatingEvent args)
		{
			this.StopAllFollowers(uid, component);
		}

		// Token: 0x06000DD2 RID: 3538 RVA: 0x0002D218 File Offset: 0x0002B418
		public void StartFollowingEntity(EntityUid follower, EntityUid entity)
		{
			if (base.Transform(entity).ParentUid == follower)
			{
				return;
			}
			base.EnsureComp<FollowerComponent>(follower).Following = entity;
			base.EnsureComp<FollowedComponent>(entity).Following.Add(follower);
			TransformComponent xform = base.Transform(follower);
			this._transform.SetParent(follower, xform, entity, null);
			xform.LocalPosition = Vector2.Zero;
			xform.LocalRotation = Angle.Zero;
			base.EnsureComp<OrbitVisualsComponent>(follower);
			StartedFollowingEntityEvent followerEv = new StartedFollowingEntityEvent(entity, follower);
			EntityStartedFollowingEvent entityEv = new EntityStartedFollowingEvent(entity, follower);
			base.RaiseLocalEvent<StartedFollowingEntityEvent>(follower, followerEv, true);
			base.RaiseLocalEvent<EntityStartedFollowingEvent>(entity, entityEv, false);
		}

		// Token: 0x06000DD3 RID: 3539 RVA: 0x0002D2B4 File Offset: 0x0002B4B4
		[NullableContext(2)]
		public void StopFollowingEntity(EntityUid uid, EntityUid target, FollowedComponent followed = null)
		{
			if (!base.Resolve<FollowedComponent>(target, ref followed, false))
			{
				return;
			}
			if (!base.HasComp<FollowerComponent>(uid))
			{
				return;
			}
			followed.Following.Remove(uid);
			if (followed.Following.Count == 0)
			{
				base.RemComp<FollowedComponent>(target);
			}
			base.RemComp<FollowerComponent>(uid);
			TransformComponent transformComponent = base.Transform(uid);
			transformComponent.AttachToGridOrMap();
			if (transformComponent.MapID == MapId.Nullspace)
			{
				base.Del(uid);
				return;
			}
			base.RemComp<OrbitVisualsComponent>(uid);
			StoppedFollowingEntityEvent uidEv = new StoppedFollowingEntityEvent(target, uid);
			EntityStoppedFollowingEvent targetEv = new EntityStoppedFollowingEvent(target, uid);
			base.RaiseLocalEvent<StoppedFollowingEntityEvent>(uid, uidEv, true);
			base.RaiseLocalEvent<EntityStoppedFollowingEvent>(target, targetEv, false);
		}

		// Token: 0x06000DD4 RID: 3540 RVA: 0x0002D354 File Offset: 0x0002B554
		[NullableContext(2)]
		public void StopAllFollowers(EntityUid uid, FollowedComponent followed = null)
		{
			if (!base.Resolve<FollowedComponent>(uid, ref followed, true))
			{
				return;
			}
			foreach (EntityUid player in followed.Following)
			{
				this.StopFollowingEntity(player, uid, followed);
			}
		}

		// Token: 0x04000D26 RID: 3366
		[Dependency]
		private readonly SharedTransformSystem _transform;
	}
}
