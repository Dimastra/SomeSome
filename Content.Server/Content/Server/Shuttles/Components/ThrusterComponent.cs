using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Content.Server.Shuttles.Systems;
using Content.Shared.Construction.Prototypes;
using Content.Shared.Damage;
using Robust.Shared.Analyzers;
using Robust.Shared.GameObjects;
using Robust.Shared.Maths;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom.Prototype;
using Robust.Shared.ViewVariables;

namespace Content.Server.Shuttles.Components
{
	// Token: 0x0200020C RID: 524
	[NullableContext(1)]
	[Nullable(0)]
	[RegisterComponent]
	[Access(new Type[]
	{
		typeof(ThrusterSystem)
	})]
	public sealed class ThrusterComponent : Component
	{
		// Token: 0x1700018B RID: 395
		// (get) Token: 0x06000A6A RID: 2666 RVA: 0x00037054 File Offset: 0x00035254
		// (set) Token: 0x06000A6B RID: 2667 RVA: 0x0003705C File Offset: 0x0003525C
		[ViewVariables]
		[DataField("enabled", false, 1, false, false, null)]
		public bool Enabled
		{
			get
			{
				return this._enabled;
			}
			set
			{
				if (this._enabled == value)
				{
					return;
				}
				this._enabled = value;
				ThrusterSystem system = EntitySystem.Get<ThrusterSystem>();
				if (!this._enabled)
				{
					system.DisableThruster(base.Owner, this, null, null);
					return;
				}
				if (system.CanEnable(base.Owner, this))
				{
					system.EnableThruster(base.Owner, this, null);
				}
			}
		}

		// Token: 0x04000659 RID: 1625
		private bool _enabled = true;

		// Token: 0x0400065A RID: 1626
		public bool IsOn;

		// Token: 0x0400065B RID: 1627
		[ViewVariables]
		public float Thrust;

		// Token: 0x0400065C RID: 1628
		[DataField("baseThrust", false, 1, false, false, null)]
		[ViewVariables]
		public float BaseThrust = 750f;

		// Token: 0x0400065D RID: 1629
		[DataField("thrusterType", false, 1, false, false, null)]
		public ThrusterType Type;

		// Token: 0x0400065E RID: 1630
		[DataField("burnShape", false, 1, false, false, null)]
		public List<Vector2> BurnPoly = new List<Vector2>
		{
			new Vector2(-0.4f, 0.5f),
			new Vector2(-0.1f, 1.2f),
			new Vector2(0.1f, 1.2f),
			new Vector2(0.4f, 0.5f)
		};

		// Token: 0x0400065F RID: 1631
		[Nullable(2)]
		[DataField("damage", false, 1, false, false, null)]
		public DamageSpecifier Damage = new DamageSpecifier();

		// Token: 0x04000660 RID: 1632
		[DataField("requireSpace", false, 1, false, false, null)]
		public bool RequireSpace = true;

		// Token: 0x04000661 RID: 1633
		public List<EntityUid> Colliding = new List<EntityUid>();

		// Token: 0x04000662 RID: 1634
		public bool Firing;

		// Token: 0x04000663 RID: 1635
		[DataField("machinePartThrust", false, 1, false, false, typeof(PrototypeIdSerializer<MachinePartPrototype>))]
		public string MachinePartThrust = "Laser";

		// Token: 0x04000664 RID: 1636
		[DataField("partRatingThrustMultiplier", false, 1, false, false, null)]
		public float PartRatingThrustMultiplier = 1.5f;
	}
}
