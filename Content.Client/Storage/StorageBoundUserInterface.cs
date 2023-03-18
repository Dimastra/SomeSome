using System;
using System.Runtime.CompilerServices;
using Content.Client.Examine;
using Content.Client.Storage.UI;
using Content.Client.UserInterface.Controls;
using Content.Client.Verbs.UI;
using Content.Shared.Input;
using Content.Shared.Interaction;
using Content.Shared.Storage;
using Robust.Client.GameObjects;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.Controls;
using Robust.Shared.GameObjects;
using Robust.Shared.Input;
using Robust.Shared.IoC;
using Robust.Shared.ViewVariables;

namespace Content.Client.Storage
{
	// Token: 0x02000124 RID: 292
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class StorageBoundUserInterface : BoundUserInterface
	{
		// Token: 0x06000802 RID: 2050 RVA: 0x000021BC File Offset: 0x000003BC
		public StorageBoundUserInterface(ClientUserInterfaceComponent owner, Enum uiKey) : base(owner, uiKey)
		{
		}

		// Token: 0x06000803 RID: 2051 RVA: 0x0002E93C File Offset: 0x0002CB3C
		protected override void Open()
		{
			base.Open();
			if (this._window == null)
			{
				IEntityManager entityManager = IoCManager.Resolve<IEntityManager>();
				this._window = new StorageWindow(entityManager)
				{
					Title = entityManager.GetComponent<MetaDataComponent>(base.Owner.Owner).EntityName
				};
				ListContainer entityList = this._window.EntityList;
				entityList.GenerateItem = (Action<ListData, ListContainerButton>)Delegate.Combine(entityList.GenerateItem, new Action<ListData, ListContainerButton>(this._window.GenerateButton));
				ListContainer entityList2 = this._window.EntityList;
				entityList2.ItemPressed = (Action<BaseButton.ButtonEventArgs, ListData>)Delegate.Combine(entityList2.ItemPressed, new Action<BaseButton.ButtonEventArgs, ListData>(this.InteractWithItem));
				this._window.StorageContainerButton.OnPressed += this.TouchedContainerButton;
				this._window.OnClose += base.Close;
				this._window.OpenCenteredLeft();
				return;
			}
			this._window.Open();
		}

		// Token: 0x06000804 RID: 2052 RVA: 0x0002EA30 File Offset: 0x0002CC30
		public void InteractWithItem(BaseButton.ButtonEventArgs args, ListData cData)
		{
			EntityListData entityListData = cData as EntityListData;
			if (entityListData == null)
			{
				return;
			}
			EntityUid uid = entityListData.Uid;
			if (args.Event.Function == EngineKeyFunctions.UIClick)
			{
				base.SendMessage(new SharedStorageComponent.StorageInteractWithItemEvent(uid));
				return;
			}
			if (IoCManager.Resolve<IEntityManager>().EntityExists(uid))
			{
				this.OnButtonPressed(args.Event, uid);
			}
		}

		// Token: 0x06000805 RID: 2053 RVA: 0x0002EA90 File Offset: 0x0002CC90
		private void OnButtonPressed(GUIBoundKeyEventArgs args, EntityUid entity)
		{
			IEntitySystemManager entitySystemManager = IoCManager.Resolve<IEntitySystemManager>();
			IEntityManager entityManager = IoCManager.Resolve<IEntityManager>();
			if (args.Function == ContentKeyFunctions.ExamineEntity)
			{
				entitySystemManager.GetEntitySystem<ExamineSystem>().DoExamine(entity, true);
			}
			else if (args.Function == EngineKeyFunctions.UseSecondary)
			{
				IoCManager.Resolve<IUserInterfaceManager>().GetUIController<VerbMenuUIController>().OpenVerbMenu(entity, false, null);
			}
			else if (args.Function == ContentKeyFunctions.ActivateItemInWorld)
			{
				IEntityNetworkManager entityNetManager = entityManager.EntityNetManager;
				if (entityNetManager != null)
				{
					entityNetManager.SendSystemNetworkMessage(new InteractInventorySlotEvent(entity, false), true);
				}
			}
			else
			{
				if (!(args.Function == ContentKeyFunctions.AltActivateItemInWorld))
				{
					return;
				}
				entityManager.RaisePredictiveEvent<InteractInventorySlotEvent>(new InteractInventorySlotEvent(entity, true));
			}
			args.Handle();
		}

		// Token: 0x06000806 RID: 2054 RVA: 0x0002EB45 File Offset: 0x0002CD45
		public void TouchedContainerButton(BaseButton.ButtonEventArgs args)
		{
			base.SendMessage(new SharedStorageComponent.StorageInsertItemMessage());
		}

		// Token: 0x06000807 RID: 2055 RVA: 0x0002EB54 File Offset: 0x0002CD54
		protected override void UpdateState(BoundUserInterfaceState state)
		{
			base.UpdateState(state);
			if (this._window != null)
			{
				SharedStorageComponent.StorageBoundUserInterfaceState storageBoundUserInterfaceState = state as SharedStorageComponent.StorageBoundUserInterfaceState;
				if (storageBoundUserInterfaceState != null)
				{
					StorageWindow window = this._window;
					if (window == null)
					{
						return;
					}
					window.BuildEntityList(storageBoundUserInterfaceState);
					return;
				}
			}
		}

		// Token: 0x06000808 RID: 2056 RVA: 0x0002EB8C File Offset: 0x0002CD8C
		protected override void Dispose(bool disposing)
		{
			base.Dispose(disposing);
			if (!disposing)
			{
				return;
			}
			if (this._window != null)
			{
				ListContainer entityList = this._window.EntityList;
				entityList.GenerateItem = (Action<ListData, ListContainerButton>)Delegate.Remove(entityList.GenerateItem, new Action<ListData, ListContainerButton>(this._window.GenerateButton));
				ListContainer entityList2 = this._window.EntityList;
				entityList2.ItemPressed = (Action<BaseButton.ButtonEventArgs, ListData>)Delegate.Remove(entityList2.ItemPressed, new Action<BaseButton.ButtonEventArgs, ListData>(this.InteractWithItem));
				this._window.StorageContainerButton.OnPressed -= this.TouchedContainerButton;
				this._window.OnClose -= base.Close;
			}
			StorageWindow window = this._window;
			if (window != null)
			{
				window.Dispose();
			}
			this._window = null;
		}

		// Token: 0x04000410 RID: 1040
		[Nullable(2)]
		[ViewVariables]
		private StorageWindow _window;
	}
}
