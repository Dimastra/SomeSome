using System;
using System.Runtime.CompilerServices;
using Content.Server.Atmos.Piping.Binary.Components;
using Content.Server.NodeContainer;
using Content.Server.NodeContainer.Nodes;
using Content.Shared.Atmos.Piping;
using Content.Shared.Audio;
using Content.Shared.Examine;
using Content.Shared.Interaction;
using Robust.Shared.Audio;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Localization;
using Robust.Shared.Player;

namespace Content.Server.Atmos.Piping.Binary.EntitySystems
{
	// Token: 0x0200076B RID: 1899
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class GasValveSystem : EntitySystem
	{
		// Token: 0x06002839 RID: 10297 RVA: 0x000D2B08 File Offset: 0x000D0D08
		public override void Initialize()
		{
			base.Initialize();
			base.SubscribeLocalEvent<GasValveComponent, ComponentStartup>(new ComponentEventHandler<GasValveComponent, ComponentStartup>(this.OnStartup), null, null);
			base.SubscribeLocalEvent<GasValveComponent, ActivateInWorldEvent>(new ComponentEventHandler<GasValveComponent, ActivateInWorldEvent>(this.OnActivate), null, null);
			base.SubscribeLocalEvent<GasValveComponent, ExaminedEvent>(new ComponentEventHandler<GasValveComponent, ExaminedEvent>(this.OnExamined), null, null);
		}

		// Token: 0x0600283A RID: 10298 RVA: 0x000D2B58 File Offset: 0x000D0D58
		private void OnExamined(EntityUid uid, GasValveComponent valve, ExaminedEvent args)
		{
			if (!base.Comp<TransformComponent>(valve.Owner).Anchored || !args.IsInDetailsRange)
			{
				return;
			}
			string str;
			if (Loc.TryGetString("gas-valve-system-examined", ref str, new ValueTuple<string, object>[]
			{
				new ValueTuple<string, object>("statusColor", valve.Open ? "green" : "orange"),
				new ValueTuple<string, object>("open", valve.Open)
			}))
			{
				args.PushMarkup(str);
			}
		}

		// Token: 0x0600283B RID: 10299 RVA: 0x000D2BDD File Offset: 0x000D0DDD
		private void OnStartup(EntityUid uid, GasValveComponent component, ComponentStartup args)
		{
			this.Set(uid, component, component.Open);
		}

		// Token: 0x0600283C RID: 10300 RVA: 0x000D2BF0 File Offset: 0x000D0DF0
		private void OnActivate(EntityUid uid, GasValveComponent component, ActivateInWorldEvent args)
		{
			this.Toggle(uid, component);
			SoundSystem.Play(component.ValveSound.GetSound(null, null), Filter.Pvs(component.Owner, 2f, null, null, null), component.Owner, new AudioParams?(AudioHelpers.WithVariation(0.25f)));
		}

		// Token: 0x0600283D RID: 10301 RVA: 0x000D2C40 File Offset: 0x000D0E40
		public void Set(EntityUid uid, GasValveComponent component, bool value)
		{
			component.Open = value;
			NodeContainerComponent nodeContainer;
			PipeNode inlet;
			PipeNode outlet;
			if (base.TryComp<NodeContainerComponent>(uid, ref nodeContainer) && nodeContainer.TryGetNode<PipeNode>(component.InletName, out inlet) && nodeContainer.TryGetNode<PipeNode>(component.OutletName, out outlet))
			{
				AppearanceComponent appearance;
				if (base.TryComp<AppearanceComponent>(component.Owner, ref appearance))
				{
					this._appearance.SetData(uid, FilterVisuals.Enabled, component.Open, appearance);
				}
				if (component.Open)
				{
					inlet.AddAlwaysReachable(outlet);
					outlet.AddAlwaysReachable(inlet);
					this._ambientSoundSystem.SetAmbience(component.Owner, true, null);
					return;
				}
				inlet.RemoveAlwaysReachable(outlet);
				outlet.RemoveAlwaysReachable(inlet);
				this._ambientSoundSystem.SetAmbience(component.Owner, false, null);
			}
		}

		// Token: 0x0600283E RID: 10302 RVA: 0x000D2CFE File Offset: 0x000D0EFE
		public void Toggle(EntityUid uid, GasValveComponent component)
		{
			this.Set(uid, component, !component.Open);
		}

		// Token: 0x04001900 RID: 6400
		[Dependency]
		private readonly SharedAmbientSoundSystem _ambientSoundSystem;

		// Token: 0x04001901 RID: 6401
		[Dependency]
		private readonly SharedAppearanceSystem _appearance;
	}
}
