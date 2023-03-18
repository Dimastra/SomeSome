using System;
using System.Linq;
using System.Runtime.CompilerServices;
using Content.Server.Disposal.Unit.Components;
using Content.Server.Disposal.Unit.EntitySystems;
using Content.Shared.Construction.Components;
using Content.Shared.Popups;
using Robust.Shared.Audio;
using Robust.Shared.Containers;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Localization;
using Robust.Shared.Maths;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.ViewVariables;

namespace Content.Server.Disposal.Tube.Components
{
	// Token: 0x0200055D RID: 1373
	[NullableContext(1)]
	[Nullable(0)]
	public abstract class DisposalTubeComponent : Component, IDisposalTubeComponent, IComponent
	{
		// Token: 0x1700045A RID: 1114
		// (get) Token: 0x06001D02 RID: 7426 RVA: 0x0009A408 File Offset: 0x00098608
		public virtual string ContainerId
		{
			get
			{
				return "DisposalTube";
			}
		}

		// Token: 0x1700045B RID: 1115
		// (get) Token: 0x06001D03 RID: 7427 RVA: 0x0009A40F File Offset: 0x0009860F
		// (set) Token: 0x06001D04 RID: 7428 RVA: 0x0009A417 File Offset: 0x00098617
		[ViewVariables]
		public Container Contents { get; private set; }

		// Token: 0x06001D05 RID: 7429
		protected abstract Direction[] ConnectableDirections();

		// Token: 0x06001D06 RID: 7430
		public abstract Direction NextDirection(DisposalHolderComponent holder);

		// Token: 0x06001D07 RID: 7431 RVA: 0x0009A420 File Offset: 0x00098620
		public void Connect()
		{
			if (this._connected)
			{
				return;
			}
			this._connected = true;
		}

		// Token: 0x06001D08 RID: 7432 RVA: 0x0009A432 File Offset: 0x00098632
		public bool CanConnect(Direction direction, IDisposalTubeComponent with)
		{
			return this._connected && this.ConnectableDirections().Contains(direction);
		}

		// Token: 0x06001D09 RID: 7433 RVA: 0x0009A450 File Offset: 0x00098650
		public void Disconnect()
		{
			if (!this._connected)
			{
				return;
			}
			this._connected = false;
			foreach (EntityUid entity in this.Contents.ContainedEntities.ToArray<EntityUid>())
			{
				DisposalHolderComponent holder;
				if (this._entMan.TryGetComponent<DisposalHolderComponent>(entity, ref holder))
				{
					EntitySystem.Get<DisposableSystem>().ExitDisposals(holder.Owner, null, null);
				}
			}
		}

		// Token: 0x06001D0A RID: 7434 RVA: 0x0009A4B8 File Offset: 0x000986B8
		public void PopupDirections(EntityUid entity)
		{
			string directions = string.Join<Direction>(", ", this.ConnectableDirections());
			base.Owner.PopupMessage(entity, Loc.GetString("disposal-tube-component-popup-directions-text", new ValueTuple<string, object>[]
			{
				new ValueTuple<string, object>("directions", directions)
			}));
		}

		// Token: 0x06001D0B RID: 7435 RVA: 0x0009A504 File Offset: 0x00098704
		protected override void Initialize()
		{
			base.Initialize();
			this.Contents = ContainerHelpers.EnsureContainer<Container>(base.Owner, this.ContainerId, null);
			ComponentExt.EnsureComponent<AnchorableComponent>(base.Owner);
		}

		// Token: 0x06001D0C RID: 7436 RVA: 0x0009A530 File Offset: 0x00098730
		protected override void OnRemove()
		{
			base.OnRemove();
			this.Disconnect();
		}

		// Token: 0x04001284 RID: 4740
		[Dependency]
		private readonly IEntityManager _entMan;

		// Token: 0x04001285 RID: 4741
		public static readonly TimeSpan ClangDelay = TimeSpan.FromSeconds(0.5);

		// Token: 0x04001286 RID: 4742
		public TimeSpan LastClang;

		// Token: 0x04001287 RID: 4743
		private bool _connected;

		// Token: 0x04001288 RID: 4744
		[DataField("clangSound", false, 1, false, false, null)]
		public SoundSpecifier ClangSound = new SoundPathSpecifier("/Audio/Effects/clang.ogg", null);
	}
}
