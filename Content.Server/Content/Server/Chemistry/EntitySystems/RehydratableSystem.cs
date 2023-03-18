using System;
using System.Runtime.CompilerServices;
using Content.Server.Chemistry.Components;
using Content.Server.Popups;
using Content.Shared.FixedPoint;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Localization;

namespace Content.Server.Chemistry.EntitySystems
{
	// Token: 0x02000698 RID: 1688
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class RehydratableSystem : EntitySystem
	{
		// Token: 0x06002320 RID: 8992 RVA: 0x000B7860 File Offset: 0x000B5A60
		public override void Initialize()
		{
			base.Initialize();
			base.SubscribeLocalEvent<RehydratableComponent, SolutionChangedEvent>(new ComponentEventHandler<RehydratableComponent, SolutionChangedEvent>(this.OnSolutionChange), null, null);
		}

		// Token: 0x06002321 RID: 8993 RVA: 0x000B787C File Offset: 0x000B5A7C
		private void OnSolutionChange(EntityUid uid, RehydratableComponent component, SolutionChangedEvent args)
		{
			if (this._solutionsSystem.GetReagentQuantity(uid, component.CatalystPrototype) > FixedPoint2.Zero)
			{
				this.Expand(component, component.Owner);
			}
		}

		// Token: 0x06002322 RID: 8994 RVA: 0x000B78AC File Offset: 0x000B5AAC
		private void Expand(RehydratableComponent component, EntityUid owner)
		{
			if (component.Expanding)
			{
				return;
			}
			component.Expanding = true;
			owner.PopupMessageEveryone(Loc.GetString("rehydratable-component-expands-message", new ValueTuple<string, object>[]
			{
				new ValueTuple<string, object>("owner", owner)
			}), null, 15);
			if (!string.IsNullOrEmpty(component.TargetPrototype))
			{
				EntityUid ent = this.EntityManager.SpawnEntity(component.TargetPrototype, this.EntityManager.GetComponent<TransformComponent>(owner).Coordinates);
				this.EntityManager.GetComponent<TransformComponent>(ent).AttachToGridOrMap();
			}
			this.EntityManager.QueueDeleteEntity(owner);
		}

		// Token: 0x040015B8 RID: 5560
		[Dependency]
		private readonly SolutionContainerSystem _solutionsSystem;
	}
}
