using System;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom;
using Robust.Shared.ViewVariables;

namespace Content.Server.Atmos.Miasma
{
	// Token: 0x0200078C RID: 1932
	[RegisterComponent]
	public sealed class PerishableComponent : Component
	{
		// Token: 0x0400198F RID: 6543
		[ViewVariables]
		public bool Progressing = true;

		// Token: 0x04001990 RID: 6544
		[DataField("timeOfDeath", false, 1, false, false, typeof(TimeOffsetSerializer))]
		[ViewVariables]
		public TimeSpan TimeOfDeath = TimeSpan.Zero;

		// Token: 0x04001991 RID: 6545
		public TimeSpan RotAfter = TimeSpan.FromMinutes(5.0);

		// Token: 0x04001992 RID: 6546
		[DataField("rotNextUpdate", false, 1, false, false, typeof(TimeOffsetSerializer))]
		public TimeSpan RotNextUpdate = TimeSpan.Zero;

		// Token: 0x04001993 RID: 6547
		[ViewVariables]
		[DataField("molsPerSecondPerUnitMass", false, 1, false, false, null)]
		public float MolsPerSecondPerUnitMass = 0.0025f;
	}
}
