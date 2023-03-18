using System;
using System.Runtime.CompilerServices;
using Content.Server.Chemistry.Components.SolutionManager;
using Content.Server.Chemistry.EntitySystems;
using Content.Server.Nutrition.Components;
using Content.Shared.Chemistry.Components;
using Content.Shared.Tag;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;

namespace Content.Server.Nutrition.EntitySystems
{
	// Token: 0x02000315 RID: 789
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class TrashOnEmptySystem : EntitySystem
	{
		// Token: 0x06001055 RID: 4181 RVA: 0x00054974 File Offset: 0x00052B74
		public override void Initialize()
		{
			base.Initialize();
			base.SubscribeLocalEvent<TrashOnEmptyComponent, ComponentStartup>(new ComponentEventHandler<TrashOnEmptyComponent, ComponentStartup>(this.OnStartup), null, null);
			base.SubscribeLocalEvent<TrashOnEmptyComponent, SolutionChangedEvent>(new ComponentEventHandler<TrashOnEmptyComponent, SolutionChangedEvent>(this.OnSolutionChange), null, null);
		}

		// Token: 0x06001056 RID: 4182 RVA: 0x000549A4 File Offset: 0x00052BA4
		public void OnStartup(EntityUid uid, TrashOnEmptyComponent component, ComponentStartup args)
		{
			this.CheckSolutions(component);
		}

		// Token: 0x06001057 RID: 4183 RVA: 0x000549AD File Offset: 0x00052BAD
		public void OnSolutionChange(EntityUid uid, TrashOnEmptyComponent component, SolutionChangedEvent args)
		{
			this.CheckSolutions(component);
		}

		// Token: 0x06001058 RID: 4184 RVA: 0x000549B8 File Offset: 0x00052BB8
		public void CheckSolutions(TrashOnEmptyComponent component)
		{
			if (!this.EntityManager.HasComponent<SolutionContainerManagerComponent>(component.Owner))
			{
				return;
			}
			Solution solution;
			if (this._solutionContainerSystem.TryGetSolution(component.Owner, component.Solution, out solution, null))
			{
				this.UpdateTags(component, solution);
			}
		}

		// Token: 0x06001059 RID: 4185 RVA: 0x00054A00 File Offset: 0x00052C00
		public void UpdateTags(TrashOnEmptyComponent component, Solution solution)
		{
			if (solution.Volume <= 0)
			{
				this._tagSystem.AddTag(component.Owner, "Trash");
				return;
			}
			if (this._tagSystem.HasTag(component.Owner, "Trash"))
			{
				this._tagSystem.RemoveTag(component.Owner, "Trash");
			}
		}

		// Token: 0x04000976 RID: 2422
		[Dependency]
		private readonly SolutionContainerSystem _solutionContainerSystem;

		// Token: 0x04000977 RID: 2423
		[Dependency]
		private readonly TagSystem _tagSystem;
	}
}
