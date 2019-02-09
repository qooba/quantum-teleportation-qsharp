FROM microsoft/dotnet
COPY . /app
WORKDIR /app

RUN \
     apt update && \
     apt-get -yq install libgomp1

RUN ["dotnet", "new", "-i", "Microsoft.Quantum.ProjectTemplates::0.3.1811.2802-preview"]
RUN ["dotnet", "restore"]
RUN ["dotnet", "build"]
EXPOSE 5000
RUN chmod +x ./entrypoint.sh
CMD /bin/bash ./entrypoint.sh
