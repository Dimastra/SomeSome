using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using Content.Client.Verbs;
using Content.Shared.Examine;
using Content.Shared.Eye.Blinding;
using Content.Shared.IdentityManagement;
using Content.Shared.Input;
using Content.Shared.Interaction;
using Content.Shared.Verbs;
using Robust.Client.GameObjects;
using Robust.Client.Graphics;
using Robust.Client.Player;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.Controls;
using Robust.Shared.GameObjects;
using Robust.Shared.Input.Binding;
using Robust.Shared.IoC;
using Robust.Shared.Localization;
using Robust.Shared.Map;
using Robust.Shared.Maths;
using Robust.Shared.Utility;

namespace Content.Client.Examine
{
	// Token: 0x0200032A RID: 810
	[NullableContext(1)]
	[Nullable(0)]
	internal sealed class ExamineSystem : ExamineSystemShared
	{
		// Token: 0x0600144D RID: 5197 RVA: 0x000771EC File Offset: 0x000753EC
		public override void Initialize()
		{
			base.UpdatesOutsidePrediction = true;
			base.SubscribeLocalEvent<GetVerbsEvent<ExamineVerb>>(new EntityEventHandler<GetVerbsEvent<ExamineVerb>>(this.AddExamineVerb), null, null);
			base.SubscribeNetworkEvent<ExamineSystemMessages.ExamineInfoResponseMessage>(new EntityEventHandler<ExamineSystemMessages.ExamineInfoResponseMessage>(this.OnExamineInfoResponse), null, null);
			CommandBinds.Builder.Bind(ContentKeyFunctions.ExamineEntity, new PointerInputCmdHandler(new PointerInputCmdDelegate2(this.HandleExamine), true, true)).Register<ExamineSystem>();
			this._idCounter = 0;
		}

		// Token: 0x0600144E RID: 5198 RVA: 0x00077258 File Offset: 0x00075458
		public override void Update(float frameTime)
		{
			Popup examineTooltipOpen = this._examineTooltipOpen;
			if (examineTooltipOpen == null || !examineTooltipOpen.Visible)
			{
				return;
			}
			if (!this._examinedEntity.Valid || !this._playerEntity.Valid)
			{
				return;
			}
			if (!base.CanExamine(this._playerEntity, this._examinedEntity))
			{
				this.CloseTooltip();
			}
		}

		// Token: 0x0600144F RID: 5199 RVA: 0x000772AD File Offset: 0x000754AD
		public override void Shutdown()
		{
			CommandBinds.Unregister<ExamineSystem>();
			base.Shutdown();
		}

		// Token: 0x06001450 RID: 5200 RVA: 0x000772BC File Offset: 0x000754BC
		[NullableContext(2)]
		public override bool CanExamine(EntityUid examiner, MapCoordinates target, SharedInteractionSystem.Ignored predicate = null, EntityUid? examined = null, ExaminerComponent examinerComp = null)
		{
			return base.Resolve<ExaminerComponent>(examiner, ref examinerComp, false) && (examinerComp.SkipChecks || ((!examinerComp.CheckInRangeUnOccluded || this._eyeManager.GetWorldViewbounds().Contains(target.Position)) && base.CanExamine(examiner, target, predicate, examined, examinerComp)));
		}

		// Token: 0x06001451 RID: 5201 RVA: 0x00077318 File Offset: 0x00075518
		private bool HandleExamine(in PointerInputCmdHandler.PointerInputCmdArgs args)
		{
			if (!args.EntityUid.IsValid() || !this.EntityManager.EntityExists(args.EntityUid))
			{
				return false;
			}
			LocalPlayer localPlayer = this._playerManager.LocalPlayer;
			this._playerEntity = ((localPlayer != null) ? localPlayer.ControlledEntity : null).GetValueOrDefault();
			if (this._playerEntity == default(EntityUid) || !base.CanExamine(this._playerEntity, args.EntityUid))
			{
				return false;
			}
			this.DoExamine(args.EntityUid, true);
			return true;
		}

		// Token: 0x06001452 RID: 5202 RVA: 0x000773B0 File Offset: 0x000755B0
		private void AddExamineVerb(GetVerbsEvent<ExamineVerb> args)
		{
			if (!base.CanExamine(args.User, args.Target))
			{
				return;
			}
			ExamineVerb examineVerb = new ExamineVerb();
			examineVerb.Category = VerbCategory.Examine;
			examineVerb.Priority = 10;
			examineVerb.Act = delegate()
			{
				this.DoExamine(args.Target, false);
			};
			examineVerb.Text = Loc.GetString("examine-verb-name");
			examineVerb.Icon = new SpriteSpecifier.Texture(new ResourcePath("/Textures/Interface/VerbIcons/examine.svg.192dpi.png", "/"));
			examineVerb.ShowOnExamineTooltip = false;
			examineVerb.ClientExclusive = true;
			args.Verbs.Add(examineVerb);
		}

		// Token: 0x06001453 RID: 5203 RVA: 0x00077468 File Offset: 0x00075668
		private void OnExamineInfoResponse(ExamineSystemMessages.ExamineInfoResponseMessage ev)
		{
			LocalPlayer localPlayer = this._playerManager.LocalPlayer;
			EntityUid? entityUid = (localPlayer != null) ? localPlayer.ControlledEntity : null;
			if (entityUid == null)
			{
				return;
			}
			if (ev.Id != 0 && ev.Id != this._idCounter)
			{
				return;
			}
			this.OpenTooltip(entityUid.Value, ev.EntityUid, ev.CenterAtCursor, ev.OpenAtOldTooltip, ev.KnowTarget);
			this.UpdateTooltipInfo(entityUid.Value, ev.EntityUid, ev.Message, ev.Verbs);
		}

		// Token: 0x06001454 RID: 5204 RVA: 0x000774FA File Offset: 0x000756FA
		public override void SendExamineTooltip(EntityUid player, EntityUid target, FormattedMessage message, bool getVerbs, bool centerAtCursor)
		{
			this.OpenTooltip(player, target, centerAtCursor, false, true);
			this.UpdateTooltipInfo(player, target, message, null);
		}

		// Token: 0x06001455 RID: 5205 RVA: 0x00077514 File Offset: 0x00075714
		public void OpenTooltip(EntityUid player, EntityUid target, bool centeredOnCursor = true, bool openAtOldTooltip = true, bool knowTarget = true)
		{
			ScreenCoordinates? screenCoordinates = (this._examineTooltipOpen != null) ? new ScreenCoordinates?(this._popupPos) : null;
			this.CloseTooltip();
			this._examinedEntity = target;
			if (openAtOldTooltip && screenCoordinates != null)
			{
				this._popupPos = screenCoordinates.Value;
			}
			else if (centeredOnCursor)
			{
				this._popupPos = this._userInterfaceManager.MousePositionScaled;
			}
			else
			{
				this._popupPos = this._eyeManager.CoordinatesToScreen(base.Transform(target).Coordinates);
				this._popupPos = this._userInterfaceManager.ScreenToUIPosition(this._popupPos);
			}
			this._examineTooltipOpen = new Popup
			{
				MaxWidth = 400f
			};
			this._userInterfaceManager.ModalRoot.AddChild(this._examineTooltipOpen);
			PanelContainer panelContainer = new PanelContainer
			{
				Name = "ExaminePopupPanel"
			};
			panelContainer.AddStyleClass("entity-tooltip");
			panelContainer.ModulateSelfOverride = new Color?(Color.LightGray.WithAlpha(0.9f));
			this._examineTooltipOpen.AddChild(panelContainer);
			BoxContainer boxContainer = new BoxContainer
			{
				Name = "ExaminePopupVbox",
				Orientation = 1
			};
			panelContainer.AddChild(boxContainer);
			BoxContainer boxContainer2 = new BoxContainer
			{
				Orientation = 0,
				SeparationOverride = new int?(5)
			};
			boxContainer.AddChild(boxContainer2);
			SpriteComponent sprite;
			if (this.EntityManager.TryGetComponent<SpriteComponent>(target, ref sprite))
			{
				boxContainer2.AddChild(new SpriteView
				{
					Sprite = sprite,
					OverrideDirection = new Direction?(0),
					Margin = new Thickness(2f, 0f, 2f, 0f)
				});
			}
			if (knowTarget)
			{
				boxContainer2.AddChild(new Label
				{
					Text = Identity.Name(target, this.EntityManager, new EntityUid?(player)),
					HorizontalExpand = true
				});
			}
			else
			{
				boxContainer2.AddChild(new Label
				{
					Text = "???",
					HorizontalExpand = true
				});
			}
			panelContainer.Measure(Vector2.Infinity);
			Vector2 vector = Vector2.ComponentMax(new ValueTuple<float, float>(300f, 0f), panelContainer.DesiredSize);
			this._examineTooltipOpen.Open(new UIBox2?(UIBox2.FromDimensions(this._popupPos.Position, vector)), null);
		}

		// Token: 0x06001456 RID: 5206 RVA: 0x00077758 File Offset: 0x00075958
		public void UpdateTooltipInfo(EntityUid player, EntityUid target, FormattedMessage message, [Nullable(new byte[]
		{
			2,
			1
		})] List<Verb> verbs = null)
		{
			Popup examineTooltipOpen = this._examineTooltipOpen;
			Control control = (examineTooltipOpen != null) ? examineTooltipOpen.GetChild(0).GetChild(0) : null;
			if (control == null)
			{
				return;
			}
			foreach (MarkupNode markupNode in message.Nodes)
			{
				if (markupNode.Name == null && !string.IsNullOrWhiteSpace(markupNode.Value.StringValue ?? ""))
				{
					RichTextLabel richTextLabel = new RichTextLabel
					{
						Margin = new Thickness(4f, 4f, 0f, 4f)
					};
					richTextLabel.SetMessage(message);
					control.AddChild(richTextLabel);
					break;
				}
			}
			if (verbs == null)
			{
				verbs = new List<Verb>();
			}
			SortedSet<Verb> localVerbs = this._verbSystem.GetLocalVerbs(target, player, typeof(ExamineVerb), false);
			localVerbs.UnionWith(verbs);
			this.AddVerbsToTooltip(localVerbs);
		}

		// Token: 0x06001457 RID: 5207 RVA: 0x0007784C File Offset: 0x00075A4C
		private void AddVerbsToTooltip(IEnumerable<Verb> verbs)
		{
			if (this._examineTooltipOpen == null)
			{
				return;
			}
			BoxContainer boxContainer = new BoxContainer
			{
				Name = "ExamineButtonsHBox",
				Orientation = 0,
				HorizontalAlignment = 3,
				VerticalAlignment = 3
			};
			foreach (Verb verb in verbs)
			{
				ExamineVerb examineVerb = verb as ExamineVerb;
				if (examineVerb != null && examineVerb.Icon != null && examineVerb.ShowOnExamineTooltip)
				{
					ExamineButton examineButton = new ExamineButton(examineVerb);
					examineButton.OnPressed += this.VerbButtonPressed;
					boxContainer.AddChild(examineButton);
				}
			}
			Popup examineTooltipOpen = this._examineTooltipOpen;
			Control control = (examineTooltipOpen != null) ? examineTooltipOpen.GetChild(0).GetChild(0) : null;
			if (control == null)
			{
				boxContainer.Dispose();
				return;
			}
			Control[] source = (from c in control.Children
			where c.Name == "ExamineButtonsHBox"
			select c).ToArray<Control>();
			if (source.Any<Control>())
			{
				control.Children.Remove(source.First<Control>());
			}
			control.AddChild(boxContainer);
		}

		// Token: 0x06001458 RID: 5208 RVA: 0x00077974 File Offset: 0x00075B74
		public void VerbButtonPressed(BaseButton.ButtonEventArgs obj)
		{
			ExamineButton examineButton = obj.Button as ExamineButton;
			if (examineButton != null)
			{
				this._verbSystem.ExecuteVerb(this._examinedEntity, examineButton.Verb);
				if (examineButton.Verb.CloseMenu ?? examineButton.Verb.CloseMenuDefault)
				{
					this.CloseTooltip();
				}
			}
		}

		// Token: 0x06001459 RID: 5209 RVA: 0x000779D8 File Offset: 0x00075BD8
		public void DoExamine(EntityUid entity, bool centeredOnCursor = true)
		{
			LocalPlayer localPlayer = this._playerManager.LocalPlayer;
			EntityUid? entityUid = (localPlayer != null) ? localPlayer.ControlledEntity : null;
			if (entityUid == null)
			{
				return;
			}
			bool knowTarget = true;
			if (base.HasComp<BlurryVisionComponent>(entityUid))
			{
				knowTarget = false;
			}
			this.OpenTooltip(entityUid.Value, entity, centeredOnCursor, false, knowTarget);
			if (entity.IsClientSide())
			{
				FormattedMessage examineText = base.GetExamineText(entity, entityUid);
				this.UpdateTooltipInfo(entityUid.Value, entity, examineText, null);
			}
			else
			{
				if (entity != this._lastExaminedEntity)
				{
					this._idCounter++;
				}
				if (this._idCounter == 2147483647)
				{
					this._idCounter = 0;
				}
				base.RaiseNetworkEvent(new ExamineSystemMessages.RequestExamineInfoMessage(entity, this._idCounter, true));
			}
			this._lastExaminedEntity = entity;
		}

		// Token: 0x0600145A RID: 5210 RVA: 0x00077A9C File Offset: 0x00075C9C
		private void CloseTooltip()
		{
			if (this._examineTooltipOpen != null)
			{
				foreach (Control control in this._examineTooltipOpen.Children)
				{
					ExamineButton examineButton = control as ExamineButton;
					if (examineButton != null)
					{
						examineButton.OnPressed -= this.VerbButtonPressed;
					}
				}
				this._examineTooltipOpen.Dispose();
				this._examineTooltipOpen = null;
			}
			if (this._requestCancelTokenSource != null)
			{
				this._requestCancelTokenSource.Cancel();
				this._requestCancelTokenSource = null;
			}
		}

		// Token: 0x04000A40 RID: 2624
		[Dependency]
		private readonly IUserInterfaceManager _userInterfaceManager;

		// Token: 0x04000A41 RID: 2625
		[Dependency]
		private readonly IPlayerManager _playerManager;

		// Token: 0x04000A42 RID: 2626
		[Dependency]
		private readonly IEyeManager _eyeManager;

		// Token: 0x04000A43 RID: 2627
		[Dependency]
		private readonly VerbSystem _verbSystem;

		// Token: 0x04000A44 RID: 2628
		public const string StyleClassEntityTooltip = "entity-tooltip";

		// Token: 0x04000A45 RID: 2629
		private EntityUid _examinedEntity;

		// Token: 0x04000A46 RID: 2630
		private EntityUid _lastExaminedEntity;

		// Token: 0x04000A47 RID: 2631
		private EntityUid _playerEntity;

		// Token: 0x04000A48 RID: 2632
		[Nullable(2)]
		private Popup _examineTooltipOpen;

		// Token: 0x04000A49 RID: 2633
		private ScreenCoordinates _popupPos;

		// Token: 0x04000A4A RID: 2634
		[Nullable(2)]
		private CancellationTokenSource _requestCancelTokenSource;

		// Token: 0x04000A4B RID: 2635
		private int _idCounter;
	}
}
