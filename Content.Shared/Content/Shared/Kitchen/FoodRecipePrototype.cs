using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Content.Shared.Chemistry.Reagent;
using Content.Shared.FixedPoint;
using Robust.Shared.Localization;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom.Prototype;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom.Prototype.Dictionary;
using Robust.Shared.ViewVariables;

namespace Content.Shared.Kitchen
{
	// Token: 0x02000387 RID: 903
	[NullableContext(1)]
	[Nullable(0)]
	[Prototype("microwaveMealRecipe", 1)]
	public sealed class FoodRecipePrototype : IPrototype
	{
		// Token: 0x17000201 RID: 513
		// (get) Token: 0x06000A75 RID: 2677 RVA: 0x00022702 File Offset: 0x00020902
		[ViewVariables]
		[IdDataField(1, null)]
		public string ID { get; }

		// Token: 0x17000202 RID: 514
		// (get) Token: 0x06000A76 RID: 2678 RVA: 0x0002270A File Offset: 0x0002090A
		[DataField("result", false, 1, false, false, typeof(PrototypeIdSerializer<EntityPrototype>))]
		public string Result { get; } = string.Empty;

		// Token: 0x17000203 RID: 515
		// (get) Token: 0x06000A77 RID: 2679 RVA: 0x00022712 File Offset: 0x00020912
		[DataField("time", false, 1, false, false, null)]
		public uint CookTime { get; } = 5;

		// Token: 0x17000204 RID: 516
		// (get) Token: 0x06000A78 RID: 2680 RVA: 0x0002271A File Offset: 0x0002091A
		public string Name
		{
			get
			{
				return Loc.GetString(this._name);
			}
		}

		// Token: 0x17000205 RID: 517
		// (get) Token: 0x06000A79 RID: 2681 RVA: 0x00022727 File Offset: 0x00020927
		public IReadOnlyDictionary<string, FixedPoint2> IngredientsReagents
		{
			get
			{
				return this._ingsReagents;
			}
		}

		// Token: 0x17000206 RID: 518
		// (get) Token: 0x06000A7A RID: 2682 RVA: 0x0002272F File Offset: 0x0002092F
		public IReadOnlyDictionary<string, FixedPoint2> IngredientsSolids
		{
			get
			{
				return this._ingsSolids;
			}
		}

		// Token: 0x06000A7B RID: 2683 RVA: 0x00022738 File Offset: 0x00020938
		public FixedPoint2 IngredientCount()
		{
			FixedPoint2 i = 0;
			i += this._ingsReagents.Count;
			foreach (FixedPoint2 j in this._ingsSolids.Values)
			{
				i += j;
			}
			return i;
		}

		// Token: 0x04000A65 RID: 2661
		[DataField("name", false, 1, false, false, null)]
		private string _name = string.Empty;

		// Token: 0x04000A66 RID: 2662
		[DataField("reagents", false, 1, false, false, typeof(PrototypeIdDictionarySerializer<FixedPoint2, ReagentPrototype>))]
		private readonly Dictionary<string, FixedPoint2> _ingsReagents = new Dictionary<string, FixedPoint2>();

		// Token: 0x04000A67 RID: 2663
		[DataField("solids", false, 1, false, false, typeof(PrototypeIdDictionarySerializer<FixedPoint2, EntityPrototype>))]
		private readonly Dictionary<string, FixedPoint2> _ingsSolids = new Dictionary<string, FixedPoint2>();
	}
}
