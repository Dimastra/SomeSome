using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Content.Client.Decals.UI;
using Content.Client.Gameplay;
using Content.Client.Sandbox;
using Content.Shared.Decals;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.Controllers;
using Robust.Client.UserInterface.Controls;
using Robust.Shared.IoC;
using Robust.Shared.Prototypes;

namespace Content.Client.UserInterface.Systems.DecalPlacer
{
	// Token: 0x0200009C RID: 156
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class DecalPlacerUIController : UIController, IOnStateExited<GameplayState>, IOnSystemChanged<SandboxSystem>, IOnSystemLoaded<SandboxSystem>, IOnSystemUnloaded<SandboxSystem>
	{
		// Token: 0x060003B2 RID: 946 RVA: 0x00015CE2 File Offset: 0x00013EE2
		public void ToggleWindow()
		{
			this.EnsureWindow();
			if (this._window.IsOpen)
			{
				this._window.Close();
				return;
			}
			if (this._sandbox.SandboxAllowed)
			{
				this._window.Open();
			}
		}

		// Token: 0x060003B3 RID: 947 RVA: 0x00015D1B File Offset: 0x00013F1B
		public void OnStateExited(GameplayState state)
		{
			if (this._window == null)
			{
				return;
			}
			this._window.Dispose();
			this._window = null;
		}

		// Token: 0x060003B4 RID: 948 RVA: 0x00015D38 File Offset: 0x00013F38
		public void OnSystemLoaded(SandboxSystem system)
		{
			this._sandbox.SandboxDisabled += this.CloseWindow;
			this._prototypes.PrototypesReloaded += this.OnPrototypesReloaded;
		}

		// Token: 0x060003B5 RID: 949 RVA: 0x00015D68 File Offset: 0x00013F68
		public void OnSystemUnloaded(SandboxSystem system)
		{
			this._sandbox.SandboxDisabled -= this.CloseWindow;
			this._prototypes.PrototypesReloaded -= this.OnPrototypesReloaded;
		}

		// Token: 0x060003B6 RID: 950 RVA: 0x00015D98 File Offset: 0x00013F98
		private void OnPrototypesReloaded(PrototypesReloadedEventArgs obj)
		{
			this.ReloadPrototypes();
		}

		// Token: 0x060003B7 RID: 951 RVA: 0x00015DA0 File Offset: 0x00013FA0
		private void ReloadPrototypes()
		{
			if (this._window == null || this._window.Disposed)
			{
				return;
			}
			IEnumerable<DecalPrototype> prototypes = this._prototypes.EnumeratePrototypes<DecalPrototype>();
			this._window.Populate(prototypes);
		}

		// Token: 0x060003B8 RID: 952 RVA: 0x00015DDC File Offset: 0x00013FDC
		private void EnsureWindow()
		{
			DecalPlacerWindow window = this._window;
			if (window != null && !window.Disposed)
			{
				return;
			}
			this._window = this.UIManager.CreateWindow<DecalPlacerWindow>();
			LayoutContainer.SetAnchorPreset(this._window, 4, false);
			this.ReloadPrototypes();
		}

		// Token: 0x060003B9 RID: 953 RVA: 0x00015E20 File Offset: 0x00014020
		private void CloseWindow()
		{
			if (this._window == null || this._window.Disposed)
			{
				return;
			}
			this._window.Close();
		}

		// Token: 0x040001BF RID: 447
		[Dependency]
		private readonly IPrototypeManager _prototypes;

		// Token: 0x040001C0 RID: 448
		[UISystemDependency]
		private readonly SandboxSystem _sandbox;

		// Token: 0x040001C1 RID: 449
		[Nullable(2)]
		private DecalPlacerWindow _window;
	}
}
