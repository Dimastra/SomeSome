using System;
using System.Runtime.CompilerServices;
using Content.Server.Chat.Systems;
using Content.Server.Speech;
using Content.Server.Speech.Components;
using Robust.Server.GameObjects;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Maths;

namespace Content.Server.SurveillanceCamera
{
	// Token: 0x02000141 RID: 321
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class SurveillanceCameraMicrophoneSystem : EntitySystem
	{
		// Token: 0x060005F0 RID: 1520 RVA: 0x0001C528 File Offset: 0x0001A728
		public override void Initialize()
		{
			base.Initialize();
			base.SubscribeLocalEvent<SurveillanceCameraMicrophoneComponent, ComponentInit>(new ComponentEventHandler<SurveillanceCameraMicrophoneComponent, ComponentInit>(this.OnInit), null, null);
			base.SubscribeLocalEvent<SurveillanceCameraMicrophoneComponent, ListenEvent>(new ComponentEventHandler<SurveillanceCameraMicrophoneComponent, ListenEvent>(this.RelayEntityMessage), null, null);
			base.SubscribeLocalEvent<SurveillanceCameraMicrophoneComponent, ListenAttemptEvent>(new ComponentEventHandler<SurveillanceCameraMicrophoneComponent, ListenAttemptEvent>(this.CanListen), null, null);
			base.SubscribeLocalEvent<ExpandICChatRecipientstEvent>(new EntityEventHandler<ExpandICChatRecipientstEvent>(this.OnExpandRecipients), null, null);
		}

		// Token: 0x060005F1 RID: 1521 RVA: 0x0001C58C File Offset: 0x0001A78C
		private void OnExpandRecipients(ExpandICChatRecipientstEvent ev)
		{
			EntityQuery<TransformComponent> xformQuery = base.GetEntityQuery<TransformComponent>();
			TransformComponent sourceXform = base.Transform(ev.Source);
			Vector2 sourcePos = this._xforms.GetWorldPosition(sourceXform, xformQuery);
			foreach (ValueTuple<SurveillanceCameraMicrophoneComponent, ActiveListenerComponent, SurveillanceCameraComponent, TransformComponent> valueTuple in base.EntityQuery<SurveillanceCameraMicrophoneComponent, ActiveListenerComponent, SurveillanceCameraComponent, TransformComponent>(false))
			{
				SurveillanceCameraComponent camera = valueTuple.Item3;
				TransformComponent xform = valueTuple.Item4;
				if (camera.ActiveViewers.Count != 0)
				{
					float range = (xform.MapID != sourceXform.MapID) ? -1f : (sourcePos - this._xforms.GetWorldPosition(xform, xformQuery)).Length;
					if (range >= 0f && range <= ev.VoiceRange)
					{
						foreach (EntityUid viewer in camera.ActiveViewers)
						{
							ActorComponent actor;
							if (base.TryComp<ActorComponent>(viewer, ref actor))
							{
								ev.Recipients.TryAdd(actor.PlayerSession, new ChatSystem.ICChatRecipientData(range, false, new bool?(true)));
							}
						}
					}
				}
			}
		}

		// Token: 0x060005F2 RID: 1522 RVA: 0x0001C6D4 File Offset: 0x0001A8D4
		private void OnInit(EntityUid uid, SurveillanceCameraMicrophoneComponent component, ComponentInit args)
		{
			if (component.Enabled)
			{
				base.EnsureComp<ActiveListenerComponent>(uid).Range = (float)component.Range;
				return;
			}
			base.RemCompDeferred<ActiveListenerComponent>(uid);
		}

		// Token: 0x060005F3 RID: 1523 RVA: 0x0001C6FA File Offset: 0x0001A8FA
		public void CanListen(EntityUid uid, SurveillanceCameraMicrophoneComponent microphone, ListenAttemptEvent args)
		{
			if (microphone.Blacklist.IsValid(args.Source, null))
			{
				args.Cancel();
			}
		}

		// Token: 0x060005F4 RID: 1524 RVA: 0x0001C718 File Offset: 0x0001A918
		public void RelayEntityMessage(EntityUid uid, SurveillanceCameraMicrophoneComponent component, ListenEvent args)
		{
			SurveillanceCameraComponent camera;
			if (!base.TryComp<SurveillanceCameraComponent>(uid, ref camera))
			{
				return;
			}
			SurveillanceCameraSpeechSendEvent ev = new SurveillanceCameraSpeechSendEvent(args.Source, args.Message);
			foreach (EntityUid monitor in camera.ActiveMonitors)
			{
				base.RaiseLocalEvent<SurveillanceCameraSpeechSendEvent>(monitor, ev, false);
			}
		}

		// Token: 0x060005F5 RID: 1525 RVA: 0x0001C78C File Offset: 0x0001A98C
		[NullableContext(2)]
		public void SetEnabled(EntityUid uid, bool value, SurveillanceCameraMicrophoneComponent microphone = null)
		{
			if (!base.Resolve<SurveillanceCameraMicrophoneComponent>(uid, ref microphone, true))
			{
				return;
			}
			if (value == microphone.Enabled)
			{
				return;
			}
			microphone.Enabled = value;
			if (value)
			{
				base.EnsureComp<ActiveListenerComponent>(uid).Range = (float)microphone.Range;
				return;
			}
			base.RemCompDeferred<ActiveListenerComponent>(uid);
		}

		// Token: 0x04000386 RID: 902
		[Dependency]
		private readonly SharedTransformSystem _xforms;
	}
}
