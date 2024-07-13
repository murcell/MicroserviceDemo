FROM mcr.microsoft.com/dotnet/sdk:8.0 as build
WORKDIR /app
EXPOSE 80
COPY Shared/FreeCourse.Shared/*.csproj Shared/FreeCourse.Shared/
COPY Fronttends/FreeCourse.Web/*.csproj Fronttends/FreeCourse.Web/
RUN dotnet restore Fronttends/FreeCourse.Web/*.csproj
COPY . .
RUN dotnet publish Fronttends/FreeCourse.Web/*.csproj -c Release -o out
FROM mcr.microsoft.com/dotnet/aspnet:8.0 as runtime
WORKDIR /app
COPY --from=build /app/out .
ENTRYPOINT [ "dotnet","FreeCourse.Web.dll" ]