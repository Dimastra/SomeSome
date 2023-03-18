using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Content.Shared.Roles;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Server.Jobs
{
	// Token: 0x0200043A RID: 1082
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class AddComponentSpecial : JobSpecial
	{
		// Token: 0x170002F1 RID: 753
		// (get) Token: 0x060015EF RID: 5615 RVA: 0x00074151 File Offset: 0x00072351
		[DataField("components", false, 1, false, false, null)]
		[AlwaysPushInheritance]
		public EntityPrototype.ComponentRegistry Components { get; } = new EntityPrototype.ComponentRegistry();

		// Token: 0x060015F0 RID: 5616 RVA: 0x0007415C File Offset: 0x0007235C
		public override void AfterEquip(EntityUid mob)
		{
			IComponentFactory factory = IoCManager.Resolve<IComponentFactory>();
			IEntityManager entityManager = IoCManager.Resolve<IEntityManager>();
			ISerializationManager serializationManager = IoCManager.Resolve<ISerializationManager>();
			foreach (KeyValuePair<string, EntityPrototype.ComponentRegistryEntry> keyValuePair in this.Components)
			{
				string text;
				EntityPrototype.ComponentRegistryEntry componentRegistryEntry;
				keyValuePair.Deconstruct(out text, out componentRegistryEntry);
				string name = text;
				EntityPrototype.ComponentRegistryEntry data = componentRegistryEntry;
				Component component = (Component)factory.GetComponent(name, false);
				component.Owner = mob;
				object temp = component;
				serializationManager.CopyTo(data.Component, ref temp, null, false, false);
				entityManager.AddComponent<Component>(mob, (Component)temp, false);
			}
		}
	}
}
