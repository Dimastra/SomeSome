using System;
using System.Runtime.CompilerServices;
using Content.Shared.Humanoid.Prototypes;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom.Prototype;

namespace Content.Server.Humanoid.Components
{
	// Token: 0x0200045C RID: 1116
	[RegisterComponent]
	public sealed class RandomHumanoidSpawnerComponent : Component
	{
		// Token: 0x04000E15 RID: 3605
		[Nullable(1)]
		[DataField("settings", false, 1, false, false, typeof(PrototypeIdSerializer<RandomHumanoidSettingsPrototype>))]
		public string SettingsPrototypeId;
	}
}
