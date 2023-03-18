using System;
using System.Collections.Generic;
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
	// Token: 0x0200055A RID: 1370
	[NullableContext(1)]
	[Nullable(0)]
	[RegisterComponent]
	[ComponentReference(typeof(IDisposalTubeComponent))]
	[ComponentReference(typeof(DisposalTubeComponent))]
	public sealed class DisposalRouterComponent : DisposalJunctionComponent
	{
		// Token: 0x17000453 RID: 1107
		// (get) Token: 0x06001CEC RID: 7404 RVA: 0x0009A009 File Offset: 0x00098209
		public override string ContainerId
		{
			get
			{
				return "DisposalRouter";
			}
		}

		// Token: 0x17000454 RID: 1108
		// (get) Token: 0x06001CED RID: 7405 RVA: 0x0009A010 File Offset: 0x00098210
		[ViewVariables]
		public bool Anchored
		{
			get
			{
				PhysicsComponent physics;
				return !this._entMan.TryGetComponent<PhysicsComponent>(base.Owner, ref physics) || physics.BodyType == 4;
			}
		}

		// Token: 0x17000455 RID: 1109
		// (get) Token: 0x06001CEE RID: 7406 RVA: 0x0009A03D File Offset: 0x0009823D
		[Nullable(2)]
		[ViewVariables]
		public BoundUserInterface UserInterface
		{
			[NullableContext(2)]
			get
			{
				return base.Owner.GetUIOrNull(SharedDisposalRouterComponent.DisposalRouterUiKey.Key);
			}
		}

		// Token: 0x06001CEF RID: 7407 RVA: 0x0009A050 File Offset: 0x00098250
		public override Direction NextDirection(DisposalHolderComponent holder)
		{
			Direction[] directions = this.ConnectableDirections();
			if (holder.Tags.Overlaps(this.Tags))
			{
				return directions[1];
			}
			return this._entMan.GetComponent<TransformComponent>(base.Owner).LocalRotation.GetDir();
		}

		// Token: 0x06001CF0 RID: 7408 RVA: 0x0009A099 File Offset: 0x00098299
		protected override void Initialize()
		{
			base.Initialize();
			if (this.UserInterface != null)
			{
				this.UserInterface.OnReceiveMessage += this.OnUiReceiveMessage;
			}
		}

		// Token: 0x06001CF1 RID: 7409 RVA: 0x0009A0C0 File Offset: 0x000982C0
		private void OnUiReceiveMessage(ServerBoundUserInterfaceMessage obj)
		{
			if (obj.Session.AttachedEntity == null)
			{
				return;
			}
			SharedDisposalRouterComponent.UiActionMessage msg = (SharedDisposalRouterComponent.UiActionMessage)obj.Message;
			if (!this.Anchored)
			{
				return;
			}
			if (msg.Action == SharedDisposalRouterComponent.UiAction.Ok && SharedDisposalRouterComponent.TagRegex.IsMatch(msg.Tags))
			{
				this.Tags.Clear();
				foreach (string tag in msg.Tags.Split(',', StringSplitOptions.RemoveEmptyEntries))
				{
					this.Tags.Add(tag.Trim());
					this.ClickSound();
				}
			}
		}

		// Token: 0x06001CF2 RID: 7410 RVA: 0x0009A158 File Offset: 0x00098358
		private void ClickSound()
		{
			SoundSystem.Play(this._clickSound.GetSound(null, null), Filter.Pvs(base.Owner, 2f, null, null, null), base.Owner, new AudioParams?(AudioParams.Default.WithVolume(-2f)));
		}

		// Token: 0x06001CF3 RID: 7411 RVA: 0x0009A1A5 File Offset: 0x000983A5
		protected override void OnRemove()
		{
			BoundUserInterface userInterface = this.UserInterface;
			if (userInterface != null)
			{
				userInterface.CloseAll();
			}
			base.OnRemove();
		}

		// Token: 0x0400127E RID: 4734
		[Dependency]
		private readonly IEntityManager _entMan;

		// Token: 0x0400127F RID: 4735
		[DataField("tags", false, 1, false, false, null)]
		public HashSet<string> Tags = new HashSet<string>();

		// Token: 0x04001280 RID: 4736
		[DataField("clickSound", false, 1, false, false, null)]
		private SoundSpecifier _clickSound = new SoundPathSpecifier("/Audio/Machines/machine_switch.ogg", null);
	}
}
