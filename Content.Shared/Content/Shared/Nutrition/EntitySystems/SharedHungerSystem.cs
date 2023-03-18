using System;
using System.Runtime.CompilerServices;
using Content.Shared.Movement.Systems;
using Content.Shared.Nutrition.Components;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;

namespace Content.Shared.Nutrition.EntitySystems
{
	// Token: 0x020002AC RID: 684
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class SharedHungerSystem : EntitySystem
	{
		// Token: 0x060007B1 RID: 1969 RVA: 0x00019EA9 File Offset: 0x000180A9
		public override void Initialize()
		{
			base.Initialize();
			base.SubscribeLocalEvent<SharedHungerComponent, RefreshMovementSpeedModifiersEvent>(new ComponentEventHandler<SharedHungerComponent, RefreshMovementSpeedModifiersEvent>(this.OnRefreshMovespeed), null, null);
		}

		// Token: 0x060007B2 RID: 1970 RVA: 0x00019EC8 File Offset: 0x000180C8
		private void OnRefreshMovespeed(EntityUid uid, SharedHungerComponent component, RefreshMovementSpeedModifiersEvent args)
		{
			if (this._jetpack.IsUserFlying(component.Owner))
			{
				return;
			}
			float mod = (component.CurrentHungerThreshold <= HungerThreshold.Starving) ? 0.75f : 1f;
			args.ModifySpeed(mod, mod);
		}

		// Token: 0x040007BE RID: 1982
		[Dependency]
		private readonly SharedJetpackSystem _jetpack;
	}
}
