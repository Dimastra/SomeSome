using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Content.Server.Chat.Systems;
using Content.Server.Interaction;
using Content.Server.Popups;
using Content.Server.Power.Components;
using Content.Server.Power.EntitySystems;
using Content.Server.Radio.Components;
using Content.Server.Speech;
using Content.Server.Speech.Components;
using Content.Shared.Examine;
using Content.Shared.Interaction;
using Content.Shared.Physics;
using Content.Shared.Popups;
using Content.Shared.Radio;
using Content.Shared.Verbs;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Localization;
using Robust.Shared.Prototypes;

namespace Content.Server.Radio.EntitySystems
{
	// Token: 0x0200025B RID: 603
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class RadioDeviceSystem : EntitySystem
	{
		// Token: 0x06000BEC RID: 3052 RVA: 0x0003ED24 File Offset: 0x0003CF24
		public override void Initialize()
		{
			base.Initialize();
			base.SubscribeLocalEvent<RadioMicrophoneComponent, ComponentInit>(new ComponentEventHandler<RadioMicrophoneComponent, ComponentInit>(this.OnMicrophoneInit), null, null);
			base.SubscribeLocalEvent<RadioMicrophoneComponent, ExaminedEvent>(new ComponentEventHandler<RadioMicrophoneComponent, ExaminedEvent>(this.OnExamine), null, null);
			base.SubscribeLocalEvent<RadioMicrophoneComponent, ActivateInWorldEvent>(new ComponentEventHandler<RadioMicrophoneComponent, ActivateInWorldEvent>(this.OnActivateMicrophone), null, null);
			base.SubscribeLocalEvent<RadioMicrophoneComponent, ListenEvent>(new ComponentEventHandler<RadioMicrophoneComponent, ListenEvent>(this.OnListen), null, null);
			base.SubscribeLocalEvent<RadioMicrophoneComponent, ListenAttemptEvent>(new ComponentEventHandler<RadioMicrophoneComponent, ListenAttemptEvent>(this.OnAttemptListen), null, null);
			base.SubscribeLocalEvent<RadioMicrophoneComponent, GetVerbsEvent<Verb>>(new ComponentEventHandler<RadioMicrophoneComponent, GetVerbsEvent<Verb>>(this.OnGetVerbs), null, null);
			base.SubscribeLocalEvent<RadioMicrophoneComponent, PowerChangedEvent>(new ComponentEventRefHandler<RadioMicrophoneComponent, PowerChangedEvent>(this.OnPowerChanged), null, null);
			base.SubscribeLocalEvent<RadioSpeakerComponent, ComponentInit>(new ComponentEventHandler<RadioSpeakerComponent, ComponentInit>(this.OnSpeakerInit), null, null);
			base.SubscribeLocalEvent<RadioSpeakerComponent, ActivateInWorldEvent>(new ComponentEventHandler<RadioSpeakerComponent, ActivateInWorldEvent>(this.OnActivateSpeaker), null, null);
			base.SubscribeLocalEvent<RadioSpeakerComponent, RadioReceiveEvent>(new ComponentEventHandler<RadioSpeakerComponent, RadioReceiveEvent>(this.OnReceiveRadio), null, null);
		}

		// Token: 0x06000BED RID: 3053 RVA: 0x0003EDFF File Offset: 0x0003CFFF
		public override void Update(float frameTime)
		{
			base.Update(frameTime);
			this._recentlySent.Clear();
		}

		// Token: 0x06000BEE RID: 3054 RVA: 0x0003EE13 File Offset: 0x0003D013
		private void OnMicrophoneInit(EntityUid uid, RadioMicrophoneComponent component, ComponentInit args)
		{
			if (component.Enabled)
			{
				base.EnsureComp<ActiveListenerComponent>(uid).Range = (float)component.ListenRange;
				return;
			}
			base.RemCompDeferred<ActiveListenerComponent>(uid);
		}

		// Token: 0x06000BEF RID: 3055 RVA: 0x0003EE39 File Offset: 0x0003D039
		private void OnSpeakerInit(EntityUid uid, RadioSpeakerComponent component, ComponentInit args)
		{
			if (component.Enabled)
			{
				base.EnsureComp<ActiveRadioComponent>(uid).Channels.UnionWith(component.Channels);
				return;
			}
			base.RemCompDeferred<ActiveRadioComponent>(uid);
		}

		// Token: 0x06000BF0 RID: 3056 RVA: 0x0003EE63 File Offset: 0x0003D063
		private void OnActivateMicrophone(EntityUid uid, RadioMicrophoneComponent component, ActivateInWorldEvent args)
		{
			this.ToggleRadioMicrophone(uid, args.User, args.Handled, component);
			args.Handled = true;
		}

		// Token: 0x06000BF1 RID: 3057 RVA: 0x0003EE80 File Offset: 0x0003D080
		private void OnActivateSpeaker(EntityUid uid, RadioSpeakerComponent component, ActivateInWorldEvent args)
		{
			this.ToggleRadioSpeaker(uid, args.User, args.Handled, component);
			args.Handled = true;
		}

		// Token: 0x06000BF2 RID: 3058 RVA: 0x0003EEA0 File Offset: 0x0003D0A0
		[NullableContext(2)]
		public void ToggleRadioMicrophone(EntityUid uid, EntityUid user, bool quiet = false, RadioMicrophoneComponent component = null)
		{
			if (!base.Resolve<RadioMicrophoneComponent>(uid, ref component, true))
			{
				return;
			}
			if (component.PowerRequired && !this.IsPowered(uid, this.EntityManager, null))
			{
				return;
			}
			this.SetMicrophoneEnabled(uid, !component.Enabled, component);
			if (!quiet)
			{
				string state = Loc.GetString(component.Enabled ? "handheld-radio-component-on-state" : "handheld-radio-component-off-state");
				string message = Loc.GetString("handheld-radio-component-on-use", new ValueTuple<string, object>[]
				{
					new ValueTuple<string, object>("radioState", state)
				});
				this._popup.PopupEntity(message, user, user, PopupType.Small);
			}
			if (component.Enabled)
			{
				base.EnsureComp<ActiveListenerComponent>(uid).Range = (float)component.ListenRange;
				return;
			}
			base.RemCompDeferred<ActiveListenerComponent>(uid);
		}

		// Token: 0x06000BF3 RID: 3059 RVA: 0x0003EF60 File Offset: 0x0003D160
		private void OnGetVerbs(EntityUid uid, RadioMicrophoneComponent component, GetVerbsEvent<Verb> args)
		{
			if (!args.CanAccess || !args.CanInteract || args.Hands == null)
			{
				return;
			}
			if (component.SupportedChannels == null || component.SupportedChannels.Count <= 1)
			{
				return;
			}
			if (component.PowerRequired && !this.IsPowered(uid, this.EntityManager, null))
			{
				return;
			}
			using (List<string>.Enumerator enumerator = component.SupportedChannels.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					string channel = enumerator.Current;
					RadioChannelPrototype proto = this._protoMan.Index<RadioChannelPrototype>(channel);
					Verb v = new Verb
					{
						Text = proto.LocalizedName,
						Priority = 1,
						Category = VerbCategory.ChannelSelect,
						Disabled = (component.BroadcastChannel == channel),
						DoContactInteraction = new bool?(true),
						Act = delegate()
						{
							component.BroadcastChannel = channel;
							this._popup.PopupEntity(Loc.GetString("handheld-radio-component-channel-set", new ValueTuple<string, object>[]
							{
								new ValueTuple<string, object>("channel", channel)
							}), uid, args.User, PopupType.Small);
						}
					};
					args.Verbs.Add(v);
				}
			}
		}

		// Token: 0x06000BF4 RID: 3060 RVA: 0x0003F0E8 File Offset: 0x0003D2E8
		private void OnPowerChanged(EntityUid uid, RadioMicrophoneComponent component, ref PowerChangedEvent args)
		{
			if (args.Powered)
			{
				return;
			}
			this.SetMicrophoneEnabled(uid, false, component);
		}

		// Token: 0x06000BF5 RID: 3061 RVA: 0x0003F0FC File Offset: 0x0003D2FC
		[NullableContext(2)]
		public void SetMicrophoneEnabled(EntityUid uid, bool enabled, RadioMicrophoneComponent component = null)
		{
			if (!base.Resolve<RadioMicrophoneComponent>(uid, ref component, false))
			{
				return;
			}
			component.Enabled = enabled;
			this._appearance.SetData(uid, RadioDeviceVisuals.Broadcasting, component.Enabled, null);
		}

		// Token: 0x06000BF6 RID: 3062 RVA: 0x0003F130 File Offset: 0x0003D330
		[NullableContext(2)]
		public void ToggleRadioSpeaker(EntityUid uid, EntityUid user, bool quiet = false, RadioSpeakerComponent component = null)
		{
			if (!base.Resolve<RadioSpeakerComponent>(uid, ref component, true))
			{
				return;
			}
			component.Enabled = !component.Enabled;
			if (!quiet)
			{
				string state = Loc.GetString(component.Enabled ? "handheld-radio-component-on-state" : "handheld-radio-component-off-state");
				string message = Loc.GetString("handheld-radio-component-on-use", new ValueTuple<string, object>[]
				{
					new ValueTuple<string, object>("radioState", state)
				});
				this._popup.PopupEntity(message, user, user, PopupType.Small);
			}
			if (component.Enabled)
			{
				base.EnsureComp<ActiveRadioComponent>(uid).Channels.UnionWith(component.Channels);
				return;
			}
			base.RemCompDeferred<ActiveRadioComponent>(uid);
		}

		// Token: 0x06000BF7 RID: 3063 RVA: 0x0003F1D8 File Offset: 0x0003D3D8
		private void OnExamine(EntityUid uid, RadioMicrophoneComponent component, ExaminedEvent args)
		{
			if (!args.IsInDetailsRange)
			{
				return;
			}
			RadioChannelPrototype proto = this._protoMan.Index<RadioChannelPrototype>(component.BroadcastChannel);
			args.PushMarkup(Loc.GetString("handheld-radio-component-on-examine", new ValueTuple<string, object>[]
			{
				new ValueTuple<string, object>("frequency", proto.Frequency)
			}));
			args.PushMarkup(Loc.GetString("handheld-radio-component-chennel-examine", new ValueTuple<string, object>[]
			{
				new ValueTuple<string, object>("channel", proto.LocalizedName)
			}));
		}

		// Token: 0x06000BF8 RID: 3064 RVA: 0x0003F260 File Offset: 0x0003D460
		private void OnListen(EntityUid uid, RadioMicrophoneComponent component, ListenEvent args)
		{
			if (base.HasComp<RadioSpeakerComponent>(args.Source))
			{
				return;
			}
			if (this._recentlySent.Add(new ValueTuple<string, EntityUid>(args.Message, args.Source)))
			{
				this._radio.SendRadioMessage(args.Source, args.Message, this._protoMan.Index<RadioChannelPrototype>(component.BroadcastChannel), new EntityUid?(uid));
			}
		}

		// Token: 0x06000BF9 RID: 3065 RVA: 0x0003F2C8 File Offset: 0x0003D4C8
		private void OnAttemptListen(EntityUid uid, RadioMicrophoneComponent component, ListenAttemptEvent args)
		{
			if ((component.PowerRequired && !this.IsPowered(uid, this.EntityManager, null)) || (component.UnobstructedRequired && !this._interaction.InRangeUnobstructed(args.Source, uid, 0f, CollisionGroup.Impassable | CollisionGroup.InteractImpassable, null, false)))
			{
				args.Cancel();
			}
		}

		// Token: 0x06000BFA RID: 3066 RVA: 0x0003F31C File Offset: 0x0003D51C
		private void OnReceiveRadio(EntityUid uid, RadioSpeakerComponent component, RadioReceiveEvent args)
		{
			TransformSpeakerNameEvent nameEv = new TransformSpeakerNameEvent(args.Source, base.Name(args.Source, null));
			base.RaiseLocalEvent<TransformSpeakerNameEvent>(args.Source, nameEv, false);
			string name = Loc.GetString("speech-name-relay", new ValueTuple<string, object>[]
			{
				new ValueTuple<string, object>("speaker", base.Name(uid, null)),
				new ValueTuple<string, object>("originalName", nameEv.Name)
			});
			bool hideGlobalGhostChat = true;
			ChatSystem chat = this._chat;
			string message = args.Message;
			InGameICChatType desiredType = InGameICChatType.Speak;
			bool hideChat = false;
			string nameOverride = name;
			chat.TrySendInGameICMessage(uid, message, desiredType, hideChat, hideGlobalGhostChat, null, null, nameOverride, false, false);
		}

		// Token: 0x04000778 RID: 1912
		[Dependency]
		private readonly IPrototypeManager _protoMan;

		// Token: 0x04000779 RID: 1913
		[Dependency]
		private readonly PopupSystem _popup;

		// Token: 0x0400077A RID: 1914
		[Dependency]
		private readonly ChatSystem _chat;

		// Token: 0x0400077B RID: 1915
		[Dependency]
		private readonly RadioSystem _radio;

		// Token: 0x0400077C RID: 1916
		[Dependency]
		private readonly InteractionSystem _interaction;

		// Token: 0x0400077D RID: 1917
		[Dependency]
		private readonly SharedAppearanceSystem _appearance;

		// Token: 0x0400077E RID: 1918
		[Nullable(new byte[]
		{
			1,
			0,
			1
		})]
		private HashSet<ValueTuple<string, EntityUid>> _recentlySent = new HashSet<ValueTuple<string, EntityUid>>();
	}
}
