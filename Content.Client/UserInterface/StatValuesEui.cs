using System;
using System.Runtime.CompilerServices;
using Content.Client.Eui;
using Content.Shared.Eui;
using Content.Shared.UserInterface;

namespace Content.Client.UserInterface
{
	// Token: 0x0200006E RID: 110
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class StatValuesEui : BaseEui
	{
		// Token: 0x06000201 RID: 513 RVA: 0x0000E41C File Offset: 0x0000C61C
		public StatValuesEui()
		{
			this._window = new StatsWindow();
			this._window.Title = "Melee stats";
			this._window.OpenCentered();
			this._window.OnClose += this.Closed;
		}

		// Token: 0x06000202 RID: 514 RVA: 0x0000E470 File Offset: 0x0000C670
		public override void HandleMessage(EuiMessageBase msg)
		{
			base.HandleMessage(msg);
			StatValuesEuiMessage statValuesEuiMessage = msg as StatValuesEuiMessage;
			if (statValuesEuiMessage == null)
			{
				return;
			}
			this._window.Title = statValuesEuiMessage.Title;
			this._window.UpdateValues(statValuesEuiMessage.Headers, statValuesEuiMessage.Values);
		}

		// Token: 0x04000144 RID: 324
		private readonly StatsWindow _window;
	}
}
