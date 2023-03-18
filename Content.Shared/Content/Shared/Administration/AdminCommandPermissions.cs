using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using Robust.Shared.Utility;
using YamlDotNet.RepresentationModel;

namespace Content.Shared.Administration
{
	// Token: 0x0200072E RID: 1838
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class AdminCommandPermissions
	{
		// Token: 0x06001640 RID: 5696 RVA: 0x00048C58 File Offset: 0x00046E58
		public void LoadPermissionsFromStream(Stream fs)
		{
			using (StreamReader reader = new StreamReader(fs, EncodingHelpers.UTF8))
			{
				YamlStream yamlStream = new YamlStream();
				yamlStream.Load(reader);
				foreach (YamlNode yamlNode in ((YamlSequenceNode)yamlStream.Documents[0].RootNode))
				{
					YamlMappingNode yamlMappingNode = (YamlMappingNode)yamlNode;
					IEnumerable<string> commands = from p in YamlHelpers.GetNode<YamlSequenceNode>(yamlMappingNode, "Commands")
					select YamlHelpers.AsString(p);
					YamlNode flagsNode;
					if (YamlHelpers.TryGetNode(yamlMappingNode, "Flags", ref flagsNode))
					{
						AdminFlags flags = AdminFlagsHelper.NamesToFlags(YamlHelpers.AsString(flagsNode).Split(",", StringSplitOptions.RemoveEmptyEntries));
						using (IEnumerator<string> enumerator2 = commands.GetEnumerator())
						{
							while (enumerator2.MoveNext())
							{
								string cmd = enumerator2.Current;
								AdminFlags[] exFlags;
								if (!this.AdminCommands.TryGetValue(cmd, out exFlags))
								{
									this.AdminCommands.Add(cmd, new AdminFlags[]
									{
										flags
									});
								}
								else
								{
									AdminFlags[] newArr = new AdminFlags[exFlags.Length + 1];
									exFlags.CopyTo(newArr, 0);
									AdminFlags[] array = exFlags;
									array[array.Length - 1] = flags;
									this.AdminCommands[cmd] = newArr;
								}
							}
							continue;
						}
					}
					this.AnyCommands.UnionWith(commands);
				}
			}
		}

		// Token: 0x06001641 RID: 5697 RVA: 0x00048E00 File Offset: 0x00047000
		public bool CanCommand(string cmdName, [Nullable(2)] AdminData admin)
		{
			if (this.AnyCommands.Contains(cmdName))
			{
				return true;
			}
			AdminFlags[] flagsReq;
			if (!this.AdminCommands.TryGetValue(cmdName, out flagsReq))
			{
				return false;
			}
			if (admin == null)
			{
				return false;
			}
			foreach (AdminFlags flagReq in flagsReq)
			{
				if (admin.HasFlag(flagReq))
				{
					return true;
				}
			}
			return false;
		}

		// Token: 0x0400168D RID: 5773
		public readonly HashSet<string> AnyCommands = new HashSet<string>();

		// Token: 0x0400168E RID: 5774
		public readonly Dictionary<string, AdminFlags[]> AdminCommands = new Dictionary<string, AdminFlags[]>();
	}
}
