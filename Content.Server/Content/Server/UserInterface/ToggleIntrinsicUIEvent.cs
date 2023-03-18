using System;
using System.Runtime.CompilerServices;
using Content.Shared.Actions;
using Robust.Shared.ViewVariables;

namespace Content.Server.UserInterface
{
	// Token: 0x020000FC RID: 252
	[NullableContext(2)]
	[Nullable(0)]
	public sealed class ToggleIntrinsicUIEvent : InstantActionEvent
	{
		// Token: 0x170000D3 RID: 211
		// (get) Token: 0x0600049D RID: 1181 RVA: 0x0001619A File Offset: 0x0001439A
		// (set) Token: 0x0600049E RID: 1182 RVA: 0x000161A2 File Offset: 0x000143A2
		[ViewVariables]
		public Enum Key { get; set; }
	}
}
