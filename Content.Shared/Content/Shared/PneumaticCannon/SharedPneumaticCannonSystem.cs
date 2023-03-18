using System;
using System.Runtime.CompilerServices;
using Content.Shared.Popups;
using Content.Shared.Weapons.Ranged.Systems;
using Robust.Shared.Containers;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;

namespace Content.Shared.PneumaticCannon
{
	// Token: 0x0200026C RID: 620
	[NullableContext(1)]
	[Nullable(0)]
	public abstract class SharedPneumaticCannonSystem : EntitySystem
	{
		// Token: 0x06000716 RID: 1814 RVA: 0x00018577 File Offset: 0x00016777
		public override void Initialize()
		{
			base.Initialize();
			base.SubscribeLocalEvent<PneumaticCannonComponent, AttemptShootEvent>(new ComponentEventRefHandler<PneumaticCannonComponent, AttemptShootEvent>(this.OnAttemptShoot), null, null);
		}

		// Token: 0x06000717 RID: 1815 RVA: 0x00018594 File Offset: 0x00016794
		private void OnAttemptShoot(EntityUid uid, PneumaticCannonComponent component, ref AttemptShootEvent args)
		{
			IContainer container;
			if (this.Container.TryGetContainer(uid, "gas_tank", ref container, null))
			{
				ContainerSlot slot = container as ContainerSlot;
				if (slot != null && slot.ContainedEntity != null)
				{
					return;
				}
			}
			args.Cancelled = true;
		}

		// Token: 0x040006FF RID: 1791
		[Dependency]
		protected readonly SharedContainerSystem Container;

		// Token: 0x04000700 RID: 1792
		[Dependency]
		protected readonly SharedPopupSystem Popup;
	}
}
