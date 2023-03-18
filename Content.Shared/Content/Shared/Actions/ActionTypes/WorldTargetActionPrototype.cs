using System;
using System.Runtime.CompilerServices;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Shared.Actions.ActionTypes
{
	// Token: 0x02000764 RID: 1892
	[NullableContext(1)]
	[Nullable(0)]
	[Prototype("worldTargetAction", 1)]
	public sealed class WorldTargetActionPrototype : WorldTargetAction, IPrototype
	{
		// Token: 0x170004E0 RID: 1248
		// (get) Token: 0x06001746 RID: 5958 RVA: 0x0004BC43 File Offset: 0x00049E43
		[IdDataField(1, null)]
		public string ID { get; }

		// Token: 0x170004E1 RID: 1249
		// (get) Token: 0x06001747 RID: 5959 RVA: 0x0004BC4B File Offset: 0x00049E4B
		// (set) Token: 0x06001748 RID: 5960 RVA: 0x0004BC53 File Offset: 0x00049E53
		[Nullable(2)]
		[DataField("serverEvent", false, 1, false, true, null)]
		public WorldTargetActionEvent ServerEvent
		{
			[NullableContext(2)]
			get
			{
				return this.Event;
			}
			[NullableContext(2)]
			set
			{
				this.Event = value;
			}
		}
	}
}
