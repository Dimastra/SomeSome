using System;
using System.Runtime.CompilerServices;
using Content.Shared.Movement.Systems;
using Robust.Shared.GameObjects;

namespace Content.Shared.Zombies
{
	// Token: 0x02000010 RID: 16
	public abstract class SharedZombieSystem : EntitySystem
	{
		// Token: 0x06000016 RID: 22 RVA: 0x00002269 File Offset: 0x00000469
		public override void Initialize()
		{
			base.Initialize();
			base.SubscribeLocalEvent<ZombieComponent, RefreshMovementSpeedModifiersEvent>(new ComponentEventHandler<ZombieComponent, RefreshMovementSpeedModifiersEvent>(this.OnRefreshSpeed), null, null);
		}

		// Token: 0x06000017 RID: 23 RVA: 0x00002288 File Offset: 0x00000488
		[NullableContext(1)]
		private void OnRefreshSpeed(EntityUid uid, ZombieComponent component, RefreshMovementSpeedModifiersEvent args)
		{
			float mod = component.ZombieMovementSpeedDebuff;
			args.ModifySpeed(mod, mod);
		}
	}
}
