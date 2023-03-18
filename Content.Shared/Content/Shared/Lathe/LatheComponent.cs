using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Content.Shared.Construction.Prototypes;
using Content.Shared.Research.Prototypes;
using Robust.Shared.Audio;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom.Prototype;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom.Prototype.List;
using Robust.Shared.ViewVariables;

namespace Content.Shared.Lathe
{
	// Token: 0x02000379 RID: 889
	[NullableContext(1)]
	[Nullable(0)]
	[RegisterComponent]
	[NetworkedComponent]
	public sealed class LatheComponent : Component
	{
		// Token: 0x04000A3D RID: 2621
		[DataField("staticRecipes", false, 1, false, false, typeof(PrototypeIdListSerializer<LatheRecipePrototype>))]
		public readonly List<string> StaticRecipes = new List<string>();

		// Token: 0x04000A3E RID: 2622
		[Nullable(new byte[]
		{
			2,
			1
		})]
		[DataField("dynamicRecipes", false, 1, false, false, typeof(PrototypeIdListSerializer<LatheRecipePrototype>))]
		public readonly List<string> DynamicRecipes;

		// Token: 0x04000A3F RID: 2623
		[DataField("queue", false, 1, false, false, null)]
		public List<LatheRecipePrototype> Queue = new List<LatheRecipePrototype>();

		// Token: 0x04000A40 RID: 2624
		[Nullable(2)]
		[DataField("producingSound", false, 1, false, false, null)]
		public SoundSpecifier ProducingSound;

		// Token: 0x04000A41 RID: 2625
		[DataField("idleState", false, 1, true, false, null)]
		public string IdleState;

		// Token: 0x04000A42 RID: 2626
		[DataField("runningState", false, 1, true, false, null)]
		public string RunningState;

		// Token: 0x04000A43 RID: 2627
		[Nullable(2)]
		[ViewVariables]
		public LatheRecipePrototype CurrentRecipe;

		// Token: 0x04000A44 RID: 2628
		[ViewVariables]
		public float TimeMultiplier = 1f;

		// Token: 0x04000A45 RID: 2629
		[DataField("machinePartPrintSpeed", false, 1, false, false, typeof(PrototypeIdSerializer<MachinePartPrototype>))]
		public string MachinePartPrintTime = "Manipulator";

		// Token: 0x04000A46 RID: 2630
		[DataField("partRatingPrintTimeMultiplier", false, 1, false, false, null)]
		public float PartRatingPrintTimeMultiplier = 0.5f;

		// Token: 0x04000A47 RID: 2631
		[ViewVariables]
		public float MaterialUseMultiplier = 1f;

		// Token: 0x04000A48 RID: 2632
		[DataField("machinePartMaterialUse", false, 1, false, false, typeof(PrototypeIdSerializer<MachinePartPrototype>))]
		public string MachinePartMaterialUse = "MatterBin";

		// Token: 0x04000A49 RID: 2633
		[DataField("partRatingMaterialUseMultiplier", false, 1, false, false, null)]
		public float PartRatingMaterialUseMultiplier = 0.75f;

		// Token: 0x04000A4A RID: 2634
		public const float DefaultPartRatingMaterialUseMultiplier = 0.75f;
	}
}
