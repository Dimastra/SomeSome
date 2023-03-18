using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Content.Server.Light.EntitySystems;
using Content.Shared.Light.Component;
using Robust.Shared.Analyzers;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.ViewVariables;

namespace Content.Server.Light.Components
{
	// Token: 0x02000416 RID: 1046
	[RegisterComponent]
	[Access(new Type[]
	{
		typeof(EmergencyLightSystem)
	})]
	public sealed class EmergencyLightComponent : SharedEmergencyLightComponent
	{
		// Token: 0x04000D26 RID: 3366
		[ViewVariables]
		public EmergencyLightState State;

		// Token: 0x04000D27 RID: 3367
		public bool ForciblyEnabled;

		// Token: 0x04000D28 RID: 3368
		[ViewVariables]
		[DataField("wattage", false, 1, false, false, null)]
		public float Wattage = 5f;

		// Token: 0x04000D29 RID: 3369
		[ViewVariables]
		[DataField("chargingWattage", false, 1, false, false, null)]
		public float ChargingWattage = 60f;

		// Token: 0x04000D2A RID: 3370
		[ViewVariables]
		[DataField("chargingEfficiency", false, 1, false, false, null)]
		public float ChargingEfficiency = 0.85f;

		// Token: 0x04000D2B RID: 3371
		[Nullable(1)]
		public Dictionary<EmergencyLightState, string> BatteryStateText = new Dictionary<EmergencyLightState, string>
		{
			{
				EmergencyLightState.Full,
				"emergency-light-component-light-state-full"
			},
			{
				EmergencyLightState.Empty,
				"emergency-light-component-light-state-empty"
			},
			{
				EmergencyLightState.Charging,
				"emergency-light-component-light-state-charging"
			},
			{
				EmergencyLightState.On,
				"emergency-light-component-light-state-on"
			}
		};
	}
}
