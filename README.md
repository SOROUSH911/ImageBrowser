# README

# ImageBrowser

The project was generated using the [Clean.Architecture.Solution.Template](https://github.com/jasontaylordev/ImageBrowser)
version 8.0.5.

## Setup before build

### AWS S3

you need to setup a S3 Bucket 

### Secret Parameters

Here is a  sample appsettings.json that you can either set as environment variable to your docker containers:

- Web
    - Needs all the configuration
- QueueApp
    - ProducitonConnection
    - SolrServerAddress
    - SolrCollectionName
    - AmazonConfiguration
- DbUp
    - only Needs ProducitonConnection

```json
{
"ProducitonConnection": "Server=ServerAddress;Port=5432;Database=postgres;User Id=postgres;Password=your_password;SSL Mode=Require;Trust Server Certificate=true",
"SolrCollectionName": "default_core",
"SolrServerAddress": "http://solr:8983/solr",
"WebClientSecret": "YourSecret",
"TokenConfiguration": {
    "SecretToken": "",
    "VarifiedSecretUrl": "", //it's this value is not used you can leave it as null or just a https://localhost:8000
    "EncRefPassword": ""
},
  "AmazonConfiguration": {
    "DefaultBucket": "your_bucket_name"
  },

}
```

## Run

There are two docker-compose.yml file and docker-compose.override.yml for debugging. 

<aside>
💡 If you’re looking to run this using Ioc on kubernetes cluster or aws cloudformation stack go to [IocImageBrowser](https://github.com/SOROUSH911/IocImageBrowser)

</aside>

if you are running the application on your local using [Docker](https://www.docker.com/) follow this steps:

### 1.  Set up aws cli to connect to your aws for ssm parameter store or use Azure Vault

We are going to use aws ssm parameter store or you can skip this by configuring the Secrets in your envrionment varaible or simply inside your appsettings.json go Secret Parameters section.

But you can change the secret manager in  `web/Program.cs` 

   

```csharp
//If you're using Azure Vault uncomment this
//builder.Services.AddKeyVaultIfConfigured(builder.Configuration);
builder.Services.AddSsmParametersIfConfigured(builder.Configuration);
```

Use [this](https://docs.aws.amazon.com/cli/latest/userguide/getting-started-quickstart.html) tutorial to set it up

<aside>
💡 We are passing the credentials file as a volume to our docker containers which are using the services like S3 and SSM parameter store. a better practice would be not to use your root account profile

</aside>

### 2. add your credentials to your containers

 The configuration file is located at **`~/.aws/config`** on Linux or macOS, and at **`C:\Users\USERNAME\.aws\config`** on Windows.

Replace this in docker-compose.yml like this (in my case I’m using Windows):

```yaml
version: '3.4'

services:

  dbup:
    image: ${DOCKER_REGISTRY-}dbup
    build:
      context: .
      dockerfile: DbUp/Dockerfile
    restart: "no"
    volumes:
     - C:\Users\USERNAME\.aws\credentials:/home/app/.aws/credentials:ro
```

As we mentioned in the Services Section. We also have an Postgresql I personally am using a PostgreSQL RDS. But you have to setup a database or you can simply create a docker container for that and our DbUp Console app and Seed functions will make sure the database is initialized correctly uncomment the postgresql section in docker-compose.yml like so:

```yaml
version: '3.8'
services:
  postgres:
    image: postgres:latest
    container_name: my-postgres-db
    environment:
      POSTGRES_DB: mydatabase
      POSTGRES_USER: myuser
      POSTGRES_PASSWORD: mypassword
    ports:
      - "5432:5432"

```

in this case our ConnectionString would be 

```json
"ProductionConnection": "Server=postgres;Port=5432;Database=mydatabase;User Id=myuser;Password=mypassword;SSL Mode=Require;Trust Server Certificate=true"
```

<aside>
💡 Make sure to replace ProductionConnection inside your [Secret Parameters](https://www.notion.so/README-ec219da4001241bcb7f9ccdbbf96da22?pvs=21).

</aside>

### 3. Run the project

Now we simply run the docker containers using the command:

```bash
docker compose -f docker-compose.yml -f docker-compose.yml \
  run 
```

<aside>
💡 Note: if we are running on local we need to make sure to use docker-compose.yml since it mocks the behaviour of ssl.

</aside>

# Services

## Clean Architecture and CQRS

## Authentication and Authorization

## File Upload and Large object pattern

## Distributed System Event bus Using Masstransit and rabbit mq

## Solr: Search Engine

## DbUp: keeping the databases up to date

## Optical Character Recognition

## Code Styles & Formatting

The template includes [EditorConfig](https://editorconfig.org/) support to help
maintain consistent coding styles for multiple developers working on the
same project across various editors and IDEs. The
**.editorconfig** file defines the coding styles applicable
to this solution.

## Test

The solution contains unit, integration, and functional tests.

To run the tests:

```bash
dotnet test
```

## Help

- To learn more about the project you can email me [Here](mailto:soroush.salari2023@gmail.com) you can find additional guidance, request new
- To report a bug just open an issue.