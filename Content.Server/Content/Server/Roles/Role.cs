using System;
using System.Runtime.CompilerServices;
using Content.Server.Mind;
using Robust.Shared.ViewVariables;

namespace Content.Server.Roles
{
	// Token: 0x0200022C RID: 556
	[NullableContext(1)]
	[Nullable(0)]
	public abstract class Role
	{
		// Token: 0x170001BE RID: 446
		// (get) Token: 0x06000B1C RID: 2844 RVA: 0x0003A2B6 File Offset: 0x000384B6
		[ViewVariables]
		public Mind Mind { get; }

		// Token: 0x170001BF RID: 447
		// (get) Token: 0x06000B1D RID: 2845
		[ViewVariables]
		public abstract string Name { get; }

		// Token: 0x170001C0 RID: 448
		// (get) Token: 0x06000B1E RID: 2846
		[ViewVariables]
		public abstract bool Antagonist { get; }

		// Token: 0x06000B1F RID: 2847 RVA: 0x0003A2BE File Offset: 0x000384BE
		protected Role(Mind mind)
		{
			this.Mind = mind;
		}

		// Token: 0x06000B20 RID: 2848 RVA: 0x0003A2CD File Offset: 0x000384CD
		public virtual void Greet()
		{
		}
	}
}
