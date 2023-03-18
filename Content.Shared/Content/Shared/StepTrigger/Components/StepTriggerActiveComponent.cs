using System;
using Content.Shared.StepTrigger.Systems;
using Robust.Shared.Analyzers;
using Robust.Shared.GameObjects;

namespace Content.Shared.StepTrigger.Components
{
	// Token: 0x0200014F RID: 335
	[RegisterComponent]
	[Access(new Type[]
	{
		typeof(StepTriggerSystem)
	})]
	public sealed class StepTriggerActiveComponent : Component
	{
	}
}
