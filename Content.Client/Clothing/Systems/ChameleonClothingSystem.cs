using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Content.Shared.Clothing.Components;
using Content.Shared.Clothing.EntitySystems;
using Content.Shared.Inventory;
using Robust.Client.GameObjects;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.IoC;
using Robust.Shared.Prototypes;

namespace Content.Client.Clothing.Systems
{
	// Token: 0x020003BA RID: 954
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class ChameleonClothingSystem : SharedChameleonClothingSystem
	{
		// Token: 0x060017AD RID: 6061 RVA: 0x00087EDA File Offset: 0x000860DA
		public override void Initialize()
		{
			base.Initialize();
			base.SubscribeLocalEvent<ChameleonClothingComponent, ComponentHandleState>(new ComponentEventRefHandler<ChameleonClothingComponent, ComponentHandleState>(this.HandleState), null, null);
			this.PrepareAllVariants();
			this._proto.PrototypesReloaded += this.OnProtoReloaded;
		}

		// Token: 0x060017AE RID: 6062 RVA: 0x00087F13 File Offset: 0x00086113
		public override void Shutdown()
		{
			base.Shutdown();
			this._proto.PrototypesReloaded -= this.OnProtoReloaded;
		}

		// Token: 0x060017AF RID: 6063 RVA: 0x00087F32 File Offset: 0x00086132
		private void OnProtoReloaded(PrototypesReloadedEventArgs _)
		{
			this.PrepareAllVariants();
		}

		// Token: 0x060017B0 RID: 6064 RVA: 0x00087F3C File Offset: 0x0008613C
		private void HandleState(EntityUid uid, ChameleonClothingComponent component, ref ComponentHandleState args)
		{
			ChameleonClothingComponentState chameleonClothingComponentState = args.Current as ChameleonClothingComponentState;
			if (chameleonClothingComponentState == null)
			{
				return;
			}
			component.SelectedId = chameleonClothingComponentState.SelectedId;
			base.UpdateVisuals(uid, component);
		}

		// Token: 0x060017B1 RID: 6065 RVA: 0x00087F70 File Offset: 0x00086170
		protected override void UpdateSprite(EntityUid uid, EntityPrototype proto)
		{
			base.UpdateSprite(uid, proto);
			SpriteComponent spriteComponent;
			SpriteComponent spriteComponent2;
			if (base.TryComp<SpriteComponent>(uid, ref spriteComponent) && proto.TryGetComponent<SpriteComponent>(ref spriteComponent2, this._factory))
			{
				spriteComponent.CopyFrom(spriteComponent2);
			}
		}

		// Token: 0x060017B2 RID: 6066 RVA: 0x00087FA8 File Offset: 0x000861A8
		public IEnumerable<string> GetValidTargets(SlotFlags slot)
		{
			HashSet<string> hashSet = new HashSet<string>();
			foreach (SlotFlags slotFlags in this._data.Keys)
			{
				if (slot.HasFlag(slotFlags))
				{
					hashSet.UnionWith(this._data[slotFlags]);
				}
			}
			return hashSet;
		}

		// Token: 0x060017B3 RID: 6067 RVA: 0x00088028 File Offset: 0x00086228
		private void PrepareAllVariants()
		{
			this._data.Clear();
			foreach (EntityPrototype entityPrototype in this._proto.EnumeratePrototypes<EntityPrototype>())
			{
				ClothingComponent clothingComponent;
				if (base.IsValidTarget(entityPrototype, SlotFlags.NONE) && entityPrototype.TryGetComponent<ClothingComponent>(ref clothingComponent, this._factory))
				{
					foreach (SlotFlags slotFlags in ChameleonClothingSystem.Slots)
					{
						if (clothingComponent.Slots.HasFlag(slotFlags))
						{
							if (!this._data.ContainsKey(slotFlags))
							{
								this._data.Add(slotFlags, new List<string>());
							}
							this._data[slotFlags].Add(entityPrototype.ID);
						}
					}
				}
			}
		}

		// Token: 0x060017B5 RID: 6069 RVA: 0x00088127 File Offset: 0x00086327
		// Note: this type is marked as 'beforefieldinit'.
		static ChameleonClothingSystem()
		{
			SlotFlags[] array = new SlotFlags[3];
			array[0] = SlotFlags.All;
			array[1] = SlotFlags.PREVENTEQUIP;
			ChameleonClothingSystem.IgnoredSlots = array;
			ChameleonClothingSystem.Slots = Enum.GetValues<SlotFlags>().Except(ChameleonClothingSystem.IgnoredSlots).ToArray<SlotFlags>();
		}

		// Token: 0x04000C16 RID: 3094
		[Dependency]
		private readonly IPrototypeManager _proto;

		// Token: 0x04000C17 RID: 3095
		[Dependency]
		private readonly IComponentFactory _factory;

		// Token: 0x04000C18 RID: 3096
		private static readonly SlotFlags[] IgnoredSlots;

		// Token: 0x04000C19 RID: 3097
		private static readonly SlotFlags[] Slots;

		// Token: 0x04000C1A RID: 3098
		private readonly Dictionary<SlotFlags, List<string>> _data = new Dictionary<SlotFlags, List<string>>();
	}
}
