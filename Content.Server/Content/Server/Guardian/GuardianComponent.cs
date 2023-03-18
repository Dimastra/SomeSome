using System;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Server.Guardian
{
	// Token: 0x02000482 RID: 1154
	[RegisterComponent]
	public sealed class GuardianComponent : Component
	{
		// Token: 0x17000318 RID: 792
		// (get) Token: 0x06001708 RID: 5896 RVA: 0x000793B5 File Offset: 0x000775B5
		// (set) Token: 0x06001709 RID: 5897 RVA: 0x000793BD File Offset: 0x000775BD
		[DataField("damageShare", false, 1, false, false, null)]
		public float DamageShare { get; set; } = 0.85f;

		// Token: 0x17000319 RID: 793
		// (get) Token: 0x0600170A RID: 5898 RVA: 0x000793C6 File Offset: 0x000775C6
		// (set) Token: 0x0600170B RID: 5899 RVA: 0x000793CE File Offset: 0x000775CE
		[DataField("distanceAllowed", false, 1, false, false, null)]
		public float DistanceAllowed { get; set; } = 5f;

		// Token: 0x04000E71 RID: 3697
		public EntityUid Host;

		// Token: 0x04000E74 RID: 3700
		public bool GuardianLoose;
	}
}
