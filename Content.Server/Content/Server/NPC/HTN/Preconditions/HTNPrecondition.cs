using System;
using System.Runtime.CompilerServices;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Server.NPC.HTN.Preconditions
{
	// Token: 0x02000362 RID: 866
	[NullableContext(1)]
	[Nullable(0)]
	[ImplicitDataDefinitionForInheritors]
	public abstract class HTNPrecondition
	{
		// Token: 0x060011F0 RID: 4592 RVA: 0x0005E593 File Offset: 0x0005C793
		public virtual void Initialize(IEntitySystemManager sysManager)
		{
			IoCManager.InjectDependencies<HTNPrecondition>(this);
		}

		// Token: 0x060011F1 RID: 4593
		public abstract bool IsMet(NPCBlackboard blackboard);
	}
}
