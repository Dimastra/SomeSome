using System;
using System.Runtime.CompilerServices;
using Content.Server.Wires;
using Content.Shared.Access;
using Content.Shared.Access.Components;
using Content.Shared.Emag.Components;
using Content.Shared.Wires;
using Robust.Shared.GameObjects;
using Robust.Shared.Maths;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Server.Access
{
	// Token: 0x02000878 RID: 2168
	[NullableContext(1)]
	[Nullable(new byte[]
	{
		0,
		1
	})]
	public sealed class AccessWireAction : ComponentWireAction<AccessReaderComponent>
	{
		// Token: 0x170007E0 RID: 2016
		// (get) Token: 0x06002F56 RID: 12118 RVA: 0x000F4DC1 File Offset: 0x000F2FC1
		// (set) Token: 0x06002F57 RID: 12119 RVA: 0x000F4DC9 File Offset: 0x000F2FC9
		public override Color Color { get; set; } = Color.Green;

		// Token: 0x170007E1 RID: 2017
		// (get) Token: 0x06002F58 RID: 12120 RVA: 0x000F4DD2 File Offset: 0x000F2FD2
		// (set) Token: 0x06002F59 RID: 12121 RVA: 0x000F4DDA File Offset: 0x000F2FDA
		public override string Name { get; set; } = "wire-name-access";

		// Token: 0x06002F5A RID: 12122 RVA: 0x000F4DE3 File Offset: 0x000F2FE3
		public override StatusLightState? GetLightState(Wire wire, AccessReaderComponent comp)
		{
			return new StatusLightState?(this.EntityManager.HasComponent<EmaggedComponent>(comp.Owner) ? StatusLightState.On : StatusLightState.Off);
		}

		// Token: 0x170007E2 RID: 2018
		// (get) Token: 0x06002F5B RID: 12123 RVA: 0x000F4E01 File Offset: 0x000F3001
		public override object StatusKey { get; } = AccessWireActionKey.Status;

		// Token: 0x06002F5C RID: 12124 RVA: 0x000F4E09 File Offset: 0x000F3009
		public override bool Cut(EntityUid user, Wire wire, AccessReaderComponent comp)
		{
			this.WiresSystem.TryCancelWireAction(wire.Owner, AccessWireAction.PulseTimeoutKey.Key);
			this.EntityManager.EnsureComponent<EmaggedComponent>(comp.Owner);
			return true;
		}

		// Token: 0x06002F5D RID: 12125 RVA: 0x000F4E36 File Offset: 0x000F3036
		public override bool Mend(EntityUid user, Wire wire, AccessReaderComponent comp)
		{
			this.EntityManager.RemoveComponent<EmaggedComponent>(comp.Owner);
			return true;
		}

		// Token: 0x06002F5E RID: 12126 RVA: 0x000F4E4C File Offset: 0x000F304C
		public override void Pulse(EntityUid user, Wire wire, AccessReaderComponent comp)
		{
			this.EntityManager.EnsureComponent<EmaggedComponent>(comp.Owner);
			this.WiresSystem.StartWireAction(wire.Owner, (float)this._pulseTimeout, AccessWireAction.PulseTimeoutKey.Key, new TimedWireEvent(new WireActionDelegate(this.AwaitPulseCancel), wire));
		}

		// Token: 0x06002F5F RID: 12127 RVA: 0x000F4E9B File Offset: 0x000F309B
		public override void Update(Wire wire)
		{
			if (!base.IsPowered(wire.Owner))
			{
				this.WiresSystem.TryCancelWireAction(wire.Owner, AccessWireAction.PulseTimeoutKey.Key);
			}
		}

		// Token: 0x06002F60 RID: 12128 RVA: 0x000F4EC4 File Offset: 0x000F30C4
		private void AwaitPulseCancel(Wire wire)
		{
			AccessReaderComponent access;
			if (!wire.IsCut && this.EntityManager.TryGetComponent<AccessReaderComponent>(wire.Owner, ref access))
			{
				this.EntityManager.RemoveComponent<EmaggedComponent>(wire.Owner);
			}
		}

		// Token: 0x04001C7D RID: 7293
		[DataField("pulseTimeout", false, 1, false, false, null)]
		private int _pulseTimeout = 30;

		// Token: 0x02000BAD RID: 2989
		[NullableContext(0)]
		private enum PulseTimeoutKey : byte
		{
			// Token: 0x04002C1B RID: 11291
			Key
		}
	}
}
