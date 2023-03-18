using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading;
using Robust.Shared.Audio;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.ViewVariables;

namespace Content.Server.Forensics
{
	// Token: 0x020004E4 RID: 1252
	[NullableContext(1)]
	[Nullable(0)]
	[RegisterComponent]
	public sealed class ForensicScannerComponent : Component
	{
		// Token: 0x0400102E RID: 4142
		[Nullable(2)]
		public CancellationTokenSource CancelToken;

		// Token: 0x0400102F RID: 4143
		[ViewVariables]
		public List<string> Fingerprints = new List<string>();

		// Token: 0x04001030 RID: 4144
		[ViewVariables]
		public List<string> Fibers = new List<string>();

		// Token: 0x04001031 RID: 4145
		[ViewVariables]
		public string LastScannedName = string.Empty;

		// Token: 0x04001032 RID: 4146
		[ViewVariables]
		public TimeSpan PrintReadyAt = TimeSpan.Zero;

		// Token: 0x04001033 RID: 4147
		[DataField("scanDelay", false, 1, false, false, null)]
		public float ScanDelay = 3f;

		// Token: 0x04001034 RID: 4148
		[DataField("printCooldown", false, 1, false, false, null)]
		public TimeSpan PrintCooldown = TimeSpan.FromSeconds(5.0);

		// Token: 0x04001035 RID: 4149
		[DataField("soundMatch", false, 1, false, false, null)]
		public SoundSpecifier SoundMatch = new SoundPathSpecifier("/Audio/Machines/Nuke/angry_beep.ogg", null);

		// Token: 0x04001036 RID: 4150
		[DataField("soundNoMatch", false, 1, false, false, null)]
		public SoundSpecifier SoundNoMatch = new SoundPathSpecifier("/Audio/Machines/airlock_deny.ogg", null);

		// Token: 0x04001037 RID: 4151
		[DataField("soundPrint", false, 1, false, false, null)]
		public SoundSpecifier SoundPrint = new SoundPathSpecifier("/Audio/Machines/short_print_and_rip.ogg", null);
	}
}
