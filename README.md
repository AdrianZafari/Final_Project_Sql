# Final_Project_Sql

## PLEASE UPDATE THE ALL INSTANCES OF CONNECTION STRING TO THE DIRECTORY YOU HAVE THE LOCAL DATABASE (.mdf) SAVED IN, OTHERWISE IT WILL NOT WORK

A fast way of doing this is by pressing `CTRL + F` in VS and searching by `UseSqlServer` to find all instances this appears. I cannot guarantee that the string formatting is consistent so I leave that in your hands. 

It's also important to update the connection string being used in the frontend (MAUI Blazor) `GUI_project`. You can do that by finding `Final_Project_Sql\GUI_project\wwwroot\appsettings.json` and updating the connection string there. 


### How to use
- The Application is pretty self contained and should be rather clear on how to use based on the instructions provided in the different pages. 
- Create an Employee first then create a Project. 
- Delete an Employee referenced in a Project and the backend will get upset with you (sorry). 
- I wrote warnings, but it's on you to follow them.  

As an aside:

I made an API with some instructions from ChatGPT as to what the main steps are for writing an API and what libraries must be used; it wouldn't connect to my frontend. The API is functional and could connect to the Database, unfortunately, setting it up with MAUI Blazor was rather hectic and wound up going nowhere. That's why there's a random API just sitting there in the directory, figured I might as well include it. 
