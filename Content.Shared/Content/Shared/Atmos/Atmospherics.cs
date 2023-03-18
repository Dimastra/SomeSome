using System;
using Robust.Shared.Maths;

namespace Content.Shared.Atmos
{
	// Token: 0x0200068F RID: 1679
	public static class Atmospherics
	{
		// Token: 0x0400141D RID: 5149
		public const float R = 8.314463f;

		// Token: 0x0400141E RID: 5150
		public const float OneAtmosphere = 101.325f;

		// Token: 0x0400141F RID: 5151
		public const float GasMinerDefaultMaxExternalPressure = 6500f;

		// Token: 0x04001420 RID: 5152
		public const float TCMB = 2.7f;

		// Token: 0x04001421 RID: 5153
		public const float T0C = 273.15f;

		// Token: 0x04001422 RID: 5154
		public const float T20C = 293.15f;

		// Token: 0x04001423 RID: 5155
		public const float CellVolume = 2500f;

		// Token: 0x04001424 RID: 5156
		public const float BreathVolume = 0.5f;

		// Token: 0x04001425 RID: 5157
		public const float BreathPercentage = 0.0002f;

		// Token: 0x04001426 RID: 5158
		public const float MolesCellStandard = 103.92799f;

		// Token: 0x04001427 RID: 5159
		public const float MolesCellGasMiner = 6666.982f;

		// Token: 0x04001428 RID: 5160
		public const float MCellWithRatio = 0.51963997f;

		// Token: 0x04001429 RID: 5161
		public const float OxygenStandard = 0.21f;

		// Token: 0x0400142A RID: 5162
		public const float NitrogenStandard = 0.79f;

		// Token: 0x0400142B RID: 5163
		public const float OxygenMolesStandard = 21.824879f;

		// Token: 0x0400142C RID: 5164
		public const float NitrogenMolesStandard = 82.10312f;

		// Token: 0x0400142D RID: 5165
		public const float FactorGasVisibleMax = 20f;

		// Token: 0x0400142E RID: 5166
		public const float GasMinMoles = 5E-08f;

		// Token: 0x0400142F RID: 5167
		public const float OpenHeatTransferCoefficient = 0.4f;

		// Token: 0x04001430 RID: 5168
		public const float HeatCapacityVacuum = 7000f;

		// Token: 0x04001431 RID: 5169
		public const float MinimumAirRatioToSuspend = 0.1f;

		// Token: 0x04001432 RID: 5170
		public const float MinimumAirRatioToMove = 0.001f;

		// Token: 0x04001433 RID: 5171
		public const float MinimumAirToSuspend = 10.392799f;

		// Token: 0x04001434 RID: 5172
		public const float MinimumTemperatureToMove = 393.15f;

		// Token: 0x04001435 RID: 5173
		public const float MinimumMolesDeltaToMove = 0.103928f;

		// Token: 0x04001436 RID: 5174
		public const float MinimumTemperatureDeltaToSuspend = 4f;

		// Token: 0x04001437 RID: 5175
		public const float MinimumTemperatureDeltaToConsider = 0.5f;

		// Token: 0x04001438 RID: 5176
		public const float MinimumTemperatureStartSuperConduction = 693.15f;

		// Token: 0x04001439 RID: 5177
		public const float MinimumTemperatureForSuperconduction = 373.15f;

		// Token: 0x0400143A RID: 5178
		public const float MinimumHeatCapacity = 0.0003f;

		// Token: 0x0400143B RID: 5179
		public const float SpaceHeatCapacity = 7000f;

		// Token: 0x0400143C RID: 5180
		public const int ExcitedGroupBreakdownCycles = 4;

		// Token: 0x0400143D RID: 5181
		public const int ExcitedGroupsDismantleCycles = 16;

		// Token: 0x0400143E RID: 5182
		public const int MonstermosHardTileLimit = 2000;

		// Token: 0x0400143F RID: 5183
		public const int MonstermosTileLimit = 200;

		// Token: 0x04001440 RID: 5184
		public const int TotalNumberOfGases = 9;

		// Token: 0x04001441 RID: 5185
		public static readonly int AdjustedNumberOfGases = MathHelper.NextMultipleOf(9, 4);

		// Token: 0x04001442 RID: 5186
		public const float FireHydrogenEnergyReleased = 560000f;

		// Token: 0x04001443 RID: 5187
		public const float FireMinimumTemperatureToExist = 373.15f;

		// Token: 0x04001444 RID: 5188
		public const float FireMinimumTemperatureToSpread = 423.15f;

		// Token: 0x04001445 RID: 5189
		public const float FireSpreadRadiosityScale = 0.85f;

		// Token: 0x04001446 RID: 5190
		public const float FirePlasmaEnergyReleased = 3000000f;

		// Token: 0x04001447 RID: 5191
		public const float FireGrowthRate = 40000f;

		// Token: 0x04001448 RID: 5192
		public const float SuperSaturationThreshold = 96f;

		// Token: 0x04001449 RID: 5193
		public const float OxygenBurnRateBase = 1.4f;

		// Token: 0x0400144A RID: 5194
		public const float PlasmaMinimumBurnTemperature = 373.15f;

		// Token: 0x0400144B RID: 5195
		public const float PlasmaUpperTemperature = 1643.15f;

		// Token: 0x0400144C RID: 5196
		public const float PlasmaOxygenFullburn = 10f;

		// Token: 0x0400144D RID: 5197
		public const float PlasmaBurnRateDelta = 9f;

		// Token: 0x0400144E RID: 5198
		public const float MinimumTritiumOxyburnEnergy = 2000000f;

		// Token: 0x0400144F RID: 5199
		public const float TritiumBurnOxyFactor = 100f;

		// Token: 0x04001450 RID: 5200
		public const float TritiumBurnTritFactor = 10f;

		// Token: 0x04001451 RID: 5201
		public const float FrezonCoolLowerTemperature = 23.15f;

		// Token: 0x04001452 RID: 5202
		public const float FrezonCoolMidTemperature = 373.15f;

		// Token: 0x04001453 RID: 5203
		public const float FrezonCoolMaximumEnergyModifier = 10f;

		// Token: 0x04001454 RID: 5204
		public const float FrezonNitrogenCoolRatio = 5f;

		// Token: 0x04001455 RID: 5205
		public const float FrezonCoolEnergyReleased = -3000000f;

		// Token: 0x04001456 RID: 5206
		public const float FrezonCoolRateModifier = 20f;

		// Token: 0x04001457 RID: 5207
		public const float FrezonProductionMaxEfficiencyTemperature = 73.15f;

		// Token: 0x04001458 RID: 5208
		public const float FrezonProductionNitrogenRatio = 10f;

		// Token: 0x04001459 RID: 5209
		public const float FrezonProductionConversionRate = 50f;

		// Token: 0x0400145A RID: 5210
		public const float MiasmicSubsumationMaxConversionRate = 5f;

		// Token: 0x0400145B RID: 5211
		public const float HazardHighPressure = 550f;

		// Token: 0x0400145C RID: 5212
		public const float WarningHighPressure = 385f;

		// Token: 0x0400145D RID: 5213
		public const float WarningLowPressure = 50f;

		// Token: 0x0400145E RID: 5214
		public const float HazardLowPressure = 20f;

		// Token: 0x0400145F RID: 5215
		public const float PressureDamageCoefficient = 4f;

		// Token: 0x04001460 RID: 5216
		public const int MaxHighPressureDamage = 4;

		// Token: 0x04001461 RID: 5217
		public const int LowPressureDamage = 4;

		// Token: 0x04001462 RID: 5218
		public const float WindowHeatTransferCoefficient = 0.1f;

		// Token: 0x04001463 RID: 5219
		public const int Directions = 4;

		// Token: 0x04001464 RID: 5220
		public const float NormalBodyTemperature = 37f;

		// Token: 0x04001465 RID: 5221
		public const float BreathMolesToReagentMultiplier = 1144f;

		// Token: 0x04001466 RID: 5222
		public const float MaxOutputPressure = 4500f;

		// Token: 0x04001467 RID: 5223
		public const float MaxTransferRate = 200f;
	}
}
