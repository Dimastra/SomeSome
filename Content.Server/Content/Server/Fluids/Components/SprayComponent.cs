using System;
using System.Runtime.CompilerServices;
using Content.Server.Fluids.EntitySystems;
using Content.Shared.FixedPoint;
using Robust.Shared.Analyzers;
using Robust.Shared.Audio;
using Robust.Shared.GameObjects;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom.Prototype;

namespace Content.Server.Fluids.Components
{
	// Token: 0x020004F9 RID: 1273
	[NullableContext(1)]
	[Nullable(0)]
	[RegisterComponent]
	[Access(new Type[]
	{
		typeof(SpraySystem)
	})]
	public sealed class SprayComponent : Component
	{
		// Token: 0x170003E1 RID: 993
		// (get) Token: 0x06001A30 RID: 6704 RVA: 0x0008A1A3 File Offset: 0x000883A3
		[DataField("spraySound", false, 1, true, false, null)]
		[Access]
		public SoundSpecifier SpraySound { get; }

		// Token: 0x0400108E RID: 4238
		public const string SolutionName = "spray";

		// Token: 0x0400108F RID: 4239
		[DataField("sprayDistance", false, 1, false, false, null)]
		public float SprayDistance = 3f;

		// Token: 0x04001090 RID: 4240
		[DataField("transferAmount", false, 1, false, false, null)]
		public FixedPoint2 TransferAmount = FixedPoint2.New(10);

		// Token: 0x04001091 RID: 4241
		[DataField("sprayVelocity", false, 1, false, false, null)]
		public float SprayVelocity = 1.5f;

		// Token: 0x04001092 RID: 4242
		[DataField("sprayAliveTime", false, 1, false, false, null)]
		public float SprayAliveTime = 0.75f;

		// Token: 0x04001093 RID: 4243
		[DataField("cooldownTime", false, 1, false, false, null)]
		public float CooldownTime = 0.5f;

		// Token: 0x04001094 RID: 4244
		[DataField("sprayedPrototype", false, 1, false, false, typeof(PrototypeIdSerializer<EntityPrototype>))]
		public string SprayedPrototype = "Vapor";

		// Token: 0x04001095 RID: 4245
		[DataField("vaporAmount", false, 1, false, false, null)]
		public int VaporAmount = 1;

		// Token: 0x04001096 RID: 4246
		[DataField("vaporSpread", false, 1, false, false, null)]
		public float VaporSpread = 90f;

		// Token: 0x04001097 RID: 4247
		[DataField("impulse", false, 1, false, false, null)]
		public float Impulse;
	}
}
