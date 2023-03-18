using System;
using System.Runtime.CompilerServices;
using Content.Shared.Actions.ActionTypes;
using Robust.Shared.GameObjects;
using Robust.Shared.Map;
using Robust.Shared.Serialization;

namespace Content.Shared.Actions
{
	// Token: 0x0200075B RID: 1883
	[NullableContext(1)]
	[Nullable(0)]
	[NetSerializable]
	[Serializable]
	public sealed class RequestPerformActionEvent : EntityEventArgs
	{
		// Token: 0x06001726 RID: 5926 RVA: 0x0004AE69 File Offset: 0x00049069
		public RequestPerformActionEvent(InstantAction action)
		{
			this.Action = action;
		}

		// Token: 0x06001727 RID: 5927 RVA: 0x0004AE78 File Offset: 0x00049078
		public RequestPerformActionEvent(EntityTargetAction action, EntityUid entityTarget)
		{
			this.Action = action;
			this.EntityTarget = new EntityUid?(entityTarget);
		}

		// Token: 0x06001728 RID: 5928 RVA: 0x0004AE93 File Offset: 0x00049093
		public RequestPerformActionEvent(WorldTargetAction action, EntityCoordinates entityCoordinatesTarget)
		{
			this.Action = action;
			this.EntityCoordinatesTarget = new EntityCoordinates?(entityCoordinatesTarget);
		}

		// Token: 0x04001703 RID: 5891
		public readonly ActionType Action;

		// Token: 0x04001704 RID: 5892
		public readonly EntityUid? EntityTarget;

		// Token: 0x04001705 RID: 5893
		public readonly EntityCoordinates? EntityCoordinatesTarget;
	}
}
