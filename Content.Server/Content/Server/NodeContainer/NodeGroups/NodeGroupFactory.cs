using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.CompilerServices;
using Robust.Shared.IoC;
using Robust.Shared.Reflection;

namespace Content.Server.NodeContainer.NodeGroups
{
	// Token: 0x02000383 RID: 899
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class NodeGroupFactory : INodeGroupFactory
	{
		// Token: 0x0600126C RID: 4716 RVA: 0x0005F334 File Offset: 0x0005D534
		public void Initialize()
		{
			foreach (Type nodeGroupType in this._reflectionManager.GetAllChildren<INodeGroup>(false))
			{
				NodeGroupAttribute att = nodeGroupType.GetCustomAttribute<NodeGroupAttribute>();
				if (att != null)
				{
					foreach (NodeGroupID groupID in att.NodeGroupIDs)
					{
						this._groupTypes.Add(groupID, nodeGroupType);
					}
				}
			}
		}

		// Token: 0x0600126D RID: 4717 RVA: 0x0005F3B8 File Offset: 0x0005D5B8
		public INodeGroup MakeNodeGroup(NodeGroupID id)
		{
			Type type;
			if (!this._groupTypes.TryGetValue(id, out type))
			{
				DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(44, 2);
				defaultInterpolatedStringHandler.AppendFormatted<NodeGroupID>(id);
				defaultInterpolatedStringHandler.AppendLiteral(" did not have an associated ");
				defaultInterpolatedStringHandler.AppendFormatted("INodeGroup");
				defaultInterpolatedStringHandler.AppendLiteral(" implementation.");
				throw new ArgumentException(defaultInterpolatedStringHandler.ToStringAndClear());
			}
			INodeGroup nodeGroup = DynamicTypeFactoryExt.CreateInstance<INodeGroup>(this._typeFactory, type);
			nodeGroup.Create(id);
			return nodeGroup;
		}

		// Token: 0x04000B4A RID: 2890
		[Dependency]
		private readonly IReflectionManager _reflectionManager;

		// Token: 0x04000B4B RID: 2891
		[Dependency]
		private readonly IDynamicTypeFactory _typeFactory;

		// Token: 0x04000B4C RID: 2892
		private readonly Dictionary<NodeGroupID, Type> _groupTypes = new Dictionary<NodeGroupID, Type>();
	}
}
