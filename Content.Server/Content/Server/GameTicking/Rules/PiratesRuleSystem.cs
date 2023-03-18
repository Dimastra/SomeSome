using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Content.Server.Administration.Commands;
using Content.Server.Cargo.Systems;
using Content.Server.Chat.Managers;
using Content.Server.Mind;
using Content.Server.Preferences.Managers;
using Content.Server.Spawners.Components;
using Content.Server.Station.Components;
using Content.Server.Station.Systems;
using Content.Shared.CCVar;
using Content.Shared.Humanoid;
using Content.Shared.Preferences;
using Content.Shared.Roles;
using Robust.Server.GameObjects;
using Robust.Server.Maps;
using Robust.Server.Player;
using Robust.Shared.Configuration;
using Robust.Shared.Enums;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Localization;
using Robust.Shared.Log;
using Robust.Shared.Map;
using Robust.Shared.Maths;
using Robust.Shared.Network;
using Robust.Shared.Prototypes;
using Robust.Shared.Random;
using Robust.Shared.Utility;
using Robust.Shared.ViewVariables;

namespace Content.Server.GameTicking.Rules
{
	// Token: 0x020004BD RID: 1213
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class PiratesRuleSystem : GameRuleSystem
	{
		// Token: 0x17000388 RID: 904
		// (get) Token: 0x060018DF RID: 6367 RVA: 0x00081AB8 File Offset: 0x0007FCB8
		public override string Prototype
		{
			get
			{
				return "Pirates";
			}
		}

		// Token: 0x060018E0 RID: 6368 RVA: 0x00081ABF File Offset: 0x0007FCBF
		public override void Initialize()
		{
			base.Initialize();
			base.SubscribeLocalEvent<RulePlayerSpawningEvent>(new EntityEventHandler<RulePlayerSpawningEvent>(this.OnPlayerSpawningEvent), null, null);
			base.SubscribeLocalEvent<RoundEndTextAppendEvent>(new EntityEventHandler<RoundEndTextAppendEvent>(this.OnRoundEndTextEvent), null, null);
		}

		// Token: 0x060018E1 RID: 6369 RVA: 0x00081AF0 File Offset: 0x0007FCF0
		private void OnRoundEndTextEvent(RoundEndTextAppendEvent ev)
		{
			if (!base.RuleAdded)
			{
				return;
			}
			if (base.Deleted(this._pirateShip, null))
			{
				ev.AddLine(Loc.GetString("pirates-no-ship"));
			}
			else
			{
				List<ValueTuple<double, EntityUid>> mostValuableThefts = new List<ValueTuple<double, EntityUid>>();
				double finalValue = this._pricingSystem.AppraiseGrid(this._pirateShip, delegate(EntityUid uid)
				{
					foreach (Mind mind2 in this._pirates)
					{
						EntityUid? currentEntity = mind2.CurrentEntity;
						if (currentEntity != null && (currentEntity == null || currentEntity.GetValueOrDefault() == uid))
						{
							return false;
						}
					}
					return true;
				}, delegate(EntityUid uid, double price)
				{
					if (this._initialItems.Contains(uid))
					{
						return;
					}
					mostValuableThefts.Add(new ValueTuple<double, EntityUid>(price, uid));
					mostValuableThefts.Sort((ValueTuple<double, EntityUid> i1, ValueTuple<double, EntityUid> i2) => i2.Item1.CompareTo(i1.Item1));
					if (mostValuableThefts.Count > 5)
					{
						Extensions.Pop<ValueTuple<double, EntityUid>>(mostValuableThefts);
					}
				});
				foreach (Mind mind in this._pirates)
				{
					if (mind.CurrentEntity != null)
					{
						finalValue += this._pricingSystem.GetPrice(mind.CurrentEntity.Value);
					}
				}
				double score = finalValue - this._initialShipValue;
				string text = "pirates-final-score";
				ValueTuple<string, object>[] array = new ValueTuple<string, object>[1];
				int num = 0;
				string item = "score";
				DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(0, 1);
				defaultInterpolatedStringHandler.AppendFormatted<double>(score, "F2");
				array[num] = new ValueTuple<string, object>(item, defaultInterpolatedStringHandler.ToStringAndClear());
				ev.AddLine(Loc.GetString(text, array));
				string text2 = "pirates-final-score-2";
				ValueTuple<string, object>[] array2 = new ValueTuple<string, object>[1];
				int num2 = 0;
				string item2 = "finalPrice";
				defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(0, 1);
				defaultInterpolatedStringHandler.AppendFormatted<double>(finalValue, "F2");
				array2[num2] = new ValueTuple<string, object>(item2, defaultInterpolatedStringHandler.ToStringAndClear());
				ev.AddLine(Loc.GetString(text2, array2));
				ev.AddLine("");
				ev.AddLine(Loc.GetString("pirates-most-valuable"));
				foreach (ValueTuple<double, EntityUid> valueTuple in mostValuableThefts)
				{
					double price2 = valueTuple.Item1;
					EntityUid obj = valueTuple.Item2;
					string text3 = "pirates-stolen-item-entry";
					ValueTuple<string, object>[] array3 = new ValueTuple<string, object>[2];
					array3[0] = new ValueTuple<string, object>("entity", obj);
					int num3 = 1;
					string item3 = "credits";
					defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(0, 1);
					defaultInterpolatedStringHandler.AppendFormatted<double>(price2, "F2");
					array3[num3] = new ValueTuple<string, object>(item3, defaultInterpolatedStringHandler.ToStringAndClear());
					ev.AddLine(Loc.GetString(text3, array3));
				}
				if (mostValuableThefts.Count == 0)
				{
					ev.AddLine(Loc.GetString("pirates-stole-nothing"));
				}
			}
			ev.AddLine("");
			ev.AddLine(Loc.GetString("pirates-list-start"));
			foreach (Mind pirates in this._pirates)
			{
				DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(5, 2);
				defaultInterpolatedStringHandler.AppendLiteral("- ");
				defaultInterpolatedStringHandler.AppendFormatted(pirates.CharacterName);
				defaultInterpolatedStringHandler.AppendLiteral(" (");
				IPlayerSession session = pirates.Session;
				defaultInterpolatedStringHandler.AppendFormatted((session != null) ? session.Name : null);
				defaultInterpolatedStringHandler.AppendLiteral(")");
				ev.AddLine(defaultInterpolatedStringHandler.ToStringAndClear());
			}
		}

		// Token: 0x060018E2 RID: 6370 RVA: 0x00081E08 File Offset: 0x00080008
		public override void Started()
		{
		}

		// Token: 0x060018E3 RID: 6371 RVA: 0x00081E0A File Offset: 0x0008000A
		public override void Ended()
		{
		}

		// Token: 0x060018E4 RID: 6372 RVA: 0x00081E0C File Offset: 0x0008000C
		private void OnPlayerSpawningEvent(RulePlayerSpawningEvent ev)
		{
			if (!base.RuleAdded)
			{
				return;
			}
			this._pirates.Clear();
			this._initialItems.Clear();
			int numOps = Math.Max(1, (int)Math.Min(Math.Floor((double)ev.PlayerPool.Count / (double)this._cfg.GetCVar<int>(CCVars.PiratesPlayersPerOp)), (double)this._cfg.GetCVar<int>(CCVars.PiratesMaxOps)));
			IPlayerSession[] ops = new IPlayerSession[numOps];
			for (int i = 0; i < numOps; i++)
			{
				ops[i] = RandomExtensions.PickAndTake<IPlayerSession>(this._random, ev.PlayerPool);
			}
			string map = "/Maps/Shuttles/pirate.yml";
			EntityQuery<TransformComponent> xformQuery = base.GetEntityQuery<TransformComponent>();
			Func<EntityUid, Box2> <>9__2;
			Box2[] aabbs = this._stationSystem.Stations.SelectMany(delegate(EntityUid x)
			{
				IEnumerable<EntityUid> grids = this.Comp<StationDataComponent>(x).Grids;
				Func<EntityUid, Box2> selector;
				if ((selector = <>9__2) == null)
				{
					selector = (<>9__2 = delegate(EntityUid x)
					{
						Matrix3 worldMatrix = xformQuery.GetComponent(x).WorldMatrix;
						Box2 localAABB = this._mapManager.GetGridComp(x).LocalAABB;
						return worldMatrix.TransformBox(ref localAABB);
					});
				}
				return grids.Select(selector);
			}).ToArray<Box2>();
			Box2 aabb = aabbs[0];
			for (int j = 1; j < aabbs.Length; j++)
			{
				aabb.Union(ref aabbs[j]);
			}
			EntityUid? gridId = this._map.LoadGrid(this.GameTicker.DefaultMap, map, new MapLoadOptions
			{
				Offset = aabb.Center + MathF.Max(aabb.Height / 2f, aabb.Width / 2f) * 2.5f
			});
			if (gridId == null)
			{
				Logger.ErrorS("pirates", "Gridid was null when loading \"" + map + "\", aborting.");
				foreach (IPlayerSession session in ops)
				{
					ev.PlayerPool.Add(session);
				}
				return;
			}
			this._pirateShip = gridId.Value;
			StartingGearPrototype pirateGear = this._prototypeManager.Index<StartingGearPrototype>("PirateGear");
			List<EntityCoordinates> spawns = new List<EntityCoordinates>();
			foreach (ValueTuple<SpawnPointComponent, MetaDataComponent, TransformComponent> valueTuple in base.EntityQuery<SpawnPointComponent, MetaDataComponent, TransformComponent>(true))
			{
				MetaDataComponent meta = valueTuple.Item2;
				TransformComponent xform = valueTuple.Item3;
				EntityPrototype entityPrototype = meta.EntityPrototype;
				if (!(((entityPrototype != null) ? entityPrototype.ID : null) != "SpawnPointPirates") && !(xform.ParentUid != this._pirateShip))
				{
					spawns.Add(xform.Coordinates);
				}
			}
			if (spawns.Count == 0)
			{
				spawns.Add(base.Transform(this._pirateShip).Coordinates);
				Logger.WarningS("pirates", "Fell back to default spawn for pirates!");
			}
			for (int k = 0; k < ops.Length; k++)
			{
				Gender gender = RandomExtensions.Prob(this._random, 0.5f) ? 3 : 2;
				string name = this._namingSystem.GetName("Human", new Gender?(gender));
				IPlayerSession session2 = ops[k];
				Mind newMind = new Mind(session2.UserId)
				{
					CharacterName = name
				};
				newMind.ChangeOwningPlayer(new NetUserId?(session2.UserId));
				EntityUid mob = base.Spawn("MobHuman", RandomExtensions.Pick<EntityCoordinates>(this._random, spawns));
				base.MetaData(mob).EntityName = name;
				newMind.TransferTo(new EntityUid?(mob), false, false);
				HumanoidCharacterProfile profile = this._prefs.GetPreferences(session2.UserId).SelectedCharacter as HumanoidCharacterProfile;
				this._stationSpawningSystem.EquipStartingGear(mob, pirateGear, profile);
				this._pirates.Add(newMind);
				this.GameTicker.PlayerJoinGame(session2);
			}
			this._initialShipValue = this._pricingSystem.AppraiseGrid(this._pirateShip, delegate(EntityUid uid)
			{
				this._initialItems.Add(uid);
				return true;
			}, null);
		}

		// Token: 0x060018E5 RID: 6373 RVA: 0x000821C0 File Offset: 0x000803C0
		public void MakePirate(Mind mind)
		{
			if (mind.OwnedEntity == null)
			{
				return;
			}
			SetOutfitCommand.SetOutfit(mind.OwnedEntity.Value, "PirateGear", this.EntityManager, null);
		}

		// Token: 0x060018E6 RID: 6374 RVA: 0x00082200 File Offset: 0x00080400
		private void OnStartAttempt(RoundStartAttemptEvent ev)
		{
			if (!base.RuleAdded)
			{
				return;
			}
			int minPlayers = this._cfg.GetCVar<int>(CCVars.PiratesMinPlayers);
			if (!ev.Forced && ev.Players.Length < minPlayers)
			{
				this._chatManager.DispatchServerAnnouncement(Loc.GetString("nukeops-not-enough-ready-players", new ValueTuple<string, object>[]
				{
					new ValueTuple<string, object>("readyPlayersCount", ev.Players.Length),
					new ValueTuple<string, object>("minimumPlayers", minPlayers)
				}), null);
				ev.Cancel();
				return;
			}
			if (ev.Players.Length == 0)
			{
				this._chatManager.DispatchServerAnnouncement(Loc.GetString("nukeops-no-one-ready"), null);
				ev.Cancel();
			}
		}

		// Token: 0x04000F7C RID: 3964
		[Dependency]
		private readonly IPrototypeManager _prototypeManager;

		// Token: 0x04000F7D RID: 3965
		[Dependency]
		private readonly IRobustRandom _random;

		// Token: 0x04000F7E RID: 3966
		[Dependency]
		private readonly IConfigurationManager _cfg;

		// Token: 0x04000F7F RID: 3967
		[Dependency]
		private readonly IChatManager _chatManager;

		// Token: 0x04000F80 RID: 3968
		[Dependency]
		private readonly IMapManager _mapManager;

		// Token: 0x04000F81 RID: 3969
		[Dependency]
		private readonly IServerPreferencesManager _prefs;

		// Token: 0x04000F82 RID: 3970
		[Dependency]
		private readonly StationSpawningSystem _stationSpawningSystem;

		// Token: 0x04000F83 RID: 3971
		[Dependency]
		private readonly StationSystem _stationSystem;

		// Token: 0x04000F84 RID: 3972
		[Dependency]
		private readonly PricingSystem _pricingSystem;

		// Token: 0x04000F85 RID: 3973
		[Dependency]
		private readonly MapLoaderSystem _map;

		// Token: 0x04000F86 RID: 3974
		[Dependency]
		private readonly NamingSystem _namingSystem;

		// Token: 0x04000F87 RID: 3975
		[ViewVariables]
		private List<Mind> _pirates = new List<Mind>();

		// Token: 0x04000F88 RID: 3976
		[ViewVariables]
		private EntityUid _pirateShip = EntityUid.Invalid;

		// Token: 0x04000F89 RID: 3977
		[ViewVariables]
		private HashSet<EntityUid> _initialItems = new HashSet<EntityUid>();

		// Token: 0x04000F8A RID: 3978
		[ViewVariables]
		private double _initialShipValue;
	}
}
