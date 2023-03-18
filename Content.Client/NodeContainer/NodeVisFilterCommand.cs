using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Robust.Shared.Console;
using Robust.Shared.GameObjects;

namespace Content.Client.NodeContainer
{
	// Token: 0x02000219 RID: 537
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class NodeVisFilterCommand : IConsoleCommand
	{
		// Token: 0x17000300 RID: 768
		// (get) Token: 0x06000E09 RID: 3593 RVA: 0x00054CFA File Offset: 0x00052EFA
		public string Command
		{
			get
			{
				return "nodevisfilter";
			}
		}

		// Token: 0x17000301 RID: 769
		// (get) Token: 0x06000E0A RID: 3594 RVA: 0x00054D01 File Offset: 0x00052F01
		public string Description
		{
			get
			{
				return "Toggles showing a specific group on nodevis";
			}
		}

		// Token: 0x17000302 RID: 770
		// (get) Token: 0x06000E0B RID: 3595 RVA: 0x00054D08 File Offset: 0x00052F08
		public string Help
		{
			get
			{
				return "Usage: nodevis [filter]\nOmit filter to list currently masked-off";
			}
		}

		// Token: 0x06000E0C RID: 3596 RVA: 0x00054D10 File Offset: 0x00052F10
		public void Execute(IConsoleShell shell, string argStr, string[] args)
		{
			NodeGroupSystem nodeGroupSystem = EntitySystem.Get<NodeGroupSystem>();
			if (args.Length == 0)
			{
				using (HashSet<string>.Enumerator enumerator = nodeGroupSystem.Filtered.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						string text = enumerator.Current;
						shell.WriteLine(text);
					}
					return;
				}
			}
			string item = args[0];
			if (!nodeGroupSystem.Filtered.Add(item))
			{
				nodeGroupSystem.Filtered.Remove(item);
			}
		}
	}
}
