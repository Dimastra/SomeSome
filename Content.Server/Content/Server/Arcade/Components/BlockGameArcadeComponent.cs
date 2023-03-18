using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Content.Server.Power.Components;
using Content.Server.UserInterface;
using Content.Shared.Arcade;
using Robust.Server.GameObjects;
using Robust.Server.Player;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Maths;
using Robust.Shared.Random;

namespace Content.Server.Arcade.Components
{
	// Token: 0x020007C1 RID: 1985
	[NullableContext(1)]
	[Nullable(0)]
	[RegisterComponent]
	public sealed class BlockGameArcadeComponent : Component
	{
		// Token: 0x170006AE RID: 1710
		// (get) Token: 0x06002B12 RID: 11026 RVA: 0x000E1470 File Offset: 0x000DF670
		public bool Powered
		{
			get
			{
				ApcPowerReceiverComponent powerReceiverComponent;
				return this._entityManager.TryGetComponent<ApcPowerReceiverComponent>(base.Owner, ref powerReceiverComponent) && powerReceiverComponent.Powered;
			}
		}

		// Token: 0x170006AF RID: 1711
		// (get) Token: 0x06002B13 RID: 11027 RVA: 0x000E149A File Offset: 0x000DF69A
		[Nullable(2)]
		public BoundUserInterface UserInterface
		{
			[NullableContext(2)]
			get
			{
				return base.Owner.GetUIOrNull(BlockGameUiKey.Key);
			}
		}

		// Token: 0x06002B14 RID: 11028 RVA: 0x000E14AD File Offset: 0x000DF6AD
		public void RegisterPlayerSession(IPlayerSession session)
		{
			if (this._player == null)
			{
				this._player = session;
			}
			else
			{
				this._spectators.Add(session);
			}
			this.UpdatePlayerStatus(session);
			BlockGameArcadeComponent.BlockGame game = this._game;
			if (game == null)
			{
				return;
			}
			game.UpdateNewPlayerUI(session);
		}

		// Token: 0x06002B15 RID: 11029 RVA: 0x000E14E4 File Offset: 0x000DF6E4
		private void DeactivePlayer(IPlayerSession session)
		{
			if (this._player != session)
			{
				return;
			}
			IPlayerSession temp = this._player;
			this._player = null;
			if (this._spectators.Count != 0)
			{
				this._player = this._spectators[0];
				this._spectators.Remove(this._player);
				this.UpdatePlayerStatus(this._player);
			}
			this._spectators.Add(temp);
			this.UpdatePlayerStatus(temp);
		}

		// Token: 0x06002B16 RID: 11030 RVA: 0x000E1559 File Offset: 0x000DF759
		public void UnRegisterPlayerSession(IPlayerSession session)
		{
			if (this._player == session)
			{
				this.DeactivePlayer(this._player);
				return;
			}
			this._spectators.Remove(session);
			this.UpdatePlayerStatus(session);
		}

		// Token: 0x06002B17 RID: 11031 RVA: 0x000E1585 File Offset: 0x000DF785
		private void UpdatePlayerStatus(IPlayerSession session)
		{
			BoundUserInterface userInterface = this.UserInterface;
			if (userInterface == null)
			{
				return;
			}
			userInterface.SendMessage(new BlockGameMessages.BlockGameUserStatusMessage(this._player == session), session);
		}

		// Token: 0x06002B18 RID: 11032 RVA: 0x000E15A6 File Offset: 0x000DF7A6
		protected override void Initialize()
		{
			base.Initialize();
			if (this.UserInterface != null)
			{
				this.UserInterface.OnReceiveMessage += this.UserInterfaceOnOnReceiveMessage;
			}
			this._game = new BlockGameArcadeComponent.BlockGame(this);
		}

		// Token: 0x06002B19 RID: 11033 RVA: 0x000E15D9 File Offset: 0x000DF7D9
		public void OnPowerStateChanged(PowerChangedEvent e)
		{
			if (e.Powered)
			{
				return;
			}
			BoundUserInterface userInterface = this.UserInterface;
			if (userInterface != null)
			{
				userInterface.CloseAll();
			}
			this._player = null;
			this._spectators.Clear();
		}

		// Token: 0x06002B1A RID: 11034 RVA: 0x000E1608 File Offset: 0x000DF808
		private void UserInterfaceOnOnReceiveMessage(ServerBoundUserInterfaceMessage obj)
		{
			BlockGameMessages.BlockGamePlayerActionMessage playerActionMessage = obj.Message as BlockGameMessages.BlockGamePlayerActionMessage;
			if (playerActionMessage != null && obj.Session == this._player)
			{
				if (playerActionMessage.PlayerAction == BlockGamePlayerAction.NewGame)
				{
					BlockGameArcadeComponent.BlockGame game = this._game;
					if (game != null && game.Started)
					{
						this._game = new BlockGameArcadeComponent.BlockGame(this);
					}
					BlockGameArcadeComponent.BlockGame game2 = this._game;
					if (game2 == null)
					{
						return;
					}
					game2.StartGame();
					return;
				}
				else
				{
					BlockGameArcadeComponent.BlockGame game3 = this._game;
					if (game3 == null)
					{
						return;
					}
					game3.ProcessInput(playerActionMessage.PlayerAction);
				}
			}
		}

		// Token: 0x06002B1B RID: 11035 RVA: 0x000E1681 File Offset: 0x000DF881
		public void DoGameTick(float frameTime)
		{
			BlockGameArcadeComponent.BlockGame game = this._game;
			if (game == null)
			{
				return;
			}
			game.GameTick(frameTime);
		}

		// Token: 0x04001A99 RID: 6809
		[Dependency]
		private readonly IRobustRandom _random;

		// Token: 0x04001A9A RID: 6810
		[Dependency]
		private readonly IEntityManager _entityManager;

		// Token: 0x04001A9B RID: 6811
		[Nullable(2)]
		private BlockGameArcadeComponent.BlockGame _game;

		// Token: 0x04001A9C RID: 6812
		[Nullable(2)]
		private IPlayerSession _player;

		// Token: 0x04001A9D RID: 6813
		private readonly List<IPlayerSession> _spectators = new List<IPlayerSession>();

		// Token: 0x02000B35 RID: 2869
		[Nullable(0)]
		private sealed class BlockGame
		{
			// Token: 0x170008CF RID: 2255
			// (get) Token: 0x0600388B RID: 14475 RVA: 0x00125EE0 File Offset: 0x001240E0
			// (set) Token: 0x0600388C RID: 14476 RVA: 0x00125EE8 File Offset: 0x001240E8
			private BlockGameArcadeComponent.BlockGame.BlockGamePiece _nextPiece
			{
				get
				{
					return this._internalNextPiece;
				}
				set
				{
					this._internalNextPiece = value;
					this.SendNextPieceUpdate();
				}
			}

			// Token: 0x0600388D RID: 14477 RVA: 0x00125EF8 File Offset: 0x001240F8
			private void SendNextPieceUpdate()
			{
				BoundUserInterface userInterface = this._component.UserInterface;
				if (userInterface == null)
				{
					return;
				}
				userInterface.SendMessage(new BlockGameMessages.BlockGameVisualUpdateMessage(this._nextPiece.BlocksForPreview(), BlockGameMessages.BlockGameVisualType.NextBlock));
			}

			// Token: 0x0600388E RID: 14478 RVA: 0x00125F30 File Offset: 0x00124130
			private void SendNextPieceUpdate(IPlayerSession session)
			{
				BoundUserInterface userInterface = this._component.UserInterface;
				if (userInterface == null)
				{
					return;
				}
				userInterface.SendMessage(new BlockGameMessages.BlockGameVisualUpdateMessage(this._nextPiece.BlocksForPreview(), BlockGameMessages.BlockGameVisualType.NextBlock), session);
			}

			// Token: 0x170008D0 RID: 2256
			// (get) Token: 0x0600388F RID: 14479 RVA: 0x00125F67 File Offset: 0x00124167
			// (set) Token: 0x06003890 RID: 14480 RVA: 0x00125F6F File Offset: 0x0012416F
			private BlockGameArcadeComponent.BlockGame.BlockGamePiece? _heldPiece
			{
				get
				{
					return this._internalHeldPiece;
				}
				set
				{
					this._internalHeldPiece = value;
					this.SendHoldPieceUpdate();
				}
			}

			// Token: 0x06003891 RID: 14481 RVA: 0x00125F80 File Offset: 0x00124180
			private void SendHoldPieceUpdate()
			{
				if (this._heldPiece != null)
				{
					BoundUserInterface userInterface = this._component.UserInterface;
					if (userInterface == null)
					{
						return;
					}
					userInterface.SendMessage(new BlockGameMessages.BlockGameVisualUpdateMessage(this._heldPiece.Value.BlocksForPreview(), BlockGameMessages.BlockGameVisualType.HoldBlock));
					return;
				}
				else
				{
					BoundUserInterface userInterface2 = this._component.UserInterface;
					if (userInterface2 == null)
					{
						return;
					}
					userInterface2.SendMessage(new BlockGameMessages.BlockGameVisualUpdateMessage(new BlockGameBlock[0], BlockGameMessages.BlockGameVisualType.HoldBlock));
					return;
				}
			}

			// Token: 0x06003892 RID: 14482 RVA: 0x00125FF0 File Offset: 0x001241F0
			private void SendHoldPieceUpdate(IPlayerSession session)
			{
				if (this._heldPiece != null)
				{
					BoundUserInterface userInterface = this._component.UserInterface;
					if (userInterface == null)
					{
						return;
					}
					userInterface.SendMessage(new BlockGameMessages.BlockGameVisualUpdateMessage(this._heldPiece.Value.BlocksForPreview(), BlockGameMessages.BlockGameVisualType.HoldBlock), session);
					return;
				}
				else
				{
					BoundUserInterface userInterface2 = this._component.UserInterface;
					if (userInterface2 == null)
					{
						return;
					}
					userInterface2.SendMessage(new BlockGameMessages.BlockGameVisualUpdateMessage(new BlockGameBlock[0], BlockGameMessages.BlockGameVisualType.HoldBlock), session);
					return;
				}
			}

			// Token: 0x170008D1 RID: 2257
			// (get) Token: 0x06003893 RID: 14483 RVA: 0x00126062 File Offset: 0x00124262
			private float Speed
			{
				get
				{
					return -0.03f * (float)this.Level + 1f * ((!this._softDropPressed) ? 1f : this._softDropModifier);
				}
			}

			// Token: 0x170008D2 RID: 2258
			// (get) Token: 0x06003894 RID: 14484 RVA: 0x0012608D File Offset: 0x0012428D
			public bool Paused
			{
				get
				{
					return !this._running || !this._started;
				}
			}

			// Token: 0x170008D3 RID: 2259
			// (get) Token: 0x06003895 RID: 14485 RVA: 0x001260A2 File Offset: 0x001242A2
			public bool Started
			{
				get
				{
					return this._started;
				}
			}

			// Token: 0x170008D4 RID: 2260
			// (get) Token: 0x06003896 RID: 14486 RVA: 0x001260AA File Offset: 0x001242AA
			// (set) Token: 0x06003897 RID: 14487 RVA: 0x001260B2 File Offset: 0x001242B2
			private int Points
			{
				get
				{
					return this._internalPoints;
				}
				set
				{
					if (this._internalPoints == value)
					{
						return;
					}
					this._internalPoints = value;
					this.SendPointsUpdate();
				}
			}

			// Token: 0x06003898 RID: 14488 RVA: 0x001260CB File Offset: 0x001242CB
			private void SendPointsUpdate()
			{
				BoundUserInterface userInterface = this._component.UserInterface;
				if (userInterface == null)
				{
					return;
				}
				userInterface.SendMessage(new BlockGameMessages.BlockGameScoreUpdateMessage(this.Points));
			}

			// Token: 0x06003899 RID: 14489 RVA: 0x001260ED File Offset: 0x001242ED
			private void SendPointsUpdate(IPlayerSession session)
			{
				BoundUserInterface userInterface = this._component.UserInterface;
				if (userInterface == null)
				{
					return;
				}
				userInterface.SendMessage(new BlockGameMessages.BlockGameScoreUpdateMessage(this.Points));
			}

			// Token: 0x170008D5 RID: 2261
			// (get) Token: 0x0600389A RID: 14490 RVA: 0x0012610F File Offset: 0x0012430F
			// (set) Token: 0x0600389B RID: 14491 RVA: 0x00126117 File Offset: 0x00124317
			public int Level
			{
				get
				{
					return this._level;
				}
				set
				{
					this._level = value;
					this.SendLevelUpdate();
				}
			}

			// Token: 0x0600389C RID: 14492 RVA: 0x00126126 File Offset: 0x00124326
			private void SendLevelUpdate()
			{
				BoundUserInterface userInterface = this._component.UserInterface;
				if (userInterface == null)
				{
					return;
				}
				userInterface.SendMessage(new BlockGameMessages.BlockGameLevelUpdateMessage(this.Level));
			}

			// Token: 0x0600389D RID: 14493 RVA: 0x00126148 File Offset: 0x00124348
			private void SendLevelUpdate(IPlayerSession session)
			{
				BoundUserInterface userInterface = this._component.UserInterface;
				if (userInterface == null)
				{
					return;
				}
				userInterface.SendMessage(new BlockGameMessages.BlockGameLevelUpdateMessage(this.Level));
			}

			// Token: 0x170008D6 RID: 2262
			// (get) Token: 0x0600389E RID: 14494 RVA: 0x0012616A File Offset: 0x0012436A
			// (set) Token: 0x0600389F RID: 14495 RVA: 0x00126174 File Offset: 0x00124374
			private int ClearedLines
			{
				get
				{
					return this._clearedLines;
				}
				set
				{
					this._clearedLines = value;
					if (this._clearedLines < this.LevelRequirement)
					{
						return;
					}
					this._clearedLines -= this.LevelRequirement;
					int level = this.Level;
					this.Level = level + 1;
				}
			}

			// Token: 0x170008D7 RID: 2263
			// (get) Token: 0x060038A0 RID: 14496 RVA: 0x001261BA File Offset: 0x001243BA
			private int LevelRequirement
			{
				get
				{
					return Math.Min(100, Math.Max(this.Level * 10 - 50, 10));
				}
			}

			// Token: 0x060038A1 RID: 14497 RVA: 0x001261D8 File Offset: 0x001243D8
			public BlockGame(BlockGameArcadeComponent component)
			{
				this._component = component;
				this._allBlockGamePieces = (BlockGameArcadeComponent.BlockGame.BlockGamePieceType[])Enum.GetValues(typeof(BlockGameArcadeComponent.BlockGame.BlockGamePieceType));
				this._internalNextPiece = this.GetRandomBlockGamePiece(this._component._random);
				this.InitializeNewBlock();
			}

			// Token: 0x060038A2 RID: 14498 RVA: 0x0012624C File Offset: 0x0012444C
			private void SendHighscoreUpdate()
			{
				ArcadeSystem entitySystem = EntitySystem.Get<ArcadeSystem>();
				BoundUserInterface userInterface = this._component.UserInterface;
				if (userInterface == null)
				{
					return;
				}
				userInterface.SendMessage(new BlockGameMessages.BlockGameHighScoreUpdateMessage(entitySystem.GetLocalHighscores(), entitySystem.GetGlobalHighscores()));
			}

			// Token: 0x060038A3 RID: 14499 RVA: 0x00126288 File Offset: 0x00124488
			private void SendHighscoreUpdate(IPlayerSession session)
			{
				ArcadeSystem entitySystem = EntitySystem.Get<ArcadeSystem>();
				BoundUserInterface userInterface = this._component.UserInterface;
				if (userInterface == null)
				{
					return;
				}
				userInterface.SendMessage(new BlockGameMessages.BlockGameHighScoreUpdateMessage(entitySystem.GetLocalHighscores(), entitySystem.GetGlobalHighscores()), session);
			}

			// Token: 0x060038A4 RID: 14500 RVA: 0x001262C2 File Offset: 0x001244C2
			public void StartGame()
			{
				BoundUserInterface userInterface = this._component.UserInterface;
				if (userInterface != null)
				{
					userInterface.SendMessage(new BlockGameMessages.BlockGameSetScreenMessage(BlockGameMessages.BlockGameScreen.Game, true));
				}
				this.FullUpdate();
				this._running = true;
				this._started = true;
			}

			// Token: 0x060038A5 RID: 14501 RVA: 0x001262F5 File Offset: 0x001244F5
			private void FullUpdate()
			{
				this.UpdateAllFieldUI();
				this.SendHoldPieceUpdate();
				this.SendNextPieceUpdate();
				this.SendPointsUpdate();
				this.SendHighscoreUpdate();
				this.SendLevelUpdate();
			}

			// Token: 0x060038A6 RID: 14502 RVA: 0x0012631B File Offset: 0x0012451B
			private void FullUpdate(IPlayerSession session)
			{
				this.UpdateFieldUI(session);
				this.SendPointsUpdate(session);
				this.SendNextPieceUpdate(session);
				this.SendHoldPieceUpdate(session);
				this.SendHighscoreUpdate(session);
				this.SendLevelUpdate(session);
			}

			// Token: 0x060038A7 RID: 14503 RVA: 0x00126347 File Offset: 0x00124547
			public void GameTick(float frameTime)
			{
				if (!this._running)
				{
					return;
				}
				this.InputTick(frameTime);
				this.FieldTick(frameTime);
			}

			// Token: 0x060038A8 RID: 14504 RVA: 0x00126360 File Offset: 0x00124560
			private void InputTick(float frameTime)
			{
				bool anythingChanged = false;
				if (this._leftPressed)
				{
					this._accumulatedLeftPressTime += frameTime;
					while (this._accumulatedLeftPressTime >= 0.08f)
					{
						if (this._currentPiece.Positions(this._currentPiecePosition.AddToX(-1), this._currentRotation).All(new Func<Vector2i, bool>(this.MoveCheck)))
						{
							this._currentPiecePosition = this._currentPiecePosition.AddToX(-1);
							anythingChanged = true;
						}
						this._accumulatedLeftPressTime -= 0.08f;
					}
				}
				if (this._rightPressed)
				{
					this._accumulatedRightPressTime += frameTime;
					while (this._accumulatedRightPressTime >= 0.08f)
					{
						if (this._currentPiece.Positions(this._currentPiecePosition.AddToX(1), this._currentRotation).All(new Func<Vector2i, bool>(this.MoveCheck)))
						{
							this._currentPiecePosition = this._currentPiecePosition.AddToX(1);
							anythingChanged = true;
						}
						this._accumulatedRightPressTime -= 0.08f;
					}
				}
				if (anythingChanged)
				{
					this.UpdateAllFieldUI();
				}
			}

			// Token: 0x060038A9 RID: 14505 RVA: 0x00126470 File Offset: 0x00124670
			private void FieldTick(float frameTime)
			{
				this._accumulatedFieldFrameTime += frameTime;
				float checkTime = Math.Max(0.03f, this.Speed);
				while (this._accumulatedFieldFrameTime >= checkTime)
				{
					if (this._softDropPressed)
					{
						this.AddPoints(1);
					}
					this.InternalFieldTick();
					this._accumulatedFieldFrameTime -= checkTime;
				}
			}

			// Token: 0x060038AA RID: 14506 RVA: 0x001264CC File Offset: 0x001246CC
			private void InternalFieldTick()
			{
				if (this._currentPiece.Positions(this._currentPiecePosition.AddToY(1), this._currentRotation).All(new Func<Vector2i, bool>(this.DropCheck)))
				{
					this._currentPiecePosition = this._currentPiecePosition.AddToY(1);
				}
				else
				{
					BlockGameBlock[] blocks = this._currentPiece.Blocks(this._currentPiecePosition, this._currentRotation);
					this._field.AddRange(blocks);
					if (this.IsGameOver)
					{
						this.InvokeGameover();
						return;
					}
					this.InitializeNewBlock();
				}
				this.CheckField();
				this.UpdateAllFieldUI();
			}

			// Token: 0x060038AB RID: 14507 RVA: 0x00126564 File Offset: 0x00124764
			private void CheckField()
			{
				int pointsToAdd = 0;
				int consecutiveLines = 0;
				int clearedLines = 0;
				for (int y = 0; y < 20; y++)
				{
					if (this.CheckLine(y))
					{
						y--;
						consecutiveLines++;
						clearedLines++;
					}
					else if (consecutiveLines != 0)
					{
						int num;
						switch (consecutiveLines)
						{
						case 1:
							num = 40;
							break;
						case 2:
							num = 100;
							break;
						case 3:
							num = 300;
							break;
						case 4:
							num = 1200;
							break;
						default:
							num = 0;
							break;
						}
						int mod = num;
						pointsToAdd += mod * (this._level + 1);
					}
				}
				this.ClearedLines += clearedLines;
				this.AddPoints(pointsToAdd);
			}

			// Token: 0x060038AC RID: 14508 RVA: 0x00126600 File Offset: 0x00124800
			private bool CheckLine(int y)
			{
				BlockGameArcadeComponent.BlockGame.<>c__DisplayClass65_0 CS$<>8__locals1 = new BlockGameArcadeComponent.BlockGame.<>c__DisplayClass65_0();
				CS$<>8__locals1.y = y;
				int x;
				int x2;
				for (x = 0; x < 10; x = x2 + 1)
				{
					if (!this._field.Any((BlockGameBlock b) => b.Position.X == x && b.Position.Y == CS$<>8__locals1.y))
					{
						return false;
					}
					x2 = x;
				}
				this._field.RemoveAll((BlockGameBlock b) => b.Position.Y == CS$<>8__locals1.y);
				this.FillLine(CS$<>8__locals1.y);
				return true;
			}

			// Token: 0x060038AD RID: 14509 RVA: 0x0012668A File Offset: 0x0012488A
			private void AddPoints(int amount)
			{
				if (amount == 0)
				{
					return;
				}
				this.Points += amount;
			}

			// Token: 0x060038AE RID: 14510 RVA: 0x001266A0 File Offset: 0x001248A0
			private void FillLine(int y)
			{
				for (int c_y = y; c_y > 0; c_y--)
				{
					for (int i = 0; i < this._field.Count; i++)
					{
						if (this._field[i].Position.Y == c_y - 1)
						{
							this._field[i] = new BlockGameBlock(this._field[i].Position.AddToY(1), this._field[i].GameBlockColor);
						}
					}
				}
			}

			// Token: 0x060038AF RID: 14511 RVA: 0x00126724 File Offset: 0x00124924
			private void InitializeNewBlock()
			{
				this.InitializeNewBlock(this._nextPiece);
				this._nextPiece = this.GetRandomBlockGamePiece(this._component._random);
				this._holdBlock = false;
				BoundUserInterface userInterface = this._component.UserInterface;
				if (userInterface == null)
				{
					return;
				}
				userInterface.SendMessage(new BlockGameMessages.BlockGameVisualUpdateMessage(this._nextPiece.BlocksForPreview(), BlockGameMessages.BlockGameVisualType.NextBlock));
			}

			// Token: 0x060038B0 RID: 14512 RVA: 0x00126784 File Offset: 0x00124984
			private void InitializeNewBlock(BlockGameArcadeComponent.BlockGame.BlockGamePiece piece)
			{
				this._currentPiecePosition = new Vector2i(5, 0);
				this._currentRotation = BlockGameArcadeComponent.BlockGame.BlockGamePieceRotation.North;
				this._currentPiece = piece;
				this.UpdateAllFieldUI();
			}

			// Token: 0x060038B1 RID: 14513 RVA: 0x001267A7 File Offset: 0x001249A7
			private bool LowerBoundCheck(Vector2i position)
			{
				return position.Y < 20;
			}

			// Token: 0x060038B2 RID: 14514 RVA: 0x001267B3 File Offset: 0x001249B3
			private bool BorderCheck(Vector2i position)
			{
				return position.X >= 0 && position.X < 10;
			}

			// Token: 0x060038B3 RID: 14515 RVA: 0x001267CC File Offset: 0x001249CC
			private bool ClearCheck(Vector2i position)
			{
				return this._field.All((BlockGameBlock block) => !position.Equals(block.Position));
			}

			// Token: 0x060038B4 RID: 14516 RVA: 0x001267FD File Offset: 0x001249FD
			private bool DropCheck(Vector2i position)
			{
				return this.LowerBoundCheck(position) && this.ClearCheck(position);
			}

			// Token: 0x060038B5 RID: 14517 RVA: 0x00126811 File Offset: 0x00124A11
			private bool MoveCheck(Vector2i position)
			{
				return this.BorderCheck(position) && this.ClearCheck(position);
			}

			// Token: 0x060038B6 RID: 14518 RVA: 0x00126825 File Offset: 0x00124A25
			private bool RotateCheck(Vector2i position)
			{
				return this.BorderCheck(position) && this.LowerBoundCheck(position) && this.ClearCheck(position);
			}

			// Token: 0x060038B7 RID: 14519 RVA: 0x00126844 File Offset: 0x00124A44
			public void ProcessInput(BlockGamePlayerAction action)
			{
				if (this._running)
				{
					switch (action)
					{
					case BlockGamePlayerAction.StartLeft:
						this._leftPressed = true;
						break;
					case BlockGamePlayerAction.StartRight:
						this._rightPressed = true;
						break;
					case BlockGamePlayerAction.Rotate:
						this.TrySetRotation(BlockGameArcadeComponent.BlockGame.Next(this._currentRotation, false));
						break;
					case BlockGamePlayerAction.CounterRotate:
						this.TrySetRotation(BlockGameArcadeComponent.BlockGame.Next(this._currentRotation, true));
						break;
					case BlockGamePlayerAction.SoftdropStart:
						this._softDropPressed = true;
						if (this._accumulatedFieldFrameTime > this.Speed)
						{
							this._accumulatedFieldFrameTime = this.Speed;
						}
						break;
					case BlockGamePlayerAction.Harddrop:
						this.PerformHarddrop();
						break;
					case BlockGamePlayerAction.Hold:
						this.HoldPiece();
						break;
					}
				}
				if (action == BlockGamePlayerAction.EndLeft)
				{
					this._leftPressed = false;
					return;
				}
				if (action != BlockGamePlayerAction.EndRight)
				{
					switch (action)
					{
					case BlockGamePlayerAction.SoftdropEnd:
						this._softDropPressed = false;
						return;
					case BlockGamePlayerAction.Harddrop:
					case BlockGamePlayerAction.Hold:
						break;
					case BlockGamePlayerAction.Pause:
					{
						this._running = false;
						BoundUserInterface userInterface = this._component.UserInterface;
						if (userInterface == null)
						{
							return;
						}
						userInterface.SendMessage(new BlockGameMessages.BlockGameSetScreenMessage(BlockGameMessages.BlockGameScreen.Pause, this._started));
						return;
					}
					case BlockGamePlayerAction.Unpause:
						if (!this._gameOver && this._started)
						{
							this._running = true;
							BoundUserInterface userInterface2 = this._component.UserInterface;
							if (userInterface2 == null)
							{
								return;
							}
							userInterface2.SendMessage(new BlockGameMessages.BlockGameSetScreenMessage(BlockGameMessages.BlockGameScreen.Game, true));
							return;
						}
						break;
					case BlockGamePlayerAction.ShowHighscores:
					{
						this._running = false;
						BoundUserInterface userInterface3 = this._component.UserInterface;
						if (userInterface3 == null)
						{
							return;
						}
						userInterface3.SendMessage(new BlockGameMessages.BlockGameSetScreenMessage(BlockGameMessages.BlockGameScreen.Highscores, this._started));
						break;
					}
					default:
						return;
					}
					return;
				}
				this._rightPressed = false;
			}

			// Token: 0x060038B8 RID: 14520 RVA: 0x001269C8 File Offset: 0x00124BC8
			private void TrySetRotation(BlockGameArcadeComponent.BlockGame.BlockGamePieceRotation rotation)
			{
				if (!this._running)
				{
					return;
				}
				if (!this._currentPiece.CanSpin)
				{
					return;
				}
				if (!this._currentPiece.Positions(this._currentPiecePosition, rotation).All(new Func<Vector2i, bool>(this.RotateCheck)))
				{
					return;
				}
				this._currentRotation = rotation;
				this.UpdateAllFieldUI();
			}

			// Token: 0x060038B9 RID: 14521 RVA: 0x00126A20 File Offset: 0x00124C20
			private void HoldPiece()
			{
				if (!this._running)
				{
					return;
				}
				if (this._holdBlock)
				{
					return;
				}
				BlockGameArcadeComponent.BlockGame.BlockGamePiece? tempHeld = this._heldPiece;
				this._heldPiece = new BlockGameArcadeComponent.BlockGame.BlockGamePiece?(this._currentPiece);
				this._holdBlock = true;
				if (tempHeld == null)
				{
					this.InitializeNewBlock();
					return;
				}
				this.InitializeNewBlock(tempHeld.Value);
			}

			// Token: 0x060038BA RID: 14522 RVA: 0x00126A7C File Offset: 0x00124C7C
			private void PerformHarddrop()
			{
				int spacesDropped = 0;
				while (this._currentPiece.Positions(this._currentPiecePosition.AddToY(1), this._currentRotation).All(new Func<Vector2i, bool>(this.DropCheck)))
				{
					this._currentPiecePosition = this._currentPiecePosition.AddToY(1);
					spacesDropped++;
				}
				this.AddPoints(spacesDropped * 2);
				this.InternalFieldTick();
			}

			// Token: 0x060038BB RID: 14523 RVA: 0x00126AE4 File Offset: 0x00124CE4
			public void UpdateAllFieldUI()
			{
				if (!this._started)
				{
					return;
				}
				List<BlockGameBlock> computedField = this.ComputeField();
				BoundUserInterface userInterface = this._component.UserInterface;
				if (userInterface == null)
				{
					return;
				}
				userInterface.SendMessage(new BlockGameMessages.BlockGameVisualUpdateMessage(computedField.ToArray(), BlockGameMessages.BlockGameVisualType.GameField));
			}

			// Token: 0x060038BC RID: 14524 RVA: 0x00126B24 File Offset: 0x00124D24
			public void UpdateFieldUI(IPlayerSession session)
			{
				if (!this._started)
				{
					return;
				}
				List<BlockGameBlock> computedField = this.ComputeField();
				BoundUserInterface userInterface = this._component.UserInterface;
				if (userInterface == null)
				{
					return;
				}
				userInterface.SendMessage(new BlockGameMessages.BlockGameVisualUpdateMessage(computedField.ToArray(), BlockGameMessages.BlockGameVisualType.GameField), session);
			}

			// Token: 0x170008D8 RID: 2264
			// (get) Token: 0x060038BD RID: 14525 RVA: 0x00126B63 File Offset: 0x00124D63
			private bool IsGameOver
			{
				get
				{
					return this._field.Any((BlockGameBlock block) => block.Position.Y == 0);
				}
			}

			// Token: 0x060038BE RID: 14526 RVA: 0x00126B90 File Offset: 0x00124D90
			private void InvokeGameover()
			{
				this._running = false;
				this._gameOver = true;
				IPlayerSession player = this._component._player;
				EntityUid? entityUid = (player != null) ? player.AttachedEntity : null;
				if (entityUid != null)
				{
					EntityUid playerEntity = entityUid.GetValueOrDefault();
					if (playerEntity.Valid)
					{
						ArcadeSystem blockGameSystem = EntitySystem.Get<ArcadeSystem>();
						this._highScorePlacement = new ArcadeSystem.HighScorePlacement?(blockGameSystem.RegisterHighScore(IoCManager.Resolve<IEntityManager>().GetComponent<MetaDataComponent>(playerEntity).EntityName, this.Points));
						this.SendHighscoreUpdate();
					}
				}
				BoundUserInterface userInterface = this._component.UserInterface;
				if (userInterface == null)
				{
					return;
				}
				userInterface.SendMessage(new BlockGameMessages.BlockGameGameOverScreenMessage(this.Points, (this._highScorePlacement != null) ? this._highScorePlacement.GetValueOrDefault().LocalPlacement : null, (this._highScorePlacement != null) ? this._highScorePlacement.GetValueOrDefault().GlobalPlacement : null));
			}

			// Token: 0x060038BF RID: 14527 RVA: 0x00126C84 File Offset: 0x00124E84
			public void UpdateNewPlayerUI(IPlayerSession session)
			{
				if (this._gameOver)
				{
					BoundUserInterface userInterface = this._component.UserInterface;
					if (userInterface != null)
					{
						userInterface.SendMessage(new BlockGameMessages.BlockGameGameOverScreenMessage(this.Points, (this._highScorePlacement != null) ? this._highScorePlacement.GetValueOrDefault().LocalPlacement : null, (this._highScorePlacement != null) ? this._highScorePlacement.GetValueOrDefault().GlobalPlacement : null), session);
					}
				}
				else if (this.Paused)
				{
					BoundUserInterface userInterface2 = this._component.UserInterface;
					if (userInterface2 != null)
					{
						userInterface2.SendMessage(new BlockGameMessages.BlockGameSetScreenMessage(BlockGameMessages.BlockGameScreen.Pause, this.Started), session);
					}
				}
				else
				{
					BoundUserInterface userInterface3 = this._component.UserInterface;
					if (userInterface3 != null)
					{
						userInterface3.SendMessage(new BlockGameMessages.BlockGameSetScreenMessage(BlockGameMessages.BlockGameScreen.Game, this.Started), session);
					}
				}
				this.FullUpdate(session);
			}

			// Token: 0x060038C0 RID: 14528 RVA: 0x00126D60 File Offset: 0x00124F60
			public List<BlockGameBlock> ComputeField()
			{
				List<BlockGameBlock> result = new List<BlockGameBlock>();
				result.AddRange(this._field);
				result.AddRange(this._currentPiece.Blocks(this._currentPiecePosition, this._currentRotation));
				Vector2i dropGhostPosition = this._currentPiecePosition;
				while (this._currentPiece.Positions(dropGhostPosition.AddToY(1), this._currentRotation).All(new Func<Vector2i, bool>(this.DropCheck)))
				{
					dropGhostPosition = dropGhostPosition.AddToY(1);
				}
				if (dropGhostPosition != this._currentPiecePosition)
				{
					BlockGameBlock[] blox = this._currentPiece.Blocks(dropGhostPosition, this._currentRotation);
					for (int i = 0; i < blox.Length; i++)
					{
						result.Add(new BlockGameBlock(blox[i].Position, BlockGameBlock.ToGhostBlockColor(blox[i].GameBlockColor)));
					}
				}
				return result;
			}

			// Token: 0x060038C1 RID: 14529 RVA: 0x00126E30 File Offset: 0x00125030
			private static BlockGameArcadeComponent.BlockGame.BlockGamePieceRotation Next(BlockGameArcadeComponent.BlockGame.BlockGamePieceRotation rotation, bool inverted)
			{
				BlockGameArcadeComponent.BlockGame.BlockGamePieceRotation result;
				switch (rotation)
				{
				case BlockGameArcadeComponent.BlockGame.BlockGamePieceRotation.North:
					result = (inverted ? BlockGameArcadeComponent.BlockGame.BlockGamePieceRotation.West : BlockGameArcadeComponent.BlockGame.BlockGamePieceRotation.East);
					break;
				case BlockGameArcadeComponent.BlockGame.BlockGamePieceRotation.East:
					result = (inverted ? BlockGameArcadeComponent.BlockGame.BlockGamePieceRotation.North : BlockGameArcadeComponent.BlockGame.BlockGamePieceRotation.South);
					break;
				case BlockGameArcadeComponent.BlockGame.BlockGamePieceRotation.South:
					result = (inverted ? BlockGameArcadeComponent.BlockGame.BlockGamePieceRotation.East : BlockGameArcadeComponent.BlockGame.BlockGamePieceRotation.West);
					break;
				case BlockGameArcadeComponent.BlockGame.BlockGamePieceRotation.West:
					result = (inverted ? BlockGameArcadeComponent.BlockGame.BlockGamePieceRotation.South : BlockGameArcadeComponent.BlockGame.BlockGamePieceRotation.North);
					break;
				default:
					throw new ArgumentOutOfRangeException("rotation", rotation, null);
				}
				return result;
			}

			// Token: 0x060038C2 RID: 14530 RVA: 0x00126E90 File Offset: 0x00125090
			private BlockGameArcadeComponent.BlockGame.BlockGamePiece GetRandomBlockGamePiece(IRobustRandom random)
			{
				if (this._blockGamePiecesBuffer.Count == 0)
				{
					this._blockGamePiecesBuffer = this._allBlockGamePieces.ToList<BlockGameArcadeComponent.BlockGame.BlockGamePieceType>();
				}
				BlockGameArcadeComponent.BlockGame.BlockGamePieceType chosenPiece = RandomExtensions.Pick<BlockGameArcadeComponent.BlockGame.BlockGamePieceType>(random, this._blockGamePiecesBuffer);
				this._blockGamePiecesBuffer.Remove(chosenPiece);
				return BlockGameArcadeComponent.BlockGame.BlockGamePiece.GetPiece(chosenPiece);
			}

			// Token: 0x04002968 RID: 10600
			private readonly BlockGameArcadeComponent _component;

			// Token: 0x04002969 RID: 10601
			private readonly List<BlockGameBlock> _field = new List<BlockGameBlock>();

			// Token: 0x0400296A RID: 10602
			private BlockGameArcadeComponent.BlockGame.BlockGamePiece _currentPiece;

			// Token: 0x0400296B RID: 10603
			private BlockGameArcadeComponent.BlockGame.BlockGamePiece _internalNextPiece;

			// Token: 0x0400296C RID: 10604
			private bool _holdBlock;

			// Token: 0x0400296D RID: 10605
			private BlockGameArcadeComponent.BlockGame.BlockGamePiece? _internalHeldPiece;

			// Token: 0x0400296E RID: 10606
			private Vector2i _currentPiecePosition;

			// Token: 0x0400296F RID: 10607
			private BlockGameArcadeComponent.BlockGame.BlockGamePieceRotation _currentRotation;

			// Token: 0x04002970 RID: 10608
			private float _softDropModifier = 0.1f;

			// Token: 0x04002971 RID: 10609
			private const float _pressCheckSpeed = 0.08f;

			// Token: 0x04002972 RID: 10610
			private bool _running;

			// Token: 0x04002973 RID: 10611
			private bool _started;

			// Token: 0x04002974 RID: 10612
			private bool _gameOver;

			// Token: 0x04002975 RID: 10613
			private bool _leftPressed;

			// Token: 0x04002976 RID: 10614
			private bool _rightPressed;

			// Token: 0x04002977 RID: 10615
			private bool _softDropPressed;

			// Token: 0x04002978 RID: 10616
			private int _internalPoints;

			// Token: 0x04002979 RID: 10617
			private ArcadeSystem.HighScorePlacement? _highScorePlacement;

			// Token: 0x0400297A RID: 10618
			private int _level;

			// Token: 0x0400297B RID: 10619
			private int _clearedLines;

			// Token: 0x0400297C RID: 10620
			private float _accumulatedLeftPressTime;

			// Token: 0x0400297D RID: 10621
			private float _accumulatedRightPressTime;

			// Token: 0x0400297E RID: 10622
			private float _accumulatedFieldFrameTime;

			// Token: 0x0400297F RID: 10623
			private readonly BlockGameArcadeComponent.BlockGame.BlockGamePieceType[] _allBlockGamePieces;

			// Token: 0x04002980 RID: 10624
			private List<BlockGameArcadeComponent.BlockGame.BlockGamePieceType> _blockGamePiecesBuffer = new List<BlockGameArcadeComponent.BlockGame.BlockGamePieceType>();

			// Token: 0x02000BC5 RID: 3013
			[NullableContext(0)]
			private enum BlockGamePieceType
			{
				// Token: 0x04002C55 RID: 11349
				I,
				// Token: 0x04002C56 RID: 11350
				L,
				// Token: 0x04002C57 RID: 11351
				LInverted,
				// Token: 0x04002C58 RID: 11352
				S,
				// Token: 0x04002C59 RID: 11353
				SInverted,
				// Token: 0x04002C5A RID: 11354
				T,
				// Token: 0x04002C5B RID: 11355
				O
			}

			// Token: 0x02000BC6 RID: 3014
			[NullableContext(0)]
			private enum BlockGamePieceRotation
			{
				// Token: 0x04002C5D RID: 11357
				North,
				// Token: 0x04002C5E RID: 11358
				East,
				// Token: 0x04002C5F RID: 11359
				South,
				// Token: 0x04002C60 RID: 11360
				West
			}

			// Token: 0x02000BC7 RID: 3015
			[Nullable(0)]
			private struct BlockGamePiece
			{
				// Token: 0x06003AC2 RID: 15042 RVA: 0x00134C2C File Offset: 0x00132E2C
				public Vector2i[] Positions(Vector2i center, BlockGameArcadeComponent.BlockGame.BlockGamePieceRotation rotation)
				{
					return (from v in this.RotatedOffsets(rotation)
					select center + v).ToArray<Vector2i>();
				}

				// Token: 0x06003AC3 RID: 15043 RVA: 0x00134C64 File Offset: 0x00132E64
				private Vector2i[] RotatedOffsets(BlockGameArcadeComponent.BlockGame.BlockGamePieceRotation rotation)
				{
					Vector2i[] rotatedOffsets = (Vector2i[])this.Offsets.Clone();
					int num;
					switch (rotation)
					{
					case BlockGameArcadeComponent.BlockGame.BlockGamePieceRotation.North:
						num = 0;
						break;
					case BlockGameArcadeComponent.BlockGame.BlockGamePieceRotation.East:
						num = 1;
						break;
					case BlockGameArcadeComponent.BlockGame.BlockGamePieceRotation.South:
						num = 2;
						break;
					case BlockGameArcadeComponent.BlockGame.BlockGamePieceRotation.West:
						num = 3;
						break;
					default:
						num = 0;
						break;
					}
					int amount = num;
					for (int i = 0; i < amount; i++)
					{
						for (int j = 0; j < rotatedOffsets.Length; j++)
						{
							rotatedOffsets[j] = rotatedOffsets[j].Rotate90DegreesAsOffset();
						}
					}
					return rotatedOffsets;
				}

				// Token: 0x06003AC4 RID: 15044 RVA: 0x00134CE4 File Offset: 0x00132EE4
				public BlockGameBlock[] Blocks(Vector2i center, BlockGameArcadeComponent.BlockGame.BlockGamePieceRotation rotation)
				{
					Vector2i[] array = this.Positions(center, rotation);
					BlockGameBlock[] result = new BlockGameBlock[array.Length];
					int i = 0;
					foreach (Vector2i position in array)
					{
						result[i++] = position.ToBlockGameBlock(this._gameBlockColor);
					}
					return result;
				}

				// Token: 0x06003AC5 RID: 15045 RVA: 0x00134D38 File Offset: 0x00132F38
				public BlockGameBlock[] BlocksForPreview()
				{
					int xOffset = 0;
					int yOffset = 0;
					foreach (Vector2i offset in this.Offsets)
					{
						if (offset.X < xOffset)
						{
							xOffset = offset.X;
						}
						if (offset.Y < yOffset)
						{
							yOffset = offset.Y;
						}
					}
					return this.Blocks(new Vector2i(-xOffset, -yOffset), BlockGameArcadeComponent.BlockGame.BlockGamePieceRotation.North);
				}

				// Token: 0x06003AC6 RID: 15046 RVA: 0x00134D9C File Offset: 0x00132F9C
				public static BlockGameArcadeComponent.BlockGame.BlockGamePiece GetPiece(BlockGameArcadeComponent.BlockGame.BlockGamePieceType type)
				{
					BlockGameArcadeComponent.BlockGame.BlockGamePiece result;
					switch (type)
					{
					case BlockGameArcadeComponent.BlockGame.BlockGamePieceType.I:
						result = new BlockGameArcadeComponent.BlockGame.BlockGamePiece
						{
							Offsets = new Vector2i[]
							{
								new Vector2i(0, -1),
								new Vector2i(0, 0),
								new Vector2i(0, 1),
								new Vector2i(0, 2)
							},
							_gameBlockColor = BlockGameBlock.BlockGameBlockColor.LightBlue,
							CanSpin = true
						};
						break;
					case BlockGameArcadeComponent.BlockGame.BlockGamePieceType.L:
						result = new BlockGameArcadeComponent.BlockGame.BlockGamePiece
						{
							Offsets = new Vector2i[]
							{
								new Vector2i(0, -1),
								new Vector2i(0, 0),
								new Vector2i(0, 1),
								new Vector2i(1, 1)
							},
							_gameBlockColor = BlockGameBlock.BlockGameBlockColor.Orange,
							CanSpin = true
						};
						break;
					case BlockGameArcadeComponent.BlockGame.BlockGamePieceType.LInverted:
						result = new BlockGameArcadeComponent.BlockGame.BlockGamePiece
						{
							Offsets = new Vector2i[]
							{
								new Vector2i(0, -1),
								new Vector2i(0, 0),
								new Vector2i(-1, 1),
								new Vector2i(0, 1)
							},
							_gameBlockColor = BlockGameBlock.BlockGameBlockColor.Blue,
							CanSpin = true
						};
						break;
					case BlockGameArcadeComponent.BlockGame.BlockGamePieceType.S:
						result = new BlockGameArcadeComponent.BlockGame.BlockGamePiece
						{
							Offsets = new Vector2i[]
							{
								new Vector2i(0, -1),
								new Vector2i(1, -1),
								new Vector2i(-1, 0),
								new Vector2i(0, 0)
							},
							_gameBlockColor = BlockGameBlock.BlockGameBlockColor.Green,
							CanSpin = true
						};
						break;
					case BlockGameArcadeComponent.BlockGame.BlockGamePieceType.SInverted:
						result = new BlockGameArcadeComponent.BlockGame.BlockGamePiece
						{
							Offsets = new Vector2i[]
							{
								new Vector2i(-1, -1),
								new Vector2i(0, -1),
								new Vector2i(0, 0),
								new Vector2i(1, 0)
							},
							_gameBlockColor = BlockGameBlock.BlockGameBlockColor.Red,
							CanSpin = true
						};
						break;
					case BlockGameArcadeComponent.BlockGame.BlockGamePieceType.T:
						result = new BlockGameArcadeComponent.BlockGame.BlockGamePiece
						{
							Offsets = new Vector2i[]
							{
								new Vector2i(0, -1),
								new Vector2i(-1, 0),
								new Vector2i(0, 0),
								new Vector2i(1, 0)
							},
							_gameBlockColor = BlockGameBlock.BlockGameBlockColor.Purple,
							CanSpin = true
						};
						break;
					case BlockGameArcadeComponent.BlockGame.BlockGamePieceType.O:
						result = new BlockGameArcadeComponent.BlockGame.BlockGamePiece
						{
							Offsets = new Vector2i[]
							{
								new Vector2i(0, -1),
								new Vector2i(1, -1),
								new Vector2i(0, 0),
								new Vector2i(1, 0)
							},
							_gameBlockColor = BlockGameBlock.BlockGameBlockColor.Yellow,
							CanSpin = false
						};
						break;
					default:
						result = new BlockGameArcadeComponent.BlockGame.BlockGamePiece
						{
							Offsets = new Vector2i[]
							{
								new Vector2i(0, 0)
							}
						};
						break;
					}
					return result;
				}

				// Token: 0x04002C61 RID: 11361
				public Vector2i[] Offsets;

				// Token: 0x04002C62 RID: 11362
				private BlockGameBlock.BlockGameBlockColor _gameBlockColor;

				// Token: 0x04002C63 RID: 11363
				public bool CanSpin;
			}
		}
	}
}
