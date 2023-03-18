using System;
using System.Runtime.CompilerServices;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;

namespace Content.Shared.Flash
{
	// Token: 0x0200048B RID: 1163
	public abstract class SharedFlashSystem : EntitySystem
	{
		// Token: 0x06000DF9 RID: 3577 RVA: 0x0002D71D File Offset: 0x0002B91D
		public override void Initialize()
		{
			base.Initialize();
			ComponentEventRefHandler<FlashableComponent, ComponentGetState> componentEventRefHandler;
			if ((componentEventRefHandler = SharedFlashSystem.<>O.<0>__OnFlashableGetState) == null)
			{
				componentEventRefHandler = (SharedFlashSystem.<>O.<0>__OnFlashableGetState = new ComponentEventRefHandler<FlashableComponent, ComponentGetState>(SharedFlashSystem.OnFlashableGetState));
			}
			base.SubscribeLocalEvent<FlashableComponent, ComponentGetState>(componentEventRefHandler, null, null);
		}

		// Token: 0x06000DFA RID: 3578 RVA: 0x0002D748 File Offset: 0x0002B948
		[NullableContext(1)]
		private static void OnFlashableGetState(EntityUid uid, FlashableComponent component, ref ComponentGetState args)
		{
			args.State = new FlashableComponentState(component.Duration, component.LastFlash);
		}

		// Token: 0x0200080D RID: 2061
		[CompilerGenerated]
		private static class <>O
		{
			// Token: 0x040018B9 RID: 6329
			[Nullable(new byte[]
			{
				0,
				1
			})]
			public static ComponentEventRefHandler<FlashableComponent, ComponentGetState> <0>__OnFlashableGetState;
		}
	}
}
