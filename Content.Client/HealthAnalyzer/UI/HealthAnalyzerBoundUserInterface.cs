using System;
using System.Runtime.CompilerServices;
using Content.Shared.MedicalScanner;
using Robust.Client.GameObjects;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;

namespace Content.Client.HealthAnalyzer.UI
{
	// Token: 0x020002D9 RID: 729
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class HealthAnalyzerBoundUserInterface : BoundUserInterface
	{
		// Token: 0x0600126B RID: 4715 RVA: 0x000021BC File Offset: 0x000003BC
		public HealthAnalyzerBoundUserInterface(ClientUserInterfaceComponent owner, Enum uiKey) : base(owner, uiKey)
		{
		}

		// Token: 0x0600126C RID: 4716 RVA: 0x0006DD68 File Offset: 0x0006BF68
		protected override void Open()
		{
			base.Open();
			this._window = new HealthAnalyzerWindow
			{
				Title = IoCManager.Resolve<IEntityManager>().GetComponent<MetaDataComponent>(base.Owner.Owner).EntityName
			};
			this._window.OnClose += base.Close;
			this._window.OpenCentered();
		}

		// Token: 0x0600126D RID: 4717 RVA: 0x0006DDC8 File Offset: 0x0006BFC8
		protected override void ReceiveMessage(BoundUserInterfaceMessage message)
		{
			if (this._window == null)
			{
				return;
			}
			SharedHealthAnalyzerComponent.HealthAnalyzerScannedUserMessage healthAnalyzerScannedUserMessage = message as SharedHealthAnalyzerComponent.HealthAnalyzerScannedUserMessage;
			if (healthAnalyzerScannedUserMessage == null)
			{
				return;
			}
			this._window.Populate(healthAnalyzerScannedUserMessage);
		}

		// Token: 0x0600126E RID: 4718 RVA: 0x0006DDF5 File Offset: 0x0006BFF5
		protected override void Dispose(bool disposing)
		{
			base.Dispose(disposing);
			if (!disposing)
			{
				return;
			}
			if (this._window != null)
			{
				this._window.OnClose -= base.Close;
			}
			HealthAnalyzerWindow window = this._window;
			if (window == null)
			{
				return;
			}
			window.Dispose();
		}

		// Token: 0x04000920 RID: 2336
		[Nullable(2)]
		private HealthAnalyzerWindow _window;
	}
}
