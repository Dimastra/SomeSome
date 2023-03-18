using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Content.Server.Humanoid;
using Content.Server.UserInterface;
using Content.Shared.Humanoid;
using Content.Shared.Humanoid.Markings;
using Content.Shared.MagicMirror;
using Robust.Server.GameObjects;
using Robust.Server.Player;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Maths;
using Robust.Shared.Players;

namespace Content.Server.MagicMirror
{
	// Token: 0x020003E1 RID: 993
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class MagicMirrorSystem : EntitySystem
	{
		// Token: 0x06001469 RID: 5225 RVA: 0x00069B9C File Offset: 0x00067D9C
		public override void Initialize()
		{
			base.Initialize();
			base.SubscribeLocalEvent<MagicMirrorComponent, ActivatableUIOpenAttemptEvent>(new ComponentEventHandler<MagicMirrorComponent, ActivatableUIOpenAttemptEvent>(this.OnOpenUIAttempt), null, null);
			base.SubscribeLocalEvent<MagicMirrorComponent, AfterActivatableUIOpenEvent>(new ComponentEventHandler<MagicMirrorComponent, AfterActivatableUIOpenEvent>(this.AfterUIOpen), null, null);
			base.SubscribeLocalEvent<MagicMirrorComponent, MagicMirrorSelectMessage>(new ComponentEventHandler<MagicMirrorComponent, MagicMirrorSelectMessage>(this.OnMagicMirrorSelect), null, null);
			base.SubscribeLocalEvent<MagicMirrorComponent, MagicMirrorChangeColorMessage>(new ComponentEventHandler<MagicMirrorComponent, MagicMirrorChangeColorMessage>(this.OnMagicMirrorChangeColor), null, null);
			base.SubscribeLocalEvent<MagicMirrorComponent, MagicMirrorAddSlotMessage>(new ComponentEventHandler<MagicMirrorComponent, MagicMirrorAddSlotMessage>(this.OnMagicMirrorAddSlot), null, null);
			base.SubscribeLocalEvent<MagicMirrorComponent, MagicMirrorRemoveSlotMessage>(new ComponentEventHandler<MagicMirrorComponent, MagicMirrorRemoveSlotMessage>(this.OnMagicMirrorRemoveSlot), null, null);
		}

		// Token: 0x0600146A RID: 5226 RVA: 0x00069C27 File Offset: 0x00067E27
		private void OnOpenUIAttempt(EntityUid uid, MagicMirrorComponent mirror, ActivatableUIOpenAttemptEvent args)
		{
			if (!base.HasComp<HumanoidAppearanceComponent>(args.User))
			{
				args.Cancel();
			}
		}

		// Token: 0x0600146B RID: 5227 RVA: 0x00069C40 File Offset: 0x00067E40
		private void OnMagicMirrorSelect(EntityUid uid, MagicMirrorComponent component, MagicMirrorSelectMessage message)
		{
			HumanoidAppearanceComponent humanoid;
			if (message.Session.AttachedEntity == null || !base.TryComp<HumanoidAppearanceComponent>(message.Session.AttachedEntity.Value, ref humanoid))
			{
				return;
			}
			MagicMirrorCategory category2 = message.Category;
			MarkingCategories category;
			if (category2 != MagicMirrorCategory.Hair)
			{
				if (category2 != MagicMirrorCategory.FacialHair)
				{
					return;
				}
				category = MarkingCategories.FacialHair;
			}
			else
			{
				category = MarkingCategories.Hair;
			}
			this._humanoid.SetMarkingId(message.Session.AttachedEntity.Value, category, message.Slot, message.Marking, null);
			this.UpdateInterface(uid, message.Session.AttachedEntity.Value, message.Session, null);
		}

		// Token: 0x0600146C RID: 5228 RVA: 0x00069CE4 File Offset: 0x00067EE4
		private void OnMagicMirrorChangeColor(EntityUid uid, MagicMirrorComponent component, MagicMirrorChangeColorMessage message)
		{
			HumanoidAppearanceComponent humanoid;
			if (message.Session.AttachedEntity == null || !base.TryComp<HumanoidAppearanceComponent>(message.Session.AttachedEntity.Value, ref humanoid))
			{
				return;
			}
			MagicMirrorCategory category2 = message.Category;
			MarkingCategories category;
			if (category2 != MagicMirrorCategory.Hair)
			{
				if (category2 != MagicMirrorCategory.FacialHair)
				{
					return;
				}
				category = MarkingCategories.FacialHair;
			}
			else
			{
				category = MarkingCategories.Hair;
			}
			this._humanoid.SetMarkingColor(message.Session.AttachedEntity.Value, category, message.Slot, message.Colors, null);
		}

		// Token: 0x0600146D RID: 5229 RVA: 0x00069D68 File Offset: 0x00067F68
		private void OnMagicMirrorRemoveSlot(EntityUid uid, MagicMirrorComponent component, MagicMirrorRemoveSlotMessage message)
		{
			HumanoidAppearanceComponent humanoid;
			if (message.Session.AttachedEntity == null || !base.TryComp<HumanoidAppearanceComponent>(message.Session.AttachedEntity.Value, ref humanoid))
			{
				return;
			}
			MagicMirrorCategory category2 = message.Category;
			MarkingCategories category;
			if (category2 != MagicMirrorCategory.Hair)
			{
				if (category2 != MagicMirrorCategory.FacialHair)
				{
					return;
				}
				category = MarkingCategories.FacialHair;
			}
			else
			{
				category = MarkingCategories.Hair;
			}
			this._humanoid.RemoveMarking(message.Session.AttachedEntity.Value, category, message.Slot, null);
			this.UpdateInterface(uid, message.Session.AttachedEntity.Value, message.Session, null);
		}

		// Token: 0x0600146E RID: 5230 RVA: 0x00069E08 File Offset: 0x00068008
		private void OnMagicMirrorAddSlot(EntityUid uid, MagicMirrorComponent component, MagicMirrorAddSlotMessage message)
		{
			HumanoidAppearanceComponent humanoid;
			if (message.Session.AttachedEntity == null || !base.TryComp<HumanoidAppearanceComponent>(message.Session.AttachedEntity.Value, ref humanoid))
			{
				return;
			}
			MagicMirrorCategory category2 = message.Category;
			MarkingCategories category;
			if (category2 != MagicMirrorCategory.Hair)
			{
				if (category2 != MagicMirrorCategory.FacialHair)
				{
					return;
				}
				category = MarkingCategories.FacialHair;
			}
			else
			{
				category = MarkingCategories.Hair;
			}
			string marking = this._markings.MarkingsByCategoryAndSpecies(category, humanoid.Species).Keys.FirstOrDefault<string>();
			if (string.IsNullOrEmpty(marking))
			{
				return;
			}
			this._humanoid.AddMarking(message.Session.AttachedEntity.Value, marking, new Color?(Color.Black), true, false, null);
			this.UpdateInterface(uid, message.Session.AttachedEntity.Value, message.Session, null);
		}

		// Token: 0x0600146F RID: 5231 RVA: 0x00069ED8 File Offset: 0x000680D8
		private void UpdateInterface(EntityUid uid, EntityUid playerUid, ICommonSession session, [Nullable(2)] HumanoidAppearanceComponent humanoid = null)
		{
			if (base.Resolve<HumanoidAppearanceComponent>(playerUid, ref humanoid, true))
			{
				IPlayerSession player = session as IPlayerSession;
				if (player != null)
				{
					IReadOnlyList<Marking> hairMarkings;
					List<Marking> hair = humanoid.MarkingSet.TryGetCategory(MarkingCategories.Hair, out hairMarkings) ? new List<Marking>(hairMarkings) : new List<Marking>();
					IReadOnlyList<Marking> facialHairMarkings;
					List<Marking> facialHair = humanoid.MarkingSet.TryGetCategory(MarkingCategories.FacialHair, out facialHairMarkings) ? new List<Marking>(facialHairMarkings) : new List<Marking>();
					MagicMirrorUiData msg = new MagicMirrorUiData(humanoid.Species, hair, humanoid.MarkingSet.PointsLeft(MarkingCategories.Hair) + hair.Count, facialHair, humanoid.MarkingSet.PointsLeft(MarkingCategories.FacialHair) + facialHair.Count);
					this._uiSystem.TrySendUiMessage(uid, MagicMirrorUiKey.Key, msg, player, null);
					return;
				}
			}
		}

		// Token: 0x06001470 RID: 5232 RVA: 0x00069F8A File Offset: 0x0006818A
		private void AfterUIOpen(EntityUid uid, MagicMirrorComponent component, AfterActivatableUIOpenEvent args)
		{
			base.Comp<HumanoidAppearanceComponent>(args.User);
			base.Comp<ActorComponent>(args.User);
			this.UpdateInterface(uid, args.User, args.Session, null);
		}

		// Token: 0x04000C98 RID: 3224
		[Dependency]
		private readonly MarkingManager _markings;

		// Token: 0x04000C99 RID: 3225
		[Dependency]
		private readonly HumanoidAppearanceSystem _humanoid;

		// Token: 0x04000C9A RID: 3226
		[Dependency]
		private readonly UserInterfaceSystem _uiSystem;
	}
}
