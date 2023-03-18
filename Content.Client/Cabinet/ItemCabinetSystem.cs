using System;
using System.Runtime.CompilerServices;
using Content.Shared.Cabinet;
using Robust.Client.GameObjects;
using Robust.Shared.GameObjects;

namespace Content.Client.Cabinet
{
	// Token: 0x02000411 RID: 1041
	public sealed class ItemCabinetSystem : SharedItemCabinetSystem
	{
		// Token: 0x060019A7 RID: 6567 RVA: 0x00093418 File Offset: 0x00091618
		[NullableContext(2)]
		protected override void UpdateAppearance(EntityUid uid, ItemCabinetComponent cabinet = null)
		{
			if (!base.Resolve<ItemCabinetComponent>(uid, ref cabinet, true))
			{
				return;
			}
			SpriteComponent spriteComponent;
			if (!base.TryComp<SpriteComponent>(uid, ref spriteComponent))
			{
				return;
			}
			string text = cabinet.Opened ? cabinet.OpenState : cabinet.ClosedState;
			if (text != null)
			{
				spriteComponent.LayerSetState(ItemCabinetVisualLayers.Door, text);
			}
			spriteComponent.LayerSetVisible(ItemCabinetVisualLayers.ContainsItem, cabinet.CabinetSlot.HasItem);
		}
	}
}
