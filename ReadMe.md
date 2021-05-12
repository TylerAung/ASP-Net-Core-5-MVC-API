Setup --> Properties

    launchSettings.json
    "launchBrowser": false, //Prevent web browser from launching on App Start
    "launchUrl": "swagger", //direct path of url on launch

Program.cs 

    CreateDefaultBuilder(args)//Sets default files and variable for project & logger configuration
    webBuilder.UseStartup<Startup>(); // Configure embedded or custom services for application needs, ultimately links to Startup.cs
Startup.cs 

    ConfigureServices() //Used to configure services, which is a reusable part of code that adds some functionality to application
    Configure() //Add different middleware components to app request pipeline

    appsettings.json
    appsetings.Development.json
    appsettings.Production.json	

As development and production environments have different URLs, ports, conn str, password and other sensitive information. There’s a need for different appsettings.json file for different environment


Setting application environment
Command prompt by typing set ASPNETCORE_ENVIRONMENT=Production in Windows

    launchSettings.json { "ASPNETCORE_ENVIRONMENT": "Development"}

VS will automatically linked appsettings.Production.json on the solution explorer once created, but in file explorer is separated

Extension Method accepts this as first parameter, which represents data type of object which will be using extension method. 
Extension Method must be defined inside a static class, which extend the behaviour of a type in .Net. 
Once defined, it can be chained multiple times on same type of object

Extensions(New Folder) --> ServicesExtensions.cs

CORS Configuration --> Give or restrict access rights to application from different domains
Send request from different domain to app

    using Microsoft.Extensions.DependencyInjection;

    public static class ServicesExtensions
    {

        public static void ConfigureCords(this IServiceCollection services)
            => services.AddCors(options =>
        {
            options.AddPolicy("CorsPolicy", builder =>
            builder.AllowAnyOrigin()
            .AllowAnyMethod()
            .AllowAnyHeader());
        });
    }

Every Request	Restrictive Request

    AllowAnyOrigin()	WithOrigins("https://example.com")
    AllowAnyMethod()	WithMethods("POST", "GET")
    AllowAnyHeader()	WithHeaders("accept", "content-type")



IIS Configuration (In the Extensions Class)
Alter ASP.NET Core App to host APP on IIS, instead of default self-hosting

        public static void ConfigureIISIntegration(this IServiceCollection services) 
            => services.Configure<IISOptions>(options => {

           });
Default values are good enough in the options property, but room for more customization is available.

Implement extension Methods onto Startup.cs

    public void ConfigureServices(IServiceCollection services) {
        services.ConfigureCors(); //Connecting to Extension Method for Cors
        services.ConfigureIISIntegration(); //Connecting to Extension Method for IIS
    }

    using Microsoft.AspNetCore.HttpOverrides;
    
    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {//Add different middleware components to app request pipeline
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseCors("CorsPolicy");
            app.UseForwardedHeaders(new ForwardedHeadersOptions
            {
                ForwardedHeaders = ForwardedHeaders.All
            });

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });


•	app.UseForwardedHeaders() will forward proxy headers to the current request. This will help us during application deployment. 

•	app.UseStaticFiles() enables using static files for the request. If we don’t set a path to the static files directory, it will use a wwwroot folder in our project by default. 

•	app.UseHsts() will add middleware for using HSTS, which adds the Strict-Transport-Security header. 

UseRouting() Add Routing features
UseAuthorization()  Adds Authorization features
UseEndpoint() with MapControllers() Adds an endpoint for the controller’s action to the routing without specifying any routes.

Microsoft Advise (Sequenc)
1)	UseCors() / UseStaticFiles()
2)	UseRouting()
3)	UseAuthorization()


 
Configuring Logging Service
Easy to debug app in development stage, but debugging in production environment is not easy
Logging helps to easily follow flow of application when debugger is inaccessible

1)	Create Class Library, One for LoggerService and another for Model?
2)	Add Project References (1 to 2, 2 to 3, 3 have access to 1 & 2)
3)	Add ILoggerManager interface into Models project and make public
4)	ILoggerManager properties(void LogInfo(string message); void LogWarn(string message); void LogDebug(string message); void LogError(string message);
5)	Install NLog.Extensions.Logging(Nuget) --> LoggerService Proj, NLog is a logging platform for .Net which help to create and log messages.
6)	Add new class LoggerManager and inherit ILoggerManager
7)	Add constructor and interface and (class scope variable private static ILogger logger = LogManager.GetCurrentClassLogger();)

    using Contracts;
    using NLog;
    namespace LoggerService
    {
        public class LoggerManager : ILoggerManager
        {
            private static ILogger logger = LogManager.GetCurrentClassLogger();
    
            public LoggerManager()
            {
    
            }
    
            public void LogDebug(string message)
            {
                logger.Debug(message);
            }
    
            public void LogError(string message)
            {
                logger.Error(message);
            }
    
            public void LogInfo(string message)
            {
                logger.Info(message);
            }
    
            public void LogWarn(string message)
            {
                logger.Warn(message);
            }
        }
    }


8)	In Main Project, create nlog.config file, add the following code but change internal & filename parameter to path, no need to create folder
    
    
    <?xml version="1.0" encoding="utf-8" ?>
    <nlog xmlns="http://www.nlog-project.org/schemas/NLog.xsd" 
          xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" 
          autoReload="true" 
          internalLogLevel="Trace" 
          internalLogFile="C:\Users\admin\Desktop\CompanyEmployeesAPI\internal_logs\internallog.txt">
      <targets>
        <target name="logfile" xsi:type="File" 
                fileName="C:\Users\admin\Desktop\CompanyEmployeesAPI/logs\${shortdate}_logfile.txt" 
                layout="${longdate} ${level:uppercase=true} ${message}"/>
      </targets>
      <rules>
        <logger name="*" minlevel="Debug" writeTo="logfile" />
      </rules>
    </nlog>
9)	Startup.cs -->  public Startup() --> 

    LogManager.LoadConfiguration(string.Concat(Directory.GetCurrentDirectory(), "/nlog.config")); 
    Configuration = configuration;
    
10)	Add another method to ServicesExtension Class


    using Contracts;
    using LoggerService;
    public static void ConfigureLoggerService(this IServiceCollection services) => 
         services.AddScoped<ILoggerManager, LoggerManager>();


11)	Update Startup.cs  public void ConfigureServices(IServiceCollection services)


    services.ConfigureLoggerService();
12)	After IISIntegration() but before AddControllers()
13)	In the API Controller  Add 


    public class ModelController : ControllerBase
    {
        private ILoggerManager _logger;
        public ModelController(ILoggerManager logger) { _logger = logger; }
        public IEnumerable<string> Get() { 
        _logger.LogInfo("Here is info message from our values controller.");
        _logger.LogDebug("Here is debug message from our values controller.");
        _logger.LogWarn("Here is warn message from our values controller.");
        _logger.LogError("Here is an error message from our values controller.");
        return new string[] { "value1", "value2" };
        }
    }
14)	Navigate to localhost:port/api/modelcontroller to test
 
DB Model & Repository Pattern
Db Class Modelling


        [Column("CompanyId")] public Guid Id { get; set; }
        [Required(ErrorMessage = "Company address is a required field.")] 
        [MaxLength(60, ErrorMessage = "Maximum length for the Address is 60 characters")]
        Establishing Foreign Key
        [Column("EmployeeId")]
        public Guid Id { get; set; }
        
        [ForeignKey(nameof(Company))] 
        public Guid CompanyId { get; set; }	[Column("CompanyId")] 
        public Guid Id { get; set; }
        
        Navigational Properties (Relational)
        public Company Company { get; set; }	public ICollection<Employee> Employees { get; set; }

Db Diagram


 	



 
ServiceExtension Connection
1)	Create Model Classes (Entities --> ModelsFiles)
2)	 Create Db Context.cs (Entities --> Files)

    public class RepositoryContext : DbContext
    {
    public RepositoryContext(DbContextOptions options) : base(options) {}
    public DbSet<Company> Companies { get; set; }
    public DbSet<Employee> Employees { get; set; }
3)	Add conn str to appSettings.json


    "ConnectionStrings": { "sqlConnection": "Data Source=Tyler-Desktop;Initial Catalog=CompanyEmployee;Integrated Security=True" },
4)	Configure ServiceExtension.cs


    public static void ConfigureSqlContext(this IServiceCollection services,
    IConfiguration configuration) =>
        services.AddDbContext<RepositoryContext>(opts =>
        opts.UseSqlServer(configuration.GetConnectionString("sqlConnection"), b=>
        b.MigrationsAssembly("CompanyEmployees")));
5)	Update Startup.cs --> ConfigureServices(IServiceCollection services)


    services.ConfigureSqlContext(Configuration);
    //Before services.AddControllers();
6)	Seed Db step (Configuration--> CompanyConfiguration/EmployeeConfiguration)
May skip to Step 8, to avoid seeding Db
    //File is only to Seed DB
    
    
    
    
    public class EmployeeConfiguration : IEntityTypeConfiguration<Employee> { 
        public void Configure(EntityTypeBuilder<Employee> builder) 
        { builder.HasData
           (new Employee 
            { 
               Id = new Guid("80abbca8-664d-4b20-b5de-024705497d4a"),
               Name = "Sam Raiden", 
               Age = 26, 
               Position = "Software developer",
               CompanyId = new Guid("c9d4c053-49b6-410c-bc78-2d54a9991870") 
            },
            new Employee 
            { 
                Id = new Guid("86dba8c0-d178-41e7-938c-ed49778fb52a"),
                Name = "Jana McLeaf", 
                Age = 30, 
                Position = "Software developer",
                CompanyId = new Guid("c9d4c053-49b6-410c-bc78-2d54a9991870") 
            },
            new Employee 
            { 
                Id = new Guid("021ca3c1-0deb-4afd-ae94-2159a8479811"),
                Name = "Kane Miller", 
                Age = 35, 
                Position = "Administrator",
                CompanyId = new Guid("3d490a70-94ce-4d15-9494-5248280c2ce3") 
            }
            ); 
        } 
    }	    //File is only to Seed DB
    public class CompanyConfiguration : IEntityTypeConfiguration<Company>
        {
            public void Configure(EntityTypeBuilder<Company> builder)
            { builder.HasData
                    ( new Company 
                    { 
                        Id = new Guid("c9d4c053-49b6-410c-bc78-2d54a9991870"), 
                        Name = "IT_Solutions Ltd", 
                        Address = "583 Wall Dr. Gwynn Oak, MD 21207", 
                        Country = "USA" 
                    }, 
                        new Company 
                        { 
                            Id = new Guid("3d490a70-94ce-4d15-9494-5248280c2ce3"), 
                            Name = "Admin_Solutions Ltd",
                            Address = "312 Forest Avenue, BF 923",
                            Country = "USA" 
                        } ); 
            } 
        }
    
    }



7)	Update RepositoryContext
protected override void OnModelCreating(ModelBuilder modelBuilder) {
modelBuilder.ApplyConfiguration(new CompanyConfiguration());
modelBuilder.ApplyConfiguration(new EmployeeConfiguration());
}
Then perform Add-Migration & Update-Db
Transfers all data from configuration files to Table

Repository Pattern Logic (Interface & Classes)
Generic repository providing CRUD methods, as a result all methods can be called upon any repository class in project.
On top of that, creating a wrapper around repository classes and injecting as a service in a dependency injection container
Contracts IRepositoryBase.cs
public interface IRepositoryBase<T>
    {
        IQueryable<T> FindAll(bool trackChanges); 
        IQueryable<T> FindByCondition
(Expression<Func<T, bool>> expression, bool trackChanges); 
        void Create(T entity); 
        void Update(T entity); 
        void Delete(T entity);
    }
MainProject --> RepositoryBase.cs
public abstract class RepositoryBase<T> : IRepositoryBase<T> where T : class
    {
        protected RepositoryContext RepositoryContext; 
        public RepositoryBase(RepositoryContext repositoryContext) 
        { 
            RepositoryContext = repositoryContext; 
        }
        public IQueryable<T> FindAll(bool trackChanges) =>
            !trackChanges? 
            RepositoryContext.Set<T>() 
            .AsNoTracking() : 
            RepositoryContext.Set<T>();
        public IQueryable<T> FindByCondition(
Expression<Func<T, bool>> expression, bool trackChanges) 
            => !trackChanges ? 
            RepositoryContext.Set<T>()
            .Where(expression).AsNoTracking() : 
            RepositoryContext.Set<T>()
            .Where(expression); 

        public void Create(T entity) => RepositoryContext.Set<T>().Add(entity); 
        public void Update(T entity) => RepositoryContext.Set<T>().Update(entity); 
        public void Delete(T entity) => RepositoryContext.Set<T>().Remove(entity);
    }

With IRepository.cs working with generic type T. This gives us reusability in the RepositoryBase.cs, allowing flexibility in specifying model class right on the RepositoryBase.cs
As T is a generic type, it allows, this offer more reusability as is not restricted to a specific model (class) for RepositoryBase.cs
Moreover, in the method IQueryable, the parameter trackChanges is used to improve read-only query performance. When result is false, the method AsNoTrack query inform EF Core,
there’s no need to track changes for required entities. Improving speed of query.
Repository User Interfaces and Classes
By inheriting from RepositoryBase.cs the user classes will have access to all methods from it. On top of that, each user class will have interface for additional model-specific methods.
Allowing separating of logic that is common and specific to each user class itself.
1)	Contracts --> ICompanyRepository & IEmployeeRepository
public interface IEmployeeRepository {} 
2)	Repository --> CompanyRepository & EmployeeRepository


    public class EmployeeRepository : 
    RepositoryBase<Employee>, IEmployeeRepository
    {
        public EmployeeRepository(RepositoryContext repositoryContext) : base(repositoryContext) { }
    }

        public class CompanyRepository : RepositoryBase<Company>, ICompanyRepository
    {  
        public CompanyRepository(RepositoryContext repositoryContext) : base (repositoryContext) { }
    }

Repository Manager
For API to return response with data from multiple resources(E.g two or more tables or so). There’s a need to instantiate both repository classes and fetch data accordingly. More instances if there’s more than two tables/resources. With that in mind, a repository manager will create instances of repository user class for us  and then register it inside dependency injection container. After which, we will inject it inside our controller(or business layer class, if big app). With constructor injection.
Contracts  IRepositoryManager

    public interface IRepositoryManager
    {
        ICompanyRepository Company { get; }
        IEmployeeRepository Employee { get; }
        void Save();
    }

Repository  RepositoryManager
    public class RepositoryManager : IRepositoryManager
    {
        private RepositoryContext _repositoryContext;
        //Properties that will expose the concrete repositories
        private ICompanyRepository _companyRepository; 
        private IEmployeeRepository _employeeRepository;

        public RepositoryManager(RepositoryContext repositoryContext) 
        {
            _repositoryContext = repositoryContext; 
        }

        
        public ICompanyRepository Company 
        { 
            get 
            { 
                if (_companyRepository == null) 
                    _companyRepository = new CompanyRepository(_repositoryContext); 
                return _companyRepository; 
            } 
        }

        public IEmployeeRepository Employee 
        { 
            get 
            { 
                if (_employeeRepository == null) 
                    _employeeRepository = new EmployeeRepository(_repositoryContext); 
                return _employeeRepository; 
            }
        }

        //Method to be used after all the modifications are finished on a certain object
        public void Save() => _repositoryContext.SaveChanges();
    }


RepositoryManager.cs  creates properties that expose the concrete repositories and have the Save() method to be used after modifications are finished on a certain object. This practice allows modification of two companies, employees, and delete one company all in a single action then just calling Save() once.
These changes will be applied or if fails, changes will be reverted.
ServicesExtension.cs--> ConfigureRepositoryManager()

    public static void ConfigureRepositoryManager(this IServiceCollection service
      => services.AddScoped<IRepositoryManager, RepositoryManager>();
      
Update Startup.cs --> ConfigureServices()

    services.ConfigureRepositoryManager();

Injecting RepositoryManager.cs inside controller gives visibility on Company and Employee properties which provides access to specific repository methods

    [ApiController]
    [Route("[controller]")]
    public class WeatherForecastController : ControllerBase
    {
        private ILoggerManager _logger;
        private readonly IRepositoryManager _repository;

        public WeatherForecastController(ILoggerManager logger, IRepositoryManager repository) 
        { 
            _logger = logger;
            _repository = repository;
        }
        
        [HttpGet] 
        public ActionResult<IEnumerable<string>> Get() { 
            _logger.LogInfo("Here is info message from our values controller.");
            _logger.LogDebug("Here is debug message from our values controller.");
            _logger.LogWarn("Here is warn message from our values controller.");
            _logger.LogError("Here is an error message from our values controller.");

            //_repository.Company.AnyMethodFromCompanyRepository(); //& Interface property, will amend
            //_repository.Employee.AnyMethodFromEmployeeRepository();

            _repository.Save();

            return new string[] { "value1", "value2" }; 
        }
    }

Good practice for an application of this size. But for larger-scale applications, advisable to create an additional business layer between controllers and repository logic. RepositoryManager service would be injected inside that Business layer freeing the controller from repository logic.
