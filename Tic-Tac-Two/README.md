Migration commands, run from solution folder:
~~~sh
dotnet ef migrations add InitialDbCreation
dotnet ef database update
dotnet ef database drop
~~~

Install EF tools:
~~~sh
dotnet tool install --global dotnet-ef
dotnet tool update --global dotnet-ef
~~~

Scaffolding utility install/update:
~~~sh
dotnet tool install --global dotnet-aspnet-codegenerator
dotnet tool update --global dotnet-aspnet-codegenerator
~~~

Run from webapp folder:
~~~sh
dotnet aspnet-codegenerator razorpage -m GameConfiguration -outDir Pages/Configurations -dc AppDbContext -udl --referenceScriptLibraries -f
dotnet aspnet-codegenerator razorpage -m SavedGame -outDir Pages/SavedGames -dc AppDbContext -udl --referenceScriptLibraries -f
~~~
