using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Content.Shared.Examine;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Shared.Construction.Steps
{
	// Token: 0x02000571 RID: 1393
	[NullableContext(1)]
	[Nullable(0)]
	[ImplicitDataDefinitionForInheritors]
	[Serializable]
	public abstract class ConstructionGraphStep
	{
		// Token: 0x1700035F RID: 863
		// (get) Token: 0x06001104 RID: 4356 RVA: 0x00038218 File Offset: 0x00036418
		[DataField("doAfter", false, 1, false, false, null)]
		public float DoAfter { get; }

		// Token: 0x17000360 RID: 864
		// (get) Token: 0x06001105 RID: 4357 RVA: 0x00038220 File Offset: 0x00036420
		public IReadOnlyList<IGraphAction> Completed
		{
			get
			{
				return this._completed;
			}
		}

		// Token: 0x06001106 RID: 4358
		public abstract void DoExamine(ExaminedEvent examinedEvent);

		// Token: 0x06001107 RID: 4359
		public abstract ConstructionGuideEntry GenerateGuideEntry();

		// Token: 0x04000FDC RID: 4060
		[DataField("completed", false, 1, false, true, null)]
		private IGraphAction[] _completed = Array.Empty<IGraphAction>();
	}
}
