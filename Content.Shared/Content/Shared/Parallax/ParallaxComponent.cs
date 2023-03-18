using System;
using System.Runtime.CompilerServices;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.IoC;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.ViewVariables;

namespace Content.Shared.Parallax
{
	// Token: 0x0200029A RID: 666
	[NullableContext(1)]
	[Nullable(0)]
	[RegisterComponent]
	[NetworkedComponent]
	public sealed class ParallaxComponent : Component
	{
		// Token: 0x1700016C RID: 364
		// (get) Token: 0x0600076E RID: 1902 RVA: 0x00019340 File Offset: 0x00017540
		// (set) Token: 0x0600076F RID: 1903 RVA: 0x00019348 File Offset: 0x00017548
		[ViewVariables]
		public string ParallaxVV
		{
			get
			{
				return this.Parallax;
			}
			set
			{
				if (value.Equals(this.Parallax))
				{
					return;
				}
				this.Parallax = value;
				IoCManager.Resolve<IEntityManager>().Dirty(this, null);
			}
		}

		// Token: 0x04000791 RID: 1937
		[DataField("parallax", false, 1, false, false, null)]
		public string Parallax = "Default";
	}
}
