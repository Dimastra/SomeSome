using System;
using System.Runtime.CompilerServices;
using Content.Server.Atmos.EntitySystems;
using Content.Server.Disposal.Unit.Components;
using Robust.Shared.GameObjects;

namespace Content.Server.Disposal.Unit.EntitySystems
{
	// Token: 0x0200054C RID: 1356
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class BeingDisposedSystem : EntitySystem
	{
		// Token: 0x06001C72 RID: 7282 RVA: 0x00097968 File Offset: 0x00095B68
		public override void Initialize()
		{
			base.Initialize();
			base.SubscribeLocalEvent<BeingDisposedComponent, InhaleLocationEvent>(new ComponentEventHandler<BeingDisposedComponent, InhaleLocationEvent>(this.OnInhaleLocation), null, null);
			base.SubscribeLocalEvent<BeingDisposedComponent, ExhaleLocationEvent>(new ComponentEventHandler<BeingDisposedComponent, ExhaleLocationEvent>(this.OnExhaleLocation), null, null);
			base.SubscribeLocalEvent<BeingDisposedComponent, AtmosExposedGetAirEvent>(new ComponentEventRefHandler<BeingDisposedComponent, AtmosExposedGetAirEvent>(this.OnGetAir), null, null);
		}

		// Token: 0x06001C73 RID: 7283 RVA: 0x000979B8 File Offset: 0x00095BB8
		private void OnGetAir(EntityUid uid, BeingDisposedComponent component, ref AtmosExposedGetAirEvent args)
		{
			DisposalHolderComponent holder;
			if (base.TryComp<DisposalHolderComponent>(component.Holder, ref holder))
			{
				args.Gas = holder.Air;
			}
		}

		// Token: 0x06001C74 RID: 7284 RVA: 0x000979E4 File Offset: 0x00095BE4
		private void OnInhaleLocation(EntityUid uid, BeingDisposedComponent component, InhaleLocationEvent args)
		{
			DisposalHolderComponent holder;
			if (base.TryComp<DisposalHolderComponent>(component.Holder, ref holder))
			{
				args.Gas = holder.Air;
			}
		}

		// Token: 0x06001C75 RID: 7285 RVA: 0x00097A10 File Offset: 0x00095C10
		private void OnExhaleLocation(EntityUid uid, BeingDisposedComponent component, ExhaleLocationEvent args)
		{
			DisposalHolderComponent holder;
			if (base.TryComp<DisposalHolderComponent>(component.Holder, ref holder))
			{
				args.Gas = holder.Air;
			}
		}
	}
}
