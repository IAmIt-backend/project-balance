using AspNet.Identity.MongoDB;

namespace Balance
{
    public class EnsureAuthIndexes
	{
		public static void Exist()
		{
			var context = ApplicationIdentityContext.Create();
			IndexChecks.EnsureUniqueIndexOnUserName(context.Users);
			IndexChecks.EnsureUniqueIndexOnRoleName(context.Roles);
		}
	}
}