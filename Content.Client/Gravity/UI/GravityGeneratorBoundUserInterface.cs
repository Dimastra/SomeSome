using System;
using System.Runtime.CompilerServices;
using Content.Shared.Gravity;
using Robust.Client.GameObjects;
using Robust.Shared.GameObjects;

namespace Content.Client.Gravity.UI
{
	// Token: 0x020002FB RID: 763
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class GravityGeneratorBoundUserInterface : BoundUserInterface
	{
		// Token: 0x06001322 RID: 4898 RVA: 0x000021BC File Offset: 0x000003BC
		public GravityGeneratorBoundUserInterface(ClientUserInterfaceComponent owner, Enum uiKey) : base(owner, uiKey)
		{
		}

		// Token: 0x06001323 RID: 4899 RVA: 0x00071CEA File Offset: 0x0006FEEA
		protected override void Open()
		{
			base.Open();
			this._window = new GravityGeneratorWindow(this, base.Owner);
			this._window.OpenCentered();
			this._window.OnClose += base.Close;
		}

		// Token: 0x06001324 RID: 4900 RVA: 0x00071D28 File Offset: 0x0006FF28
		protected override void UpdateState(BoundUserInterfaceState state)
		{
			base.UpdateState(state);
			SharedGravityGeneratorComponent.GeneratorState state2 = (SharedGravityGeneratorComponent.GeneratorState)state;
			GravityGeneratorWindow window = this._window;
			if (window == null)
			{
				return;
			}
			window.UpdateState(state2);
		}

		// Token: 0x06001325 RID: 4901 RVA: 0x00071D54 File Offset: 0x0006FF54
		protected override void Dispose(bool disposing)
		{
			base.Dispose(disposing);
			if (!disposing)
			{
				return;
			}
			GravityGeneratorWindow window = this._window;
			if (window == null)
			{
				return;
			}
			window.Dispose();
		}

		// Token: 0x06001326 RID: 4902 RVA: 0x00071D71 File Offset: 0x0006FF71
		public void SetPowerSwitch(bool on)
		{
			base.SendMessage(new SharedGravityGeneratorComponent.SwitchGeneratorMessage(on));
		}

		// Token: 0x04000997 RID: 2455
		[Nullable(2)]
		private GravityGeneratorWindow _window;
	}
}
