using System;
using Content.Shared.Movement.Systems;
using Robust.Shared.Analyzers;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.ViewVariables;

namespace Content.Shared.Movement.Components
{
	// Token: 0x020002F1 RID: 753
	[RegisterComponent]
	[NetworkedComponent]
	[Access(new Type[]
	{
		typeof(MovementSpeedModifierSystem)
	})]
	public sealed class MovementSpeedModifierComponent : Component
	{
		// Token: 0x17000196 RID: 406
		// (get) Token: 0x0600086F RID: 2159 RVA: 0x0001CB8F File Offset: 0x0001AD8F
		// (set) Token: 0x06000870 RID: 2160 RVA: 0x0001CB97 File Offset: 0x0001AD97
		[ViewVariables]
		private float _baseWalkSpeedVV
		{
			get
			{
				return this.BaseWalkSpeed;
			}
			set
			{
				this.BaseWalkSpeed = value;
				base.Dirty(null);
			}
		}

		// Token: 0x17000197 RID: 407
		// (get) Token: 0x06000871 RID: 2161 RVA: 0x0001CBA7 File Offset: 0x0001ADA7
		// (set) Token: 0x06000872 RID: 2162 RVA: 0x0001CBAF File Offset: 0x0001ADAF
		[ViewVariables]
		private float _baseSprintSpeedVV
		{
			get
			{
				return this.BaseSprintSpeed;
			}
			set
			{
				this.BaseSprintSpeed = value;
				base.Dirty(null);
			}
		}

		// Token: 0x17000198 RID: 408
		// (get) Token: 0x06000873 RID: 2163 RVA: 0x0001CBBF File Offset: 0x0001ADBF
		// (set) Token: 0x06000874 RID: 2164 RVA: 0x0001CBC7 File Offset: 0x0001ADC7
		[ViewVariables]
		[DataField("baseWalkSpeed", false, 1, false, false, null)]
		public float BaseWalkSpeed { get; set; } = 2.5f;

		// Token: 0x17000199 RID: 409
		// (get) Token: 0x06000875 RID: 2165 RVA: 0x0001CBD0 File Offset: 0x0001ADD0
		// (set) Token: 0x06000876 RID: 2166 RVA: 0x0001CBD8 File Offset: 0x0001ADD8
		[ViewVariables]
		[DataField("baseSprintSpeed", false, 1, false, false, null)]
		public float BaseSprintSpeed { get; set; } = 4.5f;

		// Token: 0x1700019A RID: 410
		// (get) Token: 0x06000877 RID: 2167 RVA: 0x0001CBE1 File Offset: 0x0001ADE1
		[ViewVariables]
		public float CurrentWalkSpeed
		{
			get
			{
				return this.WalkSpeedModifier * this.BaseWalkSpeed;
			}
		}

		// Token: 0x1700019B RID: 411
		// (get) Token: 0x06000878 RID: 2168 RVA: 0x0001CBF0 File Offset: 0x0001ADF0
		[ViewVariables]
		public float CurrentSprintSpeed
		{
			get
			{
				return this.SprintSpeedModifier * this.BaseSprintSpeed;
			}
		}

		// Token: 0x04000886 RID: 2182
		public const float DefaultMinimumFrictionSpeed = 0.005f;

		// Token: 0x04000887 RID: 2183
		public const float DefaultWeightlessFriction = 1f;

		// Token: 0x04000888 RID: 2184
		public const float DefaultWeightlessFrictionNoInput = 0.2f;

		// Token: 0x04000889 RID: 2185
		public const float DefaultWeightlessModifier = 0.7f;

		// Token: 0x0400088A RID: 2186
		public const float DefaultWeightlessAcceleration = 1f;

		// Token: 0x0400088B RID: 2187
		public const float DefaultAcceleration = 20f;

		// Token: 0x0400088C RID: 2188
		public const float DefaultFriction = 20f;

		// Token: 0x0400088D RID: 2189
		public const float DefaultFrictionNoInput = 20f;

		// Token: 0x0400088E RID: 2190
		public const float DefaultBaseWalkSpeed = 2.5f;

		// Token: 0x0400088F RID: 2191
		public const float DefaultBaseSprintSpeed = 4.5f;

		// Token: 0x04000890 RID: 2192
		[ViewVariables]
		public float WalkSpeedModifier = 1f;

		// Token: 0x04000891 RID: 2193
		[ViewVariables]
		public float SprintSpeedModifier = 1f;

		// Token: 0x04000892 RID: 2194
		[ViewVariables]
		[DataField("minimumFrictionSpeed", false, 1, false, false, null)]
		public float MinimumFrictionSpeed = 0.005f;

		// Token: 0x04000893 RID: 2195
		[ViewVariables]
		[DataField("weightlessFriction", false, 1, false, false, null)]
		public float WeightlessFriction = 1f;

		// Token: 0x04000894 RID: 2196
		[ViewVariables]
		[DataField("weightlessFrictionNoInput", false, 1, false, false, null)]
		public float WeightlessFrictionNoInput = 0.2f;

		// Token: 0x04000895 RID: 2197
		[ViewVariables]
		[DataField("weightlessModifier", false, 1, false, false, null)]
		public float WeightlessModifier = 0.7f;

		// Token: 0x04000896 RID: 2198
		[ViewVariables]
		[DataField("weightlessAcceleration", false, 1, false, false, null)]
		public float WeightlessAcceleration = 1f;

		// Token: 0x04000897 RID: 2199
		[ViewVariables]
		[DataField("acceleration", false, 1, false, false, null)]
		public float Acceleration = 20f;

		// Token: 0x04000898 RID: 2200
		[ViewVariables]
		[DataField("friction", false, 1, false, false, null)]
		public float Friction = 20f;

		// Token: 0x04000899 RID: 2201
		[ViewVariables]
		[DataField("frictionNoInput", false, 1, false, false, null)]
		public float? FrictionNoInput;
	}
}
