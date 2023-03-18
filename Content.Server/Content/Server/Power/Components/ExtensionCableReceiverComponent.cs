using System;
using System.Runtime.CompilerServices;
using Content.Server.Power.EntitySystems;
using Robust.Shared.Analyzers;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.ViewVariables;

namespace Content.Server.Power.Components
{
	// Token: 0x020002B6 RID: 694
	[NullableContext(2)]
	[Nullable(0)]
	[RegisterComponent]
	[Access(new Type[]
	{
		typeof(ExtensionCableSystem)
	})]
	public sealed class ExtensionCableReceiverComponent : Component
	{
		// Token: 0x170001ED RID: 493
		// (get) Token: 0x06000DF8 RID: 3576 RVA: 0x00047BD9 File Offset: 0x00045DD9
		// (set) Token: 0x06000DF9 RID: 3577 RVA: 0x00047BE1 File Offset: 0x00045DE1
		[ViewVariables]
		public ExtensionCableProviderComponent Provider { get; set; }

		// Token: 0x170001EE RID: 494
		// (get) Token: 0x06000DFA RID: 3578 RVA: 0x00047BEA File Offset: 0x00045DEA
		// (set) Token: 0x06000DFB RID: 3579 RVA: 0x00047BF2 File Offset: 0x00045DF2
		[ViewVariables]
		[DataField("receptionRange", false, 1, false, false, null)]
		public int ReceptionRange { get; set; } = 3;

		// Token: 0x04000843 RID: 2115
		[ViewVariables]
		public bool Connectable;
	}
}
