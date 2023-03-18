using System;
using System.Runtime.CompilerServices;
using Content.Shared.IdentityManagement.Components;
using Content.Shared.Inventory;
using Robust.Shared.Containers;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;

namespace Content.Shared.IdentityManagement
{
	// Token: 0x020003FD RID: 1021
	[NullableContext(1)]
	[Nullable(0)]
	public abstract class SharedIdentitySystem : EntitySystem
	{
		// Token: 0x06000BEB RID: 3051 RVA: 0x00027558 File Offset: 0x00025758
		public override void Initialize()
		{
			base.Initialize();
			base.SubscribeLocalEvent<IdentityComponent, ComponentInit>(new ComponentEventHandler<IdentityComponent, ComponentInit>(this.OnComponentInit), null, null);
			base.SubscribeLocalEvent<IdentityBlockerComponent, SeeIdentityAttemptEvent>(new ComponentEventHandler<IdentityBlockerComponent, SeeIdentityAttemptEvent>(this.OnSeeIdentity), null, null);
			base.SubscribeLocalEvent<IdentityBlockerComponent, InventoryRelayedEvent<SeeIdentityAttemptEvent>>(delegate(EntityUid e, IdentityBlockerComponent c, InventoryRelayedEvent<SeeIdentityAttemptEvent> ev)
			{
				this.OnSeeIdentity(e, c, ev.Args);
			}, null, null);
		}

		// Token: 0x06000BEC RID: 3052 RVA: 0x000275A8 File Offset: 0x000257A8
		private void OnSeeIdentity(EntityUid uid, IdentityBlockerComponent component, SeeIdentityAttemptEvent args)
		{
			if (component.Enabled)
			{
				args.Cancel();
			}
		}

		// Token: 0x06000BED RID: 3053 RVA: 0x000275B8 File Offset: 0x000257B8
		protected virtual void OnComponentInit(EntityUid uid, IdentityComponent component, ComponentInit args)
		{
			component.IdentityEntitySlot = this._container.EnsureContainer<ContainerSlot>(uid, SharedIdentitySystem.SlotName, null);
		}

		// Token: 0x04000BE2 RID: 3042
		[Dependency]
		private readonly SharedContainerSystem _container;

		// Token: 0x04000BE3 RID: 3043
		private static string SlotName = "identity";
	}
}
