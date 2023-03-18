using System;
using System.Runtime.CompilerServices;
using Content.Server.Power.Components;
using Content.Server.Power.EntitySystems;
using Content.Shared.Audio;
using Robust.Shared.GameObjects;

namespace Content.Server.Audio
{
	// Token: 0x0200072E RID: 1838
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class AmbientSoundSystem : SharedAmbientSoundSystem
	{
		// Token: 0x0600268E RID: 9870 RVA: 0x000CBF57 File Offset: 0x000CA157
		public override void Initialize()
		{
			base.Initialize();
			base.SubscribeLocalEvent<AmbientOnPoweredComponent, PowerChangedEvent>(new ComponentEventRefHandler<AmbientOnPoweredComponent, PowerChangedEvent>(this.HandlePowerChange), null, null);
			base.SubscribeLocalEvent<AmbientOnPoweredComponent, PowerNetBatterySupplyEvent>(new ComponentEventRefHandler<AmbientOnPoweredComponent, PowerNetBatterySupplyEvent>(this.HandlePowerSupply), null, null);
		}

		// Token: 0x0600268F RID: 9871 RVA: 0x000CBF87 File Offset: 0x000CA187
		private void HandlePowerSupply(EntityUid uid, AmbientOnPoweredComponent component, ref PowerNetBatterySupplyEvent args)
		{
			this.SetAmbience(uid, args.Supply, null);
		}

		// Token: 0x06002690 RID: 9872 RVA: 0x000CBF97 File Offset: 0x000CA197
		private void HandlePowerChange(EntityUid uid, AmbientOnPoweredComponent component, ref PowerChangedEvent args)
		{
			this.SetAmbience(uid, args.Powered, null);
		}
	}
}
