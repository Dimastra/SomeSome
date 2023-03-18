using System;
using System.Runtime.CompilerServices;
using Content.Shared.Dragon;
using Robust.Client.GameObjects;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Maths;

namespace Content.Client.Dragon
{
	// Token: 0x0200033B RID: 827
	public sealed class DragonSystem : EntitySystem
	{
		// Token: 0x0600149D RID: 5277 RVA: 0x00078B1C File Offset: 0x00076D1C
		public override void Initialize()
		{
			base.Initialize();
			base.SubscribeLocalEvent<DragonRiftComponent, ComponentHandleState>(new ComponentEventRefHandler<DragonRiftComponent, ComponentHandleState>(this.OnRiftHandleState), null, null);
		}

		// Token: 0x0600149E RID: 5278 RVA: 0x00078B38 File Offset: 0x00076D38
		[NullableContext(1)]
		private void OnRiftHandleState(EntityUid uid, DragonRiftComponent component, ref ComponentHandleState args)
		{
			DragonRiftComponentState dragonRiftComponentState = args.Current as DragonRiftComponentState;
			if (dragonRiftComponentState == null)
			{
				return;
			}
			if (component.State == dragonRiftComponentState.State)
			{
				return;
			}
			component.State = dragonRiftComponentState.State;
			SpriteComponent spriteComponent;
			base.TryComp<SpriteComponent>(uid, ref spriteComponent);
			PointLightComponent pointLightComponent;
			base.TryComp<PointLightComponent>(uid, ref pointLightComponent);
			if (spriteComponent == null && pointLightComponent == null)
			{
				return;
			}
			switch (dragonRiftComponentState.State)
			{
			case DragonRiftState.Charging:
				if (spriteComponent != null)
				{
					spriteComponent.LayerSetColor(0, Color.FromHex("#569fff", null));
				}
				if (pointLightComponent != null)
				{
					pointLightComponent.Color = Color.FromHex("#366db5", null);
					return;
				}
				break;
			case DragonRiftState.AlmostFinished:
				if (spriteComponent != null)
				{
					spriteComponent.LayerSetColor(0, Color.FromHex("#cf4cff", null));
				}
				if (pointLightComponent != null)
				{
					pointLightComponent.Color = Color.FromHex("#9e2fc1", null);
					return;
				}
				break;
			case DragonRiftState.Finished:
				if (spriteComponent != null)
				{
					spriteComponent.LayerSetColor(0, Color.FromHex("#edbc36", null));
				}
				if (pointLightComponent != null)
				{
					pointLightComponent.Color = Color.FromHex("#cbaf20", null);
				}
				break;
			default:
				return;
			}
		}
	}
}
