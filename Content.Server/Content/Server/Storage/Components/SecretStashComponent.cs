using System;
using System.Runtime.CompilerServices;
using Content.Server.Storage.EntitySystems;
using Robust.Shared.Analyzers;
using Robust.Shared.Containers;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.ViewVariables;

namespace Content.Server.Storage.Components
{
	// Token: 0x0200016D RID: 365
	[NullableContext(1)]
	[Nullable(0)]
	[RegisterComponent]
	[Access(new Type[]
	{
		typeof(SecretStashSystem)
	})]
	public sealed class SecretStashComponent : Component
	{
		// Token: 0x1700013F RID: 319
		// (get) Token: 0x06000741 RID: 1857 RVA: 0x000242E9 File Offset: 0x000224E9
		// (set) Token: 0x06000742 RID: 1858 RVA: 0x000242F1 File Offset: 0x000224F1
		[DataField("secretPartName", true, 1, false, false, null)]
		public string SecretPartName { get; set; } = "";

		// Token: 0x04000445 RID: 1093
		[DataField("maxItemSize", false, 1, false, false, null)]
		public int MaxItemSize = 12;

		// Token: 0x04000447 RID: 1095
		[ViewVariables]
		public ContainerSlot ItemContainer;
	}
}
