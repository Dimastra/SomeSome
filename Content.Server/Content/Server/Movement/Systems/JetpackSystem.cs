using System;
using System.Runtime.CompilerServices;
using Content.Server.Atmos;
using Content.Server.Atmos.Components;
using Content.Server.Atmos.EntitySystems;
using Content.Shared.Movement.Components;
using Content.Shared.Movement.Systems;
using Robust.Shared.Collections;
using Robust.Shared.IoC;
using Robust.Shared.Maths;
using Robust.Shared.Timing;

namespace Content.Server.Movement.Systems
{
	// Token: 0x0200038F RID: 911
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class JetpackSystem : SharedJetpackSystem
	{
		// Token: 0x060012AF RID: 4783 RVA: 0x00060CC0 File Offset: 0x0005EEC0
		protected override bool CanEnable(JetpackComponent component)
		{
			GasTankComponent gasTank;
			return base.CanEnable(component) && base.TryComp<GasTankComponent>(component.Owner, ref gasTank) && gasTank.Air.TotalMoles >= component.MoleUsage;
		}

		// Token: 0x060012B0 RID: 4784 RVA: 0x00060D00 File Offset: 0x0005EF00
		public override void Update(float frameTime)
		{
			base.Update(frameTime);
			ValueList<JetpackComponent> toDisable = default(ValueList<JetpackComponent>);
			foreach (ValueTuple<ActiveJetpackComponent, JetpackComponent, GasTankComponent> valueTuple in base.EntityQuery<ActiveJetpackComponent, JetpackComponent, GasTankComponent>(false))
			{
				ActiveJetpackComponent active = valueTuple.Item1;
				JetpackComponent comp = valueTuple.Item2;
				GasTankComponent gasTank = valueTuple.Item3;
				if (!(this._timing.CurTime < active.TargetTime))
				{
					active.TargetTime = this._timing.CurTime + TimeSpan.FromSeconds((double)active.EffectCooldown);
					GasMixture air = this._gasTank.RemoveAir(gasTank, comp.MoleUsage);
					if (air == null || !MathHelper.CloseTo(air.TotalMoles, comp.MoleUsage, 0.001f))
					{
						toDisable.Add(comp);
					}
					else
					{
						this._gasTank.UpdateUserInterface(gasTank, false);
					}
				}
			}
			foreach (JetpackComponent comp2 in toDisable)
			{
				base.SetEnabled(comp2, false, null);
			}
		}

		// Token: 0x04000B70 RID: 2928
		[Dependency]
		private readonly GasTankSystem _gasTank;

		// Token: 0x04000B71 RID: 2929
		[Dependency]
		private readonly IGameTiming _timing;

		// Token: 0x04000B72 RID: 2930
		private const float UpdateCooldown = 0.5f;
	}
}
