using System;
using System.Runtime.CompilerServices;
using Content.Shared.Nutrition.Components;
using Content.Shared.Stunnable;
using Content.Shared.Throwing;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;

namespace Content.Shared.Nutrition.EntitySystems
{
	// Token: 0x020002AB RID: 683
	[NullableContext(1)]
	[Nullable(0)]
	public abstract class SharedCreamPieSystem : EntitySystem
	{
		// Token: 0x060007A8 RID: 1960 RVA: 0x00019D74 File Offset: 0x00017F74
		public override void Initialize()
		{
			base.Initialize();
			base.SubscribeLocalEvent<CreamPieComponent, ThrowDoHitEvent>(new ComponentEventHandler<CreamPieComponent, ThrowDoHitEvent>(this.OnCreamPieHit), null, null);
			base.SubscribeLocalEvent<CreamPieComponent, LandEvent>(new ComponentEventRefHandler<CreamPieComponent, LandEvent>(this.OnCreamPieLand), null, null);
			base.SubscribeLocalEvent<CreamPiedComponent, ThrowHitByEvent>(new ComponentEventHandler<CreamPiedComponent, ThrowHitByEvent>(this.OnCreamPiedHitBy), null, null);
		}

		// Token: 0x060007A9 RID: 1961 RVA: 0x00019DC3 File Offset: 0x00017FC3
		public void SplatCreamPie(EntityUid uid, CreamPieComponent creamPie)
		{
			if (creamPie.Splatted)
			{
				return;
			}
			creamPie.Splatted = true;
			this.SplattedCreamPie(uid, creamPie);
		}

		// Token: 0x060007AA RID: 1962 RVA: 0x00019DDD File Offset: 0x00017FDD
		protected virtual void SplattedCreamPie(EntityUid uid, CreamPieComponent creamPie)
		{
		}

		// Token: 0x060007AB RID: 1963 RVA: 0x00019DE0 File Offset: 0x00017FE0
		public void SetCreamPied(EntityUid uid, CreamPiedComponent creamPied, bool value)
		{
			if (value == creamPied.CreamPied)
			{
				return;
			}
			creamPied.CreamPied = value;
			AppearanceComponent appearance;
			if (this.EntityManager.TryGetComponent<AppearanceComponent>(uid, ref appearance))
			{
				this._appearance.SetData(uid, CreamPiedVisuals.Creamed, value, appearance);
			}
		}

		// Token: 0x060007AC RID: 1964 RVA: 0x00019E27 File Offset: 0x00018027
		private void OnCreamPieLand(EntityUid uid, CreamPieComponent component, ref LandEvent args)
		{
			this.SplatCreamPie(uid, component);
		}

		// Token: 0x060007AD RID: 1965 RVA: 0x00019E31 File Offset: 0x00018031
		private void OnCreamPieHit(EntityUid uid, CreamPieComponent component, ThrowDoHitEvent args)
		{
			this.SplatCreamPie(uid, component);
		}

		// Token: 0x060007AE RID: 1966 RVA: 0x00019E3C File Offset: 0x0001803C
		private void OnCreamPiedHitBy(EntityUid uid, CreamPiedComponent creamPied, ThrowHitByEvent args)
		{
			CreamPieComponent creamPie;
			if (!this.EntityManager.EntityExists(args.Thrown) || !this.EntityManager.TryGetComponent<CreamPieComponent>(args.Thrown, ref creamPie))
			{
				return;
			}
			this.SetCreamPied(uid, creamPied, true);
			this.CreamedEntity(uid, creamPied, args);
			this._stunSystem.TryParalyze(uid, TimeSpan.FromSeconds((double)creamPie.ParalyzeTime), true, null);
		}

		// Token: 0x060007AF RID: 1967 RVA: 0x00019E9F File Offset: 0x0001809F
		protected virtual void CreamedEntity(EntityUid uid, CreamPiedComponent creamPied, ThrowHitByEvent args)
		{
		}

		// Token: 0x040007BC RID: 1980
		[Dependency]
		private SharedStunSystem _stunSystem;

		// Token: 0x040007BD RID: 1981
		[Dependency]
		private readonly SharedAppearanceSystem _appearance;
	}
}
