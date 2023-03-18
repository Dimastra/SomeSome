using System;
using System.Runtime.CompilerServices;
using Content.Client.Inventory;
using Content.Shared.Cuffs.Components;
using Content.Shared.Ensnaring.Components;
using Content.Shared.Hands;
using Content.Shared.Inventory.Events;
using Content.Shared.Strip;
using Content.Shared.Strip.Components;
using Robust.Client.GameObjects;
using Robust.Shared.GameObjects;

namespace Content.Client.Strip
{
	// Token: 0x02000117 RID: 279
	public sealed class StrippableSystem : SharedStrippableSystem
	{
		// Token: 0x060007BA RID: 1978 RVA: 0x0002CE54 File Offset: 0x0002B054
		public override void Initialize()
		{
			base.Initialize();
			base.SubscribeLocalEvent<StrippableComponent, CuffedStateChangeEvent>(new ComponentEventRefHandler<StrippableComponent, CuffedStateChangeEvent>(this.OnCuffStateChange), null, null);
			base.SubscribeLocalEvent<StrippableComponent, DidEquipEvent>(new ComponentEventHandler<StrippableComponent, DidEquipEvent>(this.UpdateUi), null, null);
			base.SubscribeLocalEvent<StrippableComponent, DidUnequipEvent>(new ComponentEventHandler<StrippableComponent, DidUnequipEvent>(this.UpdateUi), null, null);
			base.SubscribeLocalEvent<StrippableComponent, DidEquipHandEvent>(new ComponentEventHandler<StrippableComponent, DidEquipHandEvent>(this.UpdateUi), null, null);
			base.SubscribeLocalEvent<StrippableComponent, DidUnequipHandEvent>(new ComponentEventHandler<StrippableComponent, DidUnequipHandEvent>(this.UpdateUi), null, null);
			base.SubscribeLocalEvent<StrippableComponent, EnsnaredChangedEvent>(new ComponentEventHandler<StrippableComponent, EnsnaredChangedEvent>(this.UpdateUi), null, null);
		}

		// Token: 0x060007BB RID: 1979 RVA: 0x0002CEDF File Offset: 0x0002B0DF
		[NullableContext(1)]
		private void OnCuffStateChange(EntityUid uid, StrippableComponent component, ref CuffedStateChangeEvent args)
		{
			this.UpdateUi(uid, component, null);
		}

		// Token: 0x060007BC RID: 1980 RVA: 0x0002CEEC File Offset: 0x0002B0EC
		[NullableContext(2)]
		public void UpdateUi(EntityUid uid, StrippableComponent component = null, EntityEventArgs args = null)
		{
			ClientUserInterfaceComponent clientUserInterfaceComponent;
			if (!base.TryComp<ClientUserInterfaceComponent>(uid, ref clientUserInterfaceComponent))
			{
				return;
			}
			foreach (BoundUserInterface boundUserInterface in clientUserInterfaceComponent.Interfaces)
			{
				StrippableBoundUserInterface strippableBoundUserInterface = boundUserInterface as StrippableBoundUserInterface;
				if (strippableBoundUserInterface != null)
				{
					strippableBoundUserInterface.DirtyMenu();
				}
			}
		}
	}
}
