using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Serialization;
using Robust.Shared.ViewVariables;

namespace Content.Shared.Climbing
{
	// Token: 0x020005C5 RID: 1477
	[NullableContext(1)]
	[Nullable(0)]
	[RegisterComponent]
	[NetworkedComponent]
	public sealed class ClimbingComponent : Component
	{
		// Token: 0x1700038B RID: 907
		// (get) Token: 0x060011E1 RID: 4577 RVA: 0x0003AB3B File Offset: 0x00038D3B
		// (set) Token: 0x060011E2 RID: 4578 RVA: 0x0003AB43 File Offset: 0x00038D43
		[ViewVariables]
		public bool IsClimbing { get; set; }

		// Token: 0x1700038C RID: 908
		// (get) Token: 0x060011E3 RID: 4579 RVA: 0x0003AB4C File Offset: 0x00038D4C
		// (set) Token: 0x060011E4 RID: 4580 RVA: 0x0003AB54 File Offset: 0x00038D54
		[ViewVariables]
		public bool OwnerIsTransitioning { get; set; }

		// Token: 0x1700038D RID: 909
		// (get) Token: 0x060011E5 RID: 4581 RVA: 0x0003AB5D File Offset: 0x00038D5D
		[ViewVariables]
		public Dictionary<string, int> DisabledFixtureMasks { get; } = new Dictionary<string, int>();

		// Token: 0x040010B4 RID: 4276
		public const float BufferTime = 0.3f;

		// Token: 0x02000851 RID: 2129
		[NullableContext(0)]
		[NetSerializable]
		[Serializable]
		public sealed class ClimbModeComponentState : ComponentState
		{
			// Token: 0x06001954 RID: 6484 RVA: 0x0004FDDF File Offset: 0x0004DFDF
			public ClimbModeComponentState(bool climbing, bool isTransitioning)
			{
				this.Climbing = climbing;
				this.IsTransitioning = isTransitioning;
			}

			// Token: 0x17000529 RID: 1321
			// (get) Token: 0x06001955 RID: 6485 RVA: 0x0004FDF5 File Offset: 0x0004DFF5
			public bool Climbing { get; }

			// Token: 0x1700052A RID: 1322
			// (get) Token: 0x06001956 RID: 6486 RVA: 0x0004FDFD File Offset: 0x0004DFFD
			public bool IsTransitioning { get; }
		}
	}
}
