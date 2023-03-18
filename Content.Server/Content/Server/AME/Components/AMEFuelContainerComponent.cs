using System;
using Robust.Shared.GameObjects;
using Robust.Shared.ViewVariables;

namespace Content.Server.AME.Components
{
	// Token: 0x020007D7 RID: 2007
	[RegisterComponent]
	public sealed class AMEFuelContainerComponent : Component
	{
		// Token: 0x170006BC RID: 1724
		// (get) Token: 0x06002BA7 RID: 11175 RVA: 0x000E5668 File Offset: 0x000E3868
		// (set) Token: 0x06002BA8 RID: 11176 RVA: 0x000E5670 File Offset: 0x000E3870
		[ViewVariables]
		public int FuelAmount
		{
			get
			{
				return this._fuelAmount;
			}
			set
			{
				this._fuelAmount = value;
			}
		}

		// Token: 0x170006BD RID: 1725
		// (get) Token: 0x06002BA9 RID: 11177 RVA: 0x000E5679 File Offset: 0x000E3879
		// (set) Token: 0x06002BAA RID: 11178 RVA: 0x000E5681 File Offset: 0x000E3881
		[ViewVariables]
		public int MaxFuelAmount
		{
			get
			{
				return this._maxFuelAmount;
			}
			set
			{
				this._maxFuelAmount = value;
			}
		}

		// Token: 0x06002BAB RID: 11179 RVA: 0x000E568A File Offset: 0x000E388A
		protected override void Initialize()
		{
			base.Initialize();
			this._maxFuelAmount = 1000;
			this._fuelAmount = 1000;
		}

		// Token: 0x04001B16 RID: 6934
		private int _fuelAmount;

		// Token: 0x04001B17 RID: 6935
		private int _maxFuelAmount;
	}
}
