using System;
using System.Runtime.CompilerServices;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Server.Tabletop
{
	// Token: 0x02000131 RID: 305
	[ImplicitDataDefinitionForInheritors]
	public abstract class TabletopSetup
	{
		// Token: 0x06000582 RID: 1410
		[NullableContext(1)]
		public abstract void SetupTabletop(TabletopSession session, IEntityManager entityManager);
	}
}
