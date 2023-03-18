using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Content.Shared.ActionBlocker;
using Content.Shared.Hands.Components;
using Content.Shared.Interaction;
using Content.Shared.Physics;
using Robust.Shared.Containers;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;

namespace Content.Shared.Verbs
{
	// Token: 0x02000086 RID: 134
	[NullableContext(1)]
	[Nullable(0)]
	public abstract class SharedVerbSystem : EntitySystem
	{
		// Token: 0x06000191 RID: 401 RVA: 0x00008E41 File Offset: 0x00007041
		public override void Initialize()
		{
			base.Initialize();
			base.SubscribeAllEvent<ExecuteVerbEvent>(new EntitySessionEventHandler<ExecuteVerbEvent>(this.HandleExecuteVerb), null, null);
		}

		// Token: 0x06000192 RID: 402 RVA: 0x00008E60 File Offset: 0x00007060
		private void HandleExecuteVerb(ExecuteVerbEvent args, EntitySessionEventArgs eventArgs)
		{
			EntityUid? user = eventArgs.SenderSession.AttachedEntity;
			if (user == null)
			{
				return;
			}
			if (base.Deleted(args.Target, null) || base.Deleted(user))
			{
				return;
			}
			Verb verb;
			if (this.GetLocalVerbs(args.Target, user.Value, args.RequestedVerb.GetType(), false).TryGetValue(args.RequestedVerb, out verb))
			{
				this.ExecuteVerb(verb, user.Value, args.Target, false);
			}
		}

		// Token: 0x06000193 RID: 403 RVA: 0x00008EE0 File Offset: 0x000070E0
		public SortedSet<Verb> GetLocalVerbs(EntityUid target, EntityUid user, Type type, bool force = false)
		{
			return this.GetLocalVerbs(target, user, new List<Type>
			{
				type
			}, force);
		}

		// Token: 0x06000194 RID: 404 RVA: 0x00008EF8 File Offset: 0x000070F8
		public SortedSet<Verb> GetLocalVerbs(EntityUid target, EntityUid user, List<Type> types, bool force = false)
		{
			SortedSet<Verb> verbs = new SortedSet<Verb>();
			bool canAccess = false;
			if (force || target == user)
			{
				canAccess = true;
			}
			else if (this._interactionSystem.InRangeUnobstructed(user, target, 1.5f, CollisionGroup.Impassable | CollisionGroup.InteractImpassable, null, false))
			{
				canAccess = (this.ContainerSystem.IsInSameOrParentContainer(user, target) || this._interactionSystem.CanAccessViaStorage(user, target));
			}
			bool canInteract = force || this._actionBlockerSystem.CanInteract(user, new EntityUid?(target));
			EntityUid? @using = null;
			SharedHandsComponent hands;
			if (base.TryComp<SharedHandsComponent>(user, ref hands) && (force || this._actionBlockerSystem.CanUseHeldEntity(user)))
			{
				@using = hands.ActiveHandEntity;
				HandVirtualItemComponent pull;
				if (base.TryComp<HandVirtualItemComponent>(@using, ref pull))
				{
					@using = new EntityUid?(pull.BlockingEntity);
				}
			}
			if (types.Contains(typeof(InteractionVerb)))
			{
				GetVerbsEvent<InteractionVerb> verbEvent = new GetVerbsEvent<InteractionVerb>(user, target, @using, hands, canInteract, canAccess);
				base.RaiseLocalEvent<GetVerbsEvent<InteractionVerb>>(target, verbEvent, true);
				verbs.UnionWith(verbEvent.Verbs);
			}
			if (types.Contains(typeof(UtilityVerb)) && @using != null)
			{
				EntityUid? entityUid = @using;
				if (entityUid == null || (entityUid != null && entityUid.GetValueOrDefault() != target))
				{
					GetVerbsEvent<UtilityVerb> verbEvent2 = new GetVerbsEvent<UtilityVerb>(user, target, @using, hands, canInteract, canAccess);
					base.RaiseLocalEvent<GetVerbsEvent<UtilityVerb>>(@using.Value, verbEvent2, true);
					verbs.UnionWith(verbEvent2.Verbs);
				}
			}
			if (types.Contains(typeof(InnateVerb)))
			{
				GetVerbsEvent<InnateVerb> verbEvent3 = new GetVerbsEvent<InnateVerb>(user, target, @using, hands, canInteract, canAccess);
				base.RaiseLocalEvent<GetVerbsEvent<InnateVerb>>(user, verbEvent3, true);
				verbs.UnionWith(verbEvent3.Verbs);
			}
			if (types.Contains(typeof(AlternativeVerb)))
			{
				GetVerbsEvent<AlternativeVerb> verbEvent4 = new GetVerbsEvent<AlternativeVerb>(user, target, @using, hands, canInteract, canAccess);
				base.RaiseLocalEvent<GetVerbsEvent<AlternativeVerb>>(target, verbEvent4, true);
				verbs.UnionWith(verbEvent4.Verbs);
			}
			if (types.Contains(typeof(ActivationVerb)))
			{
				GetVerbsEvent<ActivationVerb> verbEvent5 = new GetVerbsEvent<ActivationVerb>(user, target, @using, hands, canInteract, canAccess);
				base.RaiseLocalEvent<GetVerbsEvent<ActivationVerb>>(target, verbEvent5, true);
				verbs.UnionWith(verbEvent5.Verbs);
			}
			if (types.Contains(typeof(ExamineVerb)))
			{
				GetVerbsEvent<ExamineVerb> verbEvent6 = new GetVerbsEvent<ExamineVerb>(user, target, @using, hands, canInteract, canAccess);
				base.RaiseLocalEvent<GetVerbsEvent<ExamineVerb>>(target, verbEvent6, true);
				verbs.UnionWith(verbEvent6.Verbs);
			}
			if (types.Contains(typeof(Verb)))
			{
				GetVerbsEvent<Verb> verbEvent7 = new GetVerbsEvent<Verb>(user, target, @using, hands, canInteract, canAccess);
				base.RaiseLocalEvent<GetVerbsEvent<Verb>>(target, verbEvent7, true);
				verbs.UnionWith(verbEvent7.Verbs);
			}
			return verbs;
		}

		// Token: 0x06000195 RID: 405 RVA: 0x0000917C File Offset: 0x0000737C
		public virtual void ExecuteVerb(Verb verb, EntityUid user, EntityUid target, bool forced = false)
		{
			Action act = verb.Act;
			if (act != null)
			{
				act();
			}
			if (verb.ExecutionEventArgs != null)
			{
				if (verb.EventTarget.IsValid())
				{
					base.RaiseLocalEvent(verb.EventTarget, verb.ExecutionEventArgs, false);
				}
				else
				{
					base.RaiseLocalEvent(verb.ExecutionEventArgs);
				}
			}
			if (base.Deleted(user, null) || base.Deleted(target, null))
			{
				return;
			}
			if (verb.DoContactInteraction ?? (verb.DefaultDoContactInteraction && this._interactionSystem.InRangeUnobstructed(user, target, 1.5f, CollisionGroup.Impassable | CollisionGroup.InteractImpassable, null, false)))
			{
				this._interactionSystem.DoContactInteraction(user, new EntityUid?(target), null);
			}
		}

		// Token: 0x040001B9 RID: 441
		[Dependency]
		private readonly SharedInteractionSystem _interactionSystem;

		// Token: 0x040001BA RID: 442
		[Dependency]
		private readonly ActionBlockerSystem _actionBlockerSystem;

		// Token: 0x040001BB RID: 443
		[Dependency]
		protected readonly SharedContainerSystem ContainerSystem;
	}
}
