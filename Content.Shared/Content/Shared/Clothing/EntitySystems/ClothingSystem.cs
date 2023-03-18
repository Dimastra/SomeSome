using System;
using System.Runtime.CompilerServices;
using Content.Shared.Clothing.Components;
using Content.Shared.Humanoid;
using Content.Shared.Inventory;
using Content.Shared.Inventory.Events;
using Content.Shared.Item;
using Content.Shared.Tag;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.IoC;

namespace Content.Shared.Clothing.EntitySystems
{
	// Token: 0x020005AB RID: 1451
	[NullableContext(1)]
	[Nullable(0)]
	public abstract class ClothingSystem : EntitySystem
	{
		// Token: 0x060011B3 RID: 4531 RVA: 0x00039F30 File Offset: 0x00038130
		public override void Initialize()
		{
			base.Initialize();
			base.SubscribeLocalEvent<ClothingComponent, ComponentGetState>(new ComponentEventRefHandler<ClothingComponent, ComponentGetState>(this.OnGetState), null, null);
			base.SubscribeLocalEvent<ClothingComponent, ComponentHandleState>(new ComponentEventRefHandler<ClothingComponent, ComponentHandleState>(this.OnHandleState), null, null);
			base.SubscribeLocalEvent<ClothingComponent, GotEquippedEvent>(new ComponentEventHandler<ClothingComponent, GotEquippedEvent>(this.OnGotEquipped), null, null);
			base.SubscribeLocalEvent<ClothingComponent, GotUnequippedEvent>(new ComponentEventHandler<ClothingComponent, GotUnequippedEvent>(this.OnGotUnequipped), null, null);
		}

		// Token: 0x060011B4 RID: 4532 RVA: 0x00039F98 File Offset: 0x00038198
		protected virtual void OnGotEquipped(EntityUid uid, ClothingComponent component, GotEquippedEvent args)
		{
			component.InSlot = args.Slot;
			if (args.Slot == "head" && this._tagSystem.HasTag(args.Equipment, "HidesHair"))
			{
				this._humanoidSystem.SetLayerVisibility(args.Equipee, HumanoidVisualLayers.Hair, false, false, null);
			}
		}

		// Token: 0x060011B5 RID: 4533 RVA: 0x00039FF0 File Offset: 0x000381F0
		protected virtual void OnGotUnequipped(EntityUid uid, ClothingComponent component, GotUnequippedEvent args)
		{
			component.InSlot = null;
			if (args.Slot == "head" && this._tagSystem.HasTag(args.Equipment, "HidesHair"))
			{
				this._humanoidSystem.SetLayerVisibility(args.Equipee, HumanoidVisualLayers.Hair, true, false, null);
			}
		}

		// Token: 0x060011B6 RID: 4534 RVA: 0x0003A043 File Offset: 0x00038243
		private void OnGetState(EntityUid uid, ClothingComponent component, ref ComponentGetState args)
		{
			args.State = new ClothingComponentState(component.EquippedPrefix);
		}

		// Token: 0x060011B7 RID: 4535 RVA: 0x0003A058 File Offset: 0x00038258
		private void OnHandleState(EntityUid uid, ClothingComponent component, ref ComponentHandleState args)
		{
			ClothingComponentState state = args.Current as ClothingComponentState;
			if (state != null)
			{
				this.SetEquippedPrefix(uid, state.EquippedPrefix, component);
			}
		}

		// Token: 0x060011B8 RID: 4536 RVA: 0x0003A082 File Offset: 0x00038282
		[NullableContext(2)]
		public void SetEquippedPrefix(EntityUid uid, string prefix, ClothingComponent clothing = null)
		{
			if (!base.Resolve<ClothingComponent>(uid, ref clothing, false))
			{
				return;
			}
			if (clothing.EquippedPrefix == prefix)
			{
				return;
			}
			clothing.EquippedPrefix = prefix;
			this._itemSys.VisualsChanged(uid);
			base.Dirty(clothing, null);
		}

		// Token: 0x060011B9 RID: 4537 RVA: 0x0003A0BB File Offset: 0x000382BB
		[NullableContext(2)]
		public void SetSlots(EntityUid uid, SlotFlags slots, ClothingComponent clothing = null)
		{
			if (!base.Resolve<ClothingComponent>(uid, ref clothing, true))
			{
				return;
			}
			clothing.Slots = slots;
			base.Dirty(clothing, null);
		}

		// Token: 0x060011BA RID: 4538 RVA: 0x0003A0DC File Offset: 0x000382DC
		public void CopyVisuals(EntityUid uid, ClothingComponent otherClothing, [Nullable(2)] ClothingComponent clothing = null)
		{
			if (!base.Resolve<ClothingComponent>(uid, ref clothing, true))
			{
				return;
			}
			clothing.ClothingVisuals = otherClothing.ClothingVisuals;
			clothing.EquippedPrefix = otherClothing.EquippedPrefix;
			clothing.RsiPath = otherClothing.RsiPath;
			clothing.FemaleMask = otherClothing.FemaleMask;
			this._itemSys.VisualsChanged(uid);
			base.Dirty(clothing, null);
		}

		// Token: 0x04001057 RID: 4183
		[Dependency]
		private readonly SharedItemSystem _itemSys;

		// Token: 0x04001058 RID: 4184
		[Dependency]
		private readonly SharedHumanoidAppearanceSystem _humanoidSystem;

		// Token: 0x04001059 RID: 4185
		[Dependency]
		private readonly TagSystem _tagSystem;
	}
}
