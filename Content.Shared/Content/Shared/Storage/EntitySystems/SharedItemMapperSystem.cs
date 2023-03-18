using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Content.Shared.Storage.Components;
using Robust.Shared.Containers;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;

namespace Content.Shared.Storage.EntitySystems
{
	// Token: 0x02000131 RID: 305
	[NullableContext(1)]
	[Nullable(0)]
	public abstract class SharedItemMapperSystem : EntitySystem
	{
		// Token: 0x06000381 RID: 897 RVA: 0x0000EFF8 File Offset: 0x0000D1F8
		public override void Initialize()
		{
			base.Initialize();
			base.SubscribeLocalEvent<ItemMapperComponent, ComponentInit>(new ComponentEventHandler<ItemMapperComponent, ComponentInit>(this.InitLayers), null, null);
			base.SubscribeLocalEvent<ItemMapperComponent, EntInsertedIntoContainerMessage>(new ComponentEventHandler<ItemMapperComponent, EntInsertedIntoContainerMessage>(this.MapperEntityInserted), null, null);
			base.SubscribeLocalEvent<ItemMapperComponent, EntRemovedFromContainerMessage>(new ComponentEventHandler<ItemMapperComponent, EntRemovedFromContainerMessage>(this.MapperEntityRemoved), null, null);
		}

		// Token: 0x06000382 RID: 898 RVA: 0x0000F048 File Offset: 0x0000D248
		private void InitLayers(EntityUid uid, ItemMapperComponent component, ComponentInit args)
		{
			foreach (KeyValuePair<string, SharedMapLayerData> keyValuePair in component.MapLayers)
			{
				string text;
				SharedMapLayerData sharedMapLayerData;
				keyValuePair.Deconstruct(out text, out sharedMapLayerData);
				string layerName = text;
				sharedMapLayerData.Layer = layerName;
			}
			AppearanceComponent appearanceComponent;
			if (this.EntityManager.TryGetComponent<AppearanceComponent>(component.Owner, ref appearanceComponent))
			{
				List<string> list = new List<string>(component.MapLayers.Keys);
				this._appearance.SetData(component.Owner, StorageMapVisuals.InitLayers, new ShowLayerData(list), appearanceComponent);
			}
		}

		// Token: 0x06000383 RID: 899 RVA: 0x0000F0F0 File Offset: 0x0000D2F0
		private void MapperEntityRemoved(EntityUid uid, ItemMapperComponent itemMapper, EntRemovedFromContainerMessage args)
		{
			if (itemMapper.ContainerWhitelist != null && !itemMapper.ContainerWhitelist.Contains(args.Container.ID))
			{
				return;
			}
			this.UpdateAppearance(uid, itemMapper, args);
		}

		// Token: 0x06000384 RID: 900 RVA: 0x0000F11C File Offset: 0x0000D31C
		private void MapperEntityInserted(EntityUid uid, ItemMapperComponent itemMapper, EntInsertedIntoContainerMessage args)
		{
			if (itemMapper.ContainerWhitelist != null && !itemMapper.ContainerWhitelist.Contains(args.Container.ID))
			{
				return;
			}
			this.UpdateAppearance(uid, itemMapper, args);
		}

		// Token: 0x06000385 RID: 901 RVA: 0x0000F148 File Offset: 0x0000D348
		private void UpdateAppearance(EntityUid uid, ItemMapperComponent itemMapper, ContainerModifiedMessage message)
		{
			AppearanceComponent appearanceComponent;
			IReadOnlyList<string> containedLayers;
			if (this.EntityManager.TryGetComponent<AppearanceComponent>(itemMapper.Owner, ref appearanceComponent) && this.TryGetLayers(message, itemMapper, out containedLayers))
			{
				this._appearance.SetData(itemMapper.Owner, StorageMapVisuals.LayerChanged, new ShowLayerData(containedLayers), appearanceComponent);
			}
		}

		// Token: 0x06000386 RID: 902 RVA: 0x0000F194 File Offset: 0x0000D394
		private bool TryGetLayers(ContainerModifiedMessage msg, ItemMapperComponent itemMapper, out IReadOnlyList<string> showLayers)
		{
			EntityUid[] containedLayers = this._container.GetAllContainers(msg.Container.Owner, null).Where(delegate(IContainer c)
			{
				HashSet<string> containerWhitelist = itemMapper.ContainerWhitelist;
				return containerWhitelist == null || containerWhitelist.Contains(c.ID);
			}).SelectMany((IContainer cont) => cont.ContainedEntities).ToArray<EntityUid>();
			List<string> list = new List<string>();
			using (Dictionary<string, SharedMapLayerData>.ValueCollection.Enumerator enumerator = itemMapper.MapLayers.Values.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					SharedMapLayerData mapLayerData = enumerator.Current;
					int count = containedLayers.Count((EntityUid uid) => mapLayerData.ServerWhitelist.IsValid(uid, null));
					if (count >= mapLayerData.MinCount && count <= mapLayerData.MaxCount)
					{
						list.Add(mapLayerData.Layer);
					}
				}
			}
			showLayers = list;
			return true;
		}

		// Token: 0x0400039F RID: 927
		[Dependency]
		private readonly SharedAppearanceSystem _appearance;

		// Token: 0x040003A0 RID: 928
		[Dependency]
		private readonly SharedContainerSystem _container;
	}
}
