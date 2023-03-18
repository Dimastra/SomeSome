using System;
using System.Runtime.CompilerServices;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Shared.Actions.ActionTypes
{
	// Token: 0x02000765 RID: 1893
	[NullableContext(1)]
	[Nullable(0)]
	[Prototype("entityTargetAction", 1)]
	public sealed class EntityTargetActionPrototype : EntityTargetAction, IPrototype
	{
		// Token: 0x170004E2 RID: 1250
		// (get) Token: 0x0600174A RID: 5962 RVA: 0x0004BC64 File Offset: 0x00049E64
		[IdDataField(1, null)]
		public string ID { get; }

		// Token: 0x170004E3 RID: 1251
		// (get) Token: 0x0600174B RID: 5963 RVA: 0x0004BC6C File Offset: 0x00049E6C
		// (set) Token: 0x0600174C RID: 5964 RVA: 0x0004BC74 File Offset: 0x00049E74
		[Nullable(2)]
		[DataField("serverEvent", false, 1, false, true, null)]
		public EntityTargetActionEvent ServerEvent
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
