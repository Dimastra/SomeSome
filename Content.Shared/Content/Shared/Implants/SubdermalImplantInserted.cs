using System;
using System.Runtime.CompilerServices;
using Content.Shared.Implants.Components;
using Robust.Shared.GameObjects;

namespace Content.Shared.Implants
{
	// Token: 0x020003ED RID: 1005
	[NullableContext(1)]
	[Nullable(0)]
	public class SubdermalImplantInserted
	{
		// Token: 0x06000BCD RID: 3021 RVA: 0x00026CC7 File Offset: 0x00024EC7
		public SubdermalImplantInserted(EntityUid entity, SubdermalImplantComponent component)
		{
			this.Entity = entity;
			this.Component = component;
		}

		// Token: 0x04000BBD RID: 3005
		public EntityUid Entity;

		// Token: 0x04000BBE RID: 3006
		public SubdermalImplantComponent Component;
	}
}
