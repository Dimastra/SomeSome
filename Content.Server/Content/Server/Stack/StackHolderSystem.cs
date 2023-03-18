using System;
using System.Runtime.CompilerServices;
using Content.Shared.Containers.ItemSlots;
using Content.Shared.Examine;
using Content.Shared.Interaction;
using Content.Shared.Stacks;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Localization;

namespace Content.Server.Stack
{
	// Token: 0x020001AA RID: 426
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class StackHolderSystem : EntitySystem
	{
		// Token: 0x06000869 RID: 2153 RVA: 0x0002AFB2 File Offset: 0x000291B2
		public override void Initialize()
		{
			base.Initialize();
			base.SubscribeLocalEvent<StackHolderComponent, ExaminedEvent>(new ComponentEventHandler<StackHolderComponent, ExaminedEvent>(this.OnExamined), null, null);
			base.SubscribeLocalEvent<StackHolderComponent, AfterInteractEvent>(new ComponentEventHandler<StackHolderComponent, AfterInteractEvent>(this.OnAfterInteract), null, null);
		}

		// Token: 0x0600086A RID: 2154 RVA: 0x0002AFE4 File Offset: 0x000291E4
		private void OnExamined(EntityUid uid, StackHolderComponent component, ExaminedEvent args)
		{
			EntityUid? item = this._itemSlotsSystem.GetItemOrNull(uid, "stack_slot", null);
			if (item == null)
			{
				args.PushMarkup(Loc.GetString("stack-holder-empty"));
				return;
			}
			StackComponent stack;
			if (base.TryComp<StackComponent>(item, ref stack))
			{
				args.PushMarkup(Loc.GetString("stack-holder", new ValueTuple<string, object>[]
				{
					new ValueTuple<string, object>("number", stack.Count),
					new ValueTuple<string, object>("item", item)
				}));
			}
		}

		// Token: 0x0600086B RID: 2155 RVA: 0x0002B074 File Offset: 0x00029274
		private void OnAfterInteract(EntityUid uid, StackHolderComponent component, AfterInteractEvent args)
		{
			EntityUid? item = this._itemSlotsSystem.GetItemOrNull(uid, "stack_slot", null);
			if (item == null)
			{
				if (args.Target != null)
				{
					this._itemSlotsSystem.TryInsert(uid, "stack_slot", args.Target.Value, new EntityUid?(args.User), null);
				}
				return;
			}
			AfterInteractEvent afterEv = new AfterInteractEvent(args.User, item.Value, args.Target, args.ClickLocation, args.CanReach);
			base.RaiseLocalEvent<AfterInteractEvent>(item.Value, afterEv, false);
			if (args.Target != null)
			{
				InteractUsingEvent ev = new InteractUsingEvent(args.User, item.Value, args.Target.Value, args.ClickLocation);
				base.RaiseLocalEvent<InteractUsingEvent>(args.Target.Value, ev, false);
			}
		}

		// Token: 0x04000528 RID: 1320
		[Dependency]
		private readonly ItemSlotsSystem _itemSlotsSystem;
	}
}
