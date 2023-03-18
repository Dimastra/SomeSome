using System;
using System.Runtime.CompilerServices;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom.Prototype;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom.Prototype.Array;
using Robust.Shared.ViewVariables;

namespace Content.Shared.Polymorph
{
	// Token: 0x02000263 RID: 611
	[NullableContext(1)]
	[Nullable(0)]
	[Prototype("polymorph", 1)]
	[DataDefinition]
	public sealed class PolymorphPrototype : IPrototype, IInheritingPrototype
	{
		// Token: 0x17000160 RID: 352
		// (get) Token: 0x0600070A RID: 1802 RVA: 0x000184A8 File Offset: 0x000166A8
		[ViewVariables]
		[IdDataField(1, null)]
		public string ID { get; }

		// Token: 0x17000161 RID: 353
		// (get) Token: 0x0600070B RID: 1803 RVA: 0x000184B0 File Offset: 0x000166B0
		[DataField("name", false, 1, false, false, null)]
		public string Name { get; } = string.Empty;

		// Token: 0x17000162 RID: 354
		// (get) Token: 0x0600070C RID: 1804 RVA: 0x000184B8 File Offset: 0x000166B8
		// (set) Token: 0x0600070D RID: 1805 RVA: 0x000184C0 File Offset: 0x000166C0
		[Nullable(new byte[]
		{
			2,
			1
		})]
		[ParentDataField(typeof(AbstractPrototypeIdArraySerializer<PolymorphPrototype>), 1)]
		public string[] Parents { [return: Nullable(new byte[]
		{
			2,
			1
		})] get; [param: Nullable(new byte[]
		{
			2,
			1
		})] private set; }

		// Token: 0x17000163 RID: 355
		// (get) Token: 0x0600070E RID: 1806 RVA: 0x000184C9 File Offset: 0x000166C9
		// (set) Token: 0x0600070F RID: 1807 RVA: 0x000184D1 File Offset: 0x000166D1
		[NeverPushInheritance]
		[AbstractDataField(1)]
		public bool Abstract { get; private set; }

		// Token: 0x040006E1 RID: 1761
		[DataField("entity", false, 1, true, true, typeof(PrototypeIdSerializer<EntityPrototype>))]
		public string Entity = string.Empty;

		// Token: 0x040006E2 RID: 1762
		[DataField("delay", false, 1, false, true, null)]
		public int Delay = 60;

		// Token: 0x040006E3 RID: 1763
		[DataField("duration", false, 1, false, true, null)]
		public int? Duration;

		// Token: 0x040006E4 RID: 1764
		[DataField("forced", false, 1, false, true, null)]
		public bool Forced;

		// Token: 0x040006E5 RID: 1765
		[DataField("transferDamage", false, 1, false, true, null)]
		public bool TransferDamage = true;

		// Token: 0x040006E6 RID: 1766
		[DataField("transferName", false, 1, false, true, null)]
		public bool TransferName;

		// Token: 0x040006E7 RID: 1767
		[DataField("transferHumanoidAppearance", false, 1, false, true, null)]
		public bool TransferHumanoidAppearance;

		// Token: 0x040006E8 RID: 1768
		[DataField("inventory", false, 1, false, true, null)]
		public PolymorphInventoryChange Inventory;

		// Token: 0x040006E9 RID: 1769
		[DataField("revertOnCrit", false, 1, false, true, null)]
		public bool RevertOnCrit = true;

		// Token: 0x040006EA RID: 1770
		[DataField("revertOnDeath", false, 1, false, true, null)]
		public bool RevertOnDeath = true;

		// Token: 0x040006EB RID: 1771
		[DataField("allowRepeatedMorphs", false, 1, false, true, null)]
		public bool AllowRepeatedMorphs;
	}
}
