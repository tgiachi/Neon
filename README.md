# Neon.HomeControl

Similar to [HomeAssistant](http://home-assistant.io)  and [OpenHAB](https://www.openhab.org/), but made with .net core and ❤️ in Florence, Italy

## Why

My goal is to explore the IoT world and create a completely versatile custom home management platform. The intent is not to compete with other systems, but create a good programming sandbox

## Contributors

Thanks to: Bart Kardol (@bkardol) !

## Help request

I'm looking for people to help me with the project, please contact me!

## Features

- .NET Core 3.0  
- Scripts Engine in JavaScript (using [Jint](https://github.com/sebastienros/jint) 3.0.0 beta)
- UTF-8 support
- Metric and imperial measurements support
- Routines (set of instructions)
- Automatic device detection and discovery (via Bonjour/mDns protocol)
- Docker native support
- Simple config System (pluggable and in YAML format)
- Multiple NoSQL database connectors: [LiteDB](https://www.litedb.org/),  [MongoDB](https://docs.mongodb.com/ecosystem/drivers/csharp/), [JsonFlatFile](https://github.com/ttu/json-flatfile-datastore)
- Plugins System: API library on NuGet
- Community driven development
- Low memory comsumption (about 60 ~ 70 MB)
- WebSocket server for dispatch events
- OpenAPI / Swagger / ReDoc Api documentation (/redoc /swagger endpoints)
- Isolated filesystem (secrets keys are stored encrypted)
- States management (object and boolean states)
- Metrics (with InfluxDB/Kibana/others support)
- Telegram bot for notifications (you can also execute js code! )
- WakeOnLan function (you can turn on your system!)
- Alarm system for morning wake up 😂
  
## Actual implemented components

- MQTT Client
- Weather (Dark sky API)
- Spotify API
- Sonoff-Tasmoda  
- Philip Hue (local and remote API)
- Panasonic Air Conditioner API
- OwnTracks (via MQTT)
- ~~Nest Thermo (disabled, because the api are changing)~~
- Chromecast (thanks @kakone)
- mDns/Bonjour devices listeners 
- Broadlink device
- Plex hook receiver
- Sonarr  ([here](https://github.com/Sonarr/Sonarr))
- Radarr ([here](https://github.com/Radarr/Radarr))

## Running

### Docker

```shell
docker pull tgiachi/neon:dev
docker run -it --rm  -p 5000:5000 -v /home/user/neon:/neon tgiachi/neon:dev
```

## Default config file

 You can download [here](https://github.com/tgiachi/Neon/blob/master/Neon.WebApi/default-neon-config.yaml) and rename in `neon-config.yaml`

## Script engine example

To make event management easier, I created a very simple system of rules

### Add named rule

```javascript
add_rule("test_rule", "Weather", "entity.Temperature > 30", function(entity)  
     log_info("test", "It's hot!")
end)
```

### States

```javascript
var isTommyAtHome = {
  is_home: true
}
set_state("i_m_at_home", isTommyAtHome)
```

### Commands system

```javascript
// Turn on philip hue lights group
send_command('light_status', 'Bedroom', 'ON')
```

### Routines

```javascript
// Add new routine
add_routine('my_test_routine', function() {
  log_info("Hi from Routine")
});

// Call routine
exec_routine('my_test_routine')
```

### Alarm system

```js
add_alarm("test_alarm", 07,32, function()
  log_info("It's time to wake up!");
end
)
```

## Notify action via Telegram

```js
  setTimeout(() => {
    telegram('Hi my name is Neon');
  }, 5000);
```

## Wake On Lan

```js
  wake_on_lan('00:00:00:00:00:00')
```
