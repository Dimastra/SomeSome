using System;
using System.Runtime.CompilerServices;
using Content.Server.Damage.Components;
using Content.Server.Popups;
using Content.Shared.Damage;
using Content.Shared.FixedPoint;
using Content.Shared.Popups;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;

namespace Content.Server.Damage.Systems
{
	// Token: 0x020005C4 RID: 1476
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class DamagePopupSystem : EntitySystem
	{
		// Token: 0x06001F80 RID: 8064 RVA: 0x000A56C6 File Offset: 0x000A38C6
		public override void Initialize()
		{
			base.Initialize();
			base.SubscribeLocalEvent<DamagePopupComponent, DamageChangedEvent>(new ComponentEventHandler<DamagePopupComponent, DamageChangedEvent>(this.OnDamageChange), null, null);
		}

		// Token: 0x06001F81 RID: 8065 RVA: 0x000A56E4 File Offset: 0x000A38E4
		private void OnDamageChange(EntityUid uid, DamagePopupComponent component, DamageChangedEvent args)
		{
			if (args.DamageDelta != null)
			{
				FixedPoint2 damageTotal = args.Damageable.TotalDamage;
				FixedPoint2 damageDelta = args.DamageDelta.Total;
				string text;
				switch (component.Type)
				{
				case DamagePopupType.Combined:
					text = damageDelta.ToString() + " | " + damageTotal.ToString();
					break;
				case DamagePopupType.Total:
					text = damageTotal.ToString();
					break;
				case DamagePopupType.Delta:
					text = damageDelta.ToString();
					break;
				case DamagePopupType.Hit:
					text = "!";
					break;
				default:
					text = "Invalid type";
					break;
				}
				string msg = text;
				this._popupSystem.PopupEntity(msg, uid, PopupType.Small);
			}
		}

		// Token: 0x04001392 RID: 5010
		[Dependency]
		private readonly PopupSystem _popupSystem;
	}
}
