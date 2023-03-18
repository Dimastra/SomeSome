using System;
using System.Runtime.CompilerServices;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Prototypes;

namespace Content.Shared.Prototypes
{
	// Token: 0x02000240 RID: 576
	[NullableContext(1)]
	[Nullable(0)]
	public static class EntityPrototypeHelpers
	{
		// Token: 0x06000680 RID: 1664 RVA: 0x00017231 File Offset: 0x00015431
		public static bool HasComponent<[Nullable(0)] T>(this EntityPrototype prototype, [Nullable(2)] IComponentFactory componentFactory = null) where T : IComponent
		{
			return prototype.HasComponent(typeof(T), componentFactory);
		}

		// Token: 0x06000681 RID: 1665 RVA: 0x00017244 File Offset: 0x00015444
		public static bool HasComponent(this EntityPrototype prototype, Type component, [Nullable(2)] IComponentFactory componentFactory = null)
		{
			if (componentFactory == null)
			{
				componentFactory = IoCManager.Resolve<IComponentFactory>();
			}
			ComponentRegistration registration = componentFactory.GetRegistration(component);
			return prototype.Components.ContainsKey(registration.Name);
		}

		// Token: 0x06000682 RID: 1666 RVA: 0x00017274 File Offset: 0x00015474
		public static bool HasComponent<[Nullable(0)] T>(string prototype, [Nullable(2)] IPrototypeManager prototypeManager = null, [Nullable(2)] IComponentFactory componentFactory = null) where T : IComponent
		{
			return EntityPrototypeHelpers.HasComponent(prototype, typeof(T), prototypeManager, componentFactory);
		}

		// Token: 0x06000683 RID: 1667 RVA: 0x00017288 File Offset: 0x00015488
		public static bool HasComponent(string prototype, Type component, [Nullable(2)] IPrototypeManager prototypeManager = null, [Nullable(2)] IComponentFactory componentFactory = null)
		{
			if (prototypeManager == null)
			{
				prototypeManager = IoCManager.Resolve<IPrototypeManager>();
			}
			EntityPrototype proto;
			return prototypeManager.TryIndex<EntityPrototype>(prototype, ref proto) && proto.HasComponent(component, componentFactory);
		}
	}
}
