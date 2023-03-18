using System;
using System.Runtime.CompilerServices;
using Content.Shared.Whitelist;
using Robust.Shared.Analyzers;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Shared.Actions.ActionTypes
{
	// Token: 0x0200076A RID: 1898
	[NullableContext(1)]
	[Nullable(0)]
	[NetSerializable]
	[Virtual]
	[Serializable]
	public class EntityTargetAction : TargetedAction
	{
		// Token: 0x06001760 RID: 5984 RVA: 0x0004C08C File Offset: 0x0004A28C
		public EntityTargetAction()
		{
		}

		// Token: 0x06001761 RID: 5985 RVA: 0x0004C09B File Offset: 0x0004A29B
		public EntityTargetAction(EntityTargetAction toClone)
		{
			this.CopyFrom(toClone);
		}

		// Token: 0x06001762 RID: 5986 RVA: 0x0004C0B4 File Offset: 0x0004A2B4
		public override void CopyFrom(object objectToClone)
		{
			base.CopyFrom(objectToClone);
			EntityTargetAction toClone = objectToClone as EntityTargetAction;
			if (toClone == null)
			{
				return;
			}
			this.CanTargetSelf = toClone.CanTargetSelf;
			this.Whitelist = toClone.Whitelist;
			if (toClone.Event != null)
			{
				this.Event = toClone.Event;
			}
		}

		// Token: 0x06001763 RID: 5987 RVA: 0x0004C0FF File Offset: 0x0004A2FF
		public override object Clone()
		{
			return new EntityTargetAction(this);
		}

		// Token: 0x0400173C RID: 5948
		[Nullable(2)]
		[DataField("event", false, 1, false, false, null)]
		[NonSerialized]
		public EntityTargetActionEvent Event;

		// Token: 0x0400173D RID: 5949
		[Nullable(2)]
		[DataField("whitelist", false, 1, false, false, null)]
		public EntityWhitelist Whitelist;

		// Token: 0x0400173E RID: 5950
		[DataField("canTargetSelf", false, 1, false, false, null)]
		public bool CanTargetSelf = true;
	}
}
