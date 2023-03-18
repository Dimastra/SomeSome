using System;
using System.Runtime.CompilerServices;
using Content.Shared.Actions;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom.Prototype;

namespace Content.Server.Magic.Events
{
	// Token: 0x020003E5 RID: 997
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class InstantSpawnSpellEvent : InstantActionEvent
	{
		// Token: 0x04000CB1 RID: 3249
		[DataField("prototype", false, 1, true, false, typeof(PrototypeIdSerializer<EntityPrototype>))]
		public string Prototype;

		// Token: 0x04000CB2 RID: 3250
		[DataField("preventCollide", false, 1, false, false, null)]
		public bool PreventCollideWithCaster = true;

		// Token: 0x04000CB3 RID: 3251
		[DataField("posData", false, 1, false, false, null)]
		public MagicSpawnData Pos = new TargetCasterPos();
	}
}
