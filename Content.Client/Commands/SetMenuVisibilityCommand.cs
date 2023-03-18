using System;
using System.Runtime.CompilerServices;
using Content.Client.Verbs;
using Robust.Shared.Console;
using Robust.Shared.GameObjects;

namespace Content.Client.Commands
{
	// Token: 0x020003AD RID: 941
	[NullableContext(1)]
	[Nullable(0)]
	internal sealed class SetMenuVisibilityCommand : IConsoleCommand
	{
		// Token: 0x170004C8 RID: 1224
		// (get) Token: 0x06001764 RID: 5988 RVA: 0x00086AA8 File Offset: 0x00084CA8
		public string Command
		{
			get
			{
				return "menuvis";
			}
		}

		// Token: 0x170004C9 RID: 1225
		// (get) Token: 0x06001765 RID: 5989 RVA: 0x00086AAF File Offset: 0x00084CAF
		public string Description
		{
			get
			{
				return "Set restrictions about what entities to show on the entity context menu.";
			}
		}

		// Token: 0x170004CA RID: 1226
		// (get) Token: 0x06001766 RID: 5990 RVA: 0x00086AB6 File Offset: 0x00084CB6
		public string Help
		{
			get
			{
				return "Usage: " + this.Command + " [NoFoV] [InContainer] [Invisible] [All]";
			}
		}

		// Token: 0x06001767 RID: 5991 RVA: 0x00086AD0 File Offset: 0x00084CD0
		public void Execute(IConsoleShell shell, string argStr, string[] args)
		{
			MenuVisibility visibility;
			if (!this.TryParseArguments(shell, args, out visibility))
			{
				return;
			}
			EntitySystem.Get<VerbSystem>().Visibility = visibility;
		}

		// Token: 0x06001768 RID: 5992 RVA: 0x00086AF8 File Offset: 0x00084CF8
		private bool TryParseArguments(IConsoleShell shell, string[] args, out MenuVisibility visibility)
		{
			visibility = MenuVisibility.Default;
			foreach (string text in args)
			{
				string a = text.ToLower();
				if (!(a == "nofov"))
				{
					if (!(a == "incontainer"))
					{
						if (!(a == "invisible"))
						{
							if (!(a == "all"))
							{
								shell.WriteLine("Unknown visibility argument '" + text + "'. Only 'NoFov', 'InContainer', 'Invisible' or 'All' are valid. Provide no arguments to set to default.");
								return false;
							}
							visibility |= MenuVisibility.All;
						}
						else
						{
							visibility |= MenuVisibility.Invisible;
						}
					}
					else
					{
						visibility |= MenuVisibility.InContainer;
					}
				}
				else
				{
					visibility |= MenuVisibility.NoFov;
				}
			}
			return true;
		}

		// Token: 0x04000BFD RID: 3069
		public const string CommandName = "menuvis";
	}
}
