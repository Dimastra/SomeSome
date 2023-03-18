using System;
using System.Runtime.CompilerServices;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared.VoiceMask
{
	// Token: 0x02000083 RID: 131
	[NullableContext(1)]
	[Nullable(0)]
	[NetSerializable]
	[Serializable]
	public sealed class VoiceMaskBuiState : BoundUserInterfaceState
	{
		// Token: 0x1700003B RID: 59
		// (get) Token: 0x0600018A RID: 394 RVA: 0x00008DED File Offset: 0x00006FED
		public string Name { get; }

		// Token: 0x1700003C RID: 60
		// (get) Token: 0x0600018B RID: 395 RVA: 0x00008DF5 File Offset: 0x00006FF5
		public string Voice { get; }

		// Token: 0x0600018C RID: 396 RVA: 0x00008DFD File Offset: 0x00006FFD
		public VoiceMaskBuiState(string name, string voice)
		{
			this.Name = name;
			this.Voice = voice;
		}
	}
}
