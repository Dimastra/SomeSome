using System;
using System.Runtime.CompilerServices;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;

namespace Content.Shared.Sandbox
{
	// Token: 0x020001DA RID: 474
	public abstract class SharedSandboxSystem : EntitySystem
	{
		// Token: 0x04000558 RID: 1368
		[Nullable(1)]
		[Dependency]
		protected readonly IPrototypeManager PrototypeManager;

		// Token: 0x020007AE RID: 1966
		[NetSerializable]
		[Serializable]
		protected sealed class MsgSandboxStatus : EntityEventArgs
		{
			// Token: 0x170004F4 RID: 1268
			// (get) Token: 0x060017FD RID: 6141 RVA: 0x0004D3C9 File Offset: 0x0004B5C9
			// (set) Token: 0x060017FE RID: 6142 RVA: 0x0004D3D1 File Offset: 0x0004B5D1
			public bool SandboxAllowed { get; set; }
		}

		// Token: 0x020007AF RID: 1967
		[NetSerializable]
		[Serializable]
		protected sealed class MsgSandboxRespawn : EntityEventArgs
		{
		}

		// Token: 0x020007B0 RID: 1968
		[NetSerializable]
		[Serializable]
		protected sealed class MsgSandboxGiveAccess : EntityEventArgs
		{
		}

		// Token: 0x020007B1 RID: 1969
		[NetSerializable]
		[Serializable]
		protected sealed class MsgSandboxGiveAghost : EntityEventArgs
		{
		}

		// Token: 0x020007B2 RID: 1970
		[NetSerializable]
		[Serializable]
		protected sealed class MsgSandboxSuicide : EntityEventArgs
		{
		}
	}
}
