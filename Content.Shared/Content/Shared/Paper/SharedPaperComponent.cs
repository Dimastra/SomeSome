using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared.Paper
{
	// Token: 0x020002A4 RID: 676
	public abstract class SharedPaperComponent : Component
	{
		// Token: 0x020007BF RID: 1983
		[NullableContext(1)]
		[Nullable(0)]
		[NetSerializable]
		[Serializable]
		public sealed class PaperBoundUserInterfaceState : BoundUserInterfaceState
		{
			// Token: 0x06001822 RID: 6178 RVA: 0x0004D894 File Offset: 0x0004BA94
			public PaperBoundUserInterfaceState(string text, List<string> stampedBy, SharedPaperComponent.PaperAction mode = SharedPaperComponent.PaperAction.Read)
			{
				this.Text = text;
				this.StampedBy = stampedBy;
				this.Mode = mode;
			}

			// Token: 0x040017F5 RID: 6133
			public readonly string Text;

			// Token: 0x040017F6 RID: 6134
			public readonly List<string> StampedBy;

			// Token: 0x040017F7 RID: 6135
			public readonly SharedPaperComponent.PaperAction Mode;
		}

		// Token: 0x020007C0 RID: 1984
		[NullableContext(1)]
		[Nullable(0)]
		[NetSerializable]
		[Serializable]
		public sealed class PaperInputTextMessage : BoundUserInterfaceMessage
		{
			// Token: 0x06001823 RID: 6179 RVA: 0x0004D8B1 File Offset: 0x0004BAB1
			public PaperInputTextMessage(string text)
			{
				this.Text = text;
			}

			// Token: 0x040017F8 RID: 6136
			public readonly string Text;
		}

		// Token: 0x020007C1 RID: 1985
		[NetSerializable]
		[Serializable]
		public enum PaperUiKey
		{
			// Token: 0x040017FA RID: 6138
			Key
		}

		// Token: 0x020007C2 RID: 1986
		[NetSerializable]
		[Serializable]
		public enum PaperAction
		{
			// Token: 0x040017FC RID: 6140
			Read,
			// Token: 0x040017FD RID: 6141
			Write
		}

		// Token: 0x020007C3 RID: 1987
		[NetSerializable]
		[Serializable]
		public enum PaperVisuals : byte
		{
			// Token: 0x040017FF RID: 6143
			Status,
			// Token: 0x04001800 RID: 6144
			Stamp
		}

		// Token: 0x020007C4 RID: 1988
		[NetSerializable]
		[Serializable]
		public enum PaperStatus : byte
		{
			// Token: 0x04001802 RID: 6146
			Blank,
			// Token: 0x04001803 RID: 6147
			Written
		}
	}
}
