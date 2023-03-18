using System;
using System.Runtime.CompilerServices;
using Content.Client.UserInterface.Fragments;
using Content.Shared.Mech;
using Robust.Client.GameObjects;
using Robust.Client.UserInterface;
using Robust.Shared.GameObjects;

namespace Content.Client.Mech.Ui.Equipment
{
	// Token: 0x02000242 RID: 578
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class MechGrabberUi : UIFragment
	{
		// Token: 0x06000EAF RID: 3759 RVA: 0x00058CF7 File Offset: 0x00056EF7
		public override Control GetUIFragmentRoot()
		{
			return this._fragment;
		}

		// Token: 0x06000EB0 RID: 3760 RVA: 0x00058D00 File Offset: 0x00056F00
		public override void Setup(BoundUserInterface userInterface, EntityUid? fragmentOwner)
		{
			if (fragmentOwner == null)
			{
				return;
			}
			this._fragment = new MechGrabberUiFragment();
			this._fragment.OnEjectAction += delegate(EntityUid e)
			{
				userInterface.SendMessage(new MechGrabberEjectMessage(fragmentOwner.Value, e));
			};
		}

		// Token: 0x06000EB1 RID: 3761 RVA: 0x00058D54 File Offset: 0x00056F54
		public override void UpdateState(BoundUserInterfaceState state)
		{
			MechGrabberUiState mechGrabberUiState = state as MechGrabberUiState;
			if (mechGrabberUiState == null)
			{
				return;
			}
			MechGrabberUiFragment fragment = this._fragment;
			if (fragment == null)
			{
				return;
			}
			fragment.UpdateContents(mechGrabberUiState);
		}

		// Token: 0x04000746 RID: 1862
		[Nullable(2)]
		private MechGrabberUiFragment _fragment;
	}
}
