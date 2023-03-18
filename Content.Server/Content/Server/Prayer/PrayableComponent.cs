using System;
using System.Runtime.CompilerServices;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Utility;
using Robust.Shared.ViewVariables;

namespace Content.Server.Prayer
{
	// Token: 0x0200026F RID: 623
	[NullableContext(1)]
	[Nullable(0)]
	[RegisterComponent]
	public sealed class PrayableComponent : Component
	{
		// Token: 0x040007AC RID: 1964
		[DataField("bibleUserOnly", false, 1, false, false, null)]
		[ViewVariables]
		public bool BibleUserOnly;

		// Token: 0x040007AD RID: 1965
		[DataField("sentMessage", false, 1, false, false, null)]
		[ViewVariables]
		public string SentMessage = "prayer-popup-notify-pray-sent";

		// Token: 0x040007AE RID: 1966
		[DataField("notifiactionPrefix", false, 1, false, false, null)]
		[ViewVariables]
		public string NotifiactionPrefix = "prayer-chat-notify-pray";

		// Token: 0x040007AF RID: 1967
		[DataField("verb", false, 1, false, false, null)]
		[ViewVariables]
		public string Verb = "prayer-verbs-pray";

		// Token: 0x040007B0 RID: 1968
		[Nullable(2)]
		[DataField("verbImage", false, 1, false, false, null)]
		[ViewVariables]
		public SpriteSpecifier VerbImage = new SpriteSpecifier.Texture(new ResourcePath("/Textures/Interface/pray.svg.png", "/"));
	}
}
