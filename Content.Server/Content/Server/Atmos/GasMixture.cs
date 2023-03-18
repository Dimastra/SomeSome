using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Content.Server.Atmos.Reactions;
using Content.Shared.Atmos;
using Robust.Shared.Maths;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.ViewVariables;

namespace Content.Server.Atmos
{
	// Token: 0x02000732 RID: 1842
	[NullableContext(1)]
	[Nullable(0)]
	[DataDefinition]
	[Serializable]
	public sealed class GasMixture : IEquatable<GasMixture>, ISerializationHooks
	{
		// Token: 0x170005B9 RID: 1465
		// (get) Token: 0x060026A1 RID: 9889 RVA: 0x000CC255 File Offset: 0x000CA455
		public static GasMixture SpaceGas
		{
			get
			{
				return new GasMixture
				{
					Volume = 2500f,
					Temperature = 2.7f,
					Immutable = true
				};
			}
		}

		// Token: 0x170005BA RID: 1466
		// (get) Token: 0x060026A2 RID: 9890 RVA: 0x000CC279 File Offset: 0x000CA479
		// (set) Token: 0x060026A3 RID: 9891 RVA: 0x000CC281 File Offset: 0x000CA481
		[DataField("immutable", false, 1, false, false, null)]
		public bool Immutable { get; private set; }

		// Token: 0x170005BB RID: 1467
		// (get) Token: 0x060026A4 RID: 9892 RVA: 0x000CC28A File Offset: 0x000CA48A
		[ViewVariables]
		public float TotalMoles
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return NumericsHelpers.HorizontalAdd(this.Moles);
			}
		}

		// Token: 0x170005BC RID: 1468
		// (get) Token: 0x060026A5 RID: 9893 RVA: 0x000CC29C File Offset: 0x000CA49C
		[ViewVariables]
		public float Pressure
		{
			get
			{
				if (this.Volume <= 0f)
				{
					return 0f;
				}
				return this.TotalMoles * 8.314463f * this.Temperature / this.Volume;
			}
		}

		// Token: 0x170005BD RID: 1469
		// (get) Token: 0x060026A6 RID: 9894 RVA: 0x000CC2CB File Offset: 0x000CA4CB
		// (set) Token: 0x060026A7 RID: 9895 RVA: 0x000CC2D3 File Offset: 0x000CA4D3
		[ViewVariables]
		public float Temperature
		{
			get
			{
				return this._temperature;
			}
			set
			{
				if (this.Immutable)
				{
					return;
				}
				this._temperature = MathF.Max(value, 2.7f);
			}
		}

		// Token: 0x170005BE RID: 1470
		// (get) Token: 0x060026A8 RID: 9896 RVA: 0x000CC2EF File Offset: 0x000CA4EF
		// (set) Token: 0x060026A9 RID: 9897 RVA: 0x000CC2F7 File Offset: 0x000CA4F7
		[DataField("volume", false, 1, false, false, null)]
		[ViewVariables]
		public float Volume { get; set; }

		// Token: 0x060026AA RID: 9898 RVA: 0x000CC300 File Offset: 0x000CA500
		public GasMixture()
		{
		}

		// Token: 0x060026AB RID: 9899 RVA: 0x000CC33C File Offset: 0x000CA53C
		public GasMixture(float volume = 0f)
		{
			if (volume < 0f)
			{
				volume = 0f;
			}
			this.Volume = volume;
		}

		// Token: 0x060026AC RID: 9900 RVA: 0x000CC397 File Offset: 0x000CA597
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void MarkImmutable()
		{
			this.Immutable = true;
		}

		// Token: 0x060026AD RID: 9901 RVA: 0x000CC3A0 File Offset: 0x000CA5A0
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public float GetMoles(int gasId)
		{
			return this.Moles[gasId];
		}

		// Token: 0x060026AE RID: 9902 RVA: 0x000CC3AA File Offset: 0x000CA5AA
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public float GetMoles(Gas gas)
		{
			return this.GetMoles((int)gas);
		}

		// Token: 0x060026AF RID: 9903 RVA: 0x000CC3B4 File Offset: 0x000CA5B4
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void SetMoles(int gasId, float quantity)
		{
			if (!float.IsFinite(quantity) || float.IsNegative(quantity))
			{
				DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(30, 1);
				defaultInterpolatedStringHandler.AppendLiteral("Invalid quantity \"");
				defaultInterpolatedStringHandler.AppendFormatted<float>(quantity);
				defaultInterpolatedStringHandler.AppendLiteral("\" specified!");
				throw new ArgumentException(defaultInterpolatedStringHandler.ToStringAndClear(), "quantity");
			}
			if (!this.Immutable)
			{
				this.Moles[gasId] = quantity;
			}
		}

		// Token: 0x060026B0 RID: 9904 RVA: 0x000CC41E File Offset: 0x000CA61E
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void SetMoles(Gas gas, float quantity)
		{
			this.SetMoles((int)gas, quantity);
		}

		// Token: 0x060026B1 RID: 9905 RVA: 0x000CC428 File Offset: 0x000CA628
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void AdjustMoles(int gasId, float quantity)
		{
			if (!this.Immutable)
			{
				if (!float.IsFinite(quantity))
				{
					DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(30, 1);
					defaultInterpolatedStringHandler.AppendLiteral("Invalid quantity \"");
					defaultInterpolatedStringHandler.AppendFormatted<float>(quantity);
					defaultInterpolatedStringHandler.AppendLiteral("\" specified!");
					throw new ArgumentException(defaultInterpolatedStringHandler.ToStringAndClear(), "quantity");
				}
				this.Moles[gasId] += quantity;
				float moles = this.Moles[gasId];
				if (!float.IsFinite(moles) || float.IsNegative(moles))
				{
					DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(66, 3);
					defaultInterpolatedStringHandler.AppendLiteral("Invalid mole quantity \"");
					defaultInterpolatedStringHandler.AppendFormatted<float>(moles);
					defaultInterpolatedStringHandler.AppendLiteral("\" in gas Id ");
					defaultInterpolatedStringHandler.AppendFormatted<int>(gasId);
					defaultInterpolatedStringHandler.AppendLiteral(" after adjusting moles with \"");
					defaultInterpolatedStringHandler.AppendFormatted<float>(quantity);
					defaultInterpolatedStringHandler.AppendLiteral("\"!");
					throw new Exception(defaultInterpolatedStringHandler.ToStringAndClear());
				}
			}
		}

		// Token: 0x060026B2 RID: 9906 RVA: 0x000CC50D File Offset: 0x000CA70D
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void AdjustMoles(Gas gas, float moles)
		{
			this.AdjustMoles((int)gas, moles);
		}

		// Token: 0x060026B3 RID: 9907 RVA: 0x000CC517 File Offset: 0x000CA717
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public GasMixture Remove(float amount)
		{
			return this.RemoveRatio(amount / this.TotalMoles);
		}

		// Token: 0x060026B4 RID: 9908 RVA: 0x000CC528 File Offset: 0x000CA728
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public GasMixture RemoveRatio(float ratio)
		{
			if (ratio > 0f)
			{
				if (ratio > 1f)
				{
					ratio = 1f;
				}
				GasMixture removed = new GasMixture(this.Volume)
				{
					Temperature = this.Temperature
				};
				this.Moles.CopyTo(removed.Moles.AsSpan<float>());
				NumericsHelpers.Multiply(removed.Moles, ratio);
				if (!this.Immutable)
				{
					NumericsHelpers.Sub(this.Moles, removed.Moles);
				}
				for (int i = 0; i < this.Moles.Length; i++)
				{
					float moles = this.Moles[i];
					float otherMoles = removed.Moles[i];
					if (moles < 5E-08f || float.IsNaN(moles))
					{
						this.Moles[i] = 0f;
					}
					if (otherMoles < 5E-08f || float.IsNaN(otherMoles))
					{
						removed.Moles[i] = 0f;
					}
				}
				return removed;
			}
			return new GasMixture(this.Volume)
			{
				Temperature = this.Temperature
			};
		}

		// Token: 0x060026B5 RID: 9909 RVA: 0x000CC628 File Offset: 0x000CA828
		public GasMixture RemoveVolume(float vol)
		{
			return this.RemoveRatio(vol / this.Volume);
		}

		// Token: 0x060026B6 RID: 9910 RVA: 0x000CC638 File Offset: 0x000CA838
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void CopyFromMutable(GasMixture sample)
		{
			if (this.Immutable)
			{
				return;
			}
			sample.Moles.CopyTo(this.Moles, 0);
			this.Temperature = sample.Temperature;
		}

		// Token: 0x060026B7 RID: 9911 RVA: 0x000CC661 File Offset: 0x000CA861
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Clear()
		{
			if (this.Immutable)
			{
				return;
			}
			Array.Clear(this.Moles, 0, 9);
		}

		// Token: 0x060026B8 RID: 9912 RVA: 0x000CC67A File Offset: 0x000CA87A
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Multiply(float multiplier)
		{
			if (this.Immutable)
			{
				return;
			}
			NumericsHelpers.Multiply(this.Moles, multiplier);
		}

		// Token: 0x060026B9 RID: 9913 RVA: 0x000CC696 File Offset: 0x000CA896
		void ISerializationHooks.AfterDeserialization()
		{
			Array.Resize<float>(ref this.Moles, Atmospherics.AdjustedNumberOfGases);
		}

		// Token: 0x060026BA RID: 9914 RVA: 0x000CC6A8 File Offset: 0x000CA8A8
		[NullableContext(2)]
		public override bool Equals(object obj)
		{
			GasMixture mix = obj as GasMixture;
			return mix != null && this.Equals(mix);
		}

		// Token: 0x060026BB RID: 9915 RVA: 0x000CC6C8 File Offset: 0x000CA8C8
		[NullableContext(2)]
		public bool Equals(GasMixture other)
		{
			return other != null && (this == other || (this.Moles.SequenceEqual(other.Moles) && this._temperature.Equals(other._temperature) && this.ReactionResults.SequenceEqual(other.ReactionResults) && this.Immutable == other.Immutable && this.Volume.Equals(other.Volume)));
		}

		// Token: 0x060026BC RID: 9916 RVA: 0x000CC740 File Offset: 0x000CA940
		public override int GetHashCode()
		{
			HashCode hashCode = default(HashCode);
			for (int i = 0; i < 9; i++)
			{
				float moles = this.Moles[i];
				hashCode.Add<float>(moles);
			}
			hashCode.Add<float>(this._temperature);
			hashCode.Add<bool>(this.Immutable);
			hashCode.Add<float>(this.Volume);
			return hashCode.ToHashCode();
		}

		// Token: 0x060026BD RID: 9917 RVA: 0x000CC7A4 File Offset: 0x000CA9A4
		public GasMixture Clone()
		{
			return new GasMixture
			{
				Moles = (float[])this.Moles.Clone(),
				_temperature = this._temperature,
				Immutable = this.Immutable,
				Volume = this.Volume
			};
		}

		// Token: 0x04001803 RID: 6147
		[DataField("moles", false, 1, false, false, null)]
		[ViewVariables]
		public float[] Moles = new float[Atmospherics.AdjustedNumberOfGases];

		// Token: 0x04001804 RID: 6148
		[DataField("temperature", false, 1, false, false, null)]
		[ViewVariables]
		private float _temperature = 2.7f;

		// Token: 0x04001806 RID: 6150
		[ViewVariables]
		public readonly Dictionary<GasReaction, float> ReactionResults = new Dictionary<GasReaction, float>
		{
			{
				GasReaction.Fire,
				0f
			}
		};
	}
}
