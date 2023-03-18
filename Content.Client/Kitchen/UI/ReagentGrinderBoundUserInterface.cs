using System;
using System.Runtime.CompilerServices;
using Content.Shared.Containers.ItemSlots;
using Content.Shared.Kitchen;
using Robust.Client.GameObjects;
using Robust.Client.UserInterface.Controls;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Prototypes;

namespace Content.Client.Kitchen.UI
{
	// Token: 0x02000297 RID: 663
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class ReagentGrinderBoundUserInterface : BoundUserInterface
	{
		// Token: 0x060010D4 RID: 4308 RVA: 0x000021BC File Offset: 0x000003BC
		public ReagentGrinderBoundUserInterface(ClientUserInterfaceComponent owner, Enum uiKey) : base(owner, uiKey)
		{
		}

		// Token: 0x060010D5 RID: 4309 RVA: 0x00064E14 File Offset: 0x00063014
		protected override void Open()
		{
			base.Open();
			this._menu = new GrinderMenu(this, this._entityManager, this._prototypeManager);
			this._menu.OpenCentered();
			this._menu.OnClose += base.Close;
		}

		// Token: 0x060010D6 RID: 4310 RVA: 0x00064E61 File Offset: 0x00063061
		protected override void Dispose(bool disposing)
		{
			base.Dispose(disposing);
			if (!disposing)
			{
				return;
			}
			GrinderMenu menu = this._menu;
			if (menu == null)
			{
				return;
			}
			menu.Dispose();
		}

		// Token: 0x060010D7 RID: 4311 RVA: 0x00064E80 File Offset: 0x00063080
		protected override void UpdateState(BoundUserInterfaceState state)
		{
			base.UpdateState(state);
			ReagentGrinderInterfaceState reagentGrinderInterfaceState = state as ReagentGrinderInterfaceState;
			if (reagentGrinderInterfaceState == null)
			{
				return;
			}
			GrinderMenu menu = this._menu;
			if (menu == null)
			{
				return;
			}
			menu.UpdateState(reagentGrinderInterfaceState);
		}

		// Token: 0x060010D8 RID: 4312 RVA: 0x00064EB0 File Offset: 0x000630B0
		protected override void ReceiveMessage(BoundUserInterfaceMessage message)
		{
			base.ReceiveMessage(message);
			GrinderMenu menu = this._menu;
			if (menu == null)
			{
				return;
			}
			menu.HandleMessage(message);
		}

		// Token: 0x060010D9 RID: 4313 RVA: 0x00064ECA File Offset: 0x000630CA
		[NullableContext(2)]
		public void StartGrinding(BaseButton.ButtonEventArgs args = null)
		{
			base.SendMessage(new ReagentGrinderStartMessage(GrinderProgram.Grind));
		}

		// Token: 0x060010DA RID: 4314 RVA: 0x00064ED8 File Offset: 0x000630D8
		[NullableContext(2)]
		public void StartJuicing(BaseButton.ButtonEventArgs args = null)
		{
			base.SendMessage(new ReagentGrinderStartMessage(GrinderProgram.Juice));
		}

		// Token: 0x060010DB RID: 4315 RVA: 0x00064EE6 File Offset: 0x000630E6
		[NullableContext(2)]
		public void EjectAll(BaseButton.ButtonEventArgs args = null)
		{
			base.SendMessage(new ReagentGrinderEjectChamberAllMessage());
		}

		// Token: 0x060010DC RID: 4316 RVA: 0x00064EF3 File Offset: 0x000630F3
		[NullableContext(2)]
		public void EjectBeaker(BaseButton.ButtonEventArgs args = null)
		{
			base.SendMessage(new ItemSlotButtonPressedEvent(SharedReagentGrinder.BeakerSlotId, true, true));
		}

		// Token: 0x060010DD RID: 4317 RVA: 0x00064F07 File Offset: 0x00063107
		public void EjectChamberContent(EntityUid uid)
		{
			base.SendMessage(new ReagentGrinderEjectChamberContentMessage(uid));
		}

		// Token: 0x04000846 RID: 2118
		[Dependency]
		private readonly IEntityManager _entityManager;

		// Token: 0x04000847 RID: 2119
		[Dependency]
		private readonly IPrototypeManager _prototypeManager;

		// Token: 0x04000848 RID: 2120
		[Nullable(2)]
		private GrinderMenu _menu;
	}
}
