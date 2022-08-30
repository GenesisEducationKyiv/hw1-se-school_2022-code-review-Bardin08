[![Build and Test](https://github.com/Bardin08/GenesisCaseTask/actions/workflows/build-and-test.yml/badge.svg?branch=master)](https://github.com/Bardin08/GenesisCaseTask/actions/workflows/build-and-test.yml)
[![Push Docker Image](https://github.com/Bardin08/GenesisCaseTask/actions/workflows/push-docker-image-to-hub.yml/badge.svg)](https://github.com/Bardin08/GenesisCaseTask/actions/workflows/push-docker-image-to-hub.yml)


# Genesis Case Task
This repo contains a case task for Genesis Engineering School 2022. The solution can:
1. Fetch BTC to UAH exchange rate from https://coinbase.com
2. Subscribe and notification functionality
3. Send email notifications with actual exchange rate

## How to run

**IMPORTANT**: To run the solution without any minipulations with external API tokens use already build image from the GitHub Container registry. 
Genesis case task image: https://ghcr.io/bardin08/genesis-case-task;

### Prerequirements
1. Installed .NET 6 SDK
2. Installed Docker

### Run docker image from the GitHub Container registry (Preferred)
> For this method you need only Docker installed.

1. Pull docker image from the GitHub container registry

   `$ docker pull ghcr.io/bardin08/genesis-case-task:latest`
   
   This will pull the docker image with all tokens and secrets settled.

2. Run pulled image locally
  
   `$ docker run -p <YOUR_LOCAL_PORT>:80 ghcr.io/bardin08/genesis-case-task:latest`
   
   By default, the container is running in `Development` mode and has a swagger UI at the `swagger/index.html` endpoint.
   To run the container in any other mode override `ASPNETCORE_ENVIRONMENT` variable with `--env` flag.

3. Open `localhost:<YOUR_LOCAL_PORT>/swagger` and enjoy the app!

### Run manuall (Locally)
1. Clone the repository
2. Set real values instead of ${} and ${} in appsettings.json

   In case of using gmail account DO NOT PASS gmail account's password. You need to generate an applications password. More detailes here: https://stackoverflow.com/a/32457468/13255956
   
3. Build docker image with a command
   
   `$ docker build -t genesis-case-task:latest .`
   
4. Run built image with a command

   `$ docker run -p <YOUR_LOCAL_PORT>:80 genesis-case-task:latest`
   
   By default, the container is running in `Development` mode and has a swagger UI at the `swagger/index.html` endpoint.
   To run the container in any other mode override `ASPNETCORE_ENVIRc
 5. Open `localhost:<YOUR_LOCAL_PORT>/swagger` and enjoy the app!
 
## What can be improved

1. Use tasks schedulers to send emails by timespans
2. Use another serializer to reduce files size. For example, MsgPack or binary
