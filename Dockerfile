# syntax=docker/dockerfile:1

ARG ARCH=
FROM mcr.microsoft.com/dotnet/sdk:6.0.411-alpine3.18-${ARCH} AS builder
WORKDIR /src
COPY src /src/
RUN dotnet publish -c Release /src/SpreadsheetLooker.Web -o /app

FROM mcr.microsoft.com/dotnet/aspnet:6.0.19-alpine3.18-${ARCH}
VOLUME ["/data"]
ENV CONFIG_PATH=/data/config.json
COPY --from=builder /app /app
RUN ln -s /app/SpreadsheetLooker.Web /bin/web
CMD "/bin/web"
