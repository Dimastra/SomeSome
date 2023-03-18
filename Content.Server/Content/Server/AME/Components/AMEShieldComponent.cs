using System;
using System.Runtime.CompilerServices;
using Content.Shared.AME;
using Robust.Server.GameObjects;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.ViewVariables;

namespace Content.Server.AME.Components
{
	// Token: 0x020007D9 RID: 2009
	[NullableContext(2)]
	[Nullable(0)]
	[RegisterComponent]
	public sealed class AMEShieldComponent : SharedAMEShieldComponent
	{
		// Token: 0x06002BAE RID: 11182 RVA: 0x000E56E7 File Offset: 0x000E38E7
		protected override void Initialize()
		{
			base.Initialize();
			IEntityManager entityManager = IoCManager.Resolve<IEntityManager>();
			entityManager.TryGetComponent<AppearanceComponent>(base.Owner, ref this._appearance);
			entityManager.TryGetComponent<PointLightComponent>(base.Owner, ref this._pointLight);
		}

		// Token: 0x06002BAF RID: 11183 RVA: 0x000E5719 File Offset: 0x000E3919
		public void SetCore()
		{
			if (this._isCore)
			{
				return;
			}
			this._isCore = true;
			AppearanceComponent appearance = this._appearance;
			if (appearance == null)
			{
				return;
			}
			appearance.SetData(SharedAMEShieldComponent.AMEShieldVisuals.Core, "isCore");
		}

		// Token: 0x06002BB0 RID: 11184 RVA: 0x000E5746 File Offset: 0x000E3946
		public void UnsetCore()
		{
			this._isCore = false;
			AppearanceComponent appearance = this._appearance;
			if (appearance != null)
			{
				appearance.SetData(SharedAMEShieldComponent.AMEShieldVisuals.Core, "isNotCore");
			}
			this.UpdateCoreVisuals(0, false);
		}

		// Token: 0x06002BB1 RID: 11185 RVA: 0x000E5774 File Offset: 0x000E3974
		public void UpdateCoreVisuals(int injectionStrength, bool injecting)
		{
			if (!injecting)
			{
				AppearanceComponent appearance = this._appearance;
				if (appearance != null)
				{
					appearance.SetData(SharedAMEShieldComponent.AMEShieldVisuals.CoreState, "off");
				}
				if (this._pointLight != null)
				{
					this._pointLight.Enabled = false;
				}
				return;
			}
			if (this._pointLight != null)
			{
				this._pointLight.Radius = (float)Math.Clamp(injectionStrength, 1, 12);
				this._pointLight.Enabled = true;
			}
			if (injectionStrength > 2)
			{
				AppearanceComponent appearance2 = this._appearance;
				if (appearance2 == null)
				{
					return;
				}
				appearance2.SetData(SharedAMEShieldComponent.AMEShieldVisuals.CoreState, "strong");
				return;
			}
			else
			{
				AppearanceComponent appearance3 = this._appearance;
				if (appearance3 == null)
				{
					return;
				}
				appearance3.SetData(SharedAMEShieldComponent.AMEShieldVisuals.CoreState, "weak");
				return;
			}
		}

		// Token: 0x04001B1A RID: 6938
		private bool _isCore;

		// Token: 0x04001B1B RID: 6939
		[ViewVariables]
		public int CoreIntegrity = 100;

		// Token: 0x04001B1C RID: 6940
		private AppearanceComponent _appearance;

		// Token: 0x04001B1D RID: 6941
		private PointLightComponent _pointLight;
	}
}
