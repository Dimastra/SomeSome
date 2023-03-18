using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Content.Server.UserInterface;
using Content.Shared.Shuttles.BUIStates;
using Content.Shared.Shuttles.Components;
using Content.Shared.Shuttles.Systems;
using Robust.Server.GameObjects;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Map;
using Robust.Shared.Maths;

namespace Content.Server.Shuttles.Systems
{
	// Token: 0x020001F7 RID: 503
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class RadarConsoleSystem : SharedRadarConsoleSystem
	{
		// Token: 0x060009CD RID: 2509 RVA: 0x000323C1 File Offset: 0x000305C1
		public override void Initialize()
		{
			base.Initialize();
			base.SubscribeLocalEvent<RadarConsoleComponent, ComponentStartup>(new ComponentEventHandler<RadarConsoleComponent, ComponentStartup>(this.OnRadarStartup), null, null);
		}

		// Token: 0x060009CE RID: 2510 RVA: 0x000323DD File Offset: 0x000305DD
		private void OnRadarStartup(EntityUid uid, RadarConsoleComponent component, ComponentStartup args)
		{
			this.UpdateState(component);
		}

		// Token: 0x060009CF RID: 2511 RVA: 0x000323E8 File Offset: 0x000305E8
		protected override void UpdateState(RadarConsoleComponent component)
		{
			TransformComponent xform = base.Transform(component.Owner);
			bool flag = xform.ParentUid == xform.GridUid;
			EntityCoordinates? coordinates = flag ? new EntityCoordinates?(xform.Coordinates) : null;
			Angle? angle = flag ? new Angle?(xform.LocalRotation) : null;
			IntrinsicUIComponent intrinsic;
			if (base.TryComp<IntrinsicUIComponent>(component.Owner, ref intrinsic))
			{
				foreach (IntrinsicUIEntry uiKey in intrinsic.UIs)
				{
					Enum key = uiKey.Key;
					if (key != null && key.Equals(RadarConsoleUiKey.Key))
					{
						coordinates = new EntityCoordinates?(new EntityCoordinates(component.Owner, Vector2.Zero));
						angle = new Angle?(Angle.Zero);
						break;
					}
				}
			}
			RadarConsoleBoundInterfaceState radarState = new RadarConsoleBoundInterfaceState(component.MaxRange, coordinates, angle, new List<DockingInterfaceState>());
			BoundUserInterface uiOrNull = this._uiSystem.GetUiOrNull(component.Owner, RadarConsoleUiKey.Key, null);
			if (uiOrNull == null)
			{
				return;
			}
			uiOrNull.SetState(radarState, null, true);
		}

		// Token: 0x040005E4 RID: 1508
		[Dependency]
		private readonly UserInterfaceSystem _uiSystem;
	}
}
