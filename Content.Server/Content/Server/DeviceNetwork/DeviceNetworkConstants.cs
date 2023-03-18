using System;
using System.Runtime.CompilerServices;
using Content.Server.DeviceNetwork.Components;
using Robust.Shared.Localization;
using Robust.Shared.Utility;

namespace Content.Server.DeviceNetwork
{
	// Token: 0x0200057E RID: 1406
	[NullableContext(1)]
	[Nullable(0)]
	public static class DeviceNetworkConstants
	{
		// Token: 0x06001D79 RID: 7545 RVA: 0x0009D18C File Offset: 0x0009B38C
		public static string FrequencyToString(this uint frequency)
		{
			string result = frequency.ToString();
			if (result.Length <= 2)
			{
				return result + ".0";
			}
			return result.Insert(result.Length - 1, ".");
		}

		// Token: 0x06001D7A RID: 7546 RVA: 0x0009D1CC File Offset: 0x0009B3CC
		public static string DeviceNetIdToLocalizedName(this int id)
		{
			if (!Enum.IsDefined(typeof(DeviceNetworkComponent.DeviceNetIdDefaults), id))
			{
				return id.ToString();
			}
			DeviceNetworkComponent.DeviceNetIdDefaults deviceNetIdDefaults = (DeviceNetworkComponent.DeviceNetIdDefaults)id;
			string result = deviceNetIdDefaults.ToString();
			string name;
			if (Loc.TryGetString("device-net-id-" + CaseConversion.PascalToKebab(result), ref name))
			{
				return name;
			}
			return result;
		}

		// Token: 0x040012F0 RID: 4848
		public const string Command = "command";

		// Token: 0x040012F1 RID: 4849
		public const string CmdSetState = "set_state";

		// Token: 0x040012F2 RID: 4850
		public const string CmdUpdatedState = "updated_state";

		// Token: 0x040012F3 RID: 4851
		public const string StateEnabled = "state_enabled";
	}
}
