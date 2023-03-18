using System;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared.Disposal.Components
{
	// Token: 0x020004FF RID: 1279
	public sealed class SharedDisposalTaggerComponent : Component
	{
		// Token: 0x04000EC3 RID: 3779
		[Nullable(1)]
		public static readonly Regex TagRegex = new Regex("^[a-zA-Z0-9 ]*$", RegexOptions.Compiled);

		// Token: 0x02000825 RID: 2085
		[NullableContext(1)]
		[Nullable(0)]
		[NetSerializable]
		[Serializable]
		public sealed class DisposalTaggerUserInterfaceState : BoundUserInterfaceState
		{
			// Token: 0x06001902 RID: 6402 RVA: 0x0004F5D4 File Offset: 0x0004D7D4
			public DisposalTaggerUserInterfaceState(string tag)
			{
				this.Tag = tag;
			}

			// Token: 0x04001904 RID: 6404
			public readonly string Tag;
		}

		// Token: 0x02000826 RID: 2086
		[NullableContext(1)]
		[Nullable(0)]
		[NetSerializable]
		[Serializable]
		public sealed class UiActionMessage : BoundUserInterfaceMessage
		{
			// Token: 0x06001903 RID: 6403 RVA: 0x0004F5E3 File Offset: 0x0004D7E3
			public UiActionMessage(SharedDisposalTaggerComponent.UiAction action, string tag)
			{
				this.Action = action;
				if (this.Action == SharedDisposalTaggerComponent.UiAction.Ok)
				{
					this.Tag = tag.Substring(0, Math.Min(tag.Length, 30));
				}
			}

			// Token: 0x04001905 RID: 6405
			public readonly SharedDisposalTaggerComponent.UiAction Action;

			// Token: 0x04001906 RID: 6406
			public readonly string Tag = "";
		}

		// Token: 0x02000827 RID: 2087
		[NetSerializable]
		[Serializable]
		public enum UiAction
		{
			// Token: 0x04001908 RID: 6408
			Ok
		}

		// Token: 0x02000828 RID: 2088
		[NetSerializable]
		[Serializable]
		public enum DisposalTaggerUiKey
		{
			// Token: 0x0400190A RID: 6410
			Key
		}
	}
}
