using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Content.Client.Hands.Systems;
using Content.Shared.Hands.Components;
using Robust.Shared.Analyzers;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Client.Hands
{
	// Token: 0x020002DB RID: 731
	[RegisterComponent]
	[ComponentReference(typeof(SharedHandsComponent))]
	[Access(new Type[]
	{
		typeof(HandsSystem)
	})]
	public sealed class HandsComponent : SharedHandsComponent
	{
		// Token: 0x04000921 RID: 2337
		[DataField("showInHands", false, 1, false, false, null)]
		public bool ShowInHands = true;

		// Token: 0x04000922 RID: 2338
		[Nullable(1)]
		public readonly Dictionary<HandLocation, HashSet<string>> RevealedLayers = new Dictionary<HandLocation, HashSet<string>>();
	}
}
