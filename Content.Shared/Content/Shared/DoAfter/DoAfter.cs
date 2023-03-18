using System;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Content.Shared.Hands.Components;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Map;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Timing;

namespace Content.Shared.DoAfter
{
	// Token: 0x020004F1 RID: 1265
	[NullableContext(1)]
	[Nullable(0)]
	[NetSerializable]
	[DataDefinition]
	[Serializable]
	public sealed class DoAfter
	{
		// Token: 0x17000314 RID: 788
		// (get) Token: 0x06000F55 RID: 3925 RVA: 0x0003176E File Offset: 0x0002F96E
		public DoAfterStatus Status
		{
			get
			{
				if (!this.AsTask.IsCompletedSuccessfully)
				{
					return DoAfterStatus.Running;
				}
				return this.AsTask.Result;
			}
		}

		// Token: 0x06000F56 RID: 3926 RVA: 0x0003178C File Offset: 0x0002F98C
		public DoAfter(DoAfterEventArgs eventArgs, IEntityManager entityManager)
		{
			this.EventArgs = eventArgs;
			this.StartTime = IoCManager.Resolve<IGameTiming>().CurTime;
			if (eventArgs.BreakOnUserMove)
			{
				this.UserGrid = entityManager.GetComponent<TransformComponent>(eventArgs.User).Coordinates;
			}
			if (eventArgs.Target != null && eventArgs.BreakOnTargetMove)
			{
				this.TargetGrid = entityManager.GetComponent<TransformComponent>(eventArgs.Target.Value).Coordinates;
			}
			SharedHandsComponent handsComponent;
			if (eventArgs.NeedHand && entityManager.TryGetComponent<SharedHandsComponent>(eventArgs.User, ref handsComponent))
			{
				Hand activeHand = handsComponent.ActiveHand;
				this.ActiveHand = ((activeHand != null) ? activeHand.Name : null);
				this.ActiveItem = handsComponent.ActiveHandEntity;
			}
			this.Tcs = new TaskCompletionSource<DoAfterStatus>();
			this.AsTask = this.Tcs.Task;
		}

		// Token: 0x04000E84 RID: 3716
		[Obsolete]
		[NonSerialized]
		public Task<DoAfterStatus> AsTask;

		// Token: 0x04000E85 RID: 3717
		[Obsolete("Will be obsolete for EventBus")]
		[NonSerialized]
		public TaskCompletionSource<DoAfterStatus> Tcs;

		// Token: 0x04000E86 RID: 3718
		public readonly DoAfterEventArgs EventArgs;

		// Token: 0x04000E87 RID: 3719
		public byte ID;

		// Token: 0x04000E88 RID: 3720
		public bool Cancelled;

		// Token: 0x04000E89 RID: 3721
		public float Delay;

		// Token: 0x04000E8A RID: 3722
		public TimeSpan StartTime;

		// Token: 0x04000E8B RID: 3723
		public TimeSpan CancelledTime;

		// Token: 0x04000E8C RID: 3724
		public TimeSpan Elapsed = TimeSpan.Zero;

		// Token: 0x04000E8D RID: 3725
		public TimeSpan CancelledElapsed = TimeSpan.Zero;

		// Token: 0x04000E8E RID: 3726
		public EntityCoordinates UserGrid;

		// Token: 0x04000E8F RID: 3727
		public EntityCoordinates TargetGrid;

		// Token: 0x04000E90 RID: 3728
		[Nullable(2)]
		[NonSerialized]
		public Action<bool> Done;

		// Token: 0x04000E91 RID: 3729
		[Nullable(2)]
		public readonly string ActiveHand;

		// Token: 0x04000E92 RID: 3730
		public readonly EntityUid? ActiveItem;
	}
}
