using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Content.Client.UserInterface.Fragments;
using Content.Shared.CartridgeLoader;
using Robust.Client.GameObjects;
using Robust.Client.UserInterface;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Localization;

namespace Content.Client.CartridgeLoader
{
	// Token: 0x020003FA RID: 1018
	[NullableContext(1)]
	[Nullable(0)]
	public abstract class CartridgeLoaderBoundUserInterface : BoundUserInterface
	{
		// Token: 0x060018FD RID: 6397 RVA: 0x0008F7A4 File Offset: 0x0008D9A4
		protected CartridgeLoaderBoundUserInterface(ClientUserInterfaceComponent owner, Enum uiKey) : base(owner, uiKey)
		{
			IoCManager.InjectDependencies<CartridgeLoaderBoundUserInterface>(this);
		}

		// Token: 0x060018FE RID: 6398 RVA: 0x0008F7B8 File Offset: 0x0008D9B8
		protected override void UpdateState(BoundUserInterfaceState state)
		{
			base.UpdateState(state);
			CartridgeLoaderUiState cartridgeLoaderUiState = state as CartridgeLoaderUiState;
			if (cartridgeLoaderUiState == null)
			{
				UIFragment activeCartridgeUI = this._activeCartridgeUI;
				if (activeCartridgeUI == null)
				{
					return;
				}
				activeCartridgeUI.UpdateState(state);
				return;
			}
			else
			{
				List<ValueTuple<EntityUid, CartridgeComponent>> cartridgeComponents = this.GetCartridgeComponents(cartridgeLoaderUiState.Programs);
				this.UpdateAvailablePrograms(cartridgeComponents);
				this._activeProgram = cartridgeLoaderUiState.ActiveUI;
				UIFragment uifragment = this.RetrieveCartridgeUI(cartridgeLoaderUiState.ActiveUI);
				CartridgeComponent cartridgeComponent = this.RetrieveCartridgeComponent(cartridgeLoaderUiState.ActiveUI);
				Control control = (uifragment != null) ? uifragment.GetUIFragmentRoot() : null;
				Control activeUiFragment = this._activeUiFragment;
				if (((activeUiFragment != null) ? activeUiFragment.GetType() : null) == ((control != null) ? control.GetType() : null))
				{
					return;
				}
				if (this._activeUiFragment != null)
				{
					this.DetachCartridgeUI(this._activeUiFragment);
				}
				if (control != null && this._activeProgram != null)
				{
					this.AttachCartridgeUI(control, Loc.GetString(((cartridgeComponent != null) ? cartridgeComponent.ProgramName : null) ?? "default-program-name"));
					this.SendCartridgeUiReadyEvent(this._activeProgram.Value);
				}
				this._activeCartridgeUI = uifragment;
				Control activeUiFragment2 = this._activeUiFragment;
				if (activeUiFragment2 != null)
				{
					activeUiFragment2.Dispose();
				}
				this._activeUiFragment = control;
				return;
			}
		}

		// Token: 0x060018FF RID: 6399 RVA: 0x0008F8D0 File Offset: 0x0008DAD0
		protected void ActivateCartridge(EntityUid cartridgeUid)
		{
			CartridgeLoaderUiMessage cartridgeLoaderUiMessage = new CartridgeLoaderUiMessage(cartridgeUid, CartridgeUiMessageAction.Activate);
			base.SendMessage(cartridgeLoaderUiMessage);
		}

		// Token: 0x06001900 RID: 6400 RVA: 0x0008F8EC File Offset: 0x0008DAEC
		protected void DeactivateActiveCartridge()
		{
			if (this._activeProgram == null)
			{
				return;
			}
			CartridgeLoaderUiMessage cartridgeLoaderUiMessage = new CartridgeLoaderUiMessage(this._activeProgram.Value, CartridgeUiMessageAction.Deactivate);
			base.SendMessage(cartridgeLoaderUiMessage);
		}

		// Token: 0x06001901 RID: 6401 RVA: 0x0008F920 File Offset: 0x0008DB20
		protected void InstallCartridge(EntityUid cartridgeUid)
		{
			CartridgeLoaderUiMessage cartridgeLoaderUiMessage = new CartridgeLoaderUiMessage(cartridgeUid, CartridgeUiMessageAction.Install);
			base.SendMessage(cartridgeLoaderUiMessage);
		}

		// Token: 0x06001902 RID: 6402 RVA: 0x0008F93C File Offset: 0x0008DB3C
		protected void UninstallCartridge(EntityUid cartridgeUid)
		{
			CartridgeLoaderUiMessage cartridgeLoaderUiMessage = new CartridgeLoaderUiMessage(cartridgeUid, CartridgeUiMessageAction.Uninstall);
			base.SendMessage(cartridgeLoaderUiMessage);
		}

		// Token: 0x06001903 RID: 6403 RVA: 0x0008F958 File Offset: 0x0008DB58
		[return: Nullable(new byte[]
		{
			1,
			0,
			1
		})]
		private List<ValueTuple<EntityUid, CartridgeComponent>> GetCartridgeComponents(List<EntityUid> programs)
		{
			List<ValueTuple<EntityUid, CartridgeComponent>> list = new List<ValueTuple<EntityUid, CartridgeComponent>>();
			foreach (EntityUid entityUid in programs)
			{
				CartridgeComponent cartridgeComponent = this.RetrieveCartridgeComponent(new EntityUid?(entityUid));
				if (cartridgeComponent != null)
				{
					list.Add(new ValueTuple<EntityUid, CartridgeComponent>(entityUid, cartridgeComponent));
				}
			}
			return list;
		}

		// Token: 0x06001904 RID: 6404
		protected abstract void AttachCartridgeUI(Control cartridgeUIFragment, [Nullable(2)] string title);

		// Token: 0x06001905 RID: 6405
		protected abstract void DetachCartridgeUI(Control cartridgeUIFragment);

		// Token: 0x06001906 RID: 6406
		protected abstract void UpdateAvailablePrograms([Nullable(new byte[]
		{
			1,
			0,
			1
		})] List<ValueTuple<EntityUid, CartridgeComponent>> programs);

		// Token: 0x06001907 RID: 6407 RVA: 0x0008F9C4 File Offset: 0x0008DBC4
		protected override void Dispose(bool disposing)
		{
			base.Dispose(disposing);
			if (disposing)
			{
				Control activeUiFragment = this._activeUiFragment;
				if (activeUiFragment == null)
				{
					return;
				}
				activeUiFragment.Dispose();
			}
		}

		// Token: 0x06001908 RID: 6408 RVA: 0x0008F9E0 File Offset: 0x0008DBE0
		[NullableContext(2)]
		protected CartridgeComponent RetrieveCartridgeComponent(EntityUid? cartridgeUid)
		{
			IEntityManager entityManager = this._entityManager;
			if (entityManager == null)
			{
				return null;
			}
			return EntityManagerExt.GetComponentOrNull<CartridgeComponent>(entityManager, cartridgeUid);
		}

		// Token: 0x06001909 RID: 6409 RVA: 0x0008F9F4 File Offset: 0x0008DBF4
		private void SendCartridgeUiReadyEvent(EntityUid cartridgeUid)
		{
			CartridgeLoaderUiMessage cartridgeLoaderUiMessage = new CartridgeLoaderUiMessage(cartridgeUid, CartridgeUiMessageAction.UIReady);
			base.SendMessage(cartridgeLoaderUiMessage);
		}

		// Token: 0x0600190A RID: 6410 RVA: 0x0008FA10 File Offset: 0x0008DC10
		[NullableContext(2)]
		private UIFragment RetrieveCartridgeUI(EntityUid? cartridgeUid)
		{
			IEntityManager entityManager = this._entityManager;
			UIFragmentComponent uifragmentComponent = (entityManager != null) ? EntityManagerExt.GetComponentOrNull<UIFragmentComponent>(entityManager, cartridgeUid) : null;
			if (uifragmentComponent != null)
			{
				UIFragment ui = uifragmentComponent.Ui;
				if (ui != null)
				{
					ui.Setup(this, cartridgeUid);
				}
			}
			if (uifragmentComponent == null)
			{
				return null;
			}
			return uifragmentComponent.Ui;
		}

		// Token: 0x04000CCB RID: 3275
		[Nullable(2)]
		[Dependency]
		private readonly IEntityManager _entityManager;

		// Token: 0x04000CCC RID: 3276
		private EntityUid? _activeProgram;

		// Token: 0x04000CCD RID: 3277
		[Nullable(2)]
		private UIFragment _activeCartridgeUI;

		// Token: 0x04000CCE RID: 3278
		[Nullable(2)]
		private Control _activeUiFragment;
	}
}
