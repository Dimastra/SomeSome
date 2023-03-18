using System;
using System.Runtime.CompilerServices;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Serialization;

namespace Content.Shared.Fluids
{
	// Token: 0x02000483 RID: 1155
	public abstract class SharedMoppingSystem : EntitySystem
	{
		// Token: 0x06000DEA RID: 3562 RVA: 0x0002D604 File Offset: 0x0002B804
		public override void Initialize()
		{
			base.Initialize();
			base.SubscribeLocalEvent<AbsorbentComponent, ComponentGetState>(new ComponentEventRefHandler<AbsorbentComponent, ComponentGetState>(this.OnAbsorbentGetState), null, null);
			base.SubscribeLocalEvent<AbsorbentComponent, ComponentHandleState>(new ComponentEventRefHandler<AbsorbentComponent, ComponentHandleState>(this.OnAbsorbentHandleState), null, null);
		}

		// Token: 0x06000DEB RID: 3563 RVA: 0x0002D634 File Offset: 0x0002B834
		[NullableContext(1)]
		private void OnAbsorbentHandleState(EntityUid uid, AbsorbentComponent component, ref ComponentHandleState args)
		{
			SharedMoppingSystem.AbsorbentComponentState state = args.Current as SharedMoppingSystem.AbsorbentComponentState;
			if (state == null)
			{
				return;
			}
			if (component.Progress.Equals(state.Progress))
			{
				return;
			}
			component.Progress = state.Progress;
		}

		// Token: 0x06000DEC RID: 3564 RVA: 0x0002D671 File Offset: 0x0002B871
		[NullableContext(1)]
		private void OnAbsorbentGetState(EntityUid uid, AbsorbentComponent component, ref ComponentGetState args)
		{
			args.State = new SharedMoppingSystem.AbsorbentComponentState
			{
				Progress = component.Progress
			};
		}

		// Token: 0x0200080C RID: 2060
		[NetSerializable]
		[Serializable]
		protected sealed class AbsorbentComponentState : ComponentState
		{
			// Token: 0x040018B8 RID: 6328
			public float Progress;
		}
	}
}
