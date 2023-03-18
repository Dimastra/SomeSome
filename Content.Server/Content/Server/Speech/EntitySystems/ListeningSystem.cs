using System;
using System.Runtime.CompilerServices;
using Content.Server.Chat.Systems;
using Content.Server.Speech.Components;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Maths;

namespace Content.Server.Speech.EntitySystems
{
	// Token: 0x020001B4 RID: 436
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class ListeningSystem : EntitySystem
	{
		// Token: 0x0600088A RID: 2186 RVA: 0x0002B811 File Offset: 0x00029A11
		public override void Initialize()
		{
			base.Initialize();
			base.SubscribeLocalEvent<EntitySpokeEvent>(new EntityEventHandler<EntitySpokeEvent>(this.OnSpeak), null, null);
		}

		// Token: 0x0600088B RID: 2187 RVA: 0x0002B82D File Offset: 0x00029A2D
		private void OnSpeak(EntitySpokeEvent ev)
		{
			this.PingListeners(ev.Source, ev.Message, ev.ObfuscatedMessage);
		}

		// Token: 0x0600088C RID: 2188 RVA: 0x0002B848 File Offset: 0x00029A48
		public void PingListeners(EntityUid source, string message, [Nullable(2)] string obfuscatedMessage)
		{
			EntityQuery<TransformComponent> xformQuery = base.GetEntityQuery<TransformComponent>();
			TransformComponent sourceXform = xformQuery.GetComponent(source);
			Vector2 sourcePos = this._xforms.GetWorldPosition(sourceXform, xformQuery);
			ListenAttemptEvent attemptEv = new ListenAttemptEvent(source);
			ListenEvent ev = new ListenEvent(message, source);
			ListenEvent obfuscatedEv = (obfuscatedMessage == null) ? null : new ListenEvent(obfuscatedMessage, source);
			foreach (ValueTuple<ActiveListenerComponent, TransformComponent> valueTuple in base.EntityQuery<ActiveListenerComponent, TransformComponent>(false))
			{
				ActiveListenerComponent listener = valueTuple.Item1;
				TransformComponent xform = valueTuple.Item2;
				if (xform.MapID != sourceXform.MapID)
				{
					break;
				}
				float distance = (sourcePos - this._xforms.GetWorldPosition(xform, xformQuery)).LengthSquared;
				if (distance <= listener.Range * listener.Range)
				{
					base.RaiseLocalEvent<ListenAttemptEvent>(listener.Owner, attemptEv, false);
					if (attemptEv.Cancelled)
					{
						attemptEv.Uncancel();
					}
					else if (obfuscatedEv != null && distance > 2f)
					{
						base.RaiseLocalEvent<ListenEvent>(listener.Owner, obfuscatedEv, false);
					}
					else
					{
						base.RaiseLocalEvent<ListenEvent>(listener.Owner, ev, false);
					}
				}
			}
		}

		// Token: 0x04000536 RID: 1334
		[Dependency]
		private readonly SharedTransformSystem _xforms;
	}
}
