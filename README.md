RaceDay is a comprehensive, cloud-based platform designed to revolutionize event administration for South African road races, runs, and cycling competitions, including major events like the Soweto Marathon and Cape Town Cycle Tour, as well as local fitness gatherings. This system consolidates scattered registration data by offering a unified solution that includes a RESTful API, a dynamic web portal built with the Model-View-Controller architecture, and cloud-based storage for event-related media, thereby replacing manual spreadsheets.

Event administrators, referred to as Organisers, possess the capability to establish, modify, and remove events, customize event categories, review all participant registrations, and document race outcomes and rankings. Participants, on the other hand, can register or log in, explore upcoming events, choose specific categories to enter, check their individual registration status, and review their past race results and standings.

The initial set of project deliverables, located in the `/docs` folder, includes:
* A detailed Entity Relationship Diagram (`RaceDay_ERD.pdf`) illustrating six entities, their respective keys, and relationships.
* A methodical API endpoint specification (`RaceDay_API_Endpoint_Plan.md`) outlining plans for authentication, user profiles, event management, category configuration, participant enrollment, and results processing.
* A production-ready T-SQL script (`RaceDay_Database.sql`) that defines the database schema with clean table structures, enforces data integrity through constraints, and populates the database with representative South African sample data.

To set up the database using SQL Server Management Studio (SSMS):
1. Launch SSMS and establish a connection to your SQL Server.
2. Open the `RaceDay_Database.sql` file from the `docs` directory.
3. Run the entire script. This action will generate the `RaceDayDB` database, create all six tables, implement the defined constraints, and populate it with realistic sample data.

Continuous integration and continuous deployment (CI/CD) are automatically verified by GitHub Actions with every code push, ensuring the repository adheres to the expected structure. A visual representation of the CI/CD build status is provided in `docs/ci_screenshot.png`.

A video presentation of the project is available at the unlisted YouTube link: 