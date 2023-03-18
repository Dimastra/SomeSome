using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Content.Shared.Construction.Steps;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.ViewVariables;

namespace Content.Shared.Construction
{
	// Token: 0x02000566 RID: 1382
	[NullableContext(1)]
	[Nullable(0)]
	[DataDefinition]
	[Serializable]
	public sealed class ConstructionGraphEdge
	{
		// Token: 0x1700034F RID: 847
		// (get) Token: 0x060010D3 RID: 4307 RVA: 0x00037AF7 File Offset: 0x00035CF7
		[DataField("to", false, 1, true, false, null)]
		public string Target { get; } = string.Empty;

		// Token: 0x17000350 RID: 848
		// (get) Token: 0x060010D4 RID: 4308 RVA: 0x00037AFF File Offset: 0x00035CFF
		[ViewVariables]
		public IReadOnlyList<IGraphCondition> Conditions
		{
			get
			{
				return this._conditions;
			}
		}

		// Token: 0x17000351 RID: 849
		// (get) Token: 0x060010D5 RID: 4309 RVA: 0x00037B07 File Offset: 0x00035D07
		[ViewVariables]
		public IReadOnlyList<IGraphAction> Completed
		{
			get
			{
				return this._completed;
			}
		}

		// Token: 0x17000352 RID: 850
		// (get) Token: 0x060010D6 RID: 4310 RVA: 0x00037B0F File Offset: 0x00035D0F
		[ViewVariables]
		public IReadOnlyList<ConstructionGraphStep> Steps
		{
			get
			{
				return this._steps;
			}
		}

		// Token: 0x04000FCA RID: 4042
		[DataField("steps", false, 1, false, false, null)]
		private ConstructionGraphStep[] _steps = Array.Empty<ConstructionGraphStep>();

		// Token: 0x04000FCB RID: 4043
		[DataField("conditions", false, 1, false, true, null)]
		private IGraphCondition[] _conditions = Array.Empty<IGraphCondition>();

		// Token: 0x04000FCC RID: 4044
		[DataField("completed", false, 1, false, true, null)]
		private IGraphAction[] _completed = Array.Empty<IGraphAction>();
	}
}
