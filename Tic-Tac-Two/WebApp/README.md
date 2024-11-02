dotnet aspnet-codegenerator razorpage -m Restaurant -outDir Pages/Restaurants -dc AppDbContext -udl --referenceScriptLibraries
dotnet aspnet-codegenerator razorpage -m Table -outDir Pages/Tables -dc AppDbContext -udl --referenceScriptLibraries

add "-f" in the end of command to force-write everything over (when changes are needed)