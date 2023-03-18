using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Runtime.CompilerServices;
using Content.Server.Administration;
using Content.Shared.Administration;
using Content.Shared.Damage;
using Content.Shared.Damage.Prototypes;
using Robust.Shared.Console;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Localization;
using Robust.Shared.Players;
using Robust.Shared.Prototypes;

namespace Content.Server.Damage.Commands
{
	// Token: 0x020005D2 RID: 1490
	[NullableContext(1)]
	[Nullable(0)]
	[AdminCommand(AdminFlags.Fun)]
	internal sealed class DamageCommand : IConsoleCommand
	{
		// Token: 0x170004B1 RID: 1201
		// (get) Token: 0x06001FB4 RID: 8116 RVA: 0x000A5E43 File Offset: 0x000A4043
		public string Command
		{
			get
			{
				return "damage";
			}
		}

		// Token: 0x170004B2 RID: 1202
		// (get) Token: 0x06001FB5 RID: 8117 RVA: 0x000A5E4A File Offset: 0x000A404A
		public string Description
		{
			get
			{
				return Loc.GetString("damage-command-description");
			}
		}

		// Token: 0x170004B3 RID: 1203
		// (get) Token: 0x06001FB6 RID: 8118 RVA: 0x000A5E56 File Offset: 0x000A4056
		public string Help
		{
			get
			{
				return Loc.GetString("damage-command-help", new ValueTuple<string, object>[]
				{
					new ValueTuple<string, object>("command", this.Command)
				});
			}
		}

		// Token: 0x06001FB7 RID: 8119 RVA: 0x000A5E7F File Offset: 0x000A407F
		public DamageCommand()
		{
			this._prototypeManager = IoCManager.Resolve<IPrototypeManager>();
		}

		// Token: 0x06001FB8 RID: 8120 RVA: 0x000A5E94 File Offset: 0x000A4094
		public CompletionResult GetCompletion(IConsoleShell shell, string[] args)
		{
			if (args.Length == 1)
			{
				IEnumerable<CompletionOption> first = from p in this._prototypeManager.EnumeratePrototypes<DamageTypePrototype>()
				select new CompletionOption(p.ID, null, 0);
				IEnumerable<CompletionOption> groups = from p in this._prototypeManager.EnumeratePrototypes<DamageGroupPrototype>()
				select new CompletionOption(p.ID, null, 0);
				return CompletionResult.FromHintOptions(from p in first.Concat(groups)
				orderby p.Value
				select p, Loc.GetString("damage-command-arg-type"));
			}
			if (args.Length == 2)
			{
				return CompletionResult.FromHint(Loc.GetString("damage-command-arg-quantity"));
			}
			if (args.Length == 3)
			{
				return CompletionResult.FromHint("<bool>");
			}
			if (args.Length == 4)
			{
				return CompletionResult.FromHint(Loc.GetString("damage-command-arg-target"));
			}
			return CompletionResult.Empty;
		}

		// Token: 0x06001FB9 RID: 8121 RVA: 0x000A5F88 File Offset: 0x000A4188
		private bool TryParseDamageArgs(IConsoleShell shell, EntityUid target, string[] args, [Nullable(2)] [NotNullWhen(true)] out DamageCommand.Damage func)
		{
			DamageCommand.<>c__DisplayClass10_0 CS$<>8__locals1 = new DamageCommand.<>c__DisplayClass10_0();
			if (!float.TryParse(args[1], out CS$<>8__locals1.amount))
			{
				shell.WriteLine(Loc.GetString("damage-command-error-quantity", new ValueTuple<string, object>[]
				{
					new ValueTuple<string, object>("arg", args[1])
				}));
				func = null;
				return false;
			}
			if (this._prototypeManager.TryIndex<DamageGroupPrototype>(args[0], ref CS$<>8__locals1.damageGroup))
			{
				func = delegate(EntityUid entity, bool ignoreResistances)
				{
					DamageSpecifier damage = new DamageSpecifier(CS$<>8__locals1.damageGroup, CS$<>8__locals1.amount);
					EntitySystem.Get<DamageableSystem>().TryChangeDamage(new EntityUid?(entity), damage, ignoreResistances, true, null, null);
				};
				return true;
			}
			DamageTypePrototype damageType;
			if (this._prototypeManager.TryIndex<DamageTypePrototype>(args[0], ref damageType))
			{
				func = delegate(EntityUid entity, bool ignoreResistances)
				{
					DamageSpecifier damage = new DamageSpecifier(damageType, CS$<>8__locals1.amount);
					EntitySystem.Get<DamageableSystem>().TryChangeDamage(new EntityUid?(entity), damage, ignoreResistances, true, null, null);
				};
				return true;
			}
			shell.WriteLine(Loc.GetString("damage-command-error-type", new ValueTuple<string, object>[]
			{
				new ValueTuple<string, object>("arg", args[0])
			}));
			func = null;
			return false;
		}

		// Token: 0x06001FBA RID: 8122 RVA: 0x000A6068 File Offset: 0x000A4268
		public void Execute(IConsoleShell shell, string argStr, string[] args)
		{
			if (args.Length < 2 || args.Length > 4)
			{
				shell.WriteLine(Loc.GetString("damage-command-error-args"));
				return;
			}
			IEntityManager entMan = IoCManager.Resolve<IEntityManager>();
			EntityUid target;
			if (args.Length != 4)
			{
				ICommonSession player = shell.Player;
				EntityUid? entityUid = (player != null) ? player.AttachedEntity : null;
				if (entityUid != null)
				{
					EntityUid playerEntity = entityUid.GetValueOrDefault();
					if (playerEntity.Valid)
					{
						target = playerEntity;
						goto IL_BC;
					}
				}
				shell.WriteLine(Loc.GetString("damage-command-error-player"));
				return;
			}
			if (!EntityUid.TryParse(args[3], ref target) || !entMan.EntityExists(target))
			{
				shell.WriteLine(Loc.GetString("damage-command-error-euid", new ValueTuple<string, object>[]
				{
					new ValueTuple<string, object>("arg", args[3])
				}));
				return;
			}
			IL_BC:
			DamageCommand.Damage damageFunc;
			if (!this.TryParseDamageArgs(shell, target, args, out damageFunc))
			{
				return;
			}
			bool ignoreResistances;
			if (args.Length == 3)
			{
				if (!bool.TryParse(args[2], out ignoreResistances))
				{
					shell.WriteLine(Loc.GetString("damage-command-error-bool", new ValueTuple<string, object>[]
					{
						new ValueTuple<string, object>("arg", args[2])
					}));
					return;
				}
			}
			else
			{
				ignoreResistances = false;
			}
			damageFunc(target, ignoreResistances);
		}

		// Token: 0x040013B3 RID: 5043
		private readonly IPrototypeManager _prototypeManager;

		// Token: 0x02000AB1 RID: 2737
		// (Invoke) Token: 0x06003596 RID: 13718
		[NullableContext(0)]
		private delegate void Damage(EntityUid entity, bool ignoreResistances);
	}
}
