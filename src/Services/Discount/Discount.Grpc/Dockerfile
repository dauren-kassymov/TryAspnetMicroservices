FROM mcr.microsoft.com/dotnet/aspnet:5.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build

# manually install protoc 
RUN apt-get update \
 && DEBIAN_FRONTEND=noninteractive \
    apt-get install --no-install-recommends --assume-yes \
      protobuf-compiler-grpc
# and use that compiler
ENV PROTOBUF_PROTOC=/usr/bin/protoc

WORKDIR /src
COPY ["Services/Discount/Discount.Grpc/Discount.Grpc.csproj", "Services/Discount/Discount.Grpc/"]
RUN dotnet restore "Services/Discount/Discount.Grpc/Discount.Grpc.csproj"
COPY . .
WORKDIR "/src/Services/Discount/Discount.Grpc"
RUN dotnet build "Discount.Grpc.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Discount.Grpc.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Discount.Grpc.dll"]
