using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using Content.Shared.Configurable;
using Robust.Client.GameObjects;
using Robust.Shared.GameObjects;

namespace Content.Client.Configurable.UI
{
	// Token: 0x02000397 RID: 919
	[NullableContext(2)]
	[Nullable(0)]
	public sealed class ConfigurationBoundUserInterface : BoundUserInterface
	{
		// Token: 0x17000491 RID: 1169
		// (get) Token: 0x060016E1 RID: 5857 RVA: 0x000855D1 File Offset: 0x000837D1
		// (set) Token: 0x060016E2 RID: 5858 RVA: 0x000855D9 File Offset: 0x000837D9
		public Regex Validation { get; internal set; }

		// Token: 0x060016E3 RID: 5859 RVA: 0x000021BC File Offset: 0x000003BC
		[NullableContext(1)]
		public ConfigurationBoundUserInterface(ClientUserInterfaceComponent owner, Enum uiKey) : base(owner, uiKey)
		{
		}

		// Token: 0x060016E4 RID: 5860 RVA: 0x000855E2 File Offset: 0x000837E2
		protected override void Open()
		{
			base.Open();
			this._menu = new ConfigurationMenu(this);
			this._menu.OnClose += base.Close;
			this._menu.OpenCentered();
		}

		// Token: 0x060016E5 RID: 5861 RVA: 0x00085618 File Offset: 0x00083818
		[NullableContext(1)]
		protected override void UpdateState(BoundUserInterfaceState state)
		{
			base.UpdateState(state);
			ConfigurationComponent.ConfigurationBoundUserInterfaceState configurationBoundUserInterfaceState = state as ConfigurationComponent.ConfigurationBoundUserInterfaceState;
			if (configurationBoundUserInterfaceState == null)
			{
				return;
			}
			ConfigurationMenu menu = this._menu;
			if (menu == null)
			{
				return;
			}
			menu.Populate(configurationBoundUserInterfaceState);
		}

		// Token: 0x060016E6 RID: 5862 RVA: 0x00085648 File Offset: 0x00083848
		[NullableContext(1)]
		protected override void ReceiveMessage(BoundUserInterfaceMessage message)
		{
			base.ReceiveMessage(message);
			ConfigurationComponent.ValidationUpdateMessage validationUpdateMessage = message as ConfigurationComponent.ValidationUpdateMessage;
			if (validationUpdateMessage != null)
			{
				this.Validation = new Regex(validationUpdateMessage.ValidationString, RegexOptions.Compiled);
			}
		}

		// Token: 0x060016E7 RID: 5863 RVA: 0x00085678 File Offset: 0x00083878
		[NullableContext(1)]
		public void SendConfiguration(Dictionary<string, string> config)
		{
			base.SendMessage(new ConfigurationComponent.ConfigurationUpdatedMessage(config));
		}

		// Token: 0x060016E8 RID: 5864 RVA: 0x00085686 File Offset: 0x00083886
		protected override void Dispose(bool disposing)
		{
			base.Dispose(disposing);
			if (disposing && this._menu != null)
			{
				this._menu.OnClose -= base.Close;
				this._menu.Close();
			}
		}

		// Token: 0x04000BE7 RID: 3047
		private ConfigurationMenu _menu;
	}
}
