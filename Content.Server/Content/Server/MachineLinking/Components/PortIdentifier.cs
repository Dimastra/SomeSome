using System;
using System.Runtime.CompilerServices;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Server.MachineLinking.Components
{
	// Token: 0x02000404 RID: 1028
	[NullableContext(1)]
	[Nullable(0)]
	[DataDefinition]
	public struct PortIdentifier
	{
		// Token: 0x060014D7 RID: 5335 RVA: 0x0006D1A0 File Offset: 0x0006B3A0
		public PortIdentifier(EntityUid uid, string port)
		{
			this.Uid = uid;
			this.Port = port;
		}

		// Token: 0x04000CF2 RID: 3314
		[DataField("uid", false, 1, false, false, null)]
		public EntityUid Uid;

		// Token: 0x04000CF3 RID: 3315
		[DataField("port", false, 1, false, false, null)]
		public string Port;
	}
}
