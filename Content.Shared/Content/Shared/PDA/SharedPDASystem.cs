using System;
using System.Runtime.CompilerServices;
using Content.Shared.Access.Components;
using Content.Shared.Containers.ItemSlots;
using Robust.Shared.Containers;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;

namespace Content.Shared.PDA
{
	// Token: 0x0200028C RID: 652
	[NullableContext(1)]
	[Nullable(0)]
	public abstract class SharedPDASystem : EntitySystem
	{
		// Token: 0x06000758 RID: 1880 RVA: 0x00019000 File Offset: 0x00017200
		public override void Initialize()
		{
			base.Initialize();
			base.SubscribeLocalEvent<PDAComponent, ComponentInit>(new ComponentEventHandler<PDAComponent, ComponentInit>(this.OnComponentInit), null, null);
			base.SubscribeLocalEvent<PDAComponent, ComponentRemove>(new ComponentEventHandler<PDAComponent, ComponentRemove>(this.OnComponentRemove), null, null);
			base.SubscribeLocalEvent<PDAComponent, EntInsertedIntoContainerMessage>(new ComponentEventHandler<PDAComponent, EntInsertedIntoContainerMessage>(this.OnItemInserted), null, null);
			base.SubscribeLocalEvent<PDAComponent, EntRemovedFromContainerMessage>(new ComponentEventHandler<PDAComponent, EntRemovedFromContainerMessage>(this.OnItemRemoved), null, null);
		}

		// Token: 0x06000759 RID: 1881 RVA: 0x00019068 File Offset: 0x00017268
		protected virtual void OnComponentInit(EntityUid uid, PDAComponent pda, ComponentInit args)
		{
			if (pda.IdCard != null)
			{
				pda.IdSlot.StartingItem = pda.IdCard;
			}
			this.ItemSlotsSystem.AddItemSlot(uid, "PDA-id", pda.IdSlot, null);
			this.ItemSlotsSystem.AddItemSlot(uid, "PDA-pen", pda.PenSlot, null);
			this.UpdatePdaAppearance(uid, pda);
		}

		// Token: 0x0600075A RID: 1882 RVA: 0x000190C6 File Offset: 0x000172C6
		private void OnComponentRemove(EntityUid uid, PDAComponent pda, ComponentRemove args)
		{
			this.ItemSlotsSystem.RemoveItemSlot(uid, pda.IdSlot, null);
			this.ItemSlotsSystem.RemoveItemSlot(uid, pda.PenSlot, null);
		}

		// Token: 0x0600075B RID: 1883 RVA: 0x000190EE File Offset: 0x000172EE
		protected virtual void OnItemInserted(EntityUid uid, PDAComponent pda, EntInsertedIntoContainerMessage args)
		{
			if (args.Container.ID == "PDA-id")
			{
				pda.ContainedID = base.CompOrNull<IdCardComponent>(args.Entity);
			}
			this.UpdatePdaAppearance(uid, pda);
		}

		// Token: 0x0600075C RID: 1884 RVA: 0x00019121 File Offset: 0x00017321
		protected virtual void OnItemRemoved(EntityUid uid, PDAComponent pda, EntRemovedFromContainerMessage args)
		{
			if (args.Container.ID == pda.IdSlot.ID)
			{
				pda.ContainedID = null;
			}
			this.UpdatePdaAppearance(uid, pda);
		}

		// Token: 0x0600075D RID: 1885 RVA: 0x00019150 File Offset: 0x00017350
		private void UpdatePdaAppearance(EntityUid uid, PDAComponent pda)
		{
			AppearanceComponent appearance;
			if (base.TryComp<AppearanceComponent>(pda.Owner, ref appearance))
			{
				this._appearance.SetData(uid, PDAVisuals.IDCardInserted, pda.ContainedID != null, appearance);
			}
		}

		// Token: 0x0400076C RID: 1900
		[Dependency]
		protected readonly ItemSlotsSystem ItemSlotsSystem;

		// Token: 0x0400076D RID: 1901
		[Dependency]
		protected readonly SharedAppearanceSystem _appearance;
	}
}
