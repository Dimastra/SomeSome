using System;
using System.Runtime.CompilerServices;
using Content.Shared.AirlockPainter;
using Robust.Client.GameObjects;
using Robust.Client.UserInterface.Controls;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;

namespace Content.Client.AirlockPainter.UI
{
	// Token: 0x02000482 RID: 1154
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class AirlockPainterBoundUserInterface : BoundUserInterface
	{
		// Token: 0x06001C71 RID: 7281 RVA: 0x000021BC File Offset: 0x000003BC
		public AirlockPainterBoundUserInterface(ClientUserInterfaceComponent owner, Enum uiKey) : base(owner, uiKey)
		{
		}

		// Token: 0x06001C72 RID: 7282 RVA: 0x000A4E78 File Offset: 0x000A3078
		protected override void Open()
		{
			base.Open();
			this._window = new AirlockPainterWindow();
			this._painter = this._entitySystems.GetEntitySystem<AirlockPainterSystem>();
			this._window.OpenCentered();
			this._window.OnClose += base.Close;
			this._window.OnSpritePicked = new Action<ItemList.ItemListSelectedEventArgs>(this.OnSpritePicked);
		}

		// Token: 0x06001C73 RID: 7283 RVA: 0x000A4EE0 File Offset: 0x000A30E0
		protected override void UpdateState(BoundUserInterfaceState state)
		{
			base.UpdateState(state);
			if (this._window == null)
			{
				return;
			}
			if (this._painter == null)
			{
				return;
			}
			AirlockPainterBoundUserInterfaceState airlockPainterBoundUserInterfaceState = state as AirlockPainterBoundUserInterfaceState;
			if (airlockPainterBoundUserInterfaceState == null)
			{
				return;
			}
			this._window.Populate(this._painter.Entries, airlockPainterBoundUserInterfaceState.SelectedStyle);
		}

		// Token: 0x06001C74 RID: 7284 RVA: 0x000A4F2D File Offset: 0x000A312D
		private void OnSpritePicked(ItemList.ItemListSelectedEventArgs args)
		{
			base.SendMessage(new AirlockPainterSpritePickedMessage(args.ItemIndex));
		}

		// Token: 0x04000E3B RID: 3643
		[Nullable(2)]
		private AirlockPainterWindow _window;

		// Token: 0x04000E3C RID: 3644
		[Nullable(2)]
		private AirlockPainterSystem _painter;

		// Token: 0x04000E3D RID: 3645
		[Dependency]
		private readonly IEntitySystemManager _entitySystems;
	}
}
