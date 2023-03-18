using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Content.Shared.Actions.ActionTypes;
using Content.Shared.FixedPoint;
using Robust.Shared.Analyzers;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom.Prototype;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom.Prototype.Dictionary;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom.Prototype.List;
using Robust.Shared.Utility;

namespace Content.Shared.Store
{
	// Token: 0x02000120 RID: 288
	[NullableContext(1)]
	[Nullable(0)]
	[NetSerializable]
	[Virtual]
	[DataDefinition]
	[Serializable]
	public class ListingData : IEquatable<ListingData>, ICloneable
	{
		// Token: 0x06000356 RID: 854 RVA: 0x0000E41C File Offset: 0x0000C61C
		[NullableContext(2)]
		public bool Equals(ListingData listing)
		{
			if (listing == null)
			{
				return false;
			}
			if (this.Priority != listing.Priority || this.Name != listing.Name || this.Description != listing.Description || this.ProductEntity != listing.ProductEntity || this.ProductAction != listing.ProductAction)
			{
				return false;
			}
			if (this.Icon != null && !this.Icon.Equals(listing.Icon))
			{
				return false;
			}
			if (!(from x in this.Categories
			orderby x
			select x).SequenceEqual(from x in listing.Categories
			orderby x
			select x))
			{
				return false;
			}
			if (!(from x in this.Cost
			orderby x
			select x).SequenceEqual(from x in listing.Cost
			orderby x
			select x))
			{
				return false;
			}
			if (this.Conditions != null && listing.Conditions != null)
			{
				if (!(from x in this.Conditions
				orderby x
				select x).SequenceEqual(from x in listing.Conditions
				orderby x
				select x))
				{
					return false;
				}
			}
			return true;
		}

		// Token: 0x06000357 RID: 855 RVA: 0x0000E5D0 File Offset: 0x0000C7D0
		public object Clone()
		{
			return new ListingData
			{
				Name = this.Name,
				Description = this.Description,
				Categories = this.Categories,
				Cost = this.Cost,
				Conditions = this.Conditions,
				Icon = this.Icon,
				Priority = this.Priority,
				ProductEntity = this.ProductEntity,
				ProductAction = this.ProductAction,
				ProductEvent = this.ProductEvent,
				PurchaseAmount = this.PurchaseAmount
			};
		}

		// Token: 0x0400036E RID: 878
		[DataField("name", false, 1, false, false, null)]
		public string Name = string.Empty;

		// Token: 0x0400036F RID: 879
		[DataField("description", false, 1, false, false, null)]
		public string Description = string.Empty;

		// Token: 0x04000370 RID: 880
		[DataField("categories", false, 1, true, false, typeof(PrototypeIdListSerializer<StoreCategoryPrototype>))]
		public List<string> Categories = new List<string>();

		// Token: 0x04000371 RID: 881
		[DataField("cost", false, 1, false, false, typeof(PrototypeIdDictionarySerializer<FixedPoint2, CurrencyPrototype>))]
		public Dictionary<string, FixedPoint2> Cost = new Dictionary<string, FixedPoint2>();

		// Token: 0x04000372 RID: 882
		[Nullable(new byte[]
		{
			2,
			1
		})]
		[DataField("conditions", false, 1, false, true, null)]
		[NonSerialized]
		public List<ListingCondition> Conditions;

		// Token: 0x04000373 RID: 883
		[Nullable(2)]
		[DataField("icon", false, 1, false, false, null)]
		public SpriteSpecifier Icon;

		// Token: 0x04000374 RID: 884
		[DataField("priority", false, 1, false, false, null)]
		public int Priority;

		// Token: 0x04000375 RID: 885
		[Nullable(2)]
		[DataField("productEntity", false, 1, false, false, typeof(PrototypeIdSerializer<EntityPrototype>))]
		public string ProductEntity;

		// Token: 0x04000376 RID: 886
		[Nullable(2)]
		[DataField("productAction", false, 1, false, false, typeof(PrototypeIdSerializer<InstantActionPrototype>))]
		public string ProductAction;

		// Token: 0x04000377 RID: 887
		[Nullable(2)]
		[DataField("productEvent", false, 1, false, false, null)]
		public object ProductEvent;

		// Token: 0x04000378 RID: 888
		public int PurchaseAmount;
	}
}
