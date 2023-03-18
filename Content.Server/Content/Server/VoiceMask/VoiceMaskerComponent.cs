using System;
using System.Runtime.CompilerServices;
using Content.Shared.Actions.ActionTypes;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom.Prototype;
using Robust.Shared.ViewVariables;

namespace Content.Server.VoiceMask
{
	// Token: 0x020000CB RID: 203
	[NullableContext(1)]
	[Nullable(0)]
	[RegisterComponent]
	public sealed class VoiceMaskerComponent : Component
	{
		// Token: 0x0400023A RID: 570
		[ViewVariables]
		public string LastSetName = "Unknown";

		// Token: 0x0400023B RID: 571
		[Nullable(2)]
		[ViewVariables]
		public string LastSetVoice;

		// Token: 0x0400023C RID: 572
		[DataField("action", false, 1, false, false, typeof(PrototypeIdSerializer<InstantActionPrototype>))]
		public string Action = "ChangeVoiceMask";
	}
}
