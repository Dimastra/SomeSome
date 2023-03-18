using System;
using System.Runtime.CompilerServices;
using Content.Server.Popups;
using Content.Server.Shuttles.Components;
using Content.Server.Singularity.Events;
using Content.Shared.Popups;
using Content.Shared.Singularity.Components;
using Content.Shared.Throwing;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Localization;
using Robust.Shared.Maths;
using Robust.Shared.Physics.Components;
using Robust.Shared.Physics.Events;

namespace Content.Server.Singularity.EntitySystems
{
	// Token: 0x020001E8 RID: 488
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class ContainmentFieldSystem : EntitySystem
	{
		// Token: 0x06000943 RID: 2371 RVA: 0x0002EBF2 File Offset: 0x0002CDF2
		public override void Initialize()
		{
			base.Initialize();
			base.SubscribeLocalEvent<ContainmentFieldComponent, StartCollideEvent>(new ComponentEventRefHandler<ContainmentFieldComponent, StartCollideEvent>(this.HandleFieldCollide), null, null);
			base.SubscribeLocalEvent<ContainmentFieldComponent, EventHorizonAttemptConsumeEntityEvent>(new ComponentEventHandler<ContainmentFieldComponent, EventHorizonAttemptConsumeEntityEvent>(this.HandleEventHorizon), null, null);
		}

		// Token: 0x06000944 RID: 2372 RVA: 0x0002EC24 File Offset: 0x0002CE24
		private void HandleFieldCollide(EntityUid uid, ContainmentFieldComponent component, ref StartCollideEvent args)
		{
			EntityUid otherBody = args.OtherFixture.Body.Owner;
			SpaceGarbageComponent garbage;
			if (base.TryComp<SpaceGarbageComponent>(otherBody, ref garbage))
			{
				this._popupSystem.PopupEntity(Loc.GetString("comp-field-vaporized", new ValueTuple<string, object>[]
				{
					new ValueTuple<string, object>("entity", otherBody)
				}), component.Owner, PopupType.LargeCaution);
				base.QueueDel(garbage.Owner);
			}
			PhysicsComponent physics;
			if (base.TryComp<PhysicsComponent>(otherBody, ref physics) && physics.Mass <= component.MaxMass && physics.Hard)
			{
				Vector2 fieldDir = base.Transform(component.Owner).WorldPosition;
				Vector2 playerDir = base.Transform(otherBody).WorldPosition;
				this._throwing.TryThrow(otherBody, playerDir - fieldDir, component.ThrowForce, null, 5f, null, null, null, null);
			}
		}

		// Token: 0x06000945 RID: 2373 RVA: 0x0002ED12 File Offset: 0x0002CF12
		private void HandleEventHorizon(EntityUid uid, ContainmentFieldComponent component, EventHorizonAttemptConsumeEntityEvent args)
		{
			if (!args.Cancelled && !args.EventHorizon.CanBreachContainment)
			{
				args.Cancel();
			}
		}

		// Token: 0x0400059D RID: 1437
		[Dependency]
		private readonly ThrowingSystem _throwing;

		// Token: 0x0400059E RID: 1438
		[Dependency]
		private readonly PopupSystem _popupSystem;
	}
}
