# Requirements

## Functional Requirements

* The program must read local Git repository files
* The program must read remote repositories hosted on GitHub
* The program must maintain a local, secret, version of analyzed remote repositories
* The program must be able to collect all commits, author names, and author dates from a repository
* The program must support three analyses: commit frequency, commit author, and fork
* Commit frequency must list the number of commits per day
* Commit author must list the number of commits per day per author
* Fork analysis must list all public immediate forks of the remote repository
* The program must store and update analysis results in a persistent database
* The program must use the database to expedite processing of repositories
* The program must expose a RESTful WEB API, that supplies JSON Objects
* The API must supply the results of commit frequency analysis, commit author analysis, and fork analysis
* The program must contain a frontend web-application
* The frontend must enable the user to analyze remote repositories
* The frontend must display illustrations that gives relevant insights into the repository
* The program must be able to list every repository that forks from the analyzed repository

## Non-functional Requirements

* The program must be written in C#
* The program must use the `libgit2sharp` library to interface with repositories
* The program must use one or more design patterns
* The program must use automated tests to ensure verification
* The program must be easy to change and maintain
* The documentation must be written in English
* Diagrams in the documentation must be written using UML
* The documentation must be kept up to date
* The development must use CI/CD principles
* The development must use branch protection and pull requests to enable successful changes
* All changes to the program must be done using Pull Requests
* Changes must be approved by another team member
* The test suite of the program must be kept up to date
* The programs dependencies, including runtime, must be kept up to date
* The frontend must be written with .Net Blazor
* The program must not contain any API keys or other secrets
* All communication between users and the services must be encrypted
* All communication between the programs services must be encrypted
* Users and consumers of frontend web-application and RESTful API must be authenticated to access the service
