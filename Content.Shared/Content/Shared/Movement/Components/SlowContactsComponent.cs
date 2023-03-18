using System;
using System.Runtime.CompilerServices;
using Content.Shared.Whitelist;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Shared.Movement.Components
{
	// Token: 0x020002F3 RID: 755
	[NetworkedComponent]
	[RegisterComponent]
	public sealed class SlowContactsComponent : Component
	{
		// Token: 0x1700019C RID: 412
		// (get) Token: 0x0600087B RID: 2171 RVA: 0x0001CC94 File Offset: 0x0001AE94
		// (set) Token: 0x0600087C RID: 2172 RVA: 0x0001CC9C File Offset: 0x0001AE9C
		[DataField("walkSpeedModifier", false, 1, false, false, null)]
		public float WalkSpeedModifier { get; set; } = 1f;

		// Token: 0x1700019D RID: 413
		// (get) Token: 0x0600087D RID: 2173 RVA: 0x0001CCA5 File Offset: 0x0001AEA5
		// (set) Token: 0x0600087E RID: 2174 RVA: 0x0001CCAD File Offset: 0x0001AEAD
		[DataField("sprintSpeedModifier", false, 1, false, false, null)]
		public float SprintSpeedModifier { get; set; } = 1f;

		// Token: 0x0400089F RID: 2207
		[Nullable(2)]
		[DataField("ignoreWhitelist", false, 1, false, false, null)]
		public EntityWhitelist IgnoreWhitelist;
	}
}
