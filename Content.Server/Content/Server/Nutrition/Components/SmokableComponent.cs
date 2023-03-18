using System;
using System.Runtime.CompilerServices;
using Content.Server.Nutrition.EntitySystems;
using Content.Shared.FixedPoint;
using Content.Shared.Smoking;
using Robust.Shared.Analyzers;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Server.Nutrition.Components
{
	// Token: 0x0200031D RID: 797
	[NullableContext(1)]
	[Nullable(0)]
	[RegisterComponent]
	[Access(new Type[]
	{
		typeof(SmokingSystem)
	})]
	public sealed class SmokableComponent : Component
	{
		// Token: 0x17000268 RID: 616
		// (get) Token: 0x06001086 RID: 4230 RVA: 0x000552DA File Offset: 0x000534DA
		[DataField("solution", false, 1, false, false, null)]
		public string Solution { get; } = "smokable";

		// Token: 0x17000269 RID: 617
		// (get) Token: 0x06001087 RID: 4231 RVA: 0x000552E2 File Offset: 0x000534E2
		[DataField("inhaleAmount", false, 1, false, false, null)]
		public FixedPoint2 InhaleAmount { get; } = FixedPoint2.New(0.05f);

		// Token: 0x1700026A RID: 618
		// (get) Token: 0x06001088 RID: 4232 RVA: 0x000552EA File Offset: 0x000534EA
		// (set) Token: 0x06001089 RID: 4233 RVA: 0x000552F2 File Offset: 0x000534F2
		[DataField("state", false, 1, false, false, null)]
		public SmokableState State { get; set; }

		// Token: 0x1700026B RID: 619
		// (get) Token: 0x0600108A RID: 4234 RVA: 0x000552FB File Offset: 0x000534FB
		// (set) Token: 0x0600108B RID: 4235 RVA: 0x00055303 File Offset: 0x00053503
		[DataField("exposeTemperature", false, 1, false, false, null)]
		public float ExposeTemperature { get; set; }

		// Token: 0x1700026C RID: 620
		// (get) Token: 0x0600108C RID: 4236 RVA: 0x0005530C File Offset: 0x0005350C
		// (set) Token: 0x0600108D RID: 4237 RVA: 0x00055314 File Offset: 0x00053514
		[DataField("exposeVolume", false, 1, false, false, null)]
		public float ExposeVolume { get; set; } = 1f;

		// Token: 0x040009A6 RID: 2470
		[DataField("burntPrefix", false, 1, false, false, null)]
		public string BurntPrefix = "unlit";

		// Token: 0x040009A7 RID: 2471
		[DataField("litPrefix", false, 1, false, false, null)]
		public string LitPrefix = "lit";

		// Token: 0x040009A8 RID: 2472
		[DataField("unlitPrefix", false, 1, false, false, null)]
		public string UnlitPrefix = "unlit";
	}
}
