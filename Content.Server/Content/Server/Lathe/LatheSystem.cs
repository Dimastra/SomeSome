using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Runtime.CompilerServices;
using Content.Server.Administration.Logs;
using Content.Server.Construction;
using Content.Server.Lathe.Components;
using Content.Server.Materials;
using Content.Server.Power.Components;
using Content.Server.Power.EntitySystems;
using Content.Server.Stack;
using Content.Server.UserInterface;
using Content.Shared.Administration.Logs;
using Content.Shared.Database;
using Content.Shared.Lathe;
using Content.Shared.Materials;
using Content.Shared.Research.Components;
using Content.Shared.Research.Prototypes;
using Robust.Server.GameObjects;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Prototypes;
using Robust.Shared.Timing;

namespace Content.Server.Lathe
{
	// Token: 0x02000421 RID: 1057
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class LatheSystem : SharedLatheSystem
	{
		// Token: 0x0600155A RID: 5466 RVA: 0x00070128 File Offset: 0x0006E328
		public override void Initialize()
		{
			base.Initialize();
			base.SubscribeLocalEvent<LatheComponent, GetMaterialWhitelistEvent>(new ComponentEventRefHandler<LatheComponent, GetMaterialWhitelistEvent>(this.OnGetWhitelist), null, null);
			base.SubscribeLocalEvent<LatheComponent, MapInitEvent>(new ComponentEventHandler<LatheComponent, MapInitEvent>(this.OnMapInit), null, null);
			base.SubscribeLocalEvent<LatheComponent, PowerChangedEvent>(new ComponentEventRefHandler<LatheComponent, PowerChangedEvent>(this.OnPowerChanged), null, null);
			base.SubscribeLocalEvent<LatheComponent, RefreshPartsEvent>(new ComponentEventHandler<LatheComponent, RefreshPartsEvent>(this.OnPartsRefresh), null, null);
			base.SubscribeLocalEvent<LatheComponent, UpgradeExamineEvent>(new ComponentEventHandler<LatheComponent, UpgradeExamineEvent>(this.OnUpgradeExamine), null, null);
			base.SubscribeLocalEvent<LatheComponent, TechnologyDatabaseModifiedEvent>(new ComponentEventRefHandler<LatheComponent, TechnologyDatabaseModifiedEvent>(this.OnDatabaseModified), null, null);
			base.SubscribeLocalEvent<LatheComponent, LatheQueueRecipeMessage>(new ComponentEventHandler<LatheComponent, LatheQueueRecipeMessage>(this.OnLatheQueueRecipeMessage), null, null);
			base.SubscribeLocalEvent<LatheComponent, LatheSyncRequestMessage>(new ComponentEventHandler<LatheComponent, LatheSyncRequestMessage>(this.OnLatheSyncRequestMessage), null, null);
			base.SubscribeLocalEvent<LatheComponent, BeforeActivatableUIOpenEvent>(delegate(EntityUid u, LatheComponent c, BeforeActivatableUIOpenEvent _)
			{
				this.UpdateUserInterfaceState(u, c);
			}, null, null);
			base.SubscribeLocalEvent<LatheComponent, MaterialAmountChangedEvent>(new ComponentEventRefHandler<LatheComponent, MaterialAmountChangedEvent>(this.OnMaterialAmountChanged), null, null);
			base.SubscribeLocalEvent<TechnologyDatabaseComponent, LatheGetRecipesEvent>(new ComponentEventHandler<TechnologyDatabaseComponent, LatheGetRecipesEvent>(this.OnGetRecipes), null, null);
		}

		// Token: 0x0600155B RID: 5467 RVA: 0x00070218 File Offset: 0x0006E418
		public override void Update(float frameTime)
		{
			foreach (ValueTuple<LatheProducingComponent, LatheComponent> valueTuple in base.EntityQuery<LatheProducingComponent, LatheComponent>(false))
			{
				LatheProducingComponent comp = valueTuple.Item1;
				LatheComponent lathe = valueTuple.Item2;
				if (lathe.CurrentRecipe != null && this._timing.CurTime - comp.StartTime >= comp.ProductionLength)
				{
					this.FinishProducing(comp.Owner, lathe, null);
				}
			}
		}

		// Token: 0x0600155C RID: 5468 RVA: 0x000702A4 File Offset: 0x0006E4A4
		private void OnGetWhitelist(EntityUid uid, LatheComponent component, ref GetMaterialWhitelistEvent args)
		{
			if (args.Storage != uid)
			{
				return;
			}
			List<string> materialWhitelist = new List<string>();
			foreach (string id in this.GetAllBaseRecipes(component))
			{
				LatheRecipePrototype proto;
				if (this._proto.TryIndex<LatheRecipePrototype>(id, ref proto))
				{
					foreach (KeyValuePair<string, int> keyValuePair in proto.RequiredMaterials)
					{
						string text;
						int num;
						keyValuePair.Deconstruct(out text, out num);
						string mat = text;
						if (!materialWhitelist.Contains(mat))
						{
							materialWhitelist.Add(mat);
						}
					}
				}
			}
			List<string> combined = args.Whitelist.Union(materialWhitelist).ToList<string>();
			args.Whitelist = combined;
		}

		// Token: 0x0600155D RID: 5469 RVA: 0x00070390 File Offset: 0x0006E590
		[NullableContext(2)]
		public bool TryGetAvailableRecipes(EntityUid uid, [Nullable(new byte[]
		{
			2,
			1
		})] [NotNullWhen(true)] out List<string> recipes, [NotNullWhen(true)] LatheComponent component = null)
		{
			recipes = null;
			if (!base.Resolve<LatheComponent>(uid, ref component, true))
			{
				return false;
			}
			recipes = this.GetAvailableRecipes(uid, component);
			return true;
		}

		// Token: 0x0600155E RID: 5470 RVA: 0x000703B0 File Offset: 0x0006E5B0
		public List<string> GetAvailableRecipes(EntityUid uid, LatheComponent component)
		{
			LatheGetRecipesEvent ev = new LatheGetRecipesEvent(uid)
			{
				Recipes = component.StaticRecipes
			};
			base.RaiseLocalEvent<LatheGetRecipesEvent>(uid, ev, false);
			return ev.Recipes;
		}

		// Token: 0x0600155F RID: 5471 RVA: 0x000703DF File Offset: 0x0006E5DF
		public List<string> GetAllBaseRecipes(LatheComponent component)
		{
			if (component.DynamicRecipes != null)
			{
				return component.StaticRecipes.Union(component.DynamicRecipes).ToList<string>();
			}
			return component.StaticRecipes;
		}

		// Token: 0x06001560 RID: 5472 RVA: 0x00070408 File Offset: 0x0006E608
		public bool TryAddToQueue(EntityUid uid, LatheRecipePrototype recipe, [Nullable(2)] LatheComponent component = null)
		{
			if (!base.Resolve<LatheComponent>(uid, ref component, true))
			{
				return false;
			}
			if (!base.CanProduce(uid, recipe, 1, component))
			{
				return false;
			}
			foreach (KeyValuePair<string, int> keyValuePair in recipe.RequiredMaterials)
			{
				string text;
				int num;
				keyValuePair.Deconstruct(out text, out num);
				string mat = text;
				int amount = num;
				int adjustedAmount = recipe.ApplyMaterialDiscount ? ((int)((float)(-(float)amount) * component.MaterialUseMultiplier)) : (-amount);
				this._materialStorage.TryChangeMaterialAmount(uid, mat, adjustedAmount, null);
			}
			component.Queue.Add(recipe);
			return true;
		}

		// Token: 0x06001561 RID: 5473 RVA: 0x000704B8 File Offset: 0x0006E6B8
		[NullableContext(2)]
		public bool TryStartProducing(EntityUid uid, LatheComponent component = null)
		{
			if (!base.Resolve<LatheComponent>(uid, ref component, true))
			{
				return false;
			}
			if (component.CurrentRecipe != null || component.Queue.Count <= 0 || !this.IsPowered(uid, this.EntityManager, null))
			{
				return false;
			}
			LatheRecipePrototype recipe = component.Queue.First<LatheRecipePrototype>();
			component.Queue.RemoveAt(0);
			LatheProducingComponent latheProducingComponent = base.EnsureComp<LatheProducingComponent>(uid);
			latheProducingComponent.StartTime = this._timing.CurTime;
			latheProducingComponent.ProductionLength = recipe.CompleteTime * (double)component.TimeMultiplier;
			component.CurrentRecipe = recipe;
			this._audio.PlayPvs(component.ProducingSound, uid, null);
			this.UpdateRunningAppearance(uid, true);
			this.UpdateUserInterfaceState(uid, component);
			return true;
		}

		// Token: 0x06001562 RID: 5474 RVA: 0x00070578 File Offset: 0x0006E778
		[NullableContext(2)]
		public void FinishProducing(EntityUid uid, LatheComponent comp = null, LatheProducingComponent prodComp = null)
		{
			if (!base.Resolve<LatheComponent, LatheProducingComponent>(uid, ref comp, ref prodComp, false))
			{
				return;
			}
			if (comp.CurrentRecipe != null)
			{
				EntityUid result = base.Spawn(comp.CurrentRecipe.Result, base.Transform(uid).Coordinates);
				this._stack.TryMergeToContacts(result, null, null);
			}
			comp.CurrentRecipe = null;
			prodComp.StartTime = this._timing.CurTime;
			if (!this.TryStartProducing(uid, comp))
			{
				base.RemCompDeferred(uid, prodComp);
				this.UpdateUserInterfaceState(uid, comp);
				this.UpdateRunningAppearance(uid, false);
			}
		}

		// Token: 0x06001563 RID: 5475 RVA: 0x00070604 File Offset: 0x0006E804
		[NullableContext(2)]
		public void UpdateUserInterfaceState(EntityUid uid, LatheComponent component = null)
		{
			if (!base.Resolve<LatheComponent>(uid, ref component, true))
			{
				return;
			}
			BoundUserInterface ui = this._uiSys.GetUi(uid, LatheUiKey.Key, null);
			LatheRecipePrototype producing = component.CurrentRecipe ?? component.Queue.FirstOrDefault<LatheRecipePrototype>();
			LatheUpdateState state = new LatheUpdateState(this.GetAvailableRecipes(uid, component), component.Queue, producing);
			this._uiSys.SetUiState(ui, state, null, true);
		}

		// Token: 0x06001564 RID: 5476 RVA: 0x0007066C File Offset: 0x0006E86C
		private void OnGetRecipes(EntityUid uid, TechnologyDatabaseComponent component, LatheGetRecipesEvent args)
		{
			LatheComponent latheComponent;
			if (uid != args.Lathe || !base.TryComp<LatheComponent>(uid, ref latheComponent) || latheComponent.DynamicRecipes == null)
			{
				return;
			}
			args.Recipes = args.Recipes.Union(from r in component.RecipeIds
			where latheComponent.DynamicRecipes.Contains(r)
			select r).ToList<string>();
		}

		// Token: 0x06001565 RID: 5477 RVA: 0x000706D7 File Offset: 0x0006E8D7
		private void OnMaterialAmountChanged(EntityUid uid, LatheComponent component, ref MaterialAmountChangedEvent args)
		{
			this.UpdateUserInterfaceState(uid, component);
		}

		// Token: 0x06001566 RID: 5478 RVA: 0x000706E4 File Offset: 0x0006E8E4
		private void OnMapInit(EntityUid uid, LatheComponent component, MapInitEvent args)
		{
			this._appearance.SetData(uid, LatheVisuals.IsInserting, false, null);
			this._appearance.SetData(uid, LatheVisuals.IsRunning, false, null);
			this._materialStorage.UpdateMaterialWhitelist(uid, null);
		}

		// Token: 0x06001567 RID: 5479 RVA: 0x00070730 File Offset: 0x0006E930
		private void UpdateRunningAppearance(EntityUid uid, bool isRunning)
		{
			this._appearance.SetData(uid, LatheVisuals.IsRunning, isRunning, null);
		}

		// Token: 0x06001568 RID: 5480 RVA: 0x0007074B File Offset: 0x0006E94B
		private void OnPowerChanged(EntityUid uid, LatheComponent component, ref PowerChangedEvent args)
		{
			if (!args.Powered)
			{
				base.RemComp<LatheProducingComponent>(uid);
				this.UpdateRunningAppearance(uid, false);
				return;
			}
			if (component.CurrentRecipe != null)
			{
				base.EnsureComp<LatheProducingComponent>(uid);
				this.TryStartProducing(uid, component);
			}
		}

		// Token: 0x06001569 RID: 5481 RVA: 0x00070780 File Offset: 0x0006E980
		private void OnPartsRefresh(EntityUid uid, LatheComponent component, RefreshPartsEvent args)
		{
			float printTimeRating = args.PartRatings[component.MachinePartPrintTime];
			float materialUseRating = args.PartRatings[component.MachinePartMaterialUse];
			component.TimeMultiplier = MathF.Pow(component.PartRatingPrintTimeMultiplier, printTimeRating - 1f);
			component.MaterialUseMultiplier = MathF.Pow(component.PartRatingMaterialUseMultiplier, materialUseRating - 1f);
			base.Dirty(component, null);
		}

		// Token: 0x0600156A RID: 5482 RVA: 0x000707E9 File Offset: 0x0006E9E9
		private void OnUpgradeExamine(EntityUid uid, LatheComponent component, UpgradeExamineEvent args)
		{
			args.AddPercentageUpgrade("lathe-component-upgrade-speed", 1f / component.TimeMultiplier);
			args.AddPercentageUpgrade("lathe-component-upgrade-material-use", component.MaterialUseMultiplier);
		}

		// Token: 0x0600156B RID: 5483 RVA: 0x00070813 File Offset: 0x0006EA13
		private void OnDatabaseModified(EntityUid uid, LatheComponent component, ref TechnologyDatabaseModifiedEvent args)
		{
			this.UpdateUserInterfaceState(uid, component);
		}

		// Token: 0x0600156C RID: 5484 RVA: 0x0007081D File Offset: 0x0006EA1D
		protected override bool HasRecipe(EntityUid uid, LatheRecipePrototype recipe, LatheComponent component)
		{
			return this.GetAvailableRecipes(uid, component).Contains(recipe.ID);
		}

		// Token: 0x0600156D RID: 5485 RVA: 0x00070834 File Offset: 0x0006EA34
		private void OnLatheQueueRecipeMessage(EntityUid uid, LatheComponent component, LatheQueueRecipeMessage args)
		{
			LatheRecipePrototype recipe;
			if (this._proto.TryIndex<LatheRecipePrototype>(args.ID, ref recipe))
			{
				int count = 0;
				for (int i = 0; i < args.Quantity; i++)
				{
					if (this.TryAddToQueue(uid, recipe, component))
					{
						count++;
					}
				}
				if (count > 0 && args.Session.AttachedEntity != null)
				{
					ISharedAdminLogManager adminLogger = this._adminLogger;
					LogType type = LogType.Action;
					LogImpact impact = LogImpact.Low;
					LogStringHandler logStringHandler = new LogStringHandler(13, 4);
					logStringHandler.AppendFormatted<EntityStringRepresentation>(base.ToPrettyString(args.Session.AttachedEntity.Value), "player", "ToPrettyString(args.Session.AttachedEntity.Value)");
					logStringHandler.AppendLiteral(" queued ");
					logStringHandler.AppendFormatted<int>(count, "count");
					logStringHandler.AppendLiteral(" ");
					logStringHandler.AppendFormatted(recipe.Name);
					logStringHandler.AppendLiteral(" at ");
					logStringHandler.AppendFormatted<EntityStringRepresentation>(base.ToPrettyString(uid), "lathe", "ToPrettyString(uid)");
					adminLogger.Add(type, impact, ref logStringHandler);
				}
			}
			this.TryStartProducing(uid, component);
			this.UpdateUserInterfaceState(uid, component);
		}

		// Token: 0x0600156E RID: 5486 RVA: 0x00070945 File Offset: 0x0006EB45
		private void OnLatheSyncRequestMessage(EntityUid uid, LatheComponent component, LatheSyncRequestMessage args)
		{
			this.UpdateUserInterfaceState(uid, component);
		}

		// Token: 0x04000D4F RID: 3407
		[Dependency]
		private readonly IGameTiming _timing;

		// Token: 0x04000D50 RID: 3408
		[Dependency]
		private readonly IPrototypeManager _proto;

		// Token: 0x04000D51 RID: 3409
		[Dependency]
		private readonly IAdminLogManager _adminLogger;

		// Token: 0x04000D52 RID: 3410
		[Dependency]
		private readonly SharedAppearanceSystem _appearance;

		// Token: 0x04000D53 RID: 3411
		[Dependency]
		private readonly SharedAudioSystem _audio;

		// Token: 0x04000D54 RID: 3412
		[Dependency]
		private readonly UserInterfaceSystem _uiSys;

		// Token: 0x04000D55 RID: 3413
		[Dependency]
		private readonly MaterialStorageSystem _materialStorage;

		// Token: 0x04000D56 RID: 3414
		[Dependency]
		private readonly StackSystem _stack;
	}
}
