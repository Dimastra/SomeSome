using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared.SurveillanceCamera
{
	// Token: 0x020000FE RID: 254
	[NullableContext(1)]
	[Nullable(0)]
	[NetSerializable]
	[Serializable]
	public sealed class SurveillanceCameraSetupBoundUiState : BoundUserInterfaceState
	{
		// Token: 0x17000084 RID: 132
		// (get) Token: 0x060002C9 RID: 713 RVA: 0x0000CC4F File Offset: 0x0000AE4F
		public string Name { get; }

		// Token: 0x17000085 RID: 133
		// (get) Token: 0x060002CA RID: 714 RVA: 0x0000CC57 File Offset: 0x0000AE57
		public uint Network { get; }

		// Token: 0x17000086 RID: 134
		// (get) Token: 0x060002CB RID: 715 RVA: 0x0000CC5F File Offset: 0x0000AE5F
		public List<string> Networks { get; }

		// Token: 0x17000087 RID: 135
		// (get) Token: 0x060002CC RID: 716 RVA: 0x0000CC67 File Offset: 0x0000AE67
		public bool NameDisabled { get; }

		// Token: 0x17000088 RID: 136
		// (get) Token: 0x060002CD RID: 717 RVA: 0x0000CC6F File Offset: 0x0000AE6F
		public bool NetworkDisabled { get; }

		// Token: 0x060002CE RID: 718 RVA: 0x0000CC77 File Offset: 0x0000AE77
		public SurveillanceCameraSetupBoundUiState(string name, uint network, List<string> networks, bool nameDisabled, bool networkDisabled)
		{
			this.Name = name;
			this.Network = network;
			this.Networks = networks;
			this.NameDisabled = nameDisabled;
			this.NetworkDisabled = networkDisabled;
		}
	}
}
