using System;
using System.Runtime.CompilerServices;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;
using Robust.Shared.Utility;

namespace Content.Shared.Xenoarchaeology.Equipment
{
	// Token: 0x0200001E RID: 30
	[NullableContext(2)]
	[Nullable(0)]
	[NetSerializable]
	[Serializable]
	public sealed class AnalysisConsoleScanUpdateState : BoundUserInterfaceState
	{
		// Token: 0x06000026 RID: 38 RVA: 0x0000240C File Offset: 0x0000060C
		public AnalysisConsoleScanUpdateState(EntityUid? artifact, bool analyzerConnected, bool serverConnected, bool canScan, bool canPrint, FormattedMessage scanReport, bool scanning, TimeSpan timeRemaining, TimeSpan totalTime)
		{
			this.Artifact = artifact;
			this.AnalyzerConnected = analyzerConnected;
			this.ServerConnected = serverConnected;
			this.CanScan = canScan;
			this.CanPrint = canPrint;
			this.ScanReport = scanReport;
			this.Scanning = scanning;
			this.TimeRemaining = timeRemaining;
			this.TotalTime = totalTime;
		}

		// Token: 0x04000032 RID: 50
		public EntityUid? Artifact;

		// Token: 0x04000033 RID: 51
		public bool AnalyzerConnected;

		// Token: 0x04000034 RID: 52
		public bool ServerConnected;

		// Token: 0x04000035 RID: 53
		public bool CanScan;

		// Token: 0x04000036 RID: 54
		public bool CanPrint;

		// Token: 0x04000037 RID: 55
		public FormattedMessage ScanReport;

		// Token: 0x04000038 RID: 56
		public bool Scanning;

		// Token: 0x04000039 RID: 57
		public TimeSpan TimeRemaining;

		// Token: 0x0400003A RID: 58
		public TimeSpan TotalTime;
	}
}
