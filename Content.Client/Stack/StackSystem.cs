using System;
using System.Runtime.CompilerServices;
using Content.Client.Items;
using Content.Shared.Stacks;
using Robust.Shared.GameObjects;

namespace Content.Client.Stack
{
	// Token: 0x02000134 RID: 308
	public sealed class StackSystem : SharedStackSystem
	{
		// Token: 0x06000846 RID: 2118 RVA: 0x000301DE File Offset: 0x0002E3DE
		public override void Initialize()
		{
			base.Initialize();
			base.SubscribeLocalEvent<StackComponent, ItemStatusCollectMessage>(new ComponentEventHandler<StackComponent, ItemStatusCollectMessage>(this.OnItemStatus), null, null);
		}

		// Token: 0x06000847 RID: 2119 RVA: 0x000301FA File Offset: 0x0002E3FA
		[NullableContext(1)]
		private void OnItemStatus(EntityUid uid, StackComponent component, ItemStatusCollectMessage args)
		{
			args.Controls.Add(new StackStatusControl(component));
		}

		// Token: 0x06000848 RID: 2120 RVA: 0x00030210 File Offset: 0x0002E410
		[NullableContext(2)]
		public override void SetCount(EntityUid uid, int amount, StackComponent component = null)
		{
			if (!base.Resolve<StackComponent>(uid, ref component, true))
			{
				return;
			}
			base.SetCount(uid, amount, component);
			if (component.Count <= 0)
			{
				this.Xform.DetachParentToNull(uid, base.Transform(uid));
				return;
			}
			if (component != null)
			{
				StackComponent stackComponent = component;
				stackComponent.UiUpdateNeeded = true;
			}
		}
	}
}
