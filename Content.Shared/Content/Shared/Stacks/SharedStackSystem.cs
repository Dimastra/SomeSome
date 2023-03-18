using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Content.Shared.Examine;
using Content.Shared.Hands.Components;
using Content.Shared.Hands.EntitySystems;
using Content.Shared.Interaction;
using Content.Shared.Popups;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.IoC;
using Robust.Shared.Localization;
using Robust.Shared.Map;
using Robust.Shared.Maths;
using Robust.Shared.Physics.Systems;
using Robust.Shared.Player;
using Robust.Shared.Prototypes;
using Robust.Shared.Timing;
using Robust.Shared.ViewVariables;

namespace Content.Shared.Stacks
{
	// Token: 0x0200016A RID: 362
	[NullableContext(1)]
	[Nullable(0)]
	public abstract class SharedStackSystem : EntitySystem
	{
		// Token: 0x06000455 RID: 1109 RVA: 0x00011620 File Offset: 0x0000F820
		public override void Initialize()
		{
			base.Initialize();
			base.SubscribeLocalEvent<StackComponent, ComponentGetState>(new ComponentEventRefHandler<StackComponent, ComponentGetState>(this.OnStackGetState), null, null);
			base.SubscribeLocalEvent<StackComponent, ComponentHandleState>(new ComponentEventRefHandler<StackComponent, ComponentHandleState>(this.OnStackHandleState), null, null);
			base.SubscribeLocalEvent<StackComponent, ComponentStartup>(new ComponentEventHandler<StackComponent, ComponentStartup>(this.OnStackStarted), null, null);
			base.SubscribeLocalEvent<StackComponent, ExaminedEvent>(new ComponentEventHandler<StackComponent, ExaminedEvent>(this.OnStackExamined), null, null);
			base.SubscribeLocalEvent<StackComponent, InteractUsingEvent>(new ComponentEventHandler<StackComponent, InteractUsingEvent>(this.OnStackInteractUsing), null, null);
			this._vvm.GetTypeHandler<StackComponent>().AddPath<int>("Count", (EntityUid _, StackComponent comp) => comp.Count, new ComponentPropertySetter<StackComponent, int>(this.SetCount));
		}

		// Token: 0x06000456 RID: 1110 RVA: 0x000116D9 File Offset: 0x0000F8D9
		public override void Shutdown()
		{
			base.Shutdown();
			this._vvm.GetTypeHandler<StackComponent>().RemovePath("Count");
		}

		// Token: 0x06000457 RID: 1111 RVA: 0x000116F8 File Offset: 0x0000F8F8
		private void OnStackInteractUsing(EntityUid uid, StackComponent stack, InteractUsingEvent args)
		{
			if (args.Handled)
			{
				return;
			}
			StackComponent recipientStack;
			if (!base.TryComp<StackComponent>(args.Used, ref recipientStack))
			{
				return;
			}
			int transfered;
			if (!this.TryMergeStacks(uid, args.Used, out transfered, stack, recipientStack))
			{
				return;
			}
			args.Handled = true;
			if (!this._gameTiming.IsFirstTimePredicted)
			{
				return;
			}
			EntityCoordinates popupPos = args.ClickLocation;
			if (!popupPos.IsValid(this.EntityManager))
			{
				popupPos = base.Transform(args.User).Coordinates;
			}
			int num = transfered;
			if (num <= 0)
			{
				if (num != 0)
				{
					return;
				}
				if (this.GetAvailableSpace(recipientStack) == 0)
				{
					this.PopupSystem.PopupCoordinates(Loc.GetString("comp-stack-already-full"), popupPos, Filter.Local(), false, PopupType.Small);
				}
			}
			else
			{
				SharedPopupSystem popupSystem = this.PopupSystem;
				DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(1, 1);
				defaultInterpolatedStringHandler.AppendLiteral("+");
				defaultInterpolatedStringHandler.AppendFormatted<int>(transfered);
				popupSystem.PopupCoordinates(defaultInterpolatedStringHandler.ToStringAndClear(), popupPos, Filter.Local(), false, PopupType.Small);
				if (this.GetAvailableSpace(recipientStack) == 0)
				{
					this.PopupSystem.PopupCoordinates(Loc.GetString("comp-stack-becomes-full"), popupPos.Offset(new Vector2(0f, -0.5f)), Filter.Local(), false, PopupType.Small);
					return;
				}
			}
		}

		// Token: 0x06000458 RID: 1112 RVA: 0x00011814 File Offset: 0x0000FA14
		[NullableContext(2)]
		private bool TryMergeStacks(EntityUid donor, EntityUid recipient, out int transfered, StackComponent donorStack = null, StackComponent recipientStack = null)
		{
			transfered = 0;
			if (donor == recipient)
			{
				return false;
			}
			if (!base.Resolve<StackComponent>(recipient, ref recipientStack, false) || !base.Resolve<StackComponent>(donor, ref donorStack, false))
			{
				return false;
			}
			if (recipientStack.StackTypeId == null || !recipientStack.StackTypeId.Equals(donorStack.StackTypeId))
			{
				return false;
			}
			transfered = Math.Min(donorStack.Count, this.GetAvailableSpace(recipientStack));
			this.SetCount(donor, donorStack.Count - transfered, donorStack);
			this.SetCount(recipient, recipientStack.Count + transfered, recipientStack);
			return true;
		}

		// Token: 0x06000459 RID: 1113 RVA: 0x000118A8 File Offset: 0x0000FAA8
		[NullableContext(2)]
		public void TryMergeToHands(EntityUid item, EntityUid user, StackComponent itemStack = null, SharedHandsComponent hands = null)
		{
			if (!base.Resolve<SharedHandsComponent>(user, ref hands, false))
			{
				return;
			}
			if (!base.Resolve<StackComponent>(item, ref itemStack, false))
			{
				this.HandsSystem.PickupOrDrop(new EntityUid?(user), item, true, false, hands, null);
				return;
			}
			foreach (EntityUid held in this.HandsSystem.EnumerateHeld(user, hands))
			{
				int num;
				this.TryMergeStacks(item, held, out num, itemStack, null);
				if (itemStack.Count == 0)
				{
					return;
				}
			}
			this.HandsSystem.PickupOrDrop(new EntityUid?(user), item, true, false, hands, null);
		}

		// Token: 0x0600045A RID: 1114 RVA: 0x00011958 File Offset: 0x0000FB58
		[NullableContext(2)]
		public virtual void SetCount(EntityUid uid, int amount, StackComponent component = null)
		{
			if (!base.Resolve<StackComponent>(uid, ref component, true))
			{
				return;
			}
			if (amount == component.Count)
			{
				return;
			}
			int old = component.Count;
			amount = Math.Min(amount, this.GetMaxCount(component));
			amount = Math.Max(amount, 0);
			component.Count = amount;
			base.Dirty(component, null);
			this.Appearance.SetData(uid, StackVisuals.Actual, component.Count, null);
			base.RaiseLocalEvent<StackCountChangedEvent>(uid, new StackCountChangedEvent(old, component.Count), false);
		}

		// Token: 0x0600045B RID: 1115 RVA: 0x000119DC File Offset: 0x0000FBDC
		[NullableContext(2)]
		public bool Use(EntityUid uid, int amount, StackComponent stack = null)
		{
			if (!base.Resolve<StackComponent>(uid, ref stack, true))
			{
				return false;
			}
			if (stack.Count < amount)
			{
				return false;
			}
			if (!stack.Unlimited)
			{
				this.SetCount(uid, stack.Count - amount, stack);
			}
			return true;
		}

		// Token: 0x0600045C RID: 1116 RVA: 0x00011A10 File Offset: 0x0000FC10
		[NullableContext(2)]
		public bool TryMergeToContacts(EntityUid uid, StackComponent stack = null, TransformComponent xform = null)
		{
			if (!base.Resolve<StackComponent, TransformComponent>(uid, ref stack, ref xform, false))
			{
				return false;
			}
			MapId map = xform.MapID;
			Box2 bounds = this._physics.GetWorldAABB(uid, null, null, null);
			HashSet<StackComponent> componentsIntersecting = this._entityLookup.GetComponentsIntersecting<StackComponent>(map, bounds, 10);
			bool merged = false;
			foreach (StackComponent otherStack in componentsIntersecting)
			{
				EntityUid otherEnt = otherStack.Owner;
				int num;
				if (this.TryMergeStacks(uid, otherEnt, out num, stack, otherStack))
				{
					merged = true;
					if (stack.Count <= 0)
					{
						break;
					}
				}
			}
			return merged;
		}

		// Token: 0x0600045D RID: 1117 RVA: 0x00011AB8 File Offset: 0x0000FCB8
		public int GetMaxCount(string entityId)
		{
			StackComponent stackComp;
			this._prototype.Index<EntityPrototype>(entityId).TryGetComponent<StackComponent>(ref stackComp, null);
			return this.GetMaxCount(stackComp);
		}

		// Token: 0x0600045E RID: 1118 RVA: 0x00011AE1 File Offset: 0x0000FCE1
		public int GetMaxCount(EntityUid uid)
		{
			return this.GetMaxCount(base.CompOrNull<StackComponent>(uid));
		}

		// Token: 0x0600045F RID: 1119 RVA: 0x00011AF0 File Offset: 0x0000FCF0
		[NullableContext(2)]
		public int GetMaxCount(StackComponent component)
		{
			if (component == null)
			{
				return 1;
			}
			if (component.MaxCountOverride != null)
			{
				return component.MaxCountOverride.Value;
			}
			if (component.StackTypeId == null)
			{
				return 1;
			}
			int? maxCount = this._prototype.Index<StackPrototype>(component.StackTypeId).MaxCount;
			if (maxCount == null)
			{
				return int.MaxValue;
			}
			return maxCount.GetValueOrDefault();
		}

		// Token: 0x06000460 RID: 1120 RVA: 0x00011B58 File Offset: 0x0000FD58
		public int GetAvailableSpace(StackComponent component)
		{
			return this.GetMaxCount(component) - component.Count;
		}

		// Token: 0x06000461 RID: 1121 RVA: 0x00011B68 File Offset: 0x0000FD68
		private void OnStackStarted(EntityUid uid, StackComponent component, ComponentStartup args)
		{
			AppearanceComponent appearance;
			if (!base.TryComp<AppearanceComponent>(uid, ref appearance))
			{
				return;
			}
			this.Appearance.SetData(uid, StackVisuals.Actual, component.Count, appearance);
			this.Appearance.SetData(uid, StackVisuals.MaxCount, this.GetMaxCount(component), appearance);
			this.Appearance.SetData(uid, StackVisuals.Hide, false, appearance);
		}

		// Token: 0x06000462 RID: 1122 RVA: 0x00011BD7 File Offset: 0x0000FDD7
		private void OnStackGetState(EntityUid uid, StackComponent component, ref ComponentGetState args)
		{
			args.State = new StackComponentState(component.Count, this.GetMaxCount(component));
		}

		// Token: 0x06000463 RID: 1123 RVA: 0x00011BF4 File Offset: 0x0000FDF4
		private void OnStackHandleState(EntityUid uid, StackComponent component, ref ComponentHandleState args)
		{
			StackComponentState cast = args.Current as StackComponentState;
			if (cast == null)
			{
				return;
			}
			component.MaxCountOverride = new int?(cast.MaxCount);
			this.SetCount(uid, cast.Count, component);
		}

		// Token: 0x06000464 RID: 1124 RVA: 0x00011C30 File Offset: 0x0000FE30
		private void OnStackExamined(EntityUid uid, StackComponent component, ExaminedEvent args)
		{
			if (!args.IsInDetailsRange)
			{
				return;
			}
			args.PushMarkup(Loc.GetString("comp-stack-examine-detail-count", new ValueTuple<string, object>[]
			{
				new ValueTuple<string, object>("count", component.Count),
				new ValueTuple<string, object>("markupCountColor", "lightgray")
			}));
		}

		// Token: 0x0400041D RID: 1053
		[Dependency]
		private readonly IGameTiming _gameTiming;

		// Token: 0x0400041E RID: 1054
		[Dependency]
		private readonly IPrototypeManager _prototype;

		// Token: 0x0400041F RID: 1055
		[Dependency]
		private readonly IViewVariablesManager _vvm;

		// Token: 0x04000420 RID: 1056
		[Dependency]
		protected readonly SharedAppearanceSystem Appearance;

		// Token: 0x04000421 RID: 1057
		[Dependency]
		protected readonly SharedHandsSystem HandsSystem;

		// Token: 0x04000422 RID: 1058
		[Dependency]
		protected readonly SharedTransformSystem Xform;

		// Token: 0x04000423 RID: 1059
		[Dependency]
		private readonly EntityLookupSystem _entityLookup;

		// Token: 0x04000424 RID: 1060
		[Dependency]
		private readonly SharedPhysicsSystem _physics;

		// Token: 0x04000425 RID: 1061
		[Dependency]
		protected readonly SharedPopupSystem PopupSystem;
	}
}
