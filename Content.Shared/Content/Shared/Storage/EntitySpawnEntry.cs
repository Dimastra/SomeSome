using System;
using System.Runtime.CompilerServices;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom.Prototype;
using Robust.Shared.ViewVariables;

namespace Content.Shared.Storage
{
	// Token: 0x0200012A RID: 298
	[NullableContext(2)]
	[Nullable(0)]
	[DataDefinition]
	[Serializable]
	public struct EntitySpawnEntry
	{
		// Token: 0x0600036B RID: 875 RVA: 0x0000E78C File Offset: 0x0000C98C
		public EntitySpawnEntry()
		{
			this.PrototypeId = null;
			this.SpawnProbability = 1f;
			this.GroupId = null;
			this.Amount = 1;
			this.MaxAmount = 1;
		}

		// Token: 0x0400038B RID: 907
		[ViewVariables]
		[DataField("id", false, 1, false, false, typeof(PrototypeIdSerializer<EntityPrototype>))]
		public string PrototypeId;

		// Token: 0x0400038C RID: 908
		[ViewVariables]
		[DataField("prob", false, 1, false, false, null)]
		public float SpawnProbability;

		// Token: 0x0400038D RID: 909
		[ViewVariables]
		[DataField("orGroup", false, 1, false, false, null)]
		public string GroupId;

		// Token: 0x0400038E RID: 910
		[ViewVariables]
		[DataField("amount", false, 1, false, false, null)]
		public int Amount;

		// Token: 0x0400038F RID: 911
		[ViewVariables]
		[DataField("maxAmount", false, 1, false, false, null)]
		public int MaxAmount;
	}
}
