using System;
using System.Runtime.CompilerServices;
using Content.Shared.Actions;
using Content.Shared.Polymorph;

namespace Content.Server.Polymorph.Systems
{
	// Token: 0x020002C5 RID: 709
	public sealed class PolymorphActionEvent : InstantActionEvent
	{
		// Token: 0x04000868 RID: 2152
		[Nullable(1)]
		public PolymorphPrototype Prototype;
	}
}
