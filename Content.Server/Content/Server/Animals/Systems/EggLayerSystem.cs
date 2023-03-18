using System;
using System.Runtime.CompilerServices;
using Content.Server.Actions;
using Content.Server.Animals.Components;
using Content.Server.Nutrition.Components;
using Content.Server.Popups;
using Content.Shared.Actions.ActionTypes;
using Content.Shared.Popups;
using Content.Shared.Storage;
using Robust.Server.GameObjects;
using Robust.Shared.Audio;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Localization;
using Robust.Shared.Player;
using Robust.Shared.Prototypes;
using Robust.Shared.Random;

namespace Content.Server.Animals.Systems
{
	// Token: 0x020007CF RID: 1999
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class EggLayerSystem : EntitySystem
	{
		// Token: 0x06002B75 RID: 11125 RVA: 0x000E3E7B File Offset: 0x000E207B
		public override void Initialize()
		{
			base.Initialize();
			base.SubscribeLocalEvent<EggLayerComponent, ComponentInit>(new ComponentEventHandler<EggLayerComponent, ComponentInit>(this.OnComponentInit), null, null);
			base.SubscribeLocalEvent<EggLayerComponent, EggLayInstantActionEvent>(new ComponentEventHandler<EggLayerComponent, EggLayInstantActionEvent>(this.OnEggLayAction), null, null);
		}

		// Token: 0x06002B76 RID: 11126 RVA: 0x000E3EAC File Offset: 0x000E20AC
		public override void Update(float frameTime)
		{
			base.Update(frameTime);
			foreach (EggLayerComponent eggLayer in base.EntityQuery<EggLayerComponent>(false))
			{
				if (base.HasComp<ActorComponent>(eggLayer.Owner))
				{
					break;
				}
				eggLayer.AccumulatedFrametime += frameTime;
				if (eggLayer.AccumulatedFrametime >= eggLayer.CurrentEggLayCooldown)
				{
					eggLayer.AccumulatedFrametime -= eggLayer.CurrentEggLayCooldown;
					eggLayer.CurrentEggLayCooldown = this._random.NextFloat(eggLayer.EggLayCooldownMin, eggLayer.EggLayCooldownMax);
					this.TryLayEgg(eggLayer.Owner, eggLayer);
				}
			}
		}

		// Token: 0x06002B77 RID: 11127 RVA: 0x000E3F64 File Offset: 0x000E2164
		private void OnComponentInit(EntityUid uid, EggLayerComponent component, ComponentInit args)
		{
			InstantActionPrototype action;
			if (!this._prototype.TryIndex<InstantActionPrototype>(component.EggLayAction, ref action))
			{
				return;
			}
			this._actions.AddAction(uid, new InstantAction(action), new EntityUid?(uid), null, true);
			component.CurrentEggLayCooldown = this._random.NextFloat(component.EggLayCooldownMin, component.EggLayCooldownMax);
		}

		// Token: 0x06002B78 RID: 11128 RVA: 0x000E3FBE File Offset: 0x000E21BE
		private void OnEggLayAction(EntityUid uid, EggLayerComponent component, EggLayInstantActionEvent args)
		{
			args.Handled = this.TryLayEgg(uid, component);
		}

		// Token: 0x06002B79 RID: 11129 RVA: 0x000E3FD0 File Offset: 0x000E21D0
		[NullableContext(2)]
		public bool TryLayEgg(EntityUid uid, EggLayerComponent component)
		{
			if (!base.Resolve<EggLayerComponent>(uid, ref component, true))
			{
				return false;
			}
			HungerComponent hunger;
			if (base.TryComp<HungerComponent>(uid, ref hunger))
			{
				if (hunger.CurrentHunger < component.HungerUsage)
				{
					this._popup.PopupEntity(Loc.GetString("action-popup-lay-egg-too-hungry"), uid, uid, PopupType.Small);
					return false;
				}
				hunger.CurrentHunger -= component.HungerUsage;
			}
			foreach (string ent in EntitySpawnCollection.GetSpawns(component.EggSpawn, this._random))
			{
				base.Spawn(ent, base.Transform(uid).Coordinates);
			}
			SoundSystem.Play(component.EggLaySound.GetSound(null, null), Filter.Pvs(uid, 2f, null, null, null), uid, new AudioParams?(component.EggLaySound.Params));
			this._popup.PopupEntity(Loc.GetString("action-popup-lay-egg-user"), uid, uid, PopupType.Small);
			this._popup.PopupEntity(Loc.GetString("action-popup-lay-egg-others", new ValueTuple<string, object>[]
			{
				new ValueTuple<string, object>("entity", uid)
			}), uid, Filter.PvsExcept(uid, 2f, null), true, PopupType.Small);
			return true;
		}

		// Token: 0x04001AE6 RID: 6886
		[Dependency]
		private readonly IRobustRandom _random;

		// Token: 0x04001AE7 RID: 6887
		[Dependency]
		private readonly IPrototypeManager _prototype;

		// Token: 0x04001AE8 RID: 6888
		[Dependency]
		private readonly ActionsSystem _actions;

		// Token: 0x04001AE9 RID: 6889
		[Dependency]
		private readonly PopupSystem _popup;
	}
}
