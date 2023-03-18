using System;
using System.Runtime.CompilerServices;
using Content.Shared.Input;
using Robust.Shared.Input;

namespace Content.Client.Input
{
	// Token: 0x020002B3 RID: 691
	public static class ContentContexts
	{
		// Token: 0x06001188 RID: 4488 RVA: 0x000682F4 File Offset: 0x000664F4
		[NullableContext(1)]
		public static void SetupContexts(IInputContextContainer contexts)
		{
			IInputCmdContext context = contexts.GetContext("common");
			context.AddFunction(ContentKeyFunctions.FocusChat);
			context.AddFunction(ContentKeyFunctions.FocusLocalChat);
			context.AddFunction(ContentKeyFunctions.FocusWhisperChat);
			context.AddFunction(ContentKeyFunctions.FocusRadio);
			context.AddFunction(ContentKeyFunctions.FocusOOC);
			context.AddFunction(ContentKeyFunctions.FocusAdminChat);
			context.AddFunction(ContentKeyFunctions.FocusConsoleChat);
			context.AddFunction(ContentKeyFunctions.FocusDeadChat);
			context.AddFunction(ContentKeyFunctions.CycleChatChannelForward);
			context.AddFunction(ContentKeyFunctions.CycleChatChannelBackward);
			context.AddFunction(ContentKeyFunctions.ExamineEntity);
			context.AddFunction(ContentKeyFunctions.OpenAHelp);
			context.AddFunction(ContentKeyFunctions.TakeScreenshot);
			context.AddFunction(ContentKeyFunctions.TakeScreenshotNoUI);
			context.AddFunction(ContentKeyFunctions.Point);
			context.AddFunction(ContentKeyFunctions.EditorCopyObject);
			IInputCmdContext context2 = contexts.GetContext("human");
			context2.AddFunction(EngineKeyFunctions.MoveUp);
			context2.AddFunction(EngineKeyFunctions.MoveDown);
			context2.AddFunction(EngineKeyFunctions.MoveLeft);
			context2.AddFunction(EngineKeyFunctions.MoveRight);
			context2.AddFunction(EngineKeyFunctions.Walk);
			context2.AddFunction(ContentKeyFunctions.SwapHands);
			context2.AddFunction(ContentKeyFunctions.Drop);
			context2.AddFunction(ContentKeyFunctions.UseItemInHand);
			context2.AddFunction(ContentKeyFunctions.AltUseItemInHand);
			context2.AddFunction(ContentKeyFunctions.OpenCharacterMenu);
			context2.AddFunction(ContentKeyFunctions.OpenEmotionsMenu);
			context2.AddFunction(ContentKeyFunctions.ActivateItemInWorld);
			context2.AddFunction(ContentKeyFunctions.ThrowItemInHand);
			context2.AddFunction(ContentKeyFunctions.AltActivateItemInWorld);
			context2.AddFunction(ContentKeyFunctions.TryPullObject);
			context2.AddFunction(ContentKeyFunctions.MovePulledObject);
			context2.AddFunction(ContentKeyFunctions.ReleasePulledObject);
			context2.AddFunction(ContentKeyFunctions.OpenCraftingMenu);
			context2.AddFunction(ContentKeyFunctions.OpenInventoryMenu);
			context2.AddFunction(ContentKeyFunctions.SmartEquipBackpack);
			context2.AddFunction(ContentKeyFunctions.SmartEquipBelt);
			context2.AddFunction(ContentKeyFunctions.MouseMiddle);
			context2.AddFunction(ContentKeyFunctions.ArcadeUp);
			context2.AddFunction(ContentKeyFunctions.ArcadeDown);
			context2.AddFunction(ContentKeyFunctions.ArcadeLeft);
			context2.AddFunction(ContentKeyFunctions.ArcadeRight);
			context2.AddFunction(ContentKeyFunctions.Arcade1);
			context2.AddFunction(ContentKeyFunctions.Arcade2);
			context2.AddFunction(ContentKeyFunctions.Arcade3);
			context.AddFunction(ContentKeyFunctions.OpenActionsMenu);
			foreach (BoundKeyFunction boundKeyFunction in ContentKeyFunctions.GetHotbarBoundKeys())
			{
				context.AddFunction(boundKeyFunction);
			}
			foreach (BoundKeyFunction boundKeyFunction2 in ContentKeyFunctions.GetLoadoutBoundKeys())
			{
				context.AddFunction(boundKeyFunction2);
			}
			IInputCmdContext inputCmdContext = contexts.New("aghost", "common");
			inputCmdContext.AddFunction(EngineKeyFunctions.MoveUp);
			inputCmdContext.AddFunction(EngineKeyFunctions.MoveDown);
			inputCmdContext.AddFunction(EngineKeyFunctions.MoveLeft);
			inputCmdContext.AddFunction(EngineKeyFunctions.MoveRight);
			inputCmdContext.AddFunction(EngineKeyFunctions.Walk);
			inputCmdContext.AddFunction(ContentKeyFunctions.SwapHands);
			inputCmdContext.AddFunction(ContentKeyFunctions.Drop);
			inputCmdContext.AddFunction(ContentKeyFunctions.UseItemInHand);
			inputCmdContext.AddFunction(ContentKeyFunctions.AltUseItemInHand);
			inputCmdContext.AddFunction(ContentKeyFunctions.ActivateItemInWorld);
			inputCmdContext.AddFunction(ContentKeyFunctions.ThrowItemInHand);
			inputCmdContext.AddFunction(ContentKeyFunctions.AltActivateItemInWorld);
			inputCmdContext.AddFunction(ContentKeyFunctions.TryPullObject);
			inputCmdContext.AddFunction(ContentKeyFunctions.MovePulledObject);
			inputCmdContext.AddFunction(ContentKeyFunctions.ReleasePulledObject);
			IInputCmdContext inputCmdContext2 = contexts.New("ghost", "human");
			inputCmdContext2.AddFunction(EngineKeyFunctions.MoveUp);
			inputCmdContext2.AddFunction(EngineKeyFunctions.MoveDown);
			inputCmdContext2.AddFunction(EngineKeyFunctions.MoveLeft);
			inputCmdContext2.AddFunction(EngineKeyFunctions.MoveRight);
			inputCmdContext2.AddFunction(EngineKeyFunctions.Walk);
			context.AddFunction(ContentKeyFunctions.OpenEntitySpawnWindow);
			context.AddFunction(ContentKeyFunctions.OpenSandboxWindow);
			context.AddFunction(ContentKeyFunctions.OpenTileSpawnWindow);
			context.AddFunction(ContentKeyFunctions.OpenDecalSpawnWindow);
			context.AddFunction(ContentKeyFunctions.OpenAdminMenu);
			context.AddFunction(ContentKeyFunctions.OpenGuidebook);
		}
	}
}
