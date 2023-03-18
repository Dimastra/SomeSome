using System;
using System.Runtime.CompilerServices;
using Content.Shared.Verbs;
using Robust.Client.Console;
using Robust.Shared.Console;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Utility;

namespace Content.Client.Administration.Systems
{
	// Token: 0x020004DE RID: 1246
	[NullableContext(1)]
	[Nullable(0)]
	internal sealed class AdminVerbSystem : EntitySystem
	{
		// Token: 0x06001FC9 RID: 8137 RVA: 0x000B9737 File Offset: 0x000B7937
		public override void Initialize()
		{
			base.SubscribeLocalEvent<GetVerbsEvent<Verb>>(new EntityEventHandler<GetVerbsEvent<Verb>>(this.AddAdminVerbs), null, null);
		}

		// Token: 0x06001FCA RID: 8138 RVA: 0x000B9750 File Offset: 0x000B7950
		private void AddAdminVerbs(GetVerbsEvent<Verb> args)
		{
			if (this._clientConGroupController.CanAdminMenu())
			{
				Verb verb = new Verb();
				verb.Category = VerbCategory.Debug;
				verb.Text = "View Variables";
				verb.Icon = new SpriteSpecifier.Texture(new ResourcePath("/Textures/Interface/VerbIcons/vv.svg.192dpi.png", "/"));
				verb.Act = delegate()
				{
					IConsoleHost clientConsoleHost = this._clientConsoleHost;
					DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(3, 1);
					defaultInterpolatedStringHandler.AppendLiteral("vv ");
					defaultInterpolatedStringHandler.AppendFormatted<EntityUid>(args.Target);
					clientConsoleHost.ExecuteCommand(defaultInterpolatedStringHandler.ToStringAndClear());
				};
				verb.ClientExclusive = true;
				args.Verbs.Add(verb);
			}
		}

		// Token: 0x04000F30 RID: 3888
		[Dependency]
		private readonly IClientConGroupController _clientConGroupController;

		// Token: 0x04000F31 RID: 3889
		[Dependency]
		private readonly IClientConsoleHost _clientConsoleHost;
	}
}
