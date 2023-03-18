using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Content.Client.Access.Components;
using Content.Shared.Access.Components;
using Content.Shared.Containers.ItemSlots;
using Content.Shared.CrewManifest;
using Robust.Client.GameObjects;
using Robust.Client.UserInterface.Controls;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Log;
using Robust.Shared.Prototypes;

namespace Content.Client.Access.UI
{
	// Token: 0x020004FB RID: 1275
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class IdCardConsoleBoundUserInterface : BoundUserInterface
	{
		// Token: 0x06002067 RID: 8295 RVA: 0x000021BC File Offset: 0x000003BC
		public IdCardConsoleBoundUserInterface(ClientUserInterfaceComponent owner, Enum uiKey) : base(owner, uiKey)
		{
		}

		// Token: 0x06002068 RID: 8296 RVA: 0x000BBCC8 File Offset: 0x000B9EC8
		protected override void Open()
		{
			base.Open();
			IdCardConsoleComponent idCardConsoleComponent;
			List<string> list;
			if (this._entityManager.TryGetComponent<IdCardConsoleComponent>(base.Owner.Owner, ref idCardConsoleComponent))
			{
				list = idCardConsoleComponent.AccessLevels;
				list.Sort();
			}
			else
			{
				list = new List<string>();
				string text = "idconsole";
				DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(38, 1);
				defaultInterpolatedStringHandler.AppendLiteral("No IdCardConsole component found for ");
				defaultInterpolatedStringHandler.AppendFormatted<EntityStringRepresentation>(this._entityManager.ToPrettyString(base.Owner.Owner));
				defaultInterpolatedStringHandler.AppendLiteral("!");
				Logger.ErrorS(text, defaultInterpolatedStringHandler.ToStringAndClear());
			}
			this._window = new IdCardConsoleWindow(this, this._prototypeManager, list)
			{
				Title = this._entityManager.GetComponent<MetaDataComponent>(base.Owner.Owner).EntityName
			};
			this._window.CrewManifestButton.OnPressed += delegate(BaseButton.ButtonEventArgs _)
			{
				base.SendMessage(new CrewManifestOpenUiMessage());
			};
			this._window.PrivilegedIdButton.OnPressed += delegate(BaseButton.ButtonEventArgs _)
			{
				base.SendMessage(new ItemSlotButtonPressedEvent(SharedIdCardConsoleComponent.PrivilegedIdCardSlotId, true, true));
			};
			this._window.TargetIdButton.OnPressed += delegate(BaseButton.ButtonEventArgs _)
			{
				base.SendMessage(new ItemSlotButtonPressedEvent(SharedIdCardConsoleComponent.TargetIdCardSlotId, true, true));
			};
			this._window.OnClose += base.Close;
			this._window.OpenCentered();
		}

		// Token: 0x06002069 RID: 8297 RVA: 0x000BBE04 File Offset: 0x000BA004
		protected override void Dispose(bool disposing)
		{
			base.Dispose(disposing);
			if (!disposing)
			{
				return;
			}
			IdCardConsoleWindow window = this._window;
			if (window == null)
			{
				return;
			}
			window.Dispose();
		}

		// Token: 0x0600206A RID: 8298 RVA: 0x000BBE24 File Offset: 0x000BA024
		protected override void UpdateState(BoundUserInterfaceState state)
		{
			base.UpdateState(state);
			SharedIdCardConsoleComponent.IdCardConsoleBoundUserInterfaceState state2 = (SharedIdCardConsoleComponent.IdCardConsoleBoundUserInterfaceState)state;
			IdCardConsoleWindow window = this._window;
			if (window == null)
			{
				return;
			}
			window.UpdateState(state2);
		}

		// Token: 0x0600206B RID: 8299 RVA: 0x000BBE50 File Offset: 0x000BA050
		public void SubmitData(string newFullName, string newJobTitle, List<string> newAccessList, string newJobPrototype)
		{
			if (newFullName.Length > 30)
			{
				newFullName = newFullName.Substring(0, 30);
			}
			if (newJobTitle.Length > 30)
			{
				newJobTitle = newJobTitle.Substring(0, 30);
			}
			base.SendMessage(new SharedIdCardConsoleComponent.WriteToTargetIdMessage(newFullName, newJobTitle, newAccessList, newJobPrototype));
		}

		// Token: 0x04000F70 RID: 3952
		[Dependency]
		private readonly IPrototypeManager _prototypeManager;

		// Token: 0x04000F71 RID: 3953
		[Dependency]
		private readonly IEntityManager _entityManager;

		// Token: 0x04000F72 RID: 3954
		[Nullable(2)]
		private IdCardConsoleWindow _window;
	}
}
