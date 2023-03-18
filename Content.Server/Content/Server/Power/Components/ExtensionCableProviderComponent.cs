using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Content.Server.Power.EntitySystems;
using Robust.Shared.Analyzers;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.ViewVariables;

namespace Content.Server.Power.Components
{
	// Token: 0x020002B5 RID: 693
	[NullableContext(1)]
	[Nullable(0)]
	[RegisterComponent]
	[Access(new Type[]
	{
		typeof(ExtensionCableSystem)
	})]
	public sealed class ExtensionCableProviderComponent : Component
	{
		// Token: 0x170001EA RID: 490
		// (get) Token: 0x06000DF2 RID: 3570 RVA: 0x00047B8E File Offset: 0x00045D8E
		// (set) Token: 0x06000DF3 RID: 3571 RVA: 0x00047B96 File Offset: 0x00045D96
		[ViewVariables]
		[DataField("transferRange", false, 1, false, false, null)]
		public int TransferRange { get; set; } = 3;

		// Token: 0x170001EB RID: 491
		// (get) Token: 0x06000DF4 RID: 3572 RVA: 0x00047B9F File Offset: 0x00045D9F
		[ViewVariables]
		public List<ExtensionCableReceiverComponent> LinkedReceivers { get; } = new List<ExtensionCableReceiverComponent>();

		// Token: 0x170001EC RID: 492
		// (get) Token: 0x06000DF5 RID: 3573 RVA: 0x00047BA7 File Offset: 0x00045DA7
		// (set) Token: 0x06000DF6 RID: 3574 RVA: 0x00047BAF File Offset: 0x00045DAF
		[ViewVariables]
		public bool Connectable { get; set; } = true;
	}
}
