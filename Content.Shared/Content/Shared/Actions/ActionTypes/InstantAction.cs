using System;
using System.Runtime.CompilerServices;
using Robust.Shared.Analyzers;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Shared.Actions.ActionTypes
{
	// Token: 0x02000768 RID: 1896
	[NullableContext(1)]
	[Nullable(0)]
	[NetSerializable]
	[Virtual]
	[Serializable]
	public class InstantAction : ActionType
	{
		// Token: 0x0600175A RID: 5978 RVA: 0x0004BFAF File Offset: 0x0004A1AF
		public InstantAction()
		{
		}

		// Token: 0x0600175B RID: 5979 RVA: 0x0004BFB7 File Offset: 0x0004A1B7
		public InstantAction(InstantAction toClone)
		{
			this.CopyFrom(toClone);
		}

		// Token: 0x0600175C RID: 5980 RVA: 0x0004BFC8 File Offset: 0x0004A1C8
		public override void CopyFrom(object objectToClone)
		{
			base.CopyFrom(objectToClone);
			InstantAction toClone = objectToClone as InstantAction;
			if (toClone == null)
			{
				return;
			}
			if (toClone.Event != null)
			{
				this.Event = toClone.Event;
			}
		}

		// Token: 0x0600175D RID: 5981 RVA: 0x0004BFFB File Offset: 0x0004A1FB
		public override object Clone()
		{
			return new InstantAction(this);
		}

		// Token: 0x04001735 RID: 5941
		[Nullable(2)]
		[DataField("event", false, 1, false, false, null)]
		[NonSerialized]
		public InstantActionEvent Event;
	}
}
