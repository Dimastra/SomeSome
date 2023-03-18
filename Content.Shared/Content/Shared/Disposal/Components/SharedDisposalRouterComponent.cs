using System;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared.Disposal.Components
{
	// Token: 0x020004FE RID: 1278
	public sealed class SharedDisposalRouterComponent : Component
	{
		// Token: 0x04000EC2 RID: 3778
		[Nullable(1)]
		public static readonly Regex TagRegex = new Regex("^[a-zA-Z0-9, ]*$", RegexOptions.Compiled);

		// Token: 0x02000821 RID: 2081
		[NullableContext(1)]
		[Nullable(0)]
		[NetSerializable]
		[Serializable]
		public sealed class DisposalRouterUserInterfaceState : BoundUserInterfaceState
		{
			// Token: 0x06001900 RID: 6400 RVA: 0x0004F586 File Offset: 0x0004D786
			public DisposalRouterUserInterfaceState(string tags)
			{
				this.Tags = tags;
			}

			// Token: 0x040018FD RID: 6397
			public readonly string Tags;
		}

		// Token: 0x02000822 RID: 2082
		[NullableContext(1)]
		[Nullable(0)]
		[NetSerializable]
		[Serializable]
		public sealed class UiActionMessage : BoundUserInterfaceMessage
		{
			// Token: 0x06001901 RID: 6401 RVA: 0x0004F595 File Offset: 0x0004D795
			public UiActionMessage(SharedDisposalRouterComponent.UiAction action, string tags)
			{
				this.Action = action;
				if (this.Action == SharedDisposalRouterComponent.UiAction.Ok)
				{
					this.Tags = tags.Substring(0, Math.Min(tags.Length, 150));
				}
			}

			// Token: 0x040018FE RID: 6398
			public readonly SharedDisposalRouterComponent.UiAction Action;

			// Token: 0x040018FF RID: 6399
			public readonly string Tags = "";
		}

		// Token: 0x02000823 RID: 2083
		[NetSerializable]
		[Serializable]
		public enum UiAction
		{
			// Token: 0x04001901 RID: 6401
			Ok
		}

		// Token: 0x02000824 RID: 2084
		[NetSerializable]
		[Serializable]
		public enum DisposalRouterUiKey
		{
			// Token: 0x04001903 RID: 6403
			Key
		}
	}
}
