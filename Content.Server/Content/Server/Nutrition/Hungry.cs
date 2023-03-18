using System;
using System.Runtime.CompilerServices;
using Content.Server.Administration;
using Content.Server.Nutrition.Components;
using Content.Shared.Administration;
using Content.Shared.Nutrition.Components;
using Robust.Server.Player;
using Robust.Shared.Console;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;

namespace Content.Server.Nutrition
{
	// Token: 0x02000308 RID: 776
	[NullableContext(1)]
	[Nullable(0)]
	[AdminCommand(AdminFlags.Debug)]
	public sealed class Hungry : IConsoleCommand
	{
		// Token: 0x17000253 RID: 595
		// (get) Token: 0x06000FF8 RID: 4088 RVA: 0x000513D2 File Offset: 0x0004F5D2
		public string Command
		{
			get
			{
				return "hungry";
			}
		}

		// Token: 0x17000254 RID: 596
		// (get) Token: 0x06000FF9 RID: 4089 RVA: 0x000513D9 File Offset: 0x0004F5D9
		public string Description
		{
			get
			{
				return "Makes you hungry.";
			}
		}

		// Token: 0x17000255 RID: 597
		// (get) Token: 0x06000FFA RID: 4090 RVA: 0x000513E0 File Offset: 0x0004F5E0
		public string Help
		{
			get
			{
				return this.Command ?? "";
			}
		}

		// Token: 0x06000FFB RID: 4091 RVA: 0x000513F4 File Offset: 0x0004F5F4
		public void Execute(IConsoleShell shell, string argStr, string[] args)
		{
			IPlayerSession player = shell.Player as IPlayerSession;
			if (player == null)
			{
				shell.WriteLine("You cannot use this command unless you are a player.");
				return;
			}
			EntityUid? attachedEntity = player.AttachedEntity;
			if (attachedEntity != null)
			{
				EntityUid playerEntity = attachedEntity.GetValueOrDefault();
				if (playerEntity.Valid)
				{
					HungerComponent hunger;
					if (!this._entities.TryGetComponent<HungerComponent>(playerEntity, ref hunger))
					{
						shell.WriteLine("Your entity does not have a HungerComponent component.");
						return;
					}
					float hungryThreshold = hunger.HungerThresholds[HungerThreshold.Starving];
					hunger.CurrentHunger = hungryThreshold;
					return;
				}
			}
			shell.WriteLine("You cannot use this command without an entity.");
		}

		// Token: 0x04000934 RID: 2356
		[Dependency]
		private readonly IEntityManager _entities;
	}
}
