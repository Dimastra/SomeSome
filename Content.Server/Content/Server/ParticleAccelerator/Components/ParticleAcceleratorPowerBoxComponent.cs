using System;
using System.Runtime.CompilerServices;
using Content.Server.Power.Components;
using Robust.Shared.GameObjects;
using Robust.Shared.ViewVariables;

namespace Content.Server.ParticleAccelerator.Components
{
	// Token: 0x020002EA RID: 746
	[RegisterComponent]
	[ComponentReference(typeof(ParticleAcceleratorPartComponent))]
	public sealed class ParticleAcceleratorPowerBoxComponent : ParticleAcceleratorPartComponent
	{
		// Token: 0x06000F51 RID: 3921 RVA: 0x0004E82C File Offset: 0x0004CA2C
		protected override void Initialize()
		{
			base.Initialize();
			this.PowerConsumerComponent = ComponentExt.EnsureComponentWarn<PowerConsumerComponent>(base.Owner, null);
		}

		// Token: 0x040008FD RID: 2301
		[Nullable(2)]
		[ViewVariables]
		public PowerConsumerComponent PowerConsumerComponent;
	}
}
