using System;
using System.Runtime.CompilerServices;
using System.Threading;
using Content.Shared.FixedPoint;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared.DoAfter
{
	// Token: 0x020004F8 RID: 1272
	[NullableContext(2)]
	[Nullable(0)]
	[NetSerializable]
	[Serializable]
	public sealed class DoAfterEventArgs
	{
		// Token: 0x06000F5C RID: 3932 RVA: 0x000318EC File Offset: 0x0002FAEC
		public DoAfterEventArgs(EntityUid user, float delay, CancellationToken cancelToken = default(CancellationToken), EntityUid? target = null, EntityUid? used = null)
		{
			this.User = user;
			this.Delay = delay;
			this.CancelToken = cancelToken;
			this.Target = target;
			this.Used = used;
			this.MovementThreshold = 0.1f;
			this.DamageThreshold = new FixedPoint2?(1.0);
			if (this.Target == null)
			{
				this.BreakOnTargetMove = false;
			}
		}

		// Token: 0x04000EA2 RID: 3746
		public EntityUid User;

		// Token: 0x04000EA3 RID: 3747
		public float Delay;

		// Token: 0x04000EA4 RID: 3748
		public EntityUid? Target;

		// Token: 0x04000EA5 RID: 3749
		public EntityUid? Used;

		// Token: 0x04000EA6 RID: 3750
		public bool RaiseOnUser = true;

		// Token: 0x04000EA7 RID: 3751
		public bool RaiseOnTarget = true;

		// Token: 0x04000EA8 RID: 3752
		public bool RaiseOnUsed = true;

		// Token: 0x04000EA9 RID: 3753
		[NonSerialized]
		public CancellationToken CancelToken;

		// Token: 0x04000EAA RID: 3754
		public bool NeedHand;

		// Token: 0x04000EAB RID: 3755
		public bool BreakOnUserMove;

		// Token: 0x04000EAC RID: 3756
		public bool BreakOnTargetMove;

		// Token: 0x04000EAD RID: 3757
		public float MovementThreshold;

		// Token: 0x04000EAE RID: 3758
		public bool BreakOnDamage;

		// Token: 0x04000EAF RID: 3759
		public FixedPoint2? DamageThreshold;

		// Token: 0x04000EB0 RID: 3760
		public bool BreakOnStun;

		// Token: 0x04000EB1 RID: 3761
		public bool Broadcast;

		// Token: 0x04000EB2 RID: 3762
		public float? DistanceThreshold;

		// Token: 0x04000EB3 RID: 3763
		[NonSerialized]
		public Func<bool> PostCheck;

		// Token: 0x04000EB4 RID: 3764
		[NonSerialized]
		public Func<bool> ExtraCheck;
	}
}
