using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Content.Server.Administration;
using Content.Server.Body.Systems;
using Content.Server.Cargo.Components;
using Content.Server.Chemistry.Components.SolutionManager;
using Content.Shared.Administration;
using Content.Shared.Body.Components;
using Content.Shared.Body.Part;
using Content.Shared.Chemistry.Components;
using Content.Shared.Chemistry.Reagent;
using Content.Shared.Materials;
using Content.Shared.Mobs.Components;
using Content.Shared.Mobs.Systems;
using Content.Shared.Stacks;
using Robust.Shared.Console;
using Robust.Shared.Containers;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Log;
using Robust.Shared.Map;
using Robust.Shared.Map.Components;
using Robust.Shared.Prototypes;
using Robust.Shared.Utility;

namespace Content.Server.Cargo.Systems
{
	// Token: 0x020006E2 RID: 1762
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class PricingSystem : EntitySystem
	{
		// Token: 0x06002502 RID: 9474 RVA: 0x000C17E8 File Offset: 0x000BF9E8
		public override void Initialize()
		{
			base.SubscribeLocalEvent<StaticPriceComponent, PriceCalculationEvent>(new ComponentEventRefHandler<StaticPriceComponent, PriceCalculationEvent>(this.CalculateStaticPrice), null, null);
			base.SubscribeLocalEvent<StackPriceComponent, PriceCalculationEvent>(new ComponentEventRefHandler<StackPriceComponent, PriceCalculationEvent>(this.CalculateStackPrice), null, null);
			base.SubscribeLocalEvent<MobPriceComponent, PriceCalculationEvent>(new ComponentEventRefHandler<MobPriceComponent, PriceCalculationEvent>(this.CalculateMobPrice), null, null);
			base.SubscribeLocalEvent<SolutionContainerManagerComponent, PriceCalculationEvent>(new ComponentEventRefHandler<SolutionContainerManagerComponent, PriceCalculationEvent>(this.CalculateSolutionPrice), null, null);
			this._consoleHost.RegisterCommand("appraisegrid", "Calculates the total value of the given grids.", "appraisegrid <grid Ids>", new ConCommandCallback(this.AppraiseGridCommand), false);
		}

		// Token: 0x06002503 RID: 9475 RVA: 0x000C186C File Offset: 0x000BFA6C
		[AdminCommand(AdminFlags.Debug)]
		private void AppraiseGridCommand(IConsoleShell shell, string argstr, string[] args)
		{
			if (args.Length == 0)
			{
				shell.WriteError("Not enough arguments.");
				return;
			}
			for (int i = 0; i < args.Length; i++)
			{
				string gid = args[i];
				EntityUid gridId;
				MapGridComponent mapGrid;
				if (!EntityUid.TryParse(gid, ref gridId) || !gridId.IsValid())
				{
					shell.WriteError("Invalid grid ID \"" + gid + "\".");
				}
				else if (!this._mapManager.TryGetGrid(new EntityUid?(gridId), ref mapGrid))
				{
					DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(22, 1);
					defaultInterpolatedStringHandler.AppendLiteral("Grid \"");
					defaultInterpolatedStringHandler.AppendFormatted<EntityUid>(gridId);
					defaultInterpolatedStringHandler.AppendLiteral("\" doesn't exist.");
					shell.WriteError(defaultInterpolatedStringHandler.ToStringAndClear());
				}
				else
				{
					List<ValueTuple<double, EntityUid>> mostValuable = new List<ValueTuple<double, EntityUid>>();
					double value = this.AppraiseGrid(mapGrid.Owner, null, delegate(EntityUid uid, double price)
					{
						mostValuable.Add(new ValueTuple<double, EntityUid>(price, uid));
						mostValuable.Sort((ValueTuple<double, EntityUid> i1, ValueTuple<double, EntityUid> i2) => i2.Item1.CompareTo(i1.Item1));
						if (mostValuable.Count > 5)
						{
							Extensions.Pop<ValueTuple<double, EntityUid>>(mostValuable);
						}
					});
					DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(31, 2);
					defaultInterpolatedStringHandler.AppendLiteral("Grid ");
					defaultInterpolatedStringHandler.AppendFormatted(gid);
					defaultInterpolatedStringHandler.AppendLiteral(" appraised to ");
					defaultInterpolatedStringHandler.AppendFormatted<double>(value);
					defaultInterpolatedStringHandler.AppendLiteral(" spacebucks.");
					shell.WriteLine(defaultInterpolatedStringHandler.ToStringAndClear());
					shell.WriteLine("The top most valuable items were:");
					foreach (ValueTuple<double, EntityUid> valueTuple in mostValuable)
					{
						double price2 = valueTuple.Item1;
						EntityUid ent = valueTuple.Item2;
						defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(16, 2);
						defaultInterpolatedStringHandler.AppendLiteral("- ");
						defaultInterpolatedStringHandler.AppendFormatted<EntityStringRepresentation>(base.ToPrettyString(ent));
						defaultInterpolatedStringHandler.AppendLiteral(" @ ");
						defaultInterpolatedStringHandler.AppendFormatted<double>(price2);
						defaultInterpolatedStringHandler.AppendLiteral(" spacebucks");
						shell.WriteLine(defaultInterpolatedStringHandler.ToStringAndClear());
					}
				}
			}
		}

		// Token: 0x06002504 RID: 9476 RVA: 0x000C1A50 File Offset: 0x000BFC50
		private void CalculateMobPrice(EntityUid uid, MobPriceComponent component, ref PriceCalculationEvent args)
		{
			BodyComponent body;
			MobStateComponent state;
			if (!base.TryComp<BodyComponent>(uid, ref body) || !base.TryComp<MobStateComponent>(uid, ref state))
			{
				string text = "pricing";
				DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(54, 3);
				defaultInterpolatedStringHandler.AppendLiteral("Tried to get the mob price of ");
				defaultInterpolatedStringHandler.AppendFormatted<EntityStringRepresentation>(base.ToPrettyString(uid));
				defaultInterpolatedStringHandler.AppendLiteral(", which has no ");
				defaultInterpolatedStringHandler.AppendFormatted("BodyComponent");
				defaultInterpolatedStringHandler.AppendLiteral(" and no ");
				defaultInterpolatedStringHandler.AppendFormatted("MobStateComponent");
				defaultInterpolatedStringHandler.AppendLiteral(".");
				Logger.ErrorS(text, defaultInterpolatedStringHandler.ToStringAndClear());
				return;
			}
			List<BodyPartSlot> partList = this._bodySystem.GetBodyAllSlots(new EntityUid?(uid), body).ToList<BodyPartSlot>();
			double num = (double)partList.Sum(delegate(BodyPartSlot x)
			{
				if (x.Child == null)
				{
					return 0;
				}
				return 1;
			});
			int totalParts = partList.Count;
			double partRatio = num / (double)totalParts;
			double partPenalty = component.Price * (1.0 - partRatio) * component.MissingBodyPartPenalty;
			args.Price += (component.Price - partPenalty) * (this._mobStateSystem.IsAlive(uid, state) ? 1.0 : component.DeathPenalty);
		}

		// Token: 0x06002505 RID: 9477 RVA: 0x000C1B80 File Offset: 0x000BFD80
		private void CalculateStackPrice(EntityUid uid, StackPriceComponent component, ref PriceCalculationEvent args)
		{
			StackComponent stack;
			if (!base.TryComp<StackComponent>(uid, ref stack))
			{
				string text = "pricing";
				DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(48, 2);
				defaultInterpolatedStringHandler.AppendLiteral("Tried to get the stack price of ");
				defaultInterpolatedStringHandler.AppendFormatted<EntityStringRepresentation>(base.ToPrettyString(uid));
				defaultInterpolatedStringHandler.AppendLiteral(", which has no ");
				defaultInterpolatedStringHandler.AppendFormatted("StackComponent");
				defaultInterpolatedStringHandler.AppendLiteral(".");
				Logger.ErrorS(text, defaultInterpolatedStringHandler.ToStringAndClear());
				return;
			}
			args.Price += (double)stack.Count * component.Price;
		}

		// Token: 0x06002506 RID: 9478 RVA: 0x000C1C0C File Offset: 0x000BFE0C
		private void CalculateSolutionPrice(EntityUid uid, SolutionContainerManagerComponent component, ref PriceCalculationEvent args)
		{
			float price = 0f;
			foreach (Solution solution in component.Solutions.Values)
			{
				foreach (Solution.ReagentQuantity reagent in solution.Contents)
				{
					ReagentPrototype reagentProto;
					if (this._prototypeManager.TryIndex<ReagentPrototype>(reagent.ReagentId, ref reagentProto))
					{
						price += (float)reagent.Quantity * reagentProto.PricePerUnit;
					}
				}
			}
			args.Price += (double)price;
		}

		// Token: 0x06002507 RID: 9479 RVA: 0x000C1CD4 File Offset: 0x000BFED4
		private void CalculateStaticPrice(EntityUid uid, StaticPriceComponent component, ref PriceCalculationEvent args)
		{
			args.Price += component.Price;
		}

		// Token: 0x06002508 RID: 9480 RVA: 0x000C1CE8 File Offset: 0x000BFEE8
		public double GetEstimatedPrice(EntityPrototype prototype, [Nullable(2)] IComponentFactory factory = null)
		{
			IoCManager.Resolve<IComponentFactory>(ref factory);
			double price = 0.0;
			EntityPrototype.ComponentRegistryEntry staticPriceProto;
			if (prototype.Components.TryGetValue(factory.GetComponentName(typeof(StaticPriceComponent)), out staticPriceProto))
			{
				StaticPriceComponent staticComp = (StaticPriceComponent)staticPriceProto.Component;
				price += staticComp.Price;
			}
			EntityPrototype.ComponentRegistryEntry stackpriceProto;
			EntityPrototype.ComponentRegistryEntry stackProto;
			if (prototype.Components.TryGetValue(factory.GetComponentName(typeof(StackPriceComponent)), out stackpriceProto) && prototype.Components.TryGetValue(factory.GetComponentName(typeof(StackComponent)), out stackProto))
			{
				StackPriceComponent stackPrice = (StackPriceComponent)stackpriceProto.Component;
				StackComponent stack = (StackComponent)stackProto.Component;
				price += (double)stack.Count * stackPrice.Price;
			}
			return price;
		}

		// Token: 0x06002509 RID: 9481 RVA: 0x000C1DA8 File Offset: 0x000BFFA8
		public double GetMaterialPrice(MaterialComponent component)
		{
			double price = 0.0;
			foreach (KeyValuePair<string, int> keyValuePair in component.Materials)
			{
				string text;
				int num;
				keyValuePair.Deconstruct(out text, out num);
				string id = text;
				int quantity = num;
				price += this._prototypeManager.Index<MaterialPrototype>(id).Price * (double)quantity;
			}
			return price;
		}

		// Token: 0x0600250A RID: 9482 RVA: 0x000C1E28 File Offset: 0x000C0028
		public double GetPrice(EntityUid uid)
		{
			PriceCalculationEvent ev = new PriceCalculationEvent();
			base.RaiseLocalEvent<PriceCalculationEvent>(uid, ref ev, false);
			MaterialComponent material;
			if (base.TryComp<MaterialComponent>(uid, ref material) && !base.HasComp<StackPriceComponent>(uid))
			{
				double matPrice = this.GetMaterialPrice(material);
				StackComponent stack;
				if (base.TryComp<StackComponent>(uid, ref stack))
				{
					matPrice *= (double)stack.Count;
				}
				ev.Price += matPrice;
			}
			ContainerManagerComponent containers;
			if (base.TryComp<ContainerManagerComponent>(uid, ref containers))
			{
				foreach (KeyValuePair<string, IContainer> container in containers.Containers)
				{
					foreach (EntityUid ent in container.Value.ContainedEntities)
					{
						ev.Price += this.GetPrice(ent);
					}
				}
			}
			return ev.Price;
		}

		// Token: 0x0600250B RID: 9483 RVA: 0x000C1F2C File Offset: 0x000C012C
		[NullableContext(2)]
		public double AppraiseGrid(EntityUid grid, Func<EntityUid, bool> predicate = null, Action<EntityUid, double> afterPredicate = null)
		{
			TransformComponent transformComponent = base.Transform(grid);
			double price = 0.0;
			foreach (EntityUid child in transformComponent.ChildEntities)
			{
				if (predicate == null || predicate(child))
				{
					double subPrice = this.GetPrice(child);
					price += subPrice;
					if (afterPredicate != null)
					{
						afterPredicate(child, subPrice);
					}
				}
			}
			return price;
		}

		// Token: 0x040016B1 RID: 5809
		[Dependency]
		private readonly IConsoleHost _consoleHost;

		// Token: 0x040016B2 RID: 5810
		[Dependency]
		private readonly IMapManager _mapManager;

		// Token: 0x040016B3 RID: 5811
		[Dependency]
		private readonly MobStateSystem _mobStateSystem;

		// Token: 0x040016B4 RID: 5812
		[Dependency]
		private readonly IPrototypeManager _prototypeManager;

		// Token: 0x040016B5 RID: 5813
		[Dependency]
		private readonly BodySystem _bodySystem;
	}
}
