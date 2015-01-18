namespace ProfSite.Tests.Infrastructure
{
    public abstract class AbstractDataScenario
    {
        public const string TestUserName = "testuser";
        public abstract void CreateData();

    }
}