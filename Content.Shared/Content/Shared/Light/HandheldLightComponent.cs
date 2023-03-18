using System;
using System.Runtime.CompilerServices;
using Content.Shared.Actions.ActionTypes;
using Robust.Shared.Analyzers;
using Robust.Shared.Audio;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom.Prototype;
using Robust.Shared.ViewVariables;

namespace Content.Shared.Light
{
	// Token: 0x02000361 RID: 865
	[NullableContext(1)]
	[Nullable(0)]
	[NetworkedComponent]
	[RegisterComponent]
	[Access(new Type[]
	{
		typeof(SharedHandheldLightSystem)
	})]
	public sealed class HandheldLightComponent : Component
	{
		// Token: 0x170001F0 RID: 496
		// (get) Token: 0x06000A30 RID: 2608 RVA: 0x00021F7E File Offset: 0x0002017E
		// (set) Token: 0x06000A31 RID: 2609 RVA: 0x00021F86 File Offset: 0x00020186
		[ViewVariables]
		[DataField("wattage", false, 1, false, false, null)]
		public float Wattage { get; set; } = 0.8f;

		// Token: 0x170001F1 RID: 497
		// (get) Token: 0x06000A32 RID: 2610 RVA: 0x00021F8F File Offset: 0x0002018F
		// (set) Token: 0x06000A33 RID: 2611 RVA: 0x00021F97 File Offset: 0x00020197
		[DataField("blinkingBehaviourId", false, 1, false, false, null)]
		public string BlinkingBehaviourId { get; set; } = string.Empty;

		// Token: 0x170001F2 RID: 498
		// (get) Token: 0x06000A34 RID: 2612 RVA: 0x00021FA0 File Offset: 0x000201A0
		// (set) Token: 0x06000A35 RID: 2613 RVA: 0x00021FA8 File Offset: 0x000201A8
		[DataField("radiatingBehaviourId", false, 1, false, false, null)]
		public string RadiatingBehaviourId { get; set; } = string.Empty;

		// Token: 0x040009E1 RID: 2529
		public byte? Level;

		// Token: 0x040009E2 RID: 2530
		public bool Activated;

		// Token: 0x040009E4 RID: 2532
		[DataField("turnOnSound", false, 1, false, false, null)]
		public SoundSpecifier TurnOnSound = new SoundPathSpecifier("/Audio/Items/flashlight_on.ogg", null);

		// Token: 0x040009E5 RID: 2533
		[DataField("turnOnFailSound", false, 1, false, false, null)]
		public SoundSpecifier TurnOnFailSound = new SoundPathSpecifier("/Audio/Machines/button.ogg", null);

		// Token: 0x040009E6 RID: 2534
		[DataField("turnOffSound", false, 1, false, false, null)]
		public SoundSpecifier TurnOffSound = new SoundPathSpecifier("/Audio/Items/flashlight_off.ogg", null);

		// Token: 0x040009E7 RID: 2535
		[DataField("addPrefix", false, 1, false, false, null)]
		public bool AddPrefix;

		// Token: 0x040009E8 RID: 2536
		[DataField("toggleActionId", false, 1, false, false, typeof(PrototypeIdSerializer<InstantActionPrototype>))]
		public string ToggleActionId = "ToggleLight";

		// Token: 0x040009E9 RID: 2537
		[Nullable(2)]
		[DataField("toggleAction", false, 1, false, false, null)]
		public InstantAction ToggleAction;

		// Token: 0x040009EA RID: 2538
		public const int StatusLevels = 6;

		// Token: 0x020007E3 RID: 2019
		[NullableContext(0)]
		[NetSerializable]
		[Serializable]
		public sealed class HandheldLightComponentState : ComponentState
		{
			// Token: 0x170004FE RID: 1278
			// (get) Token: 0x06001861 RID: 6241 RVA: 0x0004DFF9 File Offset: 0x0004C1F9
			public byte? Charge { get; }

			// Token: 0x170004FF RID: 1279
			// (get) Token: 0x06001862 RID: 6242 RVA: 0x0004E001 File Offset: 0x0004C201
			public bool Activated { get; }

			// Token: 0x06001863 RID: 6243 RVA: 0x0004E009 File Offset: 0x0004C209
			public HandheldLightComponentState(bool activated, byte? charge)
			{
				this.Activated = activated;
				this.Charge = charge;
			}
		}
	}
}
