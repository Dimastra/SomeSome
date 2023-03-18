using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Content.Shared.Mobs;
using Robust.Client.GameObjects;
using Robust.Shared.GameObjects;

namespace Content.Client.DamageState
{
	// Token: 0x02000361 RID: 865
	[NullableContext(1)]
	[Nullable(new byte[]
	{
		0,
		1
	})]
	public sealed class DamageStateVisualizerSystem : VisualizerSystem<DamageStateVisualsComponent>
	{
		// Token: 0x0600156F RID: 5487 RVA: 0x0007E38C File Offset: 0x0007C58C
		protected override void OnAppearanceChange(EntityUid uid, DamageStateVisualsComponent component, ref AppearanceChangeEvent args)
		{
			SpriteComponent sprite = args.Sprite;
			MobState mobState;
			if (sprite == null || !this.AppearanceSystem.TryGetData<MobState>(uid, MobStateVisuals.State, ref mobState, args.Component))
			{
				return;
			}
			Dictionary<DamageStateVisualLayers, string> dictionary;
			if (!component.States.TryGetValue(mobState, out dictionary))
			{
				return;
			}
			if (component.Rotate)
			{
				SpriteComponent spriteComponent = sprite;
				bool noRotation = mobState != MobState.Critical && mobState != MobState.Dead;
				spriteComponent.NoRotation = noRotation;
			}
			DamageStateVisualLayers[] array = new DamageStateVisualLayers[]
			{
				DamageStateVisualLayers.Base,
				DamageStateVisualLayers.BaseUnshaded
			};
			int i;
			for (i = 0; i < array.Length; i++)
			{
				DamageStateVisualLayers damageStateVisualLayers = array[i];
				int num;
				if (sprite.LayerMapTryGet(damageStateVisualLayers, ref num, false))
				{
					sprite.LayerSetVisible(damageStateVisualLayers, false);
				}
			}
			foreach (KeyValuePair<DamageStateVisualLayers, string> keyValuePair in dictionary)
			{
				DamageStateVisualLayers damageStateVisualLayers2;
				string text;
				keyValuePair.Deconstruct(out damageStateVisualLayers2, out text);
				DamageStateVisualLayers damageStateVisualLayers3 = damageStateVisualLayers2;
				string text2 = text;
				if (sprite.LayerMapTryGet(damageStateVisualLayers3, ref i, false))
				{
					sprite.LayerSetVisible(damageStateVisualLayers3, true);
					sprite.LayerSetState(damageStateVisualLayers3, text2);
				}
			}
			if (mobState == MobState.Dead)
			{
				if (sprite.DrawDepth > -4)
				{
					component.OriginalDrawDepth = new int?(sprite.DrawDepth);
					sprite.DrawDepth = -4;
					return;
				}
			}
			else if (component.OriginalDrawDepth != null)
			{
				sprite.DrawDepth = component.OriginalDrawDepth.Value;
				component.OriginalDrawDepth = null;
			}
		}
	}
}
