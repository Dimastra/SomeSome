using System;
using System.Runtime.CompilerServices;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Shared.Actions.ActionTypes
{
	// Token: 0x02000766 RID: 1894
	[NullableContext(1)]
	[Nullable(0)]
	[Prototype("instantAction", 1)]
	public sealed class InstantActionPrototype : InstantAction, IPrototype
	{
		// Token: 0x170004E4 RID: 1252
		// (get) Token: 0x0600174E RID: 5966 RVA: 0x0004BC85 File Offset: 0x00049E85
		[IdDataField(1, null)]
		public string ID { get; }

		// Token: 0x170004E5 RID: 1253
		// (get) Token: 0x0600174F RID: 5967 RVA: 0x0004BC8D File Offset: 0x00049E8D
		// (set) Token: 0x06001750 RID: 5968 RVA: 0x0004BC95 File Offset: 0x00049E95
		[Nullable(2)]
		[DataField("serverEvent", false, 1, false, true, null)]
		public InstantActionEvent ServerEvent
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
