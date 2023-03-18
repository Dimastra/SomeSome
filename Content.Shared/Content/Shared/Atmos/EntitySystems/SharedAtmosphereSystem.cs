using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Content.Shared.Atmos.Prototypes;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Prototypes;

namespace Content.Shared.Atmos.EntitySystems
{
	// Token: 0x020006DF RID: 1759
	[NullableContext(1)]
	[Nullable(0)]
	public abstract class SharedAtmosphereSystem : EntitySystem
	{
		// Token: 0x06001558 RID: 5464 RVA: 0x00045D74 File Offset: 0x00043F74
		public override void Initialize()
		{
			base.Initialize();
			for (int i = 0; i < 9; i++)
			{
				this.GasPrototypes[i] = this._prototypeManager.Index<GasPrototype>(i.ToString());
			}
		}

		// Token: 0x06001559 RID: 5465 RVA: 0x00045DAE File Offset: 0x00043FAE
		public GasPrototype GetGas(int gasId)
		{
			return this.GasPrototypes[gasId];
		}

		// Token: 0x0600155A RID: 5466 RVA: 0x00045DB8 File Offset: 0x00043FB8
		public GasPrototype GetGas(Gas gasId)
		{
			return this.GasPrototypes[(int)gasId];
		}

		// Token: 0x17000481 RID: 1153
		// (get) Token: 0x0600155B RID: 5467 RVA: 0x00045DC2 File Offset: 0x00043FC2
		public IEnumerable<GasPrototype> Gases
		{
			get
			{
				return this.GasPrototypes;
			}
		}

		// Token: 0x04001576 RID: 5494
		[Dependency]
		private readonly IPrototypeManager _prototypeManager;

		// Token: 0x04001577 RID: 5495
		protected readonly GasPrototype[] GasPrototypes = new GasPrototype[9];
	}
}
