using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Content.Shared.FixedPoint;
using Robust.Shared.IoC;
using Robust.Shared.Prototypes;

namespace Content.Shared.Kitchen
{
	// Token: 0x02000388 RID: 904
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class RecipeManager
	{
		// Token: 0x17000207 RID: 519
		// (get) Token: 0x06000A7D RID: 2685 RVA: 0x000227EB File Offset: 0x000209EB
		// (set) Token: 0x06000A7E RID: 2686 RVA: 0x000227F3 File Offset: 0x000209F3
		public List<FoodRecipePrototype> Recipes { get; private set; } = new List<FoodRecipePrototype>();

		// Token: 0x06000A7F RID: 2687 RVA: 0x000227FC File Offset: 0x000209FC
		public void Initialize()
		{
			this.Recipes = new List<FoodRecipePrototype>();
			foreach (FoodRecipePrototype item in this._prototypeManager.EnumeratePrototypes<FoodRecipePrototype>())
			{
				this.Recipes.Add(item);
			}
			this.Recipes.Sort(new RecipeManager.RecipeComparer());
		}

		// Token: 0x06000A80 RID: 2688 RVA: 0x00022870 File Offset: 0x00020A70
		public bool SolidAppears(string solidId)
		{
			return this.Recipes.Any((FoodRecipePrototype recipe) => recipe.IngredientsSolids.ContainsKey(solidId));
		}

		// Token: 0x04000A6A RID: 2666
		[Dependency]
		private readonly IPrototypeManager _prototypeManager;

		// Token: 0x020007E4 RID: 2020
		[NullableContext(2)]
		[Nullable(new byte[]
		{
			0,
			1
		})]
		private sealed class RecipeComparer : Comparer<FoodRecipePrototype>
		{
			// Token: 0x06001864 RID: 6244 RVA: 0x0004E020 File Offset: 0x0004C220
			public override int Compare(FoodRecipePrototype x, FoodRecipePrototype y)
			{
				if (x == null || y == null)
				{
					return 0;
				}
				FixedPoint2 nx = x.IngredientCount();
				FixedPoint2 ny = y.IngredientCount();
				return -nx.CompareTo(ny);
			}
		}
	}
}
