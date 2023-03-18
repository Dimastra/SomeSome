using System;
using System.Runtime.CompilerServices;
using Content.Shared.Damage;
using Robust.Shared.Audio;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom.Prototype;
using Robust.Shared.ViewVariables;

namespace Content.Shared.Projectiles
{
	// Token: 0x02000241 RID: 577
	[NullableContext(2)]
	[Nullable(0)]
	[RegisterComponent]
	[NetworkedComponent]
	public sealed class ProjectileComponent : Component
	{
		// Token: 0x1700013D RID: 317
		// (get) Token: 0x06000684 RID: 1668 RVA: 0x000172B4 File Offset: 0x000154B4
		// (set) Token: 0x06000685 RID: 1669 RVA: 0x000172BC File Offset: 0x000154BC
		public EntityUid Shooter { get; set; }

		// Token: 0x1700013E RID: 318
		// (get) Token: 0x06000686 RID: 1670 RVA: 0x000172C5 File Offset: 0x000154C5
		[DataField("deleteOnCollide", false, 1, false, false, null)]
		public bool DeleteOnCollide { get; } = 1;

		// Token: 0x1700013F RID: 319
		// (get) Token: 0x06000687 RID: 1671 RVA: 0x000172CD File Offset: 0x000154CD
		[DataField("ignoreResistances", false, 1, false, false, null)]
		public bool IgnoreResistances { get; }

		// Token: 0x04000673 RID: 1651
		[ViewVariables]
		[DataField("impactEffect", false, 1, false, false, typeof(PrototypeIdSerializer<EntityPrototype>))]
		public string ImpactEffect;

		// Token: 0x04000675 RID: 1653
		public bool IgnoreShooter = true;

		// Token: 0x04000676 RID: 1654
		[Nullable(1)]
		[DataField("damage", false, 1, true, false, null)]
		[ViewVariables]
		public DamageSpecifier Damage;

		// Token: 0x04000679 RID: 1657
		[DataField("soundHit", false, 1, false, false, null)]
		public SoundSpecifier SoundHit;

		// Token: 0x0400067A RID: 1658
		[DataField("soundForce", false, 1, false, false, null)]
		public bool ForceSound;

		// Token: 0x0400067B RID: 1659
		public bool DamagedEntity;
	}
}
