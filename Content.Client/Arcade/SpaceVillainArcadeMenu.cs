using System;
using System.Runtime.CompilerServices;
using Content.Client.Arcade.UI;
using Content.Shared.Arcade;
using Robust.Client.UserInterface.Controls;
using Robust.Client.UserInterface.CustomControls;
using Robust.Shared.Localization;

namespace Content.Client.Arcade
{
	// Token: 0x02000466 RID: 1126
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class SpaceVillainArcadeMenu : DefaultWindow
	{
		// Token: 0x170005D6 RID: 1494
		// (get) Token: 0x06001BF8 RID: 7160 RVA: 0x000A2398 File Offset: 0x000A0598
		// (set) Token: 0x06001BF9 RID: 7161 RVA: 0x000A23A0 File Offset: 0x000A05A0
		public SpaceVillainArcadeBoundUserInterface Owner { get; set; }

		// Token: 0x06001BFA RID: 7162 RVA: 0x000A23AC File Offset: 0x000A05AC
		public SpaceVillainArcadeMenu(SpaceVillainArcadeBoundUserInterface owner)
		{
			base.MinSize = (base.SetSize = new ValueTuple<float, float>(300f, 225f));
			base.Title = Loc.GetString("spacevillain-menu-title");
			this.Owner = owner;
			GridContainer gridContainer = new GridContainer
			{
				Columns = 1
			};
			GridContainer gridContainer2 = new GridContainer
			{
				Columns = 3
			};
			gridContainer2.AddChild(new Label
			{
				Text = Loc.GetString("spacevillain-menu-label-player"),
				Align = 1
			});
			gridContainer2.AddChild(new Label
			{
				Text = "|",
				Align = 1
			});
			this._enemyNameLabel = new Label
			{
				Align = 1
			};
			gridContainer2.AddChild(this._enemyNameLabel);
			this._playerInfoLabel = new Label
			{
				Align = 1
			};
			gridContainer2.AddChild(this._playerInfoLabel);
			gridContainer2.AddChild(new Label
			{
				Text = "|",
				Align = 1
			});
			this._enemyInfoLabel = new Label
			{
				Align = 1
			};
			gridContainer2.AddChild(this._enemyInfoLabel);
			CenterContainer centerContainer = new CenterContainer();
			centerContainer.AddChild(gridContainer2);
			gridContainer.AddChild(centerContainer);
			this._playerActionLabel = new Label
			{
				Align = 1
			};
			gridContainer.AddChild(this._playerActionLabel);
			this._enemyActionLabel = new Label
			{
				Align = 1
			};
			gridContainer.AddChild(this._enemyActionLabel);
			GridContainer gridContainer3 = new GridContainer
			{
				Columns = 3
			};
			this._gameButtons[0] = new SpaceVillainArcadeMenu.ActionButton(this.Owner, SharedSpaceVillainArcadeComponent.PlayerAction.Attack)
			{
				Text = Loc.GetString("spacevillain-menu-button-attack")
			};
			gridContainer3.AddChild(this._gameButtons[0]);
			this._gameButtons[1] = new SpaceVillainArcadeMenu.ActionButton(this.Owner, SharedSpaceVillainArcadeComponent.PlayerAction.Heal)
			{
				Text = Loc.GetString("spacevillain-menu-button-heal")
			};
			gridContainer3.AddChild(this._gameButtons[1]);
			this._gameButtons[2] = new SpaceVillainArcadeMenu.ActionButton(this.Owner, SharedSpaceVillainArcadeComponent.PlayerAction.Recharge)
			{
				Text = Loc.GetString("spacevillain-menu-button-recharge")
			};
			gridContainer3.AddChild(this._gameButtons[2]);
			centerContainer = new CenterContainer();
			centerContainer.AddChild(gridContainer3);
			gridContainer.AddChild(centerContainer);
			SpaceVillainArcadeMenu.ActionButton actionButton = new SpaceVillainArcadeMenu.ActionButton(this.Owner, SharedSpaceVillainArcadeComponent.PlayerAction.NewGame)
			{
				Text = Loc.GetString("spacevillain-menu-button-new-game")
			};
			gridContainer.AddChild(actionButton);
			base.Contents.AddChild(gridContainer);
		}

		// Token: 0x06001BFB RID: 7163 RVA: 0x000A2610 File Offset: 0x000A0810
		private void UpdateMetadata(SharedSpaceVillainArcadeComponent.SpaceVillainArcadeMetaDataUpdateMessage message)
		{
			base.Title = message.GameTitle;
			this._enemyNameLabel.Text = message.EnemyName;
			Button[] gameButtons = this._gameButtons;
			for (int i = 0; i < gameButtons.Length; i++)
			{
				gameButtons[i].Disabled = message.ButtonsDisabled;
			}
		}

		// Token: 0x06001BFC RID: 7164 RVA: 0x000A2660 File Offset: 0x000A0860
		public void UpdateInfo(SharedSpaceVillainArcadeComponent.SpaceVillainArcadeDataUpdateMessage message)
		{
			SharedSpaceVillainArcadeComponent.SpaceVillainArcadeMetaDataUpdateMessage spaceVillainArcadeMetaDataUpdateMessage = message as SharedSpaceVillainArcadeComponent.SpaceVillainArcadeMetaDataUpdateMessage;
			if (spaceVillainArcadeMetaDataUpdateMessage != null)
			{
				this.UpdateMetadata(spaceVillainArcadeMetaDataUpdateMessage);
			}
			Label playerInfoLabel = this._playerInfoLabel;
			DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(9, 2);
			defaultInterpolatedStringHandler.AppendLiteral("HP: ");
			defaultInterpolatedStringHandler.AppendFormatted<int>(message.PlayerHP);
			defaultInterpolatedStringHandler.AppendLiteral(" MP: ");
			defaultInterpolatedStringHandler.AppendFormatted<int>(message.PlayerMP);
			playerInfoLabel.Text = defaultInterpolatedStringHandler.ToStringAndClear();
			Label enemyInfoLabel = this._enemyInfoLabel;
			defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(9, 2);
			defaultInterpolatedStringHandler.AppendLiteral("HP: ");
			defaultInterpolatedStringHandler.AppendFormatted<int>(message.EnemyHP);
			defaultInterpolatedStringHandler.AppendLiteral(" MP: ");
			defaultInterpolatedStringHandler.AppendFormatted<int>(message.EnemyMP);
			enemyInfoLabel.Text = defaultInterpolatedStringHandler.ToStringAndClear();
			this._playerActionLabel.Text = message.PlayerActionMessage;
			this._enemyActionLabel.Text = message.EnemyActionMessage;
		}

		// Token: 0x04000E10 RID: 3600
		private readonly Label _enemyNameLabel;

		// Token: 0x04000E11 RID: 3601
		private readonly Label _playerInfoLabel;

		// Token: 0x04000E12 RID: 3602
		private readonly Label _enemyInfoLabel;

		// Token: 0x04000E13 RID: 3603
		private readonly Label _playerActionLabel;

		// Token: 0x04000E14 RID: 3604
		private readonly Label _enemyActionLabel;

		// Token: 0x04000E15 RID: 3605
		private readonly Button[] _gameButtons = new Button[3];

		// Token: 0x02000467 RID: 1127
		[Nullable(0)]
		private sealed class ActionButton : Button
		{
			// Token: 0x06001BFD RID: 7165 RVA: 0x000A273C File Offset: 0x000A093C
			public ActionButton(SpaceVillainArcadeBoundUserInterface owner, SharedSpaceVillainArcadeComponent.PlayerAction playerAction)
			{
				this._owner = owner;
				this._playerAction = playerAction;
				base.OnPressed += this.Clicked;
			}

			// Token: 0x06001BFE RID: 7166 RVA: 0x000A2764 File Offset: 0x000A0964
			private void Clicked(BaseButton.ButtonEventArgs e)
			{
				this._owner.SendAction(this._playerAction);
			}

			// Token: 0x04000E16 RID: 3606
			private readonly SpaceVillainArcadeBoundUserInterface _owner;

			// Token: 0x04000E17 RID: 3607
			private readonly SharedSpaceVillainArcadeComponent.PlayerAction _playerAction;
		}
	}
}
