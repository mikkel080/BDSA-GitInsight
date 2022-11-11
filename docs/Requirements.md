# Requirements

## Functional Requirements

* The program must read a path to a local directory containing a Git repository
* The program must read a path to a remote GitHub repository
* The program must maintain a local, secret, version of the remote repository that is updated or created when needed
* The program must be able to collect all commits, author names, and author dates from a repository
* The program must have two modes, commit frequency mode, and commit author mode
* Commit frequency mode must list the number of commits per day
* Commit author mode must list the number of commits per day per author
* The program must be able to write to standard output
* The program must store results of analysis in a database
* The content of the database must be updated to the most current analysis result
* The database must store information about the analyzed repositories and their state
* The program must use the database to expedite processing of repositories
* The program must expose a REST / RESTful API
* The REST API must supply JSON Objects

## Non-functional Requirements

* The program must be written in C#
* The program must run on dotnet
* The program must use the `libgit2sharp` library to collect Git repository data
* The program must use one or more design patterns
* The program must use automated tests to ensure verification
* The program must be easy to change and maintain
* The documentation must be written in English
* Diagrams in the documentation must be written using UML
* The development must use CI/CD principles
* The development must use branch protection and pull requests to enable successful changes
* Changes must be approved by another team member
* The development team must use an Agile and SCRUM-Like style
* The test suite of the program must be kept up to date
* The programs dependencies must be kept up to date
