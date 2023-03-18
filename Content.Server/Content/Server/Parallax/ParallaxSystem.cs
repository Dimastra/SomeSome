using System;
using System.Runtime.CompilerServices;
using Content.Shared.Parallax;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;

namespace Content.Server.Parallax
{
	// Token: 0x020002ED RID: 749
	public sealed class ParallaxSystem : SharedParallaxSystem
	{
		// Token: 0x06000F61 RID: 3937 RVA: 0x0004F2C7 File Offset: 0x0004D4C7
		public override void Initialize()
		{
			base.Initialize();
			base.SubscribeLocalEvent<ParallaxComponent, ComponentGetState>(new ComponentEventRefHandler<ParallaxComponent, ComponentGetState>(this.OnParallaxGetState), null, null);
		}

		// Token: 0x06000F62 RID: 3938 RVA: 0x0004F2E3 File Offset: 0x0004D4E3
		[NullableContext(1)]
		private void OnParallaxGetState(EntityUid uid, ParallaxComponent component, ref ComponentGetState args)
		{
			args.State = new SharedParallaxSystem.ParallaxComponentState
			{
				Parallax = component.Parallax
			};
		}
	}
}
