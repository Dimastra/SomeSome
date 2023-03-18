using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Content.Shared.Disposal.Components;
using Robust.Shared.Analyzers;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.ViewVariables;

namespace Content.Server.Disposal.Mailing
{
	// Token: 0x0200055F RID: 1375
	[NullableContext(2)]
	[Nullable(0)]
	[Access(new Type[]
	{
		typeof(MailingUnitSystem)
	})]
	[RegisterComponent]
	public sealed class MailingUnitComponent : Component
	{
		// Token: 0x0400128A RID: 4746
		[Nullable(1)]
		[DataField("targetList", false, 1, false, false, null)]
		public readonly List<string> TargetList = new List<string>();

		// Token: 0x0400128B RID: 4747
		[DataField("target", false, 1, false, false, null)]
		public string Target;

		// Token: 0x0400128C RID: 4748
		[ViewVariables]
		[DataField("tag", false, 1, false, false, null)]
		public string Tag;

		// Token: 0x0400128D RID: 4749
		public SharedDisposalUnitComponent.DisposalUnitBoundUserInterfaceState DisposalUnitInterfaceState;
	}
}
