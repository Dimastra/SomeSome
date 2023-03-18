using System;
using System.Runtime.CompilerServices;
using Content.Shared.DragDrop;
using Content.Shared.Kitchen.Components;
using Content.Shared.Nutrition.Components;
using Robust.Shared.GameObjects;

namespace Content.Shared.Kitchen
{
	// Token: 0x02000389 RID: 905
	public abstract class SharedKitchenSpikeSystem : EntitySystem
	{
		// Token: 0x06000A82 RID: 2690 RVA: 0x000228B4 File Offset: 0x00020AB4
		public override void Initialize()
		{
			base.Initialize();
			base.SubscribeLocalEvent<SharedKitchenSpikeComponent, CanDropTargetEvent>(new ComponentEventRefHandler<SharedKitchenSpikeComponent, CanDropTargetEvent>(this.OnCanDrop), null, null);
		}

		// Token: 0x06000A83 RID: 2691 RVA: 0x000228D0 File Offset: 0x00020AD0
		[NullableContext(1)]
		private void OnCanDrop(EntityUid uid, SharedKitchenSpikeComponent component, ref CanDropTargetEvent args)
		{
			if (args.Handled)
			{
				return;
			}
			args.Handled = true;
			if (!base.HasComp<ButcherableComponent>(args.Dragged))
			{
				args.CanDrop = false;
				return;
			}
			args.CanDrop = true;
		}
	}
}
