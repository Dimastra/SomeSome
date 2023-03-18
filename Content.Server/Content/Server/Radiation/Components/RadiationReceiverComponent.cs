using System;
using Content.Server.Radiation.Systems;
using Robust.Shared.Analyzers;
using Robust.Shared.GameObjects;
using Robust.Shared.ViewVariables;

namespace Content.Server.Radiation.Components
{
	// Token: 0x02000269 RID: 617
	[RegisterComponent]
	[Access(new Type[]
	{
		typeof(RadiationSystem)
	})]
	public sealed class RadiationReceiverComponent : Component
	{
		// Token: 0x0400079F RID: 1951
		[ViewVariables]
		public float CurrentRadiation;
	}
}
