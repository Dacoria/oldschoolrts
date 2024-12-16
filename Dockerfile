# ----------- Unity ---------

FROM unityci/editor:ubuntu-2020.3.0f1-webgl-0.12.0 AS unity-web-activated-old
WORKDIR /src/license
COPY ["./Unity_v2020.x.ulf", "."]
COPY ["./activate.sh", "."]
RUN chmod +x activate.sh
RUN /src/license/activate.sh

FROM unity-web-activated-old AS unity-web-build-old
WORKDIR /src/build
COPY [".", "."]
RUN chmod +x build.sh
RUN /src/build/build.sh

#----------- Nginx ---------

FROM nginx as FINAL-old
COPY --from=unity-web-build-old ["./src/build/Build/StaticFiles", "./usr/share/nginx/html/"]
RUN touch hi
EXPOSE 80