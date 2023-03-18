using System;
using System.Runtime.CompilerServices;
using Content.Shared.Damage;
using Robust.Shared.Audio;
using Robust.Shared.Log;
using Robust.Shared.Maths;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Utility;

namespace Content.Shared.Explosion
{
	// Token: 0x020004A3 RID: 1187
	[NullableContext(1)]
	[Nullable(0)]
	[Prototype("explosion", 1)]
	public sealed class ExplosionPrototype : IPrototype
	{
		// Token: 0x17000300 RID: 768
		// (get) Token: 0x06000E60 RID: 3680 RVA: 0x0002E200 File Offset: 0x0002C400
		[IdDataField(1, null)]
		public string ID { get; }

		// Token: 0x06000E61 RID: 3681 RVA: 0x0002E208 File Offset: 0x0002C408
		public float TileBreakChance(float intensity)
		{
			if (this._tileBreakChance.Length == 0 || this._tileBreakChance.Length != this._tileBreakIntensity.Length)
			{
				Logger.Error("Malformed tile break chance definitions for explosion prototype: " + this.ID);
				return 0f;
			}
			float intensity2 = intensity;
			float[] tileBreakIntensity = this._tileBreakIntensity;
			if (intensity2 >= tileBreakIntensity[tileBreakIntensity.Length - 1] || this._tileBreakIntensity.Length == 1)
			{
				float[] tileBreakChance = this._tileBreakChance;
				return tileBreakChance[tileBreakChance.Length - 1];
			}
			if (intensity <= this._tileBreakIntensity[0])
			{
				return this._tileBreakChance[0];
			}
			int i = Array.FindIndex<float>(this._tileBreakIntensity, (float k) => k >= intensity);
			float slope = (this._tileBreakChance[i] - this._tileBreakChance[i - 1]) / (this._tileBreakIntensity[i] - this._tileBreakIntensity[i - 1]);
			return this._tileBreakChance[i - 1] + slope * (intensity - this._tileBreakIntensity[i - 1]);
		}

		// Token: 0x04000D7F RID: 3455
		[DataField("damagePerIntensity", false, 1, true, false, null)]
		public readonly DamageSpecifier DamagePerIntensity;

		// Token: 0x04000D80 RID: 3456
		[DataField("tileBreakChance", false, 1, false, false, null)]
		private readonly float[] _tileBreakChance = new float[]
		{
			0f,
			1f
		};

		// Token: 0x04000D81 RID: 3457
		[DataField("tileBreakIntensity", false, 1, false, false, null)]
		private readonly float[] _tileBreakIntensity = new float[]
		{
			0f,
			15f
		};

		// Token: 0x04000D82 RID: 3458
		[DataField("tileBreakRerollReduction", false, 1, false, false, null)]
		public readonly float TileBreakRerollReduction = 10f;

		// Token: 0x04000D83 RID: 3459
		[DataField("lightColor", false, 1, false, false, null)]
		public readonly Color LightColor = Color.Orange;

		// Token: 0x04000D84 RID: 3460
		[DataField("fireColor", false, 1, false, false, null)]
		public readonly Color? FireColor;

		// Token: 0x04000D85 RID: 3461
		[DataField("Sound", false, 1, false, false, null)]
		public readonly SoundSpecifier Sound = new SoundCollectionSpecifier("explosion", null);

		// Token: 0x04000D86 RID: 3462
		[DataField("texturePath", false, 1, false, false, null)]
		public readonly ResourcePath TexturePath = new ResourcePath("/Textures/Effects/fire.rsi", "/");

		// Token: 0x04000D87 RID: 3463
		[DataField("intensityPerState", false, 1, false, false, null)]
		public float IntensityPerState = 12f;

		// Token: 0x04000D88 RID: 3464
		[DataField("fireStates", false, 1, false, false, null)]
		public readonly int FireStates = 3;
	}
}
