using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Content.Server.NodeContainer;
using Content.Server.NodeContainer.Nodes;
using Content.Server.Power.Components;
using Content.Server.Power.NodeGroups;
using Content.Server.Tools;
using Content.Shared.Examine;
using Content.Shared.Interaction;
using Content.Shared.Verbs;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Localization;
using Robust.Shared.Utility;

namespace Content.Server.Power.EntitySystems
{
	// Token: 0x0200028B RID: 651
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class CableMultitoolSystem : EntitySystem
	{
		// Token: 0x06000D12 RID: 3346 RVA: 0x0004444E File Offset: 0x0004264E
		public override void Initialize()
		{
			base.Initialize();
			base.SubscribeLocalEvent<CableComponent, GetVerbsEvent<ExamineVerb>>(new ComponentEventHandler<CableComponent, GetVerbsEvent<ExamineVerb>>(this.OnGetExamineVerbs), null, null);
			base.SubscribeLocalEvent<CableComponent, AfterInteractUsingEvent>(new ComponentEventHandler<CableComponent, AfterInteractUsingEvent>(this.OnAfterInteractUsing), null, null);
		}

		// Token: 0x06000D13 RID: 3347 RVA: 0x00044480 File Offset: 0x00042680
		private void OnAfterInteractUsing(EntityUid uid, CableComponent component, AfterInteractUsingEvent args)
		{
			if (args.Handled || args.Target == null || !args.CanReach || !this._toolSystem.HasQuality(args.Used, "Pulsing", null))
			{
				return;
			}
			FormattedMessage markup = FormattedMessage.FromMarkup(this.GenerateCableMarkup(uid, null));
			this._examineSystem.SendExamineTooltip(args.User, uid, markup, false, false);
			args.Handled = true;
		}

		// Token: 0x06000D14 RID: 3348 RVA: 0x000444F4 File Offset: 0x000426F4
		private void OnGetExamineVerbs(EntityUid uid, CableComponent component, GetVerbsEvent<ExamineVerb> args)
		{
			if (this._examineSystem.IsInDetailsRange(args.User, args.Target))
			{
				EntityUid? held = args.Using;
				bool enabled = held != null && this._toolSystem.HasQuality(held.Value, "Pulsing", null);
				ExamineVerb verb = new ExamineVerb
				{
					Disabled = !enabled,
					Message = Loc.GetString("cable-multitool-system-verb-tooltip"),
					Text = Loc.GetString("cable-multitool-system-verb-name"),
					Category = VerbCategory.Examine,
					Icon = new SpriteSpecifier.Texture(new ResourcePath("/Textures/Interface/VerbIcons/zap.svg.192dpi.png", "/")),
					Act = delegate()
					{
						FormattedMessage markup = FormattedMessage.FromMarkup(this.GenerateCableMarkup(uid, null));
						this._examineSystem.SendExamineTooltip(args.User, uid, markup, false, false);
					}
				};
				args.Verbs.Add(verb);
			}
		}

		// Token: 0x06000D15 RID: 3349 RVA: 0x000445EC File Offset: 0x000427EC
		private string GenerateCableMarkup(EntityUid uid, [Nullable(2)] NodeContainerComponent nodeContainer = null)
		{
			if (!base.Resolve<NodeContainerComponent>(uid, ref nodeContainer, true))
			{
				return Loc.GetString("cable-multitool-system-internal-error-missing-component");
			}
			foreach (KeyValuePair<string, Node> node in nodeContainer.Nodes)
			{
				if (node.Value.NodeGroup is IBasePowerNet)
				{
					IBasePowerNet p = (IBasePowerNet)node.Value.NodeGroup;
					NetworkPowerStatistics ps = this._pnSystem.GetNetworkStatistics(p.NetworkNode);
					float storageRatio = ps.InStorageCurrent / Math.Max(ps.InStorageMax, 1f);
					float outStorageRatio = ps.OutStorageCurrent / Math.Max(ps.OutStorageMax, 1f);
					return Loc.GetString("cable-multitool-system-statistics", new ValueTuple<string, object>[]
					{
						new ValueTuple<string, object>("supplyc", ps.SupplyCurrent),
						new ValueTuple<string, object>("supplyb", ps.SupplyBatteries),
						new ValueTuple<string, object>("supplym", ps.SupplyTheoretical),
						new ValueTuple<string, object>("consumption", ps.Consumption),
						new ValueTuple<string, object>("storagec", ps.InStorageCurrent),
						new ValueTuple<string, object>("storager", storageRatio),
						new ValueTuple<string, object>("storagem", ps.InStorageMax),
						new ValueTuple<string, object>("storageoc", ps.OutStorageCurrent),
						new ValueTuple<string, object>("storageor", outStorageRatio),
						new ValueTuple<string, object>("storageom", ps.OutStorageMax)
					});
				}
			}
			return Loc.GetString("cable-multitool-system-internal-error-no-power-node");
		}

		// Token: 0x040007E4 RID: 2020
		[Dependency]
		private readonly ToolSystem _toolSystem;

		// Token: 0x040007E5 RID: 2021
		[Dependency]
		private readonly PowerNetSystem _pnSystem;

		// Token: 0x040007E6 RID: 2022
		[Dependency]
		private readonly ExamineSystemShared _examineSystem;
	}
}
