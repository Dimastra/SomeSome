using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Content.Client.UserInterface.Fragments;
using Content.Shared.Mech;
using Robust.Client.GameObjects;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;

namespace Content.Client.Mech.Ui
{
	// Token: 0x0200023E RID: 574
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class MechBoundUserInterface : BoundUserInterface
	{
		// Token: 0x06000E8D RID: 3725 RVA: 0x00057C46 File Offset: 0x00055E46
		public MechBoundUserInterface(ClientUserInterfaceComponent owner, Enum uiKey) : base(owner, uiKey)
		{
			IoCManager.InjectDependencies<MechBoundUserInterface>(this);
			this._mech = owner.Owner;
		}

		// Token: 0x06000E8E RID: 3726 RVA: 0x00057C64 File Offset: 0x00055E64
		protected override void Open()
		{
			base.Open();
			this._menu = new MechMenu(this._mech);
			this._menu.OnClose += base.Close;
			this._menu.OpenCenteredLeft();
			this._menu.OnRemoveButtonPressed += delegate(EntityUid uid)
			{
				base.SendMessage(new MechEquipmentRemoveMessage(uid));
			};
		}

		// Token: 0x06000E8F RID: 3727 RVA: 0x00057CC4 File Offset: 0x00055EC4
		protected override void UpdateState(BoundUserInterfaceState state)
		{
			base.UpdateState(state);
			MechBoundUiState mechBoundUiState = state as MechBoundUiState;
			if (mechBoundUiState == null)
			{
				return;
			}
			this.UpdateEquipmentControls(mechBoundUiState);
			MechMenu menu = this._menu;
			if (menu != null)
			{
				menu.UpdateMechStats();
			}
			MechMenu menu2 = this._menu;
			if (menu2 == null)
			{
				return;
			}
			menu2.UpdateEquipmentView();
		}

		// Token: 0x06000E90 RID: 3728 RVA: 0x00057D0C File Offset: 0x00055F0C
		public void UpdateEquipmentControls(MechBoundUiState state)
		{
			MechComponent mechComponent;
			if (!this._ent.TryGetComponent<MechComponent>(this._mech, ref mechComponent))
			{
				return;
			}
			foreach (EntityUid entityUid in mechComponent.EquipmentContainer.ContainedEntities)
			{
				UIFragment equipmentUi = this.GetEquipmentUi(new EntityUid?(entityUid));
				if (equipmentUi != null)
				{
					foreach (KeyValuePair<EntityUid, BoundUserInterfaceState> keyValuePair in state.EquipmentStates)
					{
						EntityUid entityUid2;
						BoundUserInterfaceState boundUserInterfaceState;
						keyValuePair.Deconstruct(out entityUid2, out boundUserInterfaceState);
						EntityUid entityUid3 = entityUid2;
						BoundUserInterfaceState state2 = boundUserInterfaceState;
						if (entityUid == entityUid3)
						{
							equipmentUi.UpdateState(state2);
						}
					}
				}
			}
		}

		// Token: 0x06000E91 RID: 3729 RVA: 0x00057DE0 File Offset: 0x00055FE0
		protected override void Dispose(bool disposing)
		{
			base.Dispose(disposing);
			if (!disposing)
			{
				return;
			}
			MechMenu menu = this._menu;
			if (menu == null)
			{
				return;
			}
			menu.Close();
		}

		// Token: 0x06000E92 RID: 3730 RVA: 0x00057DFD File Offset: 0x00055FFD
		[NullableContext(2)]
		public UIFragment GetEquipmentUi(EntityUid? uid)
		{
			UIFragmentComponent componentOrNull = EntityManagerExt.GetComponentOrNull<UIFragmentComponent>(this._ent, uid);
			if (componentOrNull != null)
			{
				UIFragment ui = componentOrNull.Ui;
				if (ui != null)
				{
					ui.Setup(this, uid);
				}
			}
			if (componentOrNull == null)
			{
				return null;
			}
			return componentOrNull.Ui;
		}

		// Token: 0x0400073D RID: 1853
		[Dependency]
		private readonly IEntityManager _ent;

		// Token: 0x0400073E RID: 1854
		private readonly EntityUid _mech;

		// Token: 0x0400073F RID: 1855
		[Nullable(2)]
		private MechMenu _menu;
	}
}
