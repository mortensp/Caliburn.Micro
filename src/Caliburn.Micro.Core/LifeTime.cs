namespace Caliburn.Micro
{
    /// <summary>
    /// Specifies the lifetime of a service in a dependency injection container.
    /// </summary>
    /// <remarks>This enumeration is used to indicate how instances of a service are managed within the
    /// container: <list type="bullet"> <item> <term><see cref="LifeTime.PerRequst"/></term> <description>A new instance
    /// of the service is created for each request.</description> </item> <item> <term><see
    /// cref="LifeTime.Singleton"/></term> <description>A single instance of the service is created and shared across
    /// all requests.</description> </item> </list></remarks>
    public enum LifeTime //mspa
    {
        Undefined,
        PerRequst,
        Singleton,
        Instance
    }
}
