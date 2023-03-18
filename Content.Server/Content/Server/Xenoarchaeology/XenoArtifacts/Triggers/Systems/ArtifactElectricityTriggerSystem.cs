using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Content.Server.Power.Components;
using Content.Server.Power.Events;
using Content.Server.Xenoarchaeology.XenoArtifacts.Triggers.Components;
using Content.Shared.Interaction;
using Content.Shared.Tools.Components;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;

namespace Content.Server.Xenoarchaeology.XenoArtifacts.Triggers.Systems
{
	// Token: 0x02000024 RID: 36
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class ArtifactElectricityTriggerSystem : EntitySystem
	{
		// Token: 0x06000089 RID: 137 RVA: 0x00004E74 File Offset: 0x00003074
		public override void Initialize()
		{
			base.Initialize();
			base.SubscribeLocalEvent<ArtifactElectricityTriggerComponent, InteractUsingEvent>(new ComponentEventHandler<ArtifactElectricityTriggerComponent, InteractUsingEvent>(this.OnInteractUsing), null, null);
			base.SubscribeLocalEvent<ArtifactElectricityTriggerComponent, PowerPulseEvent>(new ComponentEventHandler<ArtifactElectricityTriggerComponent, PowerPulseEvent>(this.OnPowerPulse), null, null);
		}

		// Token: 0x0600008A RID: 138 RVA: 0x00004EA4 File Offset: 0x000030A4
		public override void Update(float frameTime)
		{
			base.Update(frameTime);
			List<ArtifactComponent> toUpdate = new List<ArtifactComponent>();
			foreach (ValueTuple<ArtifactElectricityTriggerComponent, PowerConsumerComponent, ArtifactComponent> valueTuple in base.EntityQuery<ArtifactElectricityTriggerComponent, PowerConsumerComponent, ArtifactComponent>(false))
			{
				ArtifactElectricityTriggerComponent trigger = valueTuple.Item1;
				PowerConsumerComponent power = valueTuple.Item2;
				ArtifactComponent artifact = valueTuple.Item3;
				if (power.ReceivedPower > trigger.MinPower)
				{
					toUpdate.Add(artifact);
				}
			}
			foreach (ArtifactComponent a in toUpdate)
			{
				this._artifactSystem.TryActivateArtifact(a.Owner, null, a);
			}
		}

		// Token: 0x0600008B RID: 139 RVA: 0x00004F7C File Offset: 0x0000317C
		private void OnInteractUsing(EntityUid uid, ArtifactElectricityTriggerComponent component, InteractUsingEvent args)
		{
			if (args.Handled)
			{
				return;
			}
			ToolComponent tool;
			if (!base.TryComp<ToolComponent>(args.Used, ref tool) || !tool.Qualities.ContainsAny(new string[]
			{
				"Pulsing"
			}))
			{
				return;
			}
			args.Handled = this._artifactSystem.TryActivateArtifact(uid, new EntityUid?(args.User), null);
		}

		// Token: 0x0600008C RID: 140 RVA: 0x00004FDC File Offset: 0x000031DC
		private void OnPowerPulse(EntityUid uid, ArtifactElectricityTriggerComponent component, PowerPulseEvent args)
		{
			this._artifactSystem.TryActivateArtifact(uid, args.User, null);
		}

		// Token: 0x04000065 RID: 101
		[Dependency]
		private readonly ArtifactSystem _artifactSystem;
	}
}
