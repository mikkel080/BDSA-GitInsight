# Requirement to Test Map

| Functional Requirement | Test|
| ------- | ------- |
| The program must read local Git repository files | NA |
|The program must read remote repositories hosted on GitHub| Tested in ModeTest.cs|
| The program must maintain a local, secret, version of analyzed remote repositories| NA |
| The program must be able to collect all commits, author names, and author dates from a repository| Yes, tested to an extent in ModeTest.cs |
|The program must support three analyses: commit frequency, commit author, and fork | Yes, tested in ModeTest.cs |
|Commit frequency must list the number of commits per day| Yes, tested to an extent in ModeTest.cs |
| Commit author must list the number of commits per day per author| Yes, tested to an extent in ModeTest.cs
| Fork analysis must list all public immediate forks of the remote repository| Tested in ForkTest.cs
|The program must store and update analysis results in a persistent database| Tested in CommitRepositoryTests.cs, AuthorRepositoryTests.cs, and RepoRepositoryTests.cs
|The program must use the database to expedite processing of repositories| NA
|The program must expose a RESTful WEB API, that supplies JSON Objects| Not finished
|The API must supply the results of commit frequency analysis, commit author analysis, and fork analysis| Not tested
|The program must contain a frontend web-application| NA
|The frontend must enable the user to analyze remote repositories| NA
|The frontend must display illustrations that gives relevant insights into the repository| NA
|The program must be able to list every repository that forks from the analyzed repository | Tested in ForkTest.cs

NOTE: The non-functional requirements are not testable/tested. Some are ensured via GitHub actions
