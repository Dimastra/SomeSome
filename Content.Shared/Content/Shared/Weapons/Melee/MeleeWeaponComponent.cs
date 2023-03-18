using System;
using System.Runtime.CompilerServices;
using Content.Shared.Damage;
using Content.Shared.FixedPoint;
using Robust.Shared.Audio;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Maths;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom.Prototype;
using Robust.Shared.ViewVariables;

namespace Content.Shared.Weapons.Melee
{
	// Token: 0x0200006D RID: 109
	[NullableContext(1)]
	[Nullable(0)]
	[RegisterComponent]
	[NetworkedComponent]
	public sealed class MeleeWeaponComponent : Component
	{
		// Token: 0x1700002E RID: 46
		// (get) Token: 0x06000144 RID: 324 RVA: 0x000070FD File Offset: 0x000052FD
		// (set) Token: 0x06000145 RID: 325 RVA: 0x00007105 File Offset: 0x00005305
		[ViewVariables]
		[DataField("hidden", false, 1, false, false, null)]
		public bool HideFromExamine { get; set; }

		// Token: 0x1700002F RID: 47
		// (get) Token: 0x06000146 RID: 326 RVA: 0x0000710E File Offset: 0x0000530E
		[ViewVariables]
		public TimeSpan WindupTime
		{
			get
			{
				if (this.AttackRate <= 0f)
				{
					return TimeSpan.Zero;
				}
				return TimeSpan.FromSeconds((double)(1f / this.AttackRate * this.HeavyWindupModifier));
			}
		}

		// Token: 0x17000030 RID: 48
		// (get) Token: 0x06000147 RID: 327 RVA: 0x0000713C File Offset: 0x0000533C
		// (set) Token: 0x06000148 RID: 328 RVA: 0x00007144 File Offset: 0x00005344
		[DataField("bluntStaminaDamageFactor", false, 1, false, false, null)]
		[ViewVariables]
		public FixedPoint2 BluntStaminaDamageFactor { get; set; } = 0.5f;

		// Token: 0x17000031 RID: 49
		// (get) Token: 0x06000149 RID: 329 RVA: 0x0000714D File Offset: 0x0000534D
		// (set) Token: 0x0600014A RID: 330 RVA: 0x00007155 File Offset: 0x00005355
		[ViewVariables]
		[DataField("soundSwing", false, 1, false, false, null)]
		public SoundSpecifier SwingSound { get; set; } = new SoundPathSpecifier("/Audio/Weapons/punchmiss.ogg", null)
		{
			Params = AudioParams.Default.WithVolume(-3f).WithVariation(new float?(0.025f))
		};

		// Token: 0x17000032 RID: 50
		// (get) Token: 0x0600014B RID: 331 RVA: 0x0000715E File Offset: 0x0000535E
		// (set) Token: 0x0600014C RID: 332 RVA: 0x00007166 File Offset: 0x00005366
		[ViewVariables]
		[DataField("soundNoDamage", false, 1, false, false, null)]
		public SoundSpecifier NoDamageSound { get; set; } = new SoundPathSpecifier("/Audio/Weapons/tap.ogg", null);

		// Token: 0x0400015B RID: 347
		[ViewVariables]
		[DataField("nextAttack", false, 1, false, false, typeof(TimeOffsetSerializer))]
		public TimeSpan NextAttack;

		// Token: 0x0400015C RID: 348
		[ViewVariables]
		[DataField("resetOnHandSelected", false, 1, false, false, null)]
		public bool ResetOnHandSelected = true;

		// Token: 0x0400015D RID: 349
		[ViewVariables]
		[DataField("attackRate", false, 1, false, false, null)]
		public float AttackRate = 1f;

		// Token: 0x0400015E RID: 350
		[ViewVariables]
		public bool Attacking;

		// Token: 0x0400015F RID: 351
		[ViewVariables]
		[DataField("canHeavyAttack", false, 1, false, false, null)]
		public bool CanHeavyAttack = true;

		// Token: 0x04000160 RID: 352
		[ViewVariables]
		[DataField("windUpStart", false, 1, false, false, null)]
		public TimeSpan? WindUpStart;

		// Token: 0x04000161 RID: 353
		[ViewVariables]
		[DataField("heavyWindupModifier", false, 1, false, false, null)]
		public float HeavyWindupModifier = 1.5f;

		// Token: 0x04000162 RID: 354
		[ViewVariables]
		[DataField("heavyDamageModifier", false, 1, false, false, null)]
		public FixedPoint2 HeavyDamageModifier = FixedPoint2.New(2);

		// Token: 0x04000163 RID: 355
		[DataField("damage", false, 1, true, false, null)]
		[ViewVariables]
		public DamageSpecifier Damage;

		// Token: 0x04000165 RID: 357
		[ViewVariables]
		[DataField("range", false, 1, false, false, null)]
		public float Range = 1.5f;

		// Token: 0x04000166 RID: 358
		[ViewVariables]
		[DataField("angle", false, 1, false, false, null)]
		public Angle Angle = Angle.FromDegrees(60.0);

		// Token: 0x04000167 RID: 359
		[ViewVariables]
		[DataField("animation", false, 1, false, false, typeof(PrototypeIdSerializer<EntityPrototype>))]
		public string ClickAnimation = "WeaponArcPunch";

		// Token: 0x04000168 RID: 360
		[ViewVariables]
		[DataField("wideAnimation", false, 1, false, false, typeof(PrototypeIdSerializer<EntityPrototype>))]
		public string WideAnimation = "WeaponArcSlash";

		// Token: 0x0400016A RID: 362
		[Nullable(2)]
		[ViewVariables]
		[DataField("soundHit", false, 1, false, false, null)]
		public SoundSpecifier HitSound;
	}
}
