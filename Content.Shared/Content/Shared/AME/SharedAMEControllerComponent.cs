using System;
using Robust.Shared.Analyzers;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared.AME
{
	// Token: 0x02000716 RID: 1814
	[Virtual]
	public class SharedAMEControllerComponent : Component
	{
		// Token: 0x02000884 RID: 2180
		[NetSerializable]
		[Serializable]
		public sealed class AMEControllerBoundUserInterfaceState : BoundUserInterfaceState
		{
			// Token: 0x060019F1 RID: 6641 RVA: 0x00051AFC File Offset: 0x0004FCFC
			public AMEControllerBoundUserInterfaceState(bool hasPower, bool isMaster, bool injecting, bool hasFuelJar, int fuelAmount, int injectionAmount, int coreCount)
			{
				this.HasPower = hasPower;
				this.IsMaster = isMaster;
				this.Injecting = injecting;
				this.HasFuelJar = hasFuelJar;
				this.FuelAmount = fuelAmount;
				this.InjectionAmount = injectionAmount;
				this.CoreCount = coreCount;
			}

			// Token: 0x04001A4A RID: 6730
			public readonly bool HasPower;

			// Token: 0x04001A4B RID: 6731
			public readonly bool IsMaster;

			// Token: 0x04001A4C RID: 6732
			public readonly bool Injecting;

			// Token: 0x04001A4D RID: 6733
			public readonly bool HasFuelJar;

			// Token: 0x04001A4E RID: 6734
			public readonly int FuelAmount;

			// Token: 0x04001A4F RID: 6735
			public readonly int InjectionAmount;

			// Token: 0x04001A50 RID: 6736
			public readonly int CoreCount;
		}

		// Token: 0x02000885 RID: 2181
		[NetSerializable]
		[Serializable]
		public sealed class UiButtonPressedMessage : BoundUserInterfaceMessage
		{
			// Token: 0x060019F2 RID: 6642 RVA: 0x00051B39 File Offset: 0x0004FD39
			public UiButtonPressedMessage(SharedAMEControllerComponent.UiButton button)
			{
				this.Button = button;
			}

			// Token: 0x04001A51 RID: 6737
			public readonly SharedAMEControllerComponent.UiButton Button;
		}

		// Token: 0x02000886 RID: 2182
		[NetSerializable]
		[Serializable]
		public enum AMEControllerUiKey
		{
			// Token: 0x04001A53 RID: 6739
			Key
		}

		// Token: 0x02000887 RID: 2183
		public enum UiButton
		{
			// Token: 0x04001A55 RID: 6741
			Eject,
			// Token: 0x04001A56 RID: 6742
			ToggleInjection,
			// Token: 0x04001A57 RID: 6743
			IncreaseFuel,
			// Token: 0x04001A58 RID: 6744
			DecreaseFuel
		}

		// Token: 0x02000888 RID: 2184
		[NetSerializable]
		[Serializable]
		public enum AMEControllerVisuals
		{
			// Token: 0x04001A5A RID: 6746
			DisplayState
		}
	}
}
