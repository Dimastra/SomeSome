using System;
using System.Runtime.CompilerServices;
using Content.Shared.Construction.Prototypes;
using Robust.Shared.Audio;
using Robust.Shared.Containers;
using Robust.Shared.GameObjects;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom.Prototype;
using Robust.Shared.ViewVariables;

namespace Content.Server.Kitchen.Components
{
	// Token: 0x02000435 RID: 1077
	[NullableContext(1)]
	[Nullable(0)]
	[RegisterComponent]
	public sealed class MicrowaveComponent : Component
	{
		// Token: 0x170002ED RID: 749
		// (get) Token: 0x060015E2 RID: 5602 RVA: 0x00073F4E File Offset: 0x0007214E
		// (set) Token: 0x060015E3 RID: 5603 RVA: 0x00073F56 File Offset: 0x00072156
		[Nullable(2)]
		public IPlayingAudioStream PlayingStream { [NullableContext(2)] get; [NullableContext(2)] set; }

		// Token: 0x04000DA2 RID: 3490
		[DataField("cookTimeMultiplier", false, 1, false, false, null)]
		[ViewVariables]
		public float CookTimeMultiplier = 1f;

		// Token: 0x04000DA3 RID: 3491
		[DataField("machinePartCookTimeMultiplier", false, 1, false, false, typeof(PrototypeIdSerializer<MachinePartPrototype>))]
		public string MachinePartCookTimeMultiplier = "Laser";

		// Token: 0x04000DA4 RID: 3492
		[DataField("cookTimeScalingConstant", false, 1, false, false, null)]
		public float CookTimeScalingConstant = 0.5f;

		// Token: 0x04000DA5 RID: 3493
		[DataField("failureResult", false, 1, false, false, typeof(PrototypeIdSerializer<EntityPrototype>))]
		public string BadRecipeEntityId = "FoodBadRecipe";

		// Token: 0x04000DA6 RID: 3494
		[DataField("beginCookingSound", false, 1, false, false, null)]
		public SoundSpecifier StartCookingSound = new SoundPathSpecifier("/Audio/Machines/microwave_start_beep.ogg", null);

		// Token: 0x04000DA7 RID: 3495
		[DataField("foodDoneSound", false, 1, false, false, null)]
		public SoundSpecifier FoodDoneSound = new SoundPathSpecifier("/Audio/Machines/microwave_done_beep.ogg", null);

		// Token: 0x04000DA8 RID: 3496
		[DataField("clickSound", false, 1, false, false, null)]
		public SoundSpecifier ClickSound = new SoundPathSpecifier("/Audio/Machines/machine_switch.ogg", null);

		// Token: 0x04000DA9 RID: 3497
		[DataField("ItemBreakSound", false, 1, false, false, null)]
		public SoundSpecifier ItemBreakSound = new SoundPathSpecifier("/Audio/Effects/clang.ogg", null);

		// Token: 0x04000DAB RID: 3499
		[DataField("loopingSound", false, 1, false, false, null)]
		public SoundSpecifier LoopingSound = new SoundPathSpecifier("/Audio/Machines/microwave_loop.ogg", null);

		// Token: 0x04000DAC RID: 3500
		[ViewVariables]
		public bool Broken;

		// Token: 0x04000DAD RID: 3501
		[DataField("currentCookTimerTime", false, 1, false, false, null)]
		[ViewVariables]
		public uint CurrentCookTimerTime = 5U;

		// Token: 0x04000DAE RID: 3502
		[DataField("temperatureUpperThreshold", false, 1, false, false, null)]
		public float TemperatureUpperThreshold = 373.15f;

		// Token: 0x04000DAF RID: 3503
		public int CurrentCookTimeButtonIndex;

		// Token: 0x04000DB0 RID: 3504
		public Container Storage;
	}
}
