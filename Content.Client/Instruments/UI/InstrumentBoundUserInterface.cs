using System;
using System.Runtime.CompilerServices;
using Robust.Client.GameObjects;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.ViewVariables;

namespace Content.Client.Instruments.UI
{
	// Token: 0x020002B0 RID: 688
	[NullableContext(2)]
	[Nullable(0)]
	public sealed class InstrumentBoundUserInterface : BoundUserInterface
	{
		// Token: 0x170003BD RID: 957
		// (get) Token: 0x0600116E RID: 4462 RVA: 0x0006730E File Offset: 0x0006550E
		// (set) Token: 0x0600116F RID: 4463 RVA: 0x00067316 File Offset: 0x00065516
		public InstrumentComponent Instrument { get; set; }

		// Token: 0x06001170 RID: 4464 RVA: 0x000021BC File Offset: 0x000003BC
		[NullableContext(1)]
		public InstrumentBoundUserInterface(ClientUserInterfaceComponent owner, Enum uiKey) : base(owner, uiKey)
		{
		}

		// Token: 0x06001171 RID: 4465 RVA: 0x00067320 File Offset: 0x00065520
		protected override void Open()
		{
			InstrumentComponent instrument;
			if (!IoCManager.Resolve<IEntityManager>().TryGetComponent<InstrumentComponent>(base.Owner.Owner, ref instrument))
			{
				return;
			}
			this.Instrument = instrument;
			this._instrumentMenu = new InstrumentMenu(this);
			this._instrumentMenu.OnClose += base.Close;
			this._instrumentMenu.OpenCentered();
		}

		// Token: 0x06001172 RID: 4466 RVA: 0x0006737C File Offset: 0x0006557C
		protected override void Dispose(bool disposing)
		{
			base.Dispose(disposing);
			if (!disposing)
			{
				return;
			}
			InstrumentMenu instrumentMenu = this._instrumentMenu;
			if (instrumentMenu == null)
			{
				return;
			}
			instrumentMenu.Dispose();
		}

		// Token: 0x0400088F RID: 2191
		[ViewVariables]
		private InstrumentMenu _instrumentMenu;
	}
}
