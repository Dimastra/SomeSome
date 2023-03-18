using System;
using System.Runtime.CompilerServices;
using Robust.Shared.Audio;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.IoC;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.ViewVariables;

namespace Content.Shared.Gravity
{
	// Token: 0x02000446 RID: 1094
	[NullableContext(1)]
	[Nullable(0)]
	[RegisterComponent]
	[NetworkedComponent]
	public sealed class GravityComponent : Component
	{
		// Token: 0x170002C2 RID: 706
		// (get) Token: 0x06000D48 RID: 3400 RVA: 0x0002BEAB File Offset: 0x0002A0AB
		// (set) Token: 0x06000D49 RID: 3401 RVA: 0x0002BEB3 File Offset: 0x0002A0B3
		[DataField("gravityShakeSound", false, 1, false, false, null)]
		public SoundSpecifier GravityShakeSound { get; set; } = new SoundPathSpecifier("/Audio/Effects/alert.ogg", null);

		// Token: 0x170002C3 RID: 707
		// (get) Token: 0x06000D4A RID: 3402 RVA: 0x0002BEBC File Offset: 0x0002A0BC
		// (set) Token: 0x06000D4B RID: 3403 RVA: 0x0002BEC4 File Offset: 0x0002A0C4
		[ViewVariables]
		public bool EnabledVV
		{
			get
			{
				return this.Enabled;
			}
			set
			{
				if (this.Enabled == value)
				{
					return;
				}
				this.Enabled = value;
				GravityChangedEvent ev = new GravityChangedEvent(base.Owner, value);
				IoCManager.Resolve<IEntityManager>().EventBus.RaiseLocalEvent<GravityChangedEvent>(base.Owner, ref ev, false);
				base.Dirty(null);
			}
		}

		// Token: 0x04000CCF RID: 3279
		[DataField("enabled", false, 1, false, false, null)]
		public bool Enabled;
	}
}
