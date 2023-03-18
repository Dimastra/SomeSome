using System;
using System.Runtime.CompilerServices;
using Robust.Shared.Analyzers;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Shared.Actions.ActionTypes
{
	// Token: 0x0200076B RID: 1899
	[NullableContext(1)]
	[Nullable(0)]
	[NetSerializable]
	[Virtual]
	[Serializable]
	public class WorldTargetAction : TargetedAction
	{
		// Token: 0x06001764 RID: 5988 RVA: 0x0004C107 File Offset: 0x0004A307
		public WorldTargetAction()
		{
		}

		// Token: 0x06001765 RID: 5989 RVA: 0x0004C10F File Offset: 0x0004A30F
		public WorldTargetAction(WorldTargetAction toClone)
		{
			this.CopyFrom(toClone);
		}

		// Token: 0x06001766 RID: 5990 RVA: 0x0004C120 File Offset: 0x0004A320
		public override void CopyFrom(object objectToClone)
		{
			base.CopyFrom(objectToClone);
			WorldTargetAction toClone = objectToClone as WorldTargetAction;
			if (toClone == null)
			{
				return;
			}
			if (toClone.Event != null)
			{
				this.Event = toClone.Event;
			}
		}

		// Token: 0x06001767 RID: 5991 RVA: 0x0004C153 File Offset: 0x0004A353
		public override object Clone()
		{
			return new WorldTargetAction(this);
		}

		// Token: 0x0400173F RID: 5951
		[Nullable(2)]
		[DataField("event", false, 1, false, false, null)]
		[NonSerialized]
		public WorldTargetActionEvent Event;
	}
}
