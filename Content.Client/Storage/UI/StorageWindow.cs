using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Content.Client.UserInterface.Controls;
using Content.Shared.Item;
using Content.Shared.Storage;
using Robust.Client.GameObjects;
using Robust.Client.Graphics;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.Controls;
using Robust.Client.UserInterface.CustomControls;
using Robust.Shared.GameObjects;
using Robust.Shared.Localization;
using Robust.Shared.Maths;

namespace Content.Client.Storage.UI
{
	// Token: 0x02000129 RID: 297
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class StorageWindow : DefaultWindow
	{
		// Token: 0x06000812 RID: 2066 RVA: 0x0002EF78 File Offset: 0x0002D178
		public StorageWindow(IEntityManager entityManager)
		{
			StorageWindow <>4__this = this;
			this._entityManager = entityManager;
			base.SetSize = new ValueTuple<float, float>(200f, 320f);
			base.Title = Loc.GetString("comp-storage-window-title");
			base.RectClipContent = true;
			this.StorageContainerButton = new ContainerButton
			{
				Name = "StorageContainerButton",
				MouseFilter = 1
			};
			base.Contents.AddChild(this.StorageContainerButton);
			PanelContainer innerContainerButton = new PanelContainer
			{
				PanelOverride = this._unHoveredBox
			};
			this.StorageContainerButton.AddChild(innerContainerButton);
			Control control = new BoxContainer
			{
				Orientation = 1,
				MouseFilter = 2
			};
			this.StorageContainerButton.AddChild(control);
			this._information = new Label
			{
				Text = Loc.GetString("comp-storage-window-volume", new ValueTuple<string, object>[]
				{
					new ValueTuple<string, object>("itemCount", 0),
					new ValueTuple<string, object>("usedVolume", 0),
					new ValueTuple<string, object>("maxVolume", 0)
				}),
				VerticalAlignment = 2
			};
			control.AddChild(this._information);
			this.EntityList = new ListContainer
			{
				Name = "EntityListContainer"
			};
			control.AddChild(this.EntityList);
			this.EntityList.OnMouseEntered += delegate(GUIMouseHoverEventArgs _)
			{
				innerContainerButton.PanelOverride = <>4__this._hoveredBox;
			};
			this.EntityList.OnMouseExited += delegate(GUIMouseHoverEventArgs _)
			{
				innerContainerButton.PanelOverride = <>4__this._unHoveredBox;
			};
		}

		// Token: 0x06000813 RID: 2067 RVA: 0x0002F158 File Offset: 0x0002D358
		public void BuildEntityList(SharedStorageComponent.StorageBoundUserInterfaceState state)
		{
			List<EntityListData> data = state.StoredEntities.ConvertAll<EntityListData>((EntityUid uid) => new EntityListData(uid));
			this.EntityList.PopulateList(data);
			if (state.StorageCapacityMax != 0)
			{
				this._information.Text = Loc.GetString("comp-storage-window-volume", new ValueTuple<string, object>[]
				{
					new ValueTuple<string, object>("itemCount", state.StoredEntities.Count),
					new ValueTuple<string, object>("usedVolume", state.StorageSizeUsed),
					new ValueTuple<string, object>("maxVolume", state.StorageCapacityMax)
				});
				return;
			}
			this._information.Text = Loc.GetString("comp-storage-window-volume-unlimited", new ValueTuple<string, object>[]
			{
				new ValueTuple<string, object>("itemCount", state.StoredEntities.Count)
			});
		}

		// Token: 0x06000814 RID: 2068 RVA: 0x0002F258 File Offset: 0x0002D458
		public void GenerateButton(ListData data, ListContainerButton button)
		{
			EntityListData entityListData = data as EntityListData;
			if (entityListData != null)
			{
				EntityUid uid = entityListData.Uid;
				if (this._entityManager.EntityExists(uid))
				{
					SpriteComponent sprite;
					this._entityManager.TryGetComponent<SpriteComponent>(uid, ref sprite);
					ItemComponent itemComponent;
					this._entityManager.TryGetComponent<ItemComponent>(uid, ref itemComponent);
					BoxContainer boxContainer = new BoxContainer();
					boxContainer.Orientation = 0;
					boxContainer.SeparationOverride = new int?(2);
					boxContainer.Children.Add(new SpriteView
					{
						HorizontalAlignment = 1,
						VerticalAlignment = 2,
						MinSize = new Vector2(32f, 32f),
						OverrideDirection = new Direction?(0),
						Sprite = sprite
					});
					boxContainer.Children.Add(new Label
					{
						HorizontalExpand = true,
						ClipText = true,
						Text = this._entityManager.GetComponent<MetaDataComponent>(uid).EntityName
					});
					boxContainer.Children.Add(new Label
					{
						Align = 2,
						Text = (((itemComponent != null) ? itemComponent.Size.ToString() : null) ?? Loc.GetString("comp-storage-no-item-size"))
					});
					button.AddChild(boxContainer);
					button.StyleClasses.Add("storageButton");
					button.EnableAllKeybinds = true;
					return;
				}
			}
		}

		// Token: 0x0400041A RID: 1050
		private IEntityManager _entityManager;

		// Token: 0x0400041B RID: 1051
		private readonly Label _information;

		// Token: 0x0400041C RID: 1052
		public readonly ContainerButton StorageContainerButton;

		// Token: 0x0400041D RID: 1053
		public readonly ListContainer EntityList;

		// Token: 0x0400041E RID: 1054
		private readonly StyleBoxFlat _hoveredBox = new StyleBoxFlat
		{
			BackgroundColor = Color.Black.WithAlpha(0.35f)
		};

		// Token: 0x0400041F RID: 1055
		private readonly StyleBoxFlat _unHoveredBox = new StyleBoxFlat
		{
			BackgroundColor = Color.Black.WithAlpha(0f)
		};
	}
}
