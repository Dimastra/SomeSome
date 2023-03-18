using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using Content.Shared.Tools;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom.Prototype;

namespace Content.Shared.Configurable
{
	// Token: 0x02000590 RID: 1424
	[NullableContext(1)]
	[Nullable(0)]
	[RegisterComponent]
	[NetworkedComponent]
	public sealed class ConfigurationComponent : Component
	{
		// Token: 0x0400101B RID: 4123
		[DataField("config", false, 1, false, false, null)]
		public readonly Dictionary<string, string> Config = new Dictionary<string, string>();

		// Token: 0x0400101C RID: 4124
		[DataField("qualityNeeded", false, 1, false, false, typeof(PrototypeIdSerializer<ToolQualityPrototype>))]
		public string QualityNeeded = "Pulsing";

		// Token: 0x0400101D RID: 4125
		[DataField("validation", false, 1, false, false, null)]
		public readonly Regex Validation = new Regex("^[a-zA-Z0-9 ]*$", RegexOptions.Compiled);

		// Token: 0x02000849 RID: 2121
		[Nullable(0)]
		[NetSerializable]
		[Serializable]
		public sealed class ConfigurationBoundUserInterfaceState : BoundUserInterfaceState
		{
			// Token: 0x17000522 RID: 1314
			// (get) Token: 0x06001945 RID: 6469 RVA: 0x0004FD23 File Offset: 0x0004DF23
			public Dictionary<string, string> Config { get; }

			// Token: 0x06001946 RID: 6470 RVA: 0x0004FD2B File Offset: 0x0004DF2B
			public ConfigurationBoundUserInterfaceState(Dictionary<string, string> config)
			{
				this.Config = config;
			}
		}

		// Token: 0x0200084A RID: 2122
		[Nullable(0)]
		[NetSerializable]
		[Serializable]
		public sealed class ConfigurationUpdatedMessage : BoundUserInterfaceMessage
		{
			// Token: 0x17000523 RID: 1315
			// (get) Token: 0x06001947 RID: 6471 RVA: 0x0004FD3A File Offset: 0x0004DF3A
			public Dictionary<string, string> Config { get; }

			// Token: 0x06001948 RID: 6472 RVA: 0x0004FD42 File Offset: 0x0004DF42
			public ConfigurationUpdatedMessage(Dictionary<string, string> config)
			{
				this.Config = config;
			}
		}

		// Token: 0x0200084B RID: 2123
		[Nullable(0)]
		[NetSerializable]
		[Serializable]
		public sealed class ValidationUpdateMessage : BoundUserInterfaceMessage
		{
			// Token: 0x17000524 RID: 1316
			// (get) Token: 0x06001949 RID: 6473 RVA: 0x0004FD51 File Offset: 0x0004DF51
			public string ValidationString { get; }

			// Token: 0x0600194A RID: 6474 RVA: 0x0004FD59 File Offset: 0x0004DF59
			public ValidationUpdateMessage(string validationString)
			{
				this.ValidationString = validationString;
			}
		}

		// Token: 0x0200084C RID: 2124
		[NullableContext(0)]
		[NetSerializable]
		[Serializable]
		public enum ConfigurationUiKey
		{
			// Token: 0x04001963 RID: 6499
			Key
		}
	}
}
