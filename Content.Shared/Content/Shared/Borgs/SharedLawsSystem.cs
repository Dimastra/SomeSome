using System;
using System.Runtime.CompilerServices;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;

namespace Content.Shared.Borgs
{
	// Token: 0x02000651 RID: 1617
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class SharedLawsSystem : EntitySystem
	{
		// Token: 0x0600137C RID: 4988 RVA: 0x00040A4D File Offset: 0x0003EC4D
		public override void Initialize()
		{
			base.Initialize();
			base.SubscribeLocalEvent<LawsComponent, ComponentGetState>(new ComponentEventRefHandler<LawsComponent, ComponentGetState>(this.OnGetState), null, null);
			base.SubscribeLocalEvent<LawsComponent, ComponentHandleState>(new ComponentEventRefHandler<LawsComponent, ComponentHandleState>(this.OnHandleState), null, null);
		}

		// Token: 0x0600137D RID: 4989 RVA: 0x00040A7D File Offset: 0x0003EC7D
		private void OnGetState(EntityUid uid, LawsComponent component, ref ComponentGetState args)
		{
			args.State = new LawsComponentState(component.Laws);
		}

		// Token: 0x0600137E RID: 4990 RVA: 0x00040A90 File Offset: 0x0003EC90
		private void OnHandleState(EntityUid uid, LawsComponent component, ref ComponentHandleState args)
		{
			LawsComponentState cast = args.Current as LawsComponentState;
			if (cast == null)
			{
				return;
			}
			component.Laws = cast.Laws;
		}
	}
}
