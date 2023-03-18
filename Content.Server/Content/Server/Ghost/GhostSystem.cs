using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Content.Server.Administration.Logs;
using Content.Server.Chat.Managers;
using Content.Server.GameTicking;
using Content.Server.Ghost.Components;
using Content.Server.Mind;
using Content.Server.Mind.Components;
using Content.Server.Players;
using Content.Server.Roles;
using Content.Server.Warps;
using Content.Shared.Actions;
using Content.Shared.Administration.Logs;
using Content.Shared.CCVar;
using Content.Shared.Chat;
using Content.Shared.Database;
using Content.Shared.Examine;
using Content.Shared.Follower;
using Content.Shared.GameTicking;
using Content.Shared.Ghost;
using Content.Shared.Mobs.Components;
using Content.Shared.Mobs.Systems;
using Content.Shared.Movement.Events;
using Content.Shared.Storage.Components;
using Robust.Server.GameObjects;
using Robust.Server.Player;
using Robust.Shared.Configuration;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Localization;
using Robust.Shared.Log;
using Robust.Shared.Maths;
using Robust.Shared.Network;
using Robust.Shared.Physics.Components;
using Robust.Shared.Physics.Systems;
using Robust.Shared.Players;
using Robust.Shared.Timing;

namespace Content.Server.Ghost
{
	// Token: 0x02000491 RID: 1169
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class GhostSystem : SharedGhostSystem
	{
		// Token: 0x06001765 RID: 5989 RVA: 0x0007A8E4 File Offset: 0x00078AE4
		public override void Initialize()
		{
			base.Initialize();
			base.SubscribeLocalEvent<GhostComponent, ComponentStartup>(new ComponentEventHandler<GhostComponent, ComponentStartup>(this.OnGhostStartup), null, null);
			base.SubscribeLocalEvent<GhostComponent, ComponentShutdown>(new ComponentEventHandler<GhostComponent, ComponentShutdown>(this.OnGhostShutdown), null, null);
			base.SubscribeLocalEvent<GhostComponent, ExaminedEvent>(new ComponentEventHandler<GhostComponent, ExaminedEvent>(this.OnGhostExamine), null, null);
			base.SubscribeLocalEvent<GhostComponent, MindRemovedMessage>(new ComponentEventHandler<GhostComponent, MindRemovedMessage>(this.OnMindRemovedMessage), null, null);
			base.SubscribeLocalEvent<GhostComponent, MindUnvisitedMessage>(new ComponentEventHandler<GhostComponent, MindUnvisitedMessage>(this.OnMindUnvisitedMessage), null, null);
			base.SubscribeLocalEvent<GhostComponent, PlayerDetachedEvent>(new ComponentEventHandler<GhostComponent, PlayerDetachedEvent>(this.OnPlayerDetached), null, null);
			base.SubscribeLocalEvent<GhostOnMoveComponent, MoveInputEvent>(new ComponentEventRefHandler<GhostOnMoveComponent, MoveInputEvent>(this.OnRelayMoveInput), null, null);
			base.SubscribeNetworkEvent<GhostWarpsRequestEvent>(new EntitySessionEventHandler<GhostWarpsRequestEvent>(this.OnGhostWarpsRequest), null, null);
			base.SubscribeNetworkEvent<GhostReturnToBodyRequest>(new EntitySessionEventHandler<GhostReturnToBodyRequest>(this.OnGhostReturnToBodyRequest), null, null);
			base.SubscribeNetworkEvent<GhostWarpToTargetRequestEvent>(new EntitySessionEventHandler<GhostWarpToTargetRequestEvent>(this.OnGhostWarpToTargetRequest), null, null);
			base.SubscribeNetworkEvent<GhostReturnToRoundRequest>(new EntitySessionEventHandler<GhostReturnToRoundRequest>(this.OnGhostReturnToRoundRequest), null, null);
			base.SubscribeLocalEvent<GhostComponent, BooActionEvent>(new ComponentEventHandler<GhostComponent, BooActionEvent>(this.OnActionPerform), null, null);
			base.SubscribeLocalEvent<GhostComponent, InsertIntoEntityStorageAttemptEvent>(new ComponentEventRefHandler<GhostComponent, InsertIntoEntityStorageAttemptEvent>(this.OnEntityStorageInsertAttempt), null, null);
			base.SubscribeLocalEvent<RoundEndTextAppendEvent>(delegate(RoundEndTextAppendEvent _)
			{
				this.MakeVisible(true);
			}, null, null);
			base.SubscribeLocalEvent<RoundRestartCleanupEvent>(new EntityEventHandler<RoundRestartCleanupEvent>(this.ResetDeathTimes), null, null);
		}

		// Token: 0x06001766 RID: 5990 RVA: 0x0007AA23 File Offset: 0x00078C23
		private void ResetDeathTimes(RoundRestartCleanupEvent ev)
		{
			this._deathTime.Clear();
		}

		// Token: 0x06001767 RID: 5991 RVA: 0x0007AA30 File Offset: 0x00078C30
		private void OnGhostReturnToRoundRequest(GhostReturnToRoundRequest msg, EntitySessionEventArgs args)
		{
			IConfigurationManager cfg = IoCManager.Resolve<IConfigurationManager>();
			int maxPlayers = cfg.GetCVar<int>(CCVars.GhostRespawnMaxPlayers);
			if (this._playerManager.PlayerCount >= maxPlayers)
			{
				string message = Loc.GetString("ghost-respawn-max-players", new ValueTuple<string, object>[]
				{
					new ValueTuple<string, object>("players", maxPlayers)
				});
				string wrappedMessage = Loc.GetString("chat-manager-server-wrap-message", new ValueTuple<string, object>[]
				{
					new ValueTuple<string, object>("message", message)
				});
				this._chatManager.ChatMessageToOne(ChatChannel.Server, message, wrappedMessage, default(EntityUid), false, args.SenderSession.ConnectedClient, new Color?(Color.Red), false, null, 0f);
				return;
			}
			NetUserId userId = args.SenderSession.UserId;
			TimeSpan deathTime;
			if (!this._deathTime.TryGetValue(userId, out deathTime))
			{
				string message2 = Loc.GetString("ghost-respawn-bug");
				string wrappedMessage2 = Loc.GetString("chat-manager-server-wrap-message", new ValueTuple<string, object>[]
				{
					new ValueTuple<string, object>("message", message2)
				});
				this._chatManager.ChatMessageToOne(ChatChannel.Server, message2, wrappedMessage2, default(EntityUid), false, args.SenderSession.ConnectedClient, new Color?(Color.Red), false, null, 0f);
				this._deathTime[userId] = this._gameTiming.CurTime;
				return;
			}
			double timeUntilRespawn = (double)cfg.GetCVar<float>(CCVars.GhostRespawnTime);
			double timePast = (this._gameTiming.CurTime - deathTime).TotalMinutes;
			if (timePast >= timeUntilRespawn)
			{
				GameTicker ticker = EntitySystem.Get<GameTicker>();
				IPlayerSession targetPlayer;
				IoCManager.Resolve<IPlayerManager>().TryGetSessionById(userId, ref targetPlayer);
				if (targetPlayer != null)
				{
					ticker.Respawn(targetPlayer);
				}
				this._deathTime.Remove(userId);
				ISharedAdminLogManager adminLogger = this._adminLogger;
				LogType type = LogType.Mind;
				LogImpact impact = LogImpact.Extreme;
				LogStringHandler logStringHandler = new LogStringHandler(44, 1);
				logStringHandler.AppendFormatted(args.SenderSession.ConnectedClient.UserName);
				logStringHandler.AppendLiteral(" вернулся в лобби посредством гост респавна.");
				adminLogger.Add(type, impact, ref logStringHandler);
				string message3 = Loc.GetString("ghost-respawn-window-rules-footer");
				string wrappedMessage3 = Loc.GetString("chat-manager-server-wrap-message", new ValueTuple<string, object>[]
				{
					new ValueTuple<string, object>("message", message3)
				});
				this._chatManager.ChatMessageToOne(ChatChannel.Server, message3, wrappedMessage3, default(EntityUid), false, args.SenderSession.ConnectedClient, new Color?(Color.Red), false, null, 0f);
				return;
			}
			string message4 = Loc.GetString("ghost-respawn-time-left", new ValueTuple<string, object>[]
			{
				new ValueTuple<string, object>("time", (int)(timeUntilRespawn - timePast))
			});
			string wrappedMessage4 = Loc.GetString("chat-manager-server-wrap-message", new ValueTuple<string, object>[]
			{
				new ValueTuple<string, object>("message", message4)
			});
			this._chatManager.ChatMessageToOne(ChatChannel.Server, message4, wrappedMessage4, default(EntityUid), false, args.SenderSession.ConnectedClient, new Color?(Color.Red), false, null, 0f);
		}

		// Token: 0x06001768 RID: 5992 RVA: 0x0007AD20 File Offset: 0x00078F20
		private void OnActionPerform(EntityUid uid, GhostComponent component, BooActionEvent args)
		{
			if (args.Handled)
			{
				return;
			}
			HashSet<EntityUid> entitiesInRange = this._lookup.GetEntitiesInRange(args.Performer, component.BooRadius, 46);
			int booCounter = 0;
			foreach (EntityUid ent in entitiesInRange)
			{
				if (this.DoGhostBooEvent(ent))
				{
					booCounter++;
				}
				if (booCounter >= component.BooMaxTargets)
				{
					break;
				}
			}
			args.Handled = true;
		}

		// Token: 0x06001769 RID: 5993 RVA: 0x0007ADAC File Offset: 0x00078FAC
		private void OnRelayMoveInput(EntityUid uid, GhostOnMoveComponent component, ref MoveInputEvent args)
		{
			if (this.EntityManager.HasComponent<VisitingMindComponent>(uid))
			{
				return;
			}
			MindComponent mind;
			if (!this.EntityManager.TryGetComponent<MindComponent>(uid, ref mind) || !mind.HasMind || mind.Mind.IsVisitingEntity)
			{
				return;
			}
			if (component.MustBeDead && (this._mobState.IsAlive(uid, null) || this._mobState.IsCritical(uid, null)))
			{
				return;
			}
			this._ticker.OnGhostAttempt(mind.Mind, component.CanReturn, false);
		}

		// Token: 0x0600176A RID: 5994 RVA: 0x0007AE30 File Offset: 0x00079030
		private void OnGhostStartup(EntityUid uid, GhostComponent component, ComponentStartup args)
		{
			VisibilityComponent visibility = this.EntityManager.EnsureComponent<VisibilityComponent>(component.Owner);
			if (this._ticker.RunLevel != GameRunLevel.PostRound)
			{
				this._visibilitySystem.AddLayer(visibility, 2, false);
				this._visibilitySystem.RemoveLayer(visibility, 1, false);
				this._visibilitySystem.RefreshVisibility(visibility);
			}
			EyeComponent eye;
			if (this.EntityManager.TryGetComponent<EyeComponent>(component.Owner, ref eye))
			{
				eye.VisibilityMask |= 2U;
			}
			TimeSpan time = this._gameTiming.CurTime;
			component.TimeOfDeath = time;
			if (component.Action.UseDelay != null)
			{
				component.Action.Cooldown = new ValueTuple<TimeSpan, TimeSpan>?(new ValueTuple<TimeSpan, TimeSpan>(time, time + component.Action.UseDelay.Value));
			}
			this._actions.AddAction(uid, component.Action, null, null, true);
		}

		// Token: 0x0600176B RID: 5995 RVA: 0x0007AF18 File Offset: 0x00079118
		private void OnGhostShutdown(EntityUid uid, GhostComponent component, ComponentShutdown args)
		{
			if (!base.Terminating(uid, null))
			{
				VisibilityComponent visibility;
				if (this.EntityManager.TryGetComponent<VisibilityComponent>(component.Owner, ref visibility))
				{
					this._visibilitySystem.RemoveLayer(visibility, 2, false);
					this._visibilitySystem.AddLayer(visibility, 1, false);
					this._visibilitySystem.RefreshVisibility(visibility);
				}
				EyeComponent eye;
				if (this.EntityManager.TryGetComponent<EyeComponent>(component.Owner, ref eye))
				{
					eye.VisibilityMask &= 4294967293U;
				}
				this._actions.RemoveAction(uid, component.Action, null);
			}
		}

		// Token: 0x0600176C RID: 5996 RVA: 0x0007AFA4 File Offset: 0x000791A4
		private void OnGhostExamine(EntityUid uid, GhostComponent component, ExaminedEvent args)
		{
			TimeSpan timeSinceDeath = this._gameTiming.RealTime.Subtract(component.TimeOfDeath);
			string deathTimeInfo = (timeSinceDeath.Minutes > 0) ? Loc.GetString("comp-ghost-examine-time-minutes", new ValueTuple<string, object>[]
			{
				new ValueTuple<string, object>("minutes", timeSinceDeath.Minutes)
			}) : Loc.GetString("comp-ghost-examine-time-seconds", new ValueTuple<string, object>[]
			{
				new ValueTuple<string, object>("seconds", timeSinceDeath.Seconds)
			});
			args.PushMarkup(deathTimeInfo);
		}

		// Token: 0x0600176D RID: 5997 RVA: 0x0007B039 File Offset: 0x00079239
		private void OnMindRemovedMessage(EntityUid uid, GhostComponent component, MindRemovedMessage args)
		{
			this.DeleteEntity(uid);
		}

		// Token: 0x0600176E RID: 5998 RVA: 0x0007B042 File Offset: 0x00079242
		private void OnMindUnvisitedMessage(EntityUid uid, GhostComponent component, MindUnvisitedMessage args)
		{
			this.DeleteEntity(uid);
		}

		// Token: 0x0600176F RID: 5999 RVA: 0x0007B04B File Offset: 0x0007924B
		private void OnPlayerDetached(EntityUid uid, GhostComponent component, PlayerDetachedEvent args)
		{
			base.QueueDel(uid);
		}

		// Token: 0x06001770 RID: 6000 RVA: 0x0007B054 File Offset: 0x00079254
		private void OnGhostWarpsRequest(GhostWarpsRequestEvent msg, EntitySessionEventArgs args)
		{
			EntityUid? attachedEntity = args.SenderSession.AttachedEntity;
			if (attachedEntity != null)
			{
				EntityUid entity = attachedEntity.GetValueOrDefault();
				if (entity.Valid && this.EntityManager.HasComponent<GhostComponent>(entity))
				{
					GhostWarpsResponseEvent response = new GhostWarpsResponseEvent(this.GetPlayerWarps(entity).Concat(this.GetLocationWarps()).ToList<GhostWarp>());
					base.RaiseNetworkEvent(response, args.SenderSession.ConnectedClient);
					return;
				}
			}
			DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(36, 2);
			defaultInterpolatedStringHandler.AppendLiteral("User ");
			defaultInterpolatedStringHandler.AppendFormatted(args.SenderSession.Name);
			defaultInterpolatedStringHandler.AppendLiteral(" sent a ");
			defaultInterpolatedStringHandler.AppendFormatted("GhostWarpsRequestEvent");
			defaultInterpolatedStringHandler.AppendLiteral(" without being a ghost.");
			Logger.Warning(defaultInterpolatedStringHandler.ToStringAndClear());
		}

		// Token: 0x06001771 RID: 6001 RVA: 0x0007B120 File Offset: 0x00079320
		private void OnGhostReturnToBodyRequest(GhostReturnToBodyRequest msg, EntitySessionEventArgs args)
		{
			EntityUid? attachedEntity = args.SenderSession.AttachedEntity;
			if (attachedEntity != null)
			{
				EntityUid attached = attachedEntity.GetValueOrDefault();
				GhostComponent ghost;
				ActorComponent actor;
				if (attached.Valid && this.EntityManager.TryGetComponent<GhostComponent>(attached, ref ghost) && ghost.CanReturnToBody && this.EntityManager.TryGetComponent<ActorComponent>(attached, ref actor))
				{
					Mind mind = actor.PlayerSession.ContentData().Mind;
					if (mind == null)
					{
						return;
					}
					mind.UnVisit();
					return;
				}
			}
			Logger.Warning("User " + args.SenderSession.Name + " sent an invalid GhostReturnToBodyRequest");
		}

		// Token: 0x06001772 RID: 6002 RVA: 0x0007B1B8 File Offset: 0x000793B8
		private void OnGhostWarpToTargetRequest(GhostWarpToTargetRequestEvent msg, EntitySessionEventArgs args)
		{
			EntityUid? attachedEntity = args.SenderSession.AttachedEntity;
			DefaultInterpolatedStringHandler defaultInterpolatedStringHandler;
			if (attachedEntity != null)
			{
				EntityUid attached = attachedEntity.GetValueOrDefault();
				GhostComponent ghost;
				if (attached.Valid && this.EntityManager.TryGetComponent<GhostComponent>(attached, ref ghost))
				{
					if (!this.EntityManager.EntityExists(msg.Target))
					{
						defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(45, 2);
						defaultInterpolatedStringHandler.AppendLiteral("User ");
						defaultInterpolatedStringHandler.AppendFormatted(args.SenderSession.Name);
						defaultInterpolatedStringHandler.AppendLiteral(" tried to warp to an invalid entity id: ");
						defaultInterpolatedStringHandler.AppendFormatted<EntityUid>(msg.Target);
						Logger.Warning(defaultInterpolatedStringHandler.ToStringAndClear());
						return;
					}
					WarpPointComponent warp;
					if ((base.TryComp<WarpPointComponent>(msg.Target, ref warp) && warp.Follow) || base.HasComp<MobStateComponent>(msg.Target))
					{
						this._followerSystem.StartFollowingEntity(ghost.Owner, msg.Target);
						return;
					}
					TransformComponent transformComponent = base.Transform(ghost.Owner);
					transformComponent.Coordinates = base.Transform(msg.Target).Coordinates;
					transformComponent.AttachToGridOrMap();
					PhysicsComponent physics;
					if (base.TryComp<PhysicsComponent>(attached, ref physics))
					{
						this._physics.SetLinearVelocity(attached, Vector2.Zero, true, true, null, physics);
					}
					return;
				}
			}
			defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(46, 2);
			defaultInterpolatedStringHandler.AppendLiteral("User ");
			defaultInterpolatedStringHandler.AppendFormatted(args.SenderSession.Name);
			defaultInterpolatedStringHandler.AppendLiteral(" tried to warp to ");
			defaultInterpolatedStringHandler.AppendFormatted<EntityUid>(msg.Target);
			defaultInterpolatedStringHandler.AppendLiteral(" without being a ghost.");
			Logger.Warning(defaultInterpolatedStringHandler.ToStringAndClear());
		}

		// Token: 0x06001773 RID: 6003 RVA: 0x0007B340 File Offset: 0x00079540
		private void DeleteEntity(EntityUid uid)
		{
			if (base.Deleted(uid, null) || base.Terminating(uid, null))
			{
				return;
			}
			MindComponent mind;
			if (this.EntityManager.TryGetComponent<MindComponent>(uid, ref mind))
			{
				this._mindSystem.SetGhostOnShutdown(uid, false, mind);
			}
			this.EntityManager.DeleteEntity(uid);
		}

		// Token: 0x06001774 RID: 6004 RVA: 0x0007B38C File Offset: 0x0007958C
		private IEnumerable<GhostWarp> GetLocationWarps()
		{
			foreach (WarpPointComponent warp in this.EntityManager.EntityQuery<WarpPointComponent>(true))
			{
				if (warp.Location != null)
				{
					yield return new GhostWarp(warp.Owner, warp.Location, true);
				}
			}
			IEnumerator<WarpPointComponent> enumerator = null;
			yield break;
			yield break;
		}

		// Token: 0x06001775 RID: 6005 RVA: 0x0007B39C File Offset: 0x0007959C
		private IEnumerable<GhostWarp> GetPlayerWarps(EntityUid except)
		{
			foreach (ICommonSession commonSession in this._playerManager.Sessions)
			{
				EntityUid? attachedEntity = commonSession.AttachedEntity;
				if (attachedEntity != null)
				{
					EntityUid attached = attachedEntity.GetValueOrDefault();
					if (attached.Valid && !(attached == except))
					{
						MindComponent mind;
						base.TryComp<MindComponent>(attached, ref mind);
						string entityName = this.EntityManager.GetComponent<MetaDataComponent>(attached).EntityName;
						string str = " (";
						string text;
						if (mind == null)
						{
							text = null;
						}
						else
						{
							Mind mind2 = mind.Mind;
							if (mind2 == null)
							{
								text = null;
							}
							else
							{
								Job currentJob = mind2.CurrentJob;
								text = ((currentJob != null) ? currentJob.Name : null);
							}
						}
						string playerInfo = entityName + str + (text ?? "Unknown") + ")";
						if (this._mobState.IsAlive(attached, null) || this._mobState.IsCritical(attached, null))
						{
							yield return new GhostWarp(attached, playerInfo, false);
						}
					}
				}
			}
			IEnumerator<ICommonSession> enumerator = null;
			yield break;
			yield break;
		}

		// Token: 0x06001776 RID: 6006 RVA: 0x0007B3B3 File Offset: 0x000795B3
		private void OnEntityStorageInsertAttempt(EntityUid uid, GhostComponent comp, ref InsertIntoEntityStorageAttemptEvent args)
		{
			args.Cancelled = true;
		}

		// Token: 0x06001777 RID: 6007 RVA: 0x0007B3BC File Offset: 0x000795BC
		public void MakeVisible(bool visible)
		{
			foreach (ValueTuple<GhostComponent, VisibilityComponent> valueTuple in base.EntityQuery<GhostComponent, VisibilityComponent>(false))
			{
				VisibilityComponent vis = valueTuple.Item2;
				if (visible)
				{
					this._visibilitySystem.AddLayer(vis, 1, false);
					this._visibilitySystem.RemoveLayer(vis, 2, false);
				}
				else
				{
					this._visibilitySystem.AddLayer(vis, 2, false);
					this._visibilitySystem.RemoveLayer(vis, 1, false);
				}
				this._visibilitySystem.RefreshVisibility(vis);
			}
		}

		// Token: 0x06001778 RID: 6008 RVA: 0x0007B454 File Offset: 0x00079654
		public bool DoGhostBooEvent(EntityUid target)
		{
			GhostBooEvent ghostBoo = new GhostBooEvent();
			base.RaiseLocalEvent<GhostBooEvent>(target, ghostBoo, true);
			return ghostBoo.Handled;
		}

		// Token: 0x04000E94 RID: 3732
		[Dependency]
		private readonly IGameTiming _gameTiming;

		// Token: 0x04000E95 RID: 3733
		[Dependency]
		private readonly IPlayerManager _playerManager;

		// Token: 0x04000E96 RID: 3734
		[Dependency]
		private readonly GameTicker _ticker;

		// Token: 0x04000E97 RID: 3735
		[Dependency]
		private readonly MindSystem _mindSystem;

		// Token: 0x04000E98 RID: 3736
		[Dependency]
		private readonly SharedActionsSystem _actions;

		// Token: 0x04000E99 RID: 3737
		[Dependency]
		private readonly VisibilitySystem _visibilitySystem;

		// Token: 0x04000E9A RID: 3738
		[Dependency]
		private readonly EntityLookupSystem _lookup;

		// Token: 0x04000E9B RID: 3739
		[Dependency]
		private readonly FollowerSystem _followerSystem;

		// Token: 0x04000E9C RID: 3740
		[Dependency]
		private readonly MobStateSystem _mobState;

		// Token: 0x04000E9D RID: 3741
		[Dependency]
		private readonly SharedPhysicsSystem _physics;

		// Token: 0x04000E9E RID: 3742
		[Dependency]
		private readonly IChatManager _chatManager;

		// Token: 0x04000E9F RID: 3743
		[Dependency]
		private readonly IAdminLogManager _adminLogger;

		// Token: 0x04000EA0 RID: 3744
		public readonly Dictionary<NetUserId, TimeSpan> _deathTime = new Dictionary<NetUserId, TimeSpan>();
	}
}
