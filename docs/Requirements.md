# Requirements

## Functional Requirements

* The program must read a path to a local directory containing a Git repo
* The program must be able to collect all commits, author names, and author dates from a Repo
* The program must have two modes, commit frequency mode, and commit author mode
* Commit frequency mode must list the number of commits per day
* Commit author mode must list the number of commits per day per author
* The program must be able to write to standard output
* The program must store results of analysis in a database
* The database must be updated to the most current analysis result
* The database must store information about the analyzed repositories and their state
* If the current state of a repository is the same as the stored one the analysis should be ommited and the output should be generated from the already stored data directly

## Non-functional Requirements

* The program must be written in C#
* The program must run using dotnet
* The documentation must be written in english
* Diagrams in the documentation must be written using UML
* The development uses CI/CD principles
* The development uses an Agile and SCRUM-Like style
* The program must be easy to change and maintain
* The program must have automated test to make correctness probable
* The test suite of the program must be kept up to date
* The program should use one or more design patterns
