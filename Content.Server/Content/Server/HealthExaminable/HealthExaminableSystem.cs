using System;
using System.Runtime.CompilerServices;
using Content.Shared.Damage;
using Content.Shared.Examine;
using Content.Shared.FixedPoint;
using Content.Shared.IdentityManagement;
using Content.Shared.Verbs;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Localization;
using Robust.Shared.Utility;

namespace Content.Server.HealthExaminable
{
	// Token: 0x02000476 RID: 1142
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class HealthExaminableSystem : EntitySystem
	{
		// Token: 0x060016D3 RID: 5843 RVA: 0x000781CD File Offset: 0x000763CD
		public override void Initialize()
		{
			base.Initialize();
			base.SubscribeLocalEvent<HealthExaminableComponent, GetVerbsEvent<ExamineVerb>>(new ComponentEventHandler<HealthExaminableComponent, GetVerbsEvent<ExamineVerb>>(this.OnGetExamineVerbs), null, null);
		}

		// Token: 0x060016D4 RID: 5844 RVA: 0x000781EC File Offset: 0x000763EC
		private void OnGetExamineVerbs(EntityUid uid, HealthExaminableComponent component, GetVerbsEvent<ExamineVerb> args)
		{
			DamageableComponent damage;
			if (!base.TryComp<DamageableComponent>(uid, ref damage))
			{
				return;
			}
			bool detailsRange = this._examineSystem.IsInDetailsRange(args.User, uid);
			ExamineVerb verb = new ExamineVerb
			{
				Act = delegate()
				{
					FormattedMessage markup = this.CreateMarkup(uid, component, damage);
					this._examineSystem.SendExamineTooltip(args.User, uid, markup, false, false);
				},
				Text = Loc.GetString("health-examinable-verb-text"),
				Category = VerbCategory.Examine,
				Disabled = !detailsRange,
				Message = Loc.GetString("health-examinable-verb-disabled"),
				Icon = new SpriteSpecifier.Texture(new ResourcePath("/Textures/Interface/VerbIcons/rejuvenate.svg.192dpi.png", "/"))
			};
			args.Verbs.Add(verb);
		}

		// Token: 0x060016D5 RID: 5845 RVA: 0x000782C8 File Offset: 0x000764C8
		private FormattedMessage CreateMarkup(EntityUid uid, HealthExaminableComponent component, DamageableComponent damage)
		{
			FormattedMessage msg = new FormattedMessage();
			bool first = true;
			foreach (string type in component.ExaminableTypes)
			{
				FixedPoint2 dmg;
				if (damage.Damage.DamageDict.TryGetValue(type, out dmg) && !(dmg == FixedPoint2.Zero))
				{
					FixedPoint2 closest = FixedPoint2.Zero;
					string chosenLocStr = string.Empty;
					foreach (FixedPoint2 threshold in component.Thresholds)
					{
						DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(20, 3);
						defaultInterpolatedStringHandler.AppendLiteral("health-examinable-");
						defaultInterpolatedStringHandler.AppendFormatted(component.LocPrefix);
						defaultInterpolatedStringHandler.AppendLiteral("-");
						defaultInterpolatedStringHandler.AppendFormatted(type);
						defaultInterpolatedStringHandler.AppendLiteral("-");
						defaultInterpolatedStringHandler.AppendFormatted<FixedPoint2>(threshold);
						string str = defaultInterpolatedStringHandler.ToStringAndClear();
						defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(20, 3);
						defaultInterpolatedStringHandler.AppendLiteral("health-examinable-");
						defaultInterpolatedStringHandler.AppendFormatted(component.LocPrefix);
						defaultInterpolatedStringHandler.AppendLiteral("-");
						defaultInterpolatedStringHandler.AppendFormatted(type);
						defaultInterpolatedStringHandler.AppendLiteral("-");
						defaultInterpolatedStringHandler.AppendFormatted<FixedPoint2>(threshold);
						string tempLocStr = Loc.GetString(defaultInterpolatedStringHandler.ToStringAndClear(), new ValueTuple<string, object>[]
						{
							new ValueTuple<string, object>("target", Identity.Entity(uid, this.EntityManager))
						});
						if (!(tempLocStr == str) && dmg > threshold && threshold > closest)
						{
							chosenLocStr = tempLocStr;
							closest = threshold;
						}
					}
					if (!(closest == FixedPoint2.Zero))
					{
						if (!first)
						{
							msg.PushNewline();
						}
						else
						{
							first = false;
						}
						msg.AddMarkup(chosenLocStr);
					}
				}
			}
			if (msg.IsEmpty)
			{
				msg.AddMarkup(Loc.GetString("health-examinable-" + component.LocPrefix + "-none"));
			}
			base.RaiseLocalEvent<HealthBeingExaminedEvent>(uid, new HealthBeingExaminedEvent(msg), true);
			return msg;
		}

		// Token: 0x04000E55 RID: 3669
		[Dependency]
		private readonly ExamineSystemShared _examineSystem;
	}
}
