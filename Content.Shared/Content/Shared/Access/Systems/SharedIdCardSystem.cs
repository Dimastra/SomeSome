using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using Content.Shared.Access.Components;
using Content.Shared.Hands.Components;
using Content.Shared.Inventory;
using Content.Shared.PDA;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.IoC;

namespace Content.Shared.Access.Systems
{
	// Token: 0x02000778 RID: 1912
	[NullableContext(1)]
	[Nullable(0)]
	public abstract class SharedIdCardSystem : EntitySystem
	{
		// Token: 0x060017A1 RID: 6049 RVA: 0x0004CCA2 File Offset: 0x0004AEA2
		public override void Initialize()
		{
			base.Initialize();
			base.SubscribeLocalEvent<IdCardComponent, ComponentGetState>(new ComponentEventRefHandler<IdCardComponent, ComponentGetState>(this.OnComponentGetState), null, null);
			base.SubscribeLocalEvent<IdCardComponent, ComponentHandleState>(new ComponentEventRefHandler<IdCardComponent, ComponentHandleState>(this.OnComponentHandleState), null, null);
		}

		// Token: 0x060017A2 RID: 6050 RVA: 0x0004CCD2 File Offset: 0x0004AED2
		private void OnComponentGetState(EntityUid uid, IdCardComponent component, ref ComponentGetState args)
		{
			args.State = new IdCardComponentState(component.FullName, component.JobTitle);
		}

		// Token: 0x060017A3 RID: 6051 RVA: 0x0004CCEC File Offset: 0x0004AEEC
		private void OnComponentHandleState(EntityUid uid, IdCardComponent component, ref ComponentHandleState args)
		{
			IdCardComponentState state = args.Current as IdCardComponentState;
			if (state != null)
			{
				component.FullName = state.FullName;
				component.JobTitle = state.JobTitle;
			}
		}

		// Token: 0x060017A4 RID: 6052 RVA: 0x0004CD20 File Offset: 0x0004AF20
		[NullableContext(2)]
		public bool TryFindIdCard(EntityUid uid, [NotNullWhen(true)] out IdCardComponent idCard)
		{
			SharedHandsComponent hands;
			if (this.EntityManager.TryGetComponent<SharedHandsComponent>(uid, ref hands))
			{
				EntityUid? activeHandEntity = hands.ActiveHandEntity;
				if (activeHandEntity != null)
				{
					EntityUid heldItem = activeHandEntity.GetValueOrDefault();
					if (this.TryGetIdCard(heldItem, out idCard))
					{
						return true;
					}
				}
			}
			EntityUid? idUid;
			return this.TryGetIdCard(uid, out idCard) || (this._inventorySystem.TryGetSlotEntity(uid, "id", out idUid, null, null) && this.TryGetIdCard(idUid.Value, out idCard));
		}

		// Token: 0x060017A5 RID: 6053 RVA: 0x0004CD98 File Offset: 0x0004AF98
		[NullableContext(2)]
		public bool TryGetIdCard(EntityUid uid, [NotNullWhen(true)] out IdCardComponent idCard)
		{
			if (this.EntityManager.TryGetComponent<IdCardComponent>(uid, ref idCard))
			{
				return true;
			}
			PDAComponent pda;
			if (this.EntityManager.TryGetComponent<PDAComponent>(uid, ref pda) && pda.ContainedID != null)
			{
				idCard = pda.ContainedID;
				return true;
			}
			return false;
		}

		// Token: 0x04001756 RID: 5974
		[Dependency]
		private readonly InventorySystem _inventorySystem;
	}
}
