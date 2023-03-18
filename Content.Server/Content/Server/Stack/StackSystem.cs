using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Content.Shared.Popups;
using Content.Shared.Stacks;
using Content.Shared.Verbs;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Localization;
using Robust.Shared.Map;
using Robust.Shared.Prototypes;

namespace Content.Server.Stack
{
	// Token: 0x020001A8 RID: 424
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class StackSystem : SharedStackSystem
	{
		// Token: 0x0600085F RID: 2143 RVA: 0x0002AC17 File Offset: 0x00028E17
		public override void Initialize()
		{
			base.Initialize();
			base.SubscribeLocalEvent<StackComponent, GetVerbsEvent<AlternativeVerb>>(new ComponentEventHandler<StackComponent, GetVerbsEvent<AlternativeVerb>>(this.OnStackAlternativeInteract), null, null);
		}

		// Token: 0x06000860 RID: 2144 RVA: 0x0002AC33 File Offset: 0x00028E33
		[NullableContext(2)]
		public override void SetCount(EntityUid uid, int amount, StackComponent component = null)
		{
			if (!base.Resolve<StackComponent>(uid, ref component, false))
			{
				return;
			}
			base.SetCount(uid, amount, component);
			if (component.Count <= 0)
			{
				base.QueueDel(uid);
			}
		}

		// Token: 0x06000861 RID: 2145 RVA: 0x0002AC5C File Offset: 0x00028E5C
		[NullableContext(2)]
		public EntityUid? Split(EntityUid uid, int amount, EntityCoordinates spawnPosition, StackComponent stack = null)
		{
			if (!base.Resolve<StackComponent>(uid, ref stack, true))
			{
				return null;
			}
			if (stack.StackTypeId == null)
			{
				return null;
			}
			StackPrototype stackType;
			string text;
			if (!this._prototypeManager.TryIndex<StackPrototype>(stack.StackTypeId, ref stackType))
			{
				EntityPrototype entityPrototype = base.Prototype(stack.Owner, null);
				text = ((entityPrototype != null) ? entityPrototype.ID : null);
			}
			else
			{
				text = stackType.Spawn;
			}
			string prototype = text;
			if (!base.Use(uid, amount, stack))
			{
				return null;
			}
			EntityUid entity = base.Spawn(prototype, spawnPosition);
			StackComponent stackComp;
			if (base.TryComp<StackComponent>(entity, ref stackComp))
			{
				this.SetCount(entity, amount, stackComp);
				stackComp.Unlimited = false;
			}
			return new EntityUid?(entity);
		}

		// Token: 0x06000862 RID: 2146 RVA: 0x0002AD10 File Offset: 0x00028F10
		public EntityUid Spawn(int amount, StackPrototype prototype, EntityCoordinates spawnPosition)
		{
			EntityUid entity = base.Spawn(prototype.Spawn, spawnPosition);
			StackComponent stack = base.Comp<StackComponent>(entity);
			this.SetCount(entity, amount, stack);
			return entity;
		}

		// Token: 0x06000863 RID: 2147 RVA: 0x0002AD40 File Offset: 0x00028F40
		public List<EntityUid> SpawnMultiple(string entityPrototype, int amount, EntityCoordinates spawnPosition)
		{
			StackComponent stack;
			this._prototypeManager.Index<EntityPrototype>(entityPrototype).TryGetComponent<StackComponent>(ref stack, null);
			int maxCountPerStack = base.GetMaxCount(stack);
			List<EntityUid> spawnedEnts = new List<EntityUid>();
			while (amount > 0)
			{
				EntityUid entity = base.Spawn(entityPrototype, spawnPosition);
				spawnedEnts.Add(entity);
				int countAmount = Math.Min(maxCountPerStack, amount);
				this.SetCount(entity, countAmount, null);
				amount -= countAmount;
			}
			return spawnedEnts;
		}

		// Token: 0x06000864 RID: 2148 RVA: 0x0002ADA0 File Offset: 0x00028FA0
		private void OnStackAlternativeInteract(EntityUid uid, StackComponent stack, GetVerbsEvent<AlternativeVerb> args)
		{
			if (!args.CanAccess || !args.CanInteract || args.Hands == null)
			{
				return;
			}
			AlternativeVerb halve = new AlternativeVerb
			{
				Text = Loc.GetString("comp-stack-split-halve"),
				Category = VerbCategory.Split,
				Act = delegate()
				{
					this.UserSplit(uid, args.User, stack.Count / 2, stack, null);
				},
				Priority = 1
			};
			args.Verbs.Add(halve);
			int priority = 0;
			int[] defaultSplitAmounts = StackSystem.DefaultSplitAmounts;
			for (int i = 0; i < defaultSplitAmounts.Length; i++)
			{
				int amount = defaultSplitAmounts[i];
				if (amount < stack.Count)
				{
					AlternativeVerb verb = new AlternativeVerb
					{
						Text = amount.ToString(),
						Category = VerbCategory.Split,
						Act = delegate()
						{
							this.UserSplit(uid, args.User, amount, stack, null);
						},
						Priority = priority
					};
					priority--;
					args.Verbs.Add(verb);
				}
			}
		}

		// Token: 0x06000865 RID: 2149 RVA: 0x0002AEF4 File Offset: 0x000290F4
		[NullableContext(2)]
		private void UserSplit(EntityUid uid, EntityUid userUid, int amount, StackComponent stack = null, TransformComponent userTransform = null)
		{
			if (!base.Resolve<StackComponent>(uid, ref stack, true))
			{
				return;
			}
			if (!base.Resolve<TransformComponent>(userUid, ref userTransform, true))
			{
				return;
			}
			if (amount <= 0)
			{
				this.PopupSystem.PopupCursor(Loc.GetString("comp-stack-split-too-small"), userUid, PopupType.Medium);
				return;
			}
			EntityUid? entityUid = this.Split(uid, amount, userTransform.Coordinates, stack);
			if (entityUid != null)
			{
				EntityUid split = entityUid.GetValueOrDefault();
				this.HandsSystem.PickupOrDrop(new EntityUid?(userUid), split, true, false, null, null);
				this.PopupSystem.PopupCursor(Loc.GetString("comp-stack-split"), userUid, PopupType.Small);
				return;
			}
		}

		// Token: 0x04000526 RID: 1318
		[Dependency]
		private readonly IPrototypeManager _prototypeManager;

		// Token: 0x04000527 RID: 1319
		public static readonly int[] DefaultSplitAmounts = new int[]
		{
			1,
			5,
			10,
			20,
			30,
			50
		};
	}
}
