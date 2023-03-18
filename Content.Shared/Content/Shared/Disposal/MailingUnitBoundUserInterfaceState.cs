using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Content.Shared.Disposal.Components;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared.Disposal
{
	// Token: 0x020004FA RID: 1274
	[NullableContext(1)]
	[Nullable(0)]
	[NetSerializable]
	[Serializable]
	public sealed class MailingUnitBoundUserInterfaceState : BoundUserInterfaceState, IEquatable<MailingUnitBoundUserInterfaceState>
	{
		// Token: 0x06000F72 RID: 3954 RVA: 0x000323B6 File Offset: 0x000305B6
		public MailingUnitBoundUserInterfaceState(SharedDisposalUnitComponent.DisposalUnitBoundUserInterfaceState disposalState, [Nullable(2)] string target, List<string> targetList, [Nullable(2)] string tag)
		{
			this.DisposalState = disposalState;
			this.Target = target;
			this.TargetList = targetList;
			this.Tag = tag;
		}

		// Token: 0x06000F73 RID: 3955 RVA: 0x000323DC File Offset: 0x000305DC
		[NullableContext(2)]
		public bool Equals(MailingUnitBoundUserInterfaceState other)
		{
			return other != null && (this == other || (this.DisposalState.Equals(other.DisposalState) && this.Target == other.Target && this.TargetList.Equals(other.TargetList) && this.Tag == other.Tag));
		}

		// Token: 0x04000EB7 RID: 3767
		[Nullable(2)]
		public string Target;

		// Token: 0x04000EB8 RID: 3768
		public List<string> TargetList;

		// Token: 0x04000EB9 RID: 3769
		[Nullable(2)]
		public string Tag;

		// Token: 0x04000EBA RID: 3770
		public SharedDisposalUnitComponent.DisposalUnitBoundUserInterfaceState DisposalState;
	}
}
