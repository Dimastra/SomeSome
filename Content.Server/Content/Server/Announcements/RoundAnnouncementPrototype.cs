using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Content.Server.GameTicking.Presets;
using Robust.Shared.Audio;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom.Prototype.List;

namespace Content.Server.Announcements
{
	// Token: 0x020007CE RID: 1998
	[NullableContext(1)]
	[Nullable(0)]
	[Prototype("roundAnnouncement", 1)]
	public sealed class RoundAnnouncementPrototype : IPrototype
	{
		// Token: 0x170006B5 RID: 1717
		// (get) Token: 0x06002B73 RID: 11123 RVA: 0x000E3E60 File Offset: 0x000E2060
		[IdDataField(1, null)]
		public string ID { get; }

		// Token: 0x04001AE3 RID: 6883
		[Nullable(2)]
		[DataField("sound", false, 1, false, false, null)]
		public SoundSpecifier Sound;

		// Token: 0x04001AE4 RID: 6884
		[Nullable(2)]
		[DataField("message", false, 1, false, false, null)]
		public string Message;

		// Token: 0x04001AE5 RID: 6885
		[DataField("presets", false, 1, false, false, typeof(PrototypeIdListSerializer<GamePresetPrototype>))]
		public List<string> GamePresets = new List<string>();
	}
}
