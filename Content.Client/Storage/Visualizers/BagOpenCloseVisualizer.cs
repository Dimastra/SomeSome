using System;
using System.Runtime.CompilerServices;
using Content.Shared.Storage.Components;
using Robust.Client.GameObjects;
using Robust.Client.Graphics;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Log;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Utility;

namespace Content.Client.Storage.Visualizers
{
	// Token: 0x02000125 RID: 293
	public sealed class BagOpenCloseVisualizer : AppearanceVisualizer, ISerializationHooks
	{
		// Token: 0x06000809 RID: 2057 RVA: 0x0002EC57 File Offset: 0x0002CE57
		void ISerializationHooks.AfterDeserialization()
		{
			if (this._openIcon == null)
			{
				Logger.Warning("BagOpenCloseVisualizer is useless with no `openIcon`");
			}
		}

		// Token: 0x0600080A RID: 2058 RVA: 0x0002EC6C File Offset: 0x0002CE6C
		[Obsolete("Subscribe to your component being initialised instead.")]
		public override void InitializeEntity(EntityUid entity)
		{
			base.InitializeEntity(entity);
			IEntityManager entityManager = IoCManager.Resolve<IEntityManager>();
			SpriteComponent spriteComponent;
			if (this._openIcon != null && entityManager.TryGetComponent<SpriteComponent>(entity, ref spriteComponent))
			{
				RSI baseRSI = spriteComponent.BaseRSI;
				ResourcePath resourcePath = (baseRSI != null) ? baseRSI.Path : null;
				if (resourcePath != null)
				{
					spriteComponent.LayerMapReserveBlank("openIcon");
					spriteComponent.LayerSetSprite("openIcon", new SpriteSpecifier.Rsi(resourcePath, this._openIcon));
					spriteComponent.LayerSetVisible("openIcon", false);
				}
			}
		}

		// Token: 0x0600080B RID: 2059 RVA: 0x0002ECE0 File Offset: 0x0002CEE0
		[NullableContext(1)]
		[Obsolete("Subscribe to AppearanceChangeEvent instead.")]
		public override void OnChangeData(AppearanceComponent component)
		{
			base.OnChangeData(component);
			IEntityManager entityManager = IoCManager.Resolve<IEntityManager>();
			SpriteComponent spriteComponent;
			if (this._openIcon == null || !entityManager.TryGetComponent<SpriteComponent>(component.Owner, ref spriteComponent))
			{
				return;
			}
			SharedBagState sharedBagState;
			if (!component.TryGetData<SharedBagState>(SharedBagOpenVisuals.BagState, ref sharedBagState))
			{
				return;
			}
			if (sharedBagState == SharedBagState.Open)
			{
				spriteComponent.LayerSetVisible("openIcon", true);
				return;
			}
			spriteComponent.LayerSetVisible("openIcon", false);
		}

		// Token: 0x04000411 RID: 1041
		[Nullable(1)]
		private const string OpenIcon = "openIcon";

		// Token: 0x04000412 RID: 1042
		[Nullable(2)]
		[DataField("openIcon", false, 1, false, false, null)]
		private string _openIcon;
	}
}
