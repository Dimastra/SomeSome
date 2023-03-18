using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Runtime.CompilerServices;
using Content.Server.Body.Components;
using Content.Server.GameTicking;
using Content.Server.Humanoid;
using Content.Server.Kitchen.Components;
using Content.Server.Mind.Components;
using Content.Shared.Body.Components;
using Content.Shared.Body.Organ;
using Content.Shared.Body.Part;
using Content.Shared.Body.Prototypes;
using Content.Shared.Body.Systems;
using Content.Shared.Coordinates;
using Content.Shared.Humanoid;
using Content.Shared.Mobs.Systems;
using Content.Shared.Movement.Events;
using Content.Shared.Random.Helpers;
using Robust.Shared.Audio;
using Robust.Shared.Containers;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Map;
using Robust.Shared.Player;
using Robust.Shared.Timing;

namespace Content.Server.Body.Systems
{
	// Token: 0x02000706 RID: 1798
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class BodySystem : SharedBodySystem
	{
		// Token: 0x060025D4 RID: 9684 RVA: 0x000C7570 File Offset: 0x000C5770
		public override void Initialize()
		{
			base.Initialize();
			base.SubscribeLocalEvent<BodyComponent, MoveInputEvent>(new ComponentEventRefHandler<BodyComponent, MoveInputEvent>(this.OnRelayMoveInput), null, null);
			base.SubscribeLocalEvent<BodyComponent, ApplyMetabolicMultiplierEvent>(new ComponentEventHandler<BodyComponent, ApplyMetabolicMultiplierEvent>(this.OnApplyMetabolicMultiplier), null, null);
			base.SubscribeLocalEvent<BodyComponent, BeingMicrowavedEvent>(new ComponentEventHandler<BodyComponent, BeingMicrowavedEvent>(this.OnBeingMicrowaved), null, null);
		}

		// Token: 0x060025D5 RID: 9685 RVA: 0x000C75C0 File Offset: 0x000C57C0
		private void OnRelayMoveInput(EntityUid uid, BodyComponent component, ref MoveInputEvent args)
		{
			MindComponent mind;
			if (this._mobState.IsDead(uid, null) && this.EntityManager.TryGetComponent<MindComponent>(uid, ref mind) && mind.HasMind)
			{
				if (mind.Mind.TimeOfDeath == null)
				{
					mind.Mind.TimeOfDeath = new TimeSpan?(this._gameTiming.RealTime);
				}
				this._ticker.OnGhostAttempt(mind.Mind, true, false);
			}
		}

		// Token: 0x060025D6 RID: 9686 RVA: 0x000C7638 File Offset: 0x000C5838
		private void OnApplyMetabolicMultiplier(EntityUid uid, BodyComponent component, ApplyMetabolicMultiplierEvent args)
		{
			foreach (ValueTuple<EntityUid, OrganComponent> organ in base.GetBodyOrgans(new EntityUid?(uid), component))
			{
				base.RaiseLocalEvent<ApplyMetabolicMultiplierEvent>(organ.Item1, args, false);
			}
		}

		// Token: 0x060025D7 RID: 9687 RVA: 0x000C7694 File Offset: 0x000C5894
		private void OnBeingMicrowaved(EntityUid uid, BodyComponent component, BeingMicrowavedEvent args)
		{
			if (args.Handled)
			{
				return;
			}
			base.Transform(uid).AttachToGridOrMap();
			this.GibBody(new EntityUid?(uid), false, component, false);
			args.Handled = true;
		}

		// Token: 0x060025D8 RID: 9688 RVA: 0x000C76C4 File Offset: 0x000C58C4
		public override bool AttachPart(EntityUid? partId, BodyPartSlot slot, [Nullable(2)] [NotNullWhen(true)] BodyPartComponent part = null)
		{
			if (!base.AttachPart(partId, slot, part))
			{
				return false;
			}
			EntityUid? body2 = part.Body;
			if (body2 != null)
			{
				EntityUid body = body2.GetValueOrDefault();
				HumanoidAppearanceComponent humanoid;
				if (base.TryComp<HumanoidAppearanceComponent>(body, ref humanoid))
				{
					HumanoidVisualLayers? layer = part.ToHumanoidLayers();
					if (layer != null)
					{
						IEnumerable<HumanoidVisualLayers> layers = HumanoidVisualLayersExtension.Sublayers(layer.Value);
						this._humanoidSystem.SetLayersVisibility(body, layers, true, true, humanoid);
					}
				}
			}
			return true;
		}

		// Token: 0x060025D9 RID: 9689 RVA: 0x000C7734 File Offset: 0x000C5934
		[NullableContext(2)]
		public override bool DropPart(EntityUid? partId, BodyPartComponent part = null)
		{
			BodyPartComponent bodyPartComponent = base.CompOrNull<BodyPartComponent>(partId);
			EntityUid? oldBody = (bodyPartComponent != null) ? bodyPartComponent.Body : null;
			if (!base.DropPart(partId, part))
			{
				return false;
			}
			HumanoidAppearanceComponent humanoid;
			if (oldBody == null || !base.TryComp<HumanoidAppearanceComponent>(oldBody, ref humanoid))
			{
				return true;
			}
			HumanoidVisualLayers? layer = part.ToHumanoidLayers();
			if (layer == null)
			{
				return true;
			}
			IEnumerable<HumanoidVisualLayers> layers = HumanoidVisualLayersExtension.Sublayers(layer.Value);
			this._humanoidSystem.SetLayersVisibility(oldBody.Value, layers, false, true, humanoid);
			return true;
		}

		// Token: 0x060025DA RID: 9690 RVA: 0x000C77B8 File Offset: 0x000C59B8
		protected override void InitBody(BodyComponent body, BodyPrototype prototype)
		{
			BodyPrototypeSlot root = prototype.Slots[prototype.Root];
			this.Containers.EnsureContainer<Container>(body.Owner, "BodyContainer", null);
			if (root.Part == null)
			{
				return;
			}
			EntityUid bodyId = base.Spawn(root.Part, body.Owner.ToCoordinates());
			BodyPartComponent partComponent = base.Comp<BodyPartComponent>(bodyId);
			BodyPartSlot slot = new BodyPartSlot(root.Part, body.Owner, new BodyPartType?(partComponent.PartType));
			body.Root = slot;
			partComponent.Body = new EntityUid?(bodyId);
			this.AttachPart(new EntityUid?(bodyId), slot, partComponent);
			base.InitPart(partComponent, prototype, prototype.Root, null);
		}

		// Token: 0x060025DB RID: 9691 RVA: 0x000C7868 File Offset: 0x000C5A68
		public override HashSet<EntityUid> GibBody(EntityUid? bodyId, bool gibOrgans = false, [Nullable(2)] BodyComponent body = null, bool deleteItems = false)
		{
			if (bodyId == null || !base.Resolve<BodyComponent>(bodyId.Value, ref body, false))
			{
				return new HashSet<EntityUid>();
			}
			HashSet<EntityUid> gibs = base.GibBody(bodyId, gibOrgans, body, deleteItems);
			EntityCoordinates coordinates = base.Transform(bodyId.Value).Coordinates;
			Filter filter = Filter.Pvs(bodyId.Value, 2f, this.EntityManager, null, null);
			AudioParams audio = AudioParams.Default.WithVariation(new float?(0.025f));
			this._audio.Play(body.GibSound, filter, coordinates, true, new AudioParams?(audio));
			ContainerManagerComponent container;
			if (base.TryComp<ContainerManagerComponent>(bodyId, ref container))
			{
				foreach (IContainer cont in container.GetAllContainers().ToArray<IContainer>())
				{
					foreach (EntityUid ent in cont.ContainedEntities.ToArray<EntityUid>())
					{
						if (deleteItems)
						{
							base.QueueDel(ent);
						}
						else
						{
							cont.Remove(ent, this.EntityManager, null, null, true, true, null, null);
							base.Transform(ent).Coordinates = coordinates;
							ent.RandomOffset(0.25f);
						}
					}
				}
			}
			base.RaiseLocalEvent<BeingGibbedEvent>(bodyId.Value, new BeingGibbedEvent(gibs), false);
			base.QueueDel(bodyId.Value);
			return gibs;
		}

		// Token: 0x0400175F RID: 5983
		[Dependency]
		private readonly GameTicker _ticker;

		// Token: 0x04001760 RID: 5984
		[Dependency]
		private readonly IGameTiming _gameTiming;

		// Token: 0x04001761 RID: 5985
		[Dependency]
		private readonly HumanoidAppearanceSystem _humanoidSystem;

		// Token: 0x04001762 RID: 5986
		[Dependency]
		private readonly MobStateSystem _mobState;

		// Token: 0x04001763 RID: 5987
		[Dependency]
		private readonly SharedAudioSystem _audio;
	}
}
