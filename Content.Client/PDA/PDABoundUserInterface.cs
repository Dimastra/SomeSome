using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Content.Client.CartridgeLoader;
using Content.Shared.CartridgeLoader;
using Content.Shared.CCVar;
using Content.Shared.Containers.ItemSlots;
using Content.Shared.CrewManifest;
using Content.Shared.PDA;
using Robust.Client.GameObjects;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.Controls;
using Robust.Shared.Configuration;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Localization;

namespace Content.Client.PDA
{
	// Token: 0x020001BD RID: 445
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class PDABoundUserInterface : CartridgeLoaderBoundUserInterface
	{
		// Token: 0x06000B69 RID: 2921 RVA: 0x0004238B File Offset: 0x0004058B
		public PDABoundUserInterface(ClientUserInterfaceComponent owner, Enum uiKey) : base(owner, uiKey)
		{
			IoCManager.InjectDependencies<PDABoundUserInterface>(this);
		}

		// Token: 0x06000B6A RID: 2922 RVA: 0x0004239C File Offset: 0x0004059C
		protected override void Open()
		{
			base.Open();
			base.SendMessage(new PDARequestUpdateInterfaceMessage());
			this._menu = new PDAMenu();
			this._menu.OpenCenteredLeft();
			this._menu.OnClose += base.Close;
			this._menu.FlashLightToggleButton.OnToggled += delegate(BaseButton.ButtonToggledEventArgs _)
			{
				base.SendMessage(new PDAToggleFlashlightMessage());
			};
			if (this._configManager.GetCVar<bool>(CCVars.CrewManifestUnsecure))
			{
				this._menu.CrewManifestButton.Visible = true;
				this._menu.CrewManifestButton.OnPressed += delegate(BaseButton.ButtonEventArgs _)
				{
					base.SendMessage(new CrewManifestOpenUiMessage());
				};
			}
			this._menu.EjectIdButton.OnPressed += delegate(BaseButton.ButtonEventArgs _)
			{
				base.SendMessage(new ItemSlotButtonPressedEvent("PDA-id", true, true));
			};
			this._menu.EjectPenButton.OnPressed += delegate(BaseButton.ButtonEventArgs _)
			{
				base.SendMessage(new ItemSlotButtonPressedEvent("PDA-pen", true, true));
			};
			this._menu.ActivateUplinkButton.OnPressed += delegate(BaseButton.ButtonEventArgs _)
			{
				base.SendMessage(new PDAShowUplinkMessage());
			};
			this._menu.ActivateMusicButton.OnPressed += delegate(BaseButton.ButtonEventArgs _)
			{
				base.SendMessage(new PDAShowMusicMessage());
			};
			this._menu.AccessRingtoneButton.OnPressed += delegate(BaseButton.ButtonEventArgs _)
			{
				base.SendMessage(new PDAShowRingtoneMessage());
			};
			this._menu.OnProgramItemPressed += base.ActivateCartridge;
			this._menu.OnInstallButtonPressed += base.InstallCartridge;
			this._menu.OnUninstallButtonPressed += base.UninstallCartridge;
			this._menu.ProgramCloseButton.OnPressed += delegate(BaseButton.ButtonEventArgs _)
			{
				base.DeactivateActiveCartridge();
			};
			PDABorderColorComponent borderColorComponent = this.GetBorderColorComponent();
			if (borderColorComponent == null)
			{
				return;
			}
			this._menu.BorderColor = borderColorComponent.BorderColor;
			this._menu.AccentHColor = borderColorComponent.AccentHColor;
			this._menu.AccentVColor = borderColorComponent.AccentVColor;
		}

		// Token: 0x06000B6B RID: 2923 RVA: 0x00042570 File Offset: 0x00040770
		protected override void UpdateState(BoundUserInterfaceState state)
		{
			base.UpdateState(state);
			PDAUpdateState pdaupdateState = state as PDAUpdateState;
			if (pdaupdateState == null)
			{
				return;
			}
			PDAMenu menu = this._menu;
			if (menu == null)
			{
				return;
			}
			menu.UpdateState(pdaupdateState);
		}

		// Token: 0x06000B6C RID: 2924 RVA: 0x000425A0 File Offset: 0x000407A0
		protected override void AttachCartridgeUI(Control cartridgeUIFragment, [Nullable(2)] string title)
		{
			PDAMenu menu = this._menu;
			if (menu != null)
			{
				menu.ProgramView.AddChild(cartridgeUIFragment);
			}
			PDAMenu menu2 = this._menu;
			if (menu2 == null)
			{
				return;
			}
			menu2.ToProgramView(title ?? Loc.GetString("comp-pda-io-program-fallback-title"));
		}

		// Token: 0x06000B6D RID: 2925 RVA: 0x000425D8 File Offset: 0x000407D8
		protected override void DetachCartridgeUI(Control cartridgeUIFragment)
		{
			if (this._menu == null)
			{
				return;
			}
			this._menu.ToHomeScreen();
			this._menu.HideProgramHeader();
			this._menu.ProgramView.RemoveChild(cartridgeUIFragment);
		}

		// Token: 0x06000B6E RID: 2926 RVA: 0x0004260A File Offset: 0x0004080A
		protected override void UpdateAvailablePrograms([Nullable(new byte[]
		{
			1,
			0,
			1
		})] List<ValueTuple<EntityUid, CartridgeComponent>> programs)
		{
			PDAMenu menu = this._menu;
			if (menu == null)
			{
				return;
			}
			menu.UpdateAvailablePrograms(programs);
		}

		// Token: 0x06000B6F RID: 2927 RVA: 0x0004261D File Offset: 0x0004081D
		protected override void Dispose(bool disposing)
		{
			base.Dispose(disposing);
			if (!disposing)
			{
				return;
			}
			PDAMenu menu = this._menu;
			if (menu == null)
			{
				return;
			}
			menu.Dispose();
		}

		// Token: 0x06000B70 RID: 2928 RVA: 0x0004263A File Offset: 0x0004083A
		[NullableContext(2)]
		private PDABorderColorComponent GetBorderColorComponent()
		{
			IEntityManager entityManager = this._entityManager;
			if (entityManager == null)
			{
				return null;
			}
			return EntityManagerExt.GetComponentOrNull<PDABorderColorComponent>(entityManager, base.Owner.Owner);
		}

		// Token: 0x04000594 RID: 1428
		[Nullable(2)]
		[Dependency]
		private readonly IEntityManager _entityManager;

		// Token: 0x04000595 RID: 1429
		[Dependency]
		private readonly IConfigurationManager _configManager;

		// Token: 0x04000596 RID: 1430
		[Nullable(2)]
		private PDAMenu _menu;
	}
}
