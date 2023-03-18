using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Content.Server.Body.Components;
using Content.Server.Body.Systems;
using Content.Server.Chemistry.EntitySystems;
using Content.Server.Fluids.Components;
using Content.Server.Nutrition.Components;
using Content.Server.Nutrition.EntitySystems;
using Content.Server.Popups;
using Content.Server.Stunnable;
using Content.Shared.Audio;
using Content.Shared.Body.Organ;
using Content.Shared.Chemistry.Components;
using Content.Shared.IdentityManagement;
using Content.Shared.Popups;
using Content.Shared.StatusEffect;
using Robust.Shared.Audio;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Localization;
using Robust.Shared.Player;

namespace Content.Server.Medical
{
	// Token: 0x020003B4 RID: 948
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class VomitSystem : EntitySystem
	{
		// Token: 0x0600138F RID: 5007 RVA: 0x00065084 File Offset: 0x00063284
		public void Vomit(EntityUid uid, float thirstAdded = -40f, float hungerAdded = -40f)
		{
			List<ValueTuple<StomachComponent, OrganComponent>> stomachList = this._bodySystem.GetBodyOrganComponents<StomachComponent>(uid, null);
			if (stomachList.Count == 0)
			{
				return;
			}
			HungerComponent hunger;
			if (base.TryComp<HungerComponent>(uid, ref hunger))
			{
				hunger.UpdateFood(hungerAdded);
			}
			ThirstComponent thirst;
			if (base.TryComp<ThirstComponent>(uid, ref thirst))
			{
				this._thirstSystem.UpdateThirst(thirst, thirstAdded);
			}
			float solutionSize = (Math.Abs(thirstAdded) + Math.Abs(hungerAdded)) / 6f;
			StatusEffectsComponent status;
			if (base.TryComp<StatusEffectsComponent>(uid, ref status))
			{
				this._stunSystem.TrySlowdown(uid, TimeSpan.FromSeconds((double)solutionSize), true, 0.5f, 0.5f, status);
			}
			EntityUid puddle = this.EntityManager.SpawnEntity("PuddleVomit", base.Transform(uid).Coordinates);
			PuddleComponent puddleComp = base.Comp<PuddleComponent>(puddle);
			SoundSystem.Play("/Audio/Effects/Fluids/splat.ogg", Filter.Pvs(uid, 2f, null, null, null), uid, new AudioParams?(AudioHelpers.WithVariation(0.2f).WithVolume(-4f)));
			this._popupSystem.PopupEntity(Loc.GetString("disease-vomit", new ValueTuple<string, object>[]
			{
				new ValueTuple<string, object>("person", Identity.Entity(uid, this.EntityManager))
			}), uid, PopupType.Small);
			Solution puddleSolution;
			if (!this._solutionSystem.TryGetSolution(puddle, puddleComp.SolutionName, out puddleSolution, null))
			{
				return;
			}
			foreach (ValueTuple<StomachComponent, OrganComponent> stomach in stomachList)
			{
				Solution sol;
				if (this._solutionSystem.TryGetSolution(stomach.Item1.Owner, "stomach", out sol, null))
				{
					this._solutionSystem.TryAddSolution(puddle, puddleSolution, sol);
				}
			}
			BloodstreamComponent bloodStream;
			if (base.TryComp<BloodstreamComponent>(uid, ref bloodStream))
			{
				Solution temp = bloodStream.ChemicalSolution.SplitSolution(solutionSize);
				this._solutionSystem.TryAddSolution(puddle, puddleSolution, temp);
			}
		}

		// Token: 0x04000BE1 RID: 3041
		[Dependency]
		private readonly StunSystem _stunSystem;

		// Token: 0x04000BE2 RID: 3042
		[Dependency]
		private readonly SolutionContainerSystem _solutionSystem;

		// Token: 0x04000BE3 RID: 3043
		[Dependency]
		private readonly PopupSystem _popupSystem;

		// Token: 0x04000BE4 RID: 3044
		[Dependency]
		private readonly BodySystem _bodySystem;

		// Token: 0x04000BE5 RID: 3045
		[Dependency]
		private readonly ThirstSystem _thirstSystem;
	}
}
