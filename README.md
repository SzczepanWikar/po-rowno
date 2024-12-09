# poRowno

Engineering thesis

<p style="text-align: center">
    <img src="./po-rowno-app/src/assets/icon/logo.svg"/>
<p>

## Prerequisites

### Environments

- [Node.js](https://nodejs.org)
- [.NET 8](https://dotnet.microsoft.com)
- [Angular](https://angular.dev)
- [Ionic](https://ionic.io)

### Databases

- [Microsoft SQL Server](https://www.microsoft.com/pl-pl/sql-server/sql-server-downloads)
- [EventStoreDB](https://www.eventstore.com)

### Other

- Email account
- [PayPal](https://developer.paypal.com/home/)

## Setup

Create database.

In `Backend` directory:

```console
dotnet restore
```

In `Backend\API` directory create `appsettings.Development.json` or fill `appsettings.json`. Base on `exampple-appsettings.json`.
In `Backend` directory:

```console
dotnet ef database update --project QueryModel --startup-project API
```

In `po-rowno-app` directory:

```console
npm i
```

## Run app

Backend

```console
cd Backend
dotnet run --project=.\API
```

Mobile app

In `po-rowno-app` directory:

```console
ionic s
```
