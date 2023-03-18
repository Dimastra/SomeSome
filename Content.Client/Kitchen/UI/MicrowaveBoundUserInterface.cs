using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Content.Shared.Chemistry.Components;
using Content.Shared.Kitchen.Components;
using Robust.Client.GameObjects;
using Robust.Client.Graphics;
using Robust.Client.UserInterface.Controls;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Localization;

namespace Content.Client.Kitchen.UI
{
	// Token: 0x02000293 RID: 659
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class MicrowaveBoundUserInterface : BoundUserInterface
	{
		// Token: 0x060010BB RID: 4283 RVA: 0x000643DB File Offset: 0x000625DB
		public MicrowaveBoundUserInterface(ClientUserInterfaceComponent owner, Enum uiKey) : base(owner, uiKey)
		{
		}

		// Token: 0x060010BC RID: 4284 RVA: 0x000643FC File Offset: 0x000625FC
		protected override void Open()
		{
			base.Open();
			this._menu = new MicrowaveMenu(this);
			this._menu.OpenCentered();
			this._menu.OnClose += base.Close;
			this._menu.StartButton.OnPressed += delegate(BaseButton.ButtonEventArgs _)
			{
				base.SendMessage(new MicrowaveStartCookMessage());
			};
			this._menu.EjectButton.OnPressed += delegate(BaseButton.ButtonEventArgs _)
			{
				base.SendMessage(new MicrowaveEjectMessage());
			};
			this._menu.IngredientsList.OnItemSelected += delegate(ItemList.ItemListSelectedEventArgs args)
			{
				base.SendMessage(new MicrowaveEjectSolidIndexedMessage(this._solids[args.ItemIndex]));
			};
			this._menu.OnCookTimeSelected += delegate(BaseButton.ButtonEventArgs args, int buttonIndex)
			{
				MicrowaveMenu.MicrowaveCookTimeButton microwaveCookTimeButton = (MicrowaveMenu.MicrowaveCookTimeButton)args.Button;
				base.SendMessage(new MicrowaveSelectCookTimeMessage(buttonIndex, microwaveCookTimeButton.CookTime));
			};
		}

		// Token: 0x060010BD RID: 4285 RVA: 0x000644A8 File Offset: 0x000626A8
		protected override void Dispose(bool disposing)
		{
			base.Dispose(disposing);
			if (!disposing)
			{
				return;
			}
			this._solids.Clear();
			MicrowaveMenu menu = this._menu;
			if (menu == null)
			{
				return;
			}
			menu.Dispose();
		}

		// Token: 0x060010BE RID: 4286 RVA: 0x000644D0 File Offset: 0x000626D0
		protected override void UpdateState(BoundUserInterfaceState state)
		{
			base.UpdateState(state);
			MicrowaveUpdateUserInterfaceState microwaveUpdateUserInterfaceState = state as MicrowaveUpdateUserInterfaceState;
			if (microwaveUpdateUserInterfaceState == null)
			{
				return;
			}
			MicrowaveMenu menu = this._menu;
			if (menu != null)
			{
				menu.ToggleBusyDisableOverlayPanel(microwaveUpdateUserInterfaceState.IsMicrowaveBusy);
			}
			this.RefreshContentsDisplay(microwaveUpdateUserInterfaceState.ContainedSolids);
			if (this._menu == null)
			{
				return;
			}
			((Button)this._menu.CookTimeButtonVbox.GetChild(microwaveUpdateUserInterfaceState.ActiveButtonIndex)).Pressed = true;
			string item = (microwaveUpdateUserInterfaceState.ActiveButtonIndex == 0) ? Loc.GetString("microwave-menu-instant-button") : microwaveUpdateUserInterfaceState.CurrentCookTime.ToString();
			this._menu.CookTimeInfoLabel.Text = Loc.GetString("microwave-bound-user-interface-cook-time-label", new ValueTuple<string, object>[]
			{
				new ValueTuple<string, object>("time", item)
			});
		}

		// Token: 0x060010BF RID: 4287 RVA: 0x00064590 File Offset: 0x00062790
		private void RefreshContentsDisplay(EntityUid[] containedSolids)
		{
			this._reagents.Clear();
			if (this._menu == null)
			{
				return;
			}
			this._solids.Clear();
			this._menu.IngredientsList.Clear();
			int i = 0;
			while (i < containedSolids.Length)
			{
				EntityUid entityUid = containedSolids[i];
				if (this._entityManager.Deleted(entityUid))
				{
					return;
				}
				IconComponent iconComponent;
				Texture texture;
				if (this._entityManager.TryGetComponent<IconComponent>(entityUid, ref iconComponent))
				{
					IDirectionalTextureProvider icon = iconComponent.Icon;
					texture = ((icon != null) ? icon.Default : null);
					goto IL_99;
				}
				SpriteComponent spriteComponent;
				if (this._entityManager.TryGetComponent<SpriteComponent>(entityUid, ref spriteComponent))
				{
					IRsiStateLike icon2 = spriteComponent.Icon;
					texture = ((icon2 != null) ? icon2.Default : null);
					goto IL_99;
				}
				IL_E0:
				i++;
				continue;
				IL_99:
				ItemList.Item item = this._menu.IngredientsList.AddItem(this._entityManager.GetComponent<MetaDataComponent>(entityUid).EntityName, texture, true);
				int key = this._menu.IngredientsList.IndexOf(item);
				this._solids.Add(key, entityUid);
				goto IL_E0;
			}
		}

		// Token: 0x0400083D RID: 2109
		[Dependency]
		private readonly IEntityManager _entityManager;

		// Token: 0x0400083E RID: 2110
		[Nullable(2)]
		private MicrowaveMenu _menu;

		// Token: 0x0400083F RID: 2111
		private readonly Dictionary<int, EntityUid> _solids = new Dictionary<int, EntityUid>();

		// Token: 0x04000840 RID: 2112
		private readonly Dictionary<int, Solution.ReagentQuantity> _reagents = new Dictionary<int, Solution.ReagentQuantity>();
	}
}
