using System;
using System.Linq;
using System.Runtime.CompilerServices;
using Content.Shared.Containers.ItemSlots;
using Content.Shared.IdentityManagement;
using Content.Shared.Implants.Components;
using Content.Shared.Popups;
using Robust.Shared.Containers;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Localization;

namespace Content.Shared.Implants
{
	// Token: 0x020003EF RID: 1007
	[NullableContext(1)]
	[Nullable(0)]
	public abstract class SharedImplanterSystem : EntitySystem
	{
		// Token: 0x06000BCF RID: 3023 RVA: 0x00026CF3 File Offset: 0x00024EF3
		public override void Initialize()
		{
			base.Initialize();
			base.SubscribeLocalEvent<ImplanterComponent, ComponentInit>(new ComponentEventHandler<ImplanterComponent, ComponentInit>(this.OnImplanterInit), null, null);
			base.SubscribeLocalEvent<ImplanterComponent, EntInsertedIntoContainerMessage>(new ComponentEventHandler<ImplanterComponent, EntInsertedIntoContainerMessage>(this.OnEntInserted), null, null);
		}

		// Token: 0x06000BD0 RID: 3024 RVA: 0x00026D23 File Offset: 0x00024F23
		private void OnImplanterInit(EntityUid uid, ImplanterComponent component, ComponentInit args)
		{
			if (component.Implant != null)
			{
				component.ImplanterSlot.StartingItem = component.Implant;
			}
			this._itemSlots.AddItemSlot(uid, "implanter_slot", component.ImplanterSlot, null);
		}

		// Token: 0x06000BD1 RID: 3025 RVA: 0x00026D58 File Offset: 0x00024F58
		private void OnEntInserted(EntityUid uid, ImplanterComponent component, EntInsertedIntoContainerMessage args)
		{
			MetaDataComponent implantData = this.EntityManager.GetComponent<MetaDataComponent>(args.Entity);
			component.ImplantData = new ValueTuple<string, string>(implantData.EntityName, implantData.EntityDescription);
		}

		// Token: 0x06000BD2 RID: 3026 RVA: 0x00026D90 File Offset: 0x00024F90
		public void Implant(EntityUid implanter, EntityUid target, ImplanterComponent component)
		{
			ContainerSlot implanterContainer = component.ImplanterSlot.ContainerSlot;
			if (implanterContainer == null)
			{
				return;
			}
			EntityUid implant = implanterContainer.ContainedEntities.FirstOrDefault<EntityUid>();
			SubdermalImplantComponent implantComp;
			if (!base.TryComp<SubdermalImplantComponent>(implant, ref implantComp))
			{
				return;
			}
			ImplantedComponent implantedComponent = base.EnsureComp<ImplantedComponent>(target);
			Container implantContainer = implantedComponent.ImplantContainer;
			if (implantedComponent.ImplantContainer.ContainedEntities.Any((EntityUid x) => this.MetaData(x).EntityName == this.MetaData(implant).EntityName))
			{
				return;
			}
			implanterContainer.Remove(implant, null, null, null, true, false, null, null);
			implantComp.ImplantedEntity = new EntityUid?(target);
			implantContainer.OccludesLight = false;
			implantContainer.Insert(implant, null, null, null, null, null);
			base.RaiseLocalEvent<SubdermalImplantInserted>(new SubdermalImplantInserted(implantComp.ImplantedEntity.Value, implantComp));
			if (component.CurrentMode == ImplanterToggleMode.Inject && !component.ImplantOnly)
			{
				this.DrawMode(component);
			}
			else
			{
				this.ImplantMode(component);
			}
			base.Dirty(component, null);
		}

		// Token: 0x06000BD3 RID: 3027 RVA: 0x00026E94 File Offset: 0x00025094
		public void Draw(EntityUid implanter, EntityUid user, EntityUid target, ImplanterComponent component)
		{
			ContainerSlot implanterContainer = component.ImplanterSlot.ContainerSlot;
			if (implanterContainer == null)
			{
				return;
			}
			bool permanentFound = false;
			IContainer implantContainer;
			if (this._container.TryGetContainer(target, "implant", ref implantContainer, null))
			{
				EntityQuery<SubdermalImplantComponent> implantCompQuery = base.GetEntityQuery<SubdermalImplantComponent>();
				foreach (EntityUid implant in implantContainer.ContainedEntities)
				{
					SubdermalImplantComponent implantComp;
					if (!implantCompQuery.TryGetComponent(implant, ref implantComp))
					{
						return;
					}
					if (implantContainer.CanRemove(implant, null))
					{
						implantContainer.Remove(implant, null, null, null, true, false, null, null);
						base.RaiseLocalEvent<SubdermalImplantRemoved>(new SubdermalImplantRemoved(implantComp.ImplantedEntity.Value, implantComp));
						implantComp.ImplantedEntity = null;
						implanterContainer.Insert(implant, null, null, null, null, null);
						permanentFound = implantComp.Permanent;
						break;
					}
					EntityUid implantName = Identity.Entity(implant, this.EntityManager);
					EntityUid targetName = Identity.Entity(target, this.EntityManager);
					string failedPermanentMessage = Loc.GetString("implanter-draw-failed-permanent", new ValueTuple<string, object>[]
					{
						new ValueTuple<string, object>("implant", implantName),
						new ValueTuple<string, object>("target", targetName)
					});
					this._popup.PopupEntity(failedPermanentMessage, target, user, PopupType.Small);
					permanentFound = implantComp.Permanent;
				}
				if (component.CurrentMode == ImplanterToggleMode.Draw && !component.ImplantOnly && !permanentFound)
				{
					this.ImplantMode(component);
				}
				base.Dirty(component, null);
			}
		}

		// Token: 0x06000BD4 RID: 3028 RVA: 0x00027044 File Offset: 0x00025244
		private void ImplantMode(ImplanterComponent component)
		{
			component.CurrentMode = ImplanterToggleMode.Inject;
			this.ChangeOnImplantVisualizer(component);
		}

		// Token: 0x06000BD5 RID: 3029 RVA: 0x00027054 File Offset: 0x00025254
		private void DrawMode(ImplanterComponent component)
		{
			component.CurrentMode = ImplanterToggleMode.Draw;
			this.ChangeOnImplantVisualizer(component);
		}

		// Token: 0x06000BD6 RID: 3030 RVA: 0x00027064 File Offset: 0x00025264
		private void ChangeOnImplantVisualizer(ImplanterComponent component)
		{
			AppearanceComponent appearance;
			if (!base.TryComp<AppearanceComponent>(component.Owner, ref appearance))
			{
				return;
			}
			bool implantFound = component.ImplanterSlot.HasItem;
			if (component.CurrentMode == ImplanterToggleMode.Inject && !component.ImplantOnly)
			{
				this._appearance.SetData(component.Owner, ImplanterVisuals.Full, implantFound, appearance);
				return;
			}
			if (component.CurrentMode == ImplanterToggleMode.Inject && component.ImplantOnly)
			{
				this._appearance.SetData(component.Owner, ImplanterVisuals.Full, implantFound, appearance);
				this._appearance.SetData(component.Owner, ImplanterImplantOnlyVisuals.ImplantOnly, component.ImplantOnly, appearance);
				return;
			}
			this._appearance.SetData(component.Owner, ImplanterVisuals.Full, implantFound, appearance);
		}

		// Token: 0x04000BC1 RID: 3009
		[Dependency]
		private readonly SharedContainerSystem _container;

		// Token: 0x04000BC2 RID: 3010
		[Dependency]
		private readonly ItemSlotsSystem _itemSlots;

		// Token: 0x04000BC3 RID: 3011
		[Dependency]
		private readonly SharedAppearanceSystem _appearance;

		// Token: 0x04000BC4 RID: 3012
		[Dependency]
		private readonly SharedPopupSystem _popup;
	}
}
