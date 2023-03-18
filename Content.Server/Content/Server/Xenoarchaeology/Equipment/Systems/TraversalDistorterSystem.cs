using System;
using System.Runtime.CompilerServices;
using Content.Server.Construction;
using Content.Server.Popups;
using Content.Server.Power.EntitySystems;
using Content.Server.Xenoarchaeology.Equipment.Components;
using Content.Server.Xenoarchaeology.XenoArtifacts;
using Content.Shared.Examine;
using Content.Shared.Interaction;
using Content.Shared.Popups;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Localization;
using Robust.Shared.Physics.Events;
using Robust.Shared.Timing;

namespace Content.Server.Xenoarchaeology.Equipment.Systems
{
	// Token: 0x02000064 RID: 100
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class TraversalDistorterSystem : EntitySystem
	{
		// Token: 0x0600012E RID: 302 RVA: 0x00007D10 File Offset: 0x00005F10
		public override void Initialize()
		{
			base.SubscribeLocalEvent<TraversalDistorterComponent, MapInitEvent>(new ComponentEventHandler<TraversalDistorterComponent, MapInitEvent>(this.OnInit), null, null);
			base.SubscribeLocalEvent<TraversalDistorterComponent, ActivateInWorldEvent>(new ComponentEventHandler<TraversalDistorterComponent, ActivateInWorldEvent>(this.OnInteract), null, null);
			base.SubscribeLocalEvent<TraversalDistorterComponent, ExaminedEvent>(new ComponentEventHandler<TraversalDistorterComponent, ExaminedEvent>(this.OnExamine), null, null);
			base.SubscribeLocalEvent<TraversalDistorterComponent, RefreshPartsEvent>(new ComponentEventHandler<TraversalDistorterComponent, RefreshPartsEvent>(this.OnRefreshParts), null, null);
			base.SubscribeLocalEvent<TraversalDistorterComponent, UpgradeExamineEvent>(new ComponentEventHandler<TraversalDistorterComponent, UpgradeExamineEvent>(this.OnUpgradeExamine), null, null);
			base.SubscribeLocalEvent<TraversalDistorterComponent, StartCollideEvent>(new ComponentEventRefHandler<TraversalDistorterComponent, StartCollideEvent>(this.OnCollide), null, null);
			base.SubscribeLocalEvent<TraversalDistorterComponent, EndCollideEvent>(new ComponentEventRefHandler<TraversalDistorterComponent, EndCollideEvent>(this.OnEndCollide), null, null);
		}

		// Token: 0x0600012F RID: 303 RVA: 0x00007DA9 File Offset: 0x00005FA9
		private void OnInit(EntityUid uid, TraversalDistorterComponent component, MapInitEvent args)
		{
			component.NextActivation = this._timing.CurTime;
		}

		// Token: 0x06000130 RID: 304 RVA: 0x00007DBC File Offset: 0x00005FBC
		private void OnInteract(EntityUid uid, TraversalDistorterComponent component, ActivateInWorldEvent args)
		{
			if (args.Handled || !this.IsPowered(uid, this.EntityManager, null))
			{
				return;
			}
			if (this._timing.CurTime < component.NextActivation)
			{
				return;
			}
			args.Handled = true;
			component.NextActivation = this._timing.CurTime + component.ActivationDelay;
			component.BiasDirection = ((component.BiasDirection == BiasDirection.In) ? BiasDirection.Out : BiasDirection.In);
			string toPopup = string.Empty;
			BiasDirection biasDirection = component.BiasDirection;
			if (biasDirection != BiasDirection.In)
			{
				if (biasDirection == BiasDirection.Out)
				{
					toPopup = Loc.GetString("traversal-distorter-set-out");
				}
			}
			else
			{
				toPopup = Loc.GetString("traversal-distorter-set-in");
			}
			this._popup.PopupEntity(toPopup, uid, PopupType.Small);
		}

		// Token: 0x06000131 RID: 305 RVA: 0x00007E6C File Offset: 0x0000606C
		private void OnExamine(EntityUid uid, TraversalDistorterComponent component, ExaminedEvent args)
		{
			string examine = string.Empty;
			BiasDirection biasDirection = component.BiasDirection;
			if (biasDirection != BiasDirection.In)
			{
				if (biasDirection == BiasDirection.Out)
				{
					examine = Loc.GetString("traversal-distorter-desc-out");
				}
			}
			else
			{
				examine = Loc.GetString("traversal-distorter-desc-in");
			}
			args.Message.AddMarkup(examine);
		}

		// Token: 0x06000132 RID: 306 RVA: 0x00007EB4 File Offset: 0x000060B4
		private void OnRefreshParts(EntityUid uid, TraversalDistorterComponent component, RefreshPartsEvent args)
		{
			float biasRating = args.PartRatings[component.MachinePartBiasChance];
			component.BiasChance = component.BaseBiasChance * MathF.Pow(component.PartRatingBiasChance, biasRating - 1f);
		}

		// Token: 0x06000133 RID: 307 RVA: 0x00007EF2 File Offset: 0x000060F2
		private void OnUpgradeExamine(EntityUid uid, TraversalDistorterComponent component, UpgradeExamineEvent args)
		{
			args.AddPercentageUpgrade("traversal-distorter-upgrade-bias", component.BiasChance / component.BaseBiasChance);
		}

		// Token: 0x06000134 RID: 308 RVA: 0x00007F0C File Offset: 0x0000610C
		private void OnCollide(EntityUid uid, TraversalDistorterComponent component, ref StartCollideEvent args)
		{
			EntityUid otherEnt = args.OtherFixture.Body.Owner;
			if (!base.HasComp<ArtifactComponent>(otherEnt))
			{
				return;
			}
			base.EnsureComp<BiasedArtifactComponent>(otherEnt).Provider = uid;
		}

		// Token: 0x06000135 RID: 309 RVA: 0x00007F44 File Offset: 0x00006144
		private void OnEndCollide(EntityUid uid, TraversalDistorterComponent component, ref EndCollideEvent args)
		{
			EntityUid otherEnt = args.OtherFixture.Body.Owner;
			if (!base.HasComp<ArtifactComponent>(otherEnt))
			{
				return;
			}
			BiasedArtifactComponent bias;
			if (base.TryComp<BiasedArtifactComponent>(otherEnt, ref bias) && bias.Provider == uid)
			{
				base.RemComp(otherEnt, bias);
			}
		}

		// Token: 0x040000F2 RID: 242
		[Dependency]
		private readonly PopupSystem _popup;

		// Token: 0x040000F3 RID: 243
		[Dependency]
		private readonly IGameTiming _timing;
	}
}
