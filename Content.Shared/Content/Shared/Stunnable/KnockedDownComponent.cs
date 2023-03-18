using System;
using System.Runtime.CompilerServices;
using Robust.Shared.Analyzers;
using Robust.Shared.Audio;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.ViewVariables;

namespace Content.Shared.Stunnable
{
	// Token: 0x0200010C RID: 268
	[RegisterComponent]
	[NetworkedComponent]
	[Access(new Type[]
	{
		typeof(SharedStunSystem)
	})]
	public sealed class KnockedDownComponent : Component
	{
		// Token: 0x17000092 RID: 146
		// (get) Token: 0x060002FD RID: 765 RVA: 0x0000D7EF File Offset: 0x0000B9EF
		// (set) Token: 0x060002FE RID: 766 RVA: 0x0000D7F7 File Offset: 0x0000B9F7
		[DataField("helpInterval", false, 1, false, false, null)]
		public float HelpInterval { get; set; } = 1f;

		// Token: 0x17000093 RID: 147
		// (get) Token: 0x060002FF RID: 767 RVA: 0x0000D800 File Offset: 0x0000BA00
		// (set) Token: 0x06000300 RID: 768 RVA: 0x0000D808 File Offset: 0x0000BA08
		[ViewVariables]
		public float HelpTimer { get; set; }

		// Token: 0x0400034A RID: 842
		[Nullable(1)]
		[DataField("helpAttemptSound", false, 1, false, false, null)]
		public SoundSpecifier StunAttemptSound = new SoundPathSpecifier("/Audio/Effects/thudswoosh.ogg", null);
	}
}
