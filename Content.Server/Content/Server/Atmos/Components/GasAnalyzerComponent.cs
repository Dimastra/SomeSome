using System;
using Content.Shared.Atmos.Components;
using Robust.Shared.GameObjects;
using Robust.Shared.Map;
using Robust.Shared.ViewVariables;

namespace Content.Server.Atmos.Components
{
	// Token: 0x020007A5 RID: 1957
	[RegisterComponent]
	[ComponentReference(typeof(SharedGasAnalyzerComponent))]
	public sealed class GasAnalyzerComponent : SharedGasAnalyzerComponent
	{
		// Token: 0x04001A42 RID: 6722
		[ViewVariables]
		public EntityUid? Target;

		// Token: 0x04001A43 RID: 6723
		[ViewVariables]
		public EntityUid User;

		// Token: 0x04001A44 RID: 6724
		[ViewVariables]
		public EntityCoordinates? LastPosition;

		// Token: 0x04001A45 RID: 6725
		[ViewVariables]
		public bool Enabled;
	}
}
