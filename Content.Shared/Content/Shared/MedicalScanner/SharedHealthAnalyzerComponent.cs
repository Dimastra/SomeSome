using System;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared.MedicalScanner
{
	// Token: 0x02000308 RID: 776
	public abstract class SharedHealthAnalyzerComponent : Component
	{
		// Token: 0x020007D6 RID: 2006
		[NetSerializable]
		[Serializable]
		public sealed class HealthAnalyzerScannedUserMessage : BoundUserInterfaceMessage
		{
			// Token: 0x0600183D RID: 6205 RVA: 0x0004DC1A File Offset: 0x0004BE1A
			public HealthAnalyzerScannedUserMessage(EntityUid? targetEntity)
			{
				this.TargetEntity = targetEntity;
			}

			// Token: 0x0400182B RID: 6187
			public readonly EntityUid? TargetEntity;
		}

		// Token: 0x020007D7 RID: 2007
		[NetSerializable]
		[Serializable]
		public enum HealthAnalyzerUiKey : byte
		{
			// Token: 0x0400182D RID: 6189
			Key
		}
	}
}
