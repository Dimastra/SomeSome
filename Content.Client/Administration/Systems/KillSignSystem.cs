using System;
using System.Runtime.CompilerServices;
using Content.Client.Administration.Components;
using Robust.Client.GameObjects;
using Robust.Shared.GameObjects;
using Robust.Shared.Maths;
using Robust.Shared.Utility;

namespace Content.Client.Administration.Systems
{
	// Token: 0x020004E2 RID: 1250
	public sealed class KillSignSystem : EntitySystem
	{
		// Token: 0x06001FD7 RID: 8151 RVA: 0x000B99E8 File Offset: 0x000B7BE8
		public override void Initialize()
		{
			base.SubscribeLocalEvent<KillSignComponent, ComponentStartup>(new ComponentEventHandler<KillSignComponent, ComponentStartup>(this.KillSignAdded), null, null);
			base.SubscribeLocalEvent<KillSignComponent, ComponentShutdown>(new ComponentEventHandler<KillSignComponent, ComponentShutdown>(this.KillSignRemoved), null, null);
		}

		// Token: 0x06001FD8 RID: 8152 RVA: 0x000B9A14 File Offset: 0x000B7C14
		[NullableContext(1)]
		private void KillSignRemoved(EntityUid uid, KillSignComponent component, ComponentShutdown args)
		{
			SpriteComponent spriteComponent;
			if (!base.TryComp<SpriteComponent>(uid, ref spriteComponent))
			{
				return;
			}
			int num;
			if (!spriteComponent.LayerMapTryGet(KillSignSystem.KillSignKey.Key, ref num, false))
			{
				return;
			}
			spriteComponent.RemoveLayer(num);
		}

		// Token: 0x06001FD9 RID: 8153 RVA: 0x000B9A48 File Offset: 0x000B7C48
		[NullableContext(1)]
		private void KillSignAdded(EntityUid uid, KillSignComponent component, ComponentStartup args)
		{
			SpriteComponent spriteComponent;
			if (!base.TryComp<SpriteComponent>(uid, ref spriteComponent))
			{
				return;
			}
			int num;
			if (spriteComponent.LayerMapTryGet(KillSignSystem.KillSignKey.Key, ref num, false))
			{
				return;
			}
			float num2 = spriteComponent.Bounds.Height / 2f + 0.1875f;
			int num3 = spriteComponent.AddLayer(new SpriteSpecifier.Rsi(new ResourcePath("Objects/Misc/killsign.rsi", "/"), "sign"), null);
			spriteComponent.LayerMapSet(KillSignSystem.KillSignKey.Key, num3);
			spriteComponent.LayerSetOffset(num3, new Vector2(0f, num2));
			spriteComponent.LayerSetShader(num3, "unshaded");
		}

		// Token: 0x020004E3 RID: 1251
		private enum KillSignKey
		{
			// Token: 0x04000F36 RID: 3894
			Key
		}
	}
}
