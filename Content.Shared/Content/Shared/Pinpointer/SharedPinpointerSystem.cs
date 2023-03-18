using System;
using System.Runtime.CompilerServices;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Maths;

namespace Content.Shared.Pinpointer
{
	// Token: 0x02000278 RID: 632
	[NullableContext(2)]
	[Nullable(0)]
	public abstract class SharedPinpointerSystem : EntitySystem
	{
		// Token: 0x06000732 RID: 1842 RVA: 0x00018928 File Offset: 0x00016B28
		public override void Initialize()
		{
			base.Initialize();
			base.SubscribeLocalEvent<PinpointerComponent, ComponentGetState>(new ComponentEventRefHandler<PinpointerComponent, ComponentGetState>(this.GetCompState), null, null);
		}

		// Token: 0x06000733 RID: 1843 RVA: 0x00018944 File Offset: 0x00016B44
		[NullableContext(1)]
		private void GetCompState(EntityUid uid, PinpointerComponent pinpointer, ref ComponentGetState args)
		{
			args.State = new PinpointerComponentState
			{
				IsActive = pinpointer.IsActive,
				ArrowAngle = pinpointer.ArrowAngle,
				DistanceToTarget = pinpointer.DistanceToTarget
			};
		}

		// Token: 0x06000734 RID: 1844 RVA: 0x00018975 File Offset: 0x00016B75
		public void SetDistance(EntityUid uid, Distance distance, PinpointerComponent pinpointer = null)
		{
			if (!base.Resolve<PinpointerComponent>(uid, ref pinpointer, true))
			{
				return;
			}
			if (distance == pinpointer.DistanceToTarget)
			{
				return;
			}
			pinpointer.DistanceToTarget = distance;
			base.Dirty(pinpointer, null);
		}

		// Token: 0x06000735 RID: 1845 RVA: 0x0001899D File Offset: 0x00016B9D
		public bool TrySetArrowAngle(EntityUid uid, Angle arrowAngle, PinpointerComponent pinpointer = null)
		{
			if (!base.Resolve<PinpointerComponent>(uid, ref pinpointer, true))
			{
				return false;
			}
			if (pinpointer.ArrowAngle.EqualsApprox(arrowAngle, pinpointer.Precision))
			{
				return false;
			}
			pinpointer.ArrowAngle = arrowAngle;
			base.Dirty(pinpointer, null);
			return true;
		}

		// Token: 0x06000736 RID: 1846 RVA: 0x000189D3 File Offset: 0x00016BD3
		public void SetActive(EntityUid uid, bool isActive, PinpointerComponent pinpointer = null)
		{
			if (!base.Resolve<PinpointerComponent>(uid, ref pinpointer, true))
			{
				return;
			}
			if (isActive == pinpointer.IsActive)
			{
				return;
			}
			pinpointer.IsActive = isActive;
			base.Dirty(pinpointer, null);
		}

		// Token: 0x06000737 RID: 1847 RVA: 0x000189FC File Offset: 0x00016BFC
		public bool TogglePinpointer(EntityUid uid, PinpointerComponent pinpointer = null)
		{
			if (!base.Resolve<PinpointerComponent>(uid, ref pinpointer, true))
			{
				return false;
			}
			bool isActive = !pinpointer.IsActive;
			this.SetActive(uid, isActive, pinpointer);
			return isActive;
		}
	}
}
