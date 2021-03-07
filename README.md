# Autopal.Funda.Top10
 * [Purpose of this project](#purpose-of-this-project)  
 * [Problem definition](#problem-definition)
 * [Using the command interface](#using-the-command-interface)
 * [Technical Summary](#technical-summary)
	 * [How to build](#how-to-build)
	 * [How to run](#how-to-run)
	 * [Project structure](#projects-structure)
		  * [Autopal.Funda.Top10.Console](#console)
		  * [Autopal.Funda.Top10.Business](#business)
		  * [Autopal.Funda.Top10.Connector](#connector)
	 * [Testing strategy](#Testing-strategy)
	 * [Third party usages](#Third-party-usages)
	 * [Legacy libraries](#legacy-libraries)
 * [Known issues](#known-issues)
## Purpose of this project
This is a command line application serving as an interface to retrieve stats from funda api based on input parameter. 
This project is implemented as a technical assignment part of job interview with [Funda](https://www.funda.nl/).
## Problem definition 
Determine which makelaar's in Amsterdam have the most object listed for sale. Make a table of the top 10. Then do the same thing but only for objects with a tuin which are listed for sale. For the assignment you may write a program in any object oriented language of your choice and you may use any libraries that you find useful. 
Tips: 
 * If you perform too many API requests in a short period of time (>100
   per minute), the API will reject the requests. Your implementation
   should mitigate (avoid) errors and handle any errors that occur, so
   take both into account.
 * Different people will look at your solution, so make sure it is easy
   to understand and go through.
 * We don't expect solutions to have a comprehensive test suite, but we
   do expect solutions to be testable.
 * One of the criteria that our reviewers value is separation of
   concerns.
## Using the command interface
```
Autopal.Funda.Top10.Console command line interface to get stats about agents by their offer count in a specific region.

Usage: Autopal.Funda.Top10.Console [-r|--region REGION] [-n|--N AGENT] [-h|--help HELP] 

Command line interface to Autopal.Funda.Top10.Console application

Available options:
  -h,--help                Show this help text
  -r,--region REGION       Mandatory parmeter which defines the {REGION} that will be 	
						   investigated.	
  -n,--N N				   The number of top {N} agents to retrieve.
                           this must be an integer. Default is 10.
 ```
 ## Technical summary
 This project is written in C# by the repository owner [Aytaç Utku TOPAL](https://github.com/putko/). Latest framework of its time is used ([.net5](https://docs.microsoft.com/en-us/dotnet/core/dotnet-five)). 
 ### How to build
 To build applications written in [.net5](https://docs.microsoft.com/en-us/dotnet/core/dotnet-five) you need to install the development tools package, i.e. the SDK. To run them – the development package.

You’ll find both here:  [https://dotnet.microsoft.com/download/dotnet/5.0]

**Using Msbuild**
You can use  _MSBuild.exe_  to build specific targets of specific projects in a solution.
Navigate to the folder of the project within your favourite terminal and run the command below:
msbuild SlnFolders.sln
```
msbuild Autopal.Funda.Top10.sln
or
dotnet build Autopal.Funda.Top10.sln
 ```

#### What programs can be used?

 - #### Visual Studio
The most used application for developing .net applications: [https://visualstudio.microsoft.com/]
Make sure that you have a version that supports [.net5](https://docs.microsoft.com/en-us/dotnet/core/dotnet-five)

 - #### Visual Studio Code
Simple, cross-platform code editor, developed open-source. It does not have as many integrations as Visual Studio, but it is lightweight and thanks to free extensions, it can be easily adapted to your needs. Certainly, however, it works comfortably with smaller projects and with websites. I use it when I’m working with React.js, for example.
- #### Other Options
Because .NET 5 comes with CLI we can create, build and run the project, and also execute tests with simple console commands. For writing code, any editor will do such as [JetBrain Rider](https://www.jetbrains.com/rider/). It is a great, cross-platform IDE, that has a built-in [Resharper](https://www.jetbrains.com/resharper/).

### How to run
To run the application, navigate to the folder that contains executable Autopal.Funda.Top10.Console.exe file and run the below command:
```
dotnet build Autopal.Funda.Top10.Console.exe -r "amsterdam" -n 10
 ```
#### Running in visual studio
Simply press F5 or in [Visual studio](https://visualstudio.microsoft.com/) .
Note: Don't forget to set the command line arguments in project properties.
### Project structure
Solution consists of 3 projects. Namely:
 - [Console](#Autopal.Funda.Top10.Console)
 - [Business](#Autopal.Funda.Top10.Business)
 - [Connector](#Autopal.Funda.Top10.Connector)
#### Autopal.Funda.Top10.Console
Console application is the host of the project. It initialize the dependency container, interact with arguments and makes the business call.
- Program.cs 
Main purpose of this class is to provide starting point to the application. It interacts with the ServiceCaller class with a parameter that is parsed from arguments.
- ConfigureExtensions.cs
Main purpose of this class is to configure dependency injection and make arrangements like automapper and logging. 
- ServiceCaller.cs
ServiceCaller class is the component that allows the separate business related information which is subject to be tested. Its concern is how input parameters will be mapped to business functions. In this case region and n parameters will be used to call related business function twice. One of the whole houses and one for the ones with a garden.

This project is only tested via Unit test. ServiceCaller class is used by mocking the related business component. 
#### Autopal.Funda.Top10.Business
Responsibility of this project is connecting to Funda Services using ***Autopal.Funda.Top10.Connector*** component. 
This project consist of one business class and its interface and one model that is used to return. This class uses the connector to retrieve data.
Data retrieval is made with pages because of the API limitation. Initial call is made to learn how many page exist. Then series of calls are made to get all offers that are listed in the given ***Region***.
After the data retrieval, data is grouped by their agents and offer count is obtained. 
Then agents are sorted using offer count value and first ***N*** number of them are return as business model ***Agent***
#### Autopal.Funda.Top10.Connector
This components main concern is isolate the external API logic from business. It consists of two sub component, namely FundaConnector and FundaClient.

 - **FundaConnector**

This component orchestrate calls made to Funda API. This is not the component that makes the actual requests but the one that ensure every requests succeed. This component also makes the model conversion from the model retrieved from Funda API to the internal connector model. 

**API Rate Limit**

This component faces some challenges. 
First of them is API rate limit. Funda API, as all other free APIs out there uses the request throttling. That means that after certain amount of call in a certain period of time, server denies your request and returns 429. 

To avoid this issue, general solution online is to make 1 request in a certain time frame. For example, if the rate limit is 100 call in 60 seconds, then basically if we made a call in every 0.6 seconds, we will not hit the limit. 

Unfortunately, this simple solution works but not effective enough. With this approach, one client will finish 200 requests in 2 minutes.

Fortunately, there is a better solution. Interpretation of rate limit of a one call in every 0.6 seconds is misleading. The correct interpretation should be 100 calls in 60 seconds. 

To be able to use this approaches, FundaConnector uses a third party library called **RateGate** of ***Jack Leitch***. In this approach SemaphoreSlim object is used to arrange 100 slots. And every new call is allowed once a occupied slot is released. 
In this approach, 100 calls is made as soon as the function is running which takes approximately 10 seconds to finish all of them because of them being asynchronous. Then RateGate limits any calls including the retries, till the completion of the 60 seconds. Then another 100 calls is made which takes another 10 seconds. So all operation takes 70 seconds instead of 120 in the worst case scenario.

**Failures and Retries**
Making remote calls are always subject to failure during the transportation or in case of the server failure of remote server. 
To provide stability, connector makes 10 retries with increasing break between each retry. 
To be able to do that, Connector uses famous third party library called **Poly**. 

- **FundaClient**

This components is used purely for retrieving the data from remote server. It creates the requests, executes it and serializes it using **Newtonsoft.json**. 

### Testing Strategy

 - **Unit Tests**

There are 4 tests projects in the solution. Each layer in the solution has 1 unit test project. 
Third party library called **Moq** is used for mocking the underlying dependencies. 
Only business layer has coverage enough to satisfy any CI process. 
The other two project only has one unit test just to prove that components are testable. 

 - **Integration Tests**
 Only the business layer has integration test with only one case with happy flow. 
### Third Party Libraries
 - Automapper
Used for converting Funda API models to Connector components model.
 - Poly 
 Used for failure tolerance
 - RateGate
 Limits the amount of call that is made for a certain period of time including the retries. 
 - Moq
  Used to provide isolation in unit tests by mocking the underlying dependencies. 
 - Newtonsoft.Json
Used to deserialize incoming json to FundaAPI model. 
### Legacy Libraries
There are some classes that are imported but neither documented nor tested. These are trusted libraries that I have been using externally. For example, ApiClientBase class is based on generated code of NSwag open api client generator. But since there is no nuget package, instead of delivering unknown dlls, I choose to import classes and trust them because they are tested in different projects of mine. 

## Known Issues

 - There is one unit test in console application that checks what output is written in console and compare it with the expected results. It passes in my local environment but for some security reason in github it fails. 
 - Parallel execution of data retrieval in page could be possible but the returned data amount were not consistent. Apparently, a threading issue occurs. But in limited time, I tried to use ConcurrentBag collection and lock objects but none of them fixes the issue. So, I reverted the code to a serial loop. 
 - Libraries that are not imported for this project are neither documented nor tested in this solution. 
 - Client libraries are generated by Aanbod.svc?wsdl=wsld0 URL. But in .net5, either support was limited or there has been an issue that I don't mind to investigate. I used HttpRequest instead with my implemented library. Further investigation would be nice to be able to consume directly the svc service.  
