using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Content.Shared.Damage;
using Robust.Shared.Audio;
using Robust.Shared.GameObjects;
using Robust.Shared.Maths;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Server.Weapons.Melee.EnergySword
{
	// Token: 0x020000B8 RID: 184
	[NullableContext(1)]
	[Nullable(0)]
	[RegisterComponent]
	internal sealed class EnergySwordComponent : Component
	{
		// Token: 0x17000062 RID: 98
		// (get) Token: 0x060002F5 RID: 757 RVA: 0x00010690 File Offset: 0x0000E890
		// (set) Token: 0x060002F6 RID: 758 RVA: 0x00010698 File Offset: 0x0000E898
		[DataField("secret", false, 1, false, false, null)]
		public bool Secret { get; set; }

		// Token: 0x17000063 RID: 99
		// (get) Token: 0x060002F7 RID: 759 RVA: 0x000106A1 File Offset: 0x0000E8A1
		// (set) Token: 0x060002F8 RID: 760 RVA: 0x000106A9 File Offset: 0x0000E8A9
		[DataField("activateSound", false, 1, false, false, null)]
		public SoundSpecifier ActivateSound { get; set; } = new SoundPathSpecifier("/Audio/Weapons/ebladeon.ogg", null);

		// Token: 0x17000064 RID: 100
		// (get) Token: 0x060002F9 RID: 761 RVA: 0x000106B2 File Offset: 0x0000E8B2
		// (set) Token: 0x060002FA RID: 762 RVA: 0x000106BA File Offset: 0x0000E8BA
		[DataField("deActivateSound", false, 1, false, false, null)]
		public SoundSpecifier DeActivateSound { get; set; } = new SoundPathSpecifier("/Audio/Weapons/ebladeoff.ogg", null);

		// Token: 0x17000065 RID: 101
		// (get) Token: 0x060002FB RID: 763 RVA: 0x000106C3 File Offset: 0x0000E8C3
		// (set) Token: 0x060002FC RID: 764 RVA: 0x000106CB File Offset: 0x0000E8CB
		[DataField("onHitOn", false, 1, false, false, null)]
		public SoundSpecifier OnHitOn { get; set; } = new SoundPathSpecifier("/Audio/Weapons/eblade1.ogg", null);

		// Token: 0x17000066 RID: 102
		// (get) Token: 0x060002FD RID: 765 RVA: 0x000106D4 File Offset: 0x0000E8D4
		// (set) Token: 0x060002FE RID: 766 RVA: 0x000106DC File Offset: 0x0000E8DC
		[DataField("onHitOff", false, 1, false, false, null)]
		public SoundSpecifier OnHitOff { get; set; } = new SoundPathSpecifier("/Audio/Weapons/genhit1.ogg", null);

		// Token: 0x04000204 RID: 516
		public Color BladeColor = Color.DodgerBlue;

		// Token: 0x04000205 RID: 517
		public bool Hacked;

		// Token: 0x04000206 RID: 518
		public bool Activated;

		// Token: 0x04000207 RID: 519
		[DataField("isSharp", false, 1, false, false, null)]
		public bool IsSharp = true;

		// Token: 0x04000209 RID: 521
		[DataField("cycleRate", false, 1, false, false, null)]
		public float CycleRate = 1f;

		// Token: 0x0400020E RID: 526
		[DataField("colorOptions", false, 1, false, false, null)]
		public List<Color> ColorOptions = new List<Color>
		{
			Color.Tomato,
			Color.DodgerBlue,
			Color.Aqua,
			Color.MediumSpringGreen,
			Color.MediumOrchid
		};

		// Token: 0x0400020F RID: 527
		[DataField("litDamageBonus", false, 1, false, false, null)]
		public DamageSpecifier LitDamageBonus = new DamageSpecifier();

		// Token: 0x04000210 RID: 528
		[DataField("litDisarmMalus", false, 1, false, false, null)]
		public float LitDisarmMalus = 0.6f;
	}
}
