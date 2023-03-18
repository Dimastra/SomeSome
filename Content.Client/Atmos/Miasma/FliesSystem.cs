using System;
using System.Runtime.CompilerServices;
using Robust.Client.GameObjects;
using Robust.Shared.GameObjects;
using Robust.Shared.Utility;

namespace Content.Client.Atmos.Miasma
{
	// Token: 0x02000454 RID: 1108
	public sealed class FliesSystem : EntitySystem
	{
		// Token: 0x06001BB1 RID: 7089 RVA: 0x000A03E3 File Offset: 0x0009E5E3
		public override void Initialize()
		{
			base.SubscribeLocalEvent<FliesComponent, ComponentStartup>(new ComponentEventHandler<FliesComponent, ComponentStartup>(this.FliesAdded), null, null);
			base.SubscribeLocalEvent<FliesComponent, ComponentShutdown>(new ComponentEventHandler<FliesComponent, ComponentShutdown>(this.FliesRemoved), null, null);
		}

		// Token: 0x06001BB2 RID: 7090 RVA: 0x000A0410 File Offset: 0x0009E610
		[NullableContext(1)]
		private void FliesRemoved(EntityUid uid, FliesComponent component, ComponentShutdown args)
		{
			SpriteComponent spriteComponent;
			if (!base.TryComp<SpriteComponent>(uid, ref spriteComponent))
			{
				return;
			}
			int num;
			if (!spriteComponent.LayerMapTryGet(FliesSystem.FliesKey.Key, ref num, false))
			{
				return;
			}
			spriteComponent.RemoveLayer(num);
		}

		// Token: 0x06001BB3 RID: 7091 RVA: 0x000A0444 File Offset: 0x0009E644
		[NullableContext(1)]
		private void FliesAdded(EntityUid uid, FliesComponent component, ComponentStartup args)
		{
			SpriteComponent spriteComponent;
			if (!base.TryComp<SpriteComponent>(uid, ref spriteComponent))
			{
				return;
			}
			int num;
			if (spriteComponent.LayerMapTryGet(FliesSystem.FliesKey.Key, ref num, false))
			{
				return;
			}
			int num2 = spriteComponent.AddLayer(new SpriteSpecifier.Rsi(new ResourcePath("Objects/Misc/flies.rsi", "/"), "flies"), null);
			spriteComponent.LayerMapSet(FliesSystem.FliesKey.Key, num2);
		}

		// Token: 0x02000455 RID: 1109
		private enum FliesKey
		{
			// Token: 0x04000DCA RID: 3530
			Key
		}
	}
}
