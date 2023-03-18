using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Content.Server.Chat.Systems;
using Content.Server.Radio.Components;
using Content.Shared.Inventory.Events;
using Content.Shared.Radio;
using Content.Shared.Radio.Components;
using Content.Shared.Radio.EntitySystems;
using Robust.Server.GameObjects;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Network;

namespace Content.Server.Radio.EntitySystems
{
	// Token: 0x0200025A RID: 602
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class HeadsetSystem : SharedHeadsetSystem
	{
		// Token: 0x06000BE3 RID: 3043 RVA: 0x0003EAF4 File Offset: 0x0003CCF4
		public override void Initialize()
		{
			base.Initialize();
			base.SubscribeLocalEvent<HeadsetComponent, RadioReceiveEvent>(new ComponentEventHandler<HeadsetComponent, RadioReceiveEvent>(this.OnHeadsetReceive), null, null);
			base.SubscribeLocalEvent<HeadsetComponent, EncryptionChannelsChangedEvent>(new ComponentEventHandler<HeadsetComponent, EncryptionChannelsChangedEvent>(this.OnKeysChanged), null, null);
			base.SubscribeLocalEvent<WearingHeadsetComponent, EntitySpokeEvent>(new ComponentEventHandler<WearingHeadsetComponent, EntitySpokeEvent>(this.OnSpeak), null, null);
		}

		// Token: 0x06000BE4 RID: 3044 RVA: 0x0003EB43 File Offset: 0x0003CD43
		private void OnKeysChanged(EntityUid uid, HeadsetComponent component, EncryptionChannelsChangedEvent args)
		{
			this.UpdateRadioChannels(uid, component, args.Component);
		}

		// Token: 0x06000BE5 RID: 3045 RVA: 0x0003EB54 File Offset: 0x0003CD54
		private void UpdateRadioChannels(EntityUid uid, HeadsetComponent headset, [Nullable(2)] EncryptionKeyHolderComponent keyHolder = null)
		{
			if (!headset.Enabled)
			{
				return;
			}
			if (!base.Resolve<EncryptionKeyHolderComponent>(uid, ref keyHolder, true))
			{
				return;
			}
			if (keyHolder.Channels.Count == 0)
			{
				base.RemComp<ActiveRadioComponent>(uid);
				return;
			}
			base.EnsureComp<ActiveRadioComponent>(uid).Channels = new HashSet<string>(keyHolder.Channels);
		}

		// Token: 0x06000BE6 RID: 3046 RVA: 0x0003EBA4 File Offset: 0x0003CDA4
		private void OnSpeak(EntityUid uid, WearingHeadsetComponent component, EntitySpokeEvent args)
		{
			EncryptionKeyHolderComponent keys;
			if (args.Channel != null && base.TryComp<EncryptionKeyHolderComponent>(component.Headset, ref keys) && keys.Channels.Contains(args.Channel.ID))
			{
				this._radio.SendRadioMessage(uid, args.Message, args.Channel, new EntityUid?(component.Headset));
				args.Channel = null;
			}
		}

		// Token: 0x06000BE7 RID: 3047 RVA: 0x0003EC0B File Offset: 0x0003CE0B
		protected override void OnGotEquipped(EntityUid uid, HeadsetComponent component, GotEquippedEvent args)
		{
			base.OnGotEquipped(uid, component, args);
			if (component.IsEquipped && component.Enabled)
			{
				base.EnsureComp<WearingHeadsetComponent>(args.Equipee).Headset = uid;
				this.UpdateRadioChannels(uid, component, null);
			}
		}

		// Token: 0x06000BE8 RID: 3048 RVA: 0x0003EC41 File Offset: 0x0003CE41
		protected override void OnGotUnequipped(EntityUid uid, HeadsetComponent component, GotUnequippedEvent args)
		{
			base.OnGotUnequipped(uid, component, args);
			component.IsEquipped = false;
			base.RemComp<ActiveRadioComponent>(uid);
			base.RemComp<WearingHeadsetComponent>(args.Equipee);
		}

		// Token: 0x06000BE9 RID: 3049 RVA: 0x0003EC68 File Offset: 0x0003CE68
		[NullableContext(2)]
		public void SetEnabled(EntityUid uid, bool value, HeadsetComponent component = null)
		{
			if (!base.Resolve<HeadsetComponent>(uid, ref component, true))
			{
				return;
			}
			if (component.Enabled == value)
			{
				return;
			}
			if (!value)
			{
				base.RemCompDeferred<ActiveRadioComponent>(uid);
				if (component.IsEquipped)
				{
					base.RemCompDeferred<WearingHeadsetComponent>(base.Transform(uid).ParentUid);
					return;
				}
			}
			else if (component.IsEquipped)
			{
				base.EnsureComp<WearingHeadsetComponent>(base.Transform(uid).ParentUid).Headset = uid;
				this.UpdateRadioChannels(uid, component, null);
			}
		}

		// Token: 0x06000BEA RID: 3050 RVA: 0x0003ECDC File Offset: 0x0003CEDC
		private void OnHeadsetReceive(EntityUid uid, HeadsetComponent component, RadioReceiveEvent args)
		{
			ActorComponent actor;
			if (base.TryComp<ActorComponent>(base.Transform(uid).ParentUid, ref actor))
			{
				this._netMan.ServerSendMessage(args.ChatMsg, actor.PlayerSession.ConnectedClient);
			}
		}

		// Token: 0x04000776 RID: 1910
		[Dependency]
		private readonly INetManager _netMan;

		// Token: 0x04000777 RID: 1911
		[Dependency]
		private readonly RadioSystem _radio;
	}
}
