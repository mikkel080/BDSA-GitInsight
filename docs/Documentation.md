# Documentation

<!-- This is a comment, write your notes in this structure -->

## Introduction

This is a ever evolving application that facilitates analysis of git repositories.
It supports both repositories that are hosted locally and on GitHub.com.
The application exposes a WEB API, and a web page where the insights and analysis results can be accessed.

## Architecture

<!-- Describe the Architecture, both of the systems themselves, and between them -->

A Class diagram, detailing some of the the classes of the program, is shown below.
The diagram also shows one of the programs namespaces or packages.
![Class diagram](ClassDiagram.png)

## RESTful WEB API

The WEB API is build on REST principles, mainly supporting `GET` requests.
The API accepts a GitHub hosted repository, and returns a JSON object with the analysis results.

The analysis consists of two distinct modes, called Frequency and Author mode.
Frequency mode describes the amount of commits that are made on a given day.
That is, the frequency of commits to the repository over time.
Author mode, on the other hand, details the amount and distribution of commits for the different authors.

The REST API exposes both modes to the API caller, packaged in the same JSON object, with different keys.

## Web page and illustrations

## Database

<!-- Document what the database contains, and when it is updated. 
Also write that it is an in-memory database and is not persistent -->

## Tests

A test suit is included with the program.
A sample git directory is included with the program tests, to facilitate testing and verification.
For testing with remote repositories, real active repos are used.
This enables easy testing, since the repositories already exists, but requires that they are not removed, made inaccessible or significantly changed.

## Quality management
