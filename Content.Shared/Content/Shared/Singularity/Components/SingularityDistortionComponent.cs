using System;
using System.Runtime.CompilerServices;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.ViewVariables;

namespace Content.Shared.Singularity.Components
{
	// Token: 0x020001B7 RID: 439
	[RegisterComponent]
	[NetworkedComponent]
	public sealed class SingularityDistortionComponent : Component
	{
		// Token: 0x170000E7 RID: 231
		// (get) Token: 0x0600050E RID: 1294 RVA: 0x0001362F File Offset: 0x0001182F
		// (set) Token: 0x0600050F RID: 1295 RVA: 0x00013637 File Offset: 0x00011837
		[ViewVariables]
		public float Intensity
		{
			get
			{
				return this._intensity;
			}
			set
			{
				ComponentExt.SetAndDirtyIfChanged<float>(this, ref this._intensity, value);
			}
		}

		// Token: 0x170000E8 RID: 232
		// (get) Token: 0x06000510 RID: 1296 RVA: 0x00013647 File Offset: 0x00011847
		// (set) Token: 0x06000511 RID: 1297 RVA: 0x0001364F File Offset: 0x0001184F
		[ViewVariables]
		public float FalloffPower
		{
			get
			{
				return this._falloffPower;
			}
			set
			{
				ComponentExt.SetAndDirtyIfChanged<float>(this, ref this._falloffPower, value);
			}
		}

		// Token: 0x06000512 RID: 1298 RVA: 0x0001365F File Offset: 0x0001185F
		[NullableContext(1)]
		public override ComponentState GetComponentState()
		{
			return new SingularityDistortionComponentState(this.Intensity, this.FalloffPower);
		}

		// Token: 0x06000513 RID: 1299 RVA: 0x00013674 File Offset: 0x00011874
		[NullableContext(2)]
		public override void HandleComponentState(ComponentState curState, ComponentState nextState)
		{
			base.HandleComponentState(curState, nextState);
			SingularityDistortionComponentState state = curState as SingularityDistortionComponentState;
			if (state == null)
			{
				return;
			}
			this.Intensity = state.Intensity;
			this.FalloffPower = state.Falloff;
		}

		// Token: 0x0400050E RID: 1294
		[DataField("intensity", false, 1, false, false, null)]
		private float _intensity = 31.25f;

		// Token: 0x0400050F RID: 1295
		[DataField("falloffPower", false, 1, false, false, null)]
		private float _falloffPower = MathF.Sqrt(2f);
	}
}
