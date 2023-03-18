using System;
using System.Runtime.CompilerServices;
using Content.Server.Storage.Components;
using Content.Shared.Rounding;
using Content.Shared.Storage.Components;
using Robust.Shared.Containers;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;

namespace Content.Server.Storage.EntitySystems
{
	// Token: 0x02000165 RID: 357
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class StorageFillVisualizerSystem : EntitySystem
	{
		// Token: 0x060006E8 RID: 1768 RVA: 0x000223D8 File Offset: 0x000205D8
		public override void Initialize()
		{
			base.Initialize();
			base.SubscribeLocalEvent<StorageFillVisualizerComponent, ComponentInit>(new ComponentEventHandler<StorageFillVisualizerComponent, ComponentInit>(this.OnInit), null, null);
			base.SubscribeLocalEvent<StorageFillVisualizerComponent, EntInsertedIntoContainerMessage>(new ComponentEventHandler<StorageFillVisualizerComponent, EntInsertedIntoContainerMessage>(this.OnInserted), null, null);
			base.SubscribeLocalEvent<StorageFillVisualizerComponent, EntRemovedFromContainerMessage>(new ComponentEventHandler<StorageFillVisualizerComponent, EntRemovedFromContainerMessage>(this.OnRemoved), null, null);
		}

		// Token: 0x060006E9 RID: 1769 RVA: 0x00022427 File Offset: 0x00020627
		private void OnInit(EntityUid uid, StorageFillVisualizerComponent component, ComponentInit args)
		{
			this.UpdateAppearance(uid, null, null, component);
		}

		// Token: 0x060006EA RID: 1770 RVA: 0x00022433 File Offset: 0x00020633
		private void OnInserted(EntityUid uid, StorageFillVisualizerComponent component, EntInsertedIntoContainerMessage args)
		{
			this.UpdateAppearance(uid, null, null, component);
		}

		// Token: 0x060006EB RID: 1771 RVA: 0x0002243F File Offset: 0x0002063F
		private void OnRemoved(EntityUid uid, StorageFillVisualizerComponent component, EntRemovedFromContainerMessage args)
		{
			this.UpdateAppearance(uid, null, null, component);
		}

		// Token: 0x060006EC RID: 1772 RVA: 0x0002244C File Offset: 0x0002064C
		[NullableContext(2)]
		private void UpdateAppearance(EntityUid uid, ServerStorageComponent storage = null, AppearanceComponent appearance = null, StorageFillVisualizerComponent component = null)
		{
			if (!base.Resolve<ServerStorageComponent, AppearanceComponent, StorageFillVisualizerComponent>(uid, ref storage, ref appearance, ref component, false))
			{
				return;
			}
			if (component.MaxFillLevels < 1)
			{
				return;
			}
			int level = ContentHelpers.RoundToEqualLevels((double)storage.StorageUsed, (double)storage.StorageCapacityMax, component.MaxFillLevels);
			this._appearance.SetData(uid, StorageFillVisuals.FillLevel, level, appearance);
		}

		// Token: 0x040003F8 RID: 1016
		[Dependency]
		private readonly SharedAppearanceSystem _appearance;
	}
}
