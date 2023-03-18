using System;
using System.Runtime.CompilerServices;
using Content.Server.Atmos.Monitor.Systems;
using Content.Server.DeviceNetwork.Systems;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;

namespace Content.Server.Atmos.Monitor
{
	// Token: 0x02000776 RID: 1910
	[NullableContext(1)]
	[Nullable(0)]
	public abstract class AirAlarmModeExecutor : IAirAlarmMode
	{
		// Token: 0x0600288B RID: 10379
		public abstract void Execute(EntityUid uid);

		// Token: 0x0600288C RID: 10380 RVA: 0x000D364C File Offset: 0x000D184C
		public AirAlarmModeExecutor()
		{
			IoCManager.InjectDependencies<AirAlarmModeExecutor>(this);
			this.DeviceNetworkSystem = EntitySystem.Get<DeviceNetworkSystem>();
			this.AirAlarmSystem = EntitySystem.Get<AirAlarmSystem>();
		}

		// Token: 0x04001932 RID: 6450
		[Dependency]
		public readonly IEntityManager EntityManager;

		// Token: 0x04001933 RID: 6451
		public readonly DeviceNetworkSystem DeviceNetworkSystem;

		// Token: 0x04001934 RID: 6452
		public readonly AirAlarmSystem AirAlarmSystem;
	}
}
