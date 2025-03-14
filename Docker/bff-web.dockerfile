FROM mcr.microsoft.com/dotnet/sdk:9.0 as BUILD
WORKDIR /app 

COPY / /app/
RUN dotnet restore ./mark.davison.novella.sln
RUN dotnet publish -c Release -o out bff-web/mark.davison.novella.bff.web/mark.davison.novella.bff.web.csproj

FROM mcr.microsoft.com/dotnet/aspnet:9.0-noble-chiseled
WORKDIR /app
COPY --from=BUILD /app/out .

ENTRYPOINT ["dotnet", "mark.davison.novella.bff.web.dll"]