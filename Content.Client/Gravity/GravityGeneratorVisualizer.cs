using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Content.Shared.Gravity;
using Robust.Client.GameObjects;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Client.Gravity
{
	// Token: 0x020002F7 RID: 759
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class GravityGeneratorVisualizer : AppearanceVisualizer
	{
		// Token: 0x170003FE RID: 1022
		// (get) Token: 0x06001314 RID: 4884 RVA: 0x0007185C File Offset: 0x0006FA5C
		// (set) Token: 0x06001315 RID: 4885 RVA: 0x000718B4 File Offset: 0x0006FAB4
		[DataField("spritemap", false, 1, false, false, null)]
		private Dictionary<string, string> _rawSpriteMap
		{
			get
			{
				return this._spriteMap.ToDictionary((KeyValuePair<GravityGeneratorStatus, string> x) => x.Key.ToString().ToLower(), (KeyValuePair<GravityGeneratorStatus, string> x) => x.Value);
			}
			set
			{
				this._spriteMap.Clear();
				foreach (GravityGeneratorStatus key in (GravityGeneratorStatus[])Enum.GetValues(typeof(GravityGeneratorStatus)))
				{
					string value2;
					if (value.TryGetValue(key.ToString().ToLower(), out value2))
					{
						this._spriteMap[key] = value2;
					}
				}
			}
		}

		// Token: 0x06001316 RID: 4886 RVA: 0x0007191C File Offset: 0x0006FB1C
		[Obsolete("Subscribe to your component being initialised instead.")]
		public override void InitializeEntity(EntityUid entity)
		{
			base.InitializeEntity(entity);
			SpriteComponent spriteComponent;
			if (!IoCManager.Resolve<IEntityManager>().TryGetComponent<SpriteComponent>(entity, ref spriteComponent))
			{
				return;
			}
			spriteComponent.LayerMapReserveBlank(GravityGeneratorVisualizer.GravityGeneratorVisualLayers.Base);
			spriteComponent.LayerMapReserveBlank(GravityGeneratorVisualizer.GravityGeneratorVisualLayers.Core);
		}

		// Token: 0x06001317 RID: 4887 RVA: 0x0007195C File Offset: 0x0006FB5C
		[Obsolete("Subscribe to AppearanceChangeEvent instead.")]
		public override void OnChangeData(AppearanceComponent component)
		{
			base.OnChangeData(component);
			SpriteComponent component2 = IoCManager.Resolve<IEntityManager>().GetComponent<SpriteComponent>(component.Owner);
			GravityGeneratorStatus key;
			string text;
			if (component.TryGetData<GravityGeneratorStatus>(GravityGeneratorVisuals.State, ref key) && this._spriteMap.TryGetValue(key, out text))
			{
				int num = component2.LayerMapGet(GravityGeneratorVisualizer.GravityGeneratorVisualLayers.Base);
				component2.LayerSetState(num, text);
			}
			float num2;
			if (component.TryGetData<float>(GravityGeneratorVisuals.Charge, ref num2))
			{
				int num3 = component2.LayerMapGet(GravityGeneratorVisualizer.GravityGeneratorVisualLayers.Core);
				if (num2 < 0.2f)
				{
					component2.LayerSetVisible(num3, false);
					return;
				}
				if (num2 >= 0.2f)
				{
					if (num2 < 0.4f)
					{
						component2.LayerSetVisible(num3, true);
						component2.LayerSetState(num3, "startup");
						return;
					}
					if (num2 < 0.6f)
					{
						component2.LayerSetVisible(num3, true);
						component2.LayerSetState(num3, "idle");
						return;
					}
					if (num2 < 0.8f)
					{
						component2.LayerSetVisible(num3, true);
						component2.LayerSetState(num3, "activating");
						return;
					}
				}
				component2.LayerSetVisible(num3, true);
				component2.LayerSetState(num3, "activated");
			}
		}

		// Token: 0x0400098C RID: 2444
		private Dictionary<GravityGeneratorStatus, string> _spriteMap = new Dictionary<GravityGeneratorStatus, string>();

		// Token: 0x020002F8 RID: 760
		[NullableContext(0)]
		public enum GravityGeneratorVisualLayers : byte
		{
			// Token: 0x0400098E RID: 2446
			Base,
			// Token: 0x0400098F RID: 2447
			Core
		}
	}
}
