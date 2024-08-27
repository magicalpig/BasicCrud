# README

A sample ASP.NET API project built using C# 12 and .NET 8.0.

## Description

A web API that supports basic CRUD operations on a collection of classical musical compositions.
The project demonstrates the use of: 

- HTTP REST API
- Entity Framework for data modeling (e.g. one-to-many relationships), cascading deletes, and db migrations
- SQLite
- A database seeder for initial data
- AutoMapper for converting domain models to DTOs and vice versa
- configuration defined in appsettings.json and appsettings.Development.json
- Swagger
- Attributes from System.ComponentModel.DataAnnotations for validation and converting enum values to user-friendly strings and vice versa
- Dependency injection


## Usage

Once the project is running, you can perform CRUD operations against the following endpoints:

- `GET /api/Composition`: Retrieve all compositions. Or you can use query parameters to filter results. None of the parameters are case-sensitive. You can combine any number of parameters. The allowed query parameters are
  - ComposerName: A composer's name or partial name (e.g. "Bach" will match a composition whose composer is "Johann Sebastian Bach")
  - Format: A named format such as "Sonata" or "Concerto" or numeric code for a format such as 2 or 3 as defined by Enums\Format.cs
  - KeySignature: A named key signature such as "CMajor" or "BFlatMinor" or numeric code for a key signature such as 1 or 2 as defined by Enums\KeySignature.cs
  - KeySignatureDisplayName: A user-friendly name for a key signature such as "C Major" or "B♭ Major" as defined by the [Display] attributes found in Enums\KeySignature.cs
- `GET /api/Composition/{id}`: Retrieve a specific composition by its ID.
- `POST /api/Composition`: Create a new composition, possibly introducing a new composer. The request body needs to be JSON.
- `PATCH /api/Composition/{id}`: Update an existing item sending only those fields that need to be updated. The request body needs to be JSON.
- `DELETE /api/Composition/{id}`: Delete a specific composition by its ID.

## Future Ideas
- make controller lighter: right now the controller is doing too much. It should be refactored to delegate more work to services or possibly use MediatR
- 