using System;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Map;
using Robust.Shared.Maths;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.ViewVariables;

namespace Content.Shared.Movement.Components
{
	// Token: 0x020002EB RID: 747
	[RegisterComponent]
	[NetworkedComponent]
	public sealed class MobMoverComponent : Component
	{
		// Token: 0x17000192 RID: 402
		// (get) Token: 0x06000861 RID: 2145 RVA: 0x0001C958 File Offset: 0x0001AB58
		// (set) Token: 0x06000862 RID: 2146 RVA: 0x0001C960 File Offset: 0x0001AB60
		[ViewVariables]
		public EntityCoordinates LastPosition { get; set; }

		// Token: 0x17000193 RID: 403
		// (get) Token: 0x06000863 RID: 2147 RVA: 0x0001C969 File Offset: 0x0001AB69
		// (set) Token: 0x06000864 RID: 2148 RVA: 0x0001C971 File Offset: 0x0001AB71
		[ViewVariables]
		public float StepSoundDistance
		{
			get
			{
				return this._stepSoundDistance;
			}
			set
			{
				if (MathHelper.CloseToPercent(this._stepSoundDistance, value, 1E-05))
				{
					return;
				}
				this._stepSoundDistance = value;
			}
		}

		// Token: 0x17000194 RID: 404
		// (get) Token: 0x06000865 RID: 2149 RVA: 0x0001C992 File Offset: 0x0001AB92
		// (set) Token: 0x06000866 RID: 2150 RVA: 0x0001C99A File Offset: 0x0001AB9A
		[ViewVariables]
		public float GrabRangeVV
		{
			get
			{
				return this.GrabRange;
			}
			set
			{
				if (MathHelper.CloseToPercent(this.GrabRange, value, 1E-05))
				{
					return;
				}
				this.GrabRange = value;
				base.Dirty(null);
			}
		}

		// Token: 0x17000195 RID: 405
		// (get) Token: 0x06000867 RID: 2151 RVA: 0x0001C9C2 File Offset: 0x0001ABC2
		// (set) Token: 0x06000868 RID: 2152 RVA: 0x0001C9CA File Offset: 0x0001ABCA
		[ViewVariables]
		public float PushStrengthVV
		{
			get
			{
				return this.PushStrength;
			}
			set
			{
				if (MathHelper.CloseToPercent(this.PushStrength, value, 1E-05))
				{
					return;
				}
				this.PushStrength = value;
				base.Dirty(null);
			}
		}

		// Token: 0x0400087F RID: 2175
		private float _stepSoundDistance;

		// Token: 0x04000880 RID: 2176
		[DataField("grabRange", false, 1, false, false, null)]
		public float GrabRange = 1f;

		// Token: 0x04000881 RID: 2177
		[DataField("pushStrength", false, 1, false, false, null)]
		public float PushStrength = 600f;
	}
}
