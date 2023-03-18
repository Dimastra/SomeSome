using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Content.Shared.Radio;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom.Prototype.Set;

namespace Content.Server.Radio.Components
{
	// Token: 0x0200025D RID: 605
	[RegisterComponent]
	public sealed class ActiveRadioComponent : Component
	{
		// Token: 0x04000784 RID: 1924
		[Nullable(1)]
		[DataField("channels", false, 1, false, false, typeof(PrototypeIdHashSetSerializer<RadioChannelPrototype>))]
		public HashSet<string> Channels = new HashSet<string>();
	}
}
