using System;
using System.Runtime.CompilerServices;
using Content.Server.Solar.Components;
using Content.Server.UserInterface;
using Content.Shared.Solar;
using Robust.Server.GameObjects;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Maths;

namespace Content.Server.Solar.EntitySystems
{
	// Token: 0x020001DE RID: 478
	[NullableContext(1)]
	[Nullable(0)]
	internal sealed class PowerSolarControlConsoleSystem : EntitySystem
	{
		// Token: 0x06000913 RID: 2323 RVA: 0x0002DB5D File Offset: 0x0002BD5D
		public override void Initialize()
		{
			base.Initialize();
			base.SubscribeLocalEvent<SolarControlConsoleComponent, SolarControlConsoleAdjustMessage>(new ComponentEventHandler<SolarControlConsoleComponent, SolarControlConsoleAdjustMessage>(this.OnUIMessage), null, null);
		}

		// Token: 0x06000914 RID: 2324 RVA: 0x0002DB7C File Offset: 0x0002BD7C
		public override void Update(float frameTime)
		{
			this._updateTimer += frameTime;
			if (this._updateTimer >= 1f)
			{
				this._updateTimer -= 1f;
				SolarControlConsoleBoundInterfaceState state = new SolarControlConsoleBoundInterfaceState(this._powerSolarSystem.TargetPanelRotation, this._powerSolarSystem.TargetPanelVelocity, this._powerSolarSystem.TotalPanelPower, this._powerSolarSystem.TowardsSun);
				foreach (SolarControlConsoleComponent solarControlConsoleComponent in this.EntityManager.EntityQuery<SolarControlConsoleComponent>(false))
				{
					BoundUserInterface uiorNull = solarControlConsoleComponent.Owner.GetUIOrNull(SolarControlConsoleUiKey.Key);
					if (uiorNull != null)
					{
						uiorNull.SetState(state, null, true);
					}
				}
			}
		}

		// Token: 0x06000915 RID: 2325 RVA: 0x0002DC48 File Offset: 0x0002BE48
		private void OnUIMessage(EntityUid uid, SolarControlConsoleComponent component, SolarControlConsoleAdjustMessage msg)
		{
			if (double.IsFinite(msg.Rotation))
			{
				this._powerSolarSystem.TargetPanelRotation = msg.Rotation.Reduced();
			}
			if (double.IsFinite(msg.AngularVelocity))
			{
				double degrees = msg.AngularVelocity.Degrees;
				degrees = Math.Clamp(degrees, -1.0, 1.0);
				this._powerSolarSystem.TargetPanelVelocity = Angle.FromDegrees(degrees);
			}
		}

		// Token: 0x0400057F RID: 1407
		[Dependency]
		private PowerSolarSystem _powerSolarSystem;

		// Token: 0x04000580 RID: 1408
		private float _updateTimer;
	}
}
