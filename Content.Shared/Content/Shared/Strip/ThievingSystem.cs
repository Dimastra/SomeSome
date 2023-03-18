using System;
using System.Runtime.CompilerServices;
using Content.Shared.Inventory;
using Content.Shared.Strip.Components;
using Robust.Shared.GameObjects;

namespace Content.Shared.Strip
{
	// Token: 0x02000113 RID: 275
	public sealed class ThievingSystem : EntitySystem
	{
		// Token: 0x06000330 RID: 816 RVA: 0x0000E04E File Offset: 0x0000C24E
		public override void Initialize()
		{
			base.Initialize();
			base.SubscribeLocalEvent<ThievingComponent, BeforeStripEvent>(new ComponentEventHandler<ThievingComponent, BeforeStripEvent>(this.OnBeforeStrip), null, null);
			base.SubscribeLocalEvent<ThievingComponent, InventoryRelayedEvent<BeforeStripEvent>>(delegate(EntityUid e, ThievingComponent c, InventoryRelayedEvent<BeforeStripEvent> ev)
			{
				this.OnBeforeStrip(e, c, ev.Args);
			}, null, null);
		}

		// Token: 0x06000331 RID: 817 RVA: 0x0000E07E File Offset: 0x0000C27E
		[NullableContext(1)]
		private void OnBeforeStrip(EntityUid uid, ThievingComponent component, BeforeStripEvent args)
		{
			args.Stealth |= component.Stealthy;
			args.Additive -= component.StripTimeReduction;
		}
	}
}
