using System;
using System.Runtime.CompilerServices;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Physics.Events;

namespace Content.Shared.Physics
{
	// Token: 0x0200027C RID: 636
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class SharedPreventCollideSystem : EntitySystem
	{
		// Token: 0x0600073B RID: 1851 RVA: 0x00018A50 File Offset: 0x00016C50
		public override void Initialize()
		{
			base.Initialize();
			base.SubscribeLocalEvent<PreventCollideComponent, ComponentGetState>(new ComponentEventRefHandler<PreventCollideComponent, ComponentGetState>(this.OnGetState), null, null);
			base.SubscribeLocalEvent<PreventCollideComponent, ComponentHandleState>(new ComponentEventRefHandler<PreventCollideComponent, ComponentHandleState>(this.OnHandleState), null, null);
			base.SubscribeLocalEvent<PreventCollideComponent, PreventCollideEvent>(new ComponentEventRefHandler<PreventCollideComponent, PreventCollideEvent>(this.OnPreventCollide), null, null);
		}

		// Token: 0x0600073C RID: 1852 RVA: 0x00018A9F File Offset: 0x00016C9F
		private void OnGetState(EntityUid uid, PreventCollideComponent component, ref ComponentGetState args)
		{
			args.State = new PreventCollideComponentState(component);
		}

		// Token: 0x0600073D RID: 1853 RVA: 0x00018AB0 File Offset: 0x00016CB0
		private void OnHandleState(EntityUid uid, PreventCollideComponent component, ref ComponentHandleState args)
		{
			PreventCollideComponentState state = args.Current as PreventCollideComponentState;
			if (state == null)
			{
				return;
			}
			component.Uid = state.Uid;
		}

		// Token: 0x0600073E RID: 1854 RVA: 0x00018ADC File Offset: 0x00016CDC
		private void OnPreventCollide(EntityUid uid, PreventCollideComponent component, ref PreventCollideEvent args)
		{
			EntityUid otherUid = args.BodyB.Owner;
			if (component.Uid == otherUid)
			{
				args.Cancelled = true;
			}
		}
	}
}
