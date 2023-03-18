using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Content.Shared.Chat;
using Content.Shared.Examine;
using Content.Shared.Hands.EntitySystems;
using Content.Shared.Interaction;
using Content.Shared.Popups;
using Content.Shared.Radio.Components;
using Content.Shared.Tools;
using Content.Shared.Tools.Components;
using Robust.Shared.Containers;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Localization;
using Robust.Shared.Player;
using Robust.Shared.Prototypes;
using Robust.Shared.Timing;

namespace Content.Shared.Radio.EntitySystems
{
	// Token: 0x02000221 RID: 545
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class EncryptionKeySystem : EntitySystem
	{
		// Token: 0x06000610 RID: 1552 RVA: 0x0001530C File Offset: 0x0001350C
		public override void Initialize()
		{
			base.Initialize();
			base.SubscribeLocalEvent<EncryptionKeyComponent, ExaminedEvent>(new ComponentEventHandler<EncryptionKeyComponent, ExaminedEvent>(this.OnKeyExamined), null, null);
			base.SubscribeLocalEvent<EncryptionKeyHolderComponent, ExaminedEvent>(new ComponentEventHandler<EncryptionKeyHolderComponent, ExaminedEvent>(this.OnHolderExamined), null, null);
			base.SubscribeLocalEvent<EncryptionKeyHolderComponent, ComponentStartup>(new ComponentEventHandler<EncryptionKeyHolderComponent, ComponentStartup>(this.OnStartup), null, null);
			base.SubscribeLocalEvent<EncryptionKeyHolderComponent, InteractUsingEvent>(new ComponentEventHandler<EncryptionKeyHolderComponent, InteractUsingEvent>(this.OnInteractUsing), null, null);
			base.SubscribeLocalEvent<EncryptionKeyHolderComponent, EntInsertedIntoContainerMessage>(new ComponentEventHandler<EncryptionKeyHolderComponent, EntInsertedIntoContainerMessage>(this.OnContainerModified), null, null);
			base.SubscribeLocalEvent<EncryptionKeyHolderComponent, EntRemovedFromContainerMessage>(new ComponentEventHandler<EncryptionKeyHolderComponent, EntRemovedFromContainerMessage>(this.OnContainerModified), null, null);
		}

		// Token: 0x06000611 RID: 1553 RVA: 0x00015398 File Offset: 0x00013598
		public void UpdateChannels(EntityUid uid, EncryptionKeyHolderComponent component)
		{
			if (!component.Initialized)
			{
				return;
			}
			component.Channels.Clear();
			component.DefaultChannel = null;
			foreach (EntityUid ent in component.KeyContainer.ContainedEntities)
			{
				EncryptionKeyComponent key;
				if (base.TryComp<EncryptionKeyComponent>(ent, ref key))
				{
					component.Channels.UnionWith(key.Channels);
					if (component.DefaultChannel == null)
					{
						component.DefaultChannel = key.DefaultChannel;
					}
				}
			}
			base.RaiseLocalEvent<EncryptionChannelsChangedEvent>(uid, new EncryptionChannelsChangedEvent(component), false);
		}

		// Token: 0x06000612 RID: 1554 RVA: 0x00015440 File Offset: 0x00013640
		private void OnContainerModified(EntityUid uid, EncryptionKeyHolderComponent component, ContainerModifiedMessage args)
		{
			if (args.Container.ID == "key_slots")
			{
				this.UpdateChannels(uid, component);
			}
		}

		// Token: 0x06000613 RID: 1555 RVA: 0x00015464 File Offset: 0x00013664
		private void OnInteractUsing(EntityUid uid, EncryptionKeyHolderComponent component, InteractUsingEvent args)
		{
			ContainerManagerComponent storage;
			if (!base.TryComp<ContainerManagerComponent>(uid, ref storage))
			{
				return;
			}
			EncryptionKeyComponent key;
			if (base.TryComp<EncryptionKeyComponent>(args.Used, ref key))
			{
				args.Handled = true;
				if (!component.KeysUnlocked)
				{
					if (this._timing.IsFirstTimePredicted)
					{
						this._popupSystem.PopupEntity(Loc.GetString("headset-encryption-keys-are-locked"), uid, Filter.Local(), false, PopupType.Small);
					}
					return;
				}
				if (component.KeySlots <= component.KeyContainer.ContainedEntities.Count)
				{
					if (this._timing.IsFirstTimePredicted)
					{
						this._popupSystem.PopupEntity(Loc.GetString("headset-encryption-key-slots-already-full"), uid, Filter.Local(), false, PopupType.Small);
					}
					return;
				}
				if (component.KeyContainer.Insert(args.Used, null, null, null, null, null))
				{
					if (this._timing.IsFirstTimePredicted)
					{
						this._popupSystem.PopupEntity(Loc.GetString("headset-encryption-key-successfully-installed"), uid, Filter.Local(), false, PopupType.Small);
					}
					this._audio.PlayPredicted(component.KeyInsertionSound, args.Target, new EntityUid?(args.User), null);
					return;
				}
			}
			ToolComponent tool;
			if (!base.TryComp<ToolComponent>(args.Used, ref tool) || !tool.Qualities.Contains(component.KeysExtractionMethod))
			{
				return;
			}
			args.Handled = true;
			if (component.KeyContainer.ContainedEntities.Count == 0)
			{
				if (this._timing.IsFirstTimePredicted)
				{
					this._popupSystem.PopupEntity(Loc.GetString("headset-encryption-keys-no-keys"), uid, Filter.Local(), false, PopupType.Small);
				}
				return;
			}
			ToolEventData toolEvData = new ToolEventData(null, 0f, null, null);
			if (!this._toolSystem.UseTool(args.Used, args.User, new EntityUid?(uid), 0f, new string[]
			{
				component.KeysExtractionMethod
			}, toolEvData, 0f, tool, null, null))
			{
				return;
			}
			EntityUid[] array = component.KeyContainer.ContainedEntities.ToArray<EntityUid>();
			SharedContainerSystem container = this._container;
			IContainer keyContainer = component.KeyContainer;
			bool flag = false;
			IEntityManager entityManager = this.EntityManager;
			container.EmptyContainer(keyContainer, flag, null, false, entityManager);
			foreach (EntityUid ent in array)
			{
				this._hands.PickupOrDrop(new EntityUid?(args.User), ent, true, false, null, null);
			}
			this._popupSystem.PopupEntity(Loc.GetString("headset-encryption-keys-all-extracted"), uid, args.User, PopupType.Small);
			this._audio.PlayPvs(component.KeyExtractionSound, args.Target, null);
		}

		// Token: 0x06000614 RID: 1556 RVA: 0x000156EB File Offset: 0x000138EB
		private void OnStartup(EntityUid uid, EncryptionKeyHolderComponent component, ComponentStartup args)
		{
			component.KeyContainer = this._container.EnsureContainer<Container>(uid, "key_slots", null);
			this.UpdateChannels(uid, component);
		}

		// Token: 0x06000615 RID: 1557 RVA: 0x00015710 File Offset: 0x00013910
		private void OnHolderExamined(EntityUid uid, EncryptionKeyHolderComponent component, ExaminedEvent args)
		{
			if (!args.IsInDetailsRange)
			{
				return;
			}
			if (component.KeyContainer.ContainedEntities.Count == 0)
			{
				args.PushMarkup(Loc.GetString("examine-headset-no-keys"));
				return;
			}
			if (component.Channels.Count > 0)
			{
				args.PushMarkup(Loc.GetString("examine-headset-channels-prefix"));
				this.AddChannelsExamine(component.Channels, component.DefaultChannel, args, this._protoManager, "examine-headset-channel");
			}
		}

		// Token: 0x06000616 RID: 1558 RVA: 0x00015788 File Offset: 0x00013988
		private void OnKeyExamined(EntityUid uid, EncryptionKeyComponent component, ExaminedEvent args)
		{
			if (!args.IsInDetailsRange)
			{
				return;
			}
			if (component.Channels.Count > 0)
			{
				args.PushMarkup(Loc.GetString("examine-encryption-key-channels-prefix"));
				this.AddChannelsExamine(component.Channels, component.DefaultChannel, args, this._protoManager, "examine-headset-channel");
			}
		}

		// Token: 0x06000617 RID: 1559 RVA: 0x000157DC File Offset: 0x000139DC
		public void AddChannelsExamine(HashSet<string> channels, [Nullable(2)] string defaultChannel, ExaminedEvent examineEvent, IPrototypeManager protoManager, string channelFTLPattern)
		{
			RadioChannelPrototype proto;
			foreach (string id in channels)
			{
				proto = protoManager.Index<RadioChannelPrototype>(id);
				string key = (id == "Common") ? ';'.ToString() : (":" + string.Join<char>(", :", proto.KeyCodes.ToArray()));
				examineEvent.PushMarkup(Loc.GetString(channelFTLPattern, new ValueTuple<string, object>[]
				{
					new ValueTuple<string, object>("color", proto.Color),
					new ValueTuple<string, object>("keys", key),
					new ValueTuple<string, object>("id", proto.LocalizedName),
					new ValueTuple<string, object>("freq", proto.Frequency)
				}));
			}
			if (defaultChannel != null && this._protoManager.TryIndex<RadioChannelPrototype>(defaultChannel, ref proto))
			{
				string msg = Loc.GetString("examine-default-channel", new ValueTuple<string, object>[]
				{
					new ValueTuple<string, object>("prefix", SharedChatSystem.DefaultChannelPrefix),
					new ValueTuple<string, object>("channel", defaultChannel),
					new ValueTuple<string, object>("color", proto.Color)
				});
				examineEvent.PushMarkup(msg);
			}
		}

		// Token: 0x0400060C RID: 1548
		[Dependency]
		private readonly IPrototypeManager _protoManager;

		// Token: 0x0400060D RID: 1549
		[Dependency]
		private readonly IGameTiming _timing;

		// Token: 0x0400060E RID: 1550
		[Dependency]
		private readonly SharedToolSystem _toolSystem;

		// Token: 0x0400060F RID: 1551
		[Dependency]
		private readonly SharedPopupSystem _popupSystem;

		// Token: 0x04000610 RID: 1552
		[Dependency]
		private readonly SharedContainerSystem _container;

		// Token: 0x04000611 RID: 1553
		[Dependency]
		private readonly SharedAudioSystem _audio;

		// Token: 0x04000612 RID: 1554
		[Dependency]
		private readonly SharedHandsSystem _hands;
	}
}
