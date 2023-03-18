using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Content.Shared.Rounding;
using Content.Shared.Stacks;
using Robust.Client.GameObjects;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Utility;

namespace Content.Client.Stack
{
	// Token: 0x02000135 RID: 309
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class StackVisualizer : AppearanceVisualizer
	{
		// Token: 0x0600084A RID: 2122 RVA: 0x00030264 File Offset: 0x0002E464
		[Obsolete("Subscribe to your component being initialised instead.")]
		public override void InitializeEntity(EntityUid entity)
		{
			base.InitializeEntity(entity);
			SpriteComponent spriteComponent;
			if (this._isComposite && this._spriteLayers.Count > 0 && IoCManager.Resolve<IEntityManager>().TryGetComponent<SpriteComponent>(entity, ref spriteComponent))
			{
				ResourcePath resourcePath = this._spritePath ?? spriteComponent.BaseRSI.Path;
				foreach (string text in this._spriteLayers)
				{
					spriteComponent.LayerMapReserveBlank(text);
					spriteComponent.LayerSetSprite(text, new SpriteSpecifier.Rsi(resourcePath, text));
					spriteComponent.LayerSetVisible(text, false);
				}
			}
		}

		// Token: 0x0600084B RID: 2123 RVA: 0x00030314 File Offset: 0x0002E514
		[Obsolete("Subscribe to AppearanceChangeEvent instead.")]
		public override void OnChangeData(AppearanceComponent component)
		{
			base.OnChangeData(component);
			SpriteComponent spriteComponent;
			if (IoCManager.Resolve<IEntityManager>().TryGetComponent<SpriteComponent>(component.Owner, ref spriteComponent))
			{
				if (this._isComposite)
				{
					this.ProcessCompositeSprites(component, spriteComponent);
					return;
				}
				this.ProcessOpaqueSprites(component, spriteComponent);
			}
		}

		// Token: 0x0600084C RID: 2124 RVA: 0x00030358 File Offset: 0x0002E558
		private void ProcessOpaqueSprites(AppearanceComponent component, SpriteComponent spriteComponent)
		{
			int num;
			if (!component.TryGetData<int>(StackVisuals.Actual, ref num))
			{
				return;
			}
			int count;
			if (!component.TryGetData<int>(StackVisuals.MaxCount, ref count))
			{
				count = this._spriteLayers.Count;
			}
			int index = ContentHelpers.RoundToEqualLevels((double)num, (double)count, this._spriteLayers.Count);
			spriteComponent.LayerSetState(0, this._spriteLayers[index]);
		}

		// Token: 0x0600084D RID: 2125 RVA: 0x000303C0 File Offset: 0x0002E5C0
		private void ProcessCompositeSprites(AppearanceComponent component, SpriteComponent spriteComponent)
		{
			bool flag;
			if (component.TryGetData<bool>(StackVisuals.Hide, ref flag) && flag)
			{
				foreach (string text in this._spriteLayers)
				{
					spriteComponent.LayerSetVisible(text, false);
				}
				return;
			}
			int num;
			if (!component.TryGetData<int>(StackVisuals.Actual, ref num))
			{
				return;
			}
			int count;
			if (!component.TryGetData<int>(StackVisuals.MaxCount, ref count))
			{
				count = this._spriteLayers.Count;
			}
			int num2 = ContentHelpers.RoundToNearestLevels((double)num, (double)count, this._spriteLayers.Count);
			for (int i = 0; i < this._spriteLayers.Count; i++)
			{
				spriteComponent.LayerSetVisible(this._spriteLayers[i], i < num2);
			}
		}

		// Token: 0x0400042D RID: 1069
		private const int IconLayer = 0;

		// Token: 0x0400042E RID: 1070
		[DataField("stackLayers", false, 1, false, false, null)]
		private readonly List<string> _spriteLayers = new List<string>();

		// Token: 0x0400042F RID: 1071
		[DataField("composite", false, 1, false, false, null)]
		private bool _isComposite;

		// Token: 0x04000430 RID: 1072
		[Nullable(2)]
		[DataField("sprite", false, 1, false, false, null)]
		private ResourcePath _spritePath;
	}
}
