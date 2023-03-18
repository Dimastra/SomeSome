using System;
using System.Runtime.CompilerServices;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Shared.Actions.ActionTypes
{
	// Token: 0x02000769 RID: 1897
	[NetSerializable]
	[Serializable]
	public abstract class TargetedAction : ActionType
	{
		// Token: 0x0600175E RID: 5982 RVA: 0x0004C004 File Offset: 0x0004A204
		[NullableContext(1)]
		public override void CopyFrom(object objectToClone)
		{
			base.CopyFrom(objectToClone);
			TargetedAction toClone = objectToClone as TargetedAction;
			if (toClone == null)
			{
				return;
			}
			this.Range = toClone.Range;
			this.CheckCanAccess = toClone.CheckCanAccess;
			this.DeselectOnMiss = toClone.DeselectOnMiss;
			this.Repeat = toClone.Repeat;
			this.InteractOnMiss = toClone.InteractOnMiss;
			this.TargetingIndicator = toClone.TargetingIndicator;
		}

		// Token: 0x04001736 RID: 5942
		[DataField("repeat", false, 1, false, false, null)]
		public bool Repeat;

		// Token: 0x04001737 RID: 5943
		[DataField("deselectOnMiss", false, 1, false, false, null)]
		public bool DeselectOnMiss;

		// Token: 0x04001738 RID: 5944
		[DataField("checkCanAccess", false, 1, false, false, null)]
		public bool CheckCanAccess = true;

		// Token: 0x04001739 RID: 5945
		[DataField("range", false, 1, false, false, null)]
		public float Range = 1.5f;

		// Token: 0x0400173A RID: 5946
		[DataField("interactOnMiss", false, 1, false, false, null)]
		public bool InteractOnMiss;

		// Token: 0x0400173B RID: 5947
		[DataField("targetingIndicator", false, 1, false, false, null)]
		public bool TargetingIndicator = true;
	}
}
