using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Content.Shared.Hands.Components;
using Content.Shared.Rotation;
using Robust.Shared.Audio;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.IoC;
using Robust.Shared.Physics;
using Robust.Shared.Physics.Dynamics;
using Robust.Shared.Physics.Systems;
using Robust.Shared.Serialization;

namespace Content.Shared.Standing
{
	// Token: 0x02000164 RID: 356
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class StandingStateSystem : EntitySystem
	{
		// Token: 0x06000449 RID: 1097 RVA: 0x000112EA File Offset: 0x0000F4EA
		public override void Initialize()
		{
			base.SubscribeLocalEvent<StandingStateComponent, ComponentGetState>(new ComponentEventRefHandler<StandingStateComponent, ComponentGetState>(this.OnGetState), null, null);
			base.SubscribeLocalEvent<StandingStateComponent, ComponentHandleState>(new ComponentEventRefHandler<StandingStateComponent, ComponentHandleState>(this.OnHandleState), null, null);
		}

		// Token: 0x0600044A RID: 1098 RVA: 0x00011314 File Offset: 0x0000F514
		private void OnHandleState(EntityUid uid, StandingStateComponent component, ref ComponentHandleState args)
		{
			StandingStateSystem.StandingComponentState state = args.Current as StandingStateSystem.StandingComponentState;
			if (state == null)
			{
				return;
			}
			component.Standing = state.Standing;
			component.ChangedFixtures = new List<string>(state.ChangedFixtures);
		}

		// Token: 0x0600044B RID: 1099 RVA: 0x0001134E File Offset: 0x0000F54E
		private void OnGetState(EntityUid uid, StandingStateComponent component, ref ComponentGetState args)
		{
			args.State = new StandingStateSystem.StandingComponentState(component.Standing, component.ChangedFixtures);
		}

		// Token: 0x0600044C RID: 1100 RVA: 0x00011367 File Offset: 0x0000F567
		[NullableContext(2)]
		public bool IsDown(EntityUid uid, StandingStateComponent standingState = null)
		{
			return base.Resolve<StandingStateComponent>(uid, ref standingState, false) && !standingState.Standing;
		}

		// Token: 0x0600044D RID: 1101 RVA: 0x00011380 File Offset: 0x0000F580
		[NullableContext(2)]
		public bool Down(EntityUid uid, bool playSound = true, bool dropHeldItems = true, StandingStateComponent standingState = null, AppearanceComponent appearance = null, SharedHandsComponent hands = null)
		{
			if (!base.Resolve<StandingStateComponent>(uid, ref standingState, false))
			{
				return false;
			}
			base.Resolve<AppearanceComponent, SharedHandsComponent>(uid, ref appearance, ref hands, false);
			if (!standingState.Standing)
			{
				return true;
			}
			if (dropHeldItems && hands != null)
			{
				base.RaiseLocalEvent<DropHandItemsEvent>(uid, new DropHandItemsEvent(), false);
			}
			DownAttemptEvent msg = new DownAttemptEvent();
			base.RaiseLocalEvent<DownAttemptEvent>(uid, msg, false);
			if (msg.Cancelled)
			{
				return false;
			}
			standingState.Standing = false;
			base.Dirty(standingState, null);
			base.RaiseLocalEvent<DownedEvent>(uid, new DownedEvent(), false);
			this._appearance.SetData(uid, RotationVisuals.RotationState, RotationState.Horizontal, appearance);
			FixturesComponent fixtureComponent;
			if (base.TryComp<FixturesComponent>(uid, ref fixtureComponent))
			{
				foreach (KeyValuePair<string, Fixture> keyValuePair in fixtureComponent.Fixtures)
				{
					string text;
					Fixture fixture2;
					keyValuePair.Deconstruct(out text, out fixture2);
					string key = text;
					Fixture fixture = fixture2;
					if ((fixture.CollisionMask & 4) != 0)
					{
						standingState.ChangedFixtures.Add(key);
						this._physics.SetCollisionMask(uid, fixture, fixture.CollisionMask & -5, fixtureComponent, null);
					}
				}
			}
			if (standingState.LifeStage <= 5)
			{
				return true;
			}
			if (playSound)
			{
				this._audio.PlayPredicted(standingState.DownSound, uid, new EntityUid?(uid), new AudioParams?(AudioParams.Default.WithVariation(new float?(0.25f))));
			}
			return true;
		}

		// Token: 0x0600044E RID: 1102 RVA: 0x000114EC File Offset: 0x0000F6EC
		[NullableContext(2)]
		public bool Stand(EntityUid uid, StandingStateComponent standingState = null, AppearanceComponent appearance = null, bool force = false)
		{
			if (!base.Resolve<StandingStateComponent>(uid, ref standingState, false))
			{
				return false;
			}
			base.Resolve<AppearanceComponent>(uid, ref appearance, false);
			if (standingState.Standing)
			{
				return true;
			}
			if (!force)
			{
				StandAttemptEvent msg = new StandAttemptEvent();
				base.RaiseLocalEvent<StandAttemptEvent>(uid, msg, false);
				if (msg.Cancelled)
				{
					return false;
				}
			}
			standingState.Standing = true;
			base.Dirty(standingState, null);
			base.RaiseLocalEvent<StoodEvent>(uid, new StoodEvent(), false);
			this._appearance.SetData(uid, RotationVisuals.RotationState, RotationState.Vertical, appearance);
			FixturesComponent fixtureComponent;
			if (base.TryComp<FixturesComponent>(uid, ref fixtureComponent))
			{
				foreach (string key in standingState.ChangedFixtures)
				{
					Fixture fixture;
					if (fixtureComponent.Fixtures.TryGetValue(key, out fixture))
					{
						this._physics.SetCollisionMask(uid, fixture, fixture.CollisionMask | 4, fixtureComponent, null);
					}
				}
			}
			standingState.ChangedFixtures.Clear();
			return true;
		}

		// Token: 0x04000419 RID: 1049
		[Dependency]
		private readonly SharedAppearanceSystem _appearance;

		// Token: 0x0400041A RID: 1050
		[Dependency]
		private readonly SharedAudioSystem _audio;

		// Token: 0x0400041B RID: 1051
		[Dependency]
		private readonly SharedPhysicsSystem _physics;

		// Token: 0x0400041C RID: 1052
		private const int StandingCollisionLayer = 4;

		// Token: 0x020007A3 RID: 1955
		[Nullable(0)]
		[NetSerializable]
		[Serializable]
		private sealed class StandingComponentState : ComponentState
		{
			// Token: 0x170004F1 RID: 1265
			// (get) Token: 0x060017E5 RID: 6117 RVA: 0x0004D2CA File Offset: 0x0004B4CA
			public bool Standing { get; }

			// Token: 0x170004F2 RID: 1266
			// (get) Token: 0x060017E6 RID: 6118 RVA: 0x0004D2D2 File Offset: 0x0004B4D2
			public List<string> ChangedFixtures { get; }

			// Token: 0x060017E7 RID: 6119 RVA: 0x0004D2DA File Offset: 0x0004B4DA
			public StandingComponentState(bool standing, List<string> changedFixtures)
			{
				this.Standing = standing;
				this.ChangedFixtures = changedFixtures;
			}
		}
	}
}
