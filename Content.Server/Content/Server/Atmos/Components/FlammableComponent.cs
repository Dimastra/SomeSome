using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Content.Shared.Damage;
using Robust.Shared.GameObjects;
using Robust.Shared.Physics.Collision.Shapes;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.ViewVariables;

namespace Content.Server.Atmos.Components
{
	// Token: 0x020007A4 RID: 1956
	[NullableContext(1)]
	[Nullable(0)]
	[RegisterComponent]
	public sealed class FlammableComponent : Component
	{
		// Token: 0x17000666 RID: 1638
		// (get) Token: 0x06002A6B RID: 10859 RVA: 0x000DFB85 File Offset: 0x000DDD85
		// (set) Token: 0x06002A6C RID: 10860 RVA: 0x000DFB8D File Offset: 0x000DDD8D
		[ViewVariables]
		public bool OnFire { get; set; }

		// Token: 0x17000667 RID: 1639
		// (get) Token: 0x06002A6D RID: 10861 RVA: 0x000DFB96 File Offset: 0x000DDD96
		// (set) Token: 0x06002A6E RID: 10862 RVA: 0x000DFB9E File Offset: 0x000DDD9E
		[ViewVariables]
		public float FireStacks { get; set; }

		// Token: 0x17000668 RID: 1640
		// (get) Token: 0x06002A6F RID: 10863 RVA: 0x000DFBA7 File Offset: 0x000DDDA7
		// (set) Token: 0x06002A70 RID: 10864 RVA: 0x000DFBAF File Offset: 0x000DDDAF
		[ViewVariables]
		[DataField("fireSpread", false, 1, false, false, null)]
		public bool FireSpread { get; private set; }

		// Token: 0x17000669 RID: 1641
		// (get) Token: 0x06002A71 RID: 10865 RVA: 0x000DFBB8 File Offset: 0x000DDDB8
		// (set) Token: 0x06002A72 RID: 10866 RVA: 0x000DFBC0 File Offset: 0x000DDDC0
		[ViewVariables]
		[DataField("canResistFire", false, 1, false, false, null)]
		public bool CanResistFire { get; private set; }

		// Token: 0x04001A3A RID: 6714
		[ViewVariables]
		public bool Resisting;

		// Token: 0x04001A3B RID: 6715
		[ViewVariables]
		public readonly List<EntityUid> Collided = new List<EntityUid>();

		// Token: 0x04001A40 RID: 6720
		[DataField("damage", false, 1, true, false, null)]
		[ViewVariables]
		public DamageSpecifier Damage = new DamageSpecifier();

		// Token: 0x04001A41 RID: 6721
		[DataField("flammableCollisionShape", false, 1, false, false, null)]
		public IPhysShape FlammableCollisionShape = new PhysShapeCircle(0.35f);
	}
}
