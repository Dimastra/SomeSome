using System;
using System.Runtime.CompilerServices;
using Robust.Client.Graphics;
using Robust.Client.UserInterface.Controls;

namespace Content.Client.Construction.UI
{
	// Token: 0x02000391 RID: 913
	[NullableContext(1)]
	public interface IConstructionMenuView : IDisposable
	{
		// Token: 0x1700047A RID: 1146
		// (get) Token: 0x06001676 RID: 5750
		// (set) Token: 0x06001677 RID: 5751
		string[] Categories { get; set; }

		// Token: 0x1700047B RID: 1147
		// (get) Token: 0x06001678 RID: 5752
		OptionButton Category { get; }

		// Token: 0x1700047C RID: 1148
		// (get) Token: 0x06001679 RID: 5753
		// (set) Token: 0x0600167A RID: 5754
		bool EraseButtonPressed { get; set; }

		// Token: 0x1700047D RID: 1149
		// (get) Token: 0x0600167B RID: 5755
		// (set) Token: 0x0600167C RID: 5756
		bool BuildButtonPressed { get; set; }

		// Token: 0x1700047E RID: 1150
		// (get) Token: 0x0600167D RID: 5757
		ItemList Recipes { get; }

		// Token: 0x1700047F RID: 1151
		// (get) Token: 0x0600167E RID: 5758
		ItemList RecipeStepList { get; }

		// Token: 0x14000080 RID: 128
		// (add) Token: 0x0600167F RID: 5759
		// (remove) Token: 0x06001680 RID: 5760
		[TupleElementNames(new string[]
		{
			"search",
			"catagory"
		})]
		[Nullable(new byte[]
		{
			1,
			0,
			1,
			1
		})]
		event EventHandler<ValueTuple<string, string>> PopulateRecipes;

		// Token: 0x14000081 RID: 129
		// (add) Token: 0x06001681 RID: 5761
		// (remove) Token: 0x06001682 RID: 5762
		[Nullable(new byte[]
		{
			1,
			2
		})]
		event EventHandler<ItemList.Item> RecipeSelected;

		// Token: 0x14000082 RID: 130
		// (add) Token: 0x06001683 RID: 5763
		// (remove) Token: 0x06001684 RID: 5764
		event EventHandler<bool> BuildButtonToggled;

		// Token: 0x14000083 RID: 131
		// (add) Token: 0x06001685 RID: 5765
		// (remove) Token: 0x06001686 RID: 5766
		event EventHandler<bool> EraseButtonToggled;

		// Token: 0x14000084 RID: 132
		// (add) Token: 0x06001687 RID: 5767
		// (remove) Token: 0x06001688 RID: 5768
		event EventHandler ClearAllGhosts;

		// Token: 0x06001689 RID: 5769
		void ClearRecipeInfo();

		// Token: 0x0600168A RID: 5770
		void SetRecipeInfo(string name, string description, Texture iconTexture, bool isItem);

		// Token: 0x0600168B RID: 5771
		void ResetPlacement();

		// Token: 0x14000085 RID: 133
		// (add) Token: 0x0600168C RID: 5772
		// (remove) Token: 0x0600168D RID: 5773
		[Nullable(2)]
		event Action OnClose;

		// Token: 0x17000480 RID: 1152
		// (get) Token: 0x0600168E RID: 5774
		bool IsOpen { get; }

		// Token: 0x0600168F RID: 5775
		void OpenCentered();

		// Token: 0x06001690 RID: 5776
		void MoveToFront();

		// Token: 0x06001691 RID: 5777
		bool IsAtFront();

		// Token: 0x06001692 RID: 5778
		void Close();
	}
}
