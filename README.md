**This library is updated fairly frequently. [Releases][7] created before making significant changes.**

# NomadCode.Azure

`NomadCode.Azure` is a very simple library to make working with data in Azure Mobile Apps easier.

# Installation

To make it easy to use anywhere, `NomadCode.Azure` is a [shared project][0] with only a few files.  

To use in your project, you can either, clone, [download][1], etc. and reference your local copy in your project.    

Or, if you're using Git, you can add it as a submodule with the following command:

```shell
cd /path/to/your/projects/root

git submodule add https://github.com/colbylwilliams/NomadCode.Azure NomadCode.Azure
```

# Use

NomadCode.Azure only has three classes: `AzureClient`, `AzureEntity`, and `AzureEntityController`.

- `AzureClient` is the class you'll use to interact with your Azure Mobile Apps data, auth, etc.

- `AzureEntity` should be used as the base class for any type stored Azure Mobile Apps.

- `AzureEntityController` should be used as the base class for server-side Controllers that correspond to subclasses of `AzureEntity`.  This will automatically handle all routing and CRUD operations, and allows you to share your `AzureEntity` subclasses between the client and server.


## Data Types

`NomadCode.Azure` can be referenced by both your client and server apps which allows the client and server to share data objects and simplifies setting up the server-side controllers that facilitate data interactions.

**Your data types should inherit from `AzureEntity`**

```C#
public class User : AzureEntity
{
	public string Username { get; set; }
}
```

**Your table controllers should inherit from `AzureEntityController`**

```C#
public class UserController : AzureEntityController<User, MyAppContext> { }
```
This will take care of implementing the CRUD routes, there's no need to add anything to the body of your controller - what you see in the example above is all you need.


## Offline sync

`NomadCode.Azure` supports using Azure Mobile Apps with and without offline sync.

**To enable offline sync, add `OFFLINE_SYNC_ENABLED` to the preprocessor directives of any consuming projects.** 


## Initialization

Before performing any CRUD opporations, you must initialize the `AzureClient`.  Initialization is a bit different depending on whether or not your app will support offline sync.   


### _With_ Offline sync

If your app supports offline sync, you initialize the `AzureClient` by first calling `RegisterTable` on **each type** you will use with Azure Mobile Apps. Then call `InitializeAzync`, passing in the url of your Azure Mobile Apps instance.

**Note: These types should inherit from `AzureEntity`**

```C#
if (!AzureClient.Shared.Initialized)
{
	AzureClient.Shared.RegisterTable<User> ();
	AzureClient.Shared.RegisterTable<Vendor> ();
	// ...the rest of your types
	
	await AzureClient.Shared.InitializeAzync ("https://{your-app}.azurewebsites.net");
}
```

### _Without_ Offline sync

If your app does not support offline sync, simply call `Initialize`, passing in the url of your Azure Mobile Apps instance like in the expample below:

```C#
AzureClient.Shared.Initialize ("https://{your-app}.azurewebsites.net");
```

## CRUD

Once the `AzureClient` is initialized, you can use the methods below to perform CRUD operations on your data:

```C#
AzureClient client = AzureClient.Shared;

// only with offline sync
await client.SyncAsync<User> ();                                // pushes local and pulls remote changes


await client.GetAsync<User> ("12345");                          // returns User.Id == "12345

await client.GetAsync<User> ();                                 // returns the all user objects

await client.GetAsync<User> (u => u.Age < 34);                  // returns users where age < 34

await client.FirstOrDefault<User> (u => u.Name == "Colby");     // returns first user with name "Colby"


await client.SaveAsync (user);                                  // inserts or updates new user

await client.SaveAsync (new List<User> { user });               // inserts or updates each user in a list


await client.DeleteAsync<User> ("12345");                       // deletes User with User.Id == "12345

await client.DeleteAsync (user);                                // deletes the user

await client.DeleteAsync (new List<User> { user });             // deletes each user in a list

await client.DeleteAsync<User> (u => u.Age < 34);               // deletes all users where age < 34
```

## Authentication

`NomadCode.Azure` also supports handeling the [client-managed][6] or [server-managed][2] authentication flow for your app, incuding storing relevant items in the keychain and silently re-authenticate users later.     

To Authenticate a user using either the client or server-managed flow, you must first set the `AzureClient.Shared.AuthProvider` to the [`MobileServiceAuthenticationProvider`][3] you want to use.

```C#
// defaults to WindowsAzureActiveDirectory
AzureClient.Shared.AuthProvider = MobileServiceAuthenticationProvider.Google;
```

### _Client-Managed_ Authentication

You authenticate using the client-managed flow by calling `AuthenticateAsync` and passing in up to two tokens, depending on the `AuthProvider` you use:

```C#
if (!AzureClient.Shared.Authenticated)
{
    // try authenticating with an existing stored token
    var result = await AzureClient.Shared.AuthenticateAsync ();
	    
	if(result.Authenticated)
	{
		// re-authetication was successful
	}
	else
	{
		// login using the provider sdk...
		await AzureClient.Shared.AuthenticateAsync (token, authCode);
	}
}

```

### _Server-Managed_ Authentication

You authenticate using the server-managed flow by calling `AuthenticateAsync`, passing a `UIViewController` on iOS and an `Activity` on Android:

```C#
if (AzureClient.Shared.AuthenticateAsync (this))
{
    // CRUD
}
```

### Re-Authentication

Regardless of which athorization flow you use, once a user is authenticated, `NomadCode.Azure` will always attempt to silently re-autheticate the user automatically.  However, in the case that the client is unable to re-autheticate the user, the `AzureClient` will invoke its `AthorizationChanged` event.  Your app should subscribe to this event and request your user to login again.

```C#
// client-managed:
AzureClient.Shared.AthorizationChanged += (sender, authorized) => 
{
	if (!authorized)
	{
		// login again using the provider sdk...
		await AzureClient.Shared.AuthenticateAsync (token, authCode);
	}
};

// server-managed:
AzureClient.Shared.AthorizationChanged += (sender, authorized) => 
{
	if (!authorized)
	{
		// login again passing a UIViewController or Activity
		await AzureClient.Shared.AuthenticateAsync (this)
	}
};
```

### Loging Out

You can logout the user by simply calling `LogoutAsync`.

```C#
AzureClient.Shared.LogoutAsync();
```

This will also purge all local data.


### Local Authentication

Finally, when your debugging your server locally, the local instance won't be able to handle the authenticaiton requests from the client.  To get around this, you can use a uri for authenticaiton that is different from the app's uri.  To do this, set the value of `AzureClient.Shared.AlternateLoginHost` to the uri you want to use for authentication (likely your live app).


# About

Created by [Colby Williams][5]. 


## License

Licensed under the MIT License (MIT).  See [LICENSE][4] for details.

[0]:https://developer.xamarin.com/guides/cross-platform/application_fundamentals/shared_projects/
[1]:https://github.com/colbylwilliams/NomadCode.Azure/archive/master.zip
[2]:https://docs.microsoft.com/en-us/azure/app-service-mobile/app-service-mobile-dotnet-how-to-use-client-library#serverflow
[3]:https://msdn.microsoft.com/library/azure/microsoft.windowsazure.mobileservices.mobileserviceauthenticationprovider(v=azure.10).aspx
[4]:https://github.com/colbylwilliams/NomadCode.Azure/blob/master/LICENSE
[5]:https://github.com/colbylwilliams
[6]:https://docs.microsoft.com/en-us/azure/app-service-mobile/app-service-mobile-dotnet-how-to-use-client-library#clientflow
[7]:https://github.com/colbylwilliams/NomadCode.Azure/releases