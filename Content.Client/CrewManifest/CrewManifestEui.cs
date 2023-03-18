using System;
using System.Runtime.CompilerServices;
using Content.Client.Eui;
using Content.Shared.CrewManifest;
using Content.Shared.Eui;

namespace Content.Client.CrewManifest
{
	// Token: 0x0200036C RID: 876
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class CrewManifestEui : BaseEui
	{
		// Token: 0x06001592 RID: 5522 RVA: 0x0007FD11 File Offset: 0x0007DF11
		public CrewManifestEui()
		{
			this._window = new CrewManifestUi();
			this._window.OnClose += delegate()
			{
				base.SendMessage(new CrewManifestEuiClosed());
			};
		}

		// Token: 0x06001593 RID: 5523 RVA: 0x0007FD3B File Offset: 0x0007DF3B
		public override void Opened()
		{
			base.Opened();
			this._window.OpenCentered();
		}

		// Token: 0x06001594 RID: 5524 RVA: 0x0007FD4E File Offset: 0x0007DF4E
		public override void Closed()
		{
			base.Closed();
			this._window.Close();
		}

		// Token: 0x06001595 RID: 5525 RVA: 0x0007FD64 File Offset: 0x0007DF64
		public override void HandleState(EuiStateBase state)
		{
			base.HandleState(state);
			CrewManifestEuiState crewManifestEuiState = state as CrewManifestEuiState;
			if (crewManifestEuiState == null)
			{
				return;
			}
			this._window.Populate(crewManifestEuiState.StationName, crewManifestEuiState.Entries);
		}

		// Token: 0x04000B49 RID: 2889
		private readonly CrewManifestUi _window;
	}
}
