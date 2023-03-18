using System;
using System.Runtime.CompilerServices;
using Content.Shared.Implants.Components;
using Robust.Shared.GameObjects;

namespace Content.Shared.Implants
{
	// Token: 0x020003EE RID: 1006
	[NullableContext(1)]
	[Nullable(0)]
	public class SubdermalImplantRemoved
	{
		// Token: 0x06000BCE RID: 3022 RVA: 0x00026CDD File Offset: 0x00024EDD
		public SubdermalImplantRemoved(EntityUid entity, SubdermalImplantComponent component)
		{
			this.Entity = entity;
			this.Component = component;
		}

		// Token: 0x04000BBF RID: 3007
		public EntityUid Entity;

		// Token: 0x04000BC0 RID: 3008
		public SubdermalImplantComponent Component;
	}
}
