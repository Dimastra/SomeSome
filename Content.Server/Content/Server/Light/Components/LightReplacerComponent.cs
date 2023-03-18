using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Content.Shared.Light.Component;
using Robust.Shared.Audio;
using Robust.Shared.Containers;
using Robust.Shared.GameObjects;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom.Prototype;
using Robust.Shared.ViewVariables;

namespace Content.Server.Light.Components
{
	// Token: 0x0200041B RID: 1051
	[NullableContext(1)]
	[Nullable(0)]
	[RegisterComponent]
	public sealed class LightReplacerComponent : Component
	{
		// Token: 0x04000D34 RID: 3380
		[DataField("sound", false, 1, false, false, null)]
		public SoundSpecifier Sound = new SoundPathSpecifier("/Audio/Weapons/click.ogg", null);

		// Token: 0x04000D35 RID: 3381
		[DataField("contents", false, 1, false, false, null)]
		public List<LightReplacerComponent.LightReplacerEntity> Contents = new List<LightReplacerComponent.LightReplacerEntity>();

		// Token: 0x04000D36 RID: 3382
		[ViewVariables]
		public IContainer InsertedBulbs;

		// Token: 0x020009C0 RID: 2496
		[NullableContext(0)]
		[DataDefinition]
		[Serializable]
		public sealed class LightReplacerEntity
		{
			// Token: 0x04002200 RID: 8704
			[Nullable(1)]
			[DataField("name", false, 1, false, false, typeof(PrototypeIdSerializer<EntityPrototype>))]
			public string PrototypeName;

			// Token: 0x04002201 RID: 8705
			[DataField("amount", false, 1, false, false, null)]
			public int Amount;

			// Token: 0x04002202 RID: 8706
			[DataField("type", false, 1, false, false, null)]
			public LightBulbType Type;
		}
	}
}
