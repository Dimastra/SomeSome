﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using CompiledRobustXaml;
using Content.Client.Administration.Systems;
using Content.Client.UserInterface.Controls;
using Content.Client.Verbs;
using Content.Client.Verbs.UI;
using Content.Shared.Administration;
using Robust.Client.AutoGenerated;
using Robust.Client.Graphics;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.Controls;
using Robust.Shared.GameObjects;
using Robust.Shared.Input;
using Robust.Shared.IoC;
using Robust.Shared.Maths;

namespace Content.Client.Administration.UI.CustomControls
{
	// Token: 0x020004CA RID: 1226
	[NullableContext(1)]
	[Nullable(0)]
	[GenerateTypedNameReferences]
	public sealed class PlayerListControl : BoxContainer
	{
		// Token: 0x140000BD RID: 189
		// (add) Token: 0x06001F28 RID: 7976 RVA: 0x000B669C File Offset: 0x000B489C
		// (remove) Token: 0x06001F29 RID: 7977 RVA: 0x000B66D4 File Offset: 0x000B48D4
		[Nullable(2)]
		[method: NullableContext(2)]
		[Nullable(2)]
		public event Action<PlayerInfo> OnSelectionChanged;

		// Token: 0x170006CB RID: 1739
		// (get) Token: 0x06001F2A RID: 7978 RVA: 0x000B6709 File Offset: 0x000B4909
		public IReadOnlyList<PlayerInfo> PlayerInfo
		{
			get
			{
				return this._playerList;
			}
		}

		// Token: 0x06001F2B RID: 7979 RVA: 0x000B6714 File Offset: 0x000B4914
		public PlayerListControl()
		{
			this._adminSystem = EntitySystem.Get<AdminSystem>();
			this._verbSystem = EntitySystem.Get<VerbSystem>();
			IoCManager.InjectDependencies<PlayerListControl>(this);
			PlayerListControl.!XamlIlPopulateTrampoline(this);
			ListContainer playerListContainer = this.PlayerListContainer;
			playerListContainer.ItemPressed = (Action<BaseButton.ButtonEventArgs, ListData>)Delegate.Combine(playerListContainer.ItemPressed, new Action<BaseButton.ButtonEventArgs, ListData>(this.PlayerListItemPressed));
			ListContainer playerListContainer2 = this.PlayerListContainer;
			playerListContainer2.GenerateItem = (Action<ListData, ListContainerButton>)Delegate.Combine(playerListContainer2.GenerateItem, new Action<ListData, ListContainerButton>(this.GenerateButton));
			this.PopulateList(this._adminSystem.PlayerList);
			this.FilterLineEdit.OnTextChanged += delegate(LineEdit.LineEditEventArgs _)
			{
				this.FilterList();
			};
			this._adminSystem.PlayerListChanged += new Action<List<PlayerInfo>>(this.PopulateList);
			this.BackgroundPanel.PanelOverride = new StyleBoxFlat
			{
				BackgroundColor = new Color(32, 32, 32, byte.MaxValue)
			};
		}

		// Token: 0x06001F2C RID: 7980 RVA: 0x000B6814 File Offset: 0x000B4A14
		private void PlayerListItemPressed(BaseButton.ButtonEventArgs args, ListData data)
		{
			PlayerListData playerListData = data as PlayerListData;
			if (playerListData != null)
			{
				PlayerInfo info = playerListData.Info;
				if (args.Event.Function == EngineKeyFunctions.UIClick)
				{
					Action<PlayerInfo> onSelectionChanged = this.OnSelectionChanged;
					if (onSelectionChanged != null)
					{
						onSelectionChanged(info);
					}
					if (this.OverrideText != null)
					{
						Control control = args.Button.Children.FirstOrDefault<Control>();
						object obj;
						if (control == null)
						{
							obj = null;
						}
						else
						{
							Control.OrderedChildCollection children = control.Children;
							obj = ((children != null) ? children.FirstOrDefault<Control>() : null);
						}
						Label label = obj as Label;
						if (label != null)
						{
							label.Text = this.GetText(info);
							return;
						}
					}
				}
				else if (args.Event.Function == EngineKeyFunctions.UseSecondary && info.EntityUid != null)
				{
					IoCManager.Resolve<IUserInterfaceManager>().GetUIController<VerbMenuUIController>().OpenVerbMenu(info.EntityUid.Value, false, null);
				}
				return;
			}
		}

		// Token: 0x06001F2D RID: 7981 RVA: 0x000B68EE File Offset: 0x000B4AEE
		public void StopFiltering()
		{
			this.FilterLineEdit.Text = string.Empty;
		}

		// Token: 0x06001F2E RID: 7982 RVA: 0x000B6900 File Offset: 0x000B4B00
		private void FilterList()
		{
			this._sortedPlayerList.Clear();
			foreach (PlayerInfo playerInfo in this._playerList)
			{
				string text = playerInfo.CharacterName + " (" + playerInfo.Username + ")";
				if (playerInfo.IdentityName != playerInfo.CharacterName)
				{
					text = text + " [" + playerInfo.IdentityName + "]";
				}
				if (string.IsNullOrEmpty(this.FilterLineEdit.Text) || text.ToLowerInvariant().Contains(this.FilterLineEdit.Text.Trim().ToLowerInvariant()))
				{
					this._sortedPlayerList.Add(playerInfo);
				}
			}
			if (this.Comparison != null)
			{
				this._sortedPlayerList.Sort((PlayerInfo a, PlayerInfo b) => this.Comparison(a, b));
			}
			this.PlayerListContainer.PopulateList((from info in this._sortedPlayerList
			select new PlayerListData(info)).ToList<PlayerListData>());
		}

		// Token: 0x06001F2F RID: 7983 RVA: 0x000B6A3C File Offset: 0x000B4C3C
		public void PopulateList([Nullable(new byte[]
		{
			2,
			1
		})] IReadOnlyList<PlayerInfo> players = null)
		{
			if (players == null)
			{
				players = this._adminSystem.PlayerList;
			}
			this._playerList = players.ToList<PlayerInfo>();
			this.FilterList();
		}

		// Token: 0x06001F30 RID: 7984 RVA: 0x000B6A60 File Offset: 0x000B4C60
		private string GetText(PlayerInfo info)
		{
			string text = info.CharacterName + " (" + info.Username + ")";
			if (this.OverrideText != null)
			{
				text = this.OverrideText(info, text);
			}
			return text;
		}

		// Token: 0x06001F31 RID: 7985 RVA: 0x000B6AA0 File Offset: 0x000B4CA0
		private void GenerateButton(ListData data, ListContainerButton button)
		{
			PlayerListData playerListData = data as PlayerListData;
			if (playerListData != null)
			{
				PlayerInfo info = playerListData.Info;
				BoxContainer boxContainer = new BoxContainer();
				boxContainer.Orientation = 1;
				boxContainer.Children.Add(new Label
				{
					ClipText = true,
					Text = this.GetText(info)
				});
				button.AddChild(boxContainer);
				button.EnableAllKeybinds = true;
				button.AddStyleClass("list-container-button");
				return;
			}
		}

		// Token: 0x170006CC RID: 1740
		// (get) Token: 0x06001F32 RID: 7986 RVA: 0x000B6B09 File Offset: 0x000B4D09
		[Nullable(0)]
		private LineEdit FilterLineEdit
		{
			[NullableContext(0)]
			get
			{
				return base.FindControl<LineEdit>("FilterLineEdit");
			}
		}

		// Token: 0x170006CD RID: 1741
		// (get) Token: 0x06001F33 RID: 7987 RVA: 0x0003A6BC File Offset: 0x000388BC
		[Nullable(0)]
		private PanelContainer BackgroundPanel
		{
			[NullableContext(0)]
			get
			{
				return base.FindControl<PanelContainer>("BackgroundPanel");
			}
		}

		// Token: 0x170006CE RID: 1742
		// (get) Token: 0x06001F34 RID: 7988 RVA: 0x000B6B16 File Offset: 0x000B4D16
		[Nullable(0)]
		public ListContainer PlayerListContainer
		{
			[NullableContext(0)]
			get
			{
				return base.FindControl<ListContainer>("PlayerListContainer");
			}
		}

		// Token: 0x06001F37 RID: 7991 RVA: 0x000B6B3C File Offset: 0x000B4D3C
		static void xaml(IServiceProvider A_0, BoxContainer A_1)
		{
			XamlIlContext.Context<BoxContainer> context = new XamlIlContext.Context<BoxContainer>(A_0, null, "resm:Content.Client.Administration.UI.CustomControls.PlayerListControl.xaml?assembly=Content.Client");
			context.RootObject = A_1;
			context.IntermediateRoot = A_1;
			A_1.Orientation = 1;
			Control control = new Control
			{
				MinSize = new Vector2(0f, 5f)
			};
			A_1.XamlChildren.Add(control);
			LineEdit lineEdit = new LineEdit();
			lineEdit.Name = "FilterLineEdit";
			control = lineEdit;
			context.RobustNameScope.Register("FilterLineEdit", control);
			lineEdit.MinSize = new Vector2(100f, 0f);
			lineEdit.HorizontalExpand = true;
			lineEdit.PlaceHolder = (string)new LocExtension("Filter").ProvideValue();
			control = lineEdit;
			A_1.XamlChildren.Add(control);
			PanelContainer panelContainer = new PanelContainer();
			panelContainer.Name = "BackgroundPanel";
			control = panelContainer;
			context.RobustNameScope.Register("BackgroundPanel", control);
			panelContainer.VerticalExpand = true;
			panelContainer.HorizontalExpand = true;
			ListContainer listContainer = new ListContainer();
			listContainer.Name = "PlayerListContainer";
			control = listContainer;
			context.RobustNameScope.Register("PlayerListContainer", control);
			listContainer.Access = new AccessLevel?(0);
			listContainer.Toggle = true;
			listContainer.Group = true;
			listContainer.MinSize = new Vector2(100f, 0f);
			control = listContainer;
			panelContainer.XamlChildren.Add(control);
			control = panelContainer;
			A_1.XamlChildren.Add(control);
			if ((control = (A_1 as Control)) != null)
			{
				context.RobustNameScope.Absorb(control.NameScope);
				control.NameScope = context.RobustNameScope;
			}
			context.RobustNameScope.Complete();
		}

		// Token: 0x06001F38 RID: 7992 RVA: 0x000B6D36 File Offset: 0x000B4F36
		private static void !XamlIlPopulateTrampoline(PlayerListControl A_0)
		{
			PlayerListControl.Populate:Content.Client.Administration.UI.CustomControls.PlayerListControl.xaml(null, A_0);
		}

		// Token: 0x04000F01 RID: 3841
		private readonly AdminSystem _adminSystem;

		// Token: 0x04000F02 RID: 3842
		private readonly VerbSystem _verbSystem;

		// Token: 0x04000F03 RID: 3843
		private List<PlayerInfo> _playerList = new List<PlayerInfo>();

		// Token: 0x04000F04 RID: 3844
		private readonly List<PlayerInfo> _sortedPlayerList = new List<PlayerInfo>();

		// Token: 0x04000F06 RID: 3846
		[Nullable(new byte[]
		{
			2,
			1,
			1,
			1
		})]
		public Func<PlayerInfo, string, string> OverrideText;

		// Token: 0x04000F07 RID: 3847
		[Nullable(new byte[]
		{
			2,
			1
		})]
		public Comparison<PlayerInfo> Comparison;
	}
}
