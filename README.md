
# GoodLawSoftware Password Manager (GLSPM)

A software that handles your own login details & credit cards



## Use Cases
[User Use Cases](https://github.com/AE-Mesco/GLSPM/blob/master/Planning/UseCases.pdf)
## Features
- Manage Application Users & Accounts
- CRUD on Login Data
- SPA web app
- CRUD Credit Cards "Soon"
- Soft Delete "Soon"
- Pagination "Soon"

## Used Technologies

- ASP dotnet core 6 'Backend'
- Blazor Webassembly 6 'Frontend'
- Entity Framework core 6 'Data Access'
- Sql Server , Sqlite , Mysql 'All These Databases are supported based on your perefers'
- Auto Mapper
- Identity Framework 
- Serilog 
- FluentValidation
- Gridify
- CubesFramework 'My own library'
- Swagger
## Architecture
 [Take a look on the Architecture](https://github.com/AE-Mesco/GLSPM/blob/master/Planning/Arch.pdf)
 
This application was build to based on some [DDD](https://martinfowler.com/bliki/DomainDrivenDesign.html) Concepts to be easy to maintain and scale up
There are 4 projects :
- Domain <Dotnet Standard 2.1 Class Library Project> : this project contains the Entities , Dtos , Base Entities & Dtos Classes , Shared constances and Repositories Interfaces.
- Application <Dotnet core 6 Class Library Project> : this project contains the Application services , Repositories implementations , Mapping Profile , Data Access , Helpers , Dtos Sever side Options and the whole infrastructure and application layer.The Application Project depends on Domain Project.
- Server <Asp dotnet core 6 web Project> : this project contains the web apis , apis docs , Db migrations,Uploaded files and needed resources  & requests handling. The Server projct depends Application & Domain
- Client <Blazor webassembly dotnet core 6 project> : this project contains the frontend pages, client side services & client side Dtos Options. The Client Project depends on Server & Domain project
 
## Data Structure
[How does the database look like](https://github.com/AE-Mesco/GLSPM/blob/master/Planning/DataStructure.pdf)
## Run Locally

- Clone the repo 
- Open it using VS code or Visual Studio 
- The default database provider is Sql server and the default server set to local db .
- There are priorities to the database providers based on the provided values in the settings file
- This is the priorities order ,'Sqlite' and if not set then 'Sql serve' and if not set then 'Mysql' and if not set then an exception would be thrown.
- To change the connection string Go to GLSMP.Server >> appsettings.json >> ConnectionString Section
- This is the defaults : 

```bash
  "ConnectionStrings": {
    "liteCS": "",
    "MSCS": "server=(LocalDB)\\MSSQLLocalDB;database=GLSPMDB;Trusted_Connection=true;",
    "MYSCS": ""
  }
```
- The Db Context will ensure if the database was created or not by itself , but its better to generate your new migration files after changing the Connection strings or the providers
- Generate new migrations files : Open an CMD on the  GLSPM.Application project path 

```bash
dotnet ef migrations add <Your Migration Class Name>
```

- Appliy the migrations to the Db

```bash
dotnet ef database update
```  


## Login

- Username : Admin
- Password : Admin@2022



## Authors

- [@AmirElkassar](https://github.com/AE-Mesco)
- [Gmail](Amir.elkassar@gmail.com)
- [Facebook](https://www.facebook.com/amiralielkassar/)
- [WhatsApp](https://wa.me/message/BAEM3XANAO2MD1)
- 201120797422


