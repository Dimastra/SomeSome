using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Content.Server.Nutrition.Components;
using Content.Shared.CCVar;
using Content.Shared.Chemistry.Components;
using Content.Shared.Chemistry.Reagent;
using Robust.Shared.Configuration;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Localization;
using Robust.Shared.Prototypes;

namespace Content.Server.Nutrition.EntitySystems
{
	// Token: 0x0200030D RID: 781
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class FlavorProfileSystem : EntitySystem
	{
		// Token: 0x17000257 RID: 599
		// (get) Token: 0x06001018 RID: 4120 RVA: 0x000526CB File Offset: 0x000508CB
		private int FlavorLimit
		{
			get
			{
				return this._configManager.GetCVar<int>(CCVars.FlavorLimit);
			}
		}

		// Token: 0x06001019 RID: 4121 RVA: 0x000526E0 File Offset: 0x000508E0
		public string GetLocalizedFlavorsMessage(EntityUid uid, EntityUid user, Solution solution, [Nullable(2)] FlavorProfileComponent flavorProfile = null)
		{
			HashSet<string> flavors = new HashSet<string>();
			if (!base.Resolve<FlavorProfileComponent>(uid, ref flavorProfile, false))
			{
				return Loc.GetString("flavor-profile-unknown");
			}
			flavors.UnionWith(flavorProfile.Flavors);
			flavors.UnionWith(this.GetFlavorsFromReagents(solution, this.FlavorLimit - flavors.Count, flavorProfile.IgnoreReagents));
			FlavorProfileModificationEvent ev = new FlavorProfileModificationEvent(user, flavors);
			base.RaiseLocalEvent<FlavorProfileModificationEvent>(ev);
			base.RaiseLocalEvent<FlavorProfileModificationEvent>(uid, ev, false);
			base.RaiseLocalEvent<FlavorProfileModificationEvent>(user, ev, false);
			return this.FlavorsToFlavorMessage(flavors);
		}

		// Token: 0x0600101A RID: 4122 RVA: 0x00052760 File Offset: 0x00050960
		public string GetLocalizedFlavorsMessage(EntityUid user, Solution solution)
		{
			HashSet<string> flavors = this.GetFlavorsFromReagents(solution, this.FlavorLimit, null);
			FlavorProfileModificationEvent ev = new FlavorProfileModificationEvent(user, flavors);
			base.RaiseLocalEvent<FlavorProfileModificationEvent>(user, ev, true);
			return this.FlavorsToFlavorMessage(flavors);
		}

		// Token: 0x0600101B RID: 4123 RVA: 0x00052794 File Offset: 0x00050994
		private string FlavorsToFlavorMessage(HashSet<string> flavorSet)
		{
			List<FlavorPrototype> flavors = new List<FlavorPrototype>();
			foreach (string flavor in flavorSet)
			{
				FlavorPrototype flavorPrototype;
				if (!string.IsNullOrEmpty(flavor) && this._prototypeManager.TryIndex<FlavorPrototype>(flavor, ref flavorPrototype))
				{
					flavors.Add(flavorPrototype);
				}
			}
			flavors.Sort((FlavorPrototype a, FlavorPrototype b) => a.FlavorType.CompareTo(b.FlavorType));
			if (flavors.Count == 1 && !string.IsNullOrEmpty(flavors[0].FlavorDescription))
			{
				return Loc.GetString("flavor-profile", new ValueTuple<string, object>[]
				{
					new ValueTuple<string, object>("flavor", Loc.GetString(flavors[0].FlavorDescription))
				});
			}
			if (flavors.Count > 1)
			{
				List<FlavorPrototype> list = flavors;
				string lastFlavor = Loc.GetString(list[list.Count - 1].FlavorDescription);
				string allFlavors = string.Join(", ", from i in flavors.GetRange(0, flavors.Count - 1)
				select Loc.GetString(i.FlavorDescription));
				return Loc.GetString("flavor-profile-multiple", new ValueTuple<string, object>[]
				{
					new ValueTuple<string, object>("flavors", allFlavors),
					new ValueTuple<string, object>("lastFlavor", lastFlavor)
				});
			}
			return Loc.GetString("flavor-profile-unknown");
		}

		// Token: 0x0600101C RID: 4124 RVA: 0x0005291C File Offset: 0x00050B1C
		private HashSet<string> GetFlavorsFromReagents(Solution solution, int desiredAmount, [Nullable(new byte[]
		{
			2,
			1
		})] HashSet<string> toIgnore = null)
		{
			HashSet<string> flavors = new HashSet<string>();
			foreach (Solution.ReagentQuantity reagent in solution.Contents)
			{
				if (toIgnore == null || !toIgnore.Contains(reagent.ReagentId))
				{
					if (flavors.Count == desiredAmount)
					{
						break;
					}
					string flavor = this._prototypeManager.Index<ReagentPrototype>(reagent.ReagentId).Flavor;
					flavors.Add(flavor);
				}
			}
			return flavors;
		}

		// Token: 0x0400094C RID: 2380
		[Dependency]
		private readonly IPrototypeManager _prototypeManager;

		// Token: 0x0400094D RID: 2381
		[Dependency]
		private readonly IConfigurationManager _configManager;

		// Token: 0x0400094E RID: 2382
		private const string BackupFlavorMessage = "flavor-profile-unknown";
	}
}
