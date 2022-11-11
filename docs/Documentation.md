# Documentation

<!-- This is a comment, write your notes in this structure -->

## Introduction

<!-- Maybe we should switch over the Overleaf/LaTeX but for now this will do-->

### C\#

The program is run with arguments that is then parsed to determine which of the two modes to run.

<!-- Write text outside this comment, and remember to follow the structure / expand as needed -->

#### Mode 1: Frequency

`static void FrequencyMode(IEnumerable<Commit> list, String prefix = "")`

Takes a list of commits that is fetched using `LibGitToSharp` and a custom prefix can be included though it has a default of an empty string.

#### Mode 2: Author

`static void AuthorMode(IEnumerable<Commit> list)`

Take a list of commits that is fetched using LibGitToSharp

### Database

### Tests

A test suit is included with the program.
A sample git directory is included with the program tests, to facilitate testing and verification.

### GitHub?
