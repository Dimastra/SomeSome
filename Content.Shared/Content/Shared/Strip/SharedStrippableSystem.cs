using System;
using System.Runtime.CompilerServices;
using Content.Shared.DragDrop;
using Content.Shared.Hands.Components;
using Content.Shared.Strip.Components;
using Robust.Shared.GameObjects;

namespace Content.Shared.Strip
{
	// Token: 0x02000112 RID: 274
	[NullableContext(1)]
	[Nullable(0)]
	public abstract class SharedStrippableSystem : EntitySystem
	{
		// Token: 0x0600032A RID: 810 RVA: 0x0000DF2C File Offset: 0x0000C12C
		public override void Initialize()
		{
			base.Initialize();
			base.SubscribeLocalEvent<StrippingComponent, CanDropTargetEvent>(new ComponentEventRefHandler<StrippingComponent, CanDropTargetEvent>(this.OnCanDropOn), null, null);
			base.SubscribeLocalEvent<StrippableComponent, CanDropDraggedEvent>(new ComponentEventRefHandler<StrippableComponent, CanDropDraggedEvent>(this.OnCanDrop), null, null);
			base.SubscribeLocalEvent<StrippableComponent, DragDropDraggedEvent>(new ComponentEventRefHandler<StrippableComponent, DragDropDraggedEvent>(this.OnDragDrop), null, null);
		}

		// Token: 0x0600032B RID: 811 RVA: 0x0000DF7B File Offset: 0x0000C17B
		private void OnDragDrop(EntityUid uid, StrippableComponent component, ref DragDropDraggedEvent args)
		{
			if (args.Handled || args.Target != args.User)
			{
				return;
			}
			this.StartOpeningStripper(args.User, component, false);
			args.Handled = true;
		}

		// Token: 0x0600032C RID: 812 RVA: 0x0000DFAE File Offset: 0x0000C1AE
		public virtual void StartOpeningStripper(EntityUid user, StrippableComponent component, bool openInCombat = false)
		{
		}

		// Token: 0x0600032D RID: 813 RVA: 0x0000DFB0 File Offset: 0x0000C1B0
		private void OnCanDropOn(EntityUid uid, StrippingComponent component, ref CanDropTargetEvent args)
		{
			args.Handled = true;
			args.CanDrop |= (uid == args.User && base.HasComp<StrippableComponent>(args.Dragged) && base.HasComp<SharedHandsComponent>(args.User));
		}

		// Token: 0x0600032E RID: 814 RVA: 0x0000DFF0 File Offset: 0x0000C1F0
		private void OnCanDrop(EntityUid uid, StrippableComponent component, ref CanDropDraggedEvent args)
		{
			args.CanDrop |= (args.Target == args.User && base.HasComp<StrippingComponent>(args.User) && base.HasComp<SharedHandsComponent>(args.User));
			if (args.CanDrop)
			{
				args.Handled = true;
			}
		}
	}
}
