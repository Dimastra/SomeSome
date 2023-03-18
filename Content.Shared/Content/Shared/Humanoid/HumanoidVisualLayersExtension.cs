using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Content.Shared.Body.Part;

namespace Content.Shared.Humanoid
{
	// Token: 0x02000408 RID: 1032
	[NullableContext(1)]
	[Nullable(0)]
	public static class HumanoidVisualLayersExtension
	{
		// Token: 0x06000C1A RID: 3098 RVA: 0x00027FA4 File Offset: 0x000261A4
		public static bool HasSexMorph(HumanoidVisualLayers layer)
		{
			return layer == HumanoidVisualLayers.Chest || layer == HumanoidVisualLayers.Head;
		}

		// Token: 0x06000C1B RID: 3099 RVA: 0x00027FC8 File Offset: 0x000261C8
		public static string GetSexMorph(HumanoidVisualLayers layer, Sex sex, string id)
		{
			if (!HumanoidVisualLayersExtension.HasSexMorph(layer) || sex == Sex.Unsexed)
			{
				return id;
			}
			DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(0, 2);
			defaultInterpolatedStringHandler.AppendFormatted(id);
			defaultInterpolatedStringHandler.AppendFormatted<Sex>(sex);
			return defaultInterpolatedStringHandler.ToStringAndClear();
		}

		// Token: 0x06000C1C RID: 3100 RVA: 0x00028003 File Offset: 0x00026203
		public static IEnumerable<HumanoidVisualLayers> Sublayers(HumanoidVisualLayers layer)
		{
			if (layer != HumanoidVisualLayers.Chest)
			{
				if (layer != HumanoidVisualLayers.Head)
				{
					switch (layer)
					{
					case HumanoidVisualLayers.RArm:
						yield return HumanoidVisualLayers.RArm;
						yield return HumanoidVisualLayers.RHand;
						goto IL_245;
					case HumanoidVisualLayers.LArm:
						yield return HumanoidVisualLayers.LArm;
						yield return HumanoidVisualLayers.LHand;
						goto IL_245;
					case HumanoidVisualLayers.RLeg:
						yield return HumanoidVisualLayers.RLeg;
						yield return HumanoidVisualLayers.RFoot;
						goto IL_245;
					case HumanoidVisualLayers.LLeg:
						yield return HumanoidVisualLayers.LLeg;
						yield return HumanoidVisualLayers.LFoot;
						goto IL_245;
					}
					yield break;
				}
				yield return HumanoidVisualLayers.Head;
				yield return HumanoidVisualLayers.Eyes;
				yield return HumanoidVisualLayers.HeadSide;
				yield return HumanoidVisualLayers.HeadTop;
				yield return HumanoidVisualLayers.Hair;
				yield return HumanoidVisualLayers.FacialHair;
				yield return HumanoidVisualLayers.Snout;
			}
			else
			{
				yield return HumanoidVisualLayers.Chest;
				yield return HumanoidVisualLayers.Tail;
			}
			IL_245:
			yield break;
		}

		// Token: 0x06000C1D RID: 3101 RVA: 0x00028014 File Offset: 0x00026214
		public static HumanoidVisualLayers? ToHumanoidLayers(this BodyPartComponent part)
		{
			switch (part.PartType)
			{
			case BodyPartType.Torso:
				return new HumanoidVisualLayers?(HumanoidVisualLayers.Chest);
			case BodyPartType.Head:
				return new HumanoidVisualLayers?(HumanoidVisualLayers.Head);
			case BodyPartType.Arm:
				switch (part.Symmetry)
				{
				case BodyPartSymmetry.Left:
					return new HumanoidVisualLayers?(HumanoidVisualLayers.LArm);
				case BodyPartSymmetry.Right:
					return new HumanoidVisualLayers?(HumanoidVisualLayers.RArm);
				}
				break;
			case BodyPartType.Hand:
				switch (part.Symmetry)
				{
				case BodyPartSymmetry.Left:
					return new HumanoidVisualLayers?(HumanoidVisualLayers.LHand);
				case BodyPartSymmetry.Right:
					return new HumanoidVisualLayers?(HumanoidVisualLayers.RHand);
				}
				break;
			case BodyPartType.Leg:
				switch (part.Symmetry)
				{
				case BodyPartSymmetry.Left:
					return new HumanoidVisualLayers?(HumanoidVisualLayers.LLeg);
				case BodyPartSymmetry.Right:
					return new HumanoidVisualLayers?(HumanoidVisualLayers.RLeg);
				}
				break;
			case BodyPartType.Foot:
				switch (part.Symmetry)
				{
				case BodyPartSymmetry.Left:
					return new HumanoidVisualLayers?(HumanoidVisualLayers.LFoot);
				case BodyPartSymmetry.Right:
					return new HumanoidVisualLayers?(HumanoidVisualLayers.RFoot);
				}
				break;
			case BodyPartType.Tail:
				return new HumanoidVisualLayers?(HumanoidVisualLayers.Tail);
			}
			return null;
		}
	}
}
