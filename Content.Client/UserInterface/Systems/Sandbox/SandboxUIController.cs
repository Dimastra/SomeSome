using System;
using System.Runtime.CompilerServices;
using Content.Client.Gameplay;
using Content.Client.Markers;
using Content.Client.Sandbox;
using Content.Client.SubFloor;
using Content.Client.UserInterface.Controls;
using Content.Client.UserInterface.Systems.DecalPlacer;
using Content.Client.UserInterface.Systems.MenuBar.Widgets;
using Content.Client.UserInterface.Systems.Sandbox.Windows;
using Content.Shared.Input;
using Robust.Client.Debugging;
using Robust.Client.Graphics;
using Robust.Client.Input;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.Controllers;
using Robust.Client.UserInterface.Controllers.Implementations;
using Robust.Client.UserInterface.Controls;
using Robust.Shared.GameObjects;
using Robust.Shared.Input.Binding;
using Robust.Shared.IoC;
using Robust.Shared.Map;
using Robust.Shared.Players;

namespace Content.Client.UserInterface.Systems.Sandbox
{
	// Token: 0x02000071 RID: 113
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class SandboxUIController : UIController, IOnStateChanged<GameplayState>, IOnStateEntered<GameplayState>, IOnStateExited<GameplayState>, IOnSystemChanged<SandboxSystem>, IOnSystemLoaded<SandboxSystem>, IOnSystemUnloaded<SandboxSystem>
	{
		// Token: 0x17000048 RID: 72
		// (get) Token: 0x06000210 RID: 528 RVA: 0x0000E784 File Offset: 0x0000C984
		private EntitySpawningUIController EntitySpawningController
		{
			get
			{
				return this.UIManager.GetUIController<EntitySpawningUIController>();
			}
		}

		// Token: 0x17000049 RID: 73
		// (get) Token: 0x06000211 RID: 529 RVA: 0x0000E791 File Offset: 0x0000C991
		private TileSpawningUIController TileSpawningController
		{
			get
			{
				return this.UIManager.GetUIController<TileSpawningUIController>();
			}
		}

		// Token: 0x1700004A RID: 74
		// (get) Token: 0x06000212 RID: 530 RVA: 0x0000E79E File Offset: 0x0000C99E
		private DecalPlacerUIController DecalPlacerController
		{
			get
			{
				return this.UIManager.GetUIController<DecalPlacerUIController>();
			}
		}

		// Token: 0x1700004B RID: 75
		// (get) Token: 0x06000213 RID: 531 RVA: 0x0000E7AB File Offset: 0x0000C9AB
		[Nullable(2)]
		private MenuButton SandboxButton
		{
			[NullableContext(2)]
			get
			{
				GameTopMenuBar activeUIWidgetOrNull = this.UIManager.GetActiveUIWidgetOrNull<GameTopMenuBar>();
				if (activeUIWidgetOrNull == null)
				{
					return null;
				}
				return activeUIWidgetOrNull.SandboxButton;
			}
		}

		// Token: 0x06000214 RID: 532 RVA: 0x0000E7C4 File Offset: 0x0000C9C4
		public void OnStateEntered(GameplayState state)
		{
			this.EnsureWindow();
			this.CheckSandboxVisibility();
			this._input.SetInputCommand(ContentKeyFunctions.OpenEntitySpawnWindow, InputCmdHandler.FromDelegate(delegate(ICommonSession _)
			{
				this.EntitySpawningController.ToggleWindow();
			}, null, true, true));
			this._input.SetInputCommand(ContentKeyFunctions.OpenSandboxWindow, InputCmdHandler.FromDelegate(delegate(ICommonSession _)
			{
				this.ToggleWindow();
			}, null, true, true));
			this._input.SetInputCommand(ContentKeyFunctions.OpenTileSpawnWindow, InputCmdHandler.FromDelegate(delegate(ICommonSession _)
			{
				this.TileSpawningController.ToggleWindow();
			}, null, true, true));
			this._input.SetInputCommand(ContentKeyFunctions.OpenDecalSpawnWindow, InputCmdHandler.FromDelegate(delegate(ICommonSession _)
			{
				this.DecalPlacerController.ToggleWindow();
			}, null, true, true));
			CommandBinds.Builder.Bind(ContentKeyFunctions.EditorCopyObject, new PointerInputCmdHandler(new PointerInputCmdDelegate(this.Copy), true, false)).Register<SandboxSystem>();
		}

		// Token: 0x06000215 RID: 533 RVA: 0x0000E895 File Offset: 0x0000CA95
		public void UnloadButton()
		{
			if (this.SandboxButton == null)
			{
				return;
			}
			this.SandboxButton.OnPressed -= this.SandboxButtonPressed;
		}

		// Token: 0x06000216 RID: 534 RVA: 0x0000E8B7 File Offset: 0x0000CAB7
		public void LoadButton()
		{
			if (this.SandboxButton == null)
			{
				return;
			}
			this.SandboxButton.OnPressed += this.SandboxButtonPressed;
		}

		// Token: 0x06000217 RID: 535 RVA: 0x0000E8DC File Offset: 0x0000CADC
		private void EnsureWindow()
		{
			SandboxWindow window = this._window;
			if (window != null && !window.Disposed)
			{
				return;
			}
			this._window = this.UIManager.CreateWindow<SandboxWindow>();
			this._window.OnOpen += delegate()
			{
				this.SandboxButton.Pressed = true;
			};
			this._window.OnClose += delegate()
			{
				this.SandboxButton.Pressed = false;
			};
			this._window.ToggleLightButton.Pressed = !this._light.Enabled;
			this._window.ToggleFovButton.Pressed = !this._eye.CurrentEye.DrawFov;
			this._window.ToggleShadowsButton.Pressed = !this._light.DrawShadows;
			this._window.ToggleSubfloorButton.Pressed = this._subfloorHide.ShowAll;
			this._window.ShowMarkersButton.Pressed = this._marker.MarkersVisible;
			this._window.ShowBbButton.Pressed = ((this._debugPhysics.Flags & 4) > 0);
			this._window.RespawnButton.OnPressed += delegate(BaseButton.ButtonEventArgs _)
			{
				this._sandbox.Respawn();
			};
			this._window.SpawnTilesButton.OnPressed += delegate(BaseButton.ButtonEventArgs _)
			{
				this.TileSpawningController.ToggleWindow();
			};
			this._window.SpawnEntitiesButton.OnPressed += delegate(BaseButton.ButtonEventArgs _)
			{
				this.EntitySpawningController.ToggleWindow();
			};
			this._window.SpawnDecalsButton.OnPressed += delegate(BaseButton.ButtonEventArgs _)
			{
				this.DecalPlacerController.ToggleWindow();
			};
			this._window.GiveFullAccessButton.OnPressed += delegate(BaseButton.ButtonEventArgs _)
			{
				this._sandbox.GiveAdminAccess();
			};
			this._window.GiveAghostButton.OnPressed += delegate(BaseButton.ButtonEventArgs _)
			{
				this._sandbox.GiveAGhost();
			};
			this._window.ToggleLightButton.OnToggled += delegate(BaseButton.ButtonToggledEventArgs _)
			{
				this._sandbox.ToggleLight();
			};
			this._window.ToggleFovButton.OnToggled += delegate(BaseButton.ButtonToggledEventArgs _)
			{
				this._sandbox.ToggleFov();
			};
			this._window.ToggleShadowsButton.OnToggled += delegate(BaseButton.ButtonToggledEventArgs _)
			{
				this._sandbox.ToggleShadows();
			};
			this._window.SuicideButton.OnPressed += delegate(BaseButton.ButtonEventArgs _)
			{
				this._sandbox.Suicide();
			};
			this._window.ToggleSubfloorButton.OnPressed += delegate(BaseButton.ButtonEventArgs _)
			{
				this._sandbox.ToggleSubFloor();
			};
			this._window.ShowMarkersButton.OnPressed += delegate(BaseButton.ButtonEventArgs _)
			{
				this._sandbox.ShowMarkers();
			};
			this._window.ShowBbButton.OnPressed += delegate(BaseButton.ButtonEventArgs _)
			{
				this._sandbox.ShowBb();
			};
			this._window.MachineLinkingButton.OnPressed += delegate(BaseButton.ButtonEventArgs _)
			{
				this._sandbox.MachineLinking();
			};
		}

		// Token: 0x06000218 RID: 536 RVA: 0x0000EB78 File Offset: 0x0000CD78
		private void CheckSandboxVisibility()
		{
			if (this.SandboxButton == null)
			{
				return;
			}
			this.SandboxButton.Visible = this._sandbox.SandboxAllowed;
		}

		// Token: 0x06000219 RID: 537 RVA: 0x0000EB99 File Offset: 0x0000CD99
		public void OnStateExited(GameplayState state)
		{
			if (this._window != null)
			{
				this._window.Dispose();
				this._window = null;
			}
			CommandBinds.Unregister<SandboxSystem>();
		}

		// Token: 0x0600021A RID: 538 RVA: 0x0000EBBA File Offset: 0x0000CDBA
		public void OnSystemLoaded(SandboxSystem system)
		{
			system.SandboxDisabled += this.CloseAll;
			system.SandboxEnabled += this.CheckSandboxVisibility;
			system.SandboxDisabled += this.CheckSandboxVisibility;
		}

		// Token: 0x0600021B RID: 539 RVA: 0x0000EBF2 File Offset: 0x0000CDF2
		public void OnSystemUnloaded(SandboxSystem system)
		{
			system.SandboxDisabled -= this.CloseAll;
			system.SandboxEnabled -= this.CheckSandboxVisibility;
			system.SandboxDisabled -= this.CheckSandboxVisibility;
		}

		// Token: 0x0600021C RID: 540 RVA: 0x0000EC2A File Offset: 0x0000CE2A
		private void SandboxButtonPressed(BaseButton.ButtonEventArgs args)
		{
			this.ToggleWindow();
		}

		// Token: 0x0600021D RID: 541 RVA: 0x0000EC32 File Offset: 0x0000CE32
		private void CloseAll()
		{
			SandboxWindow window = this._window;
			if (window != null)
			{
				window.Close();
			}
			this.EntitySpawningController.CloseWindow();
			this.TileSpawningController.CloseWindow();
		}

		// Token: 0x0600021E RID: 542 RVA: 0x0000EC5B File Offset: 0x0000CE5B
		[NullableContext(2)]
		private bool Copy(ICommonSession session, EntityCoordinates coords, EntityUid uid)
		{
			return this._sandbox.Copy(session, coords, uid);
		}

		// Token: 0x0600021F RID: 543 RVA: 0x0000EC6B File Offset: 0x0000CE6B
		private void ToggleWindow()
		{
			if (this._window == null)
			{
				return;
			}
			if (this._sandbox.SandboxAllowed && !this._window.IsOpen)
			{
				this._window.OpenCentered();
				return;
			}
			this._window.Close();
		}

		// Token: 0x0400014D RID: 333
		[Dependency]
		private readonly IEyeManager _eye;

		// Token: 0x0400014E RID: 334
		[Dependency]
		private readonly IInputManager _input;

		// Token: 0x0400014F RID: 335
		[Dependency]
		private readonly ILightManager _light;

		// Token: 0x04000150 RID: 336
		[UISystemDependency]
		private readonly DebugPhysicsSystem _debugPhysics;

		// Token: 0x04000151 RID: 337
		[UISystemDependency]
		private readonly MarkerSystem _marker;

		// Token: 0x04000152 RID: 338
		[UISystemDependency]
		private readonly SandboxSystem _sandbox;

		// Token: 0x04000153 RID: 339
		[UISystemDependency]
		private readonly SubFloorHideSystem _subfloorHide;

		// Token: 0x04000154 RID: 340
		[Nullable(2)]
		private SandboxWindow _window;
	}
}
