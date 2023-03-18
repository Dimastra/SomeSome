using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Runtime.CompilerServices;
using Content.Server.NodeContainer;
using Content.Server.NodeContainer.NodeGroups;
using Content.Server.NodeContainer.Nodes;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.ViewVariables;

namespace Content.Server.Power.Components
{
	// Token: 0x020002A6 RID: 678
	[NullableContext(2)]
	[Nullable(0)]
	public abstract class BaseNetConnectorComponent<TNetType> : Component, IBaseNetConnectorComponent<TNetType>
	{
		// Token: 0x170001E1 RID: 481
		// (get) Token: 0x06000DC3 RID: 3523 RVA: 0x000477A9 File Offset: 0x000459A9
		// (set) Token: 0x06000DC4 RID: 3524 RVA: 0x000477B1 File Offset: 0x000459B1
		[ViewVariables]
		public Voltage Voltage
		{
			get
			{
				return this._voltage;
			}
			set
			{
				this.SetVoltage(value);
			}
		}

		// Token: 0x170001E2 RID: 482
		// (get) Token: 0x06000DC5 RID: 3525 RVA: 0x000477BA File Offset: 0x000459BA
		// (set) Token: 0x06000DC6 RID: 3526 RVA: 0x000477C2 File Offset: 0x000459C2
		[ViewVariables]
		public TNetType Net
		{
			get
			{
				return this._net;
			}
			set
			{
				this.SetNet(value);
			}
		}

		// Token: 0x170001E3 RID: 483
		// (get) Token: 0x06000DC7 RID: 3527 RVA: 0x000477CB File Offset: 0x000459CB
		[ViewVariables]
		private bool _needsNet
		{
			get
			{
				return this._net != null;
			}
		}

		// Token: 0x170001E4 RID: 484
		// (get) Token: 0x06000DC8 RID: 3528 RVA: 0x000477DB File Offset: 0x000459DB
		// (set) Token: 0x06000DC9 RID: 3529 RVA: 0x000477E3 File Offset: 0x000459E3
		[DataField("node", false, 1, false, false, null)]
		public string NodeId { get; set; }

		// Token: 0x06000DCA RID: 3530 RVA: 0x000477EC File Offset: 0x000459EC
		protected override void Initialize()
		{
			base.Initialize();
			if (this._needsNet)
			{
				this.TryFindAndSetNet();
			}
		}

		// Token: 0x06000DCB RID: 3531 RVA: 0x00047802 File Offset: 0x00045A02
		protected override void OnRemove()
		{
			this.ClearNet();
			base.OnRemove();
		}

		// Token: 0x06000DCC RID: 3532 RVA: 0x00047810 File Offset: 0x00045A10
		public void TryFindAndSetNet()
		{
			TNetType net;
			if (this.TryFindNet(out net))
			{
				this.Net = net;
			}
		}

		// Token: 0x06000DCD RID: 3533 RVA: 0x0004782E File Offset: 0x00045A2E
		public void ClearNet()
		{
			if (this._net != null)
			{
				this.RemoveSelfFromNet(this._net);
			}
		}

		// Token: 0x06000DCE RID: 3534
		[NullableContext(1)]
		protected abstract void AddSelfToNet(TNetType net);

		// Token: 0x06000DCF RID: 3535
		[NullableContext(1)]
		protected abstract void RemoveSelfFromNet(TNetType net);

		// Token: 0x06000DD0 RID: 3536 RVA: 0x0004784C File Offset: 0x00045A4C
		private bool TryFindNet([NotNullWhen(true)] out TNetType foundNet)
		{
			NodeContainerComponent container;
			if (this._entMan.TryGetComponent<NodeContainerComponent>(base.Owner, ref container))
			{
				TNetType compatibleNet = (from node in container.Nodes.Values
				where (this.NodeId == null || this.NodeId == node.Name) && node.NodeGroupID == (NodeGroupID)this.Voltage
				select node.NodeGroup).OfType<TNetType>().FirstOrDefault<TNetType>();
				if (compatibleNet != null)
				{
					foundNet = compatibleNet;
					return true;
				}
			}
			foundNet = default(TNetType);
			return false;
		}

		// Token: 0x06000DD1 RID: 3537 RVA: 0x000478D2 File Offset: 0x00045AD2
		private void SetNet(TNetType newNet)
		{
			if (this._net != null)
			{
				this.RemoveSelfFromNet(this._net);
			}
			if (newNet != null)
			{
				this.AddSelfToNet(newNet);
			}
			this._net = newNet;
		}

		// Token: 0x06000DD2 RID: 3538 RVA: 0x00047903 File Offset: 0x00045B03
		private void SetVoltage(Voltage newVoltage)
		{
			this.ClearNet();
			this._voltage = newVoltage;
			this.TryFindAndSetNet();
		}

		// Token: 0x04000823 RID: 2083
		[Nullable(1)]
		[Dependency]
		private readonly IEntityManager _entMan;

		// Token: 0x04000824 RID: 2084
		[DataField("voltage", false, 1, false, false, null)]
		private Voltage _voltage = Voltage.High;

		// Token: 0x04000825 RID: 2085
		private TNetType _net;
	}
}
