using System;
using System.Runtime.CompilerServices;
using Content.Shared.Wires;
using Robust.Shared.GameObjects;

namespace Content.Server.Wires
{
	// Token: 0x02000070 RID: 112
	[NullableContext(1)]
	[Nullable(0)]
	public abstract class ComponentWireAction<[Nullable(0)] TComponent> : BaseWireAction where TComponent : Component
	{
		// Token: 0x0600015B RID: 347
		public abstract StatusLightState? GetLightState(Wire wire, TComponent component);

		// Token: 0x0600015C RID: 348 RVA: 0x00008418 File Offset: 0x00006618
		public override StatusLightState? GetLightState(Wire wire)
		{
			TComponent component;
			if (!this.EntityManager.TryGetComponent<TComponent>(wire.Owner, ref component))
			{
				return new StatusLightState?(StatusLightState.Off);
			}
			return this.GetLightState(wire, component);
		}

		// Token: 0x0600015D RID: 349
		public abstract bool Cut(EntityUid user, Wire wire, TComponent component);

		// Token: 0x0600015E RID: 350
		public abstract bool Mend(EntityUid user, Wire wire, TComponent component);

		// Token: 0x0600015F RID: 351
		public abstract void Pulse(EntityUid user, Wire wire, TComponent component);

		// Token: 0x06000160 RID: 352 RVA: 0x0000844C File Offset: 0x0000664C
		public override bool Cut(EntityUid user, Wire wire)
		{
			base.Cut(user, wire);
			TComponent component;
			return this.EntityManager.TryGetComponent<TComponent>(wire.Owner, ref component) && this.Cut(user, wire, component);
		}

		// Token: 0x06000161 RID: 353 RVA: 0x00008484 File Offset: 0x00006684
		public override bool Mend(EntityUid user, Wire wire)
		{
			base.Mend(user, wire);
			TComponent component;
			return this.EntityManager.TryGetComponent<TComponent>(wire.Owner, ref component) && this.Mend(user, wire, component);
		}

		// Token: 0x06000162 RID: 354 RVA: 0x000084BC File Offset: 0x000066BC
		public override void Pulse(EntityUid user, Wire wire)
		{
			base.Pulse(user, wire);
			TComponent component;
			if (this.EntityManager.TryGetComponent<TComponent>(wire.Owner, ref component))
			{
				this.Pulse(user, wire, component);
			}
		}
	}
}
