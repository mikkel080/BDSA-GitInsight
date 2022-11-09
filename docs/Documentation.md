# Documentation

<!-- This is a comment, write your notes in this structure -->

## Introduction

<!-- Maybe we should switch over the Overleaf/LaTeX but for now this will do-->

### C\#

The program is run with arguments that is then parsed to determine which of the two modes to run.

<!-- Write text outside this comment, and remember to follow the structure / expand as needed -->

#### Mode 1 Frequency

static void FrequencyMode(IEnumerable<Commit> list, String prefix = "")

Take a list of commits that is fetched using LibGitToSharp, and a custom prefix can be included though it has a default of an empty string.

#### Mode 2 Author
static void AuthorMode(IEnumerable<Commit> list)

Take a list of commits that is fetched using LibGitToSharp

### Database

### Tests

The tests run against a repo that is bundled in the Tests project, this makes the test consistent and not dependant on outside directories.

### GitHub?
