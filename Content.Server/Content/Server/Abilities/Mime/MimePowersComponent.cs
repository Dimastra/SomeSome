using System;
using System.Runtime.CompilerServices;
using Content.Shared.Actions.ActionTypes;
using Robust.Shared.GameObjects;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom.Prototype;
using Robust.Shared.Utility;

namespace Content.Server.Abilities.Mime
{
	// Token: 0x02000883 RID: 2179
	[NullableContext(1)]
	[Nullable(0)]
	[RegisterComponent]
	public sealed class MimePowersComponent : Component
	{
		// Token: 0x04001C95 RID: 7317
		[DataField("enabled", false, 1, false, false, null)]
		public bool Enabled = true;

		// Token: 0x04001C96 RID: 7318
		[DataField("wallPrototype", false, 1, false, false, typeof(PrototypeIdSerializer<EntityPrototype>))]
		public string WallPrototype = "WallInvisible";

		// Token: 0x04001C97 RID: 7319
		[DataField("invisibleWallAction", false, 1, false, false, null)]
		public InstantAction InvisibleWallAction = new InstantAction
		{
			UseDelay = new TimeSpan?(TimeSpan.FromSeconds(30.0)),
			Icon = new SpriteSpecifier.Texture(new ResourcePath("Structures/Walls/solid.rsi/full.png", "/")),
			DisplayName = "mime-invisible-wall",
			Description = "mime-invisible-wall-desc",
			Priority = -1,
			Event = new InvisibleWallActionEvent()
		};

		// Token: 0x04001C98 RID: 7320
		public bool VowBroken;

		// Token: 0x04001C99 RID: 7321
		public bool ReadyToRepent;

		// Token: 0x04001C9A RID: 7322
		[DataField("vowRepentTime", false, 1, false, false, typeof(TimeOffsetSerializer))]
		public TimeSpan VowRepentTime = TimeSpan.Zero;

		// Token: 0x04001C9B RID: 7323
		[DataField("vowCooldown", false, 1, false, false, typeof(TimeOffsetSerializer))]
		public TimeSpan VowCooldown = TimeSpan.FromMinutes(5.0);
	}
}
