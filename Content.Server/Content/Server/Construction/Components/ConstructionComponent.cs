using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Content.Shared.Construction.Prototypes;
using Robust.Shared.Analyzers;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom.Prototype;
using Robust.Shared.ViewVariables;

namespace Content.Server.Construction.Components
{
	// Token: 0x02000605 RID: 1541
	[NullableContext(1)]
	[Nullable(0)]
	[RegisterComponent]
	[Access(new Type[]
	{
		typeof(ConstructionSystem)
	})]
	public sealed class ConstructionComponent : Component
	{
		// Token: 0x170004F0 RID: 1264
		// (get) Token: 0x06002112 RID: 8466 RVA: 0x000AD88A File Offset: 0x000ABA8A
		// (set) Token: 0x06002113 RID: 8467 RVA: 0x000AD892 File Offset: 0x000ABA92
		[DataField("graph", false, 1, true, false, typeof(PrototypeIdSerializer<ConstructionGraphPrototype>))]
		public string Graph { get; set; } = string.Empty;

		// Token: 0x170004F1 RID: 1265
		// (get) Token: 0x06002114 RID: 8468 RVA: 0x000AD89B File Offset: 0x000ABA9B
		// (set) Token: 0x06002115 RID: 8469 RVA: 0x000AD8A3 File Offset: 0x000ABAA3
		[DataField("node", false, 1, true, false, null)]
		public string Node { get; set; }

		// Token: 0x170004F2 RID: 1266
		// (get) Token: 0x06002116 RID: 8470 RVA: 0x000AD8AC File Offset: 0x000ABAAC
		// (set) Token: 0x06002117 RID: 8471 RVA: 0x000AD8B4 File Offset: 0x000ABAB4
		[DataField("edge", false, 1, false, false, null)]
		public int? EdgeIndex { get; set; }

		// Token: 0x170004F3 RID: 1267
		// (get) Token: 0x06002118 RID: 8472 RVA: 0x000AD8BD File Offset: 0x000ABABD
		// (set) Token: 0x06002119 RID: 8473 RVA: 0x000AD8C5 File Offset: 0x000ABAC5
		[DataField("step", false, 1, false, false, null)]
		public int StepIndex { get; set; }

		// Token: 0x170004F4 RID: 1268
		// (get) Token: 0x0600211A RID: 8474 RVA: 0x000AD8CE File Offset: 0x000ABACE
		// (set) Token: 0x0600211B RID: 8475 RVA: 0x000AD8D6 File Offset: 0x000ABAD6
		[DataField("containers", false, 1, false, false, null)]
		public HashSet<string> Containers { get; set; } = new HashSet<string>();

		// Token: 0x170004F5 RID: 1269
		// (get) Token: 0x0600211C RID: 8476 RVA: 0x000AD8DF File Offset: 0x000ABADF
		// (set) Token: 0x0600211D RID: 8477 RVA: 0x000AD8E7 File Offset: 0x000ABAE7
		[Nullable(2)]
		[DataField("defaultTarget", false, 1, false, false, null)]
		public string TargetNode { [NullableContext(2)] get; [NullableContext(2)] set; }

		// Token: 0x170004F6 RID: 1270
		// (get) Token: 0x0600211E RID: 8478 RVA: 0x000AD8F0 File Offset: 0x000ABAF0
		// (set) Token: 0x0600211F RID: 8479 RVA: 0x000AD8F8 File Offset: 0x000ABAF8
		[ViewVariables]
		public int? TargetEdgeIndex { get; set; }

		// Token: 0x170004F7 RID: 1271
		// (get) Token: 0x06002120 RID: 8480 RVA: 0x000AD901 File Offset: 0x000ABB01
		// (set) Token: 0x06002121 RID: 8481 RVA: 0x000AD909 File Offset: 0x000ABB09
		[Nullable(new byte[]
		{
			2,
			1
		})]
		[ViewVariables]
		public Queue<string> NodePathfinding { [return: Nullable(new byte[]
		{
			2,
			1
		})] get; [param: Nullable(new byte[]
		{
			2,
			1
		})] set; }

		// Token: 0x170004F8 RID: 1272
		// (get) Token: 0x06002122 RID: 8482 RVA: 0x000AD912 File Offset: 0x000ABB12
		// (set) Token: 0x06002123 RID: 8483 RVA: 0x000AD91A File Offset: 0x000ABB1A
		[Nullable(2)]
		[DataField("deconstructionTarget", false, 1, false, false, null)]
		public string DeconstructionNode { [NullableContext(2)] get; [NullableContext(2)] set; } = "start";

		// Token: 0x170004F9 RID: 1273
		// (get) Token: 0x06002124 RID: 8484 RVA: 0x000AD923 File Offset: 0x000ABB23
		// (set) Token: 0x06002125 RID: 8485 RVA: 0x000AD92B File Offset: 0x000ABB2B
		[ViewVariables]
		public bool WaitingDoAfter { get; set; }

		// Token: 0x04001455 RID: 5205
		[ViewVariables]
		public readonly Queue<object> InteractionQueue = new Queue<object>();
	}
}
