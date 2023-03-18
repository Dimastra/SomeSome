using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Content.Shared.Clothing.Components;
using Content.Shared.Inventory.Events;
using Content.Shared.Tag;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Log;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization.Manager;

namespace Content.Shared.Clothing
{
	// Token: 0x020005AA RID: 1450
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class ClothingGrantingSystem : EntitySystem
	{
		// Token: 0x060011AD RID: 4525 RVA: 0x00039C88 File Offset: 0x00037E88
		public override void Initialize()
		{
			base.Initialize();
			base.SubscribeLocalEvent<ClothingGrantComponentComponent, GotEquippedEvent>(new ComponentEventHandler<ClothingGrantComponentComponent, GotEquippedEvent>(this.OnCompEquip), null, null);
			base.SubscribeLocalEvent<ClothingGrantComponentComponent, GotUnequippedEvent>(new ComponentEventHandler<ClothingGrantComponentComponent, GotUnequippedEvent>(this.OnCompUnequip), null, null);
			base.SubscribeLocalEvent<ClothingGrantTagComponent, GotEquippedEvent>(new ComponentEventHandler<ClothingGrantTagComponent, GotEquippedEvent>(this.OnTagEquip), null, null);
			base.SubscribeLocalEvent<ClothingGrantTagComponent, GotUnequippedEvent>(new ComponentEventHandler<ClothingGrantTagComponent, GotUnequippedEvent>(this.OnTagUnequip), null, null);
		}

		// Token: 0x060011AE RID: 4526 RVA: 0x00039CEC File Offset: 0x00037EEC
		private void OnCompEquip(EntityUid uid, ClothingGrantComponentComponent component, GotEquippedEvent args)
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
			if (component.Components.Count > 1)
			{
				Logger.Error("Although a component registry supports multiple components, we cannot bookkeep more than 1 component for ClothingGrantComponent at this time.");
				return;
			}
			foreach (KeyValuePair<string, EntityPrototype.ComponentRegistryEntry> keyValuePair in component.Components)
			{
				string text;
				EntityPrototype.ComponentRegistryEntry componentRegistryEntry;
				keyValuePair.Deconstruct(out text, out componentRegistryEntry);
				string name = text;
				EntityPrototype.ComponentRegistryEntry data = componentRegistryEntry;
				Component newComp = (Component)this._componentFactory.GetComponent(name, false);
				if (!base.HasComp(args.Equipee, newComp.GetType()))
				{
					newComp.Owner = args.Equipee;
					object temp = newComp;
					this._serializationManager.CopyTo(data.Component, ref temp, null, false, false);
					this.EntityManager.AddComponent<Component>(args.Equipee, (Component)temp, false);
					component.IsActive = true;
				}
			}
		}

		// Token: 0x060011AF RID: 4527 RVA: 0x00039E04 File Offset: 0x00038004
		private void OnCompUnequip(EntityUid uid, ClothingGrantComponentComponent component, GotUnequippedEvent args)
		{
			if (!component.IsActive)
			{
				return;
			}
			foreach (KeyValuePair<string, EntityPrototype.ComponentRegistryEntry> keyValuePair in component.Components)
			{
				string text;
				EntityPrototype.ComponentRegistryEntry componentRegistryEntry;
				keyValuePair.Deconstruct(out text, out componentRegistryEntry);
				string name = text;
				Component newComp = (Component)this._componentFactory.GetComponent(name, false);
				base.RemComp(args.Equipee, newComp.GetType());
			}
			component.IsActive = false;
		}

		// Token: 0x060011B0 RID: 4528 RVA: 0x00039E98 File Offset: 0x00038098
		private void OnTagEquip(EntityUid uid, ClothingGrantTagComponent component, GotEquippedEvent args)
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
			base.EnsureComp<TagComponent>(args.Equipee);
			this._tagSystem.AddTag(args.Equipee, component.Tag);
			component.IsActive = true;
		}

		// Token: 0x060011B1 RID: 4529 RVA: 0x00039EFB File Offset: 0x000380FB
		private void OnTagUnequip(EntityUid uid, ClothingGrantTagComponent component, GotUnequippedEvent args)
		{
			if (!component.IsActive)
			{
				return;
			}
			this._tagSystem.RemoveTag(args.Equipee, component.Tag);
			component.IsActive = false;
		}

		// Token: 0x04001054 RID: 4180
		[Dependency]
		private readonly IComponentFactory _componentFactory;

		// Token: 0x04001055 RID: 4181
		[Dependency]
		private readonly ISerializationManager _serializationManager;

		// Token: 0x04001056 RID: 4182
		[Dependency]
		private readonly TagSystem _tagSystem;
	}
}
