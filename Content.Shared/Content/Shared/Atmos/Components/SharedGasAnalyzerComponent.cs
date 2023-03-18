using System;
using System.Runtime.CompilerServices;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Localization;
using Robust.Shared.Serialization;

namespace Content.Shared.Atmos.Components
{
	// Token: 0x020006E4 RID: 1764
	[NetworkedComponent]
	public abstract class SharedGasAnalyzerComponent : Component
	{
		// Token: 0x0200086A RID: 2154
		[NetSerializable]
		[Serializable]
		public enum GasAnalyzerUiKey
		{
			// Token: 0x040019F7 RID: 6647
			Key
		}

		// Token: 0x0200086B RID: 2155
		[NullableContext(1)]
		[Nullable(0)]
		[NetSerializable]
		[Serializable]
		public sealed class GasAnalyzerUserMessage : BoundUserInterfaceMessage
		{
			// Token: 0x060019D9 RID: 6617 RVA: 0x000518B2 File Offset: 0x0004FAB2
			public GasAnalyzerUserMessage(SharedGasAnalyzerComponent.GasMixEntry[] nodeGasMixes, string deviceName, EntityUid deviceUid, bool deviceFlipped, [Nullable(2)] string error = null)
			{
				this.NodeGasMixes = nodeGasMixes;
				this.DeviceName = deviceName;
				this.DeviceUid = deviceUid;
				this.DeviceFlipped = deviceFlipped;
				this.Error = error;
			}

			// Token: 0x040019F8 RID: 6648
			public string DeviceName;

			// Token: 0x040019F9 RID: 6649
			public EntityUid DeviceUid;

			// Token: 0x040019FA RID: 6650
			public bool DeviceFlipped;

			// Token: 0x040019FB RID: 6651
			[Nullable(2)]
			public string Error;

			// Token: 0x040019FC RID: 6652
			public SharedGasAnalyzerComponent.GasMixEntry[] NodeGasMixes;
		}

		// Token: 0x0200086C RID: 2156
		[NullableContext(1)]
		[Nullable(0)]
		[NetSerializable]
		[Serializable]
		public struct GasMixEntry
		{
			// Token: 0x060019DA RID: 6618 RVA: 0x000518DF File Offset: 0x0004FADF
			public GasMixEntry(string name, float pressure, float temperature, [Nullable(2)] SharedGasAnalyzerComponent.GasEntry[] gases = null)
			{
				this.Name = name;
				this.Pressure = pressure;
				this.Temperature = temperature;
				this.Gases = gases;
			}

			// Token: 0x040019FD RID: 6653
			public readonly string Name;

			// Token: 0x040019FE RID: 6654
			public readonly float Pressure;

			// Token: 0x040019FF RID: 6655
			public readonly float Temperature;

			// Token: 0x04001A00 RID: 6656
			[Nullable(2)]
			public readonly SharedGasAnalyzerComponent.GasEntry[] Gases;
		}

		// Token: 0x0200086D RID: 2157
		[NullableContext(1)]
		[Nullable(0)]
		[NetSerializable]
		[Serializable]
		public struct GasEntry
		{
			// Token: 0x060019DB RID: 6619 RVA: 0x000518FE File Offset: 0x0004FAFE
			public GasEntry(string name, float amount, string color)
			{
				this.Name = name;
				this.Amount = amount;
				this.Color = color;
			}

			// Token: 0x060019DC RID: 6620 RVA: 0x00051918 File Offset: 0x0004FB18
			public override string ToString()
			{
				return Loc.GetString("gas-entry-info", new ValueTuple<string, object>[]
				{
					new ValueTuple<string, object>("gasName", this.Name),
					new ValueTuple<string, object>("gasAmount", this.Amount)
				});
			}

			// Token: 0x04001A01 RID: 6657
			public readonly string Name;

			// Token: 0x04001A02 RID: 6658
			public readonly float Amount;

			// Token: 0x04001A03 RID: 6659
			public readonly string Color;
		}

		// Token: 0x0200086E RID: 2158
		[NetSerializable]
		[Serializable]
		public sealed class GasAnalyzerDisableMessage : BoundUserInterfaceMessage
		{
		}
	}
}
