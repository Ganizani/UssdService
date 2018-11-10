# UssdService
Ussd Service Engine Responsible with providing USSD engine. 
This is a .net solution of a client/server application where by you can build any USSD Campaign

## Main components:
* UssdService:
* UssdCommon:
* DAL:
* Entities:
* BLL:
* Processors:
  ### To add a new processor
    - you need to create a new solution under the processor folder(Point the build to the bin\ forlder of the UssdService Solution)
    - Link the processor to a campaign in the the database
    - Mark it start and end date and sit down and relax. the UssdService will pick it up and run it for you.
