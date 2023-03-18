using System;
using System.Runtime.CompilerServices;
using Content.Server.Speech.Components;
using Content.Shared.Clothing.Components;
using Content.Shared.Inventory.Events;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;

namespace Content.Server.Speech.EntitySystems
{
	// Token: 0x020001B2 RID: 434
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class AddAccentClothingSystem : EntitySystem
	{
		// Token: 0x06000882 RID: 2178 RVA: 0x0002B683 File Offset: 0x00029883
		public override void Initialize()
		{
			base.Initialize();
			base.SubscribeLocalEvent<AddAccentClothingComponent, GotEquippedEvent>(new ComponentEventHandler<AddAccentClothingComponent, GotEquippedEvent>(this.OnGotEquipped), null, null);
			base.SubscribeLocalEvent<AddAccentClothingComponent, GotUnequippedEvent>(new ComponentEventHandler<AddAccentClothingComponent, GotUnequippedEvent>(this.OnGotUnequipped), null, null);
		}

		// Token: 0x06000883 RID: 2179 RVA: 0x0002B6B4 File Offset: 0x000298B4
		private void OnGotEquipped(EntityUid uid, AddAccentClothingComponent component, GotEquippedEvent args)
		{
			ClothingComponent clothing;
			if (!base.TryComp<ClothingComponent>(uid, ref clothing))
			{
				return;
			}
			if (!clothing.Slots.HasFlag(args.SlotFlags))
			{
				return;
			}
			Type componentType = this._componentFactory.GetRegistration(component.Accent, false).Type;
			if (this.EntityManager.HasComponent(args.Equipee, componentType))
			{
				return;
			}
			Component accentComponent = (Component)this._componentFactory.GetComponent(componentType);
			accentComponent.Owner = args.Equipee;
			this.EntityManager.AddComponent<Component>(args.Equipee, accentComponent, false);
			ReplacementAccentComponent rep = accentComponent as ReplacementAccentComponent;
			if (rep != null)
			{
				rep.Accent = component.ReplacementPrototype;
			}
			component.IsActive = true;
		}

		// Token: 0x06000884 RID: 2180 RVA: 0x0002B768 File Offset: 0x00029968
		private void OnGotUnequipped(EntityUid uid, AddAccentClothingComponent component, GotUnequippedEvent args)
		{
			if (!component.IsActive)
			{
				return;
			}
			Type componentType = this._componentFactory.GetRegistration(component.Accent, false).Type;
			if (this.EntityManager.HasComponent(args.Equipee, componentType))
			{
				this.EntityManager.RemoveComponent(args.Equipee, componentType);
			}
			component.IsActive = false;
		}

		// Token: 0x04000535 RID: 1333
		[Dependency]
		private readonly IComponentFactory _componentFactory;
	}
}
