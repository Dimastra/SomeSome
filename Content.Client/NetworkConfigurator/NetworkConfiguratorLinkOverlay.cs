using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Content.Shared.DeviceNetwork;
using Robust.Client.Graphics;
using Robust.Shared.Enums;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Map;
using Robust.Shared.Maths;
using Robust.Shared.Random;

namespace Content.Client.NetworkConfigurator
{
	// Token: 0x02000222 RID: 546
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class NetworkConfiguratorLinkOverlay : Overlay
	{
		// Token: 0x1700030C RID: 780
		// (get) Token: 0x06000E39 RID: 3641 RVA: 0x0000689B File Offset: 0x00004A9B
		public override OverlaySpace Space
		{
			get
			{
				return 4;
			}
		}

		// Token: 0x06000E3A RID: 3642 RVA: 0x00056203 File Offset: 0x00054403
		public NetworkConfiguratorLinkOverlay()
		{
			IoCManager.InjectDependencies<NetworkConfiguratorLinkOverlay>(this);
			this._deviceListSystem = this._entityManager.System<DeviceListSystem>();
		}

		// Token: 0x06000E3B RID: 3643 RVA: 0x0005622E File Offset: 0x0005442E
		public void ClearEntity(EntityUid uid)
		{
			this._colors.Remove(uid);
		}

		// Token: 0x06000E3C RID: 3644 RVA: 0x00056240 File Offset: 0x00054440
		protected override void Draw(in OverlayDrawArgs args)
		{
			foreach (NetworkConfiguratorActiveLinkOverlayComponent networkConfiguratorActiveLinkOverlayComponent in this._entityManager.EntityQuery<NetworkConfiguratorActiveLinkOverlayComponent>(false))
			{
				DeviceListComponent component;
				if (this._entityManager.Deleted(networkConfiguratorActiveLinkOverlayComponent.Owner) || !this._entityManager.TryGetComponent<DeviceListComponent>(networkConfiguratorActiveLinkOverlayComponent.Owner, ref component))
				{
					this._entityManager.RemoveComponentDeferred<NetworkConfiguratorActiveLinkOverlayComponent>(networkConfiguratorActiveLinkOverlayComponent.Owner);
				}
				else
				{
					Color value;
					if (!this._colors.TryGetValue(networkConfiguratorActiveLinkOverlayComponent.Owner, out value))
					{
						value..ctor((float)this._random.Next(0, 255), (float)this._random.Next(0, 255), (float)this._random.Next(0, 255), 1f);
						this._colors.Add(networkConfiguratorActiveLinkOverlayComponent.Owner, value);
					}
					TransformComponent component2 = this._entityManager.GetComponent<TransformComponent>(networkConfiguratorActiveLinkOverlayComponent.Owner);
					if (!(component2.MapID == MapId.Nullspace))
					{
						foreach (EntityUid entityUid in this._deviceListSystem.GetAllDevices(networkConfiguratorActiveLinkOverlayComponent.Owner, component))
						{
							if (!this._entityManager.Deleted(entityUid))
							{
								TransformComponent component3 = this._entityManager.GetComponent<TransformComponent>(entityUid);
								if (!(component3.MapID == MapId.Nullspace))
								{
									args.WorldHandle.DrawLine(component2.WorldPosition, component3.WorldPosition, this._colors[networkConfiguratorActiveLinkOverlayComponent.Owner]);
								}
							}
						}
					}
				}
			}
		}

		// Token: 0x04000707 RID: 1799
		[Dependency]
		private readonly IEntityManager _entityManager;

		// Token: 0x04000708 RID: 1800
		[Dependency]
		private readonly IRobustRandom _random;

		// Token: 0x04000709 RID: 1801
		private readonly DeviceListSystem _deviceListSystem;

		// Token: 0x0400070A RID: 1802
		private Dictionary<EntityUid, Color> _colors = new Dictionary<EntityUid, Color>();
	}
}
