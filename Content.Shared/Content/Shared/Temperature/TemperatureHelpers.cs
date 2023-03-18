using System;

namespace Content.Shared.Temperature
{
	// Token: 0x020000DE RID: 222
	public static class TemperatureHelpers
	{
		// Token: 0x0600026B RID: 619 RVA: 0x0000BAF8 File Offset: 0x00009CF8
		public static float CelsiusToKelvin(float celsius)
		{
			return celsius + 273.15f;
		}

		// Token: 0x0600026C RID: 620 RVA: 0x0000BB01 File Offset: 0x00009D01
		public static float CelsiusToFahrenheit(float celsius)
		{
			return celsius * 9f / 5f + 32f;
		}

		// Token: 0x0600026D RID: 621 RVA: 0x0000BB16 File Offset: 0x00009D16
		public static float KelvinToCelsius(float kelvin)
		{
			return kelvin - 273.15f;
		}

		// Token: 0x0600026E RID: 622 RVA: 0x0000BB1F File Offset: 0x00009D1F
		public static float KelvinToFahrenheit(float kelvin)
		{
			return TemperatureHelpers.CelsiusToFahrenheit(TemperatureHelpers.KelvinToCelsius(kelvin));
		}

		// Token: 0x0600026F RID: 623 RVA: 0x0000BB2C File Offset: 0x00009D2C
		public static float FahrenheitToCelsius(float fahrenheit)
		{
			return (fahrenheit - 32f) * 5f / 9f;
		}

		// Token: 0x06000270 RID: 624 RVA: 0x0000BB41 File Offset: 0x00009D41
		public static float FahrenheitToKelvin(float fahrenheit)
		{
			return TemperatureHelpers.CelsiusToKelvin(TemperatureHelpers.FahrenheitToCelsius(fahrenheit));
		}
	}
}
