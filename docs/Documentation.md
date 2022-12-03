# Documentation

<!-- This is a comment, write your notes in this structure -->

## Introduction

This is a ever evolving application that facilitates analysis of git repositories.
The analysis pertains mostly to commits, who made them and when, and who has made forks of the repositories.
It supports exclusively repositories that are hosted on GitHub.com.
The application exposes a REST API, and a web page where the insights and analysis results can be accessed.

## Starting the software locally

To start the API and consume it locally, some steps are required.
Firstly, the database needs to be started.
It is hosted in a Docker container, and its instructions can be found under [Database](#database).

Secondly, the API needs to be started using dotnet, by running the Gitinsight.API project.

Finally, to consume the API make a call to the application on `localhost:7199` using https.
This can be done using a browser, cURL, or similar.
The API takes arguments via the URL, in the form `organization-name/repository-name`.

## Architecture

<!-- Describe the Architecture, both of the systems themselves, and between them -->

A Class diagram, detailing most of the the classes of the program, is shown below.
![Class diagram](img/ClassDiagramW45.png)

An overview of the packages that the program consist of, is shown below.
![Package Diagram](img/PackageDiagram.png)

An activity diagram of the backend is shown below. It details the backends process to download and analyze remote repositories.
![Activity Diagram](img/ActivityDiagram.png)

## RESTful WEB API

The WEB API is build on REST principles, mainly supporting `GET` requests.
The API accepts a GitHub hosted repository, and returns a JSON object with the analysis results.

The analysis consists of three different parts, called Frequency, Author and Fork.
Frequency analysis describes the amount of commits that are made on a given day.
That is, the frequency of commits to the repository over time.
Author analysis, on the other hand, details the amount and distribution of commits for the different authors.

Fork analysis, uses the public GitHub API to list all direct Forks of the repository.
At the moment it only lists direct forks and not forks of forks, and so on.

The REST API exposes the analysis to the API caller, packaged in a labeled JSON object.

### JSON Object

An example JSON Object that has been build by the API can be seen [here](JSONExample.md).
It is a JSON Object that contains information about the repository, and the analysis described previously.

### GitHub API Key

The program needs to interface with the official GitHub REST API.
GitHub requires that users of their API are authenticated to use the API.
As such, users of the program need to supply a GitHub personal access token to the program.
This can be done in one of two ways.
It can be supplied as a user-secret, or as an environment variable, or both.
If both exist, the user-secret will be used.
In all cases, the key of the pair must be `GITHUBAPI`, and the value must be the token.
The token can be generated on GitHub.
See their [documentation](https://docs.github.com/en/authentication/keeping-your-account-and-data-secure/creating-a-personal-access-token).

## Authentication and access

We use Azure AD B2C to authenticate the user and GitHub as the identity provider. This means that to access our web page the user have to login using their GitHub account.

## Web page and illustrations

## Database

To start the database, run the following command in a terminal of you choice, with Docker installed.

`docker run -e "ACCEPT_EULA=Y" -e "MSSQL_SA_PASSWORD=<YourStrong@Passw0rd>" -p 1433:1433 -d mcr.microsoft.com/mssql/server:2022-latest`

The database contains information about the results from the analysis, the GitHub Repository, it's commits and the authors.
We use Data Transfer Objects to transfer data to and from the database.
The database is a relational database, build using Microsoft SQL Server.
It runs locally in a docker container, and therefore needs a local installation of docker to run.
It connects to the database  using a connection string that are saved locally using dotnet user-secrets.

## Tests

A test suit is included with the program.
For testing with remote repositories, real active repos are used.
This enables easy testing, since the repositories already exists, but requires that they are not removed, made inaccessible or significantly changed.
The tests are mainly unit tests, with some integration tests where necessary.
There are, currently, no end-to-end tests of the program.

## Quality management
