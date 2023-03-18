using System;
using System.Runtime.CompilerServices;
using Content.Server.Kitchen.EntitySystems;
using Content.Shared.Construction.Prototypes;
using Robust.Shared.Analyzers;
using Robust.Shared.Audio;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom.Prototype;
using Robust.Shared.ViewVariables;

namespace Content.Server.Kitchen.Components
{
	// Token: 0x02000437 RID: 1079
	[NullableContext(1)]
	[Nullable(0)]
	[Access(new Type[]
	{
		typeof(ReagentGrinderSystem)
	})]
	[RegisterComponent]
	public sealed class ReagentGrinderComponent : Component
	{
		// Token: 0x170002EE RID: 750
		// (get) Token: 0x060015E6 RID: 5606 RVA: 0x00074044 File Offset: 0x00072244
		// (set) Token: 0x060015E7 RID: 5607 RVA: 0x0007404C File Offset: 0x0007224C
		[DataField("clickSound", false, 1, false, false, null)]
		[ViewVariables]
		public SoundSpecifier ClickSound { get; set; } = new SoundPathSpecifier("/Audio/Machines/machine_switch.ogg", null);

		// Token: 0x170002EF RID: 751
		// (get) Token: 0x060015E8 RID: 5608 RVA: 0x00074055 File Offset: 0x00072255
		// (set) Token: 0x060015E9 RID: 5609 RVA: 0x0007405D File Offset: 0x0007225D
		[DataField("grindSound", false, 1, false, false, null)]
		[ViewVariables]
		public SoundSpecifier GrindSound { get; set; } = new SoundPathSpecifier("/Audio/Machines/blender.ogg", null);

		// Token: 0x170002F0 RID: 752
		// (get) Token: 0x060015EA RID: 5610 RVA: 0x00074066 File Offset: 0x00072266
		// (set) Token: 0x060015EB RID: 5611 RVA: 0x0007406E File Offset: 0x0007226E
		[DataField("juiceSound", false, 1, false, false, null)]
		[ViewVariables]
		public SoundSpecifier JuiceSound { get; set; } = new SoundPathSpecifier("/Audio/Machines/juicer.ogg", null);

		// Token: 0x04000DB3 RID: 3507
		[ViewVariables]
		public int StorageMaxEntities = 6;

		// Token: 0x04000DB4 RID: 3508
		[DataField("baseStorageMaxEntities", false, 1, false, false, null)]
		[ViewVariables]
		public int BaseStorageMaxEntities = 4;

		// Token: 0x04000DB5 RID: 3509
		[DataField("machinePartStorageMax", false, 1, false, false, typeof(PrototypeIdSerializer<MachinePartPrototype>))]
		public string MachinePartStorageMax = "MatterBin";

		// Token: 0x04000DB6 RID: 3510
		[DataField("storagePerPartRating", false, 1, false, false, null)]
		public int StoragePerPartRating = 4;

		// Token: 0x04000DB7 RID: 3511
		[DataField("workTime", false, 1, false, false, null)]
		[ViewVariables]
		public TimeSpan WorkTime = TimeSpan.FromSeconds(3.5);

		// Token: 0x04000DB8 RID: 3512
		[ViewVariables]
		public float WorkTimeMultiplier = 1f;

		// Token: 0x04000DB9 RID: 3513
		[DataField("machinePartWorkTime", false, 1, false, false, typeof(PrototypeIdSerializer<MachinePartPrototype>))]
		public string MachinePartWorkTime = "Manipulator";

		// Token: 0x04000DBA RID: 3514
		[DataField("partRatingWorkTimeMultiplier", false, 1, false, false, null)]
		public float PartRatingWorkTimerMulitplier = 0.6f;

		// Token: 0x04000DBE RID: 3518
		[Nullable(2)]
		public IPlayingAudioStream AudioStream;
	}
}
