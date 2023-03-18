using System;
using System.Runtime.CompilerServices;
using Content.Server.Xenoarchaeology.XenoArtifacts.Triggers.Components;
using Content.Shared.Interaction;
using Content.Shared.Physics.Pull;
using Content.Shared.Weapons.Melee.Events;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;

namespace Content.Server.Xenoarchaeology.XenoArtifacts.Triggers.Systems
{
	// Token: 0x02000028 RID: 40
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class ArtifactInteractionTriggerSystem : EntitySystem
	{
		// Token: 0x0600009B RID: 155 RVA: 0x000053B0 File Offset: 0x000035B0
		public override void Initialize()
		{
			base.Initialize();
			base.SubscribeLocalEvent<ArtifactInteractionTriggerComponent, PullStartedMessage>(new ComponentEventHandler<ArtifactInteractionTriggerComponent, PullStartedMessage>(this.OnPull), null, null);
			base.SubscribeLocalEvent<ArtifactInteractionTriggerComponent, AttackedEvent>(new ComponentEventHandler<ArtifactInteractionTriggerComponent, AttackedEvent>(this.OnAttack), null, null);
			base.SubscribeLocalEvent<ArtifactInteractionTriggerComponent, InteractHandEvent>(new ComponentEventHandler<ArtifactInteractionTriggerComponent, InteractHandEvent>(this.OnInteract), null, null);
		}

		// Token: 0x0600009C RID: 156 RVA: 0x000053FF File Offset: 0x000035FF
		private void OnPull(EntityUid uid, ArtifactInteractionTriggerComponent component, PullStartedMessage args)
		{
			if (!component.PullActivation)
			{
				return;
			}
			this._artifactSystem.TryActivateArtifact(uid, new EntityUid?(args.Puller.Owner), null);
		}

		// Token: 0x0600009D RID: 157 RVA: 0x00005428 File Offset: 0x00003628
		private void OnAttack(EntityUid uid, ArtifactInteractionTriggerComponent component, AttackedEvent args)
		{
			if (!component.AttackActivation)
			{
				return;
			}
			this._artifactSystem.TryActivateArtifact(uid, new EntityUid?(args.User), null);
		}

		// Token: 0x0600009E RID: 158 RVA: 0x0000544C File Offset: 0x0000364C
		private void OnInteract(EntityUid uid, ArtifactInteractionTriggerComponent component, InteractHandEvent args)
		{
			if (args.Handled)
			{
				return;
			}
			if (!component.EmptyHandActivation)
			{
				return;
			}
			args.Handled = this._artifactSystem.TryActivateArtifact(uid, new EntityUid?(args.User), null);
		}

		// Token: 0x0400006D RID: 109
		[Dependency]
		private readonly ArtifactSystem _artifactSystem;
	}
}
