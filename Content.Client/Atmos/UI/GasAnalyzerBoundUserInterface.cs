using System;
using System.Runtime.CompilerServices;
using Content.Shared.Atmos.Components;
using Robust.Client.GameObjects;
using Robust.Shared.GameObjects;

namespace Content.Client.Atmos.UI
{
	// Token: 0x02000434 RID: 1076
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class GasAnalyzerBoundUserInterface : BoundUserInterface
	{
		// Token: 0x06001A3A RID: 6714 RVA: 0x000021BC File Offset: 0x000003BC
		public GasAnalyzerBoundUserInterface(ClientUserInterfaceComponent owner, Enum uiKey) : base(owner, uiKey)
		{
		}

		// Token: 0x06001A3B RID: 6715 RVA: 0x00095F50 File Offset: 0x00094150
		protected override void Open()
		{
			base.Open();
			this._window = new GasAnalyzerWindow(this);
			this._window.OnClose += this.OnClose;
			this._window.OpenCentered();
		}

		// Token: 0x06001A3C RID: 6716 RVA: 0x00095F88 File Offset: 0x00094188
		protected override void ReceiveMessage(BoundUserInterfaceMessage message)
		{
			if (this._window == null)
			{
				return;
			}
			SharedGasAnalyzerComponent.GasAnalyzerUserMessage gasAnalyzerUserMessage = message as SharedGasAnalyzerComponent.GasAnalyzerUserMessage;
			if (gasAnalyzerUserMessage == null)
			{
				return;
			}
			this._window.Populate(gasAnalyzerUserMessage);
		}

		// Token: 0x06001A3D RID: 6717 RVA: 0x00095FB5 File Offset: 0x000941B5
		private void OnClose()
		{
			base.SendMessage(new SharedGasAnalyzerComponent.GasAnalyzerDisableMessage());
			base.Close();
		}

		// Token: 0x06001A3E RID: 6718 RVA: 0x00095FC8 File Offset: 0x000941C8
		protected override void Dispose(bool disposing)
		{
			base.Dispose(disposing);
			if (disposing)
			{
				GasAnalyzerWindow window = this._window;
				if (window == null)
				{
					return;
				}
				window.Dispose();
			}
		}

		// Token: 0x04000D5C RID: 3420
		[Nullable(2)]
		private GasAnalyzerWindow _window;
	}
}
