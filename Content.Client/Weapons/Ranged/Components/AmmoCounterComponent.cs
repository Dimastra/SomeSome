using System;
using System.Runtime.CompilerServices;
using Content.Shared.Weapons.Ranged.Components;
using Robust.Client.UserInterface;
using Robust.Shared.GameObjects;

namespace Content.Client.Weapons.Ranged.Components
{
	// Token: 0x02000036 RID: 54
	[RegisterComponent]
	public sealed class AmmoCounterComponent : SharedAmmoCounterComponent
	{
		// Token: 0x0400009B RID: 155
		[Nullable(2)]
		public Control Control;
	}
}
