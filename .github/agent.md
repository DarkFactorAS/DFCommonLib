# GitHub Copilot Instructions for DFCommonLib project

## Project Overview
DFCommonLib is a reusable .NET library providing common functionality for multiple projects. The library is designed to simplify:
- Database access (MySQL, Oracle) with connection pooling
- HTTP REST API client/server operations
- OAuth2 authentication and authorization with JWT tokens
- Structured logging across multiple outputs (Console, EventLog, MySQL)
- Configuration management
- Common utilities (cryptography, sessions, services)

### Test Projects
- **DFCommonLib.TestAppClient**: Client-side test application demonstrating HTTP and OAuth2 client usage
- **DFCommonLib.TestAppServer**: Server-side test application with REST endpoints and OAuth2 authentication
- **DFCommonLib.Unittests**: Comprehensive unit test suite using NUnit and Moq
- Both test apps are designed for automated testing in GitHub Actions

## Project Structure

```
DFCommonLib/                    # Core library
├── Config/                     # Configuration helpers and AppSettings
├── DataAccess/                 # Database interfaces and implementations
│   ├── Base/                   # Abstract base classes and interfaces
│   ├── MySQL/                  # MySQL-specific implementations
│   └── Oracle/                 # Oracle-specific implementations
├── HttpApi/                    # HTTP REST client/server base classes
│   ├── Client/                 # REST client implementations
│   ├── Server/                 # REST server controllers
│   ├── Common/                 # Shared models and utilities
│   └── RestData/               # REST data models
├── HttpOAuth/                  # OAuth2 implementation
│   ├── Client/                 # OAuth2 client (authentication flow)
│   ├── Server/                 # OAuth2 server (token provider)
│   ├── Model/                  # OAuth2 data models
│   └── RestData/               # OAuth2 REST responses
├── Logger/                     # Logging framework
│   ├── DFLogger.cs             # Main logger interface and factory
│   ├── ConsoleLogWriter.cs     # Console output writer
│   ├── EventLogWriter.cs       # Event log writer
│   └── MySqlLogWriter.cs       # Database log writer
└── Utils/                      # Common utilities
    ├── DFCommonUtil.cs         # General utilities
    ├── DFCrypt.cs              # Cryptography helpers
    ├── DFServices.cs           # Dependency injection helper
    └── DFUserSession.cs        # User session management
```

## Technology Stack

### Core Framework
- **.NET 8.0** (Target framework)
- **C# 12** with nullable reference types enabled
- **ASP.NET Core 8.0** for web API functionality

### Key Dependencies
- **Microsoft.Extensions.*** (DI, Configuration, Logging, Hosting)
- **Newtonsoft.Json** for JSON serialization
- **System.IdentityModel.Tokens.Jwt** for JWT token handling
- **Microsoft.AspNetCore.Authentication.JwtBearer** for authentication
- **Swashbuckle.AspNetCore** for API documentation (Swagger/OpenAPI)
- **MySql.Data** for MySQL database access
- **Oracle.ManagedDataAccess.Core** for Oracle database access

### Testing & Quality
- **NUnit 3** as the testing framework
- **Moq 4.20** for mocking dependencies
- **coverlet.collector** for code coverage
- Target: **100% unit test coverage**

### Development Tools
- **Visual Studio Code** as primary IDE
- **GitHub** for version control and CI/CD
- **GitHub Actions** for automated testing

## Coding Standards & Conventions

### Naming Conventions
- **Classes/Interfaces**: PascalCase (e.g., `DFLogger`, `IDFHttpRestClient`)
- **Methods**: PascalCase (e.g., `GetJsonData`, `AuthenticateIfNeeded`)
- **Public Properties**: PascalCase (e.g., `ClientId`, `TokenExpiresInSeconds`)
- **Private Fields**: camelCase with underscore prefix (e.g., `_logger`, `_accessToken`)
- **Constants**: UPPER_CASE with underscores (e.g., `INVALID_CLIENT_ID`)
- **Async Methods**: Suffix with `Async` when appropriate (though not enforced for all async methods)

### Interface Pattern
- All major classes should have corresponding interfaces (e.g., the generic `DFLogger<T>` implements `IDFLogger<T>`, while the non-generic `DFLogger` is a static helper)
- Interfaces enable dependency injection and unit testing with mocks
- Use generic interfaces where type safety is needed (e.g., `IDFLogger<T>`)

### Dependency Injection
- Use constructor injection for dependencies
- Register services in `Startup.cs` or service configuration methods
- Use `DFServices.GetService<T>()` helper for service location when DI isn't available
- Example pattern:
```csharp
public class MyClass
{
    private readonly IDFLogger<MyClass> _logger;
    
    public MyClass(IDFLogger<MyClass> logger)
    {
        _logger = logger;
    }
}
```

### Async/Await Pattern
- Use `async`/`await` for all I/O-bound operations (HTTP, database)
- Return `Task` or `Task<T>` for async methods
- Don't block on async code (no `.Wait()` or `.Result`)
- Example:
```csharp
public async Task<WebAPIData> GetJsonData(int methodId, string url)
{
    var response = await HandleRequest(methodId, request);
    return response;
}
```

### Nullable Reference Types
- Enable nullable reference types in all new code (`#nullable enable`)
- Use `?` for nullable reference types explicitly
- Validate input parameters and handle null cases appropriately

### Error Handling
- Use `WebAPIData` base class for API responses with error handling
- Include `ErrorMessage`, `Error`, and `Success` properties
- Log errors appropriately using `IDFLogger`
- Return structured error responses rather than throwing exceptions in API controllers

## Component Architecture

### 1. Database Access (`DataAccess/`)
**Pattern**: Repository pattern with connection pooling

**Key Classes**:
- `IDbConnectionFactory`: Interface for database connections
- `DbConnectionPool`: Connection pool management
- `IDbRepository`: Base repository interface

**Usage**:
```csharp
public class MyRepository
{
    private readonly IDbConnectionFactory _connection;
    
    public MyRepository(IDbConnectionFactory connection)
    {
        _connection = connection;
    }
    
    public List<MyModel> GetData()
    {
        string sql = "SELECT * FROM my_table";
        using (var cmd = _connection.CreateCommand(sql))
        {
            using (var reader = cmd.ExecuteReader())
            {
                // Process results
            }
        }
    }
}
```

**Supported Databases**:
- MySQL (via `MySql.Data`)
- Oracle (via `Oracle.ManagedDataAccess.Core`)

### 2. HTTP REST Client (`HttpApi/Client/`)
**Pattern**: Base client class with fluent API

**Key Classes**:
- `IDFHttpRestClient`: Base REST client interface
- `DFHttpRestClient`: Base implementation with GET/POST/PUT methods
- `DFOAuth2RestClient`: OAuth2-enabled REST client

**Usage**:
```csharp
public class MyRestClient : DFHttpRestClient
{
    public async Task<MyModel> GetData(int id)
    {
        var response = await GetJsonDataAs<MyModel>(1, $"api/data/{id}");
        return response;
    }
    
    protected override string GetModule()
    {
        return "MyApiModule";
    }
}
```

**Features**:
- Automatic JSON serialization/deserialization
- Access token management
- Request/response logging
- Error handling with `WebAPIData`

### 3. OAuth2 Authentication (`HttpOAuth/`)
**Pattern**: Two-phase authentication flow (Auth → Code → Token)

**Flow**:
1. Client requests auth code with credentials (`Auth` endpoint)
2. Client exchanges code for JWT token (`Code` endpoint)
3. Client uses JWT token for authenticated API calls

**Server Setup** (in `Startup.cs`):
```csharp
using DFCommonLib.HttpApi.OAuth2;

public void ConfigureServices(IServiceCollection services)
{
    OAuth2Server.SetupService(services);
    OAuth2Server.SetupSwaggerApi("MyService", services);
}
```

**Client Usage**:
```csharp
public class MyAuthClient : DFOAuth2RestClient
{
    public async Task<MyData> GetProtectedData()
    {
        await AuthenticateIfNeeded(); // Handles token refresh
        var response = await GetJsonDataAs<MyData>(1, "api/protected");
        return response;
    }
}
```

**Key Components**:
- `OAuth2Server`: Server setup and configuration
- `IServerOAuth2Provider`: Token generation and validation
- `IServerOAuth2Repository`: Client credentials storage
- `DFOAuth2RestClient`: Client with automatic authentication
- `DFOAuth2JwtTokenHandler`: JWT token validation

### 4. Logging (`Logger/`)
**Pattern**: Structured logging with multiple outputs

**Key Classes**:
- `IDFLogger<T>`: Generic logger interface
- `DFLogger`: Logger factory and implementation
- `ILogOutputWriter`: Output writer interface
- `ConsoleLogWriter`, `EventLogWriter`, `MySqlLogWriter`: Output implementations

**Usage**:
```csharp
public class MyClass
{
    private readonly IDFLogger<MyClass> _logger;
    
    public MyClass(IDFLogger<MyClass> logger)
    {
        _logger = logger;
    }
    
    public void DoWork()
    {
        _logger.LogInfo("Starting work");
        _logger.LogDebug("Debug details");
        _logger.LogError("An error occurred");
    }
}
```

**Log Levels**:
- `LogDebug`: Detailed debugging information
- `LogInfo`: Informational messages
- `LogWarning`: Warning messages
- `LogError`: Error messages

### 5. Configuration (`Config/`)
**Pattern**: Strongly-typed configuration with `appsettings.json`

**Key Classes**:
- `ConfigurationHelper`: Load and bind configuration
- `AppSettings`: Application settings model

**Usage**:
```csharp
var config = ConfigurationHelper.LoadConfiguration<MyConfig>("Config/appsettings.json");
```

## Testing Guidelines

### Unit Testing Framework
- Use **NUnit 3** with `[Test]` attributes
- Use **Moq** for mocking dependencies
- Organize tests in `DFCommonLib.Unittests/Tests/`

### Test Structure
```csharp
[TestFixture]
public class MyClassTests
{
    private Mock<IDFLogger<MyClass>> _mockLogger;
    private MyClass _sut; // System Under Test
    
    [SetUp]
    public void Setup()
    {
        _mockLogger = new Mock<IDFLogger<MyClass>>();
        _sut = new MyClass(_mockLogger.Object);
    }
    
    [Test]
    public void MethodName_Scenario_ExpectedBehavior()
    {
        // Arrange
        var input = "test";
        
        // Act
        var result = _sut.Method(input);
        
        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.Success, Is.True);
    }
}
```

### Test Naming Convention
Use the pattern: `MethodName_Scenario_ExpectedBehavior`
- Example: `Auth_WithValidCredentials_ReturnsAuthCode`
- Example: `GetJsonData_WhenNetworkError_HandlesGracefully`

### Mocking Dependencies
```csharp
// Mock logger
var mockLogger = new Mock<IDFLogger<MyClass>>();

// Mock database connection
var mockConnection = new Mock<IDbConnectionFactory>();
mockConnection.Setup(c => c.CreateCommand(It.IsAny<string>()))
    .Returns(mockCommand.Object);

// Verify method calls
mockLogger.Verify(l => l.LogError(It.IsAny<string>()), Times.Once);
```

### Coverage Goals
- **Target**: 100% code coverage for all functionality
- **Priority**: Core library components (DataAccess, HttpApi, HttpOAuth, Logger)
- Run tests via: `dotnet test` or VS Code Test Explorer
- Generate coverage reports using `coverlet.collector`

### Integration Testing
- Use `TestAppClient` and `TestAppServer` for end-to-end testing
- Test complete OAuth2 flow (auth → code → token → authenticated request)
- Test database operations with actual database connections
- Suitable for automated testing in GitHub Actions

## Common Development Scenarios

### 1. Creating a New REST API Endpoint

**Server Side** (in TestAppServer):
```csharp
[ApiController]
[Route("[controller]")]
public class MyController : DFRestServerController
{
    [HttpGet]
    [Route("getData")]
    public MyDataModel GetData(int id)
    {
        return new MyDataModel { Id = id, Name = "Data" };
    }
}
```

**Client Side** (in TestAppClient):
```csharp
public class MyRestClient : DFHttpRestClient
{
    public async Task<MyDataModel> GetData(int id)
    {
        var response = await GetJsonDataAs<MyDataModel>(1, $"api/getData?id={id}");
        return response;
    }
}
```

### 2. Adding OAuth2 Protection to an Endpoint

```csharp
[ApiController]
[Route("[controller]")]
public class SecureController : DFRestServerController
{
    [Authorize] // Requires valid JWT token
    [HttpGet]
    [Route("secureData")]
    public MyDataModel GetSecureData()
    {
        return new MyDataModel { Name = "Protected Data" };
    }
}
```

### 3. Creating a Database Repository

```csharp
public interface IMyRepository
{
    List<MyModel> GetAll();
    MyModel GetById(int id);
}

public class MyRepository : IMyRepository
{
    private readonly IDbConnectionFactory _connection;
    private readonly IDFLogger<MyRepository> _logger;
    
    public MyRepository(
        IDbConnectionFactory connection,
        IDFLogger<MyRepository> logger)
    {
        _connection = connection;
        _logger = logger;
    }
    
    public List<MyModel> GetAll()
    {
        var results = new List<MyModel>();
        string sql = "SELECT id, name FROM my_table";
        
        using (var cmd = _connection.CreateCommand(sql))
        {
            using (var reader = cmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    results.Add(new MyModel
                    {
                        Id = Convert.ToInt32(reader["id"]),
                        Name = reader["name"].ToString()
                    });
                }
            }
        }
        
        return results;
    }
}
```

### 4. Adding Logging to a Class

```csharp
public class MyClass
{
    private readonly IDFLogger<MyClass> _logger;
    
    public MyClass(IDFLogger<MyClass> logger)
    {
        _logger = logger;
    }
    
    public void ProcessData()
    {
        _logger.LogInfo("Starting data processing");
        
        try
        {
            // Process data
            _logger.LogDebug("Processing step 1");
            // ...
            _logger.LogInfo("Data processing completed successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error processing data: {ex.Message}");
            throw;
        }
    }
}
```

### 5. Implementing a New OAuth2 Client

```csharp
public interface IMyAuthClient : IDFOAuth2RestClient
{
    Task<MyDataModel> GetProtectedData(int id);
}

public class MyAuthClient : DFOAuth2RestClient, IMyAuthClient
{
    private const int GET_DATA = 1;
    
    public async Task<MyDataModel> GetProtectedData(int id)
    {
        await AuthenticateIfNeeded(); // Ensures valid token
        var response = await GetJsonDataAs<MyDataModel>(GET_DATA, $"api/data/{id}");
        return response;
    }
    
    protected override string GetModule()
    {
        return "MyApiModule";
    }
}
```

## Best Practices Summary

### General Principles
1. **Clarity**: Write clear, self-documenting code with meaningful names
2. **Consistency**: Follow established patterns and conventions throughout
3. **Testability**: Design for dependency injection and unit testing
4. **Error Handling**: Handle errors gracefully and provide meaningful messages
5. **Logging**: Log important operations, errors, and state changes
6. **Documentation**: Comment complex logic and document public APIs

### Code Quality
- Enable and respect nullable reference types
- Treat warnings as errors (TreatWarningsAsErrors=true)
- Use async/await for I/O operations
- Dispose resources properly (using statements)
- Validate input parameters
- Use strongly-typed models over primitive types

### Security
- Never log sensitive data (passwords, tokens, secrets)
- Use parameterized queries for database operations
- Validate and sanitize user input
- Use HTTPS for production deployments
- Rotate OAuth2 secrets regularly
- Set appropriate token expiration times

### Performance
- Use connection pooling for database operations
- Reuse HttpClient instances (static)
- Avoid blocking on async code
- Cache frequently accessed data when appropriate
- Use appropriate database indexes

## GitHub Copilot Usage Guidelines

When using GitHub Copilot with this project:

1. **Context Awareness**: Copilot understands the project structure and patterns described in this document
2. **Pattern Following**: Copilot suggestions will follow the established conventions and patterns
3. **Test Generation**: Request unit tests for new features using NUnit and Moq
4. **Documentation**: Ask Copilot to document complex methods and classes
5. **Refactoring**: Use Copilot for refactoring while maintaining existing interfaces
6. **Code Review**: Leverage Copilot to explain code and suggest improvements

### Effective Prompts
- "Create a unit test for [method name] using NUnit and Moq"
- "Implement a repository for [entity] following the existing pattern"
- "Add OAuth2 authentication to [controller/endpoint]"
- "Create a REST client for [API] extending DFHttpRestClient"
- "Add logging to [method/class] using IDFLogger"

## Additional Resources

- **Solution File**: `DFCommonLib.sln`
- **Documentation**: `DFCommonLib/Docs/readme.md`
- **Database Scripts**: `DFCommonLib/DatabaseScripts/`
- **Configuration Examples**: `DFCommonLib.TestAppClient/Config/appsettings.json`
- **Docker Support**: `docker-compose.yml`, `Dockerfile`

## Feedback and Adaptation

These instructions are living documentation and should evolve with the project:
- Adjust patterns as new requirements emerge
- Update conventions as the team's preferences evolve
- Expand examples based on common questions
- Refine guidelines based on code review feedback

When in doubt, refer to existing code in the repository as the source of truth for implementation patterns.