using System;
using System.Runtime.CompilerServices;
using Content.Client.Items;
using Content.Client.Tools.Components;
using Content.Client.Tools.UI;
using Content.Shared.Tools;
using Content.Shared.Tools.Components;
using Robust.Client.GameObjects;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;

namespace Content.Client.Tools
{
	// Token: 0x020000ED RID: 237
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class ToolSystem : SharedToolSystem
	{
		// Token: 0x060006CB RID: 1739 RVA: 0x00023CC0 File Offset: 0x00021EC0
		public override void Initialize()
		{
			base.Initialize();
			base.SubscribeLocalEvent<WelderComponent, ComponentHandleState>(new ComponentEventRefHandler<WelderComponent, ComponentHandleState>(this.OnWelderHandleState), null, null);
			base.SubscribeLocalEvent<WelderComponent, ItemStatusCollectMessage>(new ComponentEventHandler<WelderComponent, ItemStatusCollectMessage>(this.OnWelderGetStatusMessage), null, null);
			base.SubscribeLocalEvent<MultipleToolComponent, ItemStatusCollectMessage>(new ComponentEventHandler<MultipleToolComponent, ItemStatusCollectMessage>(this.OnGetStatusMessage), null, null);
		}

		// Token: 0x060006CC RID: 1740 RVA: 0x00023D10 File Offset: 0x00021F10
		[NullableContext(2)]
		public override void SetMultipleTool(EntityUid uid, MultipleToolComponent multiple = null, ToolComponent tool = null, bool playSound = false, EntityUid? user = null)
		{
			if (!base.Resolve<MultipleToolComponent>(uid, ref multiple, true))
			{
				return;
			}
			base.SetMultipleTool(uid, multiple, tool, playSound, user);
			multiple.UiUpdateNeeded = true;
			SpriteComponent spriteComponent;
			if ((long)multiple.Entries.Length > (long)((ulong)multiple.CurrentEntry) && base.TryComp<SpriteComponent>(uid, ref spriteComponent))
			{
				MultipleToolComponent.ToolEntry toolEntry = multiple.Entries[(int)multiple.CurrentEntry];
				if (toolEntry.Sprite != null)
				{
					spriteComponent.LayerSetSprite(0, toolEntry.Sprite);
				}
			}
		}

		// Token: 0x060006CD RID: 1741 RVA: 0x00023D7E File Offset: 0x00021F7E
		private void OnGetStatusMessage(EntityUid uid, MultipleToolComponent welder, ItemStatusCollectMessage args)
		{
			args.Controls.Add(new MultipleToolStatusControl(welder));
		}

		// Token: 0x060006CE RID: 1742 RVA: 0x00023D91 File Offset: 0x00021F91
		private void OnWelderGetStatusMessage(EntityUid uid, WelderComponent component, ItemStatusCollectMessage args)
		{
			args.Controls.Add(new WelderStatusControl(component));
		}

		// Token: 0x060006CF RID: 1743 RVA: 0x00023DA4 File Offset: 0x00021FA4
		private void OnWelderHandleState(EntityUid uid, WelderComponent welder, ref ComponentHandleState args)
		{
			WelderComponentState welderComponentState = args.Current as WelderComponentState;
			if (welderComponentState == null)
			{
				return;
			}
			welder.FuelCapacity = welderComponentState.FuelCapacity;
			welder.Fuel = welderComponentState.Fuel;
			welder.Lit = welderComponentState.Lit;
			welder.UiUpdateNeeded = true;
		}
	}
}
