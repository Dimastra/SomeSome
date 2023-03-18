using System;
using Robust.Shared.Analyzers;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Maths;
using Robust.Shared.ViewVariables;

namespace Content.Shared.Jittering
{
	// Token: 0x0200039D RID: 925
	[Access(new Type[]
	{
		typeof(SharedJitteringSystem)
	})]
	[RegisterComponent]
	[NetworkedComponent]
	public sealed class JitteringComponent : Component
	{
		// Token: 0x17000208 RID: 520
		// (get) Token: 0x06000A94 RID: 2708 RVA: 0x00022A40 File Offset: 0x00020C40
		// (set) Token: 0x06000A95 RID: 2709 RVA: 0x00022A48 File Offset: 0x00020C48
		[ViewVariables]
		public float Amplitude { get; set; }

		// Token: 0x17000209 RID: 521
		// (get) Token: 0x06000A96 RID: 2710 RVA: 0x00022A51 File Offset: 0x00020C51
		// (set) Token: 0x06000A97 RID: 2711 RVA: 0x00022A59 File Offset: 0x00020C59
		[ViewVariables]
		public float Frequency { get; set; }

		// Token: 0x1700020A RID: 522
		// (get) Token: 0x06000A98 RID: 2712 RVA: 0x00022A62 File Offset: 0x00020C62
		// (set) Token: 0x06000A99 RID: 2713 RVA: 0x00022A6A File Offset: 0x00020C6A
		[ViewVariables]
		public Vector2 LastJitter { get; set; }
	}
}
