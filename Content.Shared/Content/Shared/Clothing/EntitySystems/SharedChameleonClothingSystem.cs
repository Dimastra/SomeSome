using System;
using System.Runtime.CompilerServices;
using Content.Shared.Clothing.Components;
using Content.Shared.Inventory;
using Content.Shared.Inventory.Events;
using Content.Shared.Item;
using Content.Shared.Tag;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Prototypes;

namespace Content.Shared.Clothing.EntitySystems
{
	// Token: 0x020005AC RID: 1452
	[NullableContext(1)]
	[Nullable(0)]
	public abstract class SharedChameleonClothingSystem : EntitySystem
	{
		// Token: 0x060011BC RID: 4540 RVA: 0x0003A142 File Offset: 0x00038342
		public override void Initialize()
		{
			base.Initialize();
			base.SubscribeLocalEvent<ChameleonClothingComponent, GotEquippedEvent>(new ComponentEventHandler<ChameleonClothingComponent, GotEquippedEvent>(this.OnGotEquipped), null, null);
			base.SubscribeLocalEvent<ChameleonClothingComponent, GotUnequippedEvent>(new ComponentEventHandler<ChameleonClothingComponent, GotUnequippedEvent>(this.OnGotUnequipped), null, null);
		}

		// Token: 0x060011BD RID: 4541 RVA: 0x0003A172 File Offset: 0x00038372
		private void OnGotEquipped(EntityUid uid, ChameleonClothingComponent component, GotEquippedEvent args)
		{
			component.User = new EntityUid?(args.Equipee);
		}

		// Token: 0x060011BE RID: 4542 RVA: 0x0003A185 File Offset: 0x00038385
		private void OnGotUnequipped(EntityUid uid, ChameleonClothingComponent component, GotUnequippedEvent args)
		{
			component.User = null;
		}

		// Token: 0x060011BF RID: 4543 RVA: 0x0003A194 File Offset: 0x00038394
		protected void UpdateVisuals(EntityUid uid, ChameleonClothingComponent component)
		{
			EntityPrototype proto;
			if (string.IsNullOrEmpty(component.SelectedId) || !this._proto.TryIndex<EntityPrototype>(component.SelectedId, ref proto))
			{
				return;
			}
			this.UpdateSprite(uid, proto);
			MetaDataComponent metaDataComponent = base.MetaData(uid);
			metaDataComponent.EntityName = proto.Name;
			metaDataComponent.EntityDescription = proto.Description;
			ItemComponent item;
			ItemComponent otherItem;
			if (base.TryComp<ItemComponent>(uid, ref item) && proto.TryGetComponent<ItemComponent>(ref otherItem, this._factory))
			{
				this._itemSystem.CopyVisuals(uid, otherItem, item);
			}
			ClothingComponent clothing;
			ClothingComponent otherClothing;
			if (base.TryComp<ClothingComponent>(uid, ref clothing) && proto.TryGetComponent<ClothingComponent>("Clothing", ref otherClothing))
			{
				this._clothingSystem.CopyVisuals(uid, otherClothing, clothing);
			}
		}

		// Token: 0x060011C0 RID: 4544 RVA: 0x0003A23C File Offset: 0x0003843C
		protected virtual void UpdateSprite(EntityUid uid, EntityPrototype proto)
		{
		}

		// Token: 0x060011C1 RID: 4545 RVA: 0x0003A240 File Offset: 0x00038440
		public bool IsValidTarget(EntityPrototype proto, SlotFlags chameleonSlot = SlotFlags.NONE)
		{
			TagComponent tags;
			ClothingComponent clothing;
			return !proto.Abstract && !proto.NoSpawn && proto.TryGetComponent<TagComponent>(ref tags, this._factory) && tags.Tags.Contains("WhitelistChameleon") && proto.TryGetComponent<ClothingComponent>("Clothing", ref clothing) && clothing.Slots.HasFlag(chameleonSlot);
		}

		// Token: 0x0400105A RID: 4186
		[Dependency]
		private readonly IComponentFactory _factory;

		// Token: 0x0400105B RID: 4187
		[Dependency]
		private readonly IPrototypeManager _proto;

		// Token: 0x0400105C RID: 4188
		[Dependency]
		private readonly SharedItemSystem _itemSystem;

		// Token: 0x0400105D RID: 4189
		[Dependency]
		private readonly ClothingSystem _clothingSystem;
	}
}
