using System;
using System.Collections.Generic;
using System.Globalization;
using System.Runtime.CompilerServices;
using Content.Server.Administration;
using Content.Server.Cargo.Systems;
using Content.Server.EUI;
using Content.Shared.Administration;
using Content.Shared.Materials;
using Content.Shared.Research.Prototypes;
using Content.Shared.UserInterface;
using Content.Shared.Weapons.Melee;
using Robust.Server.Player;
using Robust.Shared.Console;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Localization;
using Robust.Shared.Prototypes;

namespace Content.Server.UserInterface
{
	// Token: 0x020000FF RID: 255
	[NullableContext(1)]
	[Nullable(0)]
	[AdminCommand(AdminFlags.Debug)]
	public sealed class StatValuesCommand : IConsoleCommand
	{
		// Token: 0x170000D7 RID: 215
		// (get) Token: 0x060004A7 RID: 1191 RVA: 0x00016238 File Offset: 0x00014438
		public string Command
		{
			get
			{
				return "showvalues";
			}
		}

		// Token: 0x170000D8 RID: 216
		// (get) Token: 0x060004A8 RID: 1192 RVA: 0x0001623F File Offset: 0x0001443F
		public string Description
		{
			get
			{
				return Loc.GetString("stat-values-desc");
			}
		}

		// Token: 0x170000D9 RID: 217
		// (get) Token: 0x060004A9 RID: 1193 RVA: 0x0001624B File Offset: 0x0001444B
		public string Help
		{
			get
			{
				return this.Command + " <cargosell / lathsell / melee>";
			}
		}

		// Token: 0x060004AA RID: 1194 RVA: 0x00016260 File Offset: 0x00014460
		public void Execute(IConsoleShell shell, string argStr, string[] args)
		{
			IPlayerSession pSession = shell.Player as IPlayerSession;
			if (pSession == null)
			{
				shell.WriteError(Loc.GetString("stat-values-server"));
				return;
			}
			if (args.Length != 1)
			{
				shell.WriteError(Loc.GetString("stat-values-args"));
				return;
			}
			string a = args[0];
			StatValuesEuiMessage message;
			if (!(a == "cargosell"))
			{
				if (!(a == "lathesell"))
				{
					if (!(a == "melee"))
					{
						shell.WriteError(Loc.GetString("stat-values-invalid", new ValueTuple<string, object>[]
						{
							new ValueTuple<string, object>("arg", args[0])
						}));
						return;
					}
					message = this.GetMelee();
				}
				else
				{
					message = this.GetLatheMessage();
				}
			}
			else
			{
				message = this.GetCargo();
			}
			EuiManager euiManager = IoCManager.Resolve<EuiManager>();
			StatValuesEui eui = new StatValuesEui();
			euiManager.OpenEui(eui, pSession);
			eui.SendMessage(message);
		}

		// Token: 0x060004AB RID: 1195 RVA: 0x00016330 File Offset: 0x00014530
		private StatValuesEuiMessage GetCargo()
		{
			List<string[]> values = new List<string[]>();
			IEntityManager entityManager = IoCManager.Resolve<IEntityManager>();
			PricingSystem priceSystem = entityManager.System<PricingSystem>();
			EntityQuery<MetaDataComponent> metaQuery = entityManager.GetEntityQuery<MetaDataComponent>();
			HashSet<string> prices = new HashSet<string>(256);
			foreach (EntityUid entity in entityManager.GetEntities())
			{
				MetaDataComponent meta;
				if (metaQuery.TryGetComponent(entity, ref meta))
				{
					EntityPrototype entityPrototype = meta.EntityPrototype;
					string id = (entityPrototype != null) ? entityPrototype.ID : null;
					if (id != null && prices.Add(id))
					{
						double price = priceSystem.GetPrice(entity);
						if (price != 0.0)
						{
							List<string[]> list = values;
							string[] array = new string[2];
							array[0] = id;
							int num = 1;
							DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(0, 1);
							defaultInterpolatedStringHandler.AppendFormatted<double>(price, "0");
							array[num] = defaultInterpolatedStringHandler.ToStringAndClear();
							list.Add(array);
						}
					}
				}
			}
			return new StatValuesEuiMessage
			{
				Title = Loc.GetString("stat-cargo-values"),
				Headers = new List<string>
				{
					Loc.GetString("stat-cargo-id"),
					Loc.GetString("stat-cargo-price")
				},
				Values = values
			};
		}

		// Token: 0x060004AC RID: 1196 RVA: 0x00016468 File Offset: 0x00014668
		private StatValuesEuiMessage GetMelee()
		{
			IComponentFactory compFactory = IoCManager.Resolve<IComponentFactory>();
			IPrototypeManager prototypeManager = IoCManager.Resolve<IPrototypeManager>();
			List<string[]> values = new List<string[]>();
			foreach (EntityPrototype proto in prototypeManager.EnumeratePrototypes<EntityPrototype>())
			{
				EntityPrototype.ComponentRegistryEntry meleeComp;
				if (!proto.Abstract && proto.Components.TryGetValue(compFactory.GetComponentName(typeof(MeleeWeaponComponent)), out meleeComp))
				{
					MeleeWeaponComponent comp = (MeleeWeaponComponent)meleeComp.Component;
					values.Add(new string[]
					{
						proto.ID,
						(comp.Damage.Total * comp.AttackRate).ToString(),
						comp.AttackRate.ToString(CultureInfo.CurrentCulture),
						comp.Damage.Total.ToString(),
						comp.Range.ToString(CultureInfo.CurrentCulture)
					});
				}
			}
			return new StatValuesEuiMessage
			{
				Title = "Cargo sell prices",
				Headers = new List<string>
				{
					"ID",
					"Price"
				},
				Values = values
			};
		}

		// Token: 0x060004AD RID: 1197 RVA: 0x000165BC File Offset: 0x000147BC
		private StatValuesEuiMessage GetLatheMessage()
		{
			List<string[]> values = new List<string[]>();
			IPrototypeManager protoManager = IoCManager.Resolve<IPrototypeManager>();
			IComponentFactory factory = IoCManager.Resolve<IComponentFactory>();
			PricingSystem priceSystem = IoCManager.Resolve<IEntitySystemManager>().GetEntitySystem<PricingSystem>();
			foreach (LatheRecipePrototype proto in protoManager.EnumeratePrototypes<LatheRecipePrototype>())
			{
				double cost = 0.0;
				foreach (KeyValuePair<string, int> keyValuePair in proto.RequiredMaterials)
				{
					string text;
					int num;
					keyValuePair.Deconstruct(out text, out num);
					string material = text;
					int count = num;
					double materialPrice = protoManager.Index<MaterialPrototype>(material).Price;
					cost += materialPrice * (double)count;
				}
				double sell = priceSystem.GetEstimatedPrice(protoManager.Index<EntityPrototype>(proto.Result), factory);
				List<string[]> list = values;
				string[] array = new string[3];
				array[0] = proto.ID;
				int num2 = 1;
				DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(0, 1);
				defaultInterpolatedStringHandler.AppendFormatted<double>(cost, "0");
				array[num2] = defaultInterpolatedStringHandler.ToStringAndClear();
				int num3 = 2;
				defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(0, 1);
				defaultInterpolatedStringHandler.AppendFormatted<double>(sell, "0");
				array[num3] = defaultInterpolatedStringHandler.ToStringAndClear();
				list.Add(array);
			}
			return new StatValuesEuiMessage
			{
				Title = Loc.GetString("stat-lathe-values"),
				Headers = new List<string>
				{
					Loc.GetString("stat-lathe-id"),
					Loc.GetString("stat-lathe-cost"),
					Loc.GetString("stat-lathe-sell")
				},
				Values = values
			};
		}
	}
}
