using System;
using System.Runtime.CompilerServices;
using Content.Server.Chemistry.EntitySystems;
using Content.Shared.Body.Prototypes;
using Content.Shared.Chemistry.Components;
using Content.Shared.Chemistry.Reagent;
using Content.Shared.FixedPoint;
using Robust.Shared.IoC;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom.Prototype;

namespace Content.Server.Chemistry.ReagentEffects
{
	// Token: 0x02000657 RID: 1623
	[NullableContext(2)]
	[Nullable(0)]
	public sealed class AdjustReagent : ReagentEffect
	{
		// Token: 0x0600224C RID: 8780 RVA: 0x000B382C File Offset: 0x000B1A2C
		public override void Effect(ReagentEffectArgs args)
		{
			if (args.Source != null)
			{
				SolutionContainerSystem solutionSys = args.EntityManager.EntitySysManager.GetEntitySystem<SolutionContainerSystem>();
				FixedPoint2 amount = this.Amount;
				amount *= args.Scale;
				if (this.Reagent != null)
				{
					if (amount < 0 && args.Source.ContainsReagent(this.Reagent))
					{
						solutionSys.TryRemoveReagent(args.SolutionEntity, args.Source, this.Reagent, -amount);
					}
					if (amount > 0)
					{
						FixedPoint2 fixedPoint;
						solutionSys.TryAddReagent(args.SolutionEntity, args.Source, this.Reagent, amount, out fixedPoint, null);
						return;
					}
				}
				else if (this.Group != null)
				{
					IPrototypeManager prototypeMan = IoCManager.Resolve<IPrototypeManager>();
					foreach (Solution.ReagentQuantity quant in args.Source.Contents.ToArray())
					{
						ReagentPrototype proto = prototypeMan.Index<ReagentPrototype>(quant.ReagentId);
						if (proto.Metabolisms != null && proto.Metabolisms.ContainsKey(this.Group))
						{
							if (amount < 0)
							{
								solutionSys.TryRemoveReagent(args.SolutionEntity, args.Source, quant.ReagentId, amount);
							}
							if (amount > 0)
							{
								FixedPoint2 fixedPoint;
								solutionSys.TryAddReagent(args.SolutionEntity, args.Source, quant.ReagentId, amount, out fixedPoint, null);
							}
						}
					}
				}
			}
		}

		// Token: 0x04001535 RID: 5429
		[DataField("reagent", false, 1, false, false, typeof(PrototypeIdSerializer<ReagentPrototype>))]
		public string Reagent;

		// Token: 0x04001536 RID: 5430
		[DataField("group", false, 1, false, false, typeof(PrototypeIdSerializer<MetabolismGroupPrototype>))]
		public string Group;

		// Token: 0x04001537 RID: 5431
		[DataField("amount", false, 1, true, false, null)]
		public FixedPoint2 Amount;
	}
}
