using System;
using System.Runtime.CompilerServices;
using Content.Shared.Store;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom.Prototype;
using Robust.Shared.ViewVariables;

namespace Content.Server.Traitor.Uplink.SurplusBundle
{
	// Token: 0x0200010E RID: 270
	[RegisterComponent]
	public sealed class SurplusBundleComponent : Component
	{
		// Token: 0x040002D2 RID: 722
		[ViewVariables]
		[DataField("totalPrice", false, 1, false, false, null)]
		public int TotalPrice = 20;

		// Token: 0x040002D3 RID: 723
		[Nullable(1)]
		[DataField("storePreset", false, 1, false, false, typeof(PrototypeIdSerializer<StorePresetPrototype>))]
		public string StorePreset = "StorePresetUplink";
	}
}
