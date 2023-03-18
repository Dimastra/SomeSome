using System;
using System.Runtime.CompilerServices;
using Content.Server.Administration.Logs;
using Content.Server.Hands.Components;
using Content.Server.Hands.Systems;
using Content.Shared.Administration.Logs;
using Content.Shared.Alert;
using Content.Shared.Cuffs.Components;
using Content.Shared.Database;
using Content.Shared.Hands.Components;
using Content.Shared.Hands.EntitySystems;
using Content.Shared.Interaction;
using Content.Shared.Interaction.Components;
using Content.Shared.Physics;
using Content.Shared.Popups;
using Robust.Shared.Audio;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Localization;
using Robust.Shared.Log;
using Robust.Shared.Map;
using Robust.Shared.Maths;
using Robust.Shared.Player;

namespace Content.Server.Cuffs.Components
{
	// Token: 0x020005D5 RID: 1493
	[NullableContext(1)]
	[Nullable(0)]
	[RegisterComponent]
	[ComponentReference(typeof(SharedCuffableComponent))]
	public sealed class CuffableComponent : SharedCuffableComponent
	{
		// Token: 0x06001FC5 RID: 8133 RVA: 0x000A66AE File Offset: 0x000A48AE
		protected override void Initialize()
		{
			base.Initialize();
			ComponentExt.EnsureComponentWarn<HandsComponent>(base.Owner, null);
		}

		// Token: 0x06001FC6 RID: 8134 RVA: 0x000A66C4 File Offset: 0x000A48C4
		public override ComponentState GetComponentState()
		{
			HandcuffComponent cuffs;
			if (base.CuffedHandCount > 0 && this._entMan.TryGetComponent<HandcuffComponent>(base.LastAddedCuffs, ref cuffs))
			{
				int cuffedHandCount = base.CuffedHandCount;
				bool canStillInteract = base.CanStillInteract;
				string cuffedRSI = cuffs.CuffedRSI;
				DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(1, 2);
				defaultInterpolatedStringHandler.AppendFormatted(cuffs.OverlayIconState);
				defaultInterpolatedStringHandler.AppendLiteral("-");
				defaultInterpolatedStringHandler.AppendFormatted<int>(base.CuffedHandCount);
				return new SharedCuffableComponent.CuffableComponentState(cuffedHandCount, canStillInteract, cuffedRSI, defaultInterpolatedStringHandler.ToStringAndClear(), cuffs.Color);
			}
			return new SharedCuffableComponent.CuffableComponentState(base.CuffedHandCount, base.CanStillInteract, "/Objects/Misc/handcuffs.rsi", "body-overlay-2", Color.White);
		}

		// Token: 0x06001FC7 RID: 8135 RVA: 0x000A6764 File Offset: 0x000A4964
		public bool TryAddNewCuffs(EntityUid user, EntityUid handcuff)
		{
			if (!this._entMan.HasComponent<HandcuffComponent>(handcuff))
			{
				Logger.Warning("Handcuffs being applied to player are missing a HandcuffComponent!");
				return false;
			}
			if (!EntitySystem.Get<SharedInteractionSystem>().InRangeUnobstructed(handcuff, base.Owner, 1.5f, CollisionGroup.Impassable | CollisionGroup.InteractImpassable, null, false))
			{
				Logger.Warning("Handcuffs being applied to player are obstructed or too far away! This should not happen!");
				return true;
			}
			this._entMan.EntitySysManager.GetEntitySystem<SharedHandsSystem>().TryDrop(user, handcuff, null, true, true, null);
			base.Container.Insert(handcuff, null, null, null, null, null);
			this.UpdateHeldItems(handcuff);
			return true;
		}

		// Token: 0x06001FC8 RID: 8136 RVA: 0x000A67F4 File Offset: 0x000A49F4
		public void UpdateHeldItems(EntityUid handcuff)
		{
			HandsComponent handsComponent;
			if (!this._entMan.TryGetComponent<HandsComponent>(base.Owner, ref handsComponent))
			{
				return;
			}
			SharedHandsSystem handSys = this._entMan.EntitySysManager.GetEntitySystem<SharedHandsSystem>();
			int freeHands = 0;
			foreach (Hand hand in handSys.EnumerateHands(base.Owner, handsComponent))
			{
				if (hand.HeldEntity == null)
				{
					freeHands++;
				}
				else if (!this._entMan.HasComponent<UnremoveableComponent>(hand.HeldEntity))
				{
					handSys.DoDrop(base.Owner, hand, true, handsComponent);
					freeHands++;
					if (freeHands == 2)
					{
						break;
					}
				}
			}
			HandVirtualItemSystem entitySystem = this._entMan.EntitySysManager.GetEntitySystem<HandVirtualItemSystem>();
			EntityUid? virtItem;
			if (entitySystem.TrySpawnVirtualItemInHand(handcuff, base.Owner, out virtItem))
			{
				this._entMan.EnsureComponent<UnremoveableComponent>(virtItem.Value);
			}
			EntityUid? virtItem2;
			if (entitySystem.TrySpawnVirtualItemInHand(handcuff, base.Owner, out virtItem2))
			{
				this._entMan.EnsureComponent<UnremoveableComponent>(virtItem2.Value);
			}
		}

		// Token: 0x06001FC9 RID: 8137 RVA: 0x000A6910 File Offset: 0x000A4B10
		private void UpdateAlert()
		{
			if (base.CanStillInteract)
			{
				EntitySystem.Get<AlertsSystem>().ClearAlert(base.Owner, AlertType.Handcuffed);
				return;
			}
			EntitySystem.Get<AlertsSystem>().ShowAlert(base.Owner, AlertType.Handcuffed, null, null);
		}

		// Token: 0x06001FCA RID: 8138 RVA: 0x000A695C File Offset: 0x000A4B5C
		public void TryUncuff(EntityUid user, EntityUid? cuffsToRemove = null)
		{
			CuffableComponent.<TryUncuff>d__10 <TryUncuff>d__;
			<TryUncuff>d__.<>t__builder = AsyncVoidMethodBuilder.Create();
			<TryUncuff>d__.<>4__this = this;
			<TryUncuff>d__.user = user;
			<TryUncuff>d__.cuffsToRemove = cuffsToRemove;
			<TryUncuff>d__.<>1__state = -1;
			<TryUncuff>d__.<>t__builder.Start<CuffableComponent.<TryUncuff>d__10>(ref <TryUncuff>d__);
		}

		// Token: 0x06001FCB RID: 8139 RVA: 0x000A69A4 File Offset: 0x000A4BA4
		public void Uncuff(EntityUid user, EntityUid cuffsToRemove, HandcuffComponent cuff, bool isOwner)
		{
			SoundSystem.Play(cuff.EndUncuffSound.GetSound(null, null), Filter.Pvs(base.Owner, 2f, null, null, null), base.Owner, null);
			this._entMan.EntitySysManager.GetEntitySystem<HandVirtualItemSystem>().DeleteInHandsMatching(user, cuffsToRemove);
			base.Container.Remove(cuffsToRemove, null, null, null, true, false, null, null);
			if (cuff.BreakOnRemove)
			{
				this._entMan.QueueDeleteEntity(cuffsToRemove);
				EntityUid trash = this._entMan.SpawnEntity(cuff.BrokenPrototype, MapCoordinates.Nullspace);
				this._entMan.EntitySysManager.GetEntitySystem<SharedHandsSystem>().PickupOrDrop(new EntityUid?(user), trash, true, false, null, null);
			}
			else
			{
				this._entMan.EntitySysManager.GetEntitySystem<SharedHandsSystem>().PickupOrDrop(new EntityUid?(user), cuffsToRemove, true, false, null, null);
			}
			if (base.CuffedHandCount == 0)
			{
				user.PopupMessage(Loc.GetString("cuffable-component-remove-cuffs-success-message"));
				if (!isOwner)
				{
					user.PopupMessage(base.Owner, Loc.GetString("cuffable-component-remove-cuffs-by-other-success-message", new ValueTuple<string, object>[]
					{
						new ValueTuple<string, object>("otherName", user)
					}));
				}
				LogStringHandler logStringHandler;
				if (user == base.Owner)
				{
					ISharedAdminLogManager adminLogger = this._adminLogger;
					LogType type = LogType.Action;
					LogImpact impact = LogImpact.Medium;
					logStringHandler = new LogStringHandler(37, 1);
					logStringHandler.AppendFormatted<EntityStringRepresentation>(this._entMan.ToPrettyString(user), "player", "_entMan.ToPrettyString(user)");
					logStringHandler.AppendLiteral(" has successfully uncuffed themselves");
					adminLogger.Add(type, impact, ref logStringHandler);
					return;
				}
				ISharedAdminLogManager adminLogger2 = this._adminLogger;
				LogType type2 = LogType.Action;
				LogImpact impact2 = LogImpact.Medium;
				logStringHandler = new LogStringHandler(27, 2);
				logStringHandler.AppendFormatted<EntityStringRepresentation>(this._entMan.ToPrettyString(user), "player", "_entMan.ToPrettyString(user)");
				logStringHandler.AppendLiteral(" has successfully uncuffed ");
				logStringHandler.AppendFormatted<EntityStringRepresentation>(this._entMan.ToPrettyString(base.Owner), "player", "_entMan.ToPrettyString(Owner)");
				adminLogger2.Add(type2, impact2, ref logStringHandler);
				return;
			}
			else
			{
				if (!isOwner)
				{
					user.PopupMessage(Loc.GetString("cuffable-component-remove-cuffs-partial-success-message", new ValueTuple<string, object>[]
					{
						new ValueTuple<string, object>("cuffedHandCount", base.CuffedHandCount),
						new ValueTuple<string, object>("otherName", user)
					}));
					user.PopupMessage(base.Owner, Loc.GetString("cuffable-component-remove-cuffs-by-other-partial-success-message", new ValueTuple<string, object>[]
					{
						new ValueTuple<string, object>("otherName", user),
						new ValueTuple<string, object>("cuffedHandCount", base.CuffedHandCount)
					}));
					return;
				}
				user.PopupMessage(Loc.GetString("cuffable-component-remove-cuffs-partial-success-message", new ValueTuple<string, object>[]
				{
					new ValueTuple<string, object>("cuffedHandCount", base.CuffedHandCount)
				}));
				return;
			}
		}

		// Token: 0x040013BB RID: 5051
		[Dependency]
		private readonly IEntityManager _entMan;

		// Token: 0x040013BC RID: 5052
		[Dependency]
		private readonly IEntitySystemManager _sysMan;

		// Token: 0x040013BD RID: 5053
		[Dependency]
		private readonly IComponentFactory _componentFactory;

		// Token: 0x040013BE RID: 5054
		[Dependency]
		private readonly IAdminLogManager _adminLogger;

		// Token: 0x040013BF RID: 5055
		private bool _uncuffing;
	}
}
