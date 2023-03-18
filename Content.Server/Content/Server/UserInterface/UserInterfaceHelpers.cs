using System;
using System.Runtime.CompilerServices;
using Robust.Server.GameObjects;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;

namespace Content.Server.UserInterface
{
	// Token: 0x02000101 RID: 257
	public static class UserInterfaceHelpers
	{
		// Token: 0x060004B0 RID: 1200 RVA: 0x00016778 File Offset: 0x00014978
		[NullableContext(1)]
		[Obsolete("Use UserInterfaceSystem")]
		[return: Nullable(2)]
		public static BoundUserInterface GetUIOrNull(this EntityUid entity, Enum uiKey)
		{
			return IoCManager.Resolve<IEntitySystemManager>().GetEntitySystem<UserInterfaceSystem>().GetUiOrNull(entity, uiKey, null);
		}
	}
}
