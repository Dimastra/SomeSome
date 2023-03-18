using System;
using System.Runtime.CompilerServices;
using Content.Server.UserInterface;
using Content.Shared.Communications;
using Robust.Server.GameObjects;
using Robust.Shared.Audio;
using Robust.Shared.GameObjects;
using Robust.Shared.Maths;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.ViewVariables;

namespace Content.Server.Communications
{
	// Token: 0x0200062F RID: 1583
	[NullableContext(1)]
	[Nullable(0)]
	[RegisterComponent]
	public sealed class CommunicationsConsoleComponent : SharedCommunicationsConsoleComponent
	{
		// Token: 0x17000526 RID: 1318
		// (get) Token: 0x060021B6 RID: 8630 RVA: 0x000AF98F File Offset: 0x000ADB8F
		[Nullable(2)]
		public BoundUserInterface UserInterface
		{
			[NullableContext(2)]
			get
			{
				return base.Owner.GetUIOrNull(CommunicationsConsoleUiKey.Key);
			}
		}

		// Token: 0x040014A3 RID: 5283
		public float UIUpdateAccumulator;

		// Token: 0x040014A4 RID: 5284
		[ViewVariables]
		public float AnnouncementCooldownRemaining;

		// Token: 0x040014A5 RID: 5285
		[ViewVariables]
		[DataField("title", false, 1, true, false, null)]
		public string AnnouncementDisplayName = "comms-console-announcement-title-station";

		// Token: 0x040014A6 RID: 5286
		[ViewVariables]
		[DataField("color", false, 1, false, false, null)]
		public Color AnnouncementColor = Color.Gold;

		// Token: 0x040014A7 RID: 5287
		[ViewVariables]
		[DataField("delay", false, 1, false, false, null)]
		public int DelayBetweenAnnouncements = 90;

		// Token: 0x040014A8 RID: 5288
		[ViewVariables]
		[DataField("canShuttle", false, 1, false, false, null)]
		public bool CanCallShuttle = true;

		// Token: 0x040014A9 RID: 5289
		[DataField("global", false, 1, false, false, null)]
		public bool AnnounceGlobal;

		// Token: 0x040014AA RID: 5290
		[DataField("sound", false, 1, false, false, null)]
		public SoundSpecifier AnnouncementSound = new SoundPathSpecifier("/Audio/Announcements/announce.ogg", null);
	}
}
