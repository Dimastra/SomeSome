using System;
using System.ComponentModel;
using Robust.Client.UserInterface.XAML;

namespace CompiledRobustXaml
{
	// Token: 0x02000504 RID: 1284
	internal class XamlIlContext
	{
		// Token: 0x02000505 RID: 1285
		public class Context<TTarget> : ITestRootObjectProvider, ITypeDescriptorContext, ITestProvideValueTarget, ITestUriContext, IServiceProvider
		{
			// Token: 0x17000714 RID: 1812
			// (get) Token: 0x06002096 RID: 8342 RVA: 0x000BCE84 File Offset: 0x000BB084
			virtual object RootObject
			{
				get
				{
					if (this.RootObject != null)
					{
						return this.RootObject;
					}
					if (this._sp != null)
					{
						ITestRootObjectProvider testRootObjectProvider = (ITestRootObjectProvider)this._sp.GetService(typeof(ITestRootObjectProvider));
						if (testRootObjectProvider != null)
						{
							return testRootObjectProvider.RootObject;
						}
					}
					return null;
				}
			}

			// Token: 0x17000715 RID: 1813
			// (get) Token: 0x06002097 RID: 8343 RVA: 0x000BCEEC File Offset: 0x000BB0EC
			virtual IContainer Container
			{
				get
				{
					return null;
				}
			}

			// Token: 0x17000716 RID: 1814
			// (get) Token: 0x06002098 RID: 8344 RVA: 0x000BCEFC File Offset: 0x000BB0FC
			virtual object Instance
			{
				get
				{
					return null;
				}
			}

			// Token: 0x17000717 RID: 1815
			// (get) Token: 0x06002099 RID: 8345 RVA: 0x000BCF0C File Offset: 0x000BB10C
			virtual PropertyDescriptor PropertyDescriptor
			{
				get
				{
					return null;
				}
			}

			// Token: 0x0600209A RID: 8346 RVA: 0x000BCF1C File Offset: 0x000BB11C
			virtual bool OnComponentChanging()
			{
				throw new NotSupportedException();
			}

			// Token: 0x0600209B RID: 8347 RVA: 0x000BCF30 File Offset: 0x000BB130
			virtual void OnComponentChanged()
			{
				throw new NotSupportedException();
			}

			// Token: 0x17000718 RID: 1816
			// (get) Token: 0x0600209C RID: 8348 RVA: 0x000BCF44 File Offset: 0x000BB144
			virtual object TargetObject
			{
				get
				{
					return this.ProvideTargetObject;
				}
			}

			// Token: 0x17000719 RID: 1817
			// (get) Token: 0x0600209D RID: 8349 RVA: 0x000BCF58 File Offset: 0x000BB158
			virtual object TargetProperty
			{
				get
				{
					return this.ProvideTargetProperty;
				}
			}

			// Token: 0x1700071A RID: 1818
			// (get) Token: 0x0600209E RID: 8350 RVA: 0x000BCF6C File Offset: 0x000BB16C
			// (set) Token: 0x0600209F RID: 8351 RVA: 0x000BCF80 File Offset: 0x000BB180
			public virtual Uri BaseUri
			{
				get
				{
					return this._baseUri;
				}
				set
				{
					this._baseUri = value;
				}
			}

			// Token: 0x060020A0 RID: 8352 RVA: 0x000BCF98 File Offset: 0x000BB198
			public virtual object GetService(Type A_1)
			{
				if (typeof(ITestRootObjectProvider).Equals(A_1))
				{
					return this;
				}
				if (typeof(ITypeDescriptorContext).Equals(A_1))
				{
					return this;
				}
				if (typeof(ITestProvideValueTarget).Equals(A_1))
				{
					return this;
				}
				if (typeof(ITestUriContext).Equals(A_1))
				{
					return this;
				}
				if (this._staticProviders != null)
				{
					for (int i = 0; i < this._staticProviders.Length; i++)
					{
						object obj = this._staticProviders[i];
						if (A_1.IsAssignableFrom(obj.GetType()))
						{
							return obj;
						}
					}
				}
				if (this._sp != null)
				{
					return this._sp.GetService(A_1);
				}
				return null;
			}

			// Token: 0x060020A1 RID: 8353 RVA: 0x000BD078 File Offset: 0x000BB278
			public Context(IServiceProvider A_1, object[] A_2, string A_3)
			{
				this._sp = A_1;
				this._staticProviders = A_2;
				if (A_3 != null)
				{
					this._baseUri = new Uri(A_3);
				}
				this.RobustNameScope = new NameScope();
			}

			// Token: 0x04000F81 RID: 3969
			public TTarget RootObject;

			// Token: 0x04000F82 RID: 3970
			public object IntermediateRoot;

			// Token: 0x04000F83 RID: 3971
			IServiceProvider _sp;

			// Token: 0x04000F84 RID: 3972
			object[] _staticProviders;

			// Token: 0x04000F85 RID: 3973
			public object ProvideTargetObject;

			// Token: 0x04000F86 RID: 3974
			public object ProvideTargetProperty;

			// Token: 0x04000F87 RID: 3975
			Uri _baseUri;

			// Token: 0x04000F88 RID: 3976
			public NameScope RobustNameScope;
		}
	}
}
