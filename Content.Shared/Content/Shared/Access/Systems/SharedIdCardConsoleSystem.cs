using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Content.Shared.Access.Components;
using Content.Shared.Containers.ItemSlots;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.IoC;
using Robust.Shared.Serialization;

namespace Content.Shared.Access.Systems
{
	// Token: 0x02000777 RID: 1911
	[NullableContext(1)]
	[Nullable(0)]
	public abstract class SharedIdCardConsoleSystem : EntitySystem
	{
		// Token: 0x0600179B RID: 6043 RVA: 0x0004CBA0 File Offset: 0x0004ADA0
		public override void Initialize()
		{
			base.Initialize();
			base.SubscribeLocalEvent<SharedIdCardConsoleComponent, ComponentInit>(new ComponentEventHandler<SharedIdCardConsoleComponent, ComponentInit>(this.OnComponentInit), null, null);
			base.SubscribeLocalEvent<SharedIdCardConsoleComponent, ComponentRemove>(new ComponentEventHandler<SharedIdCardConsoleComponent, ComponentRemove>(this.OnComponentRemove), null, null);
			base.SubscribeLocalEvent<SharedIdCardConsoleComponent, ComponentGetState>(new ComponentEventRefHandler<SharedIdCardConsoleComponent, ComponentGetState>(this.OnGetState), null, null);
			base.SubscribeLocalEvent<SharedIdCardConsoleComponent, ComponentHandleState>(new ComponentEventRefHandler<SharedIdCardConsoleComponent, ComponentHandleState>(this.OnHandleState), null, null);
		}

		// Token: 0x0600179C RID: 6044 RVA: 0x0004CC04 File Offset: 0x0004AE04
		private void OnHandleState(EntityUid uid, SharedIdCardConsoleComponent component, ref ComponentHandleState args)
		{
			SharedIdCardConsoleSystem.IdCardConsoleComponentState state = args.Current as SharedIdCardConsoleSystem.IdCardConsoleComponentState;
			if (state == null)
			{
				return;
			}
			component.AccessLevels = state.AccessLevels;
		}

		// Token: 0x0600179D RID: 6045 RVA: 0x0004CC2D File Offset: 0x0004AE2D
		private void OnGetState(EntityUid uid, SharedIdCardConsoleComponent component, ref ComponentGetState args)
		{
			args.State = new SharedIdCardConsoleSystem.IdCardConsoleComponentState(component.AccessLevels);
		}

		// Token: 0x0600179E RID: 6046 RVA: 0x0004CC40 File Offset: 0x0004AE40
		private void OnComponentInit(EntityUid uid, SharedIdCardConsoleComponent component, ComponentInit args)
		{
			this._itemSlotsSystem.AddItemSlot(uid, SharedIdCardConsoleComponent.PrivilegedIdCardSlotId, component.PrivilegedIdSlot, null);
			this._itemSlotsSystem.AddItemSlot(uid, SharedIdCardConsoleComponent.TargetIdCardSlotId, component.TargetIdSlot, null);
		}

		// Token: 0x0600179F RID: 6047 RVA: 0x0004CC72 File Offset: 0x0004AE72
		private void OnComponentRemove(EntityUid uid, SharedIdCardConsoleComponent component, ComponentRemove args)
		{
			this._itemSlotsSystem.RemoveItemSlot(uid, component.PrivilegedIdSlot, null);
			this._itemSlotsSystem.RemoveItemSlot(uid, component.TargetIdSlot, null);
		}

		// Token: 0x04001754 RID: 5972
		[Dependency]
		private readonly ItemSlotsSystem _itemSlotsSystem;

		// Token: 0x04001755 RID: 5973
		public const string Sawmill = "idconsole";

		// Token: 0x020008AC RID: 2220
		[Nullable(0)]
		[NetSerializable]
		[Serializable]
		private sealed class IdCardConsoleComponentState : ComponentState
		{
			// Token: 0x06001A48 RID: 6728 RVA: 0x00051FCD File Offset: 0x000501CD
			public IdCardConsoleComponentState(List<string> accessLevels)
			{
				this.AccessLevels = accessLevels;
			}

			// Token: 0x04001AAF RID: 6831
			public List<string> AccessLevels;
		}
	}
}
