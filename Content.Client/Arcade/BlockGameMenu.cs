using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using Content.Client.Arcade.UI;
using Content.Client.Resources;
using Content.Shared.Arcade;
using Content.Shared.Input;
using Robust.Client.Graphics;
using Robust.Client.ResourceManagement;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.Controls;
using Robust.Client.UserInterface.CustomControls;
using Robust.Shared.IoC;
using Robust.Shared.Localization;
using Robust.Shared.Maths;
using Robust.Shared.Utility;

namespace Content.Client.Arcade
{
	// Token: 0x02000463 RID: 1123
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class BlockGameMenu : DefaultWindow
	{
		// Token: 0x06001BD4 RID: 7124 RVA: 0x000A0F5C File Offset: 0x0009F15C
		public BlockGameMenu(BlockGameBoundUserInterface owner)
		{
			base.Title = Loc.GetString("blockgame-menu-title");
			this._owner = owner;
			base.MinSize = (base.SetSize = new ValueTuple<float, float>(410f, 490f));
			Texture texture = IoCManager.Resolve<IResourceCache>().GetTexture("/Textures/Interface/Nano/button.svg.96dpi.png");
			this._mainPanel = new PanelContainer();
			this._gameRootContainer = new BoxContainer
			{
				Orientation = 1
			};
			this._levelLabel = new Label
			{
				Align = 1,
				HorizontalExpand = true
			};
			this._gameRootContainer.AddChild(this._levelLabel);
			this._gameRootContainer.AddChild(new Control
			{
				MinSize = new Vector2(1f, 5f)
			});
			this._pointsLabel = new Label
			{
				Align = 1,
				HorizontalExpand = true
			};
			this._gameRootContainer.AddChild(this._pointsLabel);
			this._gameRootContainer.AddChild(new Control
			{
				MinSize = new Vector2(1f, 10f)
			});
			BoxContainer boxContainer = new BoxContainer
			{
				Orientation = 0
			};
			boxContainer.AddChild(this.SetupHoldBox(texture));
			boxContainer.AddChild(new Control
			{
				MinSize = new Vector2(10f, 1f)
			});
			boxContainer.AddChild(this.SetupGameGrid(texture));
			boxContainer.AddChild(new Control
			{
				MinSize = new Vector2(10f, 1f)
			});
			boxContainer.AddChild(this.SetupNextBox(texture));
			this._gameRootContainer.AddChild(boxContainer);
			this._gameRootContainer.AddChild(new Control
			{
				MinSize = new Vector2(1f, 10f)
			});
			this._pauseButton = new Button
			{
				Text = Loc.GetString("blockgame-menu-button-pause"),
				TextAlign = 1
			};
			this._pauseButton.OnPressed += delegate(BaseButton.ButtonEventArgs e)
			{
				this.TryPause();
			};
			this._gameRootContainer.AddChild(this._pauseButton);
			this._mainPanel.AddChild(this._gameRootContainer);
			StyleBoxTexture styleBoxTexture = new StyleBoxTexture
			{
				Texture = texture,
				Modulate = BlockGameMenu.OverlayShadowColor
			};
			styleBoxTexture.SetPatchMargin(15, 10f);
			this._menuRootContainer = new PanelContainer
			{
				PanelOverride = styleBoxTexture,
				VerticalAlignment = 2,
				HorizontalAlignment = 2
			};
			StyleBoxTexture styleBoxTexture2 = new StyleBoxTexture
			{
				Texture = texture,
				Modulate = BlockGameMenu.OverlayBackgroundColor
			};
			styleBoxTexture2.SetPatchMargin(15, 10f);
			PanelContainer panelContainer = new PanelContainer
			{
				PanelOverride = styleBoxTexture2,
				VerticalAlignment = 2,
				HorizontalAlignment = 2
			};
			this._menuRootContainer.AddChild(panelContainer);
			BoxContainer boxContainer2 = new BoxContainer
			{
				Orientation = 1,
				HorizontalAlignment = 2,
				VerticalAlignment = 2
			};
			this._newGameButton = new Button
			{
				Text = Loc.GetString("blockgame-menu-button-new-game"),
				TextAlign = 1
			};
			this._newGameButton.OnPressed += delegate(BaseButton.ButtonEventArgs e)
			{
				this._owner.SendAction(BlockGamePlayerAction.NewGame);
			};
			boxContainer2.AddChild(this._newGameButton);
			boxContainer2.AddChild(new Control
			{
				MinSize = new Vector2(1f, 10f)
			});
			this._scoreBoardButton = new Button
			{
				Text = Loc.GetString("blockgame-menu-button-scoreboard"),
				TextAlign = 1
			};
			this._scoreBoardButton.OnPressed += delegate(BaseButton.ButtonEventArgs e)
			{
				this._owner.SendAction(BlockGamePlayerAction.ShowHighscores);
			};
			boxContainer2.AddChild(this._scoreBoardButton);
			this._unpauseButtonMargin = new Control
			{
				MinSize = new Vector2(1f, 10f),
				Visible = false
			};
			boxContainer2.AddChild(this._unpauseButtonMargin);
			this._unpauseButton = new Button
			{
				Text = Loc.GetString("blockgame-menu-button-unpause"),
				TextAlign = 1,
				Visible = false
			};
			this._unpauseButton.OnPressed += delegate(BaseButton.ButtonEventArgs e)
			{
				this._owner.SendAction(BlockGamePlayerAction.Unpause);
			};
			boxContainer2.AddChild(this._unpauseButton);
			panelContainer.AddChild(boxContainer2);
			StyleBoxTexture styleBoxTexture3 = new StyleBoxTexture
			{
				Texture = texture,
				Modulate = BlockGameMenu.OverlayShadowColor
			};
			styleBoxTexture3.SetPatchMargin(15, 10f);
			this._gameOverRootContainer = new PanelContainer
			{
				PanelOverride = styleBoxTexture3,
				VerticalAlignment = 2,
				HorizontalAlignment = 2
			};
			StyleBoxTexture styleBoxTexture4 = new StyleBoxTexture
			{
				Texture = texture,
				Modulate = BlockGameMenu.OverlayBackgroundColor
			};
			styleBoxTexture4.SetPatchMargin(15, 10f);
			PanelContainer panelContainer2 = new PanelContainer
			{
				PanelOverride = styleBoxTexture4,
				VerticalAlignment = 2,
				HorizontalAlignment = 2
			};
			this._gameOverRootContainer.AddChild(panelContainer2);
			BoxContainer boxContainer3 = new BoxContainer
			{
				Orientation = 1,
				HorizontalAlignment = 2,
				VerticalAlignment = 2
			};
			boxContainer3.AddChild(new Label
			{
				Text = Loc.GetString("blockgame-menu-msg-game-over"),
				Align = 1
			});
			boxContainer3.AddChild(new Control
			{
				MinSize = new Vector2(1f, 10f)
			});
			this._finalScoreLabel = new Label
			{
				Align = 1
			};
			boxContainer3.AddChild(this._finalScoreLabel);
			boxContainer3.AddChild(new Control
			{
				MinSize = new Vector2(1f, 10f)
			});
			this._finalNewGameButton = new Button
			{
				Text = Loc.GetString("blockgame-menu-button-new-game"),
				TextAlign = 1
			};
			this._finalNewGameButton.OnPressed += delegate(BaseButton.ButtonEventArgs e)
			{
				this._owner.SendAction(BlockGamePlayerAction.NewGame);
			};
			boxContainer3.AddChild(this._finalNewGameButton);
			panelContainer2.AddChild(boxContainer3);
			StyleBoxTexture styleBoxTexture5 = new StyleBoxTexture
			{
				Texture = texture,
				Modulate = BlockGameMenu.OverlayShadowColor
			};
			styleBoxTexture5.SetPatchMargin(15, 10f);
			this._highscoresRootContainer = new PanelContainer
			{
				PanelOverride = styleBoxTexture5,
				VerticalAlignment = 2,
				HorizontalAlignment = 2
			};
			Color modulate;
			modulate..ctor(BlockGameMenu.OverlayBackgroundColor.R, BlockGameMenu.OverlayBackgroundColor.G, BlockGameMenu.OverlayBackgroundColor.B, 220f);
			StyleBoxTexture styleBoxTexture6 = new StyleBoxTexture
			{
				Texture = texture,
				Modulate = modulate
			};
			styleBoxTexture6.SetPatchMargin(15, 10f);
			PanelContainer panelContainer3 = new PanelContainer
			{
				PanelOverride = styleBoxTexture6,
				VerticalAlignment = 2,
				HorizontalAlignment = 2
			};
			this._highscoresRootContainer.AddChild(panelContainer3);
			BoxContainer boxContainer4 = new BoxContainer
			{
				Orientation = 1,
				HorizontalAlignment = 2,
				VerticalAlignment = 2
			};
			boxContainer4.AddChild(new Label
			{
				Text = Loc.GetString("blockgame-menu-label-highscores")
			});
			boxContainer4.AddChild(new Control
			{
				MinSize = new Vector2(1f, 10f)
			});
			BoxContainer boxContainer5 = new BoxContainer
			{
				Orientation = 0
			};
			this._localHighscoresLabel = new Label
			{
				Align = 1
			};
			boxContainer5.AddChild(this._localHighscoresLabel);
			boxContainer5.AddChild(new Control
			{
				MinSize = new Vector2(40f, 1f)
			});
			this._globalHighscoresLabel = new Label
			{
				Align = 1
			};
			boxContainer5.AddChild(this._globalHighscoresLabel);
			boxContainer4.AddChild(boxContainer5);
			boxContainer4.AddChild(new Control
			{
				MinSize = new Vector2(1f, 10f)
			});
			this._highscoreBackButton = new Button
			{
				Text = Loc.GetString("blockgame-menu-button-back"),
				TextAlign = 1
			};
			this._highscoreBackButton.OnPressed += delegate(BaseButton.ButtonEventArgs e)
			{
				this._owner.SendAction(BlockGamePlayerAction.Pause);
			};
			boxContainer4.AddChild(this._highscoreBackButton);
			panelContainer3.AddChild(boxContainer4);
			base.Contents.AddChild(this._mainPanel);
			base.CanKeyboardFocus = true;
		}

		// Token: 0x06001BD5 RID: 7125 RVA: 0x000A16FB File Offset: 0x0009F8FB
		public void SetUsability(bool isPlayer)
		{
			this._isPlayer = isPlayer;
			this.UpdateUsability();
		}

		// Token: 0x06001BD6 RID: 7126 RVA: 0x000A170C File Offset: 0x0009F90C
		private void UpdateUsability()
		{
			this._pauseButton.Disabled = !this._isPlayer;
			this._newGameButton.Disabled = !this._isPlayer;
			this._scoreBoardButton.Disabled = !this._isPlayer;
			this._unpauseButton.Disabled = !this._isPlayer;
			this._finalNewGameButton.Disabled = !this._isPlayer;
			this._highscoreBackButton.Disabled = !this._isPlayer;
		}

		// Token: 0x06001BD7 RID: 7127 RVA: 0x000A1794 File Offset: 0x0009F994
		private Control SetupGameGrid(Texture panelTex)
		{
			this._gameGrid = new GridContainer
			{
				Columns = 10,
				HSeparationOverride = new int?(1),
				VSeparationOverride = new int?(1)
			};
			this.UpdateBlocks(new BlockGameBlock[0]);
			StyleBoxTexture styleBoxTexture = new StyleBoxTexture
			{
				Texture = panelTex,
				Modulate = Color.FromHex("#4a4a51", null)
			};
			styleBoxTexture.SetPatchMargin(15, 10f);
			PanelContainer panelContainer = new PanelContainer();
			panelContainer.PanelOverride = styleBoxTexture;
			panelContainer.HorizontalExpand = true;
			panelContainer.SizeFlagsStretchRatio = 60f;
			PanelContainer panelContainer2 = new PanelContainer
			{
				PanelOverride = new StyleBoxFlat
				{
					BackgroundColor = Color.FromHex("#868686", null)
				}
			};
			panelContainer2.AddChild(this._gameGrid);
			panelContainer.AddChild(panelContainer2);
			return panelContainer;
		}

		// Token: 0x06001BD8 RID: 7128 RVA: 0x000A1870 File Offset: 0x0009FA70
		private Control SetupNextBox(Texture panelTex)
		{
			StyleBoxTexture styleBoxTexture = new StyleBoxTexture
			{
				Texture = panelTex,
				Modulate = Color.FromHex("#4a4a51", null)
			};
			styleBoxTexture.SetPatchMargin(15, 10f);
			GridContainer gridContainer = new GridContainer();
			gridContainer.Columns = 1;
			gridContainer.HorizontalExpand = true;
			gridContainer.SizeFlagsStretchRatio = 20f;
			PanelContainer panelContainer = new PanelContainer
			{
				PanelOverride = styleBoxTexture,
				MinSize = BlockGameMenu.BlockSize * 6.5f,
				HorizontalAlignment = 1,
				VerticalAlignment = 1
			};
			CenterContainer centerContainer = new CenterContainer();
			this._nextBlockGrid = new GridContainer
			{
				HSeparationOverride = new int?(1),
				VSeparationOverride = new int?(1)
			};
			centerContainer.AddChild(this._nextBlockGrid);
			panelContainer.AddChild(centerContainer);
			gridContainer.AddChild(panelContainer);
			gridContainer.AddChild(new Label
			{
				Text = Loc.GetString("blockgame-menu-label-next"),
				Align = 1
			});
			return gridContainer;
		}

		// Token: 0x06001BD9 RID: 7129 RVA: 0x000A1968 File Offset: 0x0009FB68
		private Control SetupHoldBox(Texture panelTex)
		{
			StyleBoxTexture styleBoxTexture = new StyleBoxTexture
			{
				Texture = panelTex,
				Modulate = Color.FromHex("#4a4a51", null)
			};
			styleBoxTexture.SetPatchMargin(15, 10f);
			GridContainer gridContainer = new GridContainer();
			gridContainer.Columns = 1;
			gridContainer.HorizontalExpand = true;
			gridContainer.SizeFlagsStretchRatio = 20f;
			PanelContainer panelContainer = new PanelContainer
			{
				PanelOverride = styleBoxTexture,
				MinSize = BlockGameMenu.BlockSize * 6.5f,
				HorizontalAlignment = 1,
				VerticalAlignment = 1
			};
			CenterContainer centerContainer = new CenterContainer();
			this._holdBlockGrid = new GridContainer
			{
				HSeparationOverride = new int?(1),
				VSeparationOverride = new int?(1)
			};
			centerContainer.AddChild(this._holdBlockGrid);
			panelContainer.AddChild(centerContainer);
			gridContainer.AddChild(panelContainer);
			gridContainer.AddChild(new Label
			{
				Text = Loc.GetString("blockgame-menu-label-hold"),
				Align = 1
			});
			return gridContainer;
		}

		// Token: 0x06001BDA RID: 7130 RVA: 0x000A1A60 File Offset: 0x0009FC60
		protected override void KeyboardFocusExited()
		{
			if (!base.IsOpen)
			{
				return;
			}
			if (this._gameOver)
			{
				return;
			}
			this.TryPause();
		}

		// Token: 0x06001BDB RID: 7131 RVA: 0x000A1A7A File Offset: 0x0009FC7A
		private void TryPause()
		{
			this._owner.SendAction(BlockGamePlayerAction.Pause);
		}

		// Token: 0x06001BDC RID: 7132 RVA: 0x000A1A89 File Offset: 0x0009FC89
		public void SetStarted()
		{
			this._gameOver = false;
			this._unpauseButton.Visible = true;
			this._unpauseButtonMargin.Visible = true;
		}

		// Token: 0x06001BDD RID: 7133 RVA: 0x000A1AAC File Offset: 0x0009FCAC
		public void SetScreen(BlockGameMessages.BlockGameScreen screen)
		{
			if (this._gameOver)
			{
				return;
			}
			switch (screen)
			{
			case BlockGameMessages.BlockGameScreen.Game:
				base.GrabKeyboardFocus();
				this.CloseMenus();
				this._pauseButton.Disabled = !this._isPlayer;
				return;
			case BlockGameMessages.BlockGameScreen.Pause:
				this.CloseMenus();
				this._mainPanel.AddChild(this._menuRootContainer);
				this._pauseButton.Disabled = true;
				return;
			case BlockGameMessages.BlockGameScreen.Gameover:
				this._gameOver = true;
				this._pauseButton.Disabled = true;
				this.CloseMenus();
				this._mainPanel.AddChild(this._gameOverRootContainer);
				return;
			case BlockGameMessages.BlockGameScreen.Highscores:
				this.CloseMenus();
				this._mainPanel.AddChild(this._highscoresRootContainer);
				return;
			default:
				return;
			}
		}

		// Token: 0x06001BDE RID: 7134 RVA: 0x000A1B60 File Offset: 0x0009FD60
		private void CloseMenus()
		{
			if (this._mainPanel.Children.Contains(this._menuRootContainer))
			{
				this._mainPanel.RemoveChild(this._menuRootContainer);
			}
			if (this._mainPanel.Children.Contains(this._gameOverRootContainer))
			{
				this._mainPanel.RemoveChild(this._gameOverRootContainer);
			}
			if (this._mainPanel.Children.Contains(this._highscoresRootContainer))
			{
				this._mainPanel.RemoveChild(this._highscoresRootContainer);
			}
		}

		// Token: 0x06001BDF RID: 7135 RVA: 0x000A1BE8 File Offset: 0x0009FDE8
		public void SetGameoverInfo(int amount, int? localPlacement, int? globalPlacement)
		{
			string text;
			if (globalPlacement != null)
			{
				DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(1, 1);
				defaultInterpolatedStringHandler.AppendLiteral("#");
				defaultInterpolatedStringHandler.AppendFormatted<int?>(globalPlacement);
				text = defaultInterpolatedStringHandler.ToStringAndClear();
			}
			else
			{
				text = "-";
			}
			string item = text;
			string text2;
			if (localPlacement != null)
			{
				DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(1, 1);
				defaultInterpolatedStringHandler.AppendLiteral("#");
				defaultInterpolatedStringHandler.AppendFormatted<int?>(localPlacement);
				text2 = defaultInterpolatedStringHandler.ToStringAndClear();
			}
			else
			{
				text2 = "-";
			}
			string item2 = text2;
			this._finalScoreLabel.Text = Loc.GetString("blockgame-menu-gameover-info", new ValueTuple<string, object>[]
			{
				new ValueTuple<string, object>("global", item),
				new ValueTuple<string, object>("local", item2),
				new ValueTuple<string, object>("points", amount)
			});
		}

		// Token: 0x06001BE0 RID: 7136 RVA: 0x000A1CB5 File Offset: 0x0009FEB5
		public void UpdatePoints(int points)
		{
			this._pointsLabel.Text = Loc.GetString("blockgame-menu-label-points", new ValueTuple<string, object>[]
			{
				new ValueTuple<string, object>("points", points)
			});
		}

		// Token: 0x06001BE1 RID: 7137 RVA: 0x000A1CE9 File Offset: 0x0009FEE9
		public void UpdateLevel(int level)
		{
			this._levelLabel.Text = Loc.GetString("blockgame-menu-label-level", new ValueTuple<string, object>[]
			{
				new ValueTuple<string, object>("level", level + 1)
			});
		}

		// Token: 0x06001BE2 RID: 7138 RVA: 0x000A1D20 File Offset: 0x0009FF20
		public void UpdateHighscores(List<BlockGameMessages.HighScoreEntry> localHighscores, List<BlockGameMessages.HighScoreEntry> globalHighscores)
		{
			StringBuilder stringBuilder = new StringBuilder(Loc.GetString("blockgame-menu-text-station") + "\n");
			StringBuilder stringBuilder2 = new StringBuilder(Loc.GetString("blockgame-menu-text-nanotrasen") + "\n");
			for (int i = 0; i < 5; i++)
			{
				StringBuilder stringBuilder3 = stringBuilder;
				string value;
				if (localHighscores.Count <= i)
				{
					DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(10, 1);
					defaultInterpolatedStringHandler.AppendLiteral("#");
					defaultInterpolatedStringHandler.AppendFormatted<int>(i + 1);
					defaultInterpolatedStringHandler.AppendLiteral(": ??? - 0");
					value = defaultInterpolatedStringHandler.ToStringAndClear();
				}
				else
				{
					DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(6, 3);
					defaultInterpolatedStringHandler.AppendLiteral("#");
					defaultInterpolatedStringHandler.AppendFormatted<int>(i + 1);
					defaultInterpolatedStringHandler.AppendLiteral(": ");
					defaultInterpolatedStringHandler.AppendFormatted(localHighscores[i].Name);
					defaultInterpolatedStringHandler.AppendLiteral(" - ");
					defaultInterpolatedStringHandler.AppendFormatted<int>(localHighscores[i].Score);
					value = defaultInterpolatedStringHandler.ToStringAndClear();
				}
				stringBuilder3.AppendLine(value);
				StringBuilder stringBuilder4 = stringBuilder2;
				string value2;
				if (globalHighscores.Count <= i)
				{
					DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(10, 1);
					defaultInterpolatedStringHandler.AppendLiteral("#");
					defaultInterpolatedStringHandler.AppendFormatted<int>(i + 1);
					defaultInterpolatedStringHandler.AppendLiteral(": ??? - 0");
					value2 = defaultInterpolatedStringHandler.ToStringAndClear();
				}
				else
				{
					DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(6, 3);
					defaultInterpolatedStringHandler.AppendLiteral("#");
					defaultInterpolatedStringHandler.AppendFormatted<int>(i + 1);
					defaultInterpolatedStringHandler.AppendLiteral(": ");
					defaultInterpolatedStringHandler.AppendFormatted(globalHighscores[i].Name);
					defaultInterpolatedStringHandler.AppendLiteral(" - ");
					defaultInterpolatedStringHandler.AppendFormatted<int>(globalHighscores[i].Score);
					value2 = defaultInterpolatedStringHandler.ToStringAndClear();
				}
				stringBuilder4.AppendLine(value2);
			}
			this._localHighscoresLabel.Text = stringBuilder.ToString();
			this._globalHighscoresLabel.Text = stringBuilder2.ToString();
		}

		// Token: 0x06001BE3 RID: 7139 RVA: 0x000A1EE8 File Offset: 0x000A00E8
		protected override void KeyBindDown(GUIBoundKeyEventArgs args)
		{
			base.KeyBindDown(args);
			if (!this._isPlayer || args.Handled)
			{
				return;
			}
			if (args.Function == ContentKeyFunctions.ArcadeLeft)
			{
				this._owner.SendAction(BlockGamePlayerAction.StartLeft);
				return;
			}
			if (args.Function == ContentKeyFunctions.ArcadeRight)
			{
				this._owner.SendAction(BlockGamePlayerAction.StartRight);
				return;
			}
			if (args.Function == ContentKeyFunctions.ArcadeUp)
			{
				this._owner.SendAction(BlockGamePlayerAction.Rotate);
				return;
			}
			if (args.Function == ContentKeyFunctions.Arcade3)
			{
				this._owner.SendAction(BlockGamePlayerAction.CounterRotate);
				return;
			}
			if (args.Function == ContentKeyFunctions.ArcadeDown)
			{
				this._owner.SendAction(BlockGamePlayerAction.SoftdropStart);
				return;
			}
			if (args.Function == ContentKeyFunctions.Arcade2)
			{
				this._owner.SendAction(BlockGamePlayerAction.Hold);
				return;
			}
			if (args.Function == ContentKeyFunctions.Arcade1)
			{
				this._owner.SendAction(BlockGamePlayerAction.Harddrop);
			}
		}

		// Token: 0x06001BE4 RID: 7140 RVA: 0x000A1FE8 File Offset: 0x000A01E8
		protected override void KeyBindUp(GUIBoundKeyEventArgs args)
		{
			base.KeyBindUp(args);
			if (!this._isPlayer || args.Handled)
			{
				return;
			}
			if (args.Function == ContentKeyFunctions.ArcadeLeft)
			{
				this._owner.SendAction(BlockGamePlayerAction.EndLeft);
				return;
			}
			if (args.Function == ContentKeyFunctions.ArcadeRight)
			{
				this._owner.SendAction(BlockGamePlayerAction.EndRight);
				return;
			}
			if (args.Function == ContentKeyFunctions.ArcadeDown)
			{
				this._owner.SendAction(BlockGamePlayerAction.SoftdropEnd);
			}
		}

		// Token: 0x06001BE5 RID: 7141 RVA: 0x000A206C File Offset: 0x000A026C
		public void UpdateNextBlock(BlockGameBlock[] blocks)
		{
			this._nextBlockGrid.RemoveAllChildren();
			if (blocks.Length == 0)
			{
				return;
			}
			int num = blocks.Max((BlockGameBlock b) => b.Position.X) + 1;
			int num2 = blocks.Max((BlockGameBlock b) => b.Position.Y) + 1;
			this._nextBlockGrid.Columns = num;
			for (int i = 0; i < num2; i++)
			{
				for (int j = 0; j < num; j++)
				{
					Color colorForPosition = this.GetColorForPosition(blocks, j, i);
					this._nextBlockGrid.AddChild(new PanelContainer
					{
						PanelOverride = new StyleBoxFlat
						{
							BackgroundColor = colorForPosition
						},
						MinSize = BlockGameMenu.BlockSize,
						RectDrawClipMargin = 0
					});
				}
			}
		}

		// Token: 0x06001BE6 RID: 7142 RVA: 0x000A2140 File Offset: 0x000A0340
		public void UpdateHeldBlock(BlockGameBlock[] blocks)
		{
			this._holdBlockGrid.RemoveAllChildren();
			if (blocks.Length == 0)
			{
				return;
			}
			int num = blocks.Max((BlockGameBlock b) => b.Position.X) + 1;
			int num2 = blocks.Max((BlockGameBlock b) => b.Position.Y) + 1;
			this._holdBlockGrid.Columns = num;
			for (int i = 0; i < num2; i++)
			{
				for (int j = 0; j < num; j++)
				{
					Color colorForPosition = this.GetColorForPosition(blocks, j, i);
					this._holdBlockGrid.AddChild(new PanelContainer
					{
						PanelOverride = new StyleBoxFlat
						{
							BackgroundColor = colorForPosition
						},
						MinSize = BlockGameMenu.BlockSize,
						RectDrawClipMargin = 0
					});
				}
			}
		}

		// Token: 0x06001BE7 RID: 7143 RVA: 0x000A2214 File Offset: 0x000A0414
		public void UpdateBlocks(BlockGameBlock[] blocks)
		{
			this._gameGrid.RemoveAllChildren();
			for (int i = 0; i < 20; i++)
			{
				for (int j = 0; j < 10; j++)
				{
					Color colorForPosition = this.GetColorForPosition(blocks, j, i);
					this._gameGrid.AddChild(new PanelContainer
					{
						PanelOverride = new StyleBoxFlat
						{
							BackgroundColor = colorForPosition
						},
						MinSize = BlockGameMenu.BlockSize,
						RectDrawClipMargin = 0
					});
				}
			}
		}

		// Token: 0x06001BE8 RID: 7144 RVA: 0x000A2284 File Offset: 0x000A0484
		private Color GetColorForPosition(BlockGameBlock[] blocks, int x, int y)
		{
			Color result = Color.Transparent;
			BlockGameBlock? blockGameBlock = Extensions.FirstOrNull<BlockGameBlock>(blocks, (BlockGameBlock b) => b.Position.X == x && b.Position.Y == y);
			if (blockGameBlock != null)
			{
				result = BlockGameBlock.ToColor(blockGameBlock.Value.GameBlockColor);
			}
			return result;
		}

		// Token: 0x04000DEE RID: 3566
		private static readonly Color OverlayBackgroundColor = new Color(74, 74, 81, 180);

		// Token: 0x04000DEF RID: 3567
		private static readonly Color OverlayShadowColor = new Color(0, 0, 0, 83);

		// Token: 0x04000DF0 RID: 3568
		private static readonly Vector2 BlockSize = new Vector2(15f, 15f);

		// Token: 0x04000DF1 RID: 3569
		private readonly BlockGameBoundUserInterface _owner;

		// Token: 0x04000DF2 RID: 3570
		private readonly PanelContainer _mainPanel;

		// Token: 0x04000DF3 RID: 3571
		private BoxContainer _gameRootContainer;

		// Token: 0x04000DF4 RID: 3572
		private GridContainer _gameGrid;

		// Token: 0x04000DF5 RID: 3573
		private GridContainer _nextBlockGrid;

		// Token: 0x04000DF6 RID: 3574
		private GridContainer _holdBlockGrid;

		// Token: 0x04000DF7 RID: 3575
		private readonly Label _pointsLabel;

		// Token: 0x04000DF8 RID: 3576
		private readonly Label _levelLabel;

		// Token: 0x04000DF9 RID: 3577
		private readonly Button _pauseButton;

		// Token: 0x04000DFA RID: 3578
		private readonly PanelContainer _menuRootContainer;

		// Token: 0x04000DFB RID: 3579
		private readonly Button _unpauseButton;

		// Token: 0x04000DFC RID: 3580
		private readonly Control _unpauseButtonMargin;

		// Token: 0x04000DFD RID: 3581
		private readonly Button _newGameButton;

		// Token: 0x04000DFE RID: 3582
		private readonly Button _scoreBoardButton;

		// Token: 0x04000DFF RID: 3583
		private readonly PanelContainer _gameOverRootContainer;

		// Token: 0x04000E00 RID: 3584
		private readonly Label _finalScoreLabel;

		// Token: 0x04000E01 RID: 3585
		private readonly Button _finalNewGameButton;

		// Token: 0x04000E02 RID: 3586
		private readonly PanelContainer _highscoresRootContainer;

		// Token: 0x04000E03 RID: 3587
		private readonly Label _localHighscoresLabel;

		// Token: 0x04000E04 RID: 3588
		private readonly Label _globalHighscoresLabel;

		// Token: 0x04000E05 RID: 3589
		private readonly Button _highscoreBackButton;

		// Token: 0x04000E06 RID: 3590
		private bool _isPlayer;

		// Token: 0x04000E07 RID: 3591
		private bool _gameOver;
	}
}
