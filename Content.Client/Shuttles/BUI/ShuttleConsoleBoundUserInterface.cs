using System;
using System.Runtime.CompilerServices;
using Content.Client.Shuttles.UI;
using Content.Shared.Shuttles.BUIStates;
using Content.Shared.Shuttles.Events;
using Robust.Client.GameObjects;
using Robust.Shared.GameObjects;

namespace Content.Client.Shuttles.BUI
{
	// Token: 0x0200015B RID: 347
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class ShuttleConsoleBoundUserInterface : BoundUserInterface
	{
		// Token: 0x0600091D RID: 2333 RVA: 0x000021BC File Offset: 0x000003BC
		public ShuttleConsoleBoundUserInterface(ClientUserInterfaceComponent owner, Enum uiKey) : base(owner, uiKey)
		{
		}

		// Token: 0x0600091E RID: 2334 RVA: 0x00035ACC File Offset: 0x00033CCC
		protected override void Open()
		{
			base.Open();
			this._window = new ShuttleConsoleWindow();
			ShuttleConsoleWindow window = this._window;
			window.UndockPressed = (Action<EntityUid>)Delegate.Combine(window.UndockPressed, new Action<EntityUid>(this.OnUndockPressed));
			ShuttleConsoleWindow window2 = this._window;
			window2.StartAutodockPressed = (Action<EntityUid>)Delegate.Combine(window2.StartAutodockPressed, new Action<EntityUid>(this.OnAutodockPressed));
			ShuttleConsoleWindow window3 = this._window;
			window3.StopAutodockPressed = (Action<EntityUid>)Delegate.Combine(window3.StopAutodockPressed, new Action<EntityUid>(this.OnStopAutodockPressed));
			ShuttleConsoleWindow window4 = this._window;
			window4.DestinationPressed = (Action<EntityUid>)Delegate.Combine(window4.DestinationPressed, new Action<EntityUid>(this.OnDestinationPressed));
			this._window.OpenCentered();
			this._window.OnClose += this.OnClose;
		}

		// Token: 0x0600091F RID: 2335 RVA: 0x00035BA8 File Offset: 0x00033DA8
		private void OnDestinationPressed(EntityUid obj)
		{
			base.SendMessage(new ShuttleConsoleDestinationMessage
			{
				Destination = obj
			});
		}

		// Token: 0x06000920 RID: 2336 RVA: 0x00035BBC File Offset: 0x00033DBC
		private void OnClose()
		{
			base.Close();
		}

		// Token: 0x06000921 RID: 2337 RVA: 0x00035BC4 File Offset: 0x00033DC4
		protected override void Dispose(bool disposing)
		{
			base.Dispose(disposing);
			if (disposing)
			{
				ShuttleConsoleWindow window = this._window;
				if (window == null)
				{
					return;
				}
				window.Dispose();
			}
		}

		// Token: 0x06000922 RID: 2338 RVA: 0x00035BE0 File Offset: 0x00033DE0
		private void OnStopAutodockPressed(EntityUid obj)
		{
			base.SendMessage(new StopAutodockRequestMessage
			{
				DockEntity = obj
			});
		}

		// Token: 0x06000923 RID: 2339 RVA: 0x00035BF4 File Offset: 0x00033DF4
		private void OnAutodockPressed(EntityUid obj)
		{
			base.SendMessage(new AutodockRequestMessage
			{
				DockEntity = obj
			});
		}

		// Token: 0x06000924 RID: 2340 RVA: 0x00035C08 File Offset: 0x00033E08
		private void OnUndockPressed(EntityUid obj)
		{
			base.SendMessage(new UndockRequestMessage
			{
				DockEntity = obj
			});
		}

		// Token: 0x06000925 RID: 2341 RVA: 0x00035C1C File Offset: 0x00033E1C
		protected override void UpdateState(BoundUserInterfaceState state)
		{
			base.UpdateState(state);
			ShuttleConsoleBoundInterfaceState shuttleConsoleBoundInterfaceState = state as ShuttleConsoleBoundInterfaceState;
			if (shuttleConsoleBoundInterfaceState == null)
			{
				return;
			}
			ShuttleConsoleWindow window = this._window;
			if (window != null)
			{
				window.SetMatrix(shuttleConsoleBoundInterfaceState.Coordinates, shuttleConsoleBoundInterfaceState.Angle);
			}
			ShuttleConsoleWindow window2 = this._window;
			if (window2 == null)
			{
				return;
			}
			window2.UpdateState(shuttleConsoleBoundInterfaceState);
		}

		// Token: 0x04000491 RID: 1169
		[Nullable(2)]
		private ShuttleConsoleWindow _window;
	}
}
