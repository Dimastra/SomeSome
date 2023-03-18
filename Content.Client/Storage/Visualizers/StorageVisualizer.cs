using System;
using System.Runtime.CompilerServices;
using Content.Shared.Storage;
using Robust.Client.GameObjects;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Client.Storage.Visualizers
{
	// Token: 0x02000127 RID: 295
	[NullableContext(2)]
	[Nullable(0)]
	public sealed class StorageVisualizer : AppearanceVisualizer
	{
		// Token: 0x0600080F RID: 2063 RVA: 0x0002EDD4 File Offset: 0x0002CFD4
		[Obsolete("Subscribe to your component being initialised instead.")]
		public override void InitializeEntity(EntityUid entity)
		{
			SpriteComponent spriteComponent;
			if (!IoCManager.Resolve<IEntityManager>().TryGetComponent<SpriteComponent>(entity, ref spriteComponent))
			{
				return;
			}
			if (this._stateBase != null)
			{
				spriteComponent.LayerSetState(0, this._stateBase);
			}
			if (this._stateBaseAlt == null)
			{
				this._stateBaseAlt = this._stateBase;
			}
		}

		// Token: 0x06000810 RID: 2064 RVA: 0x0002EE20 File Offset: 0x0002D020
		[NullableContext(1)]
		[Obsolete("Subscribe to AppearanceChangeEvent instead.")]
		public override void OnChangeData(AppearanceComponent component)
		{
			base.OnChangeData(component);
			SpriteComponent spriteComponent;
			if (!IoCManager.Resolve<IEntityManager>().TryGetComponent<SpriteComponent>(component.Owner, ref spriteComponent))
			{
				return;
			}
			bool flag;
			component.TryGetData<bool>(StorageVisuals.Open, ref flag);
			int num;
			if (spriteComponent.LayerMapTryGet(StorageVisualLayers.Door, ref num, false))
			{
				spriteComponent.LayerSetVisible(StorageVisualLayers.Door, true);
				if (flag)
				{
					if (this._stateOpen != null)
					{
						spriteComponent.LayerSetState(StorageVisualLayers.Door, this._stateOpen);
						spriteComponent.LayerSetVisible(StorageVisualLayers.Door, true);
					}
					if (this._stateBaseAlt != null)
					{
						spriteComponent.LayerSetState(0, this._stateBaseAlt);
					}
				}
				else if (!flag)
				{
					if (this._stateClosed != null)
					{
						spriteComponent.LayerSetState(StorageVisualLayers.Door, this._stateClosed);
					}
					else
					{
						spriteComponent.LayerSetVisible(StorageVisualLayers.Door, false);
					}
					if (this._stateBase != null)
					{
						spriteComponent.LayerSetState(0, this._stateBase);
					}
				}
				else
				{
					spriteComponent.LayerSetVisible(StorageVisualLayers.Door, false);
				}
			}
			bool flag2;
			if (component.TryGetData<bool>(StorageVisuals.CanLock, ref flag2) && flag2)
			{
				bool flag3;
				if (!component.TryGetData<bool>(StorageVisuals.Locked, ref flag3))
				{
					flag3 = true;
				}
				spriteComponent.LayerSetVisible(StorageVisualLayers.Lock, !flag);
				if (!flag)
				{
					spriteComponent.LayerSetState(StorageVisualLayers.Lock, flag3 ? "locked" : "unlocked");
				}
			}
		}

		// Token: 0x04000413 RID: 1043
		[DataField("state", false, 1, false, false, null)]
		private string _stateBase;

		// Token: 0x04000414 RID: 1044
		[DataField("state_alt", false, 1, false, false, null)]
		private string _stateBaseAlt;

		// Token: 0x04000415 RID: 1045
		[DataField("state_open", false, 1, false, false, null)]
		private string _stateOpen;

		// Token: 0x04000416 RID: 1046
		[DataField("state_closed", false, 1, false, false, null)]
		private string _stateClosed;
	}
}
