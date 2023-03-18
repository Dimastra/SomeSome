using System;
using System.Linq;
using System.Runtime.CompilerServices;
using Content.Client.Eui;
using Content.Shared.Administration.Logs;
using Content.Shared.Database;
using Content.Shared.Eui;
using Robust.Client.Graphics;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.Controls;
using Robust.Shared.IoC;

namespace Content.Client.Administration.UI.Logs
{
	// Token: 0x020004C2 RID: 1218
	[NullableContext(2)]
	[Nullable(0)]
	public sealed class AdminLogsEui : BaseEui
	{
		// Token: 0x06001EF9 RID: 7929 RVA: 0x000B5EBC File Offset: 0x000B40BC
		public AdminLogsEui()
		{
			this.LogsWindow = new AdminLogsWindow();
			this.LogsControl = this.LogsWindow.Logs;
			this.LogsControl.LogSearch.OnTextEntered += delegate(LineEdit.LineEditEventArgs _)
			{
				this.RequestLogs();
			};
			this.LogsControl.RefreshButton.OnPressed += delegate(BaseButton.ButtonEventArgs _)
			{
				this.RequestLogs();
			};
			this.LogsControl.NextButton.OnPressed += delegate(BaseButton.ButtonEventArgs _)
			{
				this.NextLogs();
			};
			this.LogsControl.PopOutButton.OnPressed += delegate(BaseButton.ButtonEventArgs _)
			{
				this.PopOut();
			};
		}

		// Token: 0x170006BF RID: 1727
		// (get) Token: 0x06001EFA RID: 7930 RVA: 0x000B5F62 File Offset: 0x000B4162
		// (set) Token: 0x06001EFB RID: 7931 RVA: 0x000B5F6A File Offset: 0x000B416A
		private WindowRoot Root { get; set; }

		// Token: 0x170006C0 RID: 1728
		// (get) Token: 0x06001EFC RID: 7932 RVA: 0x000B5F73 File Offset: 0x000B4173
		// (set) Token: 0x06001EFD RID: 7933 RVA: 0x000B5F7B File Offset: 0x000B417B
		private IClydeWindow ClydeWindow { get; set; }

		// Token: 0x170006C1 RID: 1729
		// (get) Token: 0x06001EFE RID: 7934 RVA: 0x000B5F84 File Offset: 0x000B4184
		// (set) Token: 0x06001EFF RID: 7935 RVA: 0x000B5F8C File Offset: 0x000B418C
		private AdminLogsWindow LogsWindow { get; set; }

		// Token: 0x170006C2 RID: 1730
		// (get) Token: 0x06001F00 RID: 7936 RVA: 0x000B5F95 File Offset: 0x000B4195
		[Nullable(1)]
		private AdminLogsControl LogsControl { [NullableContext(1)] get; }

		// Token: 0x170006C3 RID: 1731
		// (get) Token: 0x06001F01 RID: 7937 RVA: 0x000B5F9D File Offset: 0x000B419D
		// (set) Token: 0x06001F02 RID: 7938 RVA: 0x000B5FA5 File Offset: 0x000B41A5
		private bool FirstState { get; set; } = true;

		// Token: 0x06001F03 RID: 7939 RVA: 0x000B5FAE File Offset: 0x000B41AE
		private void OnRequestClosed(WindowRequestClosedEventArgs args)
		{
			base.SendMessage(new AdminLogsEuiMsg.Close());
		}

		// Token: 0x06001F04 RID: 7940 RVA: 0x000B5FBC File Offset: 0x000B41BC
		private void RequestLogs()
		{
			AdminLogsEuiMsg.LogsRequest msg = new AdminLogsEuiMsg.LogsRequest(new int?(this.LogsControl.SelectedRoundId), this.LogsControl.Search, this.LogsControl.SelectedTypes.ToHashSet<LogType>(), null, null, null, this.LogsControl.SelectedPlayers.ToArray<Guid>(), null, null, DateOrder.Descending);
			base.SendMessage(msg);
		}

		// Token: 0x06001F05 RID: 7941 RVA: 0x000B6030 File Offset: 0x000B4230
		private void NextLogs()
		{
			AdminLogsEuiMsg.NextLogsRequest msg = new AdminLogsEuiMsg.NextLogsRequest();
			base.SendMessage(msg);
		}

		// Token: 0x06001F06 RID: 7942 RVA: 0x000B604C File Offset: 0x000B424C
		private void PopOut()
		{
			if (this.LogsWindow == null)
			{
				return;
			}
			this.LogsControl.Orphan();
			this.LogsWindow.Dispose();
			this.LogsWindow = null;
			IClydeMonitor monitor = this._clyde.EnumerateMonitors().First<IClydeMonitor>();
			this.ClydeWindow = this._clyde.CreateWindow(new WindowCreateParameters
			{
				Maximized = false,
				Title = "Admin Logs",
				Monitor = monitor,
				Width = 1000,
				Height = 400
			});
			this.ClydeWindow.RequestClosed += this.OnRequestClosed;
			this.ClydeWindow.DisposeOnClose = true;
			this.Root = this._uiManager.CreateWindowRoot(this.ClydeWindow);
			this.Root.AddChild(this.LogsControl);
			this.LogsControl.PopOutButton.Disabled = true;
			this.LogsControl.PopOutButton.Visible = false;
		}

		// Token: 0x06001F07 RID: 7943 RVA: 0x000B6144 File Offset: 0x000B4344
		[NullableContext(1)]
		public override void HandleState(EuiStateBase state)
		{
			AdminLogsEuiState adminLogsEuiState = (AdminLogsEuiState)state;
			if (adminLogsEuiState.IsLoading)
			{
				return;
			}
			this.LogsControl.SetCurrentRound(adminLogsEuiState.RoundId);
			this.LogsControl.SetPlayers(adminLogsEuiState.Players);
			if (!this.FirstState)
			{
				return;
			}
			this.FirstState = false;
			this.LogsControl.SetRoundSpinBox(adminLogsEuiState.RoundId);
			this.RequestLogs();
		}

		// Token: 0x06001F08 RID: 7944 RVA: 0x000B61AC File Offset: 0x000B43AC
		[NullableContext(1)]
		public override void HandleMessage(EuiMessageBase msg)
		{
			base.HandleMessage(msg);
			AdminLogsEuiMsg.NewLogs newLogs = msg as AdminLogsEuiMsg.NewLogs;
			if (newLogs != null)
			{
				if (newLogs.Replace)
				{
					this.LogsControl.SetLogs(newLogs.Logs);
				}
				else
				{
					this.LogsControl.AddLogs(newLogs.Logs);
				}
				this.LogsControl.NextButton.Disabled = !newLogs.HasNext;
				return;
			}
			AdminLogsEuiMsg.SetLogFilter setLogFilter = msg as AdminLogsEuiMsg.SetLogFilter;
			if (setLogFilter == null)
			{
				return;
			}
			if (setLogFilter.Search != null)
			{
				this.LogsControl.LogSearch.SetText(setLogFilter.Search, false);
			}
			if (setLogFilter.Types != null)
			{
				this.LogsControl.SetTypesSelection(setLogFilter.Types, setLogFilter.InvertTypes);
			}
		}

		// Token: 0x06001F09 RID: 7945 RVA: 0x000B6259 File Offset: 0x000B4459
		public override void Opened()
		{
			base.Opened();
			AdminLogsWindow logsWindow = this.LogsWindow;
			if (logsWindow == null)
			{
				return;
			}
			logsWindow.OpenCentered();
		}

		// Token: 0x06001F0A RID: 7946 RVA: 0x000B6274 File Offset: 0x000B4474
		public override void Closed()
		{
			base.Closed();
			if (this.ClydeWindow != null)
			{
				this.ClydeWindow.RequestClosed -= this.OnRequestClosed;
			}
			this.LogsControl.Dispose();
			AdminLogsWindow logsWindow = this.LogsWindow;
			if (logsWindow != null)
			{
				logsWindow.Dispose();
			}
			WindowRoot root = this.Root;
			if (root != null)
			{
				root.Dispose();
			}
			IClydeWindow clydeWindow = this.ClydeWindow;
			if (clydeWindow == null)
			{
				return;
			}
			clydeWindow.Dispose();
		}

		// Token: 0x04000EF3 RID: 3827
		[Nullable(1)]
		[Dependency]
		private readonly IClyde _clyde;

		// Token: 0x04000EF4 RID: 3828
		[Nullable(1)]
		[Dependency]
		private readonly IUserInterfaceManager _uiManager;
	}
}
