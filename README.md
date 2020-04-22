## My Final Web-Project at SoftUni using ASP .NET Core
https://letssport.azurewebsites.net

regular user: **test** pass: **test123**    
admin user: **admin** pass: **admin123** 

* LetsSport make it simple to discover sport activities happening nearby, as well as the people that want to participate in them. 
* You can simply join an existing event or create a new one.
* If you are Sport-Arena Manager you can also add it to our database and gain more customers.

### Description and Futures:
	Users futures:
		- register as regular user or arena admin, log and logout.
		- update their profile info, add/change/delete avatar image.
		- create, update, cancel event.
		- invite other users to event.
		- send rental requests to Arenas.
		- chat with coo-users in event details page
		- report bad users.
		- join, and leave other events.
		- change their status for receiving invitations for events.
		- contact app-admin via contact-form
	Arena Admin futures:
		- register as arena-admin
		- has all futures as regular user
		- update arena info, add/change/delete main image and other images.
		- create Arena
		- receive rental requests
		- approve, deny, cancel events
		- change arena status
	App Admin futures:
		- add/update new countries, cities and sports
		- monitor, update, events and arenas
		- receive and handle reports for bad users

```diff
+ In User area is implemented filtration for all events and arenas for better interaction.
+ In Admin area is implemented filtration for all countries, cities, sports, evens, arenas, reports. 
+ Live-time chat integrated in event details page for logged users.
+ Pop-up messages for most actions.
+ Pagination implemented in all listing pages.
+ Third-party authentication include - register with Facebook or Google account.
```
## Using ASP.NET Core 3.1 Template by : [Nikolay Kostov](https://github.com/NikolayIT)

### Technologies used:
* .NET Core 3.1
* ASP .NET Core 3.1
* SignalR
* Entity Framework Core 3.1
* xUnit
* Moq
* MyTested.AspNetCore.Mvc
* JavaScript
* jQuery
* Bootstrap
* HTML 5
* CSS
* FontAwesome
* Cloudinary
* Google ReCaptcha
* SendGrid
* ipinfo

### Dependencies:
* [Cloudinary](https://www.cloudinary.com/)
* [SendGrid](https://www.sendgrid.com/)
* [ipinfo](https://www.ipinfo.io/)

## Screen Shots:

### Home Page:
![alt text](https://res.cloudinary.com/dziee8jfp/image/upload/v1587216948/ProjectScreenShots/1.home_page_mh10cs.jpg)

### Logged User Index Page:
![alt text](https://res.cloudinary.com/dziee8jfp/image/upload/v1587216948/ProjectScreenShots/2.home_logged_s9u5jb.jpg)

### Event Details Page for Logged User with Chat-Room:
![alt text](https://res.cloudinary.com/dziee8jfp/image/upload/v1587216948/ProjectScreenShots/3.event_details_yrl4q1.png)

### Arena Details Page for Arena Administrator:
![alt text](https://res.cloudinary.com/dziee8jfp/image/upload/v1587216950/ProjectScreenShots/4.arena_zar7zf.png)

### Arena Details Page:
![alt text](https://res.cloudinary.com/dziee8jfp/image/upload/v1587216948/ProjectScreenShots/5.koldr_iwefee.jpg)
	
### Arena Events Page:
![alt text](https://res.cloudinary.com/dziee8jfp/image/upload/v1587218316/ProjectScreenShots/6.arena_events_midne2.jpg)

### Administration Page:
![alt text](https://res.cloudinary.com/dziee8jfp/image/upload/v1587218442/ProjectScreenShots/7.admin_bvkzau.jpg)	





