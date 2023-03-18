using System;
using System.Runtime.CompilerServices;
using Content.Server.Light.EntitySystems;
using Content.Shared.Damage;
using Content.Shared.Light.Component;
using Content.Shared.MachineLinking;
using Robust.Shared.Analyzers;
using Robust.Shared.Audio;
using Robust.Shared.Containers;
using Robust.Shared.GameObjects;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom.Prototype;
using Robust.Shared.ViewVariables;

namespace Content.Server.Light.Components
{
	// Token: 0x0200041F RID: 1055
	[NullableContext(1)]
	[Nullable(0)]
	[RegisterComponent]
	[Access(new Type[]
	{
		typeof(PoweredLightSystem)
	})]
	public sealed class PoweredLightComponent : Component
	{
		// Token: 0x04000D3A RID: 3386
		[DataField("burnHandSound", false, 1, false, false, null)]
		public SoundSpecifier BurnHandSound = new SoundPathSpecifier("/Audio/Effects/lightburn.ogg", null);

		// Token: 0x04000D3B RID: 3387
		[DataField("turnOnSound", false, 1, false, false, null)]
		public SoundSpecifier TurnOnSound = new SoundPathSpecifier("/Audio/Machines/light_tube_on.ogg", null);

		// Token: 0x04000D3C RID: 3388
		[Nullable(2)]
		[DataField("hasLampOnSpawn", false, 1, false, false, typeof(PrototypeIdSerializer<EntityPrototype>))]
		public string HasLampOnSpawn;

		// Token: 0x04000D3D RID: 3389
		[DataField("bulb", false, 1, false, false, null)]
		public LightBulbType BulbType;

		// Token: 0x04000D3E RID: 3390
		[DataField("on", false, 1, false, false, null)]
		public bool On = true;

		// Token: 0x04000D3F RID: 3391
		[DataField("damage", false, 1, true, false, null)]
		[ViewVariables]
		public DamageSpecifier Damage;

		// Token: 0x04000D40 RID: 3392
		[DataField("ignoreGhostsBoo", false, 1, false, false, null)]
		public bool IgnoreGhostsBoo;

		// Token: 0x04000D41 RID: 3393
		[DataField("ghostBlinkingTime", false, 1, false, false, null)]
		public TimeSpan GhostBlinkingTime = TimeSpan.FromSeconds(10.0);

		// Token: 0x04000D42 RID: 3394
		[DataField("ghostBlinkingCooldown", false, 1, false, false, null)]
		public TimeSpan GhostBlinkingCooldown = TimeSpan.FromSeconds(60.0);

		// Token: 0x04000D43 RID: 3395
		[ViewVariables]
		public ContainerSlot LightBulbContainer;

		// Token: 0x04000D44 RID: 3396
		[ViewVariables]
		public bool CurrentLit;

		// Token: 0x04000D45 RID: 3397
		[ViewVariables]
		public bool IsBlinking;

		// Token: 0x04000D46 RID: 3398
		[ViewVariables]
		public TimeSpan LastThunk;

		// Token: 0x04000D47 RID: 3399
		[ViewVariables]
		public TimeSpan? LastGhostBlink;

		// Token: 0x04000D48 RID: 3400
		[DataField("onPort", false, 1, false, false, typeof(PrototypeIdSerializer<ReceiverPortPrototype>))]
		public string OnPort = "On";

		// Token: 0x04000D49 RID: 3401
		[DataField("offPort", false, 1, false, false, typeof(PrototypeIdSerializer<ReceiverPortPrototype>))]
		public string OffPort = "Off";

		// Token: 0x04000D4A RID: 3402
		[DataField("togglePort", false, 1, false, false, typeof(PrototypeIdSerializer<ReceiverPortPrototype>))]
		public string TogglePort = "Toggle";

		// Token: 0x04000D4B RID: 3403
		[DataField("ejectBulbDelay", false, 1, false, false, null)]
		public float EjectBulbDelay = 2f;
	}
}
