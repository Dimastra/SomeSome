using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Content.Shared.Tag;
using Content.Shared.Tools;
using Robust.Shared.Containers;
using Robust.Shared.GameObjects;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom.Prototype;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom.Prototype.Dictionary;
using Robust.Shared.ViewVariables;

namespace Content.Server.Mech.Components
{
	// Token: 0x020003C9 RID: 969
	[NullableContext(1)]
	[Nullable(0)]
	[RegisterComponent]
	public sealed class MechAssemblyComponent : Component
	{
		// Token: 0x04000C5B RID: 3163
		[DataField("requiredParts", false, 1, true, false, typeof(PrototypeIdDictionarySerializer<bool, TagPrototype>))]
		public Dictionary<string, bool> RequiredParts = new Dictionary<string, bool>();

		// Token: 0x04000C5C RID: 3164
		[DataField("finishedPrototype", false, 1, true, false, typeof(PrototypeIdSerializer<EntityPrototype>))]
		public string FinishedPrototype;

		// Token: 0x04000C5D RID: 3165
		[ViewVariables]
		public Container PartsContainer;

		// Token: 0x04000C5E RID: 3166
		[DataField("qualityNeeded", false, 1, false, false, typeof(PrototypeIdSerializer<ToolQualityPrototype>))]
		public string QualityNeeded = "Prying";
	}
}
