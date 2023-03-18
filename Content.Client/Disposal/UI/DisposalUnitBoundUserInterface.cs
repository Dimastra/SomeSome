using System;
using System.Runtime.CompilerServices;
using Content.Client.Disposal.Components;
using Content.Client.Disposal.Systems;
using Content.Shared.Disposal;
using Content.Shared.Disposal.Components;
using Robust.Client.GameObjects;
using Robust.Client.UserInterface.Controls;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;

namespace Content.Client.Disposal.UI
{
	// Token: 0x0200034F RID: 847
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class DisposalUnitBoundUserInterface : BoundUserInterface
	{
		// Token: 0x060014F8 RID: 5368 RVA: 0x0007B136 File Offset: 0x00079336
		public DisposalUnitBoundUserInterface(ClientUserInterfaceComponent owner, Enum uiKey) : base(owner, uiKey)
		{
			IoCManager.InjectDependencies<DisposalUnitBoundUserInterface>(this);
		}

		// Token: 0x060014F9 RID: 5369 RVA: 0x0007B147 File Offset: 0x00079347
		private void ButtonPressed(SharedDisposalUnitComponent.UiButton button)
		{
			base.SendMessage(new SharedDisposalUnitComponent.UiButtonPressedMessage(button));
		}

		// Token: 0x060014FA RID: 5370 RVA: 0x0007B158 File Offset: 0x00079358
		private void TargetSelected(ItemList.ItemListSelectedEventArgs args)
		{
			ItemList.Item item = args.ItemList.get_IndexItem(args.ItemIndex);
			base.SendMessage(new TargetSelectedMessage(item.Text));
		}

		// Token: 0x060014FB RID: 5371 RVA: 0x0007B188 File Offset: 0x00079388
		protected override void Open()
		{
			base.Open();
			if (this.UiKey is MailingUnitUiKey)
			{
				this.MailingUnitWindow = new MailingUnitWindow();
				this.MailingUnitWindow.OpenCenteredRight();
				this.MailingUnitWindow.OnClose += base.Close;
				this.MailingUnitWindow.Eject.OnPressed += delegate(BaseButton.ButtonEventArgs _)
				{
					this.ButtonPressed(SharedDisposalUnitComponent.UiButton.Eject);
				};
				this.MailingUnitWindow.Engage.OnPressed += delegate(BaseButton.ButtonEventArgs _)
				{
					this.ButtonPressed(SharedDisposalUnitComponent.UiButton.Engage);
				};
				this.MailingUnitWindow.Power.OnPressed += delegate(BaseButton.ButtonEventArgs _)
				{
					this.ButtonPressed(SharedDisposalUnitComponent.UiButton.Power);
				};
				this.MailingUnitWindow.TargetListContainer.OnItemSelected += this.TargetSelected;
				return;
			}
			if (this.UiKey is SharedDisposalUnitComponent.DisposalUnitUiKey)
			{
				this.DisposalUnitWindow = new DisposalUnitWindow();
				this.DisposalUnitWindow.OpenCenteredRight();
				this.DisposalUnitWindow.OnClose += base.Close;
				this.DisposalUnitWindow.Eject.OnPressed += delegate(BaseButton.ButtonEventArgs _)
				{
					this.ButtonPressed(SharedDisposalUnitComponent.UiButton.Eject);
				};
				this.DisposalUnitWindow.Engage.OnPressed += delegate(BaseButton.ButtonEventArgs _)
				{
					this.ButtonPressed(SharedDisposalUnitComponent.UiButton.Engage);
				};
				this.DisposalUnitWindow.Power.OnPressed += delegate(BaseButton.ButtonEventArgs _)
				{
					this.ButtonPressed(SharedDisposalUnitComponent.UiButton.Power);
				};
			}
		}

		// Token: 0x060014FC RID: 5372 RVA: 0x0007B2DC File Offset: 0x000794DC
		protected override void UpdateState(BoundUserInterfaceState state)
		{
			base.UpdateState(state);
			if (!(state is MailingUnitBoundUserInterfaceState) && !(state is SharedDisposalUnitComponent.DisposalUnitBoundUserInterfaceState))
			{
				return;
			}
			EntityUid owner = base.Owner.Owner;
			DisposalUnitComponent disposalUnitComponent;
			if (!this._entityManager.TryGetComponent<DisposalUnitComponent>(owner, ref disposalUnitComponent))
			{
				return;
			}
			MailingUnitBoundUserInterfaceState mailingUnitBoundUserInterfaceState = state as MailingUnitBoundUserInterfaceState;
			if (mailingUnitBoundUserInterfaceState == null)
			{
				SharedDisposalUnitComponent.DisposalUnitBoundUserInterfaceState disposalUnitBoundUserInterfaceState = state as SharedDisposalUnitComponent.DisposalUnitBoundUserInterfaceState;
				if (disposalUnitBoundUserInterfaceState != null)
				{
					DisposalUnitWindow disposalUnitWindow = this.DisposalUnitWindow;
					if (disposalUnitWindow != null)
					{
						disposalUnitWindow.UpdateState(disposalUnitBoundUserInterfaceState);
					}
					disposalUnitComponent.UiState = disposalUnitBoundUserInterfaceState;
				}
			}
			else
			{
				MailingUnitWindow mailingUnitWindow = this.MailingUnitWindow;
				if (mailingUnitWindow != null)
				{
					mailingUnitWindow.UpdateState(mailingUnitBoundUserInterfaceState);
				}
				disposalUnitComponent.UiState = mailingUnitBoundUserInterfaceState.DisposalState;
			}
			this._entityManager.System<DisposalUnitSystem>().UpdateActive(owner, true);
		}

		// Token: 0x060014FD RID: 5373 RVA: 0x0007B381 File Offset: 0x00079581
		protected override void Dispose(bool disposing)
		{
			base.Dispose(disposing);
			if (!disposing)
			{
				return;
			}
			MailingUnitWindow mailingUnitWindow = this.MailingUnitWindow;
			if (mailingUnitWindow != null)
			{
				mailingUnitWindow.Dispose();
			}
			DisposalUnitWindow disposalUnitWindow = this.DisposalUnitWindow;
			if (disposalUnitWindow == null)
			{
				return;
			}
			disposalUnitWindow.Dispose();
		}

		// Token: 0x060014FE RID: 5374 RVA: 0x0007B3B0 File Offset: 0x000795B0
		public bool? UpdateWindowState(SharedDisposalUnitComponent.DisposalUnitBoundUserInterfaceState state)
		{
			if (!(this.UiKey is SharedDisposalUnitComponent.DisposalUnitUiKey))
			{
				MailingUnitWindow mailingUnitWindow = this.MailingUnitWindow;
				if (mailingUnitWindow == null)
				{
					return null;
				}
				return new bool?(mailingUnitWindow.UpdatePressure(state.FullPressureTime));
			}
			else
			{
				DisposalUnitWindow disposalUnitWindow = this.DisposalUnitWindow;
				if (disposalUnitWindow == null)
				{
					return null;
				}
				return new bool?(disposalUnitWindow.UpdateState(state));
			}
		}

		// Token: 0x04000AE4 RID: 2788
		[Dependency]
		private readonly IEntityManager _entityManager;

		// Token: 0x04000AE5 RID: 2789
		[Nullable(2)]
		public MailingUnitWindow MailingUnitWindow;

		// Token: 0x04000AE6 RID: 2790
		[Nullable(2)]
		public DisposalUnitWindow DisposalUnitWindow;
	}
}
