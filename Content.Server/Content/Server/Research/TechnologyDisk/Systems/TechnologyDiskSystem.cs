using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Content.Server.Popups;
using Content.Server.Research.Systems;
using Content.Server.Research.TechnologyDisk.Components;
using Content.Shared.Examine;
using Content.Shared.Interaction;
using Content.Shared.Popups;
using Content.Shared.Research.Components;
using Content.Shared.Research.Prototypes;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Localization;
using Robust.Shared.Prototypes;
using Robust.Shared.Random;

namespace Content.Server.Research.TechnologyDisk.Systems
{
	// Token: 0x0200023B RID: 571
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class TechnologyDiskSystem : EntitySystem
	{
		// Token: 0x06000B67 RID: 2919 RVA: 0x0003C25A File Offset: 0x0003A45A
		public override void Initialize()
		{
			base.SubscribeLocalEvent<TechnologyDiskComponent, AfterInteractEvent>(new ComponentEventHandler<TechnologyDiskComponent, AfterInteractEvent>(this.OnAfterInteract), null, null);
			base.SubscribeLocalEvent<TechnologyDiskComponent, ExaminedEvent>(new ComponentEventHandler<TechnologyDiskComponent, ExaminedEvent>(this.OnExamine), null, null);
			base.SubscribeLocalEvent<TechnologyDiskComponent, MapInitEvent>(new ComponentEventHandler<TechnologyDiskComponent, MapInitEvent>(this.OnMapInit), null, null);
		}

		// Token: 0x06000B68 RID: 2920 RVA: 0x0003C298 File Offset: 0x0003A498
		private void OnAfterInteract(EntityUid uid, TechnologyDiskComponent component, AfterInteractEvent args)
		{
			if (!args.Handled && args.CanReach)
			{
				EntityUid? target2 = args.Target;
				if (target2 != null)
				{
					EntityUid target = target2.GetValueOrDefault();
					TechnologyDatabaseComponent database;
					if (!base.HasComp<ResearchServerComponent>(target) || !base.TryComp<TechnologyDatabaseComponent>(target, ref database))
					{
						return;
					}
					if (component.Recipes != null)
					{
						foreach (string recipe in component.Recipes)
						{
							this._research.AddLatheRecipe(target, recipe, database, false);
						}
						base.Dirty(database, null);
					}
					this._popup.PopupEntity(Loc.GetString("tech-disk-inserted"), target, args.User, PopupType.Small);
					this.EntityManager.DeleteEntity(uid);
					args.Handled = true;
					return;
				}
			}
		}

		// Token: 0x06000B69 RID: 2921 RVA: 0x0003C378 File Offset: 0x0003A578
		private void OnExamine(EntityUid uid, TechnologyDiskComponent component, ExaminedEvent args)
		{
			string message = Loc.GetString("tech-disk-examine-none");
			if (component.Recipes != null && component.Recipes.Any<string>())
			{
				LatheRecipePrototype prototype = this._prototype.Index<LatheRecipePrototype>(component.Recipes[0]);
				EntityPrototype resultPrototype = this._prototype.Index<EntityPrototype>(prototype.Result);
				message = Loc.GetString("tech-disk-examine", new ValueTuple<string, object>[]
				{
					new ValueTuple<string, object>("result", resultPrototype.Name)
				});
				if (component.Recipes.Count > 1)
				{
					message = message + " " + Loc.GetString("tech-disk-examine-more");
				}
			}
			args.PushMarkup(message);
		}

		// Token: 0x06000B6A RID: 2922 RVA: 0x0003C428 File Offset: 0x0003A628
		private void OnMapInit(EntityUid uid, TechnologyDiskComponent component, MapInitEvent args)
		{
			if (component.Recipes != null)
			{
				return;
			}
			List<string> allTechs = new List<string>();
			foreach (TechnologyPrototype tech2 in this._prototype.EnumeratePrototypes<TechnologyPrototype>())
			{
				allTechs.AddRange(tech2.UnlockedRecipes);
			}
			allTechs = allTechs.Distinct<string>().ToList<string>();
			List<string> allUnlocked = new List<string>();
			foreach (TechnologyDatabaseComponent database in base.EntityQuery<TechnologyDatabaseComponent>(false))
			{
				allUnlocked.AddRange(database.RecipeIds);
			}
			allUnlocked = allUnlocked.Distinct<string>().ToList<string>();
			List<string> validTechs = (from tech in allTechs
			where !allUnlocked.Contains(tech)
			select tech).ToList<string>();
			component.Recipes = new List<string>();
			component.Recipes.Add(RandomExtensions.Pick<string>(this._random, validTechs));
		}

		// Token: 0x04000709 RID: 1801
		[Dependency]
		private readonly PopupSystem _popup;

		// Token: 0x0400070A RID: 1802
		[Dependency]
		private readonly ResearchSystem _research;

		// Token: 0x0400070B RID: 1803
		[Dependency]
		private readonly IPrototypeManager _prototype;

		// Token: 0x0400070C RID: 1804
		[Dependency]
		private readonly IRobustRandom _random;
	}
}
