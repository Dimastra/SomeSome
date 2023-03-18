using System;
using System.Runtime.CompilerServices;
using Content.Server.Chemistry.EntitySystems;
using Content.Server.Nutrition.EntitySystems;
using Content.Shared.Chemistry.Components;
using Content.Shared.FixedPoint;
using Robust.Shared.Analyzers;
using Robust.Shared.Audio;
using Robust.Shared.GameObjects;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom.Prototype;
using Robust.Shared.ViewVariables;

namespace Content.Server.Nutrition.Components
{
	// Token: 0x0200031A RID: 794
	[NullableContext(1)]
	[Nullable(0)]
	[RegisterComponent]
	[Access(new Type[]
	{
		typeof(FoodSystem)
	})]
	public sealed class FoodComponent : Component
	{
		// Token: 0x1700025E RID: 606
		// (get) Token: 0x06001069 RID: 4201 RVA: 0x00054D15 File Offset: 0x00052F15
		// (set) Token: 0x0600106A RID: 4202 RVA: 0x00054D1D File Offset: 0x00052F1D
		[DataField("solution", false, 1, false, false, null)]
		public string SolutionName { get; set; } = "food";

		// Token: 0x1700025F RID: 607
		// (get) Token: 0x0600106B RID: 4203 RVA: 0x00054D26 File Offset: 0x00052F26
		// (set) Token: 0x0600106C RID: 4204 RVA: 0x00054D2E File Offset: 0x00052F2E
		[DataField("useSound", false, 1, false, false, null)]
		public SoundSpecifier UseSound { get; set; } = new SoundPathSpecifier("/Audio/Items/eatfood.ogg", null);

		// Token: 0x17000260 RID: 608
		// (get) Token: 0x0600106D RID: 4205 RVA: 0x00054D37 File Offset: 0x00052F37
		// (set) Token: 0x0600106E RID: 4206 RVA: 0x00054D3F File Offset: 0x00052F3F
		[Nullable(2)]
		[DataField("trash", false, 1, false, false, typeof(PrototypeIdSerializer<EntityPrototype>))]
		public string TrashPrototype { [NullableContext(2)] get; [NullableContext(2)] set; }

		// Token: 0x17000261 RID: 609
		// (get) Token: 0x0600106F RID: 4207 RVA: 0x00054D48 File Offset: 0x00052F48
		// (set) Token: 0x06001070 RID: 4208 RVA: 0x00054D50 File Offset: 0x00052F50
		[DataField("transferAmount", false, 1, false, false, null)]
		public FixedPoint2? TransferAmount { get; set; } = new FixedPoint2?(FixedPoint2.New(5));

		// Token: 0x17000262 RID: 610
		// (get) Token: 0x06001071 RID: 4209 RVA: 0x00054D5C File Offset: 0x00052F5C
		[ViewVariables]
		public int UsesRemaining
		{
			get
			{
				Solution solution;
				if (!EntitySystem.Get<SolutionContainerSystem>().TryGetSolution(base.Owner, this.SolutionName, out solution, null))
				{
					return 0;
				}
				if (this.TransferAmount == null)
				{
					if (!(solution.Volume == 0))
					{
						return 1;
					}
					return 0;
				}
				else
				{
					if (!(solution.Volume == 0))
					{
						return Math.Max(1, (int)Math.Ceiling((double)(solution.Volume / this.TransferAmount.Value).Float()));
					}
					return 0;
				}
			}
		}

		// Token: 0x0400098E RID: 2446
		[DataField("utensil", false, 1, false, false, null)]
		public UtensilType Utensil = UtensilType.Fork;

		// Token: 0x0400098F RID: 2447
		[DataField("utensilRequired", false, 1, false, false, null)]
		public bool UtensilRequired;

		// Token: 0x04000990 RID: 2448
		[DataField("eatMessage", false, 1, false, false, null)]
		public string EatMessage = "food-nom";

		// Token: 0x04000991 RID: 2449
		[DataField("forceFeed", false, 1, false, false, null)]
		public bool ForceFeed;

		// Token: 0x04000992 RID: 2450
		[DataField("delay", false, 1, false, false, null)]
		public float Delay = 1f;

		// Token: 0x04000993 RID: 2451
		[DataField("forceFeedDelay", false, 1, false, false, null)]
		public float ForceFeedDelay = 3f;
	}
}
