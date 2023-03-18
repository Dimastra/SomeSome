using System;
using System.Runtime.CompilerServices;
using Content.Shared.Arcade;
using Robust.Client.GameObjects;
using Robust.Shared.GameObjects;

namespace Content.Client.Arcade.UI
{
	// Token: 0x02000468 RID: 1128
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class BlockGameBoundUserInterface : BoundUserInterface
	{
		// Token: 0x06001BFF RID: 7167 RVA: 0x000021BC File Offset: 0x000003BC
		public BlockGameBoundUserInterface(ClientUserInterfaceComponent owner, Enum uiKey) : base(owner, uiKey)
		{
		}

		// Token: 0x06001C00 RID: 7168 RVA: 0x000A2777 File Offset: 0x000A0977
		protected override void Open()
		{
			base.Open();
			this._menu = new BlockGameMenu(this);
			this._menu.OnClose += base.Close;
			this._menu.OpenCentered();
		}

		// Token: 0x06001C01 RID: 7169 RVA: 0x000A27B0 File Offset: 0x000A09B0
		protected override void ReceiveMessage(BoundUserInterfaceMessage message)
		{
			BlockGameMessages.BlockGameVisualUpdateMessage blockGameVisualUpdateMessage = message as BlockGameMessages.BlockGameVisualUpdateMessage;
			if (blockGameVisualUpdateMessage == null)
			{
				BlockGameMessages.BlockGameScoreUpdateMessage blockGameScoreUpdateMessage = message as BlockGameMessages.BlockGameScoreUpdateMessage;
				if (blockGameScoreUpdateMessage == null)
				{
					BlockGameMessages.BlockGameUserStatusMessage blockGameUserStatusMessage = message as BlockGameMessages.BlockGameUserStatusMessage;
					if (blockGameUserStatusMessage == null)
					{
						BlockGameMessages.BlockGameSetScreenMessage blockGameSetScreenMessage = message as BlockGameMessages.BlockGameSetScreenMessage;
						if (blockGameSetScreenMessage == null)
						{
							BlockGameMessages.BlockGameHighScoreUpdateMessage blockGameHighScoreUpdateMessage = message as BlockGameMessages.BlockGameHighScoreUpdateMessage;
							if (blockGameHighScoreUpdateMessage == null)
							{
								BlockGameMessages.BlockGameLevelUpdateMessage blockGameLevelUpdateMessage = message as BlockGameMessages.BlockGameLevelUpdateMessage;
								if (blockGameLevelUpdateMessage == null)
								{
									return;
								}
								BlockGameMenu menu = this._menu;
								if (menu == null)
								{
									return;
								}
								menu.UpdateLevel(blockGameLevelUpdateMessage.Level);
							}
							else
							{
								BlockGameMenu menu2 = this._menu;
								if (menu2 == null)
								{
									return;
								}
								menu2.UpdateHighscores(blockGameHighScoreUpdateMessage.LocalHighscores, blockGameHighScoreUpdateMessage.GlobalHighscores);
								return;
							}
						}
						else
						{
							if (blockGameSetScreenMessage.IsStarted)
							{
								BlockGameMenu menu3 = this._menu;
								if (menu3 != null)
								{
									menu3.SetStarted();
								}
							}
							BlockGameMenu menu4 = this._menu;
							if (menu4 != null)
							{
								menu4.SetScreen(blockGameSetScreenMessage.Screen);
							}
							BlockGameMessages.BlockGameGameOverScreenMessage blockGameGameOverScreenMessage = blockGameSetScreenMessage as BlockGameMessages.BlockGameGameOverScreenMessage;
							if (blockGameGameOverScreenMessage != null)
							{
								BlockGameMenu menu5 = this._menu;
								if (menu5 == null)
								{
									return;
								}
								menu5.SetGameoverInfo(blockGameGameOverScreenMessage.FinalScore, blockGameGameOverScreenMessage.LocalPlacement, blockGameGameOverScreenMessage.GlobalPlacement);
								return;
							}
						}
						return;
					}
					BlockGameMenu menu6 = this._menu;
					if (menu6 == null)
					{
						return;
					}
					menu6.SetUsability(blockGameUserStatusMessage.IsPlayer);
					return;
				}
				else
				{
					BlockGameMenu menu7 = this._menu;
					if (menu7 == null)
					{
						return;
					}
					menu7.UpdatePoints(blockGameScoreUpdateMessage.Points);
					return;
				}
			}
			else
			{
				switch (blockGameVisualUpdateMessage.GameVisualType)
				{
				case BlockGameMessages.BlockGameVisualType.GameField:
				{
					BlockGameMenu menu8 = this._menu;
					if (menu8 == null)
					{
						return;
					}
					menu8.UpdateBlocks(blockGameVisualUpdateMessage.Blocks);
					return;
				}
				case BlockGameMessages.BlockGameVisualType.HoldBlock:
				{
					BlockGameMenu menu9 = this._menu;
					if (menu9 == null)
					{
						return;
					}
					menu9.UpdateHeldBlock(blockGameVisualUpdateMessage.Blocks);
					return;
				}
				case BlockGameMessages.BlockGameVisualType.NextBlock:
				{
					BlockGameMenu menu10 = this._menu;
					if (menu10 == null)
					{
						return;
					}
					menu10.UpdateNextBlock(blockGameVisualUpdateMessage.Blocks);
					return;
				}
				default:
					return;
				}
			}
		}

		// Token: 0x06001C02 RID: 7170 RVA: 0x000A2934 File Offset: 0x000A0B34
		public void SendAction(BlockGamePlayerAction action)
		{
			base.SendMessage(new BlockGameMessages.BlockGamePlayerActionMessage(action));
		}

		// Token: 0x06001C03 RID: 7171 RVA: 0x000A2942 File Offset: 0x000A0B42
		protected override void Dispose(bool disposing)
		{
			base.Dispose(disposing);
			if (!disposing)
			{
				return;
			}
			BlockGameMenu menu = this._menu;
			if (menu == null)
			{
				return;
			}
			menu.Dispose();
		}

		// Token: 0x04000E18 RID: 3608
		[Nullable(2)]
		private BlockGameMenu _menu;
	}
}
