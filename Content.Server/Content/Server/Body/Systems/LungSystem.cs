using System;
using System.Runtime.CompilerServices;
using Content.Server.Atmos.Components;
using Content.Server.Atmos.EntitySystems;
using Content.Server.Body.Components;
using Content.Server.Chemistry.EntitySystems;
using Content.Shared.Atmos;
using Content.Shared.FixedPoint;
using Content.Shared.Inventory.Events;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;

namespace Content.Server.Body.Systems
{
	// Token: 0x02000709 RID: 1801
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class LungSystem : EntitySystem
	{
		// Token: 0x060025F5 RID: 9717 RVA: 0x000C827C File Offset: 0x000C647C
		public override void Initialize()
		{
			base.Initialize();
			base.SubscribeLocalEvent<LungComponent, ComponentInit>(new ComponentEventHandler<LungComponent, ComponentInit>(this.OnComponentInit), null, null);
			base.SubscribeLocalEvent<BreathToolComponent, GotEquippedEvent>(new ComponentEventHandler<BreathToolComponent, GotEquippedEvent>(this.OnGotEquipped), null, null);
			base.SubscribeLocalEvent<BreathToolComponent, GotUnequippedEvent>(new ComponentEventHandler<BreathToolComponent, GotUnequippedEvent>(this.OnGotUnequipped), null, null);
		}

		// Token: 0x060025F6 RID: 9718 RVA: 0x000C82CB File Offset: 0x000C64CB
		private void OnGotUnequipped(EntityUid uid, BreathToolComponent component, GotUnequippedEvent args)
		{
			this._atmosphereSystem.DisconnectInternals(component);
		}

		// Token: 0x060025F7 RID: 9719 RVA: 0x000C82DC File Offset: 0x000C64DC
		private void OnGotEquipped(EntityUid uid, BreathToolComponent component, GotEquippedEvent args)
		{
			if ((args.SlotFlags & component.AllowedSlots) != component.AllowedSlots)
			{
				return;
			}
			component.IsFunctional = true;
			InternalsComponent internals;
			if (base.TryComp<InternalsComponent>(args.Equipee, ref internals))
			{
				component.ConnectedInternalsEntity = new EntityUid?(args.Equipee);
				this._internals.ConnectBreathTool(internals, uid);
			}
		}

		// Token: 0x060025F8 RID: 9720 RVA: 0x000C8334 File Offset: 0x000C6534
		private void OnComponentInit(EntityUid uid, LungComponent component, ComponentInit args)
		{
			component.LungSolution = this._solutionContainerSystem.EnsureSolution(uid, LungSystem.LungSolutionName, null);
			component.LungSolution.MaxVolume = 100f;
			component.LungSolution.CanReact = false;
		}

		// Token: 0x060025F9 RID: 9721 RVA: 0x000C8370 File Offset: 0x000C6570
		public void GasToReagent(EntityUid uid, LungComponent lung)
		{
			foreach (int i in Enum.GetValues<Gas>())
			{
				float moles = lung.Air.Moles[i];
				if (moles > 0f)
				{
					string reagent = this._atmosphereSystem.GasReagents[i];
					if (reagent != null)
					{
						float amount = moles * 1144f;
						FixedPoint2 fixedPoint;
						this._solutionContainerSystem.TryAddReagent(uid, lung.LungSolution, reagent, amount, out fixedPoint, null);
					}
				}
			}
		}

		// Token: 0x0400176C RID: 5996
		[Dependency]
		private readonly InternalsSystem _internals;

		// Token: 0x0400176D RID: 5997
		[Dependency]
		private readonly SolutionContainerSystem _solutionContainerSystem;

		// Token: 0x0400176E RID: 5998
		[Dependency]
		private readonly AtmosphereSystem _atmosphereSystem;

		// Token: 0x0400176F RID: 5999
		public static string LungSolutionName = "Lung";
	}
}
