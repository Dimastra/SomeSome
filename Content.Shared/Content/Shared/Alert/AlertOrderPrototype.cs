using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.ViewVariables;

namespace Content.Shared.Alert
{
	// Token: 0x0200071C RID: 1820
	[NullableContext(1)]
	[Nullable(0)]
	[Prototype("alertOrder", 1)]
	[DataDefinition]
	public sealed class AlertOrderPrototype : IPrototype, IComparer<AlertPrototype>
	{
		// Token: 0x17000494 RID: 1172
		// (get) Token: 0x060015FE RID: 5630 RVA: 0x00047EC1 File Offset: 0x000460C1
		[ViewVariables]
		[IdDataField(1, null)]
		public string ID { get; }

		// Token: 0x17000495 RID: 1173
		// (get) Token: 0x060015FF RID: 5631 RVA: 0x00047ECC File Offset: 0x000460CC
		// (set) Token: 0x06001600 RID: 5632 RVA: 0x00047FD8 File Offset: 0x000461D8
		[TupleElementNames(new string[]
		{
			"type",
			"alert"
		})]
		[Nullable(new byte[]
		{
			1,
			0,
			1,
			1
		})]
		[DataField("order", false, 1, false, false, null)]
		private List<ValueTuple<string, string>> Order
		{
			[return: TupleElementNames(new string[]
			{
				"type",
				"alert"
			})]
			[return: Nullable(new byte[]
			{
				1,
				0,
				1,
				1
			})]
			get
			{
				List<ValueTuple<string, string>> res = new List<ValueTuple<string, string>>(this._typeToIdx.Count + this._categoryToIdx.Count);
				foreach (KeyValuePair<AlertType, int> keyValuePair in this._typeToIdx)
				{
					AlertType alertType;
					int num;
					keyValuePair.Deconstruct(out alertType, out num);
					AlertType type = alertType;
					int id = num;
					res.Insert(id, new ValueTuple<string, string>("alertType", type.ToString()));
				}
				foreach (KeyValuePair<AlertCategory, int> keyValuePair2 in this._categoryToIdx)
				{
					int num;
					AlertCategory alertCategory;
					keyValuePair2.Deconstruct(out alertCategory, out num);
					AlertCategory category = alertCategory;
					int id2 = num;
					res.Insert(id2, new ValueTuple<string, string>("category", category.ToString()));
				}
				return res;
			}
			[param: TupleElementNames(new string[]
			{
				"type",
				"alert"
			})]
			[param: Nullable(new byte[]
			{
				1,
				0,
				1,
				1
			})]
			set
			{
				int i = 0;
				foreach (ValueTuple<string, string> valueTuple in value)
				{
					string type = valueTuple.Item1;
					string alert = valueTuple.Item2;
					if (!(type == "alertType"))
					{
						if (!(type == "category"))
						{
							throw new ArgumentException();
						}
						this._categoryToIdx[Enum.Parse<AlertCategory>(alert)] = i++;
					}
					else
					{
						this._typeToIdx[Enum.Parse<AlertType>(alert)] = i++;
					}
				}
			}
		}

		// Token: 0x06001601 RID: 5633 RVA: 0x00048080 File Offset: 0x00046280
		private int GetOrderIndex(AlertPrototype alert)
		{
			int idx;
			if (this._typeToIdx.TryGetValue(alert.AlertType, out idx))
			{
				return idx;
			}
			if (alert.Category != null && this._categoryToIdx.TryGetValue(alert.Category.Value, out idx))
			{
				return idx;
			}
			return -1;
		}

		// Token: 0x06001602 RID: 5634 RVA: 0x000480D4 File Offset: 0x000462D4
		[NullableContext(2)]
		public int Compare(AlertPrototype x, AlertPrototype y)
		{
			if (x == null && y == null)
			{
				return 0;
			}
			if (x == null)
			{
				return 1;
			}
			if (y == null)
			{
				return -1;
			}
			int idx = this.GetOrderIndex(x);
			int idy = this.GetOrderIndex(y);
			if (idx == -1 && idy == -1)
			{
				return (int)(x.AlertType - y.AlertType);
			}
			if (idx == -1)
			{
				return 1;
			}
			if (idy == -1)
			{
				return -1;
			}
			int result = idx - idy;
			if (result == 0)
			{
				return (int)(x.AlertType - y.AlertType);
			}
			return result;
		}

		// Token: 0x04001645 RID: 5701
		private readonly Dictionary<AlertType, int> _typeToIdx = new Dictionary<AlertType, int>();

		// Token: 0x04001646 RID: 5702
		private readonly Dictionary<AlertCategory, int> _categoryToIdx = new Dictionary<AlertCategory, int>();
	}
}
