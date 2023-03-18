using System;
using System.Runtime.CompilerServices;
using Content.Server.Cuffs.Components;
using Content.Shared.Implants;
using Content.Shared.Implants.Components;
using Content.Shared.Interaction.Events;
using Content.Shared.Mobs;
using Robust.Shared.Containers;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;

namespace Content.Server.Implants
{
	// Token: 0x02000452 RID: 1106
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class SubdermalImplantSystem : SharedSubdermalImplantSystem
	{
		// Token: 0x06001652 RID: 5714 RVA: 0x00075CBC File Offset: 0x00073EBC
		public override void Initialize()
		{
			base.Initialize();
			base.SubscribeLocalEvent<SubdermalImplantComponent, UseFreedomImplantEvent>(new ComponentEventHandler<SubdermalImplantComponent, UseFreedomImplantEvent>(this.OnFreedomImplant), null, null);
			base.SubscribeLocalEvent<ImplantedComponent, MobStateChangedEvent>(new ComponentEventHandler<ImplantedComponent, MobStateChangedEvent>(this.RelayToImplantEvent<MobStateChangedEvent>), null, null);
			base.SubscribeLocalEvent<ImplantedComponent, SuicideEvent>(new ComponentEventHandler<ImplantedComponent, SuicideEvent>(this.RelayToImplantEvent<SuicideEvent>), null, null);
		}

		// Token: 0x06001653 RID: 5715 RVA: 0x00075D0C File Offset: 0x00073F0C
		private void OnFreedomImplant(EntityUid uid, SubdermalImplantComponent component, UseFreedomImplantEvent args)
		{
			CuffableComponent cuffs;
			if (!base.TryComp<CuffableComponent>(component.ImplantedEntity, ref cuffs) || cuffs.Container.ContainedEntities.Count < 1)
			{
				return;
			}
			HandcuffComponent cuff;
			if (base.TryComp<HandcuffComponent>(cuffs.LastAddedCuffs, ref cuff))
			{
				cuffs.Uncuff(component.ImplantedEntity.Value, cuffs.LastAddedCuffs, cuff, true);
			}
		}

		// Token: 0x06001654 RID: 5716 RVA: 0x00075D68 File Offset: 0x00073F68
		private void RelayToImplantEvent<T>(EntityUid uid, ImplantedComponent component, T args)
		{
			IContainer implantContainer;
			if (!this._container.TryGetContainer(uid, "implant", ref implantContainer, null))
			{
				return;
			}
			foreach (EntityUid implant in implantContainer.ContainedEntities)
			{
				base.RaiseLocalEvent<T>(implant, args, false);
			}
		}

		// Token: 0x06001655 RID: 5717 RVA: 0x00075DD0 File Offset: 0x00073FD0
		private void RelayToImplantEventByRef<T>(EntityUid uid, ImplantedComponent component, ref T args)
		{
			IContainer implantContainer;
			if (!this._container.TryGetContainer(uid, "implant", ref implantContainer, null))
			{
				return;
			}
			foreach (EntityUid implant in implantContainer.ContainedEntities)
			{
				base.RaiseLocalEvent<T>(implant, ref args, false);
			}
		}

		// Token: 0x06001656 RID: 5718 RVA: 0x00075E38 File Offset: 0x00074038
		private void RelayToImplantedEvent<[Nullable(0)] T>(EntityUid uid, SubdermalImplantComponent component, T args) where T : EntityEventArgs
		{
			if (component.ImplantedEntity != null)
			{
				base.RaiseLocalEvent<T>(component.ImplantedEntity.Value, args, false);
			}
		}

		// Token: 0x06001657 RID: 5719 RVA: 0x00075E5A File Offset: 0x0007405A
		private void RelayToImplantedEventByRef<[Nullable(0)] T>(EntityUid uid, SubdermalImplantComponent component, ref T args) where T : EntityEventArgs
		{
			if (component.ImplantedEntity != null)
			{
				base.RaiseLocalEvent<T>(component.ImplantedEntity.Value, ref args, false);
			}
		}

		// Token: 0x04000DF5 RID: 3573
		[Dependency]
		private readonly SharedContainerSystem _container;
	}
}
