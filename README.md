[![Quality Gate Status](https://sonarcloud.io/api/project_badges/measure?project=nanoframework_nanoFramework.Device.OneWire&metric=alert_status)](https://sonarcloud.io/dashboard?id=nanoframework_nanoFramework.Device.OneWire) [![Reliability Rating](https://sonarcloud.io/api/project_badges/measure?project=nanoframework_nanoFramework.Device.OneWire&metric=reliability_rating)](https://sonarcloud.io/dashboard?id=nanoframework_nanoFramework.Device.OneWire) [![NuGet](https://img.shields.io/nuget/dt/nanoFramework.Device.OneWire.svg?label=NuGet&style=flat&logo=nuget)](https://www.nuget.org/packages/nanoFramework.Device.OneWire/) [![#yourfirstpr](https://img.shields.io/badge/first--timers--only-friendly-blue.svg)](https://github.com/nanoframework/Home/blob/main/CONTRIBUTING.md) [![Discord](https://img.shields.io/discord/478725473862549535.svg?logo=discord&logoColor=white&label=Discord&color=7289DA)](https://discord.gg/gCyBu8T)

![nanoFramework logo](https://raw.githubusercontent.com/nanoframework/Home/main/resources/logo/nanoFramework-repo-logo.png)

-----

# Welcome to the .NET **nanoFramework** 1-Wire&reg; Class Library repository

## Build status

| Component | Build Status | NuGet Package |
|:-|---|---|
| nanoFramework.Device.OneWire | [![Build Status](https://dev.azure.com/nanoframework/nanoFramework.Device.OneWire/_apis/build/status/nanoFramework.Devices.OneWire?repoName=nanoframework%2FnanoFramework.Device.OneWire&branchName=main)](https://dev.azure.com/nanoframework/nanoFramework.Device.OneWire/_build/latest?definitionId=15&repoName=nanoframework%2FnanoFramework.Device.OneWire&branchName=main) | [![NuGet](https://img.shields.io/nuget/v/nanoFramework.Device.OneWire.svg?label=NuGet&style=flat&logo=nuget)](https://www.nuget.org/packages/nanoFramework.Device.OneWire/) |

## 1-Wire&reg; bus

1-Wire&reg; it's a communication protocol, property of Maxim Semiconductor. You can read the technical details about it on [this guide](https://www.maximintegrated.com/en/design/technical-documents/tutorials/1/1796.html).

## .NET nanoFramework implementation

Our low level implementation of the 1-Wire communication uses an UART to achieve precise timing with the less possible burden on the MCU.
For that reason it requires an UART and shunting together it's RX and TX pins. Depending on the bus length and impedance it may be required connecting an external pull-up resistor to provide the necessary signalling for 1-Wire communication.

**Important**: If you're using an ESP32 device it's mandatory to configure the UART2 pins before creating the `OneWireHost`. To do that, you have to add a reference to [`nanoFramework.Hardware.ESP32`](https://www.nuget.org/packages/nanoFramework.Hardware.Esp32). In the code snippet below we're assigning GPIOs 21 and 22 to UART2. Also note that `UART2` it's referred as `COM3` in C#.

```csharp
////////////////////////////////////////////////////////////////////////
// Configure pins 21 and 22 to be used in UART2 (that's refered as COM3)
Configuration.SetPinFunction(21, DeviceFunction.COM3_RX);
Configuration.SetPinFunction(22, DeviceFunction.COM3_TX);
```

Take note, on some ESP32 development kits, the pins you're planning on using for UART2 could be used for internal purposes.
For example, development kits based on either ESP32-WROOM-32 or ESP32-WROVER-E can have the same pinouts and silkscreen. If the kit is based on ESP32-WROVER-E the GPIOs 17 and 16 are used to address its extended memory (PSRAM), and cannot be used for other purposes, event though they are present as external pins.
You can use any other GPIO pins that's free for UART2 pins using Configuration.SetPinFunction.

For other devices, like STM32 ones, there is no need to configure the GPIO pins. You have to find in the respective device documentation what are the UART pins used for 1-Wire.

## Usage examples

To connect to a 1-Wire bus and perform operations with the connected devices, one has to first instantiate the OneWireHost.

```csharp
OneWireHost _OneWireHost = new OneWireHost();
```

To find the first device connected to the 1-Wire bus, and perform a reset on the bus before performing the search, the following call should be made:

```csharp
_OneWireHost.FindFirstDevice(true, false);
```

To write a byte with the value 0x44 to the connected device:

```csharp
_OneWireHost.WriteByte(0x44);
```

To get a list with the serial number of all the 1-Wire devices connected to the bus:

```csharp
var deviceList = _OneWireHost.FindAllDevices();

foreach(byte[] device in deviceList)
{
    string serial = "";

    foreach (byte b in device)
    {
        serial += b.ToString("X2");
    }

    Console.WriteLine($"{serial}");
}
```

## Feedback and documentation

For documentation, providing feedback, issues and finding out how to contribute please refer to the [Home repo](https://github.com/nanoframework/Home).

Join our Discord community [here](https://discord.gg/gCyBu8T).

## Credits

The list of contributors to this project can be found at [CONTRIBUTORS](https://github.com/nanoframework/Home/blob/main/CONTRIBUTORS.md).

## License

The **nanoFramework** Class Libraries are licensed under the [MIT license](LICENSE.md).

## Code of Conduct

This project has adopted the code of conduct defined by the Contributor Covenant to clarify expected behaviour in our community.
For more information see the [.NET Foundation Code of Conduct](https://dotnetfoundation.org/code-of-conduct).

### .NET Foundation

This project is supported by the [.NET Foundation](https://dotnetfoundation.org).
