using System;
using System.Runtime.CompilerServices;
using Content.Server.Stunnable.Systems;
using Robust.Shared.Analyzers;
using Robust.Shared.Audio;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.ViewVariables;

namespace Content.Server.Stunnable.Components
{
	// Token: 0x0200014E RID: 334
	[NullableContext(1)]
	[Nullable(0)]
	[RegisterComponent]
	[Access(new Type[]
	{
		typeof(StunbatonSystem)
	})]
	public sealed class StunbatonComponent : Component
	{
		// Token: 0x17000127 RID: 295
		// (get) Token: 0x06000655 RID: 1621 RVA: 0x0001E8CF File Offset: 0x0001CACF
		// (set) Token: 0x06000656 RID: 1622 RVA: 0x0001E8D7 File Offset: 0x0001CAD7
		[ViewVariables]
		[DataField("energyPerUse", false, 1, false, false, null)]
		public float EnergyPerUse { get; set; } = 350f;

		// Token: 0x17000128 RID: 296
		// (get) Token: 0x06000657 RID: 1623 RVA: 0x0001E8E0 File Offset: 0x0001CAE0
		// (set) Token: 0x06000658 RID: 1624 RVA: 0x0001E8E8 File Offset: 0x0001CAE8
		[ViewVariables]
		[DataField("onThrowStunChance", false, 1, false, false, null)]
		public float OnThrowStunChance { get; set; } = 0.2f;

		// Token: 0x17000129 RID: 297
		// (get) Token: 0x06000659 RID: 1625 RVA: 0x0001E8F1 File Offset: 0x0001CAF1
		// (set) Token: 0x0600065A RID: 1626 RVA: 0x0001E8F9 File Offset: 0x0001CAF9
		[DataField("stunSound", false, 1, false, false, null)]
		public SoundSpecifier StunSound { get; set; } = new SoundPathSpecifier("/Audio/Weapons/egloves.ogg", null);

		// Token: 0x1700012A RID: 298
		// (get) Token: 0x0600065B RID: 1627 RVA: 0x0001E902 File Offset: 0x0001CB02
		// (set) Token: 0x0600065C RID: 1628 RVA: 0x0001E90A File Offset: 0x0001CB0A
		[DataField("sparksSound", false, 1, false, false, null)]
		public SoundSpecifier SparksSound { get; set; } = new SoundCollectionSpecifier("sparks", null);

		// Token: 0x1700012B RID: 299
		// (get) Token: 0x0600065D RID: 1629 RVA: 0x0001E913 File Offset: 0x0001CB13
		// (set) Token: 0x0600065E RID: 1630 RVA: 0x0001E91B File Offset: 0x0001CB1B
		[DataField("turnOnFailSound", false, 1, false, false, null)]
		public SoundSpecifier TurnOnFailSound { get; set; } = new SoundPathSpecifier("/Audio/Machines/button.ogg", null);

		// Token: 0x040003AC RID: 940
		public bool Activated;
	}
}
