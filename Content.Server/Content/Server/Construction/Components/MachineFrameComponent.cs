using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Content.Shared.Construction.Components;
using Content.Shared.Construction.Prototypes;
using Robust.Shared.Containers;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom.Prototype.Dictionary;
using Robust.Shared.ViewVariables;

namespace Content.Server.Construction.Components
{
	// Token: 0x02000609 RID: 1545
	[NullableContext(1)]
	[Nullable(0)]
	[RegisterComponent]
	public sealed class MachineFrameComponent : Component
	{
		// Token: 0x170004FB RID: 1275
		// (get) Token: 0x0600212B RID: 8491 RVA: 0x000AD981 File Offset: 0x000ABB81
		[ViewVariables]
		public bool HasBoard
		{
			get
			{
				Container boardContainer = this.BoardContainer;
				return boardContainer == null || boardContainer.ContainedEntities.Count != 0;
			}
		}

		// Token: 0x0400145C RID: 5212
		public const string PartContainerName = "machine_parts";

		// Token: 0x0400145D RID: 5213
		public const string BoardContainerName = "machine_board";

		// Token: 0x0400145E RID: 5214
		[DataField("progress", false, 1, false, false, typeof(PrototypeIdDictionarySerializer<int, MachinePartPrototype>))]
		public readonly Dictionary<string, int> Progress = new Dictionary<string, int>();

		// Token: 0x0400145F RID: 5215
		[ViewVariables]
		public readonly Dictionary<string, int> MaterialProgress = new Dictionary<string, int>();

		// Token: 0x04001460 RID: 5216
		[ViewVariables]
		public readonly Dictionary<string, int> ComponentProgress = new Dictionary<string, int>();

		// Token: 0x04001461 RID: 5217
		[ViewVariables]
		public readonly Dictionary<string, int> TagProgress = new Dictionary<string, int>();

		// Token: 0x04001462 RID: 5218
		[DataField("requirements", false, 1, false, false, typeof(PrototypeIdDictionarySerializer<int, MachinePartPrototype>))]
		public Dictionary<string, int> Requirements = new Dictionary<string, int>();

		// Token: 0x04001463 RID: 5219
		[ViewVariables]
		public Dictionary<string, int> MaterialRequirements = new Dictionary<string, int>();

		// Token: 0x04001464 RID: 5220
		[ViewVariables]
		public Dictionary<string, GenericPartInfo> ComponentRequirements = new Dictionary<string, GenericPartInfo>();

		// Token: 0x04001465 RID: 5221
		[ViewVariables]
		public Dictionary<string, GenericPartInfo> TagRequirements = new Dictionary<string, GenericPartInfo>();

		// Token: 0x04001466 RID: 5222
		[ViewVariables]
		public Container BoardContainer;

		// Token: 0x04001467 RID: 5223
		[ViewVariables]
		public Container PartContainer;
	}
}
