using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;
using Content.Server.Actions;
using Content.Server.Chat.Managers;
using Content.Server.Disease;
using Content.Server.Disease.Components;
using Content.Server.GameTicking.Presets;
using Content.Server.Mind;
using Content.Server.Mind.Components;
using Content.Server.Players;
using Content.Server.Popups;
using Content.Server.Preferences.Managers;
using Content.Server.RoundEnd;
using Content.Server.Traitor;
using Content.Server.Zombies;
using Content.Shared.Actions.ActionTypes;
using Content.Shared.CCVar;
using Content.Shared.Chat;
using Content.Shared.Humanoid;
using Content.Shared.Mobs;
using Content.Shared.Mobs.Components;
using Content.Shared.Mobs.Systems;
using Content.Shared.Popups;
using Content.Shared.Preferences;
using Content.Shared.Roles;
using Content.Shared.Zombies;
using Robust.Server.Player;
using Robust.Shared.Configuration;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Localization;
using Robust.Shared.Log;
using Robust.Shared.Maths;
using Robust.Shared.Prototypes;
using Robust.Shared.Random;

namespace Content.Server.GameTicking.Rules
{
	// Token: 0x020004C4 RID: 1220
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class ZombieRuleSystem : GameRuleSystem
	{
		// Token: 0x1700039B RID: 923
		// (get) Token: 0x06001945 RID: 6469 RVA: 0x000855FD File Offset: 0x000837FD
		public override string Prototype
		{
			get
			{
				return "Zombie";
			}
		}

		// Token: 0x06001946 RID: 6470 RVA: 0x00085604 File Offset: 0x00083804
		public override void Initialize()
		{
			base.Initialize();
			base.SubscribeLocalEvent<RoundStartAttemptEvent>(new EntityEventHandler<RoundStartAttemptEvent>(this.OnStartAttempt), null, null);
			base.SubscribeLocalEvent<MobStateChangedEvent>(new EntityEventHandler<MobStateChangedEvent>(this.OnMobStateChanged), null, null);
			base.SubscribeLocalEvent<RoundEndTextAppendEvent>(new EntityEventHandler<RoundEndTextAppendEvent>(this.OnRoundEndText), null, null);
			base.SubscribeLocalEvent<RulePlayerJobsAssignedEvent>(new EntityEventHandler<RulePlayerJobsAssignedEvent>(this.OnJobAssigned), null, null);
			base.SubscribeLocalEvent<EntityZombifiedEvent>(new EntityEventHandler<EntityZombifiedEvent>(this.OnEntityZombified), null, null);
			base.SubscribeLocalEvent<ZombifyOnDeathComponent, ZombifySelfActionEvent>(new ComponentEventHandler<ZombifyOnDeathComponent, ZombifySelfActionEvent>(this.OnZombifySelf), null, null);
		}

		// Token: 0x06001947 RID: 6471 RVA: 0x00085690 File Offset: 0x00083890
		private void OnRoundEndText(RoundEndTextAppendEvent ev)
		{
			if (!base.RuleAdded)
			{
				return;
			}
			List<EntityUid> livingHumans;
			float percent = this.GetInfectedPercentage(out livingHumans);
			if (percent <= 0f)
			{
				ev.AddLine(Loc.GetString("zombie-round-end-amount-none"));
			}
			else if ((double)percent <= 0.25)
			{
				ev.AddLine(Loc.GetString("zombie-round-end-amount-low"));
			}
			else if ((double)percent <= 0.5)
			{
				ev.AddLine(Loc.GetString("zombie-round-end-amount-medium", new ValueTuple<string, object>[]
				{
					new ValueTuple<string, object>("percent", Math.Round((double)(percent * 100f), 2).ToString(CultureInfo.InvariantCulture))
				}));
			}
			else if (percent < 1f)
			{
				ev.AddLine(Loc.GetString("zombie-round-end-amount-high", new ValueTuple<string, object>[]
				{
					new ValueTuple<string, object>("percent", Math.Round((double)(percent * 100f), 2).ToString(CultureInfo.InvariantCulture))
				}));
			}
			else
			{
				ev.AddLine(Loc.GetString("zombie-round-end-amount-all"));
			}
			ev.AddLine(Loc.GetString("zombie-round-end-initial-count", new ValueTuple<string, object>[]
			{
				new ValueTuple<string, object>("initialCount", this._initialInfectedNames.Count)
			}));
			foreach (KeyValuePair<string, string> player in this._initialInfectedNames)
			{
				ev.AddLine(Loc.GetString("zombie-round-end-user-was-initial", new ValueTuple<string, object>[]
				{
					new ValueTuple<string, object>("name", player.Key),
					new ValueTuple<string, object>("username", player.Value)
				}));
			}
			if (livingHumans.Count > 0 && livingHumans.Count <= this._initialInfectedNames.Count)
			{
				ev.AddLine("");
				ev.AddLine(Loc.GetString("zombie-round-end-survivor-count", new ValueTuple<string, object>[]
				{
					new ValueTuple<string, object>("count", livingHumans.Count)
				}));
				foreach (EntityUid survivor in livingHumans)
				{
					MetaDataComponent meta = base.MetaData(survivor);
					string username = string.Empty;
					MindComponent mindcomp;
					if (base.TryComp<MindComponent>(survivor, ref mindcomp) && mindcomp.Mind != null && mindcomp.Mind.Session != null)
					{
						username = mindcomp.Mind.Session.Name;
					}
					ev.AddLine(Loc.GetString("zombie-round-end-user-was-survivor", new ValueTuple<string, object>[]
					{
						new ValueTuple<string, object>("name", meta.EntityName),
						new ValueTuple<string, object>("username", username)
					}));
				}
			}
		}

		// Token: 0x06001948 RID: 6472 RVA: 0x00085980 File Offset: 0x00083B80
		private void OnJobAssigned(RulePlayerJobsAssignedEvent ev)
		{
			if (!base.RuleAdded)
			{
				return;
			}
			this._initialInfectedNames = new Dictionary<string, string>();
			this.InfectInitialPlayers();
		}

		// Token: 0x06001949 RID: 6473 RVA: 0x0008599C File Offset: 0x00083B9C
		private void OnMobStateChanged(MobStateChangedEvent ev)
		{
			if (!base.RuleAdded)
			{
				return;
			}
			this.CheckRoundEnd(ev.Target);
		}

		// Token: 0x0600194A RID: 6474 RVA: 0x000859B4 File Offset: 0x00083BB4
		private void OnEntityZombified(EntityZombifiedEvent ev)
		{
			if (!base.RuleAdded)
			{
				return;
			}
			this.CheckRoundEnd(ev.Target);
		}

		// Token: 0x0600194B RID: 6475 RVA: 0x000859CC File Offset: 0x00083BCC
		private void CheckRoundEnd(EntityUid target)
		{
			if (!base.HasComp<HumanoidAppearanceComponent>(target))
			{
				return;
			}
			List<EntityUid> num;
			float infectedPercentage = this.GetInfectedPercentage(out num);
			if (num.Count == 1)
			{
				this._popup.PopupEntity(Loc.GetString("zombie-alone"), num[0], num[0], PopupType.Small);
			}
			if (infectedPercentage >= 1f)
			{
				this._roundEndSystem.EndRound();
			}
		}

		// Token: 0x0600194C RID: 6476 RVA: 0x00085A2C File Offset: 0x00083C2C
		private void OnStartAttempt(RoundStartAttemptEvent ev)
		{
			if (!base.RuleAdded)
			{
				return;
			}
			int? minPlayers = this._prototypeManager.Index<GamePresetPrototype>(this.Prototype).MinPlayers;
			if (!ev.Forced)
			{
				int num = ev.Players.Length;
				int? num2 = minPlayers;
				if (num < num2.GetValueOrDefault() & num2 != null)
				{
					this._chatManager.DispatchServerAnnouncement(Loc.GetString("zombie-not-enough-ready-players", new ValueTuple<string, object>[]
					{
						new ValueTuple<string, object>("readyPlayersCount", ev.Players.Length),
						new ValueTuple<string, object>("minimumPlayers", minPlayers)
					}), null);
					ev.Cancel();
					return;
				}
			}
			if (ev.Players.Length == 0)
			{
				this._chatManager.DispatchServerAnnouncement(Loc.GetString("zombie-no-one-ready"), null);
				ev.Cancel();
			}
		}

		// Token: 0x0600194D RID: 6477 RVA: 0x00085B0B File Offset: 0x00083D0B
		public override void Started()
		{
			this.InfectInitialPlayers();
		}

		// Token: 0x0600194E RID: 6478 RVA: 0x00085B13 File Offset: 0x00083D13
		public override void Ended()
		{
		}

		// Token: 0x0600194F RID: 6479 RVA: 0x00085B18 File Offset: 0x00083D18
		private void OnZombifySelf(EntityUid uid, ZombifyOnDeathComponent component, ZombifySelfActionEvent args)
		{
			this._zombify.ZombifyEntity(uid);
			InstantAction action = new InstantAction(this._prototypeManager.Index<InstantActionPrototype>("TurnUndead"));
			this._action.RemoveAction(uid, action, null);
		}

		// Token: 0x06001950 RID: 6480 RVA: 0x00085B58 File Offset: 0x00083D58
		private float GetInfectedPercentage(out List<EntityUid> livingHumans)
		{
			IEnumerable<ValueTuple<HumanoidAppearanceComponent, MobStateComponent>> enumerable = base.EntityQuery<HumanoidAppearanceComponent, MobStateComponent>(true);
			EntityQuery<ZombieComponent> allZombers = base.GetEntityQuery<ZombieComponent>();
			List<EntityUid> totalPlayers = new List<EntityUid>();
			List<EntityUid> livingZombies = new List<EntityUid>();
			livingHumans = new List<EntityUid>();
			foreach (ValueTuple<HumanoidAppearanceComponent, MobStateComponent> valueTuple in enumerable)
			{
				MobStateComponent mob = valueTuple.Item2;
				if (this._mobState.IsAlive(mob.Owner, mob))
				{
					totalPlayers.Add(mob.Owner);
					if (allZombers.HasComponent(mob.Owner))
					{
						livingZombies.Add(mob.Owner);
					}
					else
					{
						livingHumans.Add(mob.Owner);
					}
				}
			}
			return (float)livingZombies.Count / (float)totalPlayers.Count;
		}

		// Token: 0x06001951 RID: 6481 RVA: 0x00085C20 File Offset: 0x00083E20
		private void InfectInitialPlayers()
		{
			List<IPlayerSession> list = this._playerManager.ServerSessions.ToList<IPlayerSession>();
			List<IPlayerSession> playerList = new List<IPlayerSession>();
			List<IPlayerSession> prefList = new List<IPlayerSession>();
			foreach (IPlayerSession player in list)
			{
				if (player.AttachedEntity != null && base.HasComp<DiseaseCarrierComponent>(player.AttachedEntity))
				{
					playerList.Add(player);
					if (((HumanoidCharacterProfile)this._prefs.GetPreferences(player.UserId).SelectedCharacter).AntagPreferences.Contains("InitialInfected"))
					{
						prefList.Add(player);
					}
				}
			}
			if (playerList.Count == 0)
			{
				return;
			}
			int playersPerInfected = this._cfg.GetCVar<int>(CCVars.ZombiePlayersPerInfected);
			int maxInfected = this._cfg.GetCVar<int>(CCVars.ZombieMaxInitialInfected);
			int numInfected = Math.Max(1, (int)Math.Min(Math.Floor((double)playerList.Count / (double)playersPerInfected), (double)maxInfected));
			for (int i = 0; i < numInfected; i++)
			{
				IPlayerSession zombie;
				if (prefList.Count == 0)
				{
					if (playerList.Count == 0)
					{
						Logger.InfoS("preset", "Insufficient number of players. stopping selection.");
						return;
					}
					zombie = RandomExtensions.PickAndTake<IPlayerSession>(this._random, playerList);
					Logger.InfoS("preset", "Insufficient preferred patient 0, picking at random.");
				}
				else
				{
					zombie = RandomExtensions.PickAndTake<IPlayerSession>(this._random, prefList);
					playerList.Remove(zombie);
					Logger.InfoS("preset", "Selected a patient 0.");
				}
				PlayerData playerData = zombie.Data.ContentData();
				Mind mind = (playerData != null) ? playerData.Mind : null;
				if (mind == null)
				{
					Logger.ErrorS("preset", "Failed getting mind for picked patient 0.");
				}
				else
				{
					mind.AddRole(new TraitorRole(mind, this._prototypeManager.Index<AntagPrototype>("InitialInfected")));
					string inCharacterName = string.Empty;
					if (mind.OwnedEntity != null)
					{
						this._diseaseSystem.TryAddDisease(mind.OwnedEntity.Value, "PassiveZombieVirus", null);
						inCharacterName = base.MetaData(mind.OwnedEntity.Value).EntityName;
						InstantAction action = new InstantAction(this._prototypeManager.Index<InstantActionPrototype>("TurnUndead"));
						this._action.AddAction(mind.OwnedEntity.Value, action, null, null, true);
					}
					if (mind.Session != null)
					{
						string message = Loc.GetString("zombie-patientzero-role-greeting");
						string wrappedMessage = Loc.GetString("chat-manager-server-wrap-message", new ValueTuple<string, object>[]
						{
							new ValueTuple<string, object>("message", message)
						});
						this._initialInfectedNames.Add(inCharacterName, mind.Session.Name);
						this._chatManager.ChatMessageToOne(ChatChannel.Server, message, wrappedMessage, default(EntityUid), false, mind.Session.ConnectedClient, new Color?(Color.Plum), false, null, 0f);
					}
				}
			}
		}

		// Token: 0x04000FD8 RID: 4056
		[Dependency]
		private readonly IPrototypeManager _prototypeManager;

		// Token: 0x04000FD9 RID: 4057
		[Dependency]
		private readonly IRobustRandom _random;

		// Token: 0x04000FDA RID: 4058
		[Dependency]
		private readonly IConfigurationManager _cfg;

		// Token: 0x04000FDB RID: 4059
		[Dependency]
		private readonly IChatManager _chatManager;

		// Token: 0x04000FDC RID: 4060
		[Dependency]
		private readonly IPlayerManager _playerManager;

		// Token: 0x04000FDD RID: 4061
		[Dependency]
		private readonly IServerPreferencesManager _prefs;

		// Token: 0x04000FDE RID: 4062
		[Dependency]
		private readonly RoundEndSystem _roundEndSystem;

		// Token: 0x04000FDF RID: 4063
		[Dependency]
		private readonly DiseaseSystem _diseaseSystem;

		// Token: 0x04000FE0 RID: 4064
		[Dependency]
		private readonly PopupSystem _popup;

		// Token: 0x04000FE1 RID: 4065
		[Dependency]
		private readonly ActionsSystem _action;

		// Token: 0x04000FE2 RID: 4066
		[Dependency]
		private readonly MobStateSystem _mobState;

		// Token: 0x04000FE3 RID: 4067
		[Dependency]
		private readonly ZombifyOnDeathSystem _zombify;

		// Token: 0x04000FE4 RID: 4068
		private Dictionary<string, string> _initialInfectedNames = new Dictionary<string, string>();

		// Token: 0x04000FE5 RID: 4069
		private const string PatientZeroPrototypeID = "InitialInfected";

		// Token: 0x04000FE6 RID: 4070
		private const string InitialZombieVirusPrototype = "PassiveZombieVirus";

		// Token: 0x04000FE7 RID: 4071
		private const string ZombifySelfActionPrototype = "TurnUndead";
	}
}
