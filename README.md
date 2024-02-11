# Prusa Push Notif Dispatcher
Push status updates from your Prusa 3D printer right to your phone!

## Pushover
You will need an account at [Pushover](https://pushover.net/) to be able to run this. Be sure to also create an application and get the associated `API Token/Key` as well.

## Settings
Once you create an account and have both a `User Key` and an `API Token/Key`, you need to pass them in at runtime. The settings are:

* PushoverUserKey
* PushoverAppKey
* PrinterUsername (optional, defaults to maker)
* PrinterApiKey
* PrinterUrl
* PollRateInSeconds (optional, defaults to 5)

## Deploying
Simply run with the necessary environment variables set and the image specified.
```bash
docker run -itd --restart unless-stopped \
--env Settings__PushoverUserKey='abcd1234' \
--env Settings__PushoverAppKey='abcd1234' \
--env Settings__PrinterApiKey='abcd1234' \
--env Settings__PrinterUrl='192.168.1.50' \
ghcr.io/rickdgray/prusapushdispatcher:main
```

## Building
Projects in .NET have an unusual folder structure, so when building we must both specify the context to be at the root of the solution, but also specify the location of the `dockerfile`.
```bash
docker build -f .\PrusaPushDispatcher\Dockerfile .
```

## Debugging
You can edit the `launchSettings.json` file with your secrets. Then set the startup dropdown to "Docker" so that Visual Studio will create a container. This will allow you to debug with a local container.
```json
{
  "profiles": {
    "Docker": {
      "commandName": "Docker",
      "environmentVariables": {
        "Settings__PushoverUserKey": "abcd1234",
        "Settings__PushoverAppKey": "abcd1234",
        "Settings__PrinterApiKey": "abcd1234"
      }
    }
  }
}
```