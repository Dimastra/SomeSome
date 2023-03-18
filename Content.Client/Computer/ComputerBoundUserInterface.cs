using System;
using System.Runtime.CompilerServices;
using Robust.Client.GameObjects;
using Robust.Client.UserInterface.CustomControls;
using Robust.Shared.Analyzers;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;

namespace Content.Client.Computer
{
	// Token: 0x02000399 RID: 921
	[NullableContext(1)]
	[Nullable(0)]
	[Virtual]
	public class ComputerBoundUserInterface<[Nullable(0)] TWindow, [Nullable(0)] TState> : ComputerBoundUserInterfaceBase where TWindow : BaseWindow, IComputerWindow<TState>, new() where TState : BoundUserInterfaceState
	{
		// Token: 0x060016F0 RID: 5872 RVA: 0x00085AC8 File Offset: 0x00083CC8
		protected override void Open()
		{
			base.Open();
			this._window = (TWindow)((object)this._dynamicTypeFactory.CreateInstance(typeof(TWindow)));
			this._window.SetupComputerWindow(this);
			this._window.OnClose += base.Close;
			this._window.OpenCentered();
		}

		// Token: 0x060016F1 RID: 5873 RVA: 0x00085B38 File Offset: 0x00083D38
		public ComputerBoundUserInterface(ClientUserInterfaceComponent owner, Enum uiKey) : base(owner, uiKey)
		{
		}

		// Token: 0x060016F2 RID: 5874 RVA: 0x00085B42 File Offset: 0x00083D42
		protected override void UpdateState(BoundUserInterfaceState state)
		{
			base.UpdateState(state);
			if (this._window == null)
			{
				return;
			}
			this._window.UpdateState((TState)((object)state));
		}

		// Token: 0x060016F3 RID: 5875 RVA: 0x00085B6F File Offset: 0x00083D6F
		protected override void Dispose(bool disposing)
		{
			base.Dispose(disposing);
			if (disposing)
			{
				TWindow twindow = this._window;
				if (twindow == null)
				{
					return;
				}
				twindow.Dispose();
			}
		}

		// Token: 0x04000BEC RID: 3052
		[Dependency]
		private readonly IDynamicTypeFactory _dynamicTypeFactory;

		// Token: 0x04000BED RID: 3053
		[Nullable(2)]
		private TWindow _window;
	}
}
