using System;
using System.Runtime.CompilerServices;
using Content.Server.Atmos;
using Content.Server.Body.Systems;
using Content.Shared.Chemistry.Components;
using Robust.Shared.Analyzers;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.ViewVariables;

namespace Content.Server.Body.Components
{
	// Token: 0x02000714 RID: 1812
	[NullableContext(1)]
	[Nullable(0)]
	[RegisterComponent]
	[Access(new Type[]
	{
		typeof(LungSystem)
	})]
	public sealed class LungComponent : Component
	{
		// Token: 0x170005A1 RID: 1441
		// (get) Token: 0x06002625 RID: 9765 RVA: 0x000C96DB File Offset: 0x000C78DB
		// (set) Token: 0x06002626 RID: 9766 RVA: 0x000C96E3 File Offset: 0x000C78E3
		[DataField("air", false, 1, false, false, null)]
		[Access]
		public GasMixture Air { get; set; } = new GasMixture
		{
			Volume = 6f,
			Temperature = 37f
		};

		// Token: 0x040017A2 RID: 6050
		[ViewVariables]
		[Access]
		public Solution LungSolution;
	}
}
