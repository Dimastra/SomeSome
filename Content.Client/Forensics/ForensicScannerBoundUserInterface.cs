using System;
using System.Runtime.CompilerServices;
using System.Threading;
using Content.Shared.Forensics;
using Robust.Client.GameObjects;
using Robust.Client.UserInterface.Controls;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Timing;

namespace Content.Client.Forensics
{
	// Token: 0x0200030C RID: 780
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class ForensicScannerBoundUserInterface : BoundUserInterface
	{
		// Token: 0x060013B2 RID: 5042 RVA: 0x000021BC File Offset: 0x000003BC
		public ForensicScannerBoundUserInterface(ClientUserInterfaceComponent owner, Enum uiKey) : base(owner, uiKey)
		{
		}

		// Token: 0x060013B3 RID: 5043 RVA: 0x00073FA8 File Offset: 0x000721A8
		protected override void Open()
		{
			base.Open();
			this._window = new ForensicScannerMenu();
			this._window.OnClose += base.Close;
			this._window.Print.OnPressed += delegate(BaseButton.ButtonEventArgs _)
			{
				this.Print();
			};
			this._window.Clear.OnPressed += delegate(BaseButton.ButtonEventArgs _)
			{
				this.Clear();
			};
			this._window.OpenCentered();
		}

		// Token: 0x060013B4 RID: 5044 RVA: 0x00074020 File Offset: 0x00072220
		private void Print()
		{
			base.SendMessage(new ForensicScannerPrintMessage());
			if (this._window != null)
			{
				this._window.UpdatePrinterState(true);
			}
			Timer.Spawn(this._printCooldown, delegate()
			{
				if (this._window != null)
				{
					this._window.UpdatePrinterState(false);
				}
			}, default(CancellationToken));
		}

		// Token: 0x060013B5 RID: 5045 RVA: 0x0007406C File Offset: 0x0007226C
		private void Clear()
		{
			base.SendMessage(new ForensicScannerClearMessage());
		}

		// Token: 0x060013B6 RID: 5046 RVA: 0x0007407C File Offset: 0x0007227C
		protected override void UpdateState(BoundUserInterfaceState state)
		{
			base.UpdateState(state);
			if (this._window == null)
			{
				return;
			}
			ForensicScannerBoundUserInterfaceState forensicScannerBoundUserInterfaceState = state as ForensicScannerBoundUserInterfaceState;
			if (forensicScannerBoundUserInterfaceState == null)
			{
				return;
			}
			this._printCooldown = forensicScannerBoundUserInterfaceState.PrintCooldown;
			if (forensicScannerBoundUserInterfaceState.PrintReadyAt > this._gameTiming.CurTime)
			{
				Timer.Spawn(forensicScannerBoundUserInterfaceState.PrintReadyAt - this._gameTiming.CurTime, delegate()
				{
					if (this._window != null)
					{
						this._window.UpdatePrinterState(false);
					}
				}, default(CancellationToken));
			}
			this._window.UpdateState(forensicScannerBoundUserInterfaceState);
		}

		// Token: 0x060013B7 RID: 5047 RVA: 0x00074104 File Offset: 0x00072304
		protected override void Dispose(bool disposing)
		{
			base.Dispose(disposing);
			if (!disposing)
			{
				return;
			}
			ForensicScannerMenu window = this._window;
			if (window == null)
			{
				return;
			}
			window.Dispose();
		}

		// Token: 0x040009E3 RID: 2531
		[Dependency]
		private readonly IGameTiming _gameTiming;

		// Token: 0x040009E4 RID: 2532
		[Nullable(2)]
		private ForensicScannerMenu _window;

		// Token: 0x040009E5 RID: 2533
		private TimeSpan _printCooldown;
	}
}
