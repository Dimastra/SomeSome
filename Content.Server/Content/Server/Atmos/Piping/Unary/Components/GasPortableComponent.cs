using System;
using System.Runtime.CompilerServices;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.ViewVariables;

namespace Content.Server.Atmos.Piping.Unary.Components
{
	// Token: 0x02000752 RID: 1874
	[NullableContext(1)]
	[Nullable(0)]
	[RegisterComponent]
	public sealed class GasPortableComponent : Component
	{
		// Token: 0x170005ED RID: 1517
		// (get) Token: 0x06002777 RID: 10103 RVA: 0x000CFB94 File Offset: 0x000CDD94
		// (set) Token: 0x06002778 RID: 10104 RVA: 0x000CFB9C File Offset: 0x000CDD9C
		[ViewVariables]
		[DataField("port", false, 1, false, false, null)]
		public string PortName { get; set; } = "port";
	}
}
