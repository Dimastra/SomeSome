using System;
using Content.Shared.Hands.Components;
using Robust.Shared.GameObjects;

namespace Content.Server.Hands.Components
{
	// Token: 0x0200047A RID: 1146
	[RegisterComponent]
	[ComponentReference(typeof(SharedHandsComponent))]
	public sealed class HandsComponent : SharedHandsComponent
	{
	}
}
