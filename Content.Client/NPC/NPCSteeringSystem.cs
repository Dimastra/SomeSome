using System;
using System.Runtime.CompilerServices;
using Content.Shared.NPC;
using Content.Shared.NPC.Events;
using Robust.Client.Graphics;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;

namespace Content.Client.NPC
{
	// Token: 0x0200020C RID: 524
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class NPCSteeringSystem : SharedNPCSteeringSystem
	{
		// Token: 0x170002E5 RID: 741
		// (get) Token: 0x06000DB3 RID: 3507 RVA: 0x00052942 File Offset: 0x00050B42
		// (set) Token: 0x06000DB4 RID: 3508 RVA: 0x0005294C File Offset: 0x00050B4C
		public bool DebugEnabled
		{
			get
			{
				return this._debugEnabled;
			}
			set
			{
				if (this._debugEnabled == value)
				{
					return;
				}
				this._debugEnabled = value;
				if (this._debugEnabled)
				{
					this._overlay.AddOverlay(new NPCSteeringOverlay(this.EntityManager));
					base.RaiseNetworkEvent(new RequestNPCSteeringDebugEvent
					{
						Enabled = true
					});
					return;
				}
				this._overlay.RemoveOverlay<NPCSteeringOverlay>();
				base.RaiseNetworkEvent(new RequestNPCSteeringDebugEvent
				{
					Enabled = false
				});
				foreach (NPCSteeringComponent npcsteeringComponent in base.EntityQuery<NPCSteeringComponent>(true))
				{
					base.RemCompDeferred<NPCSteeringComponent>(npcsteeringComponent.Owner);
				}
			}
		}

		// Token: 0x06000DB5 RID: 3509 RVA: 0x00052A04 File Offset: 0x00050C04
		public override void Initialize()
		{
			base.Initialize();
			base.SubscribeNetworkEvent<NPCSteeringDebugEvent>(new EntityEventHandler<NPCSteeringDebugEvent>(this.OnDebugEvent), null, null);
		}

		// Token: 0x06000DB6 RID: 3510 RVA: 0x00052A20 File Offset: 0x00050C20
		private void OnDebugEvent(NPCSteeringDebugEvent ev)
		{
			if (!this.DebugEnabled)
			{
				return;
			}
			foreach (NPCSteeringDebugData npcsteeringDebugData in ev.Data)
			{
				if (base.Exists(npcsteeringDebugData.EntityUid))
				{
					NPCSteeringComponent npcsteeringComponent = base.EnsureComp<NPCSteeringComponent>(npcsteeringDebugData.EntityUid);
					npcsteeringComponent.Direction = npcsteeringDebugData.Direction;
					npcsteeringComponent.DangerMap = npcsteeringDebugData.Danger;
					npcsteeringComponent.InterestMap = npcsteeringDebugData.Interest;
					npcsteeringComponent.DangerPoints = npcsteeringDebugData.DangerPoints;
				}
			}
		}

		// Token: 0x040006C4 RID: 1732
		[Dependency]
		private readonly IOverlayManager _overlay;

		// Token: 0x040006C5 RID: 1733
		private bool _debugEnabled;
	}
}
