using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Content.Server.Power.Components;
using Content.Server.UserInterface;
using Content.Shared.Arcade;
using Robust.Server.GameObjects;
using Robust.Shared.Audio;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Localization;
using Robust.Shared.Player;
using Robust.Shared.Prototypes;
using Robust.Shared.Random;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom.Prototype.List;
using Robust.Shared.ViewVariables;

namespace Content.Server.Arcade.Components
{
	// Token: 0x020007C2 RID: 1986
	[NullableContext(1)]
	[Nullable(0)]
	[RegisterComponent]
	public sealed class SpaceVillainArcadeComponent : SharedSpaceVillainArcadeComponent
	{
		// Token: 0x170006B0 RID: 1712
		// (get) Token: 0x06002B1D RID: 11037 RVA: 0x000E16A8 File Offset: 0x000DF8A8
		private bool Powered
		{
			get
			{
				ApcPowerReceiverComponent powerReceiverComponent;
				return this._entityManager.TryGetComponent<ApcPowerReceiverComponent>(base.Owner, ref powerReceiverComponent) && powerReceiverComponent.Powered;
			}
		}

		// Token: 0x170006B1 RID: 1713
		// (get) Token: 0x06002B1E RID: 11038 RVA: 0x000E16D2 File Offset: 0x000DF8D2
		[Nullable(2)]
		[ViewVariables]
		private BoundUserInterface UserInterface
		{
			[NullableContext(2)]
			get
			{
				return base.Owner.GetUIOrNull(SharedSpaceVillainArcadeComponent.SpaceVillainArcadeUiKey.Key);
			}
		}

		// Token: 0x06002B1F RID: 11039 RVA: 0x000E16E8 File Offset: 0x000DF8E8
		protected override void Initialize()
		{
			base.Initialize();
			if (this.UserInterface != null)
			{
				this.UserInterface.OnReceiveMessage += this.UserInterfaceOnOnReceiveMessage;
			}
			this._rewardAmount = new Random().Next(this._rewardMinAmount, this._rewardMaxAmount + 1);
		}

		// Token: 0x06002B20 RID: 11040 RVA: 0x000E1738 File Offset: 0x000DF938
		public void OnPowerStateChanged(PowerChangedEvent e)
		{
			if (e.Powered)
			{
				return;
			}
			BoundUserInterface userInterface = this.UserInterface;
			if (userInterface == null)
			{
				return;
			}
			userInterface.CloseAll();
		}

		// Token: 0x06002B21 RID: 11041 RVA: 0x000E1754 File Offset: 0x000DF954
		private void UserInterfaceOnOnReceiveMessage(ServerBoundUserInterfaceMessage serverMsg)
		{
			if (!this.Powered)
			{
				return;
			}
			SharedSpaceVillainArcadeComponent.SpaceVillainArcadePlayerActionMessage msg = serverMsg.Message as SharedSpaceVillainArcadeComponent.SpaceVillainArcadePlayerActionMessage;
			if (msg == null)
			{
				return;
			}
			switch (msg.PlayerAction)
			{
			case SharedSpaceVillainArcadeComponent.PlayerAction.Attack:
			{
				SpaceVillainArcadeComponent.SpaceVillainGame game = this.Game;
				if (game == null)
				{
					return;
				}
				game.ExecutePlayerAction(msg.PlayerAction);
				return;
			}
			case SharedSpaceVillainArcadeComponent.PlayerAction.Heal:
			{
				SpaceVillainArcadeComponent.SpaceVillainGame game2 = this.Game;
				if (game2 == null)
				{
					return;
				}
				game2.ExecutePlayerAction(msg.PlayerAction);
				return;
			}
			case SharedSpaceVillainArcadeComponent.PlayerAction.Recharge:
			{
				SpaceVillainArcadeComponent.SpaceVillainGame game3 = this.Game;
				if (game3 == null)
				{
					return;
				}
				game3.ExecutePlayerAction(msg.PlayerAction);
				return;
			}
			case SharedSpaceVillainArcadeComponent.PlayerAction.NewGame:
			{
				SoundSystem.Play(this._newGameSound.GetSound(null, null), Filter.Pvs(base.Owner, 2f, null, null, null), base.Owner, new AudioParams?(AudioParams.Default.WithVolume(-4f)));
				this.Game = new SpaceVillainArcadeComponent.SpaceVillainGame(this);
				BoundUserInterface userInterface = this.UserInterface;
				if (userInterface == null)
				{
					return;
				}
				userInterface.SendMessage(this.Game.GenerateMetaDataMessage());
				return;
			}
			case SharedSpaceVillainArcadeComponent.PlayerAction.RequestData:
			{
				BoundUserInterface userInterface2 = this.UserInterface;
				if (userInterface2 == null)
				{
					return;
				}
				userInterface2.SendMessage(this.Game.GenerateMetaDataMessage());
				return;
			}
			default:
				return;
			}
		}

		// Token: 0x06002B22 RID: 11042 RVA: 0x000E1864 File Offset: 0x000DFA64
		public void ProcessWin()
		{
			if (this._rewardAmount > 0)
			{
				this._entityManager.SpawnEntity(RandomExtensions.Pick<string>(this._random, this._possibleRewards), this._entityManager.GetComponent<TransformComponent>(base.Owner).Coordinates);
				this._rewardAmount--;
			}
		}

		// Token: 0x06002B23 RID: 11043 RVA: 0x000E18BB File Offset: 0x000DFABB
		public string GenerateFightVerb()
		{
			return RandomExtensions.Pick<string>(this._random, this._possibleFightVerbs);
		}

		// Token: 0x06002B24 RID: 11044 RVA: 0x000E18CE File Offset: 0x000DFACE
		public string GenerateEnemyName()
		{
			return RandomExtensions.Pick<string>(this._random, this._possibleFirstEnemyNames) + " " + RandomExtensions.Pick<string>(this._random, this._possibleLastEnemyNames);
		}

		// Token: 0x04001A9E RID: 6814
		[Dependency]
		private readonly IRobustRandom _random;

		// Token: 0x04001A9F RID: 6815
		[Dependency]
		private readonly IEntityManager _entityManager;

		// Token: 0x04001AA0 RID: 6816
		[ViewVariables]
		public bool OverflowFlag;

		// Token: 0x04001AA1 RID: 6817
		[ViewVariables]
		public bool PlayerInvincibilityFlag;

		// Token: 0x04001AA2 RID: 6818
		[ViewVariables]
		public bool EnemyInvincibilityFlag;

		// Token: 0x04001AA3 RID: 6819
		[ViewVariables]
		public SpaceVillainArcadeComponent.SpaceVillainGame Game;

		// Token: 0x04001AA4 RID: 6820
		[DataField("newGameSound", false, 1, false, false, null)]
		private SoundSpecifier _newGameSound = new SoundPathSpecifier("/Audio/Effects/Arcade/newgame.ogg", null);

		// Token: 0x04001AA5 RID: 6821
		[DataField("playerAttackSound", false, 1, false, false, null)]
		private SoundSpecifier _playerAttackSound = new SoundPathSpecifier("/Audio/Effects/Arcade/player_attack.ogg", null);

		// Token: 0x04001AA6 RID: 6822
		[DataField("playerHealSound", false, 1, false, false, null)]
		private SoundSpecifier _playerHealSound = new SoundPathSpecifier("/Audio/Effects/Arcade/player_heal.ogg", null);

		// Token: 0x04001AA7 RID: 6823
		[DataField("playerChargeSound", false, 1, false, false, null)]
		private SoundSpecifier _playerChargeSound = new SoundPathSpecifier("/Audio/Effects/Arcade/player_charge.ogg", null);

		// Token: 0x04001AA8 RID: 6824
		[DataField("winSound", false, 1, false, false, null)]
		private SoundSpecifier _winSound = new SoundPathSpecifier("/Audio/Effects/Arcade/win.ogg", null);

		// Token: 0x04001AA9 RID: 6825
		[DataField("gameOverSound", false, 1, false, false, null)]
		private SoundSpecifier _gameOverSound = new SoundPathSpecifier("/Audio/Effects/Arcade/gameover.ogg", null);

		// Token: 0x04001AAA RID: 6826
		[ViewVariables]
		[DataField("possibleFightVerbs", false, 1, false, false, null)]
		private List<string> _possibleFightVerbs = new List<string>
		{
			"Defeat",
			"Annihilate",
			"Save",
			"Strike",
			"Stop",
			"Destroy",
			"Robust",
			"Romance",
			"Pwn",
			"Own"
		};

		// Token: 0x04001AAB RID: 6827
		[ViewVariables]
		[DataField("possibleFirstEnemyNames", false, 1, false, false, null)]
		private List<string> _possibleFirstEnemyNames = new List<string>
		{
			"the Automatic",
			"Farmer",
			"Lord",
			"Professor",
			"the Cuban",
			"the Evil",
			"the Dread King",
			"the Space",
			"Lord",
			"the Great",
			"Duke",
			"General"
		};

		// Token: 0x04001AAC RID: 6828
		[ViewVariables]
		[DataField("possibleLastEnemyNames", false, 1, false, false, null)]
		private List<string> _possibleLastEnemyNames = new List<string>
		{
			"Melonoid",
			"Murdertron",
			"Sorcerer",
			"Ruin",
			"Jeff",
			"Ectoplasm",
			"Crushulon",
			"Uhangoid",
			"Vhakoid",
			"Peteoid",
			"slime",
			"Griefer",
			"ERPer",
			"Lizard Man",
			"Unicorn"
		};

		// Token: 0x04001AAD RID: 6829
		[ViewVariables]
		[DataField("possibleRewards", false, 1, false, false, typeof(PrototypeIdListSerializer<EntityPrototype>))]
		private List<string> _possibleRewards = new List<string>
		{
			"ToyMouse",
			"ToyAi",
			"ToyNuke",
			"ToyAssistant",
			"ToyGriffin",
			"ToyHonk",
			"ToyIan",
			"ToyMarauder",
			"ToyMauler",
			"ToyGygax",
			"ToyOdysseus",
			"ToyOwlman",
			"ToyDeathRipley",
			"ToyPhazon",
			"ToyFireRipley",
			"ToyReticence",
			"ToyRipley",
			"ToySeraph",
			"ToyDurand",
			"ToySkeleton",
			"FoamCrossbow",
			"RevolverCapGun",
			"PlushieLizard",
			"PlushieAtmosian",
			"PlushieSpaceLizard",
			"PlushieNuke",
			"PlushieCarp",
			"PlushieRatvar",
			"PlushieNar",
			"PlushieSnake",
			"Basketball",
			"Football",
			"PlushieRouny",
			"PlushieBee",
			"PlushieSlime",
			"BalloonCorgi",
			"ToySword",
			"CrayonBox",
			"BoxDonkSoftBox",
			"BoxCartridgeCap",
			"HarmonicaInstrument",
			"OcarinaInstrument",
			"RecorderInstrument",
			"GunpetInstrument",
			"BirdToyInstrument"
		};

		// Token: 0x04001AAE RID: 6830
		[DataField("rewardMinAmount", false, 1, false, false, null)]
		public int _rewardMinAmount;

		// Token: 0x04001AAF RID: 6831
		[DataField("rewardMaxAmount", false, 1, false, false, null)]
		public int _rewardMaxAmount;

		// Token: 0x04001AB0 RID: 6832
		[ViewVariables]
		public int _rewardAmount;

		// Token: 0x02000B36 RID: 2870
		[Nullable(0)]
		public sealed class SpaceVillainGame
		{
			// Token: 0x170008D9 RID: 2265
			// (get) Token: 0x060038C3 RID: 14531 RVA: 0x00126EDB File Offset: 0x001250DB
			[ViewVariables]
			public string Name
			{
				get
				{
					return this._fightVerb + " " + this._enemyName;
				}
			}

			// Token: 0x060038C4 RID: 14532 RVA: 0x00126EF3 File Offset: 0x001250F3
			public SpaceVillainGame(SpaceVillainArcadeComponent owner) : this(owner, owner.GenerateFightVerb(), owner.GenerateEnemyName())
			{
			}

			// Token: 0x060038C5 RID: 14533 RVA: 0x00126F08 File Offset: 0x00125108
			public SpaceVillainGame(SpaceVillainArcadeComponent owner, string fightVerb, string enemyName)
			{
				IoCManager.InjectDependencies<SpaceVillainArcadeComponent.SpaceVillainGame>(this);
				this._owner = owner;
				this._fightVerb = fightVerb;
				this._enemyName = enemyName;
			}

			// Token: 0x060038C6 RID: 14534 RVA: 0x00126F94 File Offset: 0x00125194
			private void ValidateVars()
			{
				if (this._owner.OverflowFlag)
				{
					return;
				}
				if (this._playerHp > this._playerHpMax)
				{
					this._playerHp = this._playerHpMax;
				}
				if (this._playerMp > this._playerMpMax)
				{
					this._playerMp = this._playerMpMax;
				}
				if (this._enemyHp > this._enemyHpMax)
				{
					this._enemyHp = this._enemyHpMax;
				}
				if (this._enemyMp > this._enemyMpMax)
				{
					this._enemyMp = this._enemyMpMax;
				}
			}

			// Token: 0x060038C7 RID: 14535 RVA: 0x00127018 File Offset: 0x00125218
			public void ExecutePlayerAction(SharedSpaceVillainArcadeComponent.PlayerAction action)
			{
				if (!this._running)
				{
					return;
				}
				switch (action)
				{
				case SharedSpaceVillainArcadeComponent.PlayerAction.Attack:
				{
					int attackAmount = this._random.Next(2, 6);
					this._latestPlayerActionMessage = Loc.GetString("space-villain-game-player-attack-message", new ValueTuple<string, object>[]
					{
						new ValueTuple<string, object>("enemyName", this._enemyName),
						new ValueTuple<string, object>("attackAmount", attackAmount)
					});
					SoundSystem.Play(this._owner._playerAttackSound.GetSound(null, null), Filter.Pvs(this._owner.Owner, 2f, null, null, null), this._owner.Owner, new AudioParams?(AudioParams.Default.WithVolume(-4f)));
					if (!this._owner.EnemyInvincibilityFlag)
					{
						this._enemyHp -= attackAmount;
					}
					this._turtleTracker -= ((this._turtleTracker > 0) ? 1 : 0);
					break;
				}
				case SharedSpaceVillainArcadeComponent.PlayerAction.Heal:
				{
					int pointAmount = this._random.Next(1, 3);
					int healAmount = this._random.Next(6, 8);
					this._latestPlayerActionMessage = Loc.GetString("space-villain-game-player-heal-message", new ValueTuple<string, object>[]
					{
						new ValueTuple<string, object>("magicPointAmount", pointAmount),
						new ValueTuple<string, object>("healAmount", healAmount)
					});
					SoundSystem.Play(this._owner._playerHealSound.GetSound(null, null), Filter.Pvs(this._owner.Owner, 2f, null, null, null), this._owner.Owner, new AudioParams?(AudioParams.Default.WithVolume(-4f)));
					if (!this._owner.PlayerInvincibilityFlag)
					{
						this._playerMp -= pointAmount;
					}
					this._playerHp += healAmount;
					this._turtleTracker++;
					break;
				}
				case SharedSpaceVillainArcadeComponent.PlayerAction.Recharge:
				{
					int chargeAmount = this._random.Next(4, 7);
					this._latestPlayerActionMessage = Loc.GetString("space-villain-game-player-recharge-message", new ValueTuple<string, object>[]
					{
						new ValueTuple<string, object>("regainedPoints", chargeAmount)
					});
					SoundSystem.Play(this._owner._playerChargeSound.GetSound(null, null), Filter.Pvs(this._owner.Owner, 2f, null, null, null), this._owner.Owner, new AudioParams?(AudioParams.Default.WithVolume(-4f)));
					this._playerMp += chargeAmount;
					this._turtleTracker -= ((this._turtleTracker > 0) ? 1 : 0);
					break;
				}
				}
				if (!this.CheckGameConditions())
				{
					return;
				}
				this.ValidateVars();
				this.ExecuteAiAction();
				if (!this.CheckGameConditions())
				{
					return;
				}
				this.ValidateVars();
				this.UpdateUi(false);
			}

			// Token: 0x060038C8 RID: 14536 RVA: 0x001272E8 File Offset: 0x001254E8
			private bool CheckGameConditions()
			{
				if (this._playerHp > 0 && this._playerMp > 0 && (this._enemyHp <= 0 || this._enemyMp <= 0))
				{
					this._running = false;
					this.UpdateUi(Loc.GetString("space-villain-game-player-wins-message"), Loc.GetString("space-villain-game-enemy-dies-message", new ValueTuple<string, object>[]
					{
						new ValueTuple<string, object>("enemyName", this._enemyName)
					}), true);
					SoundSystem.Play(this._owner._winSound.GetSound(null, null), Filter.Pvs(this._owner.Owner, 2f, null, null, null), this._owner.Owner, new AudioParams?(AudioParams.Default.WithVolume(-4f)));
					this._owner.ProcessWin();
					return false;
				}
				if (this._playerHp > 0 && this._playerMp > 0)
				{
					return true;
				}
				if (this._enemyHp > 0 && this._enemyMp > 0)
				{
					this._running = false;
					this.UpdateUi(Loc.GetString("space-villain-game-player-loses-message"), Loc.GetString("space-villain-game-enemy-cheers-message", new ValueTuple<string, object>[]
					{
						new ValueTuple<string, object>("enemyName", this._enemyName)
					}), true);
					SoundSystem.Play(this._owner._gameOverSound.GetSound(null, null), Filter.Pvs(this._owner.Owner, 2f, null, null, null), this._owner.Owner, new AudioParams?(AudioParams.Default.WithVolume(-4f)));
					return false;
				}
				if (this._enemyHp <= 0 || this._enemyMp <= 0)
				{
					this._running = false;
					this.UpdateUi(Loc.GetString("space-villain-game-player-loses-message"), Loc.GetString("space-villain-game-enemy-dies-with-player-message ", new ValueTuple<string, object>[]
					{
						new ValueTuple<string, object>("enemyName", this._enemyName)
					}), true);
					SoundSystem.Play(this._owner._gameOverSound.GetSound(null, null), Filter.Pvs(this._owner.Owner, 2f, null, null, null), this._owner.Owner, new AudioParams?(AudioParams.Default.WithVolume(-4f)));
					return false;
				}
				return true;
			}

			// Token: 0x060038C9 RID: 14537 RVA: 0x0012751F File Offset: 0x0012571F
			private void UpdateUi(bool metadata = false)
			{
				BoundUserInterface userInterface = this._owner.UserInterface;
				if (userInterface == null)
				{
					return;
				}
				userInterface.SendMessage(metadata ? this.GenerateMetaDataMessage() : this.GenerateUpdateMessage());
			}

			// Token: 0x060038CA RID: 14538 RVA: 0x00127547 File Offset: 0x00125747
			private void UpdateUi(string message1, string message2, bool metadata = false)
			{
				this._latestPlayerActionMessage = message1;
				this._latestEnemyActionMessage = message2;
				this.UpdateUi(metadata);
			}

			// Token: 0x060038CB RID: 14539 RVA: 0x00127560 File Offset: 0x00125760
			private void ExecuteAiAction()
			{
				if (this._turtleTracker >= 4)
				{
					int boomAmount = this._random.Next(5, 10);
					this._latestEnemyActionMessage = Loc.GetString("space-villain-game-enemy-throws-bomb-message", new ValueTuple<string, object>[]
					{
						new ValueTuple<string, object>("enemyName", this._enemyName),
						new ValueTuple<string, object>("damageReceived", boomAmount)
					});
					if (this._owner.PlayerInvincibilityFlag)
					{
						return;
					}
					this._playerHp -= boomAmount;
					this._turtleTracker--;
					return;
				}
				else if (this._enemyMp <= 5 && RandomExtensions.Prob(this._random, 0.7f))
				{
					int stealAmount = this._random.Next(2, 3);
					this._latestEnemyActionMessage = Loc.GetString("space-villain-game-enemy-steals-player-power-message", new ValueTuple<string, object>[]
					{
						new ValueTuple<string, object>("enemyName", this._enemyName),
						new ValueTuple<string, object>("stolenAmount", stealAmount)
					});
					if (this._owner.PlayerInvincibilityFlag)
					{
						return;
					}
					this._playerMp -= stealAmount;
					this._enemyMp += stealAmount;
					return;
				}
				else
				{
					if (this._enemyHp <= 10 && this._enemyMp > 4)
					{
						this._enemyHp += 4;
						this._enemyMp -= 4;
						this._latestEnemyActionMessage = Loc.GetString("space-villain-game-enemy-heals-message", new ValueTuple<string, object>[]
						{
							new ValueTuple<string, object>("enemyName", this._enemyName),
							new ValueTuple<string, object>("healedAmount", 4)
						});
						return;
					}
					int attackAmount = this._random.Next(3, 6);
					this._latestEnemyActionMessage = Loc.GetString("space-villain-game-enemy-attacks-message", new ValueTuple<string, object>[]
					{
						new ValueTuple<string, object>("enemyName", this._enemyName),
						new ValueTuple<string, object>("damageDealt", attackAmount)
					});
					if (this._owner.PlayerInvincibilityFlag)
					{
						return;
					}
					this._playerHp -= attackAmount;
					return;
				}
			}

			// Token: 0x060038CC RID: 14540 RVA: 0x00127774 File Offset: 0x00125974
			public SharedSpaceVillainArcadeComponent.SpaceVillainArcadeMetaDataUpdateMessage GenerateMetaDataMessage()
			{
				return new SharedSpaceVillainArcadeComponent.SpaceVillainArcadeMetaDataUpdateMessage(this._playerHp, this._playerMp, this._enemyHp, this._enemyMp, this._latestPlayerActionMessage, this._latestEnemyActionMessage, this.Name, this._enemyName, !this._running);
			}

			// Token: 0x060038CD RID: 14541 RVA: 0x001277BF File Offset: 0x001259BF
			public SharedSpaceVillainArcadeComponent.SpaceVillainArcadeDataUpdateMessage GenerateUpdateMessage()
			{
				return new SharedSpaceVillainArcadeComponent.SpaceVillainArcadeDataUpdateMessage(this._playerHp, this._playerMp, this._enemyHp, this._enemyMp, this._latestPlayerActionMessage, this._latestEnemyActionMessage);
			}

			// Token: 0x04002981 RID: 10625
			[Dependency]
			private readonly IRobustRandom _random;

			// Token: 0x04002982 RID: 10626
			[ViewVariables]
			private readonly SpaceVillainArcadeComponent _owner;

			// Token: 0x04002983 RID: 10627
			[ViewVariables]
			private int _playerHp = 30;

			// Token: 0x04002984 RID: 10628
			[ViewVariables]
			private int _playerHpMax = 30;

			// Token: 0x04002985 RID: 10629
			[ViewVariables]
			private int _playerMp = 10;

			// Token: 0x04002986 RID: 10630
			[ViewVariables]
			private int _playerMpMax = 10;

			// Token: 0x04002987 RID: 10631
			[ViewVariables]
			private int _enemyHp = 45;

			// Token: 0x04002988 RID: 10632
			[ViewVariables]
			private int _enemyHpMax = 45;

			// Token: 0x04002989 RID: 10633
			[ViewVariables]
			private int _enemyMp = 20;

			// Token: 0x0400298A RID: 10634
			[ViewVariables]
			private int _enemyMpMax = 20;

			// Token: 0x0400298B RID: 10635
			[ViewVariables]
			private int _turtleTracker;

			// Token: 0x0400298C RID: 10636
			[ViewVariables]
			private readonly string _fightVerb;

			// Token: 0x0400298D RID: 10637
			[ViewVariables]
			private readonly string _enemyName;

			// Token: 0x0400298E RID: 10638
			[ViewVariables]
			private bool _running = true;

			// Token: 0x0400298F RID: 10639
			private string _latestPlayerActionMessage = "";

			// Token: 0x04002990 RID: 10640
			private string _latestEnemyActionMessage = "";
		}
	}
}
