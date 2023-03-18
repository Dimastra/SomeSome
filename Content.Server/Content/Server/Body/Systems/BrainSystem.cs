using System;
using System.Runtime.CompilerServices;
using Content.Server.Body.Components;
using Content.Server.Ghost.Components;
using Content.Server.Mind;
using Content.Server.Mind.Components;
using Content.Shared.Body.Components;
using Content.Shared.Body.Events;
using Content.Shared.Body.Organ;
using Content.Shared.Movement.Components;
using Robust.Shared.GameObjects;

namespace Content.Server.Body.Systems
{
	// Token: 0x02000707 RID: 1799
	public sealed class BrainSystem : EntitySystem
	{
		// Token: 0x060025DD RID: 9693 RVA: 0x000C79E8 File Offset: 0x000C5BE8
		public override void Initialize()
		{
			base.Initialize();
			base.SubscribeLocalEvent<BrainComponent, AddedToBodyEvent>(delegate(EntityUid uid, BrainComponent _, AddedToBodyEvent args)
			{
				this.HandleMind(args.Body, uid);
			}, null, null);
			base.SubscribeLocalEvent<BrainComponent, AddedToPartEvent>(delegate(EntityUid uid, BrainComponent _, AddedToPartEvent args)
			{
				this.HandleMind(args.Part, uid);
			}, null, null);
			base.SubscribeLocalEvent<BrainComponent, AddedToPartInBodyEvent>(delegate(EntityUid uid, BrainComponent _, AddedToPartInBodyEvent args)
			{
				this.HandleMind(args.Body, uid);
			}, null, null);
			base.SubscribeLocalEvent<BrainComponent, RemovedFromBodyEvent>(new ComponentEventHandler<BrainComponent, RemovedFromBodyEvent>(this.OnRemovedFromBody), null, null);
			base.SubscribeLocalEvent<BrainComponent, RemovedFromPartEvent>(delegate(EntityUid uid, BrainComponent _, RemovedFromPartEvent args)
			{
				this.HandleMind(uid, args.Old);
			}, null, null);
			base.SubscribeLocalEvent<BrainComponent, RemovedFromPartInBodyEvent>(delegate(EntityUid uid, BrainComponent _, RemovedFromPartInBodyEvent args)
			{
				this.HandleMind(args.OldBody, uid);
			}, null, null);
		}

		// Token: 0x060025DE RID: 9694 RVA: 0x000C7A74 File Offset: 0x000C5C74
		[NullableContext(1)]
		private void OnRemovedFromBody(EntityUid uid, BrainComponent component, RemovedFromBodyEvent args)
		{
			OrganComponent organ;
			if (this.EntityManager.TryGetComponent<OrganComponent>(uid, ref organ))
			{
				OrganSlot parentSlot = organ.ParentSlot;
				if (parentSlot != null)
				{
					EntityUid parent = parentSlot.Parent;
					this.HandleMind(parent, args.Old);
					return;
				}
			}
		}

		// Token: 0x060025DF RID: 9695 RVA: 0x000C7AB4 File Offset: 0x000C5CB4
		private void HandleMind(EntityUid newEntity, EntityUid oldEntity)
		{
			this.EntityManager.EnsureComponent<MindComponent>(newEntity);
			MindComponent mindComponent = this.EntityManager.EnsureComponent<MindComponent>(oldEntity);
			base.EnsureComp<GhostOnMoveComponent>(newEntity);
			if (base.HasComp<BodyComponent>(newEntity))
			{
				base.Comp<GhostOnMoveComponent>(newEntity).MustBeDead = true;
			}
			base.EnsureComp<InputMoverComponent>(newEntity);
			Mind mind = mindComponent.Mind;
			if (mind == null)
			{
				return;
			}
			mind.TransferTo(new EntityUid?(newEntity), false, true);
		}
	}
}
