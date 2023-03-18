using System;
using System.Runtime.CompilerServices;
using Content.Server.Chemistry.Components.SolutionManager;
using Content.Server.Chemistry.EntitySystems;
using Content.Server.Nutrition.Components;
using Content.Shared.Chemistry.Components;
using Content.Shared.Examine;
using Content.Shared.FixedPoint;
using Content.Shared.Hands.EntitySystems;
using Content.Shared.Interaction;
using Robust.Shared.Audio;
using Robust.Shared.Containers;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Localization;
using Robust.Shared.Player;

namespace Content.Server.Nutrition.EntitySystems
{
	// Token: 0x02000311 RID: 785
	[NullableContext(1)]
	[Nullable(0)]
	internal sealed class SliceableFoodSystem : EntitySystem
	{
		// Token: 0x06001031 RID: 4145 RVA: 0x00053A8C File Offset: 0x00051C8C
		public override void Initialize()
		{
			base.Initialize();
			base.SubscribeLocalEvent<SliceableFoodComponent, ExaminedEvent>(new ComponentEventHandler<SliceableFoodComponent, ExaminedEvent>(this.OnExamined), null, null);
			base.SubscribeLocalEvent<SliceableFoodComponent, InteractUsingEvent>(new ComponentEventHandler<SliceableFoodComponent, InteractUsingEvent>(this.OnInteractUsing), null, null);
			base.SubscribeLocalEvent<SliceableFoodComponent, ComponentStartup>(new ComponentEventHandler<SliceableFoodComponent, ComponentStartup>(this.OnComponentStartup), null, null);
		}

		// Token: 0x06001032 RID: 4146 RVA: 0x00053ADB File Offset: 0x00051CDB
		private void OnInteractUsing(EntityUid uid, SliceableFoodComponent component, InteractUsingEvent args)
		{
			if (args.Handled)
			{
				return;
			}
			if (this.TrySliceFood(uid, args.User, args.Used, component, null, null))
			{
				args.Handled = true;
			}
		}

		// Token: 0x06001033 RID: 4147 RVA: 0x00053B08 File Offset: 0x00051D08
		[NullableContext(2)]
		private bool TrySliceFood(EntityUid uid, EntityUid user, EntityUid usedItem, SliceableFoodComponent component = null, FoodComponent food = null, TransformComponent transform = null)
		{
			if (!base.Resolve<SliceableFoodComponent, FoodComponent, TransformComponent>(uid, ref component, ref food, ref transform, true) || string.IsNullOrEmpty(component.Slice))
			{
				return false;
			}
			Solution solution;
			if (!this._solutionContainerSystem.TryGetSolution(uid, food.SolutionName, out solution, null))
			{
				return false;
			}
			UtensilComponent utensil;
			if (!this.EntityManager.TryGetComponent<UtensilComponent>(usedItem, ref utensil) || (utensil.Types & UtensilType.Knife) == UtensilType.None)
			{
				return false;
			}
			EntityUid sliceUid = this.EntityManager.SpawnEntity(component.Slice, transform.Coordinates);
			Solution lostSolution = this._solutionContainerSystem.SplitSolution(uid, solution, solution.Volume / FixedPoint2.New((int)component.Count));
			this.FillSlice(sliceUid, lostSolution);
			bool inCont = this._containerSystem.IsEntityInContainer(component.Owner, null);
			if (inCont)
			{
				this._handsSystem.PickupOrDrop(new EntityUid?(user), sliceUid, true, false, null, null);
			}
			else
			{
				TransformComponent xform = base.Transform(sliceUid);
				this._containerSystem.AttachParentToContainerOrGrid(xform);
				xform.LocalRotation = 0f;
			}
			SoundSystem.Play(component.Sound.GetSound(null, null), Filter.Pvs(uid, 2f, null, null, null), transform.Coordinates, new AudioParams?(AudioParams.Default.WithVolume(-2f)));
			SliceableFoodComponent sliceableFoodComponent = component;
			sliceableFoodComponent.Count -= 1;
			if (component.Count < 1)
			{
				this.EntityManager.DeleteEntity(uid);
				return true;
			}
			if (component.Count > 1)
			{
				return true;
			}
			sliceUid = this.EntityManager.SpawnEntity(component.Slice, transform.Coordinates);
			this.FillSlice(sliceUid, solution);
			if (inCont)
			{
				this._handsSystem.PickupOrDrop(new EntityUid?(user), sliceUid, true, false, null, null);
			}
			else
			{
				TransformComponent xform2 = base.Transform(sliceUid);
				this._containerSystem.AttachParentToContainerOrGrid(xform2);
				xform2.LocalRotation = 0f;
			}
			this.EntityManager.DeleteEntity(uid);
			return true;
		}

		// Token: 0x06001034 RID: 4148 RVA: 0x00053CF0 File Offset: 0x00051EF0
		private void FillSlice(EntityUid sliceUid, Solution solution)
		{
			FoodComponent sliceFoodComp;
			Solution itsSolution;
			if (this.EntityManager.TryGetComponent<FoodComponent>(sliceUid, ref sliceFoodComp) && this._solutionContainerSystem.TryGetSolution(sliceUid, sliceFoodComp.SolutionName, out itsSolution, null))
			{
				this._solutionContainerSystem.RemoveAllSolution(sliceUid, itsSolution);
				Solution lostSolutionPart = solution.SplitSolution(itsSolution.AvailableVolume);
				this._solutionContainerSystem.TryAddSolution(sliceUid, itsSolution, lostSolutionPart);
			}
		}

		// Token: 0x06001035 RID: 4149 RVA: 0x00053D50 File Offset: 0x00051F50
		private void OnComponentStartup(EntityUid uid, SliceableFoodComponent component, ComponentStartup args)
		{
			component.Count = component.TotalCount;
			FoodComponent foodComp = this.EntityManager.EnsureComponent<FoodComponent>(uid);
			this.EntityManager.EnsureComponent<SolutionContainerManagerComponent>(uid);
			this._solutionContainerSystem.EnsureSolution(uid, foodComp.SolutionName, null);
		}

		// Token: 0x06001036 RID: 4150 RVA: 0x00053D97 File Offset: 0x00051F97
		private void OnExamined(EntityUid uid, SliceableFoodComponent component, ExaminedEvent args)
		{
			args.PushMarkup(Loc.GetString("sliceable-food-component-on-examine-remaining-slices-text", new ValueTuple<string, object>[]
			{
				new ValueTuple<string, object>("remainingCount", component.Count)
			}));
		}

		// Token: 0x04000960 RID: 2400
		[Dependency]
		private readonly SolutionContainerSystem _solutionContainerSystem;

		// Token: 0x04000961 RID: 2401
		[Dependency]
		private readonly SharedHandsSystem _handsSystem;

		// Token: 0x04000962 RID: 2402
		[Dependency]
		private readonly SharedContainerSystem _containerSystem;
	}
}
