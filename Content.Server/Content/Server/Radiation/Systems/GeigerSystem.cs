using System;
using System.Runtime.CompilerServices;
using Content.Server.Radiation.Components;
using Content.Server.Radiation.Events;
using Content.Shared.Hands;
using Content.Shared.Interaction;
using Content.Shared.Inventory.Events;
using Content.Shared.Radiation.Components;
using Content.Shared.Radiation.Systems;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.IoC;
using Robust.Shared.Maths;

namespace Content.Server.Radiation.Systems
{
	// Token: 0x02000263 RID: 611
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class GeigerSystem : SharedGeigerSystem
	{
		// Token: 0x06000C07 RID: 3079 RVA: 0x0003F7C8 File Offset: 0x0003D9C8
		public override void Initialize()
		{
			base.Initialize();
			base.SubscribeLocalEvent<GeigerComponent, ActivateInWorldEvent>(new ComponentEventHandler<GeigerComponent, ActivateInWorldEvent>(this.OnActivate), null, null);
			base.SubscribeLocalEvent<GeigerComponent, GotEquippedEvent>(new ComponentEventHandler<GeigerComponent, GotEquippedEvent>(this.OnEquipped), null, null);
			base.SubscribeLocalEvent<GeigerComponent, GotEquippedHandEvent>(new ComponentEventHandler<GeigerComponent, GotEquippedHandEvent>(this.OnEquippedHand), null, null);
			base.SubscribeLocalEvent<GeigerComponent, GotUnequippedEvent>(new ComponentEventHandler<GeigerComponent, GotUnequippedEvent>(this.OnUnequipped), null, null);
			base.SubscribeLocalEvent<GeigerComponent, GotUnequippedHandEvent>(new ComponentEventHandler<GeigerComponent, GotUnequippedHandEvent>(this.OnUnequippedHand), null, null);
			base.SubscribeLocalEvent<RadiationSystemUpdatedEvent>(new EntityEventHandler<RadiationSystemUpdatedEvent>(this.OnUpdate), null, null);
			base.SubscribeLocalEvent<GeigerComponent, ComponentGetState>(new ComponentEventRefHandler<GeigerComponent, ComponentGetState>(this.OnGetState), null, null);
		}

		// Token: 0x06000C08 RID: 3080 RVA: 0x0003F867 File Offset: 0x0003DA67
		private void OnActivate(EntityUid uid, GeigerComponent component, ActivateInWorldEvent args)
		{
			if (args.Handled || component.AttachedToSuit)
			{
				return;
			}
			args.Handled = true;
			this.SetEnabled(uid, component, !component.IsEnabled);
		}

		// Token: 0x06000C09 RID: 3081 RVA: 0x0003F892 File Offset: 0x0003DA92
		private void OnEquipped(EntityUid uid, GeigerComponent component, GotEquippedEvent args)
		{
			if (component.AttachedToSuit)
			{
				this.SetEnabled(uid, component, true);
			}
			this.SetUser(component, new EntityUid?(args.Equipee));
		}

		// Token: 0x06000C0A RID: 3082 RVA: 0x0003F8B7 File Offset: 0x0003DAB7
		private void OnEquippedHand(EntityUid uid, GeigerComponent component, GotEquippedHandEvent args)
		{
			if (component.AttachedToSuit)
			{
				return;
			}
			this.SetUser(component, new EntityUid?(args.User));
		}

		// Token: 0x06000C0B RID: 3083 RVA: 0x0003F8D4 File Offset: 0x0003DAD4
		private void OnUnequipped(EntityUid uid, GeigerComponent component, GotUnequippedEvent args)
		{
			if (component.AttachedToSuit)
			{
				this.SetEnabled(uid, component, false);
			}
			this.SetUser(component, null);
		}

		// Token: 0x06000C0C RID: 3084 RVA: 0x0003F904 File Offset: 0x0003DB04
		private void OnUnequippedHand(EntityUid uid, GeigerComponent component, GotUnequippedHandEvent args)
		{
			if (component.AttachedToSuit)
			{
				return;
			}
			this.SetUser(component, null);
		}

		// Token: 0x06000C0D RID: 3085 RVA: 0x0003F92C File Offset: 0x0003DB2C
		private void OnUpdate(RadiationSystemUpdatedEvent ev)
		{
			foreach (ValueTuple<GeigerComponent, RadiationReceiverComponent> valueTuple in base.EntityQuery<GeigerComponent, RadiationReceiverComponent>(false))
			{
				GeigerComponent geiger = valueTuple.Item1;
				float rads = valueTuple.Item2.CurrentRadiation;
				this.SetCurrentRadiation(geiger.Owner, geiger, rads);
			}
		}

		// Token: 0x06000C0E RID: 3086 RVA: 0x0003F994 File Offset: 0x0003DB94
		private void OnGetState(EntityUid uid, GeigerComponent component, ref ComponentGetState args)
		{
			args.State = new GeigerComponentState
			{
				CurrentRadiation = component.CurrentRadiation,
				DangerLevel = component.DangerLevel,
				IsEnabled = component.IsEnabled,
				User = component.User
			};
		}

		// Token: 0x06000C0F RID: 3087 RVA: 0x0003F9D4 File Offset: 0x0003DBD4
		private void SetCurrentRadiation(EntityUid uid, GeigerComponent component, float rads)
		{
			if (MathHelper.CloseTo(component.CurrentRadiation, rads, GeigerSystem.ApproxEqual))
			{
				return;
			}
			GeigerDangerLevel dangerLevel = component.DangerLevel;
			GeigerDangerLevel newLevel = GeigerSystem.RadsToLevel(rads);
			component.CurrentRadiation = rads;
			component.DangerLevel = newLevel;
			if (dangerLevel != newLevel)
			{
				this.UpdateAppearance(uid, component, null);
			}
			base.Dirty(component, null);
		}

		// Token: 0x06000C10 RID: 3088 RVA: 0x0003FA24 File Offset: 0x0003DC24
		private void SetUser(GeigerComponent component, EntityUid? user)
		{
			if (component.User == user)
			{
				return;
			}
			component.User = user;
			base.Dirty(component, null);
		}

		// Token: 0x06000C11 RID: 3089 RVA: 0x0003FA80 File Offset: 0x0003DC80
		private void SetEnabled(EntityUid uid, GeigerComponent component, bool isEnabled)
		{
			if (component.IsEnabled == isEnabled)
			{
				return;
			}
			component.IsEnabled = isEnabled;
			if (!isEnabled)
			{
				component.CurrentRadiation = 0f;
				component.DangerLevel = GeigerDangerLevel.None;
			}
			this._radiation.SetCanReceive(uid, isEnabled);
			this.UpdateAppearance(uid, component, null);
			base.Dirty(component, null);
		}

		// Token: 0x06000C12 RID: 3090 RVA: 0x0003FAD4 File Offset: 0x0003DCD4
		[NullableContext(2)]
		private void UpdateAppearance(EntityUid uid, GeigerComponent component = null, AppearanceComponent appearance = null)
		{
			if (!base.Resolve<GeigerComponent, AppearanceComponent>(uid, ref component, ref appearance, false))
			{
				return;
			}
			this._appearance.SetData(uid, GeigerVisuals.IsEnabled, component.IsEnabled, appearance);
			this._appearance.SetData(uid, GeigerVisuals.DangerLevel, component.DangerLevel, appearance);
		}

		// Token: 0x06000C13 RID: 3091 RVA: 0x0003FB2C File Offset: 0x0003DD2C
		public static GeigerDangerLevel RadsToLevel(float rads)
		{
			GeigerDangerLevel result;
			if (rads >= 0.2f)
			{
				if (rads >= 1f)
				{
					if (rads >= 3f)
					{
						if (rads >= 6f)
						{
							result = GeigerDangerLevel.Extreme;
						}
						else
						{
							result = GeigerDangerLevel.High;
						}
					}
					else
					{
						result = GeigerDangerLevel.Med;
					}
				}
				else
				{
					result = GeigerDangerLevel.Low;
				}
			}
			else
			{
				result = GeigerDangerLevel.None;
			}
			return result;
		}

		// Token: 0x0400078F RID: 1935
		[Dependency]
		private readonly SharedAppearanceSystem _appearance;

		// Token: 0x04000790 RID: 1936
		[Dependency]
		private readonly RadiationSystem _radiation;

		// Token: 0x04000791 RID: 1937
		private static readonly float ApproxEqual = 0.01f;
	}
}
