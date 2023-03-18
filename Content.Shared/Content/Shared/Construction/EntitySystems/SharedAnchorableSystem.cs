using System;
using System.Runtime.CompilerServices;
using Content.Shared.Construction.Components;
using Content.Shared.Containers.ItemSlots;
using Content.Shared.Interaction;
using Content.Shared.Pulling.Components;
using Content.Shared.Tools.Components;
using Robust.Shared.GameObjects;

namespace Content.Shared.Construction.EntitySystems
{
	// Token: 0x0200057D RID: 1405
	public abstract class SharedAnchorableSystem : EntitySystem
	{
		// Token: 0x06001143 RID: 4419 RVA: 0x00038C41 File Offset: 0x00036E41
		public override void Initialize()
		{
			base.Initialize();
			base.SubscribeLocalEvent<AnchorableComponent, InteractUsingEvent>(new ComponentEventHandler<AnchorableComponent, InteractUsingEvent>(this.OnInteractUsing), new Type[]
			{
				typeof(ItemSlotsSystem)
			}, new Type[]
			{
				typeof(SharedConstructionSystem)
			});
		}

		// Token: 0x06001144 RID: 4420 RVA: 0x00038C84 File Offset: 0x00036E84
		[NullableContext(1)]
		private void OnInteractUsing(EntityUid uid, AnchorableComponent anchorable, InteractUsingEvent args)
		{
			if (args.Handled)
			{
				return;
			}
			ToolComponent usedTool;
			if (!base.TryComp<ToolComponent>(args.Used, ref usedTool) || !usedTool.Qualities.Contains(anchorable.Tool))
			{
				return;
			}
			args.Handled = true;
			this.TryToggleAnchor(uid, args.User, args.Used, anchorable, null, null, usedTool);
		}

		// Token: 0x06001145 RID: 4421 RVA: 0x00038CDC File Offset: 0x00036EDC
		[NullableContext(2)]
		public virtual void TryToggleAnchor(EntityUid uid, EntityUid userUid, EntityUid usingUid, AnchorableComponent anchorable = null, TransformComponent transform = null, SharedPullableComponent pullable = null, ToolComponent usingTool = null)
		{
		}
	}
}
