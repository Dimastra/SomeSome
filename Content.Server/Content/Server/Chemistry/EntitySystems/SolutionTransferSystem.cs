using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Content.Server.Administration.Logs;
using Content.Server.Chemistry.Components.SolutionManager;
using Content.Shared.Administration.Logs;
using Content.Shared.Chemistry;
using Content.Shared.Chemistry.Components;
using Content.Shared.Database;
using Content.Shared.FixedPoint;
using Content.Shared.Interaction;
using Content.Shared.Popups;
using Content.Shared.Verbs;
using Robust.Server.GameObjects;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Localization;

namespace Content.Server.Chemistry.EntitySystems
{
	// Token: 0x020006A0 RID: 1696
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class SolutionTransferSystem : EntitySystem
	{
		// Token: 0x06002360 RID: 9056 RVA: 0x000B8AEC File Offset: 0x000B6CEC
		public override void Initialize()
		{
			base.Initialize();
			base.SubscribeLocalEvent<SolutionTransferComponent, GetVerbsEvent<AlternativeVerb>>(new ComponentEventHandler<SolutionTransferComponent, GetVerbsEvent<AlternativeVerb>>(this.AddSetTransferVerbs), null, null);
			base.SubscribeLocalEvent<SolutionTransferComponent, AfterInteractEvent>(new ComponentEventHandler<SolutionTransferComponent, AfterInteractEvent>(this.OnAfterInteract), null, null);
			base.SubscribeLocalEvent<SolutionTransferComponent, TransferAmountSetValueMessage>(new ComponentEventHandler<SolutionTransferComponent, TransferAmountSetValueMessage>(this.OnTransferAmountSetValueMessage), null, null);
		}

		// Token: 0x06002361 RID: 9057 RVA: 0x000B8B3C File Offset: 0x000B6D3C
		private void OnTransferAmountSetValueMessage(EntityUid uid, SolutionTransferComponent solutionTransfer, TransferAmountSetValueMessage message)
		{
			FixedPoint2 newTransferAmount = FixedPoint2.Clamp(message.Value, solutionTransfer.MinimumTransferAmount, solutionTransfer.MaximumTransferAmount);
			solutionTransfer.TransferAmount = newTransferAmount;
			EntityUid? attachedEntity = message.Session.AttachedEntity;
			if (attachedEntity != null)
			{
				EntityUid user = attachedEntity.GetValueOrDefault();
				if (user.Valid)
				{
					this._popupSystem.PopupEntity(Loc.GetString("comp-solution-transfer-set-amount", new ValueTuple<string, object>[]
					{
						new ValueTuple<string, object>("amount", newTransferAmount)
					}), uid, user, PopupType.Small);
				}
			}
		}

		// Token: 0x06002362 RID: 9058 RVA: 0x000B8BC4 File Offset: 0x000B6DC4
		private void AddSetTransferVerbs(EntityUid uid, SolutionTransferComponent component, GetVerbsEvent<AlternativeVerb> args)
		{
			if (!args.CanAccess || !args.CanInteract || !component.CanChangeTransferAmount || args.Hands == null)
			{
				return;
			}
			ActorComponent actor;
			if (!this.EntityManager.TryGetComponent<ActorComponent>(args.User, ref actor))
			{
				return;
			}
			AlternativeVerb custom = new AlternativeVerb();
			custom.Text = Loc.GetString("comp-solution-transfer-verb-custom-amount");
			custom.Category = VerbCategory.SetTransferAmount;
			custom.Act = delegate()
			{
				this._userInterfaceSystem.TryOpen(args.Target, TransferAmountUiKey.Key, actor.PlayerSession, null);
			};
			custom.Priority = 1;
			args.Verbs.Add(custom);
			int priority = 0;
			using (List<int>.Enumerator enumerator = SolutionTransferSystem.DefaultTransferAmounts.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					int amount = enumerator.Current;
					if (amount >= component.MinimumTransferAmount.Int() && amount <= component.MaximumTransferAmount.Int())
					{
						AlternativeVerb verb = new AlternativeVerb();
						verb.Text = Loc.GetString("comp-solution-transfer-verb-amount", new ValueTuple<string, object>[]
						{
							new ValueTuple<string, object>("amount", amount)
						});
						verb.Category = VerbCategory.SetTransferAmount;
						verb.Act = delegate()
						{
							component.TransferAmount = FixedPoint2.New(amount);
							this._popupSystem.PopupEntity(Loc.GetString("comp-solution-transfer-set-amount", new ValueTuple<string, object>[]
							{
								new ValueTuple<string, object>("amount", amount)
							}), uid, args.User, PopupType.Small);
						};
						verb.Priority = priority;
						priority--;
						args.Verbs.Add(verb);
					}
				}
			}
		}

		// Token: 0x06002363 RID: 9059 RVA: 0x000B8DBC File Offset: 0x000B6FBC
		private void OnAfterInteract(EntityUid uid, SolutionTransferComponent component, AfterInteractEvent args)
		{
			if (!args.CanReach || args.Target == null)
			{
				return;
			}
			EntityUid target = args.Target.Value;
			Solution targetDrain;
			RefillableSolutionComponent refillComp;
			Solution ownerRefill;
			if (component.CanReceive && !this.EntityManager.HasComponent<RefillableSolutionComponent>(target) && this._solutionContainerSystem.TryGetDrainableSolution(target, out targetDrain, null, null) && this.EntityManager.TryGetComponent<RefillableSolutionComponent>(uid, ref refillComp) && this._solutionContainerSystem.TryGetRefillableSolution(uid, out ownerRefill, null, refillComp))
			{
				FixedPoint2 transferAmount = component.TransferAmount;
				RefillableSolutionComponent refill;
				if (this.EntityManager.TryGetComponent<RefillableSolutionComponent>(uid, ref refill) && refill.MaxRefill != null)
				{
					transferAmount = FixedPoint2.Min(transferAmount, refill.MaxRefill.Value);
				}
				FixedPoint2 transferred = this.Transfer(args.User, target, targetDrain, uid, ownerRefill, transferAmount);
				if (transferred > 0)
				{
					string msg = (ownerRefill.AvailableVolume == 0) ? "comp-solution-transfer-fill-fully" : "comp-solution-transfer-fill-normal";
					this._popupSystem.PopupEntity(Loc.GetString(msg, new ValueTuple<string, object>[]
					{
						new ValueTuple<string, object>("owner", args.Target),
						new ValueTuple<string, object>("amount", transferred),
						new ValueTuple<string, object>("target", uid)
					}), uid, args.User, PopupType.Small);
					args.Handled = true;
					return;
				}
			}
			Solution targetRefill;
			Solution ownerDrain;
			if (component.CanSend && this._solutionContainerSystem.TryGetRefillableSolution(target, out targetRefill, null, null) && this._solutionContainerSystem.TryGetDrainableSolution(uid, out ownerDrain, null, null))
			{
				FixedPoint2 transferAmount2 = component.TransferAmount;
				RefillableSolutionComponent refill2;
				if (this.EntityManager.TryGetComponent<RefillableSolutionComponent>(target, ref refill2) && refill2.MaxRefill != null)
				{
					transferAmount2 = FixedPoint2.Min(transferAmount2, refill2.MaxRefill.Value);
				}
				FixedPoint2 transferred2 = this.Transfer(args.User, uid, ownerDrain, target, targetRefill, transferAmount2);
				if (transferred2 > 0)
				{
					string message = Loc.GetString("comp-solution-transfer-transfer-solution", new ValueTuple<string, object>[]
					{
						new ValueTuple<string, object>("amount", transferred2),
						new ValueTuple<string, object>("target", target)
					});
					this._popupSystem.PopupEntity(message, uid, args.User, PopupType.Small);
					args.Handled = true;
				}
			}
		}

		// Token: 0x06002364 RID: 9060 RVA: 0x000B9034 File Offset: 0x000B7234
		public FixedPoint2 Transfer(EntityUid user, EntityUid sourceEntity, Solution source, EntityUid targetEntity, Solution target, FixedPoint2 amount)
		{
			SolutionTransferAttemptEvent transferAttempt = new SolutionTransferAttemptEvent(sourceEntity, targetEntity);
			base.RaiseLocalEvent<SolutionTransferAttemptEvent>(sourceEntity, transferAttempt, true);
			if (transferAttempt.Cancelled)
			{
				this._popupSystem.PopupEntity(transferAttempt.CancelReason, sourceEntity, user, PopupType.Small);
				return FixedPoint2.Zero;
			}
			if (source.Volume == 0)
			{
				this._popupSystem.PopupEntity(Loc.GetString("comp-solution-transfer-is-empty", new ValueTuple<string, object>[]
				{
					new ValueTuple<string, object>("target", sourceEntity)
				}), sourceEntity, user, PopupType.Small);
				return FixedPoint2.Zero;
			}
			base.RaiseLocalEvent<SolutionTransferAttemptEvent>(targetEntity, transferAttempt, true);
			if (transferAttempt.Cancelled)
			{
				this._popupSystem.PopupEntity(transferAttempt.CancelReason, sourceEntity, user, PopupType.Small);
				return FixedPoint2.Zero;
			}
			if (target.AvailableVolume == 0)
			{
				this._popupSystem.PopupEntity(Loc.GetString("comp-solution-transfer-is-full", new ValueTuple<string, object>[]
				{
					new ValueTuple<string, object>("target", targetEntity)
				}), targetEntity, user, PopupType.Small);
				return FixedPoint2.Zero;
			}
			FixedPoint2 actualAmount = FixedPoint2.Min(amount, FixedPoint2.Min(source.Volume, target.AvailableVolume));
			Solution solution = this._solutionContainerSystem.Drain(sourceEntity, source, actualAmount, null);
			this._solutionContainerSystem.Refill(targetEntity, target, solution, null);
			ISharedAdminLogManager adminLogger = this._adminLogger;
			LogType type = LogType.Action;
			LogImpact impact = LogImpact.Medium;
			LogStringHandler logStringHandler = new LogStringHandler(38, 4);
			logStringHandler.AppendFormatted<EntityStringRepresentation>(this.EntityManager.ToPrettyString(user), "player", "EntityManager.ToPrettyString(user)");
			logStringHandler.AppendLiteral(" transferred ");
			logStringHandler.AppendFormatted(string.Join<Solution.ReagentQuantity>(", ", solution.Contents));
			logStringHandler.AppendLiteral(" to ");
			logStringHandler.AppendFormatted<EntityStringRepresentation>(this.EntityManager.ToPrettyString(targetEntity), "entity", "EntityManager.ToPrettyString(targetEntity)");
			logStringHandler.AppendLiteral(", which now contains ");
			logStringHandler.AppendFormatted(string.Join<Solution.ReagentQuantity>(", ", target.Contents));
			adminLogger.Add(type, impact, ref logStringHandler);
			return actualAmount;
		}

		// Token: 0x040015C7 RID: 5575
		[Dependency]
		private readonly SharedPopupSystem _popupSystem;

		// Token: 0x040015C8 RID: 5576
		[Dependency]
		private readonly SolutionContainerSystem _solutionContainerSystem;

		// Token: 0x040015C9 RID: 5577
		[Dependency]
		private readonly UserInterfaceSystem _userInterfaceSystem;

		// Token: 0x040015CA RID: 5578
		[Dependency]
		private readonly IAdminLogManager _adminLogger;

		// Token: 0x040015CB RID: 5579
		public static readonly List<int> DefaultTransferAmounts = new List<int>
		{
			1,
			5,
			10,
			25,
			50,
			100,
			250,
			500,
			1000
		};
	}
}
