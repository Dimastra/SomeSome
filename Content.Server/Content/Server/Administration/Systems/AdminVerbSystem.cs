using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using Content.Server.Administration.Commands;
using Content.Server.Administration.Components;
using Content.Server.Administration.Logs;
using Content.Server.Administration.Managers;
using Content.Server.Administration.UI;
using Content.Server.Atmos;
using Content.Server.Atmos.Components;
using Content.Server.Atmos.EntitySystems;
using Content.Server.Body.Components;
using Content.Server.Body.Systems;
using Content.Server.Cargo.Components;
using Content.Server.Chat.Systems;
using Content.Server.Chemistry.Components.SolutionManager;
using Content.Server.Chemistry.EntitySystems;
using Content.Server.Damage.Systems;
using Content.Server.Disease;
using Content.Server.Disease.Components;
using Content.Server.Disposal.Tube.Components;
using Content.Server.Doors.Systems;
using Content.Server.Electrocution;
using Content.Server.EUI;
using Content.Server.Explosion.EntitySystems;
using Content.Server.GameTicking.Rules;
using Content.Server.Ghost.Roles;
using Content.Server.GhostKick;
using Content.Server.Hands.Components;
using Content.Server.Hands.Systems;
using Content.Server.Medical;
using Content.Server.Mind;
using Content.Server.Mind.Commands;
using Content.Server.Mind.Components;
using Content.Server.Nutrition.EntitySystems;
using Content.Server.Players;
using Content.Server.Pointing.Components;
using Content.Server.Polymorph.Systems;
using Content.Server.Popups;
using Content.Server.Power.Components;
using Content.Server.Prayer;
using Content.Server.Speech.Components;
using Content.Server.Stack;
using Content.Server.Station.Components;
using Content.Server.Station.Systems;
using Content.Server.Storage.Components;
using Content.Server.Storage.EntitySystems;
using Content.Server.Tabletop;
using Content.Server.Tabletop.Components;
using Content.Server.Tools.Systems;
using Content.Server.Xenoarchaeology.XenoArtifacts;
using Content.Server.Xenoarchaeology.XenoArtifacts.Triggers.Components;
using Content.Server.Zombies;
using Content.Shared.Access;
using Content.Shared.Access.Components;
using Content.Shared.Access.Systems;
using Content.Shared.Administration;
using Content.Shared.Atmos;
using Content.Shared.Body.Components;
using Content.Shared.Body.Organ;
using Content.Shared.Body.Part;
using Content.Shared.Clothing.Components;
using Content.Shared.Configurable;
using Content.Shared.Construction.Components;
using Content.Shared.Damage;
using Content.Shared.Database;
using Content.Shared.Disease;
using Content.Shared.Doors.Components;
using Content.Shared.Electrocution;
using Content.Shared.FixedPoint;
using Content.Shared.GameTicking;
using Content.Shared.Interaction.Components;
using Content.Shared.Interaction.Helpers;
using Content.Shared.Inventory;
using Content.Shared.Mobs;
using Content.Shared.Mobs.Components;
using Content.Shared.Mobs.Systems;
using Content.Shared.Movement.Components;
using Content.Shared.Movement.Systems;
using Content.Shared.Nutrition.Components;
using Content.Shared.PDA;
using Content.Shared.Popups;
using Content.Shared.Speech;
using Content.Shared.Stacks;
using Content.Shared.Tabletop.Components;
using Content.Shared.Verbs;
using Content.Shared.Weapons.Ranged.Components;
using Robust.Server.Console;
using Robust.Server.GameObjects;
using Robust.Server.Physics;
using Robust.Server.Player;
using Robust.Shared.Console;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Localization;
using Robust.Shared.Map;
using Robust.Shared.Map.Components;
using Robust.Shared.Maths;
using Robust.Shared.Network;
using Robust.Shared.Physics;
using Robust.Shared.Physics.Components;
using Robust.Shared.Physics.Dynamics;
using Robust.Shared.Physics.Systems;
using Robust.Shared.Player;
using Robust.Shared.Players;
using Robust.Shared.Prototypes;
using Robust.Shared.Random;
using Robust.Shared.Timing;
using Robust.Shared.Utility;

namespace Content.Server.Administration.Systems
{
	// Token: 0x0200080D RID: 2061
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class AdminVerbSystem : EntitySystem
	{
		// Token: 0x06002CC0 RID: 11456 RVA: 0x000E91CC File Offset: 0x000E73CC
		private void AddAntagVerbs(GetVerbsEvent<Verb> args)
		{
			ActorComponent actor;
			if (!this.EntityManager.TryGetComponent<ActorComponent>(args.User, ref actor))
			{
				return;
			}
			IPlayerSession player = actor.PlayerSession;
			if (!this._adminManager.HasAdminFlag(player, AdminFlags.Fun))
			{
				return;
			}
			MindComponent targetMindComp;
			if (!base.TryComp<MindComponent>(args.Target, ref targetMindComp) || targetMindComp == null)
			{
				return;
			}
			Verb traitor = new Verb
			{
				Text = "Make Traitor",
				Category = VerbCategory.Antag,
				Icon = new SpriteSpecifier.Rsi(new ResourcePath("/Textures/Structures/Wallmounts/posters.rsi", "/"), "poster5_contraband"),
				Act = delegate()
				{
					if (targetMindComp.Mind == null || targetMindComp.Mind.Session == null)
					{
						return;
					}
					this._traitorRule.MakeTraitor(targetMindComp.Mind.Session);
				},
				Impact = LogImpact.High,
				Message = Loc.GetString("admin-verb-make-traitor")
			};
			args.Verbs.Add(traitor);
			Verb zombie = new Verb
			{
				Text = "Make Zombie",
				Category = VerbCategory.Antag,
				Icon = new SpriteSpecifier.Rsi(new ResourcePath("/Textures/Structures/Wallmounts/signs.rsi", "/"), "bio"),
				Act = delegate()
				{
					MindComponent mindComp;
					this.TryComp<MindComponent>(args.Target, ref mindComp);
					if (mindComp == null || mindComp.Mind == null)
					{
						return;
					}
					this._zombify.ZombifyEntity(targetMindComp.Owner);
				},
				Impact = LogImpact.High,
				Message = Loc.GetString("admin-verb-make-zombie")
			};
			args.Verbs.Add(zombie);
			Verb nukeOp = new Verb
			{
				Text = "Make nuclear operative",
				Category = VerbCategory.Antag,
				Icon = new SpriteSpecifier.Rsi(new ResourcePath("/Textures/Structures/Wallmounts/signs.rsi", "/"), "radiation"),
				Act = delegate()
				{
					if (targetMindComp.Mind == null || targetMindComp.Mind.Session == null)
					{
						return;
					}
					this._nukeopsRule.MakeLoneNukie(targetMindComp.Mind);
				},
				Impact = LogImpact.High,
				Message = Loc.GetString("admin-verb-make-nuclear-operative")
			};
			args.Verbs.Add(nukeOp);
			Verb pirate = new Verb
			{
				Text = "Make Pirate",
				Category = VerbCategory.Antag,
				Icon = new SpriteSpecifier.Rsi(new ResourcePath("/Textures/Clothing/Head/Hats/pirate.rsi", "/"), "icon"),
				Act = delegate()
				{
					if (targetMindComp.Mind == null || targetMindComp.Mind.Session == null)
					{
						return;
					}
					this._piratesRule.MakePirate(targetMindComp.Mind);
				},
				Impact = LogImpact.High,
				Message = Loc.GetString("admin-verb-make-pirate")
			};
			args.Verbs.Add(pirate);
		}

		// Token: 0x06002CC1 RID: 11457 RVA: 0x000E9420 File Offset: 0x000E7620
		public override void Initialize()
		{
			base.SubscribeLocalEvent<GetVerbsEvent<Verb>>(new EntityEventHandler<GetVerbsEvent<Verb>>(this.AddAdminVerbs), null, null);
			base.SubscribeLocalEvent<GetVerbsEvent<Verb>>(new EntityEventHandler<GetVerbsEvent<Verb>>(this.AddDebugVerbs), null, null);
			base.SubscribeLocalEvent<GetVerbsEvent<Verb>>(new EntityEventHandler<GetVerbsEvent<Verb>>(this.AddSmiteVerbs), null, null);
			base.SubscribeLocalEvent<GetVerbsEvent<Verb>>(new EntityEventHandler<GetVerbsEvent<Verb>>(this.AddTricksVerbs), null, null);
			base.SubscribeLocalEvent<GetVerbsEvent<Verb>>(new EntityEventHandler<GetVerbsEvent<Verb>>(this.AddAntagVerbs), null, null);
			base.SubscribeLocalEvent<RoundRestartCleanupEvent>(new EntityEventHandler<RoundRestartCleanupEvent>(this.Reset), null, null);
			base.SubscribeLocalEvent<SolutionContainerManagerComponent, SolutionChangedEvent>(new ComponentEventHandler<SolutionContainerManagerComponent, SolutionChangedEvent>(this.OnSolutionChanged), null, null);
		}

		// Token: 0x06002CC2 RID: 11458 RVA: 0x000E94BC File Offset: 0x000E76BC
		private void AddAdminVerbs(GetVerbsEvent<Verb> args)
		{
			ActorComponent actor;
			if (!this.EntityManager.TryGetComponent<ActorComponent>(args.User, ref actor))
			{
				return;
			}
			IPlayerSession player = actor.PlayerSession;
			if (this._adminManager.HasAdminFlag(player, AdminFlags.Admin))
			{
				ActorComponent targetActor;
				if (base.TryComp<ActorComponent>(args.Target, ref targetActor))
				{
					Verb verb = new Verb();
					verb.Text = Loc.GetString("ahelp-verb-get-data-text");
					verb.Category = VerbCategory.Admin;
					verb.Icon = new SpriteSpecifier.Texture(new ResourcePath("/Textures/Interface/gavel.svg.192dpi.png", "/"));
					verb.Act = delegate()
					{
						IConsoleHost console = this._console;
						ICommonSession player = player;
						DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(12, 1);
						defaultInterpolatedStringHandler.AppendLiteral("openahelp \"");
						defaultInterpolatedStringHandler.AppendFormatted<NetUserId>(targetActor.PlayerSession.UserId);
						defaultInterpolatedStringHandler.AppendLiteral("\"");
						console.RemoteExecuteCommand(player, defaultInterpolatedStringHandler.ToStringAndClear());
					};
					verb.Impact = LogImpact.Low;
					args.Verbs.Add(verb);
					Verb prayerVerb = new Verb();
					prayerVerb.Text = Loc.GetString("prayer-verbs-subtle-message");
					prayerVerb.Category = VerbCategory.Admin;
					prayerVerb.Icon = new SpriteSpecifier.Texture(new ResourcePath("/Textures/Interface/pray.svg.png", "/"));
					Action<string, string> <>9__2;
					prayerVerb.Act = delegate()
					{
						QuickDialogSystem quickDialog = this._quickDialog;
						IPlayerSession player = player;
						string title = "Subtle Message";
						string prompt = "Message";
						string prompt2 = "Popup Message";
						Action<string, string> okAction;
						if ((okAction = <>9__2) == null)
						{
							okAction = (<>9__2 = delegate(string message, string popupMessage)
							{
								this._prayerSystem.SendSubtleMessage(targetActor.PlayerSession, player, message, (popupMessage == "") ? Loc.GetString("prayer-popup-subtle-default") : popupMessage);
							});
						}
						quickDialog.OpenDialog<string, string>(player, title, prompt, prompt2, okAction, null);
					};
					prayerVerb.Impact = LogImpact.Low;
					args.Verbs.Add(prayerVerb);
					bool frozen = base.HasComp<AdminFrozenComponent>(args.Target);
					args.Verbs.Add(new Verb
					{
						Priority = -1,
						Text = (frozen ? Loc.GetString("admin-verbs-unfreeze") : Loc.GetString("admin-verbs-freeze")),
						Category = VerbCategory.Admin,
						Icon = new SpriteSpecifier.Texture(new ResourcePath("/Textures/Interface/VerbIcons/snow.svg.192dpi.png", "/")),
						Act = delegate()
						{
							if (frozen)
							{
								this.RemComp<AdminFrozenComponent>(args.Target);
								return;
							}
							this.EnsureComp<AdminFrozenComponent>(args.Target);
						},
						Impact = LogImpact.Medium
					});
				}
				if (this._adminManager.HasAdminFlag(player, AdminFlags.Logs))
				{
					Verb logsVerbEntity = new Verb
					{
						Priority = -2,
						Text = Loc.GetString("admin-verbs-admin-logs-entity"),
						Category = VerbCategory.Admin,
						Act = delegate()
						{
							AdminLogsEui ui = new AdminLogsEui();
							this._eui.OpenEui(ui, player);
							MindComponent mind;
							if (this._entMan.TryGetComponent<MindComponent>(args.Target, ref mind) && mind.Mind != null && mind.Mind.Session != null)
							{
								string filter = mind.Mind.Session.Data.UserName;
								ui.SetLogFilter(filter, false, null);
								return;
							}
							ui.SetLogFilter(args.Target.GetHashCode().ToString(), false, null);
						},
						Impact = LogImpact.Low
					};
					args.Verbs.Add(logsVerbEntity);
				}
				args.Verbs.Add(new Verb
				{
					Text = Loc.GetString("admin-verbs-teleport-to"),
					Category = VerbCategory.Admin,
					Icon = new SpriteSpecifier.Texture(new ResourcePath("/Textures/Interface/VerbIcons/open.svg.192dpi.png", "/")),
					Act = delegate()
					{
						IConsoleHost console = this._console;
						ICommonSession player = player;
						DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(5, 1);
						defaultInterpolatedStringHandler.AppendLiteral("tpto ");
						defaultInterpolatedStringHandler.AppendFormatted<EntityUid>(args.Target);
						console.ExecuteCommand(player, defaultInterpolatedStringHandler.ToStringAndClear());
					},
					Impact = LogImpact.Low
				});
				args.Verbs.Add(new Verb
				{
					Text = Loc.GetString("admin-verbs-teleport-here"),
					Category = VerbCategory.Admin,
					Icon = new SpriteSpecifier.Texture(new ResourcePath("/Textures/Interface/VerbIcons/close.svg.192dpi.png", "/")),
					Act = delegate()
					{
						IConsoleHost console = this._console;
						ICommonSession player = player;
						DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(6, 2);
						defaultInterpolatedStringHandler.AppendLiteral("tpto ");
						defaultInterpolatedStringHandler.AppendFormatted<EntityUid>(args.Target);
						defaultInterpolatedStringHandler.AppendLiteral(" ");
						defaultInterpolatedStringHandler.AppendFormatted<EntityUid>(args.User);
						console.ExecuteCommand(player, defaultInterpolatedStringHandler.ToStringAndClear());
					},
					Impact = LogImpact.Low
				});
				if (base.HasComp<ActorComponent>(args.Target))
				{
					args.Verbs.Add(new Verb
					{
						Text = Loc.GetString("admin-player-actions-respawn"),
						Category = VerbCategory.Admin,
						Act = delegate()
						{
							ActorComponent actor2;
							if (!this.TryComp<ActorComponent>(args.Target, ref actor2))
							{
								return;
							}
							this._console.ExecuteCommand(player, "respawn " + actor2.PlayerSession.Name);
						},
						ConfirmationPopup = true
					});
				}
			}
		}

		// Token: 0x06002CC3 RID: 11459 RVA: 0x000E98C0 File Offset: 0x000E7AC0
		private void AddDebugVerbs(GetVerbsEvent<Verb> args)
		{
			ActorComponent actor;
			if (!this.EntityManager.TryGetComponent<ActorComponent>(args.User, ref actor))
			{
				return;
			}
			IPlayerSession player = actor.PlayerSession;
			if (this._groupController.CanCommand(player, "deleteentity"))
			{
				Verb verb = new Verb
				{
					Text = Loc.GetString("delete-verb-get-data-text"),
					Category = VerbCategory.Debug,
					Icon = new SpriteSpecifier.Texture(new ResourcePath("/Textures/Interface/VerbIcons/delete_transparent.svg.192dpi.png", "/")),
					Act = delegate()
					{
						this.EntityManager.DeleteEntity(args.Target);
					},
					Impact = LogImpact.Medium,
					ConfirmationPopup = true
				};
				args.Verbs.Add(verb);
			}
			if (this._groupController.CanCommand(player, "rejuvenate"))
			{
				Verb verb2 = new Verb
				{
					Text = Loc.GetString("rejuvenate-verb-get-data-text"),
					Category = VerbCategory.Debug,
					Icon = new SpriteSpecifier.Texture(new ResourcePath("/Textures/Interface/VerbIcons/rejuvenate.svg.192dpi.png", "/")),
					Act = delegate()
					{
						RejuvenateCommand.PerformRejuvenate(args.Target);
					},
					Impact = LogImpact.Medium
				};
				args.Verbs.Add(verb2);
			}
			if (this._groupController.CanCommand(player, "controlmob") && args.User != args.Target)
			{
				Verb verb3 = new Verb
				{
					Text = Loc.GetString("control-mob-verb-get-data-text"),
					Category = VerbCategory.Debug,
					Act = delegate()
					{
						MakeSentientCommand.MakeSentient(args.Target, this.EntityManager, true, true);
						PlayerData playerData = player.ContentData();
						if (playerData == null)
						{
							return;
						}
						Mind mind = playerData.Mind;
						if (mind == null)
						{
							return;
						}
						mind.TransferTo(new EntityUid?(args.Target), true, false);
					},
					Impact = LogImpact.High,
					ConfirmationPopup = true
				};
				args.Verbs.Add(verb3);
			}
			ArtifactComponent artifact;
			if (this._adminManager.IsAdmin(player, false) && base.TryComp<ArtifactComponent>(args.Target, ref artifact))
			{
				args.Verbs.Add(new Verb
				{
					Text = Loc.GetString("artifact-verb-make-always-active"),
					Category = VerbCategory.Debug,
					Act = delegate()
					{
						this.EntityManager.AddComponent<ArtifactTimerTriggerComponent>(args.Target);
					},
					Disabled = this.EntityManager.HasComponent<ArtifactTimerTriggerComponent>(args.Target),
					Impact = LogImpact.High
				});
				args.Verbs.Add(new Verb
				{
					Text = Loc.GetString("artifact-verb-activate"),
					Category = VerbCategory.Debug,
					Act = delegate()
					{
						ArtifactSystem artifactSystem = this._artifactSystem;
						EntityUid target = args.Target;
						ArtifactComponent artifact = artifact;
						artifactSystem.ForceActivateArtifact(target, null, artifact);
					},
					Impact = LogImpact.High
				});
			}
			if (this._groupController.CanCommand(player, "makesentient") && args.User != args.Target && !this.EntityManager.HasComponent<MindComponent>(args.Target))
			{
				Verb verb4 = new Verb
				{
					Text = Loc.GetString("make-sentient-verb-get-data-text"),
					Category = VerbCategory.Debug,
					Icon = new SpriteSpecifier.Texture(new ResourcePath("/Textures/Interface/VerbIcons/sentient.svg.192dpi.png", "/")),
					Act = delegate()
					{
						MakeSentientCommand.MakeSentient(args.Target, this.EntityManager, true, true);
					},
					Impact = LogImpact.Medium
				};
				args.Verbs.Add(verb4);
			}
			if (this._groupController.CanCommand(player, "setoutfit") && this.EntityManager.HasComponent<InventoryComponent>(args.Target))
			{
				Verb verb5 = new Verb
				{
					Text = Loc.GetString("set-outfit-verb-get-data-text"),
					Category = VerbCategory.Debug,
					Icon = new SpriteSpecifier.Texture(new ResourcePath("/Textures/Interface/VerbIcons/outfit.svg.192dpi.png", "/")),
					Act = delegate()
					{
						this._euiManager.OpenEui(new SetOutfitEui(args.Target), player);
					},
					Impact = LogImpact.Medium
				};
				args.Verbs.Add(verb5);
			}
			if (this._groupController.CanCommand(player, "inrangeunoccluded"))
			{
				Verb verb6 = new Verb
				{
					Text = Loc.GetString("in-range-unoccluded-verb-get-data-text"),
					Category = VerbCategory.Debug,
					Icon = new SpriteSpecifier.Texture(new ResourcePath("/Textures/Interface/VerbIcons/information.svg.192dpi.png", "/")),
					Act = delegate()
					{
						string message = args.User.InRangeUnOccluded(args.Target, 16f, null, true) ? Loc.GetString("in-range-unoccluded-verb-on-activate-not-occluded") : Loc.GetString("in-range-unoccluded-verb-on-activate-occluded");
						args.Target.PopupMessage(args.User, message);
					}
				};
				args.Verbs.Add(verb6);
			}
			IDisposalTubeComponent tube;
			if (this._groupController.CanCommand(player, "tubeconnections") && this.EntityManager.TryGetComponent<IDisposalTubeComponent>(args.Target, ref tube))
			{
				Verb verb7 = new Verb
				{
					Text = Loc.GetString("tube-direction-verb-get-data-text"),
					Category = VerbCategory.Debug,
					Icon = new SpriteSpecifier.Texture(new ResourcePath("/Textures/Interface/VerbIcons/information.svg.192dpi.png", "/")),
					Act = delegate()
					{
						tube.PopupDirections(args.User);
					}
				};
				args.Verbs.Add(verb7);
			}
			if (this._groupController.CanCommand(player, "makeghostrole"))
			{
				MindComponent componentOrNull = EntityManagerExt.GetComponentOrNull<MindComponent>(this.EntityManager, args.Target);
				if (componentOrNull == null || !componentOrNull.HasMind)
				{
					Verb verb8 = new Verb();
					verb8.Text = Loc.GetString("make-ghost-role-verb-get-data-text");
					verb8.Category = VerbCategory.Debug;
					verb8.Act = delegate()
					{
						this._ghostRoleSystem.OpenMakeGhostRoleEui(player, args.Target);
					};
					verb8.Impact = LogImpact.Medium;
					args.Verbs.Add(verb8);
				}
			}
			ConfigurationComponent config;
			if (this._groupController.CanAdminMenu(player) && this.EntityManager.TryGetComponent<ConfigurationComponent>(args.Target, ref config))
			{
				Verb verb9 = new Verb
				{
					Text = Loc.GetString("configure-verb-get-data-text"),
					Icon = new SpriteSpecifier.Texture(new ResourcePath("/Textures/Interface/VerbIcons/settings.svg.192dpi.png", "/")),
					Category = VerbCategory.Debug,
					Act = delegate()
					{
						this._uiSystem.TryOpen(args.Target, ConfigurationComponent.ConfigurationUiKey.Key, actor.PlayerSession, null);
					}
				};
				args.Verbs.Add(verb9);
			}
			if (this._groupController.CanCommand(player, "addreagent") && this.EntityManager.HasComponent<SolutionContainerManagerComponent>(args.Target))
			{
				Verb verb10 = new Verb
				{
					Text = Loc.GetString("edit-solutions-verb-get-data-text"),
					Category = VerbCategory.Debug,
					Icon = new SpriteSpecifier.Texture(new ResourcePath("/Textures/Interface/VerbIcons/spill.svg.192dpi.png", "/")),
					Act = delegate()
					{
						this.OpenEditSolutionsEui(player, args.Target);
					},
					Impact = LogImpact.Medium
				};
				args.Verbs.Add(verb10);
			}
		}

		// Token: 0x06002CC4 RID: 11460 RVA: 0x000E9FA0 File Offset: 0x000E81A0
		private void OnSolutionChanged(EntityUid uid, SolutionContainerManagerComponent component, SolutionChangedEvent args)
		{
			foreach (EditSolutionsEui eui in this._openSolutionUis.Values)
			{
				if (eui.Target == uid)
				{
					eui.StateDirty();
				}
			}
		}

		// Token: 0x06002CC5 RID: 11461 RVA: 0x000EA008 File Offset: 0x000E8208
		public void OpenEditSolutionsEui(IPlayerSession session, EntityUid uid)
		{
			if (session.AttachedEntity == null)
			{
				return;
			}
			if (this._openSolutionUis.ContainsKey(session))
			{
				this._openSolutionUis[session].Close();
			}
			EditSolutionsEui eui = this._openSolutionUis[session] = new EditSolutionsEui(uid);
			this._euiManager.OpenEui(eui, session);
			eui.StateDirty();
		}

		// Token: 0x06002CC6 RID: 11462 RVA: 0x000EA070 File Offset: 0x000E8270
		public void OnEditSolutionsEuiClosed(IPlayerSession session)
		{
			EditSolutionsEui eui;
			this._openSolutionUis.Remove(session, out eui);
		}

		// Token: 0x06002CC7 RID: 11463 RVA: 0x000EA08C File Offset: 0x000E828C
		private void Reset(RoundRestartCleanupEvent ev)
		{
			this._openSolutionUis.Clear();
		}

		// Token: 0x06002CC8 RID: 11464 RVA: 0x000EA09C File Offset: 0x000E829C
		private void AddSmiteVerbs(GetVerbsEvent<Verb> args)
		{
			ActorComponent actor;
			if (!this.EntityManager.TryGetComponent<ActorComponent>(args.User, ref actor))
			{
				return;
			}
			IPlayerSession player = actor.PlayerSession;
			if (!this._adminManager.HasAdminFlag(player, AdminFlags.Fun))
			{
				return;
			}
			if (base.HasComp<MapComponent>(args.Target) || base.HasComp<MapGridComponent>(args.Target))
			{
				return;
			}
			Verb explode = new Verb
			{
				Text = "Explode",
				Category = VerbCategory.Smite,
				Icon = new SpriteSpecifier.Texture(new ResourcePath("/Textures/Interface/VerbIcons/smite.svg.192dpi.png", "/")),
				Act = delegate()
				{
					MapCoordinates coords = this.Transform(args.Target).MapPosition;
					Timer.Spawn(this._gameTiming.TickPeriod, delegate()
					{
						this._explosionSystem.QueueExplosion(coords, "Default", 4f, 1f, 2f, 1f, 0, true, false, false);
					}, CancellationToken.None);
					this._bodySystem.GibBody(new EntityUid?(args.Target), false, null, false);
				},
				Impact = LogImpact.Extreme,
				Message = Loc.GetString("admin-smite-explode-description")
			};
			args.Verbs.Add(explode);
			Verb chess = new Verb
			{
				Text = "Chess Dimension",
				Category = VerbCategory.Smite,
				Icon = new SpriteSpecifier.Rsi(new ResourcePath("/Textures/Objects/Fun/Tabletop/chessboard.rsi", "/"), "chessboard"),
				Act = delegate()
				{
					this._godmodeSystem.EnableGodmode(args.Target);
					this.EnsureComp<TabletopDraggableComponent>(args.Target);
					this.RemComp<PhysicsComponent>(args.Target);
					TransformComponent xform = this.Transform(args.Target);
					this._popupSystem.PopupEntity(Loc.GetString("admin-smite-chess-self"), args.Target, args.Target, PopupType.LargeCaution);
					this._popupSystem.PopupCoordinates(Loc.GetString("admin-smite-chess-others", new ValueTuple<string, object>[]
					{
						new ValueTuple<string, object>("name", args.Target)
					}), xform.Coordinates, Filter.PvsExcept(args.Target, 2f, null), true, PopupType.MediumCaution);
					EntityUid board = this.Spawn("ChessBoard", xform.Coordinates);
					TabletopSession session = this._tabletopSystem.EnsureSession(this.Comp<TabletopGameComponent>(board));
					xform.Coordinates = EntityCoordinates.FromMap(this._mapManager, session.Position);
					xform.WorldRotation = Angle.Zero;
				},
				Impact = LogImpact.Extreme,
				Message = Loc.GetString("admin-smite-chess-dimension-description")
			};
			args.Verbs.Add(chess);
			FlammableComponent flammable;
			if (base.TryComp<FlammableComponent>(args.Target, ref flammable))
			{
				Verb flames = new Verb
				{
					Text = "Set Alight",
					Category = VerbCategory.Smite,
					Icon = new SpriteSpecifier.Texture(new ResourcePath("/Textures/Interface/Alerts/Fire/fire.png", "/")),
					Act = delegate()
					{
						flammable.FireStacks = 20f;
						this._flammableSystem.Ignite(args.Target, null);
						TransformComponent xform = this.Transform(args.Target);
						this._popupSystem.PopupEntity(Loc.GetString("admin-smite-set-alight-self"), args.Target, args.Target, PopupType.LargeCaution);
						this._popupSystem.PopupCoordinates(Loc.GetString("admin-smite-set-alight-others", new ValueTuple<string, object>[]
						{
							new ValueTuple<string, object>("name", args.Target)
						}), xform.Coordinates, Filter.PvsExcept(args.Target, 2f, null), true, PopupType.MediumCaution);
					},
					Impact = LogImpact.Extreme,
					Message = Loc.GetString("admin-smite-set-alight-description")
				};
				args.Verbs.Add(flames);
			}
			Verb monkey = new Verb
			{
				Text = "Monkeyify",
				Category = VerbCategory.Smite,
				Icon = new SpriteSpecifier.Rsi(new ResourcePath("/Textures/Mobs/Animals/monkey.rsi", "/"), "dead"),
				Act = delegate()
				{
					this._polymorphableSystem.PolymorphEntity(args.Target, "AdminMonkeySmite");
				},
				Impact = LogImpact.Extreme,
				Message = Loc.GetString("admin-smite-monkeyify-description")
			};
			args.Verbs.Add(monkey);
			Verb disposalBin = new Verb
			{
				Text = "Garbage Can",
				Category = VerbCategory.Smite,
				Icon = new SpriteSpecifier.Rsi(new ResourcePath("/Textures/Structures/Piping/disposal.rsi", "/"), "disposal"),
				Act = delegate()
				{
					this._polymorphableSystem.PolymorphEntity(args.Target, "AdminDisposalsSmite");
				},
				Impact = LogImpact.Extreme,
				Message = Loc.GetString("admin-smite-garbage-can-description")
			};
			args.Verbs.Add(disposalBin);
			DiseaseCarrierComponent carrier;
			if (base.TryComp<DiseaseCarrierComponent>(args.Target, ref carrier))
			{
				Verb lungCancer = new Verb
				{
					Text = "Lung Cancer",
					Category = VerbCategory.Smite,
					Icon = new SpriteSpecifier.Rsi(new ResourcePath("/Textures/Mobs/Species/Human/organs.rsi", "/"), "lung-l"),
					Act = delegate()
					{
						this._diseaseSystem.TryInfect(carrier, this._prototypeManager.Index<DiseasePrototype>("StageIIIALungCancer"), 1f, true);
					},
					Impact = LogImpact.Extreme,
					Message = Loc.GetString("admin-smite-lung-cancer-description")
				};
				args.Verbs.Add(lungCancer);
			}
			DamageableComponent damageable;
			if (base.TryComp<DamageableComponent>(args.Target, ref damageable) && base.HasComp<MobStateComponent>(args.Target))
			{
				Verb hardElectrocute = new Verb
				{
					Text = "Electrocute",
					Category = VerbCategory.Smite,
					Icon = new SpriteSpecifier.Rsi(new ResourcePath("/Textures/Clothing/Hands/Gloves/Color/yellow.rsi", "/"), "icon"),
					Act = delegate()
					{
						FixedPoint2? criticalThreshold;
						int damageToDeal;
						if (!this._mobThresholdSystem.TryGetThresholdForState(args.Target, MobState.Critical, out criticalThreshold, null))
						{
							FixedPoint2? deadThreshold;
							if (!this._mobThresholdSystem.TryGetThresholdForState(args.Target, MobState.Dead, out deadThreshold, null))
							{
								return;
							}
							damageToDeal = deadThreshold.Value.Int() - (int)damageable.TotalDamage;
						}
						else
						{
							damageToDeal = criticalThreshold.Value.Int() - (int)damageable.TotalDamage;
						}
						if (damageToDeal <= 0)
						{
							damageToDeal = 100;
						}
						SlotDefinition[] slotDefinitions;
						if (this._inventorySystem.TryGetSlots(args.Target, out slotDefinitions, null))
						{
							foreach (SlotDefinition slot in slotDefinitions)
							{
								EntityUid? slotEnt;
								if (this._inventorySystem.TryGetSlotEntity(args.Target, slot.Name, out slotEnt, null, null))
								{
									this.RemComp<InsulatedComponent>(slotEnt.Value);
								}
							}
						}
						this._electrocutionSystem.TryDoElectrocution(args.Target, null, damageToDeal, TimeSpan.FromSeconds(30.0), true, 1f, null, true);
					},
					Impact = LogImpact.Extreme,
					Message = Loc.GetString("admin-smite-electrocute-description")
				};
				args.Verbs.Add(hardElectrocute);
			}
			CreamPiedComponent creamPied;
			if (base.TryComp<CreamPiedComponent>(args.Target, ref creamPied))
			{
				Verb creamPie = new Verb
				{
					Text = "Creampie",
					Category = VerbCategory.Smite,
					Icon = new SpriteSpecifier.Rsi(new ResourcePath("/Textures/Objects/Consumable/Food/Baked/pie.rsi", "/"), "plain-slice"),
					Act = delegate()
					{
						this._creamPieSystem.SetCreamPied(args.Target, creamPied, true);
					},
					Impact = LogImpact.Extreme,
					Message = Loc.GetString("admin-smite-creampie-description")
				};
				args.Verbs.Add(creamPie);
			}
			BloodstreamComponent bloodstream;
			if (base.TryComp<BloodstreamComponent>(args.Target, ref bloodstream))
			{
				Verb bloodRemoval = new Verb
				{
					Text = "Remove blood",
					Category = VerbCategory.Smite,
					Icon = new SpriteSpecifier.Rsi(new ResourcePath("/Textures/Fluids/tomato_splat.rsi", "/"), "puddle-1"),
					Act = delegate()
					{
						this._bloodstreamSystem.SpillAllSolutions(args.Target, bloodstream);
						TransformComponent xform = this.Transform(args.Target);
						this._popupSystem.PopupEntity(Loc.GetString("admin-smite-remove-blood-self"), args.Target, args.Target, PopupType.LargeCaution);
						this._popupSystem.PopupCoordinates(Loc.GetString("admin-smite-remove-blood-others", new ValueTuple<string, object>[]
						{
							new ValueTuple<string, object>("name", args.Target)
						}), xform.Coordinates, Filter.PvsExcept(args.Target, 2f, null), true, PopupType.MediumCaution);
					},
					Impact = LogImpact.Extreme,
					Message = Loc.GetString("admin-smite-remove-blood-description")
				};
				args.Verbs.Add(bloodRemoval);
			}
			BodyComponent body;
			if (base.TryComp<BodyComponent>(args.Target, ref body))
			{
				Verb vomitOrgans = new Verb
				{
					Text = "Vomit organs",
					Category = VerbCategory.Smite,
					Icon = new SpriteSpecifier.Rsi(new ResourcePath("/Textures/Fluids/vomit_toxin.rsi", "/"), "vomit_toxin-1"),
					Act = delegate()
					{
						this._vomitSystem.Vomit(args.Target, -1000f, -1000f);
						List<ValueTuple<TransformComponent, OrganComponent>> bodyOrganComponents = this._bodySystem.GetBodyOrganComponents<TransformComponent>(args.Target, body);
						TransformComponent baseXform = this.Transform(args.Target);
						foreach (ValueTuple<TransformComponent, OrganComponent> valueTuple in bodyOrganComponents)
						{
							TransformComponent xform = valueTuple.Item1;
							OrganComponent organ = valueTuple.Item2;
							if (!this.HasComp<BrainComponent>(xform.Owner) && !this.HasComp<EyeComponent>(xform.Owner))
							{
								EntityCoordinates coordinates = baseXform.Coordinates.Offset(this._random.NextVector2(0.5f, 0.75f));
								this._bodySystem.DropOrganAt(new EntityUid?(organ.Owner), coordinates, organ);
							}
						}
						this._popupSystem.PopupEntity(Loc.GetString("admin-smite-vomit-organs-self"), args.Target, args.Target, PopupType.LargeCaution);
						this._popupSystem.PopupCoordinates(Loc.GetString("admin-smite-vomit-organs-others", new ValueTuple<string, object>[]
						{
							new ValueTuple<string, object>("name", args.Target)
						}), baseXform.Coordinates, Filter.PvsExcept(args.Target, 2f, null), true, PopupType.MediumCaution);
					},
					Impact = LogImpact.Extreme,
					Message = Loc.GetString("admin-smite-vomit-organs-description")
				};
				args.Verbs.Add(vomitOrgans);
				Verb handsRemoval = new Verb
				{
					Text = "Remove hands",
					Category = VerbCategory.Smite,
					Icon = new SpriteSpecifier.Texture(new ResourcePath("/Textures/Interface/AdminActions/remove-hands.png", "/")),
					Act = delegate()
					{
						TransformComponent baseXform = this.Transform(args.Target);
						foreach (ValueTuple<EntityUid, BodyPartComponent> part in this._bodySystem.GetBodyChildrenOfType(new EntityUid?(args.Target), BodyPartType.Hand, null))
						{
							this._bodySystem.DropPartAt(new EntityUid?(part.Item1), baseXform.Coordinates, part.Item2);
						}
						this._popupSystem.PopupEntity(Loc.GetString("admin-smite-remove-hands-self"), args.Target, args.Target, PopupType.LargeCaution);
						this._popupSystem.PopupCoordinates(Loc.GetString("admin-smite-remove-hands-others", new ValueTuple<string, object>[]
						{
							new ValueTuple<string, object>("name", args.Target)
						}), baseXform.Coordinates, Filter.PvsExcept(args.Target, 2f, null), true, PopupType.Medium);
					},
					Impact = LogImpact.Extreme,
					Message = Loc.GetString("admin-smite-remove-hands-description")
				};
				args.Verbs.Add(handsRemoval);
				Verb handRemoval = new Verb
				{
					Text = "Remove hands",
					Category = VerbCategory.Smite,
					Icon = new SpriteSpecifier.Texture(new ResourcePath("/Textures/Interface/AdminActions/remove-hand.png", "/")),
					Act = delegate()
					{
						TransformComponent baseXform = this.Transform(args.Target);
						using (IEnumerator<ValueTuple<EntityUid, BodyPartComponent>> enumerator = this._bodySystem.GetBodyChildrenOfType(new EntityUid?(body.Owner), BodyPartType.Hand, body).GetEnumerator())
						{
							if (enumerator.MoveNext())
							{
								ValueTuple<EntityUid, BodyPartComponent> part = enumerator.Current;
								this._bodySystem.DropPartAt(new EntityUid?(part.Item1), baseXform.Coordinates, part.Item2);
							}
						}
						this._popupSystem.PopupEntity(Loc.GetString("admin-smite-remove-hands-self"), args.Target, args.Target, PopupType.LargeCaution);
						this._popupSystem.PopupCoordinates(Loc.GetString("admin-smite-remove-hands-others", new ValueTuple<string, object>[]
						{
							new ValueTuple<string, object>("name", args.Target)
						}), baseXform.Coordinates, Filter.PvsExcept(args.Target, 2f, null), true, PopupType.Medium);
					},
					Impact = LogImpact.Extreme,
					Message = Loc.GetString("admin-smite-remove-hand-description")
				};
				args.Verbs.Add(handRemoval);
				Verb stomachRemoval = new Verb
				{
					Text = "Stomach Removal",
					Category = VerbCategory.Smite,
					Icon = new SpriteSpecifier.Rsi(new ResourcePath("/Textures/Mobs/Species/Human/organs.rsi", "/"), "stomach"),
					Act = delegate()
					{
						foreach (ValueTuple<StomachComponent, OrganComponent> valueTuple in this._bodySystem.GetBodyOrganComponents<StomachComponent>(args.Target, body))
						{
							StomachComponent component = valueTuple.Item1;
							this.QueueDel(component.Owner);
						}
						this._popupSystem.PopupEntity(Loc.GetString("admin-smite-stomach-removal-self"), args.Target, args.Target, PopupType.LargeCaution);
					},
					Impact = LogImpact.Extreme,
					Message = Loc.GetString("admin-smite-stomach-removal-description")
				};
				args.Verbs.Add(stomachRemoval);
				Verb lungRemoval = new Verb
				{
					Text = "Lungs Removal",
					Category = VerbCategory.Smite,
					Icon = new SpriteSpecifier.Rsi(new ResourcePath("/Textures/Mobs/Species/Human/organs.rsi", "/"), "lung-r"),
					Act = delegate()
					{
						foreach (ValueTuple<LungComponent, OrganComponent> valueTuple in this._bodySystem.GetBodyOrganComponents<LungComponent>(args.Target, body))
						{
							LungComponent component = valueTuple.Item1;
							this.QueueDel(component.Owner);
						}
						this._popupSystem.PopupEntity(Loc.GetString("admin-smite-lung-removal-self"), args.Target, args.Target, PopupType.LargeCaution);
					},
					Impact = LogImpact.Extreme,
					Message = Loc.GetString("admin-smite-lung-removal-description")
				};
				args.Verbs.Add(lungRemoval);
			}
			PhysicsComponent physics;
			if (base.TryComp<PhysicsComponent>(args.Target, ref physics))
			{
				Verb pinball = new Verb
				{
					Text = "Pinball",
					Category = VerbCategory.Smite,
					Icon = new SpriteSpecifier.Rsi(new ResourcePath("/Textures/Objects/Fun/toys.rsi", "/"), "basketball"),
					Act = delegate()
					{
						TransformComponent transformComponent = this.Transform(args.Target);
						FixturesComponent fixtures = this.Comp<FixturesComponent>(args.Target);
						transformComponent.Anchored = false;
						this._physics.SetBodyType(args.Target, 8, fixtures, physics, null);
						this._physics.SetBodyStatus(physics, 1, true);
						this._physics.WakeBody(args.Target, false, fixtures, physics);
						foreach (Fixture fixture in fixtures.Fixtures.Values)
						{
							if (fixture.Hard)
							{
								this._physics.SetRestitution(args.Target, fixture, 1.1f, false, fixtures);
							}
						}
						this._fixtures.FixtureUpdate(args.Target, true, fixtures, physics);
						this._physics.SetLinearVelocity(args.Target, this._random.NextVector2(1.5f, 1.5f), true, true, fixtures, physics);
						this._physics.SetAngularVelocity(args.Target, 37.699112f, true, fixtures, physics);
						this._physics.SetLinearDamping(physics, 0f, true);
						this._physics.SetAngularDamping(physics, 0f, true);
					},
					Impact = LogImpact.Extreme,
					Message = Loc.GetString("admin-smite-pinball-description")
				};
				args.Verbs.Add(pinball);
				Verb yeet = new Verb
				{
					Text = "Yeet",
					Category = VerbCategory.Smite,
					Icon = new SpriteSpecifier.Texture(new ResourcePath("/Textures/Interface/VerbIcons/eject.svg.192dpi.png", "/")),
					Act = delegate()
					{
						TransformComponent transformComponent = this.Transform(args.Target);
						FixturesComponent fixtures = this.Comp<FixturesComponent>(args.Target);
						transformComponent.Anchored = false;
						this._physics.SetBodyType(args.Target, 8, null, physics, null);
						this._physics.SetBodyStatus(physics, 1, true);
						this._physics.WakeBody(args.Target, false, fixtures, physics);
						foreach (Fixture fixture in fixtures.Fixtures.Values)
						{
							this._physics.SetHard(args.Target, fixture, false, fixtures);
						}
						this._physics.SetLinearVelocity(args.Target, this._random.NextVector2(8f, 8f), true, true, fixtures, physics);
						this._physics.SetAngularVelocity(args.Target, 37.699112f, true, fixtures, physics);
						this._physics.SetLinearDamping(physics, 0f, true);
						this._physics.SetAngularDamping(physics, 0f, true);
					},
					Impact = LogImpact.Extreme,
					Message = Loc.GetString("admin-smite-yeet-description")
				};
				args.Verbs.Add(yeet);
			}
			Verb bread = new Verb
			{
				Text = "Become Bread",
				Category = VerbCategory.Smite,
				Icon = new SpriteSpecifier.Rsi(new ResourcePath("/Textures/Objects/Consumable/Food/Baked/bread.rsi", "/"), "plain"),
				Act = delegate()
				{
					this._polymorphableSystem.PolymorphEntity(args.Target, "AdminBreadSmite");
				},
				Impact = LogImpact.Extreme,
				Message = Loc.GetString("admin-smite-become-bread-description")
			};
			args.Verbs.Add(bread);
			Verb mouse = new Verb
			{
				Text = "Become Mouse",
				Category = VerbCategory.Smite,
				Icon = new SpriteSpecifier.Rsi(new ResourcePath("/Textures/Mobs/Animals/mouse.rsi", "/"), "icon-0"),
				Act = delegate()
				{
					this._polymorphableSystem.PolymorphEntity(args.Target, "AdminMouseSmite");
				},
				Impact = LogImpact.Extreme,
				Message = Loc.GetString("admin-smite-become-mouse-description")
			};
			args.Verbs.Add(mouse);
			ActorComponent actorComponent;
			if (base.TryComp<ActorComponent>(args.Target, ref actorComponent))
			{
				Verb ghostKick = new Verb
				{
					Text = "Ghostkick",
					Category = VerbCategory.Smite,
					Icon = new SpriteSpecifier.Texture(new ResourcePath("/Textures/Interface/gavel.svg.192dpi.png", "/")),
					Act = delegate()
					{
						this._ghostKickManager.DoDisconnect(actorComponent.PlayerSession.ConnectedClient, "Smitten.");
					},
					Impact = LogImpact.Extreme,
					Message = Loc.GetString("admin-smite-ghostkick-description")
				};
				args.Verbs.Add(ghostKick);
			}
			InventoryComponent inventory;
			if (base.TryComp<InventoryComponent>(args.Target, ref inventory))
			{
				Verb nyanify = new Verb
				{
					Text = "Nyanify",
					Category = VerbCategory.Smite,
					Icon = new SpriteSpecifier.Rsi(new ResourcePath("/Textures/Clothing/Head/Hats/catears.rsi", "/"), "icon"),
					Act = delegate()
					{
						EntityUid ears = this.Spawn("ClothingHeadHatCatEars", this.Transform(args.Target).Coordinates);
						this.EnsureComp<UnremoveableComponent>(ears);
						this._inventorySystem.TryUnequip(args.Target, "head", true, true, false, inventory, null);
						this._inventorySystem.TryEquip(args.Target, ears, "head", true, true, false, inventory, null);
					},
					Impact = LogImpact.Extreme,
					Message = Loc.GetString("admin-smite-nyanify-description")
				};
				args.Verbs.Add(nyanify);
				Verb killSign = new Verb
				{
					Text = "Kill sign",
					Category = VerbCategory.Smite,
					Icon = new SpriteSpecifier.Rsi(new ResourcePath("/Textures/Objects/Misc/killsign.rsi", "/"), "icon"),
					Act = delegate()
					{
						this.EnsureComp<KillSignComponent>(args.Target);
					},
					Impact = LogImpact.Extreme,
					Message = Loc.GetString("admin-smite-kill-sign-description")
				};
				args.Verbs.Add(killSign);
				Action<EntityUid, EntityUid> <>9__23;
				Verb clown = new Verb
				{
					Text = "Clown",
					Category = VerbCategory.Smite,
					Icon = new SpriteSpecifier.Rsi(new ResourcePath("/Textures/Objects/Fun/bikehorn.rsi", "/"), "icon"),
					Act = delegate()
					{
						EntityUid target = args.Target;
						string gear = "ClownGear";
						IEntityManager entityManager = this.EntityManager;
						Action<EntityUid, EntityUid> onEquipped;
						if ((onEquipped = <>9__23) == null)
						{
							onEquipped = (<>9__23 = delegate(EntityUid _, EntityUid clothing)
							{
								if (this.HasComp<ClothingComponent>(clothing))
								{
									this.EnsureComp<UnremoveableComponent>(clothing);
								}
								this.EnsureComp<ClumsyComponent>(args.Target);
							});
						}
						SetOutfitCommand.SetOutfit(target, gear, entityManager, onEquipped);
					},
					Impact = LogImpact.Extreme,
					Message = Loc.GetString("admin-smite-clown-description")
				};
				args.Verbs.Add(clown);
				Action<EntityUid, EntityUid> <>9__25;
				Verb maiden = new Verb
				{
					Text = "Maid",
					Category = VerbCategory.Smite,
					Icon = new SpriteSpecifier.Rsi(new ResourcePath("/Textures/Clothing/Uniforms/Jumpskirt/janimaid.rsi", "/"), "icon"),
					Act = delegate()
					{
						EntityUid target = args.Target;
						string gear = "JanitorMaidGear";
						IEntityManager entityManager = this.EntityManager;
						Action<EntityUid, EntityUid> onEquipped;
						if ((onEquipped = <>9__25) == null)
						{
							onEquipped = (<>9__25 = delegate(EntityUid _, EntityUid clothing)
							{
								if (this.HasComp<ClothingComponent>(clothing))
								{
									this.EnsureComp<UnremoveableComponent>(clothing);
								}
								this.EnsureComp<ClumsyComponent>(args.Target);
							});
						}
						SetOutfitCommand.SetOutfit(target, gear, entityManager, onEquipped);
					},
					Impact = LogImpact.Extreme,
					Message = Loc.GetString("admin-smite-maid-description")
				};
				args.Verbs.Add(maiden);
			}
			Verb angerPointingArrows = new Verb
			{
				Text = "Anger Pointing Arrows",
				Category = VerbCategory.Smite,
				Icon = new SpriteSpecifier.Rsi(new ResourcePath("/Textures/Interface/Misc/pointing.rsi", "/"), "pointing"),
				Act = delegate()
				{
					this.EnsureComp<PointingArrowAngeringComponent>(args.Target);
				},
				Impact = LogImpact.Extreme,
				Message = Loc.GetString("admin-smite-anger-pointing-arrows-description")
			};
			args.Verbs.Add(angerPointingArrows);
			Verb dust = new Verb
			{
				Text = "Dust",
				Category = VerbCategory.Smite,
				Icon = new SpriteSpecifier.Rsi(new ResourcePath("/Textures/Objects/Materials/materials", "/"), "ash"),
				Act = delegate()
				{
					this.EntityManager.QueueDeleteEntity(args.Target);
					this.Spawn("Ash", this.Transform(args.Target).Coordinates);
					this._popupSystem.PopupEntity(Loc.GetString("admin-smite-turned-ash-other", new ValueTuple<string, object>[]
					{
						new ValueTuple<string, object>("name", args.Target)
					}), args.Target, PopupType.LargeCaution);
				},
				Impact = LogImpact.Extreme,
				Message = Loc.GetString("admin-smite-dust-description")
			};
			args.Verbs.Add(dust);
			Verb youtubeVideoSimulation = new Verb
			{
				Text = "Buffering",
				Category = VerbCategory.Smite,
				Icon = new SpriteSpecifier.Texture(new ResourcePath("/Textures/Interface/Misc/buffering_smite_icon.png", "/")),
				Act = delegate()
				{
					this.EnsureComp<BufferingComponent>(args.Target);
				},
				Impact = LogImpact.Extreme,
				Message = Loc.GetString("admin-smite-buffering-description")
			};
			args.Verbs.Add(youtubeVideoSimulation);
			Verb instrumentation = new Verb
			{
				Text = "Become Instrument",
				Category = VerbCategory.Smite,
				Icon = new SpriteSpecifier.Rsi(new ResourcePath("/Textures/Objects/Fun/Instruments/h_synthesizer.rsi", "/"), "icon"),
				Act = delegate()
				{
					this._polymorphableSystem.PolymorphEntity(args.Target, "AdminInstrumentSmite");
				},
				Impact = LogImpact.Extreme,
				Message = Loc.GetString("admin-smite-become-instrument-description")
			};
			args.Verbs.Add(instrumentation);
			Verb noGravity = new Verb
			{
				Text = "Remove gravity",
				Category = VerbCategory.Smite,
				Icon = new SpriteSpecifier.Rsi(new ResourcePath("/Textures/Structures/Machines/gravity_generator.rsi", "/"), "off"),
				Act = delegate()
				{
					MovementIgnoreGravityComponent grav = this.EnsureComp<MovementIgnoreGravityComponent>(args.Target);
					grav.Weightless = true;
					this.Dirty(grav, null);
				},
				Impact = LogImpact.Extreme,
				Message = Loc.GetString("admin-smite-remove-gravity-description")
			};
			args.Verbs.Add(noGravity);
			Verb reptilian = new Verb
			{
				Text = "Reptilian Species Swap",
				Category = VerbCategory.Smite,
				Icon = new SpriteSpecifier.Rsi(new ResourcePath("/Textures/Objects/Fun/toys.rsi", "/"), "plushie_lizard"),
				Act = delegate()
				{
					this._polymorphableSystem.PolymorphEntity(args.Target, "AdminLizardSmite");
				},
				Impact = LogImpact.Extreme,
				Message = Loc.GetString("admin-smite-reptilian-species-swap-description")
			};
			args.Verbs.Add(reptilian);
			Verb locker = new Verb
			{
				Text = "Locker stuff",
				Category = VerbCategory.Smite,
				Icon = new SpriteSpecifier.Rsi(new ResourcePath("/Textures/Structures/Storage/closet.rsi", "/"), "generic"),
				Act = delegate()
				{
					TransformComponent xform = this.Transform(args.Target);
					EntityUid locker2 = this.Spawn("ClosetMaintenance", xform.Coordinates);
					EntityStorageComponent storage;
					if (this.TryComp<EntityStorageComponent>(locker2, ref storage))
					{
						this._entityStorageSystem.ToggleOpen(args.Target, locker2, storage);
						this._entityStorageSystem.Insert(args.Target, locker2, storage);
						this._entityStorageSystem.ToggleOpen(args.Target, locker2, storage);
					}
					this._weldableSystem.ForceWeldedState(locker2, true, null);
				},
				Impact = LogImpact.Extreme,
				Message = Loc.GetString("admin-smite-locker-stuff-description")
			};
			args.Verbs.Add(locker);
			Verb headstand = new Verb
			{
				Text = "Headstand",
				Category = VerbCategory.Smite,
				Icon = new SpriteSpecifier.Texture(new ResourcePath("/Textures/Interface/VerbIcons/refresh.svg.192dpi.png", "/")),
				Act = delegate()
				{
					this.EnsureComp<HeadstandComponent>(args.Target);
				},
				Impact = LogImpact.Extreme,
				Message = Loc.GetString("admin-smite-headstand-description")
			};
			args.Verbs.Add(headstand);
			Verb zoomIn = new Verb
			{
				Text = "Zoom in",
				Category = VerbCategory.Smite,
				Icon = new SpriteSpecifier.Texture(new ResourcePath("/Textures/Interface/AdminActions/zoom.png", "/")),
				Act = delegate()
				{
					EyeComponent eye = this.EnsureComp<EyeComponent>(args.Target);
					eye.Zoom *= Vector2.One * 0.2f;
					this.Dirty(eye, null);
				},
				Impact = LogImpact.Extreme,
				Message = Loc.GetString("admin-smite-zoom-in-description")
			};
			args.Verbs.Add(zoomIn);
			Verb flipEye = new Verb
			{
				Text = "Flip eye",
				Category = VerbCategory.Smite,
				Icon = new SpriteSpecifier.Texture(new ResourcePath("/Textures/Interface/AdminActions/flip.png", "/")),
				Act = delegate()
				{
					EyeComponent eye = this.EnsureComp<EyeComponent>(args.Target);
					eye.Zoom *= -1f;
					this.Dirty(eye, null);
				},
				Impact = LogImpact.Extreme,
				Message = Loc.GetString("admin-smite-flip-eye-description")
			};
			args.Verbs.Add(flipEye);
			Verb runWalkSwap = new Verb
			{
				Text = "Run Walk Swap",
				Category = VerbCategory.Smite,
				Icon = new SpriteSpecifier.Texture(new ResourcePath("/Textures/Interface/AdminActions/run-walk-swap.png", "/")),
				Act = delegate()
				{
					MovementSpeedModifierComponent movementSpeed = this.EnsureComp<MovementSpeedModifierComponent>(args.Target);
					MovementSpeedModifierComponent movementSpeedModifierComponent = movementSpeed;
					MovementSpeedModifierComponent movementSpeedModifierComponent2 = movementSpeed;
					float baseWalkSpeed = movementSpeed.BaseWalkSpeed;
					float baseSprintSpeed = movementSpeed.BaseSprintSpeed;
					movementSpeedModifierComponent.BaseSprintSpeed = baseWalkSpeed;
					movementSpeedModifierComponent2.BaseWalkSpeed = baseSprintSpeed;
					this.Dirty(movementSpeed, null);
					this._popupSystem.PopupEntity(Loc.GetString("admin-smite-run-walk-swap-prompt"), args.Target, args.Target, PopupType.LargeCaution);
				},
				Impact = LogImpact.Extreme,
				Message = Loc.GetString("admin-smite-run-walk-swap-description")
			};
			args.Verbs.Add(runWalkSwap);
			Verb backwardsAccent = new Verb
			{
				Text = "Speak Backwards",
				Category = VerbCategory.Smite,
				Icon = new SpriteSpecifier.Texture(new ResourcePath("/Textures/Interface/AdminActions/help-backwards.png", "/")),
				Act = delegate()
				{
					this.EnsureComp<BackwardsAccentComponent>(args.Target);
				},
				Impact = LogImpact.Extreme,
				Message = Loc.GetString("admin-smite-speak-backwards-description")
			};
			args.Verbs.Add(backwardsAccent);
			Verb disarmProne = new Verb
			{
				Text = "Disarm Prone",
				Category = VerbCategory.Smite,
				Icon = new SpriteSpecifier.Texture(new ResourcePath("/Textures/Interface/Actions/disarm.png", "/")),
				Act = delegate()
				{
					this.EnsureComp<DisarmProneComponent>(args.Target);
				},
				Impact = LogImpact.Extreme,
				Message = Loc.GetString("admin-smite-disarm-prone-description")
			};
			args.Verbs.Add(disarmProne);
			Verb superSpeed = new Verb
			{
				Text = "Super speed",
				Category = VerbCategory.Smite,
				Icon = new SpriteSpecifier.Texture(new ResourcePath("/Textures/Interface/AdminActions/super_speed.png", "/")),
				Act = delegate()
				{
					MovementSpeedModifierComponent movementSpeed = this.EnsureComp<MovementSpeedModifierComponent>(args.Target);
					MovementSpeedModifierSystem movementSpeedModifierSystem = this._movementSpeedModifierSystem;
					if (movementSpeedModifierSystem != null)
					{
						movementSpeedModifierSystem.ChangeBaseSpeed(args.Target, 400f, 8000f, 40f, movementSpeed);
					}
					this._popupSystem.PopupEntity(Loc.GetString("admin-smite-super-speed-prompt"), args.Target, args.Target, PopupType.LargeCaution);
				},
				Impact = LogImpact.Extreme,
				Message = Loc.GetString("admin-smite-super-speed-description")
			};
			args.Verbs.Add(superSpeed);
		}

		// Token: 0x06002CC9 RID: 11465 RVA: 0x000EB328 File Offset: 0x000E9528
		private void AddTricksVerbs(GetVerbsEvent<Verb> args)
		{
			ActorComponent actor;
			if (!this.EntityManager.TryGetComponent<ActorComponent>(args.User, ref actor))
			{
				return;
			}
			IPlayerSession player = actor.PlayerSession;
			if (!this._adminManager.HasAdminFlag(player, AdminFlags.Admin))
			{
				return;
			}
			if (this._adminManager.HasAdminFlag(player, AdminFlags.Admin))
			{
				AirlockComponent airlock;
				if (base.TryComp<AirlockComponent>(args.Target, ref airlock))
				{
					Verb bolt = new Verb
					{
						Text = (airlock.BoltsDown ? "Unbolt" : "Bolt"),
						Category = VerbCategory.Tricks,
						Icon = (airlock.BoltsDown ? new SpriteSpecifier.Texture(new ResourcePath("/Textures/Interface/AdminActions/unbolt.png", "/")) : new SpriteSpecifier.Texture(new ResourcePath("/Textures/Interface/AdminActions/bolt.png", "/"))),
						Act = delegate()
						{
							this._airlockSystem.SetBoltsWithAudio(args.Target, airlock, !airlock.BoltsDown);
						},
						Impact = LogImpact.Medium,
						Message = Loc.GetString(airlock.BoltsDown ? "admin-trick-unbolt-description" : "admin-trick-bolt-description"),
						Priority = (airlock.BoltsDown ? 0 : 0)
					};
					args.Verbs.Add(bolt);
					Verb emergencyAccess = new Verb
					{
						Text = (airlock.EmergencyAccess ? "Emergency Access Off" : "Emergency Access On"),
						Category = VerbCategory.Tricks,
						Icon = new SpriteSpecifier.Texture(new ResourcePath("/Textures/Interface/AdminActions/emergency_access.png", "/")),
						Act = delegate()
						{
							this._airlockSystem.ToggleEmergencyAccess(args.Target, airlock);
						},
						Impact = LogImpact.Medium,
						Message = Loc.GetString(airlock.EmergencyAccess ? "admin-trick-emergency-access-off-description" : "admin-trick-emergency-access-on-description"),
						Priority = (airlock.EmergencyAccess ? -1 : -1)
					};
					args.Verbs.Add(emergencyAccess);
				}
				if (base.HasComp<DamageableComponent>(args.Target))
				{
					Verb rejuvenate = new Verb
					{
						Text = "Rejuvenate",
						Category = VerbCategory.Tricks,
						Icon = new SpriteSpecifier.Texture(new ResourcePath("/Textures/Interface/AdminActions/rejuvenate.png", "/")),
						Act = delegate()
						{
							RejuvenateCommand.PerformRejuvenate(args.Target);
						},
						Impact = LogImpact.Extreme,
						Message = Loc.GetString("admin-trick-rejuvenate-description"),
						Priority = -12
					};
					args.Verbs.Add(rejuvenate);
				}
				if (!this._godmodeSystem.HasGodmode(args.Target))
				{
					Verb makeIndestructible = new Verb
					{
						Text = "Make Indestructible",
						Category = VerbCategory.Tricks,
						Icon = new SpriteSpecifier.Texture(new ResourcePath("/Textures/Interface/VerbIcons/plus.svg.192dpi.png", "/")),
						Act = delegate()
						{
							this._godmodeSystem.EnableGodmode(args.Target);
						},
						Impact = LogImpact.Extreme,
						Message = Loc.GetString("admin-trick-make-indestructible-description"),
						Priority = -2
					};
					args.Verbs.Add(makeIndestructible);
				}
				else
				{
					Verb makeVulnerable = new Verb
					{
						Text = "Make Vulnerable",
						Category = VerbCategory.Tricks,
						Icon = new SpriteSpecifier.Texture(new ResourcePath("/Textures/Interface/VerbIcons/plus.svg.192dpi.png", "/")),
						Act = delegate()
						{
							this._godmodeSystem.DisableGodmode(args.Target);
						},
						Impact = LogImpact.Extreme,
						Message = Loc.GetString("admin-trick-make-vulnerable-description"),
						Priority = -2
					};
					args.Verbs.Add(makeVulnerable);
				}
				BatteryComponent battery;
				if (base.TryComp<BatteryComponent>(args.Target, ref battery))
				{
					Verb refillBattery = new Verb
					{
						Text = "Refill Battery",
						Category = VerbCategory.Tricks,
						Icon = new SpriteSpecifier.Texture(new ResourcePath("/Textures/Interface/AdminActions/fill_battery.png", "/")),
						Act = delegate()
						{
							battery.CurrentCharge = battery.MaxCharge;
							this.Dirty(battery, null);
						},
						Impact = LogImpact.Medium,
						Message = Loc.GetString("admin-trick-refill-battery-description"),
						Priority = -4
					};
					args.Verbs.Add(refillBattery);
					Verb drainBattery = new Verb
					{
						Text = "Drain Battery",
						Category = VerbCategory.Tricks,
						Icon = new SpriteSpecifier.Texture(new ResourcePath("/Textures/Interface/AdminActions/drain_battery.png", "/")),
						Act = delegate()
						{
							battery.CurrentCharge = 0f;
							this.Dirty(battery, null);
						},
						Impact = LogImpact.Medium,
						Message = Loc.GetString("admin-trick-drain-battery-description"),
						Priority = -5
					};
					args.Verbs.Add(drainBattery);
					Verb infiniteBattery = new Verb
					{
						Text = "Infinite Battery",
						Category = VerbCategory.Tricks,
						Icon = new SpriteSpecifier.Texture(new ResourcePath("/Textures/Interface/AdminActions/infinite_battery.png", "/")),
						Act = delegate()
						{
							BatterySelfRechargerComponent batterySelfRechargerComponent = this.EnsureComp<BatterySelfRechargerComponent>(args.Target);
							batterySelfRechargerComponent.AutoRecharge = true;
							batterySelfRechargerComponent.AutoRechargeRate = battery.MaxCharge;
						},
						Impact = LogImpact.Medium,
						Message = Loc.GetString("admin-trick-infinite-battery-object-description"),
						Priority = -20
					};
					args.Verbs.Add(infiniteBattery);
				}
				AnchorableComponent anchor;
				if (base.TryComp<AnchorableComponent>(args.Target, ref anchor))
				{
					Verb blockUnanchor = new Verb
					{
						Text = "Block Unanchoring",
						Category = VerbCategory.Tricks,
						Icon = new SpriteSpecifier.Texture(new ResourcePath("/Textures/Interface/VerbIcons/anchor.svg.192dpi.png", "/")),
						Act = delegate()
						{
							this.RemComp(args.Target, anchor);
						},
						Impact = LogImpact.Medium,
						Message = Loc.GetString("admin-trick-block-unanchoring-description"),
						Priority = -3
					};
					args.Verbs.Add(blockUnanchor);
				}
				GasTankComponent tank;
				if (base.TryComp<GasTankComponent>(args.Target, ref tank))
				{
					Verb refillInternalsO2 = new Verb
					{
						Text = "Refill Internals Oxygen",
						Category = VerbCategory.Tricks,
						Icon = new SpriteSpecifier.Rsi(new ResourcePath("/Textures/Objects/Tanks/oxygen.rsi", "/"), "icon"),
						Act = delegate()
						{
							this.RefillGasTank(args.Target, Gas.Oxygen, tank);
						},
						Impact = LogImpact.Extreme,
						Message = Loc.GetString("admin-trick-internals-refill-oxygen-description"),
						Priority = -6
					};
					args.Verbs.Add(refillInternalsO2);
					Verb refillInternalsN2 = new Verb
					{
						Text = "Refill Internals Nitrogen",
						Category = VerbCategory.Tricks,
						Icon = new SpriteSpecifier.Rsi(new ResourcePath("/Textures/Objects/Tanks/red.rsi", "/"), "icon"),
						Act = delegate()
						{
							this.RefillGasTank(args.Target, Gas.Nitrogen, tank);
						},
						Impact = LogImpact.Extreme,
						Message = Loc.GetString("admin-trick-internals-refill-nitrogen-description"),
						Priority = -7
					};
					args.Verbs.Add(refillInternalsN2);
					Verb refillInternalsPlasma = new Verb
					{
						Text = "Refill Internals Plasma",
						Category = VerbCategory.Tricks,
						Icon = new SpriteSpecifier.Rsi(new ResourcePath("/Textures/Objects/Tanks/plasma.rsi", "/"), "icon"),
						Act = delegate()
						{
							this.RefillGasTank(args.Target, Gas.Plasma, tank);
						},
						Impact = LogImpact.Extreme,
						Message = Loc.GetString("admin-trick-internals-refill-plasma-description"),
						Priority = -8
					};
					args.Verbs.Add(refillInternalsPlasma);
				}
				if (base.HasComp<InventoryComponent>(args.Target))
				{
					Verb refillInternalsO3 = new Verb
					{
						Text = "Refill Internals Oxygen",
						Category = VerbCategory.Tricks,
						Icon = new SpriteSpecifier.Rsi(new ResourcePath("/Textures/Objects/Tanks/oxygen.rsi", "/"), "icon"),
						Act = delegate()
						{
							foreach (SlotDefinition slot in this._inventorySystem.GetSlots(args.Target, null))
							{
								EntityUid? entity;
								if (this._inventorySystem.TryGetSlotEntity(args.Target, slot.Name, out entity, null, null) && this.TryComp<GasTankComponent>(entity, ref tank))
								{
									this.RefillGasTank(entity.Value, Gas.Oxygen, tank);
								}
							}
							foreach (EntityUid held in this._handsSystem.EnumerateHeld(args.Target, null))
							{
								if (this.TryComp<GasTankComponent>(held, ref tank))
								{
									this.RefillGasTank(held, Gas.Oxygen, tank);
								}
							}
						},
						Impact = LogImpact.Extreme,
						Message = Loc.GetString("admin-trick-internals-refill-oxygen-description"),
						Priority = -6
					};
					args.Verbs.Add(refillInternalsO3);
					Verb refillInternalsN3 = new Verb
					{
						Text = "Refill Internals Nitrogen",
						Category = VerbCategory.Tricks,
						Icon = new SpriteSpecifier.Rsi(new ResourcePath("/Textures/Objects/Tanks/red.rsi", "/"), "icon"),
						Act = delegate()
						{
							foreach (SlotDefinition slot in this._inventorySystem.GetSlots(args.Target, null))
							{
								EntityUid? entity;
								if (this._inventorySystem.TryGetSlotEntity(args.Target, slot.Name, out entity, null, null) && this.TryComp<GasTankComponent>(entity, ref tank))
								{
									this.RefillGasTank(entity.Value, Gas.Nitrogen, tank);
								}
							}
							foreach (EntityUid held in this._handsSystem.EnumerateHeld(args.Target, null))
							{
								if (this.TryComp<GasTankComponent>(held, ref tank))
								{
									this.RefillGasTank(held, Gas.Nitrogen, tank);
								}
							}
						},
						Impact = LogImpact.Extreme,
						Message = Loc.GetString("admin-trick-internals-refill-nitrogen-description"),
						Priority = -7
					};
					args.Verbs.Add(refillInternalsN3);
					Verb refillInternalsPlasma2 = new Verb
					{
						Text = "Refill Internals Plasma",
						Category = VerbCategory.Tricks,
						Icon = new SpriteSpecifier.Rsi(new ResourcePath("/Textures/Objects/Tanks/plasma.rsi", "/"), "icon"),
						Act = delegate()
						{
							foreach (SlotDefinition slot in this._inventorySystem.GetSlots(args.Target, null))
							{
								EntityUid? entity;
								if (this._inventorySystem.TryGetSlotEntity(args.Target, slot.Name, out entity, null, null) && this.TryComp<GasTankComponent>(entity, ref tank))
								{
									this.RefillGasTank(entity.Value, Gas.Plasma, tank);
								}
							}
							foreach (EntityUid held in this._handsSystem.EnumerateHeld(args.Target, null))
							{
								if (this.TryComp<GasTankComponent>(held, ref tank))
								{
									this.RefillGasTank(held, Gas.Plasma, tank);
								}
							}
						},
						Impact = LogImpact.Extreme,
						Message = Loc.GetString("admin-trick-internals-refill-plasma-description"),
						Priority = -8
					};
					args.Verbs.Add(refillInternalsPlasma2);
				}
				Verb sendToTestArena = new Verb
				{
					Text = "Send to test arena",
					Category = VerbCategory.Tricks,
					Icon = new SpriteSpecifier.Texture(new ResourcePath("/Textures/Interface/VerbIcons/eject.svg.192dpi.png", "/")),
					Act = delegate()
					{
						ValueTuple<EntityUid, EntityUid?> valueTuple = this._adminTestArenaSystem.AssertArenaLoaded(player);
						EntityUid mapUid = valueTuple.Item1;
						EntityUid? gridUid = valueTuple.Item2;
						this.Transform(args.Target).Coordinates = new EntityCoordinates(gridUid ?? mapUid, Vector2.One);
					},
					Impact = LogImpact.Medium,
					Message = Loc.GetString("admin-trick-send-to-test-arena-description"),
					Priority = -9
				};
				args.Verbs.Add(sendToTestArena);
				EntityUid? activeId = this.FindActiveId(args.Target);
				if (activeId != null)
				{
					Verb grantAllAccess = new Verb
					{
						Text = "Grant All Access",
						Category = VerbCategory.Tricks,
						Icon = new SpriteSpecifier.Rsi(new ResourcePath("/Textures/Objects/Misc/id_cards.rsi", "/"), "centcom"),
						Act = delegate()
						{
							this.GiveAllAccess(activeId.Value);
						},
						Impact = LogImpact.Extreme,
						Message = Loc.GetString("admin-trick-grant-all-access-description"),
						Priority = -10
					};
					args.Verbs.Add(grantAllAccess);
					Verb revokeAllAccess = new Verb
					{
						Text = "Revoke All Access",
						Category = VerbCategory.Tricks,
						Icon = new SpriteSpecifier.Rsi(new ResourcePath("/Textures/Objects/Misc/id_cards.rsi", "/"), "default"),
						Act = delegate()
						{
							this.RevokeAllAccess(activeId.Value);
						},
						Impact = LogImpact.Extreme,
						Message = Loc.GetString("admin-trick-revoke-all-access-description"),
						Priority = -11
					};
					args.Verbs.Add(revokeAllAccess);
				}
				if (base.HasComp<AccessComponent>(args.Target))
				{
					Verb grantAllAccess2 = new Verb
					{
						Text = "Grant All Access",
						Category = VerbCategory.Tricks,
						Icon = new SpriteSpecifier.Rsi(new ResourcePath("/Textures/Objects/Misc/id_cards.rsi", "/"), "centcom"),
						Act = delegate()
						{
							this.GiveAllAccess(args.Target);
						},
						Impact = LogImpact.Extreme,
						Message = Loc.GetString("admin-trick-grant-all-access-description"),
						Priority = -10
					};
					args.Verbs.Add(grantAllAccess2);
					Verb revokeAllAccess2 = new Verb
					{
						Text = "Revoke All Access",
						Category = VerbCategory.Tricks,
						Icon = new SpriteSpecifier.Rsi(new ResourcePath("/Textures/Objects/Misc/id_cards.rsi", "/"), "default"),
						Act = delegate()
						{
							this.RevokeAllAccess(args.Target);
						},
						Impact = LogImpact.Extreme,
						Message = Loc.GetString("admin-trick-revoke-all-access-description"),
						Priority = -11
					};
					args.Verbs.Add(revokeAllAccess2);
				}
			}
			StackComponent stack;
			if (base.TryComp<StackComponent>(args.Target, ref stack))
			{
				Action<int> <>9__21;
				Verb adjustStack = new Verb
				{
					Text = "Adjust Stack",
					Category = VerbCategory.Tricks,
					Icon = new SpriteSpecifier.Texture(new ResourcePath("/Textures/Interface/AdminActions/adjust-stack.png", "/")),
					Act = delegate()
					{
						QuickDialogSystem quickDialog = this._quickDialog;
						IPlayerSession player = player;
						string title = "Adjust stack";
						DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(13, 1);
						defaultInterpolatedStringHandler.AppendLiteral("Amount (max ");
						defaultInterpolatedStringHandler.AppendFormatted<int>(this._stackSystem.GetMaxCount(stack));
						defaultInterpolatedStringHandler.AppendLiteral(")");
						string prompt = defaultInterpolatedStringHandler.ToStringAndClear();
						Action<int> okAction;
						if ((okAction = <>9__21) == null)
						{
							okAction = (<>9__21 = delegate(int newAmount)
							{
								this._stackSystem.SetCount(args.Target, newAmount, stack);
							});
						}
						quickDialog.OpenDialog<int>(player, title, prompt, okAction, null);
					},
					Impact = LogImpact.Medium,
					Message = Loc.GetString("admin-trick-adjust-stack-description"),
					Priority = -13
				};
				args.Verbs.Add(adjustStack);
				Verb fillStack = new Verb
				{
					Text = "Fill Stack",
					Category = VerbCategory.Tricks,
					Icon = new SpriteSpecifier.Texture(new ResourcePath("/Textures/Interface/AdminActions/fill-stack.png", "/")),
					Act = delegate()
					{
						this._stackSystem.SetCount(args.Target, this._stackSystem.GetMaxCount(stack), stack);
					},
					Impact = LogImpact.Medium,
					Message = Loc.GetString("admin-trick-fill-stack-description"),
					Priority = -14
				};
				args.Verbs.Add(fillStack);
			}
			Action<string> <>9__24;
			Verb rename = new Verb
			{
				Text = "Rename",
				Category = VerbCategory.Tricks,
				Icon = new SpriteSpecifier.Texture(new ResourcePath("/Textures/Interface/AdminActions/rename.png", "/")),
				Act = delegate()
				{
					QuickDialogSystem quickDialog = this._quickDialog;
					IPlayerSession player = player;
					string title = "Rename";
					string prompt = "Name";
					Action<string> okAction;
					if ((okAction = <>9__24) == null)
					{
						okAction = (<>9__24 = delegate(string newName)
						{
							this.MetaData(args.Target).EntityName = newName;
						});
					}
					quickDialog.OpenDialog<string>(player, title, prompt, okAction, null);
				},
				Impact = LogImpact.Medium,
				Message = Loc.GetString("admin-trick-rename-description"),
				Priority = -15
			};
			args.Verbs.Add(rename);
			Action<LongString> <>9__26;
			Verb redescribe = new Verb
			{
				Text = "Redescribe",
				Category = VerbCategory.Tricks,
				Icon = new SpriteSpecifier.Texture(new ResourcePath("/Textures/Interface/AdminActions/redescribe.png", "/")),
				Act = delegate()
				{
					QuickDialogSystem quickDialog = this._quickDialog;
					IPlayerSession player = player;
					string title = "Redescribe";
					string prompt = "Description";
					Action<LongString> okAction;
					if ((okAction = <>9__26) == null)
					{
						okAction = (<>9__26 = delegate(LongString newDescription)
						{
							this.MetaData(args.Target).EntityDescription = newDescription.String;
						});
					}
					quickDialog.OpenDialog<LongString>(player, title, prompt, okAction, null);
				},
				Impact = LogImpact.Medium,
				Message = Loc.GetString("admin-trick-redescribe-description"),
				Priority = -16
			};
			args.Verbs.Add(redescribe);
			Action<string, LongString> <>9__28;
			Verb renameAndRedescribe = new Verb
			{
				Text = "Redescribe",
				Category = VerbCategory.Tricks,
				Icon = new SpriteSpecifier.Texture(new ResourcePath("/Textures/Interface/AdminActions/rename_and_redescribe.png", "/")),
				Act = delegate()
				{
					QuickDialogSystem quickDialog = this._quickDialog;
					IPlayerSession player = player;
					string title = "Rename & Redescribe";
					string prompt = "Name";
					string prompt2 = "Description";
					Action<string, LongString> okAction;
					if ((okAction = <>9__28) == null)
					{
						okAction = (<>9__28 = delegate(string newName, LongString newDescription)
						{
							MetaDataComponent metaDataComponent = this.MetaData(args.Target);
							metaDataComponent.EntityName = newName;
							metaDataComponent.EntityDescription = newDescription.String;
						});
					}
					quickDialog.OpenDialog<string, LongString>(player, title, prompt, prompt2, okAction, null);
				},
				Impact = LogImpact.Medium,
				Message = Loc.GetString("admin-trick-rename-and-redescribe-description"),
				Priority = -17
			};
			args.Verbs.Add(renameAndRedescribe);
			SpeechComponent speechComponent;
			if (base.TryComp<SpeechComponent>(args.Target, ref speechComponent))
			{
				Action<string> <>9__30;
				Verb forceSay = new Verb
				{
					Text = "Force Say",
					Category = VerbCategory.Tricks,
					Icon = new SpriteSpecifier.Texture(new ResourcePath("/Textures/Interface/AdminActions/force_say.png", "/")),
					Act = delegate()
					{
						QuickDialogSystem quickDialog = this._quickDialog;
						IPlayerSession player = player;
						string title = "Force Say";
						string prompt = "Say";
						Action<string> okAction;
						if ((okAction = <>9__30) == null)
						{
							okAction = (<>9__30 = delegate(string say)
							{
								InGameICChatType chatType = InGameICChatType.Speak;
								if (say.StartsWith("*"))
								{
									chatType = InGameICChatType.Emote;
									say = say.Substring(1);
								}
								else if (say.StartsWith("#"))
								{
									chatType = InGameICChatType.Whisper;
									say = say.Substring(1);
								}
								this._chatSystem.TrySendInGameICMessage(args.Target, say, chatType, false, false, null, null, null, true, false);
							});
						}
						quickDialog.OpenDialog<string>(player, title, prompt, okAction, null);
					},
					Impact = LogImpact.Medium,
					Message = Loc.GetString("admin-trick-force-say"),
					Priority = -1
				};
				args.Verbs.Add(forceSay);
			}
			StationDataComponent stationData;
			if (base.TryComp<StationDataComponent>(args.Target, ref stationData))
			{
				if (this._adminManager.HasAdminFlag(player, AdminFlags.Round))
				{
					Verb barJobSlots = new Verb
					{
						Text = "Bar job slots",
						Category = VerbCategory.Tricks,
						Icon = new SpriteSpecifier.Texture(new ResourcePath("/Textures/Interface/AdminActions/bar_jobslots.png", "/")),
						Act = delegate()
						{
							foreach (KeyValuePair<string, uint?> keyValuePair in this._stationJobsSystem.GetJobs(args.Target, null))
							{
								string text;
								uint? num;
								keyValuePair.Deconstruct(out text, out num);
								string job = text;
								this._stationJobsSystem.TrySetJobSlot(args.Target, job, 0, true, null);
							}
						},
						Impact = LogImpact.Extreme,
						Message = Loc.GetString("admin-trick-bar-job-slots-description"),
						Priority = -18
					};
					args.Verbs.Add(barJobSlots);
				}
				Verb locateCargoShuttle = new Verb
				{
					Text = "Locate Cargo Shuttle",
					Category = VerbCategory.Tricks,
					Icon = new SpriteSpecifier.Rsi(new ResourcePath("/Textures/Clothing/Head/Soft/cargosoft.rsi", "/"), "icon"),
					Act = delegate()
					{
						EntityUid? shuttle = this.Comp<StationCargoOrderDatabaseComponent>(args.Target).Shuttle;
						if (shuttle == null)
						{
							return;
						}
						this.Transform(args.User).Coordinates = new EntityCoordinates(shuttle.Value, Vector2.Zero);
					},
					Impact = LogImpact.Low,
					Message = Loc.GetString("admin-trick-locate-cargo-shuttle-description"),
					Priority = -19
				};
				args.Verbs.Add(locateCargoShuttle);
			}
			IEnumerable<EntityUid> childEnum;
			if (this.TryGetGridChildren(args.Target, out childEnum))
			{
				Verb refillBattery2 = new Verb
				{
					Text = "Refill Battery",
					Category = VerbCategory.Tricks,
					Icon = new SpriteSpecifier.Texture(new ResourcePath("/Textures/Interface/AdminActions/fill_battery.png", "/")),
					Act = delegate()
					{
						foreach (EntityUid ent in childEnum)
						{
							if (this.HasComp<StationInfiniteBatteryTargetComponent>(ent))
							{
								BatteryComponent battery = this.EnsureComp<BatteryComponent>(ent);
								battery.CurrentCharge = battery.MaxCharge;
								this.Dirty(battery, null);
							}
						}
					},
					Impact = LogImpact.Extreme,
					Message = Loc.GetString("admin-trick-refill-battery-description"),
					Priority = -4
				};
				args.Verbs.Add(refillBattery2);
				Verb drainBattery2 = new Verb
				{
					Text = "Drain Battery",
					Category = VerbCategory.Tricks,
					Icon = new SpriteSpecifier.Texture(new ResourcePath("/Textures/Interface/AdminActions/drain_battery.png", "/")),
					Act = delegate()
					{
						foreach (EntityUid ent in childEnum)
						{
							if (this.HasComp<StationInfiniteBatteryTargetComponent>(ent))
							{
								BatteryComponent battery = this.EnsureComp<BatteryComponent>(ent);
								battery.CurrentCharge = 0f;
								this.Dirty(battery, null);
							}
						}
					},
					Impact = LogImpact.Extreme,
					Message = Loc.GetString("admin-trick-drain-battery-description"),
					Priority = -5
				};
				args.Verbs.Add(drainBattery2);
				Verb infiniteBattery2 = new Verb
				{
					Text = "Infinite Battery",
					Category = VerbCategory.Tricks,
					Icon = new SpriteSpecifier.Texture(new ResourcePath("/Textures/Interface/AdminActions/infinite_battery.png", "/")),
					Act = delegate()
					{
						foreach (EntityUid ent in childEnum)
						{
							if (this.HasComp<StationInfiniteBatteryTargetComponent>(ent))
							{
								BatterySelfRechargerComponent batterySelfRechargerComponent = this.EnsureComp<BatterySelfRechargerComponent>(ent);
								BatteryComponent battery = this.EnsureComp<BatteryComponent>(ent);
								batterySelfRechargerComponent.AutoRecharge = true;
								batterySelfRechargerComponent.AutoRechargeRate = battery.MaxCharge;
							}
						}
					},
					Impact = LogImpact.Extreme,
					Message = Loc.GetString("admin-trick-infinite-battery-description"),
					Priority = -20
				};
				args.Verbs.Add(infiniteBattery2);
			}
			PhysicsComponent physics;
			if (base.TryComp<PhysicsComponent>(args.Target, ref physics))
			{
				Verb haltMovement = new Verb
				{
					Text = "Halt Movement",
					Category = VerbCategory.Tricks,
					Icon = new SpriteSpecifier.Texture(new ResourcePath("/Textures/Interface/AdminActions/halt.png", "/")),
					Act = delegate()
					{
						this._physics.SetLinearVelocity(args.Target, Vector2.Zero, true, true, null, physics);
						this._physics.SetAngularVelocity(args.Target, 0f, true, null, physics);
					},
					Impact = LogImpact.Medium,
					Message = Loc.GetString("admin-trick-halt-movement-description"),
					Priority = -21
				};
				args.Verbs.Add(haltMovement);
			}
			MapComponent map;
			if (base.TryComp<MapComponent>(args.Target, ref map) && this._adminManager.HasAdminFlag(player, AdminFlags.Mapping))
			{
				if (this._mapManager.IsMapPaused(map.WorldMap))
				{
					Verb unpauseMap = new Verb
					{
						Text = "Unpause Map",
						Category = VerbCategory.Tricks,
						Icon = new SpriteSpecifier.Texture(new ResourcePath("/Textures/Interface/AdminActions/play.png", "/")),
						Act = delegate()
						{
							this._mapManager.SetMapPaused(map.WorldMap, false);
						},
						Impact = LogImpact.Extreme,
						Message = Loc.GetString("admin-trick-unpause-map-description"),
						Priority = -22
					};
					args.Verbs.Add(unpauseMap);
				}
				else
				{
					Verb pauseMap = new Verb
					{
						Text = "Pause Map",
						Category = VerbCategory.Tricks,
						Icon = new SpriteSpecifier.Texture(new ResourcePath("/Textures/Interface/AdminActions/pause.png", "/")),
						Act = delegate()
						{
							this._mapManager.SetMapPaused(map.WorldMap, true);
						},
						Impact = LogImpact.Extreme,
						Message = Loc.GetString("admin-trick-pause-map-description"),
						Priority = -23
					};
					args.Verbs.Add(pauseMap);
				}
			}
			JointComponent joints;
			if (base.TryComp<JointComponent>(args.Target, ref joints))
			{
				Verb snapJoints = new Verb
				{
					Text = "Snap Joints",
					Category = VerbCategory.Tricks,
					Icon = new SpriteSpecifier.Texture(new ResourcePath("/Textures/Interface/AdminActions/snap_joints.png", "/")),
					Act = delegate()
					{
						this._jointSystem.ClearJoints(joints);
					},
					Impact = LogImpact.Medium,
					Message = Loc.GetString("admin-trick-snap-joints-description"),
					Priority = -24
				};
				args.Verbs.Add(snapJoints);
			}
			GunComponent gun;
			if (base.TryComp<GunComponent>(args.Target, ref gun))
			{
				Verb minigunFire = new Verb
				{
					Text = "Make Minigun",
					Category = VerbCategory.Tricks,
					Icon = new SpriteSpecifier.Rsi(new ResourcePath("/Textures/Objects/Weapons/Guns/HMGs/minigun.rsi", "/"), "icon"),
					Act = delegate()
					{
						gun.FireRate = 15f;
					},
					Impact = LogImpact.Medium,
					Message = Loc.GetString("admin-trick-minigun-fire-description"),
					Priority = -25
				};
				args.Verbs.Add(minigunFire);
			}
			BallisticAmmoProviderComponent ballisticAmmo;
			if (base.TryComp<BallisticAmmoProviderComponent>(args.Target, ref ballisticAmmo))
			{
				Action<int> <>9__42;
				Verb setCapacity = new Verb
				{
					Text = "Set Bullet Amount",
					Category = VerbCategory.Tricks,
					Icon = new SpriteSpecifier.Rsi(new ResourcePath("/Textures/Objects/Fun/caps.rsi", "/"), "mag-6"),
					Act = delegate()
					{
						QuickDialogSystem quickDialog = this._quickDialog;
						IPlayerSession player = player;
						string title = "Set Bullet Amount";
						DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(14, 1);
						defaultInterpolatedStringHandler.AppendLiteral("Amount (max ");
						defaultInterpolatedStringHandler.AppendFormatted<int>(ballisticAmmo.Capacity);
						defaultInterpolatedStringHandler.AppendLiteral("):");
						string prompt = defaultInterpolatedStringHandler.ToStringAndClear();
						Action<int> okAction;
						if ((okAction = <>9__42) == null)
						{
							okAction = (<>9__42 = delegate(int amount)
							{
								ballisticAmmo.UnspawnedCount = amount;
							});
						}
						quickDialog.OpenDialog<int>(player, title, prompt, okAction, null);
					},
					Impact = LogImpact.Medium,
					Message = Loc.GetString("admin-trick-set-bullet-amount-description"),
					Priority = -26
				};
				args.Verbs.Add(setCapacity);
			}
		}

		// Token: 0x06002CCA RID: 11466 RVA: 0x000EC960 File Offset: 0x000EAB60
		[NullableContext(2)]
		private void RefillGasTank(EntityUid tank, Gas gasType, GasTankComponent tankComponent)
		{
			if (!base.Resolve<GasTankComponent>(tank, ref tankComponent, true))
			{
				return;
			}
			float mixSize = tankComponent.Air.Volume;
			GasMixture newMix = new GasMixture(mixSize);
			newMix.SetMoles(gasType, 1000f * mixSize / 2437.3848f);
			newMix.Temperature = 293.15f;
			tankComponent.Air = newMix;
		}

		// Token: 0x06002CCB RID: 11467 RVA: 0x000EC9B3 File Offset: 0x000EABB3
		[NullableContext(2)]
		private bool TryGetGridChildren(EntityUid target, [NotNullWhen(true)] out IEnumerable<EntityUid> enumerator)
		{
			if (!base.HasComp<MapComponent>(target) && !base.HasComp<MapGridComponent>(target) && !base.HasComp<StationDataComponent>(target))
			{
				enumerator = null;
				return false;
			}
			enumerator = this.GetGridChildrenInner(target);
			return true;
		}

		// Token: 0x06002CCC RID: 11468 RVA: 0x000EC9DF File Offset: 0x000EABDF
		private IEnumerable<EntityUid> GetGridChildrenInner(EntityUid target)
		{
			StationDataComponent station;
			if (base.TryComp<StationDataComponent>(target, ref station))
			{
				foreach (EntityUid grid in station.Grids)
				{
					foreach (EntityUid ent in base.Transform(grid).ChildEntities)
					{
						yield return ent;
					}
					IEnumerator<EntityUid> enumerator2 = null;
				}
				HashSet<EntityUid>.Enumerator enumerator = default(HashSet<EntityUid>.Enumerator);
			}
			else if (base.HasComp<MapComponent>(target))
			{
				foreach (EntityUid possibleGrid in base.Transform(target).ChildEntities)
				{
					foreach (EntityUid ent2 in base.Transform(possibleGrid).ChildEntities)
					{
						yield return ent2;
					}
					IEnumerator<EntityUid> enumerator3 = null;
				}
				IEnumerator<EntityUid> enumerator2 = null;
			}
			else
			{
				foreach (EntityUid ent3 in base.Transform(target).ChildEntities)
				{
					yield return ent3;
				}
				IEnumerator<EntityUid> enumerator2 = null;
			}
			yield break;
			yield break;
		}

		// Token: 0x06002CCD RID: 11469 RVA: 0x000EC9F8 File Offset: 0x000EABF8
		private EntityUid? FindActiveId(EntityUid target)
		{
			EntityUid? slotEntity;
			HandsComponent hands;
			if (this._inventorySystem.TryGetSlotEntity(target, "id", out slotEntity, null, null))
			{
				if (base.HasComp<AccessComponent>(slotEntity))
				{
					return new EntityUid?(slotEntity.Value);
				}
				PDAComponent pda;
				if (base.TryComp<PDAComponent>(slotEntity, ref pda) && pda.ContainedID != null)
				{
					return new EntityUid?(pda.ContainedID.Owner);
				}
			}
			else if (base.TryComp<HandsComponent>(target, ref hands))
			{
				foreach (EntityUid held in this._handsSystem.EnumerateHeld(target, hands))
				{
					if (base.HasComp<AccessComponent>(held))
					{
						return new EntityUid?(held);
					}
				}
			}
			return null;
		}

		// Token: 0x06002CCE RID: 11470 RVA: 0x000ECAC4 File Offset: 0x000EACC4
		private void GiveAllAccess(EntityUid entity)
		{
			string[] allAccess = (from p in this._prototypeManager.EnumeratePrototypes<AccessLevelPrototype>()
			select p.ID).ToArray<string>();
			this._accessSystem.TrySetTags(entity, allAccess, null);
		}

		// Token: 0x06002CCF RID: 11471 RVA: 0x000ECB15 File Offset: 0x000EAD15
		private void RevokeAllAccess(EntityUid entity)
		{
			this._accessSystem.TrySetTags(entity, new string[0], null);
		}

		// Token: 0x04001B9A RID: 7066
		[Dependency]
		private readonly ZombifyOnDeathSystem _zombify;

		// Token: 0x04001B9B RID: 7067
		[Dependency]
		private readonly TraitorRuleSystem _traitorRule;

		// Token: 0x04001B9C RID: 7068
		[Dependency]
		private readonly NukeopsRuleSystem _nukeopsRule;

		// Token: 0x04001B9D RID: 7069
		[Dependency]
		private readonly PiratesRuleSystem _piratesRule;

		// Token: 0x04001B9E RID: 7070
		[Dependency]
		private readonly IConGroupController _groupController;

		// Token: 0x04001B9F RID: 7071
		[Dependency]
		private readonly IConsoleHost _console;

		// Token: 0x04001BA0 RID: 7072
		[Dependency]
		private readonly IAdminManager _adminManager;

		// Token: 0x04001BA1 RID: 7073
		[Dependency]
		private readonly IGameTiming _gameTiming;

		// Token: 0x04001BA2 RID: 7074
		[Dependency]
		private readonly IMapManager _mapManager;

		// Token: 0x04001BA3 RID: 7075
		[Dependency]
		private readonly IPrototypeManager _prototypeManager;

		// Token: 0x04001BA4 RID: 7076
		[Dependency]
		private readonly EuiManager _euiManager;

		// Token: 0x04001BA5 RID: 7077
		[Dependency]
		private readonly GhostRoleSystem _ghostRoleSystem;

		// Token: 0x04001BA6 RID: 7078
		[Dependency]
		private readonly ArtifactSystem _artifactSystem;

		// Token: 0x04001BA7 RID: 7079
		[Dependency]
		private readonly UserInterfaceSystem _uiSystem;

		// Token: 0x04001BA8 RID: 7080
		[Dependency]
		private readonly PrayerSystem _prayerSystem;

		// Token: 0x04001BA9 RID: 7081
		[Dependency]
		private readonly EuiManager _eui;

		// Token: 0x04001BAA RID: 7082
		[Dependency]
		private readonly EntityManager _entMan;

		// Token: 0x04001BAB RID: 7083
		private readonly Dictionary<IPlayerSession, EditSolutionsEui> _openSolutionUis = new Dictionary<IPlayerSession, EditSolutionsEui>();

		// Token: 0x04001BAC RID: 7084
		[Dependency]
		private readonly IRobustRandom _random;

		// Token: 0x04001BAD RID: 7085
		[Dependency]
		private readonly BloodstreamSystem _bloodstreamSystem;

		// Token: 0x04001BAE RID: 7086
		[Dependency]
		private readonly BodySystem _bodySystem;

		// Token: 0x04001BAF RID: 7087
		[Dependency]
		private readonly CreamPieSystem _creamPieSystem;

		// Token: 0x04001BB0 RID: 7088
		[Dependency]
		private readonly DiseaseSystem _diseaseSystem;

		// Token: 0x04001BB1 RID: 7089
		[Dependency]
		private readonly ElectrocutionSystem _electrocutionSystem;

		// Token: 0x04001BB2 RID: 7090
		[Dependency]
		private readonly EntityStorageSystem _entityStorageSystem;

		// Token: 0x04001BB3 RID: 7091
		[Dependency]
		private readonly ExplosionSystem _explosionSystem;

		// Token: 0x04001BB4 RID: 7092
		[Dependency]
		private readonly FixtureSystem _fixtures;

		// Token: 0x04001BB5 RID: 7093
		[Dependency]
		private readonly FlammableSystem _flammableSystem;

		// Token: 0x04001BB6 RID: 7094
		[Dependency]
		private readonly GhostKickManager _ghostKickManager;

		// Token: 0x04001BB7 RID: 7095
		[Dependency]
		private readonly GodmodeSystem _godmodeSystem;

		// Token: 0x04001BB8 RID: 7096
		[Dependency]
		private readonly InventorySystem _inventorySystem;

		// Token: 0x04001BB9 RID: 7097
		[Dependency]
		private readonly MovementSpeedModifierSystem _movementSpeedModifierSystem;

		// Token: 0x04001BBA RID: 7098
		[Dependency]
		private readonly PolymorphableSystem _polymorphableSystem;

		// Token: 0x04001BBB RID: 7099
		[Dependency]
		private readonly MobThresholdSystem _mobThresholdSystem;

		// Token: 0x04001BBC RID: 7100
		[Dependency]
		private readonly PopupSystem _popupSystem;

		// Token: 0x04001BBD RID: 7101
		[Dependency]
		private readonly SharedPhysicsSystem _physics;

		// Token: 0x04001BBE RID: 7102
		[Dependency]
		private readonly TabletopSystem _tabletopSystem;

		// Token: 0x04001BBF RID: 7103
		[Dependency]
		private readonly VomitSystem _vomitSystem;

		// Token: 0x04001BC0 RID: 7104
		[Dependency]
		private readonly WeldableSystem _weldableSystem;

		// Token: 0x04001BC1 RID: 7105
		[Dependency]
		private readonly AirlockSystem _airlockSystem;

		// Token: 0x04001BC2 RID: 7106
		[Dependency]
		private readonly StackSystem _stackSystem;

		// Token: 0x04001BC3 RID: 7107
		[Dependency]
		private readonly SharedAccessSystem _accessSystem;

		// Token: 0x04001BC4 RID: 7108
		[Dependency]
		private readonly HandsSystem _handsSystem;

		// Token: 0x04001BC5 RID: 7109
		[Dependency]
		private readonly QuickDialogSystem _quickDialog;

		// Token: 0x04001BC6 RID: 7110
		[Dependency]
		private readonly AdminTestArenaSystem _adminTestArenaSystem;

		// Token: 0x04001BC7 RID: 7111
		[Dependency]
		private readonly StationJobsSystem _stationJobsSystem;

		// Token: 0x04001BC8 RID: 7112
		[Dependency]
		private readonly JointSystem _jointSystem;

		// Token: 0x04001BC9 RID: 7113
		[Dependency]
		private readonly ChatSystem _chatSystem;

		// Token: 0x02000B55 RID: 2901
		[NullableContext(0)]
		public enum TricksVerbPriorities
		{
			// Token: 0x04002A2A RID: 10794
			Bolt,
			// Token: 0x04002A2B RID: 10795
			Unbolt = 0,
			// Token: 0x04002A2C RID: 10796
			EmergencyAccessOn = -1,
			// Token: 0x04002A2D RID: 10797
			EmergencyAccessOff = -1,
			// Token: 0x04002A2E RID: 10798
			MakeIndestructible = -2,
			// Token: 0x04002A2F RID: 10799
			MakeVulnerable = -2,
			// Token: 0x04002A30 RID: 10800
			BlockUnanchoring = -3,
			// Token: 0x04002A31 RID: 10801
			RefillBattery = -4,
			// Token: 0x04002A32 RID: 10802
			DrainBattery = -5,
			// Token: 0x04002A33 RID: 10803
			RefillOxygen = -6,
			// Token: 0x04002A34 RID: 10804
			RefillNitrogen = -7,
			// Token: 0x04002A35 RID: 10805
			RefillPlasma = -8,
			// Token: 0x04002A36 RID: 10806
			SendToTestArena = -9,
			// Token: 0x04002A37 RID: 10807
			GrantAllAccess = -10,
			// Token: 0x04002A38 RID: 10808
			RevokeAllAccess = -11,
			// Token: 0x04002A39 RID: 10809
			Rejuvenate = -12,
			// Token: 0x04002A3A RID: 10810
			AdjustStack = -13,
			// Token: 0x04002A3B RID: 10811
			FillStack = -14,
			// Token: 0x04002A3C RID: 10812
			Rename = -15,
			// Token: 0x04002A3D RID: 10813
			Redescribe = -16,
			// Token: 0x04002A3E RID: 10814
			RenameAndRedescribe = -17,
			// Token: 0x04002A3F RID: 10815
			BarJobSlots = -18,
			// Token: 0x04002A40 RID: 10816
			LocateCargoShuttle = -19,
			// Token: 0x04002A41 RID: 10817
			InfiniteBattery = -20,
			// Token: 0x04002A42 RID: 10818
			HaltMovement = -21,
			// Token: 0x04002A43 RID: 10819
			Unpause = -22,
			// Token: 0x04002A44 RID: 10820
			Pause = -23,
			// Token: 0x04002A45 RID: 10821
			SnapJoints = -24,
			// Token: 0x04002A46 RID: 10822
			MakeMinigun = -25,
			// Token: 0x04002A47 RID: 10823
			SetBulletAmount = -26,
			// Token: 0x04002A48 RID: 10824
			ForceSay = -1
		}
	}
}
