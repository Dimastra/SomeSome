using System;
using System.Runtime.CompilerServices;
using Robust.Shared.Input;

namespace Content.Shared.Input
{
	// Token: 0x020003E9 RID: 1001
	[NullableContext(1)]
	[Nullable(0)]
	[KeyFunctions]
	public static class ContentKeyFunctions
	{
		// Token: 0x06000BC7 RID: 3015 RVA: 0x00026688 File Offset: 0x00024888
		public static BoundKeyFunction[] GetHotbarBoundKeys()
		{
			return new BoundKeyFunction[]
			{
				ContentKeyFunctions.Hotbar1,
				ContentKeyFunctions.Hotbar2,
				ContentKeyFunctions.Hotbar3,
				ContentKeyFunctions.Hotbar4,
				ContentKeyFunctions.Hotbar5,
				ContentKeyFunctions.Hotbar6,
				ContentKeyFunctions.Hotbar7,
				ContentKeyFunctions.Hotbar8,
				ContentKeyFunctions.Hotbar9,
				ContentKeyFunctions.Hotbar0
			};
		}

		// Token: 0x06000BC8 RID: 3016 RVA: 0x00026718 File Offset: 0x00024918
		public static BoundKeyFunction[] GetLoadoutBoundKeys()
		{
			return new BoundKeyFunction[]
			{
				ContentKeyFunctions.Loadout1,
				ContentKeyFunctions.Loadout2,
				ContentKeyFunctions.Loadout3,
				ContentKeyFunctions.Loadout4,
				ContentKeyFunctions.Loadout5,
				ContentKeyFunctions.Loadout6,
				ContentKeyFunctions.Loadout7,
				ContentKeyFunctions.Loadout8,
				ContentKeyFunctions.Loadout9,
				ContentKeyFunctions.Loadout0
			};
		}

		// Token: 0x04000B67 RID: 2919
		public static readonly BoundKeyFunction UseItemInHand = "ActivateItemInHand";

		// Token: 0x04000B68 RID: 2920
		public static readonly BoundKeyFunction AltUseItemInHand = "AltActivateItemInHand";

		// Token: 0x04000B69 RID: 2921
		public static readonly BoundKeyFunction ActivateItemInWorld = "ActivateItemInWorld";

		// Token: 0x04000B6A RID: 2922
		public static readonly BoundKeyFunction AltActivateItemInWorld = "AltActivateItemInWorld";

		// Token: 0x04000B6B RID: 2923
		public static readonly BoundKeyFunction Drop = "Drop";

		// Token: 0x04000B6C RID: 2924
		public static readonly BoundKeyFunction ExamineEntity = "ExamineEntity";

		// Token: 0x04000B6D RID: 2925
		public static readonly BoundKeyFunction FocusChat = "FocusChatInputWindow";

		// Token: 0x04000B6E RID: 2926
		public static readonly BoundKeyFunction FocusLocalChat = "FocusLocalChatWindow";

		// Token: 0x04000B6F RID: 2927
		public static readonly BoundKeyFunction FocusWhisperChat = "FocusWhisperChatWindow";

		// Token: 0x04000B70 RID: 2928
		public static readonly BoundKeyFunction FocusRadio = "FocusRadioWindow";

		// Token: 0x04000B71 RID: 2929
		public static readonly BoundKeyFunction FocusOOC = "FocusOOCWindow";

		// Token: 0x04000B72 RID: 2930
		public static readonly BoundKeyFunction FocusAdminChat = "FocusAdminChatWindow";

		// Token: 0x04000B73 RID: 2931
		public static readonly BoundKeyFunction FocusDeadChat = "FocusDeadChatWindow";

		// Token: 0x04000B74 RID: 2932
		public static readonly BoundKeyFunction FocusConsoleChat = "FocusConsoleChatWindow";

		// Token: 0x04000B75 RID: 2933
		public static readonly BoundKeyFunction CycleChatChannelForward = "CycleChatChannelForward";

		// Token: 0x04000B76 RID: 2934
		public static readonly BoundKeyFunction CycleChatChannelBackward = "CycleChatChannelBackward";

		// Token: 0x04000B77 RID: 2935
		public static readonly BoundKeyFunction OpenCharacterMenu = "OpenCharacterMenu";

		// Token: 0x04000B78 RID: 2936
		public static readonly BoundKeyFunction OpenEmotionsMenu = "OpenEmotionsMenu";

		// Token: 0x04000B79 RID: 2937
		public static readonly BoundKeyFunction OpenCraftingMenu = "OpenCraftingMenu";

		// Token: 0x04000B7A RID: 2938
		public static readonly BoundKeyFunction OpenGuidebook = "OpenGuidebook";

		// Token: 0x04000B7B RID: 2939
		public static readonly BoundKeyFunction OpenInventoryMenu = "OpenInventoryMenu";

		// Token: 0x04000B7C RID: 2940
		public static readonly BoundKeyFunction SmartEquipBackpack = "SmartEquipBackpack";

		// Token: 0x04000B7D RID: 2941
		public static readonly BoundKeyFunction SmartEquipBelt = "SmartEquipBelt";

		// Token: 0x04000B7E RID: 2942
		public static readonly BoundKeyFunction OpenAHelp = "OpenAHelp";

		// Token: 0x04000B7F RID: 2943
		public static readonly BoundKeyFunction SwapHands = "SwapHands";

		// Token: 0x04000B80 RID: 2944
		public static readonly BoundKeyFunction ThrowItemInHand = "ThrowItemInHand";

		// Token: 0x04000B81 RID: 2945
		public static readonly BoundKeyFunction TryPullObject = "TryPullObject";

		// Token: 0x04000B82 RID: 2946
		public static readonly BoundKeyFunction MovePulledObject = "MovePulledObject";

		// Token: 0x04000B83 RID: 2947
		public static readonly BoundKeyFunction ReleasePulledObject = "ReleasePulledObject";

		// Token: 0x04000B84 RID: 2948
		public static readonly BoundKeyFunction MouseMiddle = "MouseMiddle";

		// Token: 0x04000B85 RID: 2949
		public static readonly BoundKeyFunction OpenEntitySpawnWindow = "OpenEntitySpawnWindow";

		// Token: 0x04000B86 RID: 2950
		public static readonly BoundKeyFunction OpenSandboxWindow = "OpenSandboxWindow";

		// Token: 0x04000B87 RID: 2951
		public static readonly BoundKeyFunction OpenTileSpawnWindow = "OpenTileSpawnWindow";

		// Token: 0x04000B88 RID: 2952
		public static readonly BoundKeyFunction OpenDecalSpawnWindow = "OpenDecalSpawnWindow";

		// Token: 0x04000B89 RID: 2953
		public static readonly BoundKeyFunction OpenAdminMenu = "OpenAdminMenu";

		// Token: 0x04000B8A RID: 2954
		public static readonly BoundKeyFunction TakeScreenshot = "TakeScreenshot";

		// Token: 0x04000B8B RID: 2955
		public static readonly BoundKeyFunction TakeScreenshotNoUI = "TakeScreenshotNoUI";

		// Token: 0x04000B8C RID: 2956
		public static readonly BoundKeyFunction Point = "Point";

		// Token: 0x04000B8D RID: 2957
		public static readonly BoundKeyFunction ArcadeUp = "ArcadeUp";

		// Token: 0x04000B8E RID: 2958
		public static readonly BoundKeyFunction ArcadeDown = "ArcadeDown";

		// Token: 0x04000B8F RID: 2959
		public static readonly BoundKeyFunction ArcadeLeft = "ArcadeLeft";

		// Token: 0x04000B90 RID: 2960
		public static readonly BoundKeyFunction ArcadeRight = "ArcadeRight";

		// Token: 0x04000B91 RID: 2961
		public static readonly BoundKeyFunction Arcade1 = "Arcade1";

		// Token: 0x04000B92 RID: 2962
		public static readonly BoundKeyFunction Arcade2 = "Arcade2";

		// Token: 0x04000B93 RID: 2963
		public static readonly BoundKeyFunction Arcade3 = "Arcade3";

		// Token: 0x04000B94 RID: 2964
		public static readonly BoundKeyFunction OpenActionsMenu = "OpenAbilitiesMenu";

		// Token: 0x04000B95 RID: 2965
		public static readonly BoundKeyFunction ShuttleStrafeLeft = "ShuttleStrafeLeft";

		// Token: 0x04000B96 RID: 2966
		public static readonly BoundKeyFunction ShuttleStrafeUp = "ShuttleStrafeUp";

		// Token: 0x04000B97 RID: 2967
		public static readonly BoundKeyFunction ShuttleStrafeRight = "ShuttleStrafeRight";

		// Token: 0x04000B98 RID: 2968
		public static readonly BoundKeyFunction ShuttleStrafeDown = "ShuttleStrafeDown";

		// Token: 0x04000B99 RID: 2969
		public static readonly BoundKeyFunction ShuttleRotateLeft = "ShuttleRotateLeft";

		// Token: 0x04000B9A RID: 2970
		public static readonly BoundKeyFunction ShuttleRotateRight = "ShuttleRotateRight";

		// Token: 0x04000B9B RID: 2971
		public static readonly BoundKeyFunction ShuttleBrake = "ShuttleBrake";

		// Token: 0x04000B9C RID: 2972
		public static readonly BoundKeyFunction Hotbar0 = "Hotbar0";

		// Token: 0x04000B9D RID: 2973
		public static readonly BoundKeyFunction Hotbar1 = "Hotbar1";

		// Token: 0x04000B9E RID: 2974
		public static readonly BoundKeyFunction Hotbar2 = "Hotbar2";

		// Token: 0x04000B9F RID: 2975
		public static readonly BoundKeyFunction Hotbar3 = "Hotbar3";

		// Token: 0x04000BA0 RID: 2976
		public static readonly BoundKeyFunction Hotbar4 = "Hotbar4";

		// Token: 0x04000BA1 RID: 2977
		public static readonly BoundKeyFunction Hotbar5 = "Hotbar5";

		// Token: 0x04000BA2 RID: 2978
		public static readonly BoundKeyFunction Hotbar6 = "Hotbar6";

		// Token: 0x04000BA3 RID: 2979
		public static readonly BoundKeyFunction Hotbar7 = "Hotbar7";

		// Token: 0x04000BA4 RID: 2980
		public static readonly BoundKeyFunction Hotbar8 = "Hotbar8";

		// Token: 0x04000BA5 RID: 2981
		public static readonly BoundKeyFunction Hotbar9 = "Hotbar9";

		// Token: 0x04000BA6 RID: 2982
		public static readonly BoundKeyFunction Loadout0 = "Loadout0";

		// Token: 0x04000BA7 RID: 2983
		public static readonly BoundKeyFunction Loadout1 = "Loadout1";

		// Token: 0x04000BA8 RID: 2984
		public static readonly BoundKeyFunction Loadout2 = "Loadout2";

		// Token: 0x04000BA9 RID: 2985
		public static readonly BoundKeyFunction Loadout3 = "Loadout3";

		// Token: 0x04000BAA RID: 2986
		public static readonly BoundKeyFunction Loadout4 = "Loadout4";

		// Token: 0x04000BAB RID: 2987
		public static readonly BoundKeyFunction Loadout5 = "Loadout5";

		// Token: 0x04000BAC RID: 2988
		public static readonly BoundKeyFunction Loadout6 = "Loadout6";

		// Token: 0x04000BAD RID: 2989
		public static readonly BoundKeyFunction Loadout7 = "Loadout7";

		// Token: 0x04000BAE RID: 2990
		public static readonly BoundKeyFunction Loadout8 = "Loadout8";

		// Token: 0x04000BAF RID: 2991
		public static readonly BoundKeyFunction Loadout9 = "Loadout9";

		// Token: 0x04000BB0 RID: 2992
		public static readonly BoundKeyFunction Vote0 = "Vote0";

		// Token: 0x04000BB1 RID: 2993
		public static readonly BoundKeyFunction Vote1 = "Vote1";

		// Token: 0x04000BB2 RID: 2994
		public static readonly BoundKeyFunction Vote2 = "Vote2";

		// Token: 0x04000BB3 RID: 2995
		public static readonly BoundKeyFunction Vote3 = "Vote3";

		// Token: 0x04000BB4 RID: 2996
		public static readonly BoundKeyFunction Vote4 = "Vote4";

		// Token: 0x04000BB5 RID: 2997
		public static readonly BoundKeyFunction Vote5 = "Vote5";

		// Token: 0x04000BB6 RID: 2998
		public static readonly BoundKeyFunction Vote6 = "Vote6";

		// Token: 0x04000BB7 RID: 2999
		public static readonly BoundKeyFunction Vote7 = "Vote7";

		// Token: 0x04000BB8 RID: 3000
		public static readonly BoundKeyFunction Vote8 = "Vote8";

		// Token: 0x04000BB9 RID: 3001
		public static readonly BoundKeyFunction Vote9 = "Vote9";

		// Token: 0x04000BBA RID: 3002
		public static readonly BoundKeyFunction EditorCopyObject = "EditorCopyObject";
	}
}
