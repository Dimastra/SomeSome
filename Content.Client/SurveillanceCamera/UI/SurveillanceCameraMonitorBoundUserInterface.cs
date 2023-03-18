using System;
using System.Runtime.CompilerServices;
using Content.Client.Eye;
using Content.Shared.SurveillanceCamera;
using Robust.Client.GameObjects;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;

namespace Content.Client.SurveillanceCamera.UI
{
	// Token: 0x0200010A RID: 266
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class SurveillanceCameraMonitorBoundUserInterface : BoundUserInterface
	{
		// Token: 0x06000753 RID: 1875 RVA: 0x0002653A File Offset: 0x0002473A
		public SurveillanceCameraMonitorBoundUserInterface(ClientUserInterfaceComponent owner, Enum uiKey) : base(owner, uiKey)
		{
			IoCManager.InjectDependencies<SurveillanceCameraMonitorBoundUserInterface>(this);
			this._eyeLerpingSystem = this._entityManager.EntitySysManager.GetEntitySystem<EyeLerpingSystem>();
			this._surveillanceCameraMonitorSystem = this._entityManager.EntitySysManager.GetEntitySystem<SurveillanceCameraMonitorSystem>();
		}

		// Token: 0x06000754 RID: 1876 RVA: 0x00026578 File Offset: 0x00024778
		protected override void Open()
		{
			base.Open();
			this._window = new SurveillanceCameraMonitorWindow();
			if (base.State != null)
			{
				this.UpdateState(base.State);
			}
			this._window.OpenCentered();
			this._window.CameraSelected += this.OnCameraSelected;
			this._window.SubnetOpened += this.OnSubnetRequest;
			this._window.CameraRefresh += this.OnCameraRefresh;
			this._window.SubnetRefresh += this.OnSubnetRefresh;
			this._window.OnClose += base.Close;
			this._window.CameraSwitchTimer += this.OnCameraSwitchTimer;
			this._window.CameraDisconnect += this.OnCameraDisconnect;
		}

		// Token: 0x06000755 RID: 1877 RVA: 0x00026656 File Offset: 0x00024856
		private void OnCameraSelected(string address)
		{
			base.SendMessage(new SurveillanceCameraMonitorSwitchMessage(address));
		}

		// Token: 0x06000756 RID: 1878 RVA: 0x00026664 File Offset: 0x00024864
		private void OnSubnetRequest(string subnet)
		{
			base.SendMessage(new SurveillanceCameraMonitorSubnetRequestMessage(subnet));
		}

		// Token: 0x06000757 RID: 1879 RVA: 0x00026672 File Offset: 0x00024872
		private void OnCameraSwitchTimer()
		{
			this._surveillanceCameraMonitorSystem.AddTimer(base.Owner.Owner, new Action(this._window.OnSwitchTimerComplete));
		}

		// Token: 0x06000758 RID: 1880 RVA: 0x0002669B File Offset: 0x0002489B
		private void OnCameraRefresh()
		{
			base.SendMessage(new SurveillanceCameraRefreshCamerasMessage());
		}

		// Token: 0x06000759 RID: 1881 RVA: 0x000266A8 File Offset: 0x000248A8
		private void OnSubnetRefresh()
		{
			base.SendMessage(new SurveillanceCameraRefreshSubnetsMessage());
		}

		// Token: 0x0600075A RID: 1882 RVA: 0x000266B5 File Offset: 0x000248B5
		private void OnCameraDisconnect()
		{
			base.SendMessage(new SurveillanceCameraDisconnectMessage());
		}

		// Token: 0x0600075B RID: 1883 RVA: 0x000266C4 File Offset: 0x000248C4
		protected override void UpdateState(BoundUserInterfaceState state)
		{
			if (this._window != null)
			{
				SurveillanceCameraMonitorUiState surveillanceCameraMonitorUiState = state as SurveillanceCameraMonitorUiState;
				if (surveillanceCameraMonitorUiState != null)
				{
					if (surveillanceCameraMonitorUiState.ActiveCamera == null)
					{
						this._window.UpdateState(null, surveillanceCameraMonitorUiState.Subnets, surveillanceCameraMonitorUiState.ActiveAddress, surveillanceCameraMonitorUiState.ActiveSubnet, surveillanceCameraMonitorUiState.Cameras);
						if (this._currentCamera != null)
						{
							this._surveillanceCameraMonitorSystem.RemoveTimer(base.Owner.Owner);
							this._eyeLerpingSystem.RemoveEye(this._currentCamera.Value);
							this._currentCamera = null;
							return;
						}
					}
					else
					{
						if (this._currentCamera == null)
						{
							this._eyeLerpingSystem.AddEye(surveillanceCameraMonitorUiState.ActiveCamera.Value, null, false);
							this._currentCamera = surveillanceCameraMonitorUiState.ActiveCamera;
						}
						else if (this._currentCamera != surveillanceCameraMonitorUiState.ActiveCamera)
						{
							this._eyeLerpingSystem.RemoveEye(this._currentCamera.Value);
							this._eyeLerpingSystem.AddEye(surveillanceCameraMonitorUiState.ActiveCamera.Value, null, false);
							this._currentCamera = surveillanceCameraMonitorUiState.ActiveCamera;
						}
						EyeComponent eyeComponent;
						if (this._entityManager.TryGetComponent<EyeComponent>(surveillanceCameraMonitorUiState.ActiveCamera, ref eyeComponent))
						{
							this._window.UpdateState(eyeComponent.Eye, surveillanceCameraMonitorUiState.Subnets, surveillanceCameraMonitorUiState.ActiveAddress, surveillanceCameraMonitorUiState.ActiveSubnet, surveillanceCameraMonitorUiState.Cameras);
						}
					}
					return;
				}
			}
		}

		// Token: 0x0600075C RID: 1884 RVA: 0x00026854 File Offset: 0x00024A54
		protected override void Dispose(bool disposing)
		{
			base.Dispose(disposing);
			if (this._currentCamera != null)
			{
				this._eyeLerpingSystem.RemoveEye(this._currentCamera.Value);
				this._currentCamera = null;
			}
			if (disposing)
			{
				SurveillanceCameraMonitorWindow window = this._window;
				if (window == null)
				{
					return;
				}
				window.Dispose();
			}
		}

		// Token: 0x04000367 RID: 871
		[Dependency]
		private readonly IEntityManager _entityManager;

		// Token: 0x04000368 RID: 872
		private readonly EyeLerpingSystem _eyeLerpingSystem;

		// Token: 0x04000369 RID: 873
		private readonly SurveillanceCameraMonitorSystem _surveillanceCameraMonitorSystem;

		// Token: 0x0400036A RID: 874
		[Nullable(2)]
		private SurveillanceCameraMonitorWindow _window;

		// Token: 0x0400036B RID: 875
		private EntityUid? _currentCamera;
	}
}
