using System;
using System.Runtime.CompilerServices;
using Content.Shared.Singularity.Components;
using Robust.Client.GameObjects;
using Robust.Shared.GameObjects;

namespace Content.Client.ParticleAccelerator.UI
{
	// Token: 0x020001CB RID: 459
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class ParticleAcceleratorBoundUserInterface : BoundUserInterface
	{
		// Token: 0x06000C13 RID: 3091 RVA: 0x000021BC File Offset: 0x000003BC
		public ParticleAcceleratorBoundUserInterface(ClientUserInterfaceComponent owner, Enum uiKey) : base(owner, uiKey)
		{
		}

		// Token: 0x06000C14 RID: 3092 RVA: 0x00045DAB File Offset: 0x00043FAB
		protected override void Open()
		{
			base.Open();
			this._menu = new ParticleAcceleratorControlMenu(this);
			this._menu.OnClose += base.Close;
			this._menu.OpenCentered();
		}

		// Token: 0x06000C15 RID: 3093 RVA: 0x00045DE1 File Offset: 0x00043FE1
		public void SendEnableMessage(bool enable)
		{
			base.SendMessage(new ParticleAcceleratorSetEnableMessage(enable));
		}

		// Token: 0x06000C16 RID: 3094 RVA: 0x00045DEF File Offset: 0x00043FEF
		public void SendPowerStateMessage(ParticleAcceleratorPowerState state)
		{
			base.SendMessage(new ParticleAcceleratorSetPowerStateMessage(state));
		}

		// Token: 0x06000C17 RID: 3095 RVA: 0x00045DFD File Offset: 0x00043FFD
		public void SendScanPartsMessage()
		{
			base.SendMessage(new ParticleAcceleratorRescanPartsMessage());
		}

		// Token: 0x06000C18 RID: 3096 RVA: 0x00045E0A File Offset: 0x0004400A
		protected override void UpdateState(BoundUserInterfaceState state)
		{
			ParticleAcceleratorControlMenu menu = this._menu;
			if (menu == null)
			{
				return;
			}
			menu.DataUpdate((ParticleAcceleratorUIState)state);
		}

		// Token: 0x06000C19 RID: 3097 RVA: 0x00045E22 File Offset: 0x00044022
		protected override void Dispose(bool disposing)
		{
			base.Dispose(disposing);
			ParticleAcceleratorControlMenu menu = this._menu;
			if (menu != null)
			{
				menu.Dispose();
			}
			this._menu = null;
		}

		// Token: 0x040005C4 RID: 1476
		[Nullable(2)]
		private ParticleAcceleratorControlMenu _menu;
	}
}
