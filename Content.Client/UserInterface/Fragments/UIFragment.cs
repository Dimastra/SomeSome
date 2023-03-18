using System;
using System.Runtime.CompilerServices;
using Robust.Client.GameObjects;
using Robust.Client.UserInterface;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Client.UserInterface.Fragments
{
	// Token: 0x020000D1 RID: 209
	[NullableContext(1)]
	[Nullable(0)]
	[ImplicitDataDefinitionForInheritors]
	public abstract class UIFragment
	{
		// Token: 0x060005E4 RID: 1508
		public abstract Control GetUIFragmentRoot();

		// Token: 0x060005E5 RID: 1509
		public abstract void Setup(BoundUserInterface userInterface, EntityUid? fragmentOwner);

		// Token: 0x060005E6 RID: 1510
		public abstract void UpdateState(BoundUserInterfaceState state);
	}
}
