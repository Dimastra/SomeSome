using System;
using System.Runtime.CompilerServices;
using Content.Shared.Stacks;
using Content.Shared.Storage.Components;
using Robust.Shared.Containers;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;

namespace Content.Shared.Storage.EntitySystems
{
	// Token: 0x02000130 RID: 304
	[NullableContext(1)]
	[Nullable(0)]
	public abstract class SharedItemCounterSystem : EntitySystem
	{
		// Token: 0x0600037C RID: 892 RVA: 0x0000EEAF File Offset: 0x0000D0AF
		public override void Initialize()
		{
			base.Initialize();
			base.SubscribeLocalEvent<ItemCounterComponent, EntInsertedIntoContainerMessage>(new ComponentEventHandler<ItemCounterComponent, EntInsertedIntoContainerMessage>(this.CounterEntityInserted), null, null);
			base.SubscribeLocalEvent<ItemCounterComponent, EntRemovedFromContainerMessage>(new ComponentEventHandler<ItemCounterComponent, EntRemovedFromContainerMessage>(this.CounterEntityRemoved), null, null);
		}

		// Token: 0x0600037D RID: 893 RVA: 0x0000EEE0 File Offset: 0x0000D0E0
		private void CounterEntityInserted(EntityUid uid, ItemCounterComponent itemCounter, EntInsertedIntoContainerMessage args)
		{
			AppearanceComponent appearanceComponent;
			if (!this.EntityManager.TryGetComponent<AppearanceComponent>(itemCounter.Owner, ref appearanceComponent))
			{
				return;
			}
			int? count = this.GetCount(args, itemCounter);
			if (count == null)
			{
				return;
			}
			this._appearance.SetData(itemCounter.Owner, StackVisuals.Actual, count, appearanceComponent);
			if (itemCounter.MaxAmount != null)
			{
				this._appearance.SetData(itemCounter.Owner, StackVisuals.MaxCount, itemCounter.MaxAmount, appearanceComponent);
			}
		}

		// Token: 0x0600037E RID: 894 RVA: 0x0000EF68 File Offset: 0x0000D168
		private void CounterEntityRemoved(EntityUid uid, ItemCounterComponent itemCounter, EntRemovedFromContainerMessage args)
		{
			AppearanceComponent appearanceComponent;
			if (!this.EntityManager.TryGetComponent<AppearanceComponent>(itemCounter.Owner, ref appearanceComponent))
			{
				return;
			}
			int? count = this.GetCount(args, itemCounter);
			if (count == null)
			{
				return;
			}
			this._appearance.SetData(itemCounter.Owner, StackVisuals.Actual, count, appearanceComponent);
			if (itemCounter.MaxAmount != null)
			{
				this._appearance.SetData(itemCounter.Owner, StackVisuals.MaxCount, itemCounter.MaxAmount, appearanceComponent);
			}
		}

		// Token: 0x0600037F RID: 895
		protected abstract int? GetCount(ContainerModifiedMessage msg, ItemCounterComponent itemCounter);

		// Token: 0x0400039E RID: 926
		[Dependency]
		private readonly SharedAppearanceSystem _appearance;
	}
}
