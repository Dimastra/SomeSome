using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Content.Server.Atmos;
using Content.Server.Atmos.EntitySystems;
using Content.Server.Xenoarchaeology.XenoArtifacts.Triggers.Components;
using Content.Shared.Interaction;
using Content.Shared.Temperature;
using Content.Shared.Weapons.Melee.Events;
using Robust.Server.GameObjects;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;

namespace Content.Server.Xenoarchaeology.XenoArtifacts.Triggers.Systems
{
	// Token: 0x02000027 RID: 39
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class ArtifactHeatTriggerSystem : EntitySystem
	{
		// Token: 0x06000095 RID: 149 RVA: 0x000051D4 File Offset: 0x000033D4
		public override void Initialize()
		{
			base.Initialize();
			base.SubscribeLocalEvent<ArtifactHeatTriggerComponent, AttackedEvent>(new ComponentEventHandler<ArtifactHeatTriggerComponent, AttackedEvent>(this.OnAttacked), null, null);
			base.SubscribeLocalEvent<ArtifactHeatTriggerComponent, InteractUsingEvent>(new ComponentEventHandler<ArtifactHeatTriggerComponent, InteractUsingEvent>(this.OnUsing), null, null);
		}

		// Token: 0x06000096 RID: 150 RVA: 0x00005204 File Offset: 0x00003404
		public override void Update(float frameTime)
		{
			base.Update(frameTime);
			List<ArtifactComponent> toUpdate = new List<ArtifactComponent>();
			foreach (ValueTuple<ArtifactHeatTriggerComponent, TransformComponent, ArtifactComponent> valueTuple in base.EntityQuery<ArtifactHeatTriggerComponent, TransformComponent, ArtifactComponent>(false))
			{
				ArtifactHeatTriggerComponent trigger = valueTuple.Item1;
				TransformComponent transform = valueTuple.Item2;
				ArtifactComponent artifact = valueTuple.Item3;
				EntityUid uid = trigger.Owner;
				GasMixture environment = this._atmosphereSystem.GetTileMixture(transform.GridUid, transform.MapUid, this._transformSystem.GetGridOrMapTilePosition(uid, transform), false);
				if (environment != null && environment.Temperature >= trigger.ActivationTemperature)
				{
					toUpdate.Add(artifact);
				}
			}
			foreach (ArtifactComponent a in toUpdate)
			{
				this._artifactSystem.TryActivateArtifact(a.Owner, null, a);
			}
		}

		// Token: 0x06000097 RID: 151 RVA: 0x00005310 File Offset: 0x00003510
		private void OnAttacked(EntityUid uid, ArtifactHeatTriggerComponent component, AttackedEvent args)
		{
			if (!component.ActivateHotItems || !this.CheckHot(args.Used))
			{
				return;
			}
			this._artifactSystem.TryActivateArtifact(uid, new EntityUid?(args.User), null);
		}

		// Token: 0x06000098 RID: 152 RVA: 0x00005342 File Offset: 0x00003542
		private void OnUsing(EntityUid uid, ArtifactHeatTriggerComponent component, InteractUsingEvent args)
		{
			if (args.Handled)
			{
				return;
			}
			if (!component.ActivateHotItems || !this.CheckHot(args.Used))
			{
				return;
			}
			args.Handled = this._artifactSystem.TryActivateArtifact(uid, new EntityUid?(args.User), null);
		}

		// Token: 0x06000099 RID: 153 RVA: 0x00005384 File Offset: 0x00003584
		private bool CheckHot(EntityUid usedUid)
		{
			IsHotEvent hotEvent = new IsHotEvent();
			base.RaiseLocalEvent<IsHotEvent>(usedUid, hotEvent, false);
			return hotEvent.IsHot;
		}

		// Token: 0x0400006A RID: 106
		[Dependency]
		private readonly AtmosphereSystem _atmosphereSystem;

		// Token: 0x0400006B RID: 107
		[Dependency]
		private readonly ArtifactSystem _artifactSystem;

		// Token: 0x0400006C RID: 108
		[Dependency]
		private readonly TransformSystem _transformSystem;
	}
}
