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
        /// <summary>
        /// Represents an uninitialized or unknown value.
        /// </summary>
        /// <remarks>Use this member to indicate that a value has not been set or cannot be determined.
        /// This is commonly used as a default or sentinel value in enumerations or value types.</remarks>
        Undefined,
        /// <summary>
        /// Specifies that a service or component is created once per request.
        /// </summary>
        /// <remarks>Use this value to indicate that a new instance should be provided for each incoming
        /// request, ensuring request-level isolation. This is commonly used in web applications to scope dependencies
        /// to the lifetime of a single HTTP request.</remarks>
        PerRequst,
        /// <summary>
        /// Represents a type that ensures only a single instance exists throughout the application's lifetime.
        /// </summary>
        /// <remarks>Use this type to implement the singleton design pattern, which restricts
        /// instantiation to one object and provides a global point of access. This is commonly used for shared
        /// resources or configuration objects.</remarks>
        Singleton,
        /// <summary>
        /// Gets the singleton instance of the class.
        /// </summary>
        /// <remarks>Use this property to access the shared instance rather than creating a new object.
        /// This ensures consistent state and resource management across the application.</remarks>
        Instance
    }
}
