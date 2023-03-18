using System;
using System.Runtime.CompilerServices;
using Content.Server.Disposal.Unit.Components;
using Content.Server.UserInterface;
using Content.Shared.Disposal.Components;
using Robust.Server.GameObjects;
using Robust.Shared.Audio;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Maths;
using Robust.Shared.Physics.Components;
using Robust.Shared.Player;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.ViewVariables;

namespace Content.Server.Disposal.Tube.Components
{
	// Token: 0x0200055B RID: 1371
	[NullableContext(1)]
	[Nullable(0)]
	[RegisterComponent]
	[ComponentReference(typeof(IDisposalTubeComponent))]
	[ComponentReference(typeof(DisposalTubeComponent))]
	public sealed class DisposalTaggerComponent : DisposalTransitComponent
	{
		// Token: 0x17000456 RID: 1110
		// (get) Token: 0x06001CF5 RID: 7413 RVA: 0x0009A1F7 File Offset: 0x000983F7
		public override string ContainerId
		{
			get
			{
				return "DisposalTagger";
			}
		}

		// Token: 0x17000457 RID: 1111
		// (get) Token: 0x06001CF6 RID: 7414 RVA: 0x0009A200 File Offset: 0x00098400
		[ViewVariables]
		public bool Anchored
		{
			get
			{
				PhysicsComponent physics;
				return !this._entMan.TryGetComponent<PhysicsComponent>(base.Owner, ref physics) || physics.BodyType == 4;
			}
		}

		// Token: 0x17000458 RID: 1112
		// (get) Token: 0x06001CF7 RID: 7415 RVA: 0x0009A22D File Offset: 0x0009842D
		[Nullable(2)]
		[ViewVariables]
		public BoundUserInterface UserInterface
		{
			[NullableContext(2)]
			get
			{
				return base.Owner.GetUIOrNull(SharedDisposalTaggerComponent.DisposalTaggerUiKey.Key);
			}
		}

		// Token: 0x06001CF8 RID: 7416 RVA: 0x0009A240 File Offset: 0x00098440
		public override Direction NextDirection(DisposalHolderComponent holder)
		{
			holder.Tags.Add(this.Tag);
			return base.NextDirection(holder);
		}

		// Token: 0x06001CF9 RID: 7417 RVA: 0x0009A25B File Offset: 0x0009845B
		protected override void Initialize()
		{
			base.Initialize();
			if (this.UserInterface != null)
			{
				this.UserInterface.OnReceiveMessage += this.OnUiReceiveMessage;
			}
		}

		// Token: 0x06001CFA RID: 7418 RVA: 0x0009A284 File Offset: 0x00098484
		private void OnUiReceiveMessage(ServerBoundUserInterfaceMessage obj)
		{
			SharedDisposalTaggerComponent.UiActionMessage msg = (SharedDisposalTaggerComponent.UiActionMessage)obj.Message;
			if (!this.Anchored)
			{
				return;
			}
			if (msg.Action == SharedDisposalTaggerComponent.UiAction.Ok && SharedDisposalTaggerComponent.TagRegex.IsMatch(msg.Tag))
			{
				this.Tag = msg.Tag;
				this.ClickSound();
			}
		}

		// Token: 0x06001CFB RID: 7419 RVA: 0x0009A2D4 File Offset: 0x000984D4
		private void ClickSound()
		{
			SoundSystem.Play(this._clickSound.GetSound(null, null), Filter.Pvs(base.Owner, 2f, null, null, null), base.Owner, new AudioParams?(AudioParams.Default.WithVolume(-2f)));
		}

		// Token: 0x06001CFC RID: 7420 RVA: 0x0009A321 File Offset: 0x00098521
		protected override void OnRemove()
		{
			base.OnRemove();
			BoundUserInterface userInterface = this.UserInterface;
			if (userInterface == null)
			{
				return;
			}
			userInterface.CloseAll();
		}

		// Token: 0x04001281 RID: 4737
		[Dependency]
		private readonly IEntityManager _entMan;

		// Token: 0x04001282 RID: 4738
		[ViewVariables]
		[DataField("tag", false, 1, false, false, null)]
		public string Tag = "";

		// Token: 0x04001283 RID: 4739
		[DataField("clickSound", false, 1, false, false, null)]
		private SoundSpecifier _clickSound = new SoundPathSpecifier("/Audio/Machines/machine_switch.ogg", null);
	}
}
