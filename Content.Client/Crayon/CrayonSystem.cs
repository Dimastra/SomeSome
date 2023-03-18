using System;
using System.Runtime.CompilerServices;
using Content.Client.Items;
using Content.Client.Message;
using Content.Shared.Crayon;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.Controls;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Localization;
using Robust.Shared.Timing;

namespace Content.Client.Crayon
{
	// Token: 0x02000376 RID: 886
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class CrayonSystem : SharedCrayonSystem
	{
		// Token: 0x060015CB RID: 5579 RVA: 0x00080FA8 File Offset: 0x0007F1A8
		public override void Initialize()
		{
			base.Initialize();
			ComponentEventRefHandler<CrayonComponent, ComponentHandleState> componentEventRefHandler;
			if ((componentEventRefHandler = CrayonSystem.<>O.<0>__OnCrayonHandleState) == null)
			{
				componentEventRefHandler = (CrayonSystem.<>O.<0>__OnCrayonHandleState = new ComponentEventRefHandler<CrayonComponent, ComponentHandleState>(CrayonSystem.OnCrayonHandleState));
			}
			base.SubscribeLocalEvent<CrayonComponent, ComponentHandleState>(componentEventRefHandler, null, null);
			ComponentEventHandler<CrayonComponent, ItemStatusCollectMessage> componentEventHandler;
			if ((componentEventHandler = CrayonSystem.<>O.<1>__OnCrayonItemStatus) == null)
			{
				componentEventHandler = (CrayonSystem.<>O.<1>__OnCrayonItemStatus = new ComponentEventHandler<CrayonComponent, ItemStatusCollectMessage>(CrayonSystem.OnCrayonItemStatus));
			}
			base.SubscribeLocalEvent<CrayonComponent, ItemStatusCollectMessage>(componentEventHandler, null, null);
		}

		// Token: 0x060015CC RID: 5580 RVA: 0x00081004 File Offset: 0x0007F204
		private static void OnCrayonHandleState(EntityUid uid, CrayonComponent component, ref ComponentHandleState args)
		{
			CrayonComponentState crayonComponentState = args.Current as CrayonComponentState;
			if (crayonComponentState == null)
			{
				return;
			}
			component.Color = crayonComponentState.Color;
			component.SelectedState = crayonComponentState.State;
			component.Charges = crayonComponentState.Charges;
			component.Capacity = crayonComponentState.Capacity;
			component.UIUpdateNeeded = true;
		}

		// Token: 0x060015CD RID: 5581 RVA: 0x00081058 File Offset: 0x0007F258
		private static void OnCrayonItemStatus(EntityUid uid, CrayonComponent component, ItemStatusCollectMessage args)
		{
			args.Controls.Add(new CrayonSystem.StatusControl(component));
		}

		// Token: 0x02000377 RID: 887
		[Nullable(0)]
		private sealed class StatusControl : Control
		{
			// Token: 0x060015CF RID: 5583 RVA: 0x00081073 File Offset: 0x0007F273
			public StatusControl(CrayonComponent parent)
			{
				this._parent = parent;
				this._label = new RichTextLabel
				{
					StyleClasses = 
					{
						"ItemStatus"
					}
				};
				base.AddChild(this._label);
				parent.UIUpdateNeeded = true;
			}

			// Token: 0x060015D0 RID: 5584 RVA: 0x000810B0 File Offset: 0x0007F2B0
			protected override void FrameUpdate(FrameEventArgs args)
			{
				base.FrameUpdate(args);
				if (!this._parent.UIUpdateNeeded)
				{
					return;
				}
				this._parent.UIUpdateNeeded = false;
				this._label.SetMarkup(Loc.GetString("crayon-drawing-label", new ValueTuple<string, object>[]
				{
					new ValueTuple<string, object>("color", this._parent.Color),
					new ValueTuple<string, object>("state", this._parent.SelectedState),
					new ValueTuple<string, object>("charges", this._parent.Charges),
					new ValueTuple<string, object>("capacity", this._parent.Capacity)
				}));
			}

			// Token: 0x04000B66 RID: 2918
			private readonly CrayonComponent _parent;

			// Token: 0x04000B67 RID: 2919
			private readonly RichTextLabel _label;
		}

		// Token: 0x02000378 RID: 888
		[CompilerGenerated]
		private static class <>O
		{
			// Token: 0x04000B68 RID: 2920
			[Nullable(new byte[]
			{
				0,
				1
			})]
			public static ComponentEventRefHandler<CrayonComponent, ComponentHandleState> <0>__OnCrayonHandleState;

			// Token: 0x04000B69 RID: 2921
			[Nullable(new byte[]
			{
				0,
				1,
				1
			})]
			public static ComponentEventHandler<CrayonComponent, ItemStatusCollectMessage> <1>__OnCrayonItemStatus;
		}
	}
}
