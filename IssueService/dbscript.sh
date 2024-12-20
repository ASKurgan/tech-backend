docker-compose up -d

dotnet-ef database drop -f -c IssuesWriteDbContext -p ./src/Issues/SachkovTech.Issues.Infrastructure/ -s ./src/SachkovTech.Web/

dotnet-ef migrations remove -c IssuesWriteDbContext -p ./src/Issues/SachkovTech.Issues.Infrastructure/ -s ./src/SachkovTech.Web/
dotnet-ef migrations remove -c FilesWriteDbContext -p ./src/Files/SachkovTech.Files.Infrastructure/ -s ./src/SachkovTech.Web/
dotnet-ef migrations remove -c AccountsWriteDbContext -p ./src/Accounts/SachkovTech.Accounts.Infrastructure/ -s ./src/SachkovTech.Web/

dotnet-ef migrations add init -c IssuesWriteDbContext -p ./src/Issues/SachkovTech.Issues.Infrastructure/ -s ./src/SachkovTech.Web/
dotnet-ef migrations add init -c FilesWriteDbContext -p ./src/Files/SachkovTech.Files.Infrastructure/ -s ./src/SachkovTech.Web/
dotnet-ef migrations add init -c AccountsWriteDbContext -p ./src/Accounts/SachkovTech.Accounts.Infrastructure/ -s ./src/SachkovTech.Web/

dotnet-ef database update -c IssuesWriteDbContext -p ./src/Issues/SachkovTech.Issues.Infrastructure/ -s ./src/SachkovTech.Web/
dotnet-ef database update -c FilesWriteDbContext -p ./src/Files/SachkovTech.Files.Infrastructure/ -s ./src/SachkovTech.Web/
dotnet-ef database update -c AccountsWriteDbContext -p ./src/Accounts/SachkovTech.Accounts.Infrastructure/ -s ./src/SachkovTech.Web/

pause