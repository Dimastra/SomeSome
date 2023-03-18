using System;
using System.Linq;
using System.Runtime.CompilerServices;
using Content.Shared.Projectiles;
using Content.Shared.Pulling;
using Content.Shared.Pulling.Components;
using Content.Shared.Teleportation.Components;
using Robust.Shared.Audio;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.IoC;
using Robust.Shared.Map;
using Robust.Shared.Maths;
using Robust.Shared.Network;
using Robust.Shared.Physics.Dynamics;
using Robust.Shared.Physics.Events;
using Robust.Shared.Random;

namespace Content.Shared.Teleportation.Systems
{
	// Token: 0x020000E0 RID: 224
	[NullableContext(1)]
	[Nullable(0)]
	public abstract class SharedPortalSystem : EntitySystem
	{
		// Token: 0x06000278 RID: 632 RVA: 0x0000BDA8 File Offset: 0x00009FA8
		public override void Initialize()
		{
			base.SubscribeLocalEvent<PortalComponent, StartCollideEvent>(new ComponentEventRefHandler<PortalComponent, StartCollideEvent>(this.OnCollide), null, null);
			base.SubscribeLocalEvent<PortalComponent, EndCollideEvent>(new ComponentEventRefHandler<PortalComponent, EndCollideEvent>(this.OnEndCollide), null, null);
			base.SubscribeLocalEvent<PortalTimeoutComponent, ComponentGetState>(new ComponentEventRefHandler<PortalTimeoutComponent, ComponentGetState>(this.OnGetState), null, null);
			base.SubscribeLocalEvent<PortalTimeoutComponent, ComponentHandleState>(new ComponentEventRefHandler<PortalTimeoutComponent, ComponentHandleState>(this.OnHandleState), null, null);
		}

		// Token: 0x06000279 RID: 633 RVA: 0x0000BE05 File Offset: 0x0000A005
		private void OnGetState(EntityUid uid, PortalTimeoutComponent component, ref ComponentGetState args)
		{
			args.State = new PortalTimeoutComponentState(component.EnteredPortal);
		}

		// Token: 0x0600027A RID: 634 RVA: 0x0000BE18 File Offset: 0x0000A018
		private void OnHandleState(EntityUid uid, PortalTimeoutComponent component, ref ComponentHandleState args)
		{
			PortalTimeoutComponentState state = args.Current as PortalTimeoutComponentState;
			if (state != null)
			{
				component.EnteredPortal = state.EnteredPortal;
			}
		}

		// Token: 0x0600027B RID: 635 RVA: 0x0000BE40 File Offset: 0x0000A040
		private bool ShouldCollide(Fixture our, Fixture other)
		{
			return our.ID == "portalFixture" && (other.Hard || other.ID == "projectile");
		}

		// Token: 0x0600027C RID: 636 RVA: 0x0000BE70 File Offset: 0x0000A070
		private void OnCollide(EntityUid uid, PortalComponent component, ref StartCollideEvent args)
		{
			if (!this.ShouldCollide(args.OurFixture, args.OtherFixture))
			{
				return;
			}
			EntityUid subject = args.OtherFixture.Body.Owner;
			if (base.Transform(subject).Anchored)
			{
				return;
			}
			SharedPullableComponent pullable;
			if (base.TryComp<SharedPullableComponent>(subject, ref pullable) && pullable.BeingPulled)
			{
				this._pulling.TryStopPull(pullable, null);
			}
			SharedPullerComponent pulling;
			SharedPullableComponent subjectPulling;
			if (base.TryComp<SharedPullerComponent>(subject, ref pulling) && pulling.Pulling != null && base.TryComp<SharedPullableComponent>(pulling.Pulling.Value, ref subjectPulling))
			{
				this._pulling.TryStopPull(subjectPulling, null);
			}
			if (base.HasComp<PortalTimeoutComponent>(subject))
			{
				return;
			}
			LinkedEntityComponent link;
			if (base.TryComp<LinkedEntityComponent>(uid, ref link))
			{
				if (!link.LinkedEntities.Any<EntityUid>())
				{
					return;
				}
				if (this._netMan.IsClient)
				{
					EntityUid first = link.LinkedEntities.First<EntityUid>();
					bool exists = base.Exists(first);
					if (link.LinkedEntities.Count != 1 || !exists || (exists && base.Transform(first).MapID == MapId.Nullspace))
					{
						return;
					}
				}
				EntityUid target = RandomExtensions.Pick<EntityUid>(this._random, link.LinkedEntities);
				if (base.HasComp<PortalComponent>(target))
				{
					PortalTimeoutComponent timeout = base.EnsureComp<PortalTimeoutComponent>(subject);
					timeout.EnteredPortal = new EntityUid?(uid);
					base.Dirty(timeout, null);
				}
				this.TeleportEntity(uid, subject, base.Transform(target).Coordinates, new EntityUid?(target), null);
				return;
			}
			else
			{
				if (this._netMan.IsClient)
				{
					return;
				}
				Vector2 randVector = this._random.NextVector2(component.MaxRandomRadius);
				EntityCoordinates newCoords = base.Transform(uid).Coordinates.Offset(randVector);
				this.TeleportEntity(uid, subject, newCoords, null, null);
				return;
			}
		}

		// Token: 0x0600027D RID: 637 RVA: 0x0000C050 File Offset: 0x0000A250
		private void OnEndCollide(EntityUid uid, PortalComponent component, ref EndCollideEvent args)
		{
			if (!this.ShouldCollide(args.OurFixture, args.OtherFixture))
			{
				return;
			}
			EntityUid subject = args.OtherFixture.Body.Owner;
			PortalTimeoutComponent timeout;
			if (base.TryComp<PortalTimeoutComponent>(subject, ref timeout))
			{
				EntityUid? enteredPortal = timeout.EnteredPortal;
				if (enteredPortal == null || (enteredPortal != null && enteredPortal.GetValueOrDefault() != uid))
				{
					base.RemComp<PortalTimeoutComponent>(subject);
				}
			}
		}

		// Token: 0x0600027E RID: 638 RVA: 0x0000C0C8 File Offset: 0x0000A2C8
		[NullableContext(2)]
		private void TeleportEntity(EntityUid portal, EntityUid subject, EntityCoordinates target, EntityUid? targetEntity = null, PortalComponent portalComponent = null)
		{
			if (!base.Resolve<PortalComponent>(portal, ref portalComponent, true))
			{
				return;
			}
			PortalComponent portalComponent2 = base.CompOrNull<PortalComponent>(targetEntity);
			SoundSpecifier arrivalSound = ((portalComponent2 != null) ? portalComponent2.ArrivalSound : null) ?? portalComponent.ArrivalSound;
			SoundSpecifier departureSound = portalComponent.DepartureSound;
			ProjectileComponent projectile;
			if (base.TryComp<ProjectileComponent>(subject, ref projectile))
			{
				projectile.IgnoreShooter = false;
			}
			this.LogTeleport(portal, subject, base.Transform(subject).Coordinates, target);
			base.Transform(subject).Coordinates = target;
			this._audio.PlayPredicted(departureSound, portal, new EntityUid?(subject), null);
			this._audio.PlayPredicted(arrivalSound, subject, new EntityUid?(subject), null);
		}

		// Token: 0x0600027F RID: 639 RVA: 0x0000C178 File Offset: 0x0000A378
		protected virtual void LogTeleport(EntityUid portal, EntityUid subject, EntityCoordinates source, EntityCoordinates target)
		{
		}

		// Token: 0x040002D7 RID: 727
		[Dependency]
		private readonly IRobustRandom _random;

		// Token: 0x040002D8 RID: 728
		[Dependency]
		private readonly INetManager _netMan;

		// Token: 0x040002D9 RID: 729
		[Dependency]
		private readonly SharedAudioSystem _audio;

		// Token: 0x040002DA RID: 730
		[Dependency]
		private readonly SharedPullingSystem _pulling;

		// Token: 0x040002DB RID: 731
		private const string PortalFixture = "portalFixture";

		// Token: 0x040002DC RID: 732
		private const string ProjectileFixture = "projectile";
	}
}
