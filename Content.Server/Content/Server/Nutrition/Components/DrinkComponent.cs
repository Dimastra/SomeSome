using System;
using System.Runtime.CompilerServices;
using Content.Server.Nutrition.EntitySystems;
using Content.Shared.FixedPoint;
using Robust.Shared.Analyzers;
using Robust.Shared.Audio;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.ViewVariables;

namespace Content.Server.Nutrition.Components
{
	// Token: 0x02000318 RID: 792
	[NullableContext(1)]
	[Nullable(0)]
	[RegisterComponent]
	[Access(new Type[]
	{
		typeof(DrinkSystem)
	})]
	public sealed class DrinkComponent : Component
	{
		// Token: 0x1700025A RID: 602
		// (get) Token: 0x06001061 RID: 4193 RVA: 0x00054C08 File Offset: 0x00052E08
		// (set) Token: 0x06001062 RID: 4194 RVA: 0x00054C10 File Offset: 0x00052E10
		[DataField("solution", false, 1, false, false, null)]
		public string SolutionName { get; set; } = "drink";

		// Token: 0x1700025B RID: 603
		// (get) Token: 0x06001063 RID: 4195 RVA: 0x00054C19 File Offset: 0x00052E19
		// (set) Token: 0x06001064 RID: 4196 RVA: 0x00054C21 File Offset: 0x00052E21
		[ViewVariables]
		public FixedPoint2 TransferAmount { get; private set; } = FixedPoint2.New(5);

		// Token: 0x0400097D RID: 2429
		public const string DefaultSolutionName = "drink";

		// Token: 0x0400097E RID: 2430
		[DataField("useSound", false, 1, false, false, null)]
		public SoundSpecifier UseSound = new SoundPathSpecifier("/Audio/Items/drink.ogg", null);

		// Token: 0x0400097F RID: 2431
		[DataField("isOpen", false, 1, false, false, null)]
		internal bool DefaultToOpened;

		// Token: 0x04000981 RID: 2433
		[ViewVariables]
		public bool Opened;

		// Token: 0x04000982 RID: 2434
		[DataField("openSounds", false, 1, false, false, null)]
		public SoundSpecifier OpenSounds = new SoundCollectionSpecifier("canOpenSounds", null);

		// Token: 0x04000983 RID: 2435
		[DataField("pressurized", false, 1, false, false, null)]
		public bool Pressurized;

		// Token: 0x04000984 RID: 2436
		[DataField("burstSound", false, 1, false, false, null)]
		public SoundSpecifier BurstSound = new SoundPathSpecifier("/Audio/Effects/flash_bang.ogg", null);

		// Token: 0x04000985 RID: 2437
		[DataField("forceDrink", false, 1, false, false, null)]
		public bool ForceDrink;

		// Token: 0x04000986 RID: 2438
		[DataField("delay", false, 1, false, false, null)]
		public float Delay = 1f;

		// Token: 0x04000987 RID: 2439
		[DataField("forceFeedDelay", false, 1, false, false, null)]
		public float ForceFeedDelay = 3f;
	}
}
