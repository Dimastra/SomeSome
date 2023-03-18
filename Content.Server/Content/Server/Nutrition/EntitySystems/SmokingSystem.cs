using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Content.Server.Atmos.EntitySystems;
using Content.Server.Body.Components;
using Content.Server.Body.Systems;
using Content.Server.Chemistry.Components.SolutionManager;
using Content.Server.Chemistry.EntitySystems;
using Content.Server.Nutrition.Components;
using Content.Shared.Chemistry;
using Content.Shared.Chemistry.Components;
using Content.Shared.Chemistry.Reagent;
using Content.Shared.Clothing.Components;
using Content.Shared.Clothing.EntitySystems;
using Content.Shared.Containers.ItemSlots;
using Content.Shared.FixedPoint;
using Content.Shared.Interaction;
using Content.Shared.Inventory;
using Content.Shared.Item;
using Content.Shared.Smoking;
using Content.Shared.Temperature;
using Robust.Server.GameObjects;
using Robust.Shared.Containers;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Maths;

namespace Content.Server.Nutrition.EntitySystems
{
	// Token: 0x02000312 RID: 786
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class SmokingSystem : EntitySystem
	{
		// Token: 0x06001038 RID: 4152 RVA: 0x00053DD4 File Offset: 0x00051FD4
		private void InitializeCigars()
		{
			base.SubscribeLocalEvent<CigarComponent, ActivateInWorldEvent>(new ComponentEventHandler<CigarComponent, ActivateInWorldEvent>(this.OnCigarActivatedEvent), null, null);
			base.SubscribeLocalEvent<CigarComponent, InteractUsingEvent>(new ComponentEventHandler<CigarComponent, InteractUsingEvent>(this.OnCigarInteractUsingEvent), null, null);
			base.SubscribeLocalEvent<CigarComponent, SmokableSolutionEmptyEvent>(new ComponentEventHandler<CigarComponent, SmokableSolutionEmptyEvent>(this.OnCigarSolutionEmptyEvent), null, null);
			base.SubscribeLocalEvent<CigarComponent, AfterInteractEvent>(new ComponentEventHandler<CigarComponent, AfterInteractEvent>(this.OnCigarAfterInteract), null, null);
		}

		// Token: 0x06001039 RID: 4153 RVA: 0x00053E34 File Offset: 0x00052034
		private void OnCigarActivatedEvent(EntityUid uid, CigarComponent component, ActivateInWorldEvent args)
		{
			if (args.Handled)
			{
				return;
			}
			SmokableComponent smokable;
			if (!this.EntityManager.TryGetComponent<SmokableComponent>(uid, ref smokable))
			{
				return;
			}
			if (smokable.State != SmokableState.Lit)
			{
				return;
			}
			this.SetSmokableState(uid, SmokableState.Burnt, smokable, null, null);
			args.Handled = true;
		}

		// Token: 0x0600103A RID: 4154 RVA: 0x00053E78 File Offset: 0x00052078
		private void OnCigarInteractUsingEvent(EntityUid uid, CigarComponent component, InteractUsingEvent args)
		{
			if (args.Handled)
			{
				return;
			}
			SmokableComponent smokable;
			if (!this.EntityManager.TryGetComponent<SmokableComponent>(uid, ref smokable))
			{
				return;
			}
			if (smokable.State != SmokableState.Unlit)
			{
				return;
			}
			IsHotEvent isHotEvent = new IsHotEvent();
			base.RaiseLocalEvent<IsHotEvent>(args.Used, isHotEvent, false);
			if (!isHotEvent.IsHot)
			{
				return;
			}
			this.SetSmokableState(uid, SmokableState.Lit, smokable, null, null);
			args.Handled = true;
		}

		// Token: 0x0600103B RID: 4155 RVA: 0x00053ED8 File Offset: 0x000520D8
		public void OnCigarAfterInteract(EntityUid uid, CigarComponent component, AfterInteractEvent args)
		{
			EntityUid? targetEntity = args.Target;
			SmokableComponent smokable;
			if (targetEntity == null || !args.CanReach || !this.EntityManager.TryGetComponent<SmokableComponent>(uid, ref smokable) || smokable.State == SmokableState.Lit)
			{
				return;
			}
			IsHotEvent isHotEvent = new IsHotEvent();
			base.RaiseLocalEvent<IsHotEvent>(targetEntity.Value, isHotEvent, true);
			if (!isHotEvent.IsHot)
			{
				return;
			}
			this.SetSmokableState(uid, SmokableState.Lit, smokable, null, null);
			args.Handled = true;
		}

		// Token: 0x0600103C RID: 4156 RVA: 0x00053F47 File Offset: 0x00052147
		private void OnCigarSolutionEmptyEvent(EntityUid uid, CigarComponent component, SmokableSolutionEmptyEvent args)
		{
			this.SetSmokableState(uid, SmokableState.Burnt, null, null, null);
		}

		// Token: 0x0600103D RID: 4157 RVA: 0x00053F54 File Offset: 0x00052154
		public override void Initialize()
		{
			base.SubscribeLocalEvent<SmokableComponent, IsHotEvent>(new ComponentEventHandler<SmokableComponent, IsHotEvent>(this.OnSmokableIsHotEvent), null, null);
			base.SubscribeLocalEvent<SmokableComponent, ComponentShutdown>(new ComponentEventHandler<SmokableComponent, ComponentShutdown>(this.OnSmokableShutdownEvent), null, null);
			this.InitializeCigars();
			this.InitializePipes();
		}

		// Token: 0x0600103E RID: 4158 RVA: 0x00053F8C File Offset: 0x0005218C
		[NullableContext(2)]
		public void SetSmokableState(EntityUid uid, SmokableState state, SmokableComponent smokable = null, AppearanceComponent appearance = null, ClothingComponent clothing = null)
		{
			if (!base.Resolve<SmokableComponent, AppearanceComponent, ClothingComponent>(uid, ref smokable, ref appearance, ref clothing, true))
			{
				return;
			}
			smokable.State = state;
			this._appearance.SetData(uid, SmokingVisuals.Smoking, state, appearance);
			string text;
			if (state != SmokableState.Lit)
			{
				if (state != SmokableState.Burnt)
				{
					text = smokable.UnlitPrefix;
				}
				else
				{
					text = smokable.BurntPrefix;
				}
			}
			else
			{
				text = smokable.LitPrefix;
			}
			string newState = text;
			this._clothing.SetEquippedPrefix(uid, newState, clothing);
			this._items.SetHeldPrefix(uid, newState, null);
			if (state == SmokableState.Lit)
			{
				this._active.Add(uid);
				return;
			}
			this._active.Remove(uid);
		}

		// Token: 0x0600103F RID: 4159 RVA: 0x0005402C File Offset: 0x0005222C
		private void OnSmokableIsHotEvent(EntityUid uid, SmokableComponent component, IsHotEvent args)
		{
			args.IsHot = (component.State == SmokableState.Lit);
		}

		// Token: 0x06001040 RID: 4160 RVA: 0x0005403D File Offset: 0x0005223D
		private void OnSmokableShutdownEvent(EntityUid uid, SmokableComponent component, ComponentShutdown args)
		{
			this._active.Remove(uid);
		}

		// Token: 0x06001041 RID: 4161 RVA: 0x0005404C File Offset: 0x0005224C
		public override void Update(float frameTime)
		{
			this._timer += frameTime;
			if (this._timer < 3f)
			{
				return;
			}
			foreach (EntityUid uid in this._active.ToArray<EntityUid>())
			{
				SmokableComponent smokable;
				Solution solution;
				if (!base.TryComp<SmokableComponent>(uid, ref smokable))
				{
					this._active.Remove(uid);
				}
				else if (!this._solutionContainerSystem.TryGetSolution(uid, smokable.Solution, out solution, null))
				{
					this._active.Remove(uid);
				}
				else
				{
					if (smokable.ExposeTemperature > 0f && smokable.ExposeVolume > 0f)
					{
						TransformComponent transform = base.Transform(uid);
						EntityUid? entityUid = transform.GridUid;
						if (entityUid != null)
						{
							EntityUid gridUid = entityUid.GetValueOrDefault();
							Vector2i position = this._transformSystem.GetGridOrMapTilePosition(uid, transform);
							this._atmos.HotspotExpose(gridUid, position, smokable.ExposeTemperature, smokable.ExposeVolume, true);
						}
					}
					Solution inhaledSolution = this._solutionContainerSystem.SplitSolution(uid, solution, smokable.InhaleAmount * this._timer);
					if (solution.Volume == FixedPoint2.Zero)
					{
						base.RaiseLocalEvent<SmokableSolutionEmptyEvent>(uid, new SmokableSolutionEmptyEvent(), true);
					}
					IContainerManager containerManager;
					EntityUid? inMaskSlotUid;
					if (!(inhaledSolution.Volume == FixedPoint2.Zero) && ContainerHelpers.TryGetContainerMan(smokable.Owner, ref containerManager, null) && this._inventorySystem.TryGetSlotEntity(containerManager.Owner, "mask", out inMaskSlotUid, null, null))
					{
						EntityUid? entityUid = inMaskSlotUid;
						EntityUid owner = smokable.Owner;
						BloodstreamComponent bloodstream;
						if (entityUid != null && (entityUid == null || entityUid.GetValueOrDefault() == owner) && base.TryComp<BloodstreamComponent>(containerManager.Owner, ref bloodstream))
						{
							this._reactiveSystem.ReactionEntity(containerManager.Owner, ReactionMethod.Ingestion, inhaledSolution);
							this._bloodstreamSystem.TryAddToChemicals(containerManager.Owner, inhaledSolution, bloodstream);
						}
					}
				}
			}
			this._timer -= 3f;
		}

		// Token: 0x06001042 RID: 4162 RVA: 0x00054258 File Offset: 0x00052458
		private void InitializePipes()
		{
			base.SubscribeLocalEvent<SmokingPipeComponent, InteractUsingEvent>(new ComponentEventHandler<SmokingPipeComponent, InteractUsingEvent>(this.OnPipeInteractUsingEvent), null, null);
			base.SubscribeLocalEvent<SmokingPipeComponent, SmokableSolutionEmptyEvent>(new ComponentEventHandler<SmokingPipeComponent, SmokableSolutionEmptyEvent>(this.OnPipeSolutionEmptyEvent), null, null);
			base.SubscribeLocalEvent<SmokingPipeComponent, AfterInteractEvent>(new ComponentEventHandler<SmokingPipeComponent, AfterInteractEvent>(this.OnPipeAfterInteract), null, null);
			base.SubscribeLocalEvent<SmokingPipeComponent, ComponentInit>(new ComponentEventHandler<SmokingPipeComponent, ComponentInit>(this.OnComponentInit), null, null);
		}

		// Token: 0x06001043 RID: 4163 RVA: 0x000542B5 File Offset: 0x000524B5
		public void OnComponentInit(EntityUid uid, SmokingPipeComponent pipe, ComponentInit args)
		{
			this._itemSlotsSystem.AddItemSlot(uid, "bowl_slot", pipe.BowlSlot, null);
		}

		// Token: 0x06001044 RID: 4164 RVA: 0x000542D0 File Offset: 0x000524D0
		private void OnPipeInteractUsingEvent(EntityUid uid, SmokingPipeComponent component, InteractUsingEvent args)
		{
			if (args.Handled)
			{
				return;
			}
			SmokableComponent smokable;
			if (!this.EntityManager.TryGetComponent<SmokableComponent>(uid, ref smokable))
			{
				return;
			}
			if (smokable.State != SmokableState.Unlit)
			{
				return;
			}
			IsHotEvent isHotEvent = new IsHotEvent();
			base.RaiseLocalEvent<IsHotEvent>(args.Used, isHotEvent, false);
			if (!isHotEvent.IsHot)
			{
				return;
			}
			if (this.TryTransferReagents(component, smokable))
			{
				this.SetSmokableState(uid, SmokableState.Lit, smokable, null, null);
			}
			args.Handled = true;
		}

		// Token: 0x06001045 RID: 4165 RVA: 0x0005433C File Offset: 0x0005253C
		public void OnPipeAfterInteract(EntityUid uid, SmokingPipeComponent component, AfterInteractEvent args)
		{
			EntityUid? targetEntity = args.Target;
			SmokableComponent smokable;
			if (targetEntity == null || !args.CanReach || !this.EntityManager.TryGetComponent<SmokableComponent>(uid, ref smokable) || smokable.State == SmokableState.Lit)
			{
				return;
			}
			IsHotEvent isHotEvent = new IsHotEvent();
			base.RaiseLocalEvent<IsHotEvent>(targetEntity.Value, isHotEvent, true);
			if (!isHotEvent.IsHot)
			{
				return;
			}
			if (this.TryTransferReagents(component, smokable))
			{
				this.SetSmokableState(uid, SmokableState.Lit, smokable, null, null);
			}
			args.Handled = true;
		}

		// Token: 0x06001046 RID: 4166 RVA: 0x000543B5 File Offset: 0x000525B5
		private void OnPipeSolutionEmptyEvent(EntityUid uid, SmokingPipeComponent component, SmokableSolutionEmptyEvent args)
		{
			this._itemSlotsSystem.SetLock(component.Owner, component.BowlSlot, false, null);
			this.SetSmokableState(uid, SmokableState.Unlit, null, null, null);
		}

		// Token: 0x06001047 RID: 4167 RVA: 0x000543DC File Offset: 0x000525DC
		private bool TryTransferReagents(SmokingPipeComponent component, SmokableComponent smokable)
		{
			if (component.BowlSlot.Item == null)
			{
				return false;
			}
			EntityUid contents = component.BowlSlot.Item.Value;
			SolutionContainerManagerComponent reagents;
			Solution pipeSolution;
			if (!base.TryComp<SolutionContainerManagerComponent>(contents, ref reagents) || !this._solutionContainerSystem.TryGetSolution(smokable.Owner, smokable.Solution, out pipeSolution, null))
			{
				return false;
			}
			foreach (KeyValuePair<string, Solution> reagentSolution in reagents.Solutions)
			{
				this._solutionContainerSystem.TryAddSolution(smokable.Owner, pipeSolution, reagentSolution.Value);
			}
			this.EntityManager.DeleteEntity(contents);
			this._itemSlotsSystem.SetLock(component.Owner, component.BowlSlot, true, null);
			return true;
		}

		// Token: 0x04000963 RID: 2403
		[Dependency]
		private readonly ReactiveSystem _reactiveSystem;

		// Token: 0x04000964 RID: 2404
		[Dependency]
		private readonly SolutionContainerSystem _solutionContainerSystem;

		// Token: 0x04000965 RID: 2405
		[Dependency]
		private readonly BloodstreamSystem _bloodstreamSystem;

		// Token: 0x04000966 RID: 2406
		[Dependency]
		private readonly AtmosphereSystem _atmos;

		// Token: 0x04000967 RID: 2407
		[Dependency]
		private readonly TransformSystem _transformSystem;

		// Token: 0x04000968 RID: 2408
		[Dependency]
		private readonly InventorySystem _inventorySystem;

		// Token: 0x04000969 RID: 2409
		[Dependency]
		private readonly ClothingSystem _clothing;

		// Token: 0x0400096A RID: 2410
		[Dependency]
		private readonly SharedItemSystem _items;

		// Token: 0x0400096B RID: 2411
		[Dependency]
		private readonly SharedAppearanceSystem _appearance;

		// Token: 0x0400096C RID: 2412
		private const float UpdateTimer = 3f;

		// Token: 0x0400096D RID: 2413
		private float _timer;

		// Token: 0x0400096E RID: 2414
		private readonly HashSet<EntityUid> _active = new HashSet<EntityUid>();

		// Token: 0x0400096F RID: 2415
		[Dependency]
		private readonly ItemSlotsSystem _itemSlotsSystem;
	}
}
