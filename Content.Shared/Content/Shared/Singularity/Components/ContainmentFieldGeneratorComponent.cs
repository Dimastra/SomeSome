using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Content.Shared.Tag;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Maths;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom.Prototype;
using Robust.Shared.ViewVariables;

namespace Content.Shared.Singularity.Components
{
	// Token: 0x020001A1 RID: 417
	[NullableContext(1)]
	[Nullable(0)]
	[RegisterComponent]
	[NetworkedComponent]
	public sealed class ContainmentFieldGeneratorComponent : Component
	{
		// Token: 0x170000E2 RID: 226
		// (get) Token: 0x060004FD RID: 1277 RVA: 0x000132E8 File Offset: 0x000114E8
		// (set) Token: 0x060004FE RID: 1278 RVA: 0x000132F0 File Offset: 0x000114F0
		[DataField("powerBuffer", false, 1, false, false, null)]
		public int PowerBuffer
		{
			get
			{
				return this._powerBuffer;
			}
			set
			{
				this._powerBuffer = Math.Clamp(value, 0, 25);
			}
		}

		// Token: 0x04000491 RID: 1169
		private int _powerBuffer;

		// Token: 0x04000492 RID: 1170
		[ViewVariables]
		[DataField("powerMinimum", false, 1, false, false, null)]
		public int PowerMinimum = 6;

		// Token: 0x04000493 RID: 1171
		[ViewVariables]
		[DataField("power", false, 1, false, false, null)]
		public int PowerReceived = 3;

		// Token: 0x04000494 RID: 1172
		[ViewVariables]
		[DataField("powerLoss", false, 1, false, false, null)]
		public int PowerLoss = 2;

		// Token: 0x04000495 RID: 1173
		[DataField("accumulator", false, 1, false, false, null)]
		public float Accumulator;

		// Token: 0x04000496 RID: 1174
		[DataField("threshold", false, 1, false, false, null)]
		public float Threshold = 10f;

		// Token: 0x04000497 RID: 1175
		[DataField("maxLength", false, 1, false, false, null)]
		public float MaxLength = 8f;

		// Token: 0x04000498 RID: 1176
		[ViewVariables]
		[DataField("idTag", false, 1, false, false, typeof(PrototypeIdSerializer<TagPrototype>))]
		public string IDTag = "EmitterBolt";

		// Token: 0x04000499 RID: 1177
		[ViewVariables]
		public bool Enabled;

		// Token: 0x0400049A RID: 1178
		[ViewVariables]
		public bool IsConnected;

		// Token: 0x0400049B RID: 1179
		[DataField("collisionMask", false, 1, false, false, null)]
		public int CollisionMask = 31;

		// Token: 0x0400049C RID: 1180
		[Nullable(new byte[]
		{
			1,
			0,
			1,
			1
		})]
		[ViewVariables]
		public Dictionary<Direction, ValueTuple<ContainmentFieldGeneratorComponent, List<EntityUid>>> Connections = new Dictionary<Direction, ValueTuple<ContainmentFieldGeneratorComponent, List<EntityUid>>>();

		// Token: 0x0400049D RID: 1181
		[ViewVariables]
		[DataField("createdField", false, 1, false, false, typeof(PrototypeIdSerializer<EntityPrototype>))]
		public string CreatedField = "ContainmentField";
	}
}
