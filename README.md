# Reservation Api

## Required Software/Setup

- .Net 6 SDK: https://dotnet.microsoft.com/download/visual-studio-sdks
- Visual Studio 2022: https://visualstudio.microsoft.com/downloads/
- PostgresSql:  https://www.postgresql.org/download/windows/
    - Be sure to install PgAdmin 4
    - For the database superuser (postgres) give any password.
    - Once installed, Create a test account.
        - Launch pgAdmin 4
        - Navigate to Servers => Login/Group Roles => right clieck and select create
            - Name: test
            - Password: test
            - All privilages enabled 
- Download and open the BookingService: https://github.com/JohnSteeleUrban/reservation-api
- Open the solution with Visual Studio 2022.
- Right click the solution in the Solution Explorer and click *Restore Nuget Packages*
- Run the *Reservation.Api* Profile.


## Notes
The application will automatically create the database in your local Postgres db so long as the above instructions have been followed and the difault server port has not been changed from 5432.

The Browser will open on a Swaggar page so that you can test and see where I broke stuff or did not test due to limited time.

Design
------
The idea behind the app was to sepatate any current and potential business logic or validation from the data layer.  I would usually use the FluentValidation to validate all requests but left that out for now. All business logic is in the ReservationService.  I also went with a service because in my opinion adding the repository pattern is overkill is small CRUD apps.  Also, EF implements the repository and unit of work pattern under the hood already, or at least something similar.  If this were the start of a monolith or larger api, I'd implement the Repository pattern and a more DDD design with IAggregate/Root notation where appropriate as well as lock down entities and keep behavior in each entity. 
Also, it's sometimes easier in Macro/large microservices to separate out Infrastrure/services/etc. into different libraries.  I've done this in the past but can't justify it here.

Data
----
I mulled over how to represent Availability.  Another way that would take up lots of space (disk space is pretty cheap, though) but maybe simplify some queries is to separate each entry into a 15 time slot so it matches 1 to 1 with Reservations.  I thought of this as i got going and would probably make the query for reservations slots available to clients much easier.  In hindsight I would have done that and it would probably actually work (i'm assuming there's problems with that endpoint, I haven't had time to test).

ORM
----
I'd also add CQRS patterns with .net's Mediator library.  This makes it simple to add command handlers and queries.  If the load on this api was going to be super high, I might user Dapper for the queries because you can easily write raw queries and I've heard in the past it's slightly more efficient than EntityFramework.  Though with the sql notation available in Entity Framework, I'm not sure that's true anymore ¯\_(ツ)_/¯

Like to add
---------
- Logging.  If this were going to the cloud, like say, Azure, I'd add App Insights to it so you can perform traces and alerts.  I'd need to add actual logging in the app, probably using Serilog.
- Tests.  Lots of unit tests.
- Specific Dto's for requests and responses.  I tried to keep it simple for now.

  
Thanks for the oportunity!!

With that, enjoy!
