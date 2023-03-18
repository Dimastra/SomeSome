using System;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared.DeviceNetwork
{
	// Token: 0x02000516 RID: 1302
	[NetSerializable]
	[Serializable]
	public sealed class NetworkConfiguratorButtonPressedMessage : BoundUserInterfaceMessage
	{
		// Token: 0x06000FCA RID: 4042 RVA: 0x00032EFD File Offset: 0x000310FD
		public NetworkConfiguratorButtonPressedMessage(NetworkConfiguratorButtonKey buttonKey)
		{
			this.ButtonKey = buttonKey;
		}

		// Token: 0x04000F04 RID: 3844
		public readonly NetworkConfiguratorButtonKey ButtonKey;
	}
}
