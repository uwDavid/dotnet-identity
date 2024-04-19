# .NET Identity Examples

Code examples for .NET Identity using Razor pages.

To run the application:

```bash
make api
make app
```

## API

We want our WebApp_UnderTheHood to call Web_API using JWT token scheme.
Requests to test API endpoints are located in `/http` folder

## App

Some authorization scheme is implemented at Web App Login
We app calls the Web API `/weatherforeast` via token authorization.
