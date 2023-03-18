using System;
using System.Runtime.CompilerServices;
using Content.Shared.Access.Systems;
using Robust.Client.GameObjects;
using Robust.Shared.GameObjects;

namespace Content.Client.Access.UI
{
	// Token: 0x020004F9 RID: 1273
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class AgentIDCardBoundUserInterface : BoundUserInterface
	{
		// Token: 0x06002052 RID: 8274 RVA: 0x000021BC File Offset: 0x000003BC
		public AgentIDCardBoundUserInterface(ClientUserInterfaceComponent owner, Enum uiKey) : base(owner, uiKey)
		{
		}

		// Token: 0x06002053 RID: 8275 RVA: 0x000BB814 File Offset: 0x000B9A14
		protected override void Open()
		{
			base.Open();
			this._window = new AgentIDCardWindow();
			if (base.State != null)
			{
				this.UpdateState(base.State);
			}
			this._window.OpenCentered();
			this._window.OnClose += base.Close;
			this._window.OnNameEntered += this.OnNameChanged;
			this._window.OnJobEntered += this.OnJobChanged;
		}

		// Token: 0x06002054 RID: 8276 RVA: 0x000BB896 File Offset: 0x000B9A96
		private void OnNameChanged(string newName)
		{
			base.SendMessage(new AgentIDCardNameChangedMessage(newName));
		}

		// Token: 0x06002055 RID: 8277 RVA: 0x000BB8A4 File Offset: 0x000B9AA4
		private void OnJobChanged(string newJob)
		{
			base.SendMessage(new AgentIDCardJobChangedMessage(newJob));
		}

		// Token: 0x06002056 RID: 8278 RVA: 0x000BB8B4 File Offset: 0x000B9AB4
		protected override void UpdateState(BoundUserInterfaceState state)
		{
			base.UpdateState(state);
			if (this._window != null)
			{
				AgentIDCardBoundUserInterfaceState agentIDCardBoundUserInterfaceState = state as AgentIDCardBoundUserInterfaceState;
				if (agentIDCardBoundUserInterfaceState != null)
				{
					this._window.SetCurrentName(agentIDCardBoundUserInterfaceState.CurrentName);
					this._window.SetCurrentJob(agentIDCardBoundUserInterfaceState.CurrentJob);
					return;
				}
			}
		}

		// Token: 0x06002057 RID: 8279 RVA: 0x000BB8FD File Offset: 0x000B9AFD
		protected override void Dispose(bool disposing)
		{
			base.Dispose(disposing);
			if (!disposing)
			{
				return;
			}
			AgentIDCardWindow window = this._window;
			if (window == null)
			{
				return;
			}
			window.Dispose();
		}

		// Token: 0x04000F6D RID: 3949
		[Nullable(2)]
		private AgentIDCardWindow _window;
	}
}
