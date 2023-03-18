using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Shared.Disposal.Components
{
	// Token: 0x02000502 RID: 1282
	[NetworkedComponent]
	public abstract class SharedDisposalUnitComponent : Component
	{
		// Token: 0x06000F7F RID: 3967 RVA: 0x000325DE File Offset: 0x000307DE
		[NullableContext(1)]
		public override ComponentState GetComponentState()
		{
			return new SharedDisposalUnitComponent.DisposalUnitComponentState(this.RecentlyEjected);
		}

		// Token: 0x04000EC9 RID: 3785
		[Nullable(1)]
		public const string ContainerId = "DisposalUnit";

		// Token: 0x04000ECA RID: 3786
		[Nullable(1)]
		public List<EntityUid> RecentlyEjected = new List<EntityUid>();

		// Token: 0x04000ECB RID: 3787
		[DataField("flushTime", false, 1, true, false, null)]
		public readonly float FlushTime;

		// Token: 0x04000ECC RID: 3788
		[DataField("mobsCanEnter", false, 1, false, false, null)]
		public bool MobsCanEnter = true;

		// Token: 0x02000829 RID: 2089
		[NetSerializable]
		[Serializable]
		public enum Visuals : byte
		{
			// Token: 0x0400190C RID: 6412
			VisualState,
			// Token: 0x0400190D RID: 6413
			Handle,
			// Token: 0x0400190E RID: 6414
			Light
		}

		// Token: 0x0200082A RID: 2090
		[NetSerializable]
		[Serializable]
		public enum VisualState : byte
		{
			// Token: 0x04001910 RID: 6416
			UnAnchored,
			// Token: 0x04001911 RID: 6417
			Anchored,
			// Token: 0x04001912 RID: 6418
			Flushing,
			// Token: 0x04001913 RID: 6419
			Charging
		}

		// Token: 0x0200082B RID: 2091
		[NetSerializable]
		[Serializable]
		public enum HandleState : byte
		{
			// Token: 0x04001915 RID: 6421
			Normal,
			// Token: 0x04001916 RID: 6422
			Engaged
		}

		// Token: 0x0200082C RID: 2092
		[NetSerializable]
		[Serializable]
		public enum LightStates : byte
		{
			// Token: 0x04001918 RID: 6424
			Off,
			// Token: 0x04001919 RID: 6425
			Charging,
			// Token: 0x0400191A RID: 6426
			Full,
			// Token: 0x0400191B RID: 6427
			Ready = 4
		}

		// Token: 0x0200082D RID: 2093
		[NetSerializable]
		[Serializable]
		public enum UiButton : byte
		{
			// Token: 0x0400191D RID: 6429
			Eject,
			// Token: 0x0400191E RID: 6430
			Engage,
			// Token: 0x0400191F RID: 6431
			Power
		}

		// Token: 0x0200082E RID: 2094
		[NetSerializable]
		[Serializable]
		public enum PressureState : byte
		{
			// Token: 0x04001921 RID: 6433
			Ready,
			// Token: 0x04001922 RID: 6434
			Pressurizing
		}

		// Token: 0x0200082F RID: 2095
		[NullableContext(1)]
		[Nullable(0)]
		[NetSerializable]
		[Serializable]
		protected sealed class DisposalUnitComponentState : ComponentState
		{
			// Token: 0x06001904 RID: 6404 RVA: 0x0004F61F File Offset: 0x0004D81F
			public DisposalUnitComponentState(List<EntityUid> uids)
			{
				this.RecentlyEjected = uids;
			}

			// Token: 0x04001923 RID: 6435
			public List<EntityUid> RecentlyEjected;
		}

		// Token: 0x02000830 RID: 2096
		[NullableContext(1)]
		[Nullable(0)]
		[NetSerializable]
		[Serializable]
		public sealed class DisposalUnitBoundUserInterfaceState : BoundUserInterfaceState, IEquatable<SharedDisposalUnitComponent.DisposalUnitBoundUserInterfaceState>
		{
			// Token: 0x06001905 RID: 6405 RVA: 0x0004F62E File Offset: 0x0004D82E
			public DisposalUnitBoundUserInterfaceState(string unitName, string unitState, TimeSpan fullPressureTime, bool powered, bool engaged)
			{
				this.UnitName = unitName;
				this.UnitState = unitState;
				this.FullPressureTime = fullPressureTime;
				this.Powered = powered;
				this.Engaged = engaged;
			}

			// Token: 0x06001906 RID: 6406 RVA: 0x0004F65C File Offset: 0x0004D85C
			[NullableContext(2)]
			public bool Equals(SharedDisposalUnitComponent.DisposalUnitBoundUserInterfaceState other)
			{
				return other != null && (this == other || (this.UnitName == other.UnitName && this.UnitState == other.UnitState && this.Powered == other.Powered && this.Engaged == other.Engaged && this.FullPressureTime.Equals(other.FullPressureTime)));
			}

			// Token: 0x04001924 RID: 6436
			public readonly string UnitName;

			// Token: 0x04001925 RID: 6437
			public readonly string UnitState;

			// Token: 0x04001926 RID: 6438
			public readonly TimeSpan FullPressureTime;

			// Token: 0x04001927 RID: 6439
			public readonly bool Powered;

			// Token: 0x04001928 RID: 6440
			public readonly bool Engaged;
		}

		// Token: 0x02000831 RID: 2097
		[NetSerializable]
		[Serializable]
		public sealed class UiButtonPressedMessage : BoundUserInterfaceMessage
		{
			// Token: 0x06001907 RID: 6407 RVA: 0x0004F6C9 File Offset: 0x0004D8C9
			public UiButtonPressedMessage(SharedDisposalUnitComponent.UiButton button)
			{
				this.Button = button;
			}

			// Token: 0x04001929 RID: 6441
			public readonly SharedDisposalUnitComponent.UiButton Button;
		}

		// Token: 0x02000832 RID: 2098
		[NetSerializable]
		[Serializable]
		public enum DisposalUnitUiKey : byte
		{
			// Token: 0x0400192B RID: 6443
			Key
		}
	}
}
