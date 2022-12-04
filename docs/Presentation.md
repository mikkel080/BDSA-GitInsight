# GitInsight - Group 1: Zero

`emno`, `amdh`, `mbia`, `ehel`, `hast`, `rafa`

## Validation: Value for Customer

## Verification: Sample of Tests

* Unit
* Integration
* End-to-end :(
* User test :(
* Performance :(

### Unit Test

```C#
[Fact]
public void analysisReturnsSomethingAtAll()
{
    var forkNames = program.forkAnalysis("itu-bdsa", "lecture-code");

    forkNames.Should().NotBeNull();
    forkNames.RepositoryIdentifiers.Count().Should().BeGreaterThanOrEqualTo(9);
    forkNames.RepositoryIdentifiers.Should().Contain(new RepositoryIdentifier("jskoven", "lecture-code"));
}
```

### Integration Test

```C#
[Fact]
public void CommitFrequency()
{
    var output = program.Run("Miniim98", "Assignment00_BDSA_2022");
    output.Should().Contain("2022-09-04T00:00:00");
    output.Should().Contain("\"Count\": 3");
}
```

## Development

* Change Management
* Pull Requests
* Code Review & peer approval
* Workflows
* Continuous Integration / continuous deployment

## Software Process

* Iterative processes

## Design Patterns

* Factory
* Repository
* Concurrency: `async`

## Architectural Patterns

* Client server Architecture
* Layers: Frontend & Backend
* Backend: API, domain logic, data access, data persistance

## Design Principles

<!--
How do you make sure that you implemented the right application?
That is, provide a mapping from functional and non-functional requirements in text form to respective test cases.

Show what kind of tests are contained in your test suites and demonstrate what they are testing.

Did you apply any design patterns in your application?
If yes, describe where you applied which design pattern and what problem it solves, i.e., the reason for its application.
You choose if you present design patterns in code over via diagrams that illustrate them.

Did you apply any architectural patterns in your application?
If yes, describe where you applied which architectural pattern and describe the reason for its application.
Likely, architectural patterns in your application are best illustrated using suitable diagrams.

Did you follow any design principles?
If yes, show case some instances of [them]

Present UML diagrams for aspects of your applications that you want to highlight during your presentation, e.g., the structure of your applications, important interactions of certain classes, component, or sub-systems, etc.
-->