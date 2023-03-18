using System;

namespace Content.Shared.Humanoid.Markings
{
	// Token: 0x0200041C RID: 1052
	public static class MarkingCategoriesConversion
	{
		// Token: 0x06000C80 RID: 3200 RVA: 0x00028FBC File Offset: 0x000271BC
		public static MarkingCategories FromHumanoidVisualLayers(HumanoidVisualLayers layer)
		{
			switch (layer)
			{
			case HumanoidVisualLayers.Tail:
				return MarkingCategories.Tail;
			case HumanoidVisualLayers.Hair:
				return MarkingCategories.Hair;
			case HumanoidVisualLayers.FacialHair:
				return MarkingCategories.FacialHair;
			case HumanoidVisualLayers.Chest:
				return MarkingCategories.Chest;
			case HumanoidVisualLayers.Head:
				return MarkingCategories.Head;
			case HumanoidVisualLayers.Snout:
				return MarkingCategories.Snout;
			case HumanoidVisualLayers.HeadSide:
				return MarkingCategories.HeadSide;
			case HumanoidVisualLayers.HeadTop:
				return MarkingCategories.HeadTop;
			case HumanoidVisualLayers.RArm:
				return MarkingCategories.Arms;
			case HumanoidVisualLayers.LArm:
				return MarkingCategories.Arms;
			case HumanoidVisualLayers.RHand:
				return MarkingCategories.Arms;
			case HumanoidVisualLayers.LHand:
				return MarkingCategories.Arms;
			case HumanoidVisualLayers.RLeg:
				return MarkingCategories.Legs;
			case HumanoidVisualLayers.LLeg:
				return MarkingCategories.Legs;
			case HumanoidVisualLayers.RFoot:
				return MarkingCategories.Legs;
			case HumanoidVisualLayers.LFoot:
				return MarkingCategories.Legs;
			}
			return MarkingCategories.Overlay;
		}
	}
}
