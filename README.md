# BDSA-GitInsight

The goal of the project is to build an application called GitInsight that allows users to get insights over development in Git/GitHub repositories.

This project is a part of the course Analysis, Design and Software Architecture at IT University of Copenhagen.

## Group

Number: 1

Name: Zero

Members: `emno`, `amdh`, `mbia`, `ehel`, `hast`, `rafa`

## Group expectations

We expect to use about 6-10 hours a week on the project and we have a loose format, where members can skip meetings or take them online if needed, as long as they stay up-to-date on our discord.

## Documentation

For the Documentation of the program see [Documentation](docs/Documentation.md).
For an overview of the Development Goals and intended process see [Development Goals](docs/DevelopmentGoals.md).

## Run the software

To run this project you need docker installed, a personal GitHub Token saved in user-secrets in the GitInsight project, a connection string saved in user-secrets in the GitInsight.Api project.
To start the project open a terminal and run these commands:

```shell
make run-docker
make run-api
make run-blazor
```

Start with docker, wait 10 seconds and then run the last two.
Now you should be able to access the webpage on [https://localhost:7129](https://localhost:7129)
