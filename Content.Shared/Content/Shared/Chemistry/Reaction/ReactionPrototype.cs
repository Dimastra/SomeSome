using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Content.Shared.Chemistry.Reagent;
using Content.Shared.Database;
using Content.Shared.FixedPoint;
using Robust.Shared.Audio;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom.Prototype.Dictionary;
using Robust.Shared.ViewVariables;

namespace Content.Shared.Chemistry.Reaction
{
	// Token: 0x020005EF RID: 1519
	[NullableContext(1)]
	[Nullable(0)]
	[Prototype("reaction", 1)]
	public sealed class ReactionPrototype : IPrototype, IComparable<ReactionPrototype>
	{
		// Token: 0x170003B1 RID: 945
		// (get) Token: 0x06001271 RID: 4721 RVA: 0x0003C191 File Offset: 0x0003A391
		[ViewVariables]
		[IdDataField(1, null)]
		public string ID { get; }

		// Token: 0x170003B2 RID: 946
		// (get) Token: 0x06001272 RID: 4722 RVA: 0x0003C199 File Offset: 0x0003A399
		[DataField("name", false, 1, false, false, null)]
		public string Name { get; } = string.Empty;

		// Token: 0x170003B3 RID: 947
		// (get) Token: 0x06001273 RID: 4723 RVA: 0x0003C1A1 File Offset: 0x0003A3A1
		// (set) Token: 0x06001274 RID: 4724 RVA: 0x0003C1A9 File Offset: 0x0003A3A9
		[DataField("sound", false, 1, false, true, null)]
		public SoundSpecifier Sound { get; private set; } = new SoundPathSpecifier("/Audio/Effects/Chemistry/bubbles.ogg", null);

		// Token: 0x06001275 RID: 4725 RVA: 0x0003C1B4 File Offset: 0x0003A3B4
		[NullableContext(2)]
		public int CompareTo(ReactionPrototype other)
		{
			if (other == null)
			{
				return -1;
			}
			if (this.Priority != other.Priority)
			{
				return other.Priority - this.Priority;
			}
			if (this.Products.Count != other.Products.Count)
			{
				return this.Products.Count - other.Products.Count;
			}
			return string.Compare(this.ID, other.ID, StringComparison.Ordinal);
		}

		// Token: 0x04001134 RID: 4404
		[DataField("reactants", false, 1, false, false, typeof(PrototypeIdDictionarySerializer<ReactantPrototype, ReagentPrototype>))]
		public Dictionary<string, ReactantPrototype> Reactants = new Dictionary<string, ReactantPrototype>();

		// Token: 0x04001135 RID: 4405
		[DataField("minTemp", false, 1, false, false, null)]
		public float MinimumTemperature;

		// Token: 0x04001136 RID: 4406
		[DataField("maxTemp", false, 1, false, false, null)]
		public float MaximumTemperature = float.PositiveInfinity;

		// Token: 0x04001137 RID: 4407
		[Nullable(new byte[]
		{
			2,
			1
		})]
		[DataField("requiredMixerCategories", false, 1, false, false, null)]
		public List<string> MixingCategories;

		// Token: 0x04001138 RID: 4408
		[DataField("products", false, 1, false, false, typeof(PrototypeIdDictionarySerializer<FixedPoint2, ReagentPrototype>))]
		public Dictionary<string, FixedPoint2> Products = new Dictionary<string, FixedPoint2>();

		// Token: 0x04001139 RID: 4409
		[DataField("effects", false, 1, false, true, null)]
		public List<ReagentEffect> Effects = new List<ReagentEffect>();

		// Token: 0x0400113A RID: 4410
		[DataField("impact", false, 1, false, true, null)]
		public LogImpact Impact = LogImpact.Low;

		// Token: 0x0400113C RID: 4412
		[DataField("quantized", false, 1, false, false, null)]
		public bool Quantized;

		// Token: 0x0400113D RID: 4413
		[DataField("priority", false, 1, false, false, null)]
		public int Priority;
	}
}
