using System;
using System.Runtime.CompilerServices;
using Content.Shared.Actions;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom.Prototype;

namespace Content.Server.Magic.Events
{
	// Token: 0x020003EA RID: 1002
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class ProjectileSpellEvent : WorldTargetActionEvent
	{
		// Token: 0x04000CB8 RID: 3256
		[DataField("prototype", false, 1, true, false, typeof(PrototypeIdSerializer<EntityPrototype>))]
		public string Prototype;

		// Token: 0x04000CB9 RID: 3257
		[DataField("posData", false, 1, false, false, null)]
		public MagicSpawnData Pos = new TargetCasterPos();
	}
}
