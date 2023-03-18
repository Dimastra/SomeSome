using System;
using System.Runtime.CompilerServices;
using Content.Server.Labels.Components;
using Content.Server.Paper;
using Content.Shared.Containers.ItemSlots;
using Content.Shared.Examine;
using Content.Shared.Labels;
using Robust.Shared.Containers;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Localization;
using Robust.Shared.Utility;

namespace Content.Server.Labels
{
	// Token: 0x02000426 RID: 1062
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class LabelSystem : EntitySystem
	{
		// Token: 0x0600157F RID: 5503 RVA: 0x00070E2C File Offset: 0x0006F02C
		public override void Initialize()
		{
			base.Initialize();
			base.SubscribeLocalEvent<LabelComponent, ExaminedEvent>(new ComponentEventHandler<LabelComponent, ExaminedEvent>(this.OnExamine), null, null);
			base.SubscribeLocalEvent<PaperLabelComponent, ComponentInit>(new ComponentEventHandler<PaperLabelComponent, ComponentInit>(this.OnComponentInit), null, null);
			base.SubscribeLocalEvent<PaperLabelComponent, ComponentRemove>(new ComponentEventHandler<PaperLabelComponent, ComponentRemove>(this.OnComponentRemove), null, null);
			base.SubscribeLocalEvent<PaperLabelComponent, EntInsertedIntoContainerMessage>(new ComponentEventHandler<PaperLabelComponent, EntInsertedIntoContainerMessage>(this.OnContainerModified), null, null);
			base.SubscribeLocalEvent<PaperLabelComponent, EntRemovedFromContainerMessage>(new ComponentEventHandler<PaperLabelComponent, EntRemovedFromContainerMessage>(this.OnContainerModified), null, null);
			base.SubscribeLocalEvent<PaperLabelComponent, ExaminedEvent>(new ComponentEventHandler<PaperLabelComponent, ExaminedEvent>(this.OnExamined), null, null);
		}

		// Token: 0x06001580 RID: 5504 RVA: 0x00070EB8 File Offset: 0x0006F0B8
		[NullableContext(2)]
		public void Label(EntityUid uid, string text, MetaDataComponent metadata = null, LabelComponent label = null)
		{
			if (!base.Resolve<MetaDataComponent>(uid, ref metadata, true))
			{
				return;
			}
			if (!base.Resolve<LabelComponent>(uid, ref label, false))
			{
				label = base.EnsureComp<LabelComponent>(uid);
			}
			if (!string.IsNullOrEmpty(text))
			{
				LabelComponent labelComponent = label;
				if (labelComponent.OriginalName == null)
				{
					labelComponent.OriginalName = metadata.EntityName;
				}
				label.CurrentLabel = text;
				metadata.EntityName = label.OriginalName + " (" + text + ")";
				return;
			}
			if (label.OriginalName == null)
			{
				return;
			}
			metadata.EntityName = label.OriginalName;
			label.CurrentLabel = null;
			label.OriginalName = null;
		}

		// Token: 0x06001581 RID: 5505 RVA: 0x00070F58 File Offset: 0x0006F158
		private void OnComponentInit(EntityUid uid, PaperLabelComponent component, ComponentInit args)
		{
			this._itemSlotsSystem.AddItemSlot(uid, "paper_label", component.LabelSlot, null);
			AppearanceComponent appearance;
			if (!this.EntityManager.TryGetComponent<AppearanceComponent>(uid, ref appearance))
			{
				return;
			}
			this._appearance.SetData(uid, PaperLabelVisuals.HasLabel, false, appearance);
		}

		// Token: 0x06001582 RID: 5506 RVA: 0x00070FA7 File Offset: 0x0006F1A7
		private void OnComponentRemove(EntityUid uid, PaperLabelComponent component, ComponentRemove args)
		{
			this._itemSlotsSystem.RemoveItemSlot(uid, component.LabelSlot, null);
		}

		// Token: 0x06001583 RID: 5507 RVA: 0x00070FBC File Offset: 0x0006F1BC
		private void OnExamine(EntityUid uid, [Nullable(2)] LabelComponent label, ExaminedEvent args)
		{
			if (!base.Resolve<LabelComponent>(uid, ref label, true))
			{
				return;
			}
			if (label.CurrentLabel == null)
			{
				return;
			}
			FormattedMessage message = new FormattedMessage();
			message.AddText(Loc.GetString("hand-labeler-has-label", new ValueTuple<string, object>[]
			{
				new ValueTuple<string, object>("label", label.CurrentLabel)
			}));
			args.PushMessage(message);
		}

		// Token: 0x06001584 RID: 5508 RVA: 0x0007101C File Offset: 0x0006F21C
		private void OnExamined(EntityUid uid, PaperLabelComponent comp, ExaminedEvent args)
		{
			EntityUid? item2 = comp.LabelSlot.Item;
			if (item2 != null)
			{
				EntityUid item = item2.GetValueOrDefault();
				if (item.Valid)
				{
					if (!args.IsInDetailsRange)
					{
						args.PushMarkup(Loc.GetString("comp-paper-label-has-label-cant-read"));
						return;
					}
					PaperComponent paper;
					if (!this.EntityManager.TryGetComponent<PaperComponent>(item, ref paper))
					{
						return;
					}
					if (string.IsNullOrWhiteSpace(paper.Content))
					{
						args.PushMarkup(Loc.GetString("comp-paper-label-has-label-blank"));
						return;
					}
					args.PushMarkup(Loc.GetString("comp-paper-label-has-label"));
					string text = paper.Content;
					args.PushMarkup(text.TrimEnd());
					return;
				}
			}
		}

		// Token: 0x06001585 RID: 5509 RVA: 0x000710BC File Offset: 0x0006F2BC
		private void OnContainerModified(EntityUid uid, PaperLabelComponent label, ContainerModifiedMessage args)
		{
			if (!label.Initialized)
			{
				return;
			}
			if (args.Container.ID != label.LabelSlot.ID)
			{
				return;
			}
			AppearanceComponent appearance;
			if (!this.EntityManager.TryGetComponent<AppearanceComponent>(uid, ref appearance))
			{
				return;
			}
			this._appearance.SetData(uid, PaperLabelVisuals.HasLabel, label.LabelSlot.HasItem, appearance);
		}

		// Token: 0x04000D5F RID: 3423
		[Dependency]
		private readonly ItemSlotsSystem _itemSlotsSystem;

		// Token: 0x04000D60 RID: 3424
		[Dependency]
		private readonly SharedAppearanceSystem _appearance;

		// Token: 0x04000D61 RID: 3425
		public const string ContainerName = "paper_label";
	}
}
