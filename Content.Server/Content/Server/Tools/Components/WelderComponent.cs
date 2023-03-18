using System;
using System.Runtime.CompilerServices;
using Content.Shared.Chemistry.Reagent;
using Content.Shared.Damage;
using Content.Shared.FixedPoint;
using Content.Shared.Tools.Components;
using Robust.Shared.Audio;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom.Prototype;

namespace Content.Server.Tools.Components
{
	// Token: 0x0200011B RID: 283
	[NullableContext(1)]
	[Nullable(0)]
	[RegisterComponent]
	public sealed class WelderComponent : SharedWelderComponent
	{
		// Token: 0x170000E4 RID: 228
		// (get) Token: 0x06000517 RID: 1303 RVA: 0x00018C77 File Offset: 0x00016E77
		[DataField("fuelSolution", false, 1, false, false, null)]
		public string FuelSolution { get; } = "Welder";

		// Token: 0x170000E5 RID: 229
		// (get) Token: 0x06000518 RID: 1304 RVA: 0x00018C7F File Offset: 0x00016E7F
		[DataField("fuelReagent", false, 1, false, false, typeof(PrototypeIdSerializer<ReagentPrototype>))]
		public string FuelReagent { get; } = "WeldingFuel";

		// Token: 0x170000E6 RID: 230
		// (get) Token: 0x06000519 RID: 1305 RVA: 0x00018C87 File Offset: 0x00016E87
		[DataField("fuelConsumption", false, 1, false, false, null)]
		public FixedPoint2 FuelConsumption { get; } = FixedPoint2.New(0.05f);

		// Token: 0x170000E7 RID: 231
		// (get) Token: 0x0600051A RID: 1306 RVA: 0x00018C8F File Offset: 0x00016E8F
		[DataField("welderOnConsume", false, 1, false, false, null)]
		public FixedPoint2 FuelLitCost { get; } = FixedPoint2.New(0.5f);

		// Token: 0x170000E8 RID: 232
		// (get) Token: 0x0600051B RID: 1307 RVA: 0x00018C97 File Offset: 0x00016E97
		[DataField("welderOffSounds", false, 1, false, false, null)]
		public SoundSpecifier WelderOffSounds { get; } = new SoundCollectionSpecifier("WelderOff", null);

		// Token: 0x170000E9 RID: 233
		// (get) Token: 0x0600051C RID: 1308 RVA: 0x00018C9F File Offset: 0x00016E9F
		[DataField("welderOnSounds", false, 1, false, false, null)]
		public SoundSpecifier WelderOnSounds { get; } = new SoundCollectionSpecifier("WelderOn", null);

		// Token: 0x170000EA RID: 234
		// (get) Token: 0x0600051D RID: 1309 RVA: 0x00018CA7 File Offset: 0x00016EA7
		[DataField("welderRefill", false, 1, false, false, null)]
		public SoundSpecifier WelderRefill { get; } = new SoundPathSpecifier("/Audio/Effects/refill.ogg", null);

		// Token: 0x0400030A RID: 778
		[DataField("litMeleeDamageBonus", false, 1, false, false, null)]
		public DamageSpecifier LitMeleeDamageBonus = new DamageSpecifier();

		// Token: 0x0400030B RID: 779
		[DataField("tankSafe", false, 1, false, false, null)]
		public bool TankSafe;
	}
}
