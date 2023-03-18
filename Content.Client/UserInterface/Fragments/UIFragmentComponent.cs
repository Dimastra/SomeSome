using System;
using System.Runtime.CompilerServices;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Client.UserInterface.Fragments
{
	// Token: 0x020000D2 RID: 210
	[RegisterComponent]
	public sealed class UIFragmentComponent : Component
	{
		// Token: 0x040002A5 RID: 677
		[Nullable(2)]
		[DataField("ui", true, 1, false, false, null)]
		public UIFragment Ui;
	}
}
