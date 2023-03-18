using System;
using Content.Server.Administration.Systems;
using Robust.Shared.Analyzers;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Server.Administration.Components
{
	// Token: 0x02000826 RID: 2086
	[RegisterComponent]
	[Access(new Type[]
	{
		typeof(BufferingSystem)
	})]
	public sealed class BufferingComponent : Component
	{
		// Token: 0x04001C44 RID: 7236
		[DataField("minBufferTime", false, 1, false, false, null)]
		public float MinimumBufferTime = 0.5f;

		// Token: 0x04001C45 RID: 7237
		[DataField("maxBufferTime", false, 1, false, false, null)]
		public float MaximumBufferTime = 1.5f;

		// Token: 0x04001C46 RID: 7238
		[DataField("minTimeTilNextBuffer", false, 1, false, false, null)]
		public float MinimumTimeTilNextBuffer = 10f;

		// Token: 0x04001C47 RID: 7239
		[DataField("maxTimeTilNextBuffer", false, 1, false, false, null)]
		public float MaximumTimeTilNextBuffer = 120f;

		// Token: 0x04001C48 RID: 7240
		[DataField("timeTilNextBuffer", false, 1, false, false, null)]
		public float TimeTilNextBuffer = 15f;

		// Token: 0x04001C49 RID: 7241
		[DataField("bufferingIcon", false, 1, false, false, null)]
		public EntityUid? BufferingIcon;

		// Token: 0x04001C4A RID: 7242
		[DataField("bufferingTimer", false, 1, false, false, null)]
		public float BufferingTimer;
	}
}
