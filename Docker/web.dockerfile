FROM mcr.microsoft.com/dotnet/sdk:9.0 as BUILD
WORKDIR /app 

ENV CI_BUILD=true

COPY / /app/

RUN rm web/mark.davison.novella.web.ui/wwwroot/css/*.css -f
RUN rm web/mark.davison.novella.web.ui/wwwroot/css/*.min.css -f

RUN dotnet tool install Excubo.WebCompiler --global
RUN /root/.dotnet/tools/webcompiler web/mark.davison.novella.web.ui/wwwroot/css -r -m -o web/mark.davison.novella.web.ui/wwwroot/css -z disable

RUN dotnet restore web/mark.davison.novella.web.ui/mark.davison.novella.web.ui.csproj
RUN dotnet publish -c Release -o /app/publish/ web/mark.davison.novella.web.ui/mark.davison.novella.web.ui.csproj

FROM nginx:alpine AS FINAL
WORKDIR /usr/share/nginx/html
COPY --from=BUILD /app/publish/wwwroot .
COPY web/entry.sh /usr/share/nginx/html/entry.sh
COPY web/nginx.conf /etc/nginx/nginx.conf

RUN ls -l /usr/share/nginx/html
RUN ls -l /usr/share/nginx/html/css

RUN chmod +x /usr/share/nginx/html/entry.sh

WORKDIR /usr/share/nginx/html

CMD ["sh", "entry.sh"]