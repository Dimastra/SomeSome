using System;
using System.Runtime.CompilerServices;
using Content.Client.Administration.Components;
using Robust.Client.GameObjects;
using Robust.Shared.GameObjects;
using Robust.Shared.Maths;

namespace Content.Client.Administration.Systems
{
	// Token: 0x020004E1 RID: 1249
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class HeadstandSystem : EntitySystem
	{
		// Token: 0x06001FD3 RID: 8147 RVA: 0x000B98DC File Offset: 0x000B7ADC
		public override void Initialize()
		{
			base.SubscribeLocalEvent<HeadstandComponent, ComponentStartup>(new ComponentEventHandler<HeadstandComponent, ComponentStartup>(this.OnHeadstandAdded), null, null);
			base.SubscribeLocalEvent<HeadstandComponent, ComponentShutdown>(new ComponentEventHandler<HeadstandComponent, ComponentShutdown>(this.OnHeadstandRemoved), null, null);
		}

		// Token: 0x06001FD4 RID: 8148 RVA: 0x000B9908 File Offset: 0x000B7B08
		private void OnHeadstandAdded(EntityUid uid, HeadstandComponent component, ComponentStartup args)
		{
			SpriteComponent spriteComponent;
			if (!base.TryComp<SpriteComponent>(uid, ref spriteComponent))
			{
				return;
			}
			foreach (ISpriteLayer spriteLayer in spriteComponent.AllLayers)
			{
				spriteLayer.Rotation += Angle.FromDegrees(180.0);
			}
		}

		// Token: 0x06001FD5 RID: 8149 RVA: 0x000B9978 File Offset: 0x000B7B78
		private void OnHeadstandRemoved(EntityUid uid, HeadstandComponent component, ComponentShutdown args)
		{
			SpriteComponent spriteComponent;
			if (!base.TryComp<SpriteComponent>(uid, ref spriteComponent))
			{
				return;
			}
			foreach (ISpriteLayer spriteLayer in spriteComponent.AllLayers)
			{
				spriteLayer.Rotation -= Angle.FromDegrees(180.0);
			}
		}
	}
}
