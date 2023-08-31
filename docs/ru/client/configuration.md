# Клиент/Конфигурация

Можно настраивать поведение клиента через `JsonRpcClientOptionsBase`, переопределение свойств или методов, а также через замену внутренних сервисов.

> См. [Примеры](examples) с демонстрацией работы

## JsonRpcClientOptionsBase

### Url

> Значение по умолчанию: `null`

> Базовый URL для отправки HTTP запросов (настраивает внутренний HttpClient)

У каждого метода клиента есть перегрузка, принимающая `requestUrl` первым параметром - он будет добавлен к `JsonRpcClientOptionsBase.Url` для случаев, когда некоторые методы API доступны по другому адресу.

Есть два ограничения, чтобы не допустить неожиданного поведения при соединении частей URL:
 - `JsonRpcClientOptionsBase.Url` должен заканчиваться на `/`
 - `requestUrl` Не может начинаться со `/`

### Timeout

> Значение по умолчанию: `TimeSpan.FromSeconds(10)`

> Таймаут на отправку HTTP запросов (настраивает внутренний HttpClient)

## Свойства и методы

### UserAgent

> Значение по умолчанию: `"Tochka.JsonRpc.Client"`

> Значение HTTP заголовка User-Agent (настраивает внутренний HttpClient)

### DataJsonSerializerOptions

> Значение по умолчанию: `JsonRpcSerializerOptions.SnakeCase`

> `JsonSerializerOptions` используемые для сериализации поля `params` и десериализации полей `result` или `error.data`

В пакете `Tochka.JsonRpc.Common` определены `JsonRpcSerializerOptions.SnakeCase` и `JsonRpcSerializerOptions.CamelCase`.

Подробнее: [Сериализация](serialization).

### HeadersJsonSerializerOptions

> Значение по умолчанию: `JsonRpcSerializerOptions.Headers`

> `JsonSerializerOptions` используемые для сериализации и десериализации "заголовков" JSON-RPC: `id`, `jsonrpc`, и тд.

Не рекомендуется менять, так как объект "заголовков" запроса/ответа имеет фиксированный формат и не подразумевает каких-либо изменений.

Подробнее: [Сериализация](serialization).

### Encoding

> Значение по умолчанию: `Encoding.UTF8`

> Кодировка, используемая при отправке HTTP запросов

### Client

> Внутренний `HttpClient`, используемый для отправки HTTP запросов

Можно настраивать, чтобы добиться какой-то особой логики при отправке HTTP запросов.

### ParseBody(...)

> Логика десериализации тела HTTP ответа

Можно переопределить метод, если ответы от API нарушают протокол JSON-RPC или содержат дополнительную информацию.

Не рекомендуется менять, так как объект "заголовков" ответа имеет фиксированный формат.

### CreateHttpContent(...)

> Логика сериализации запросов и оборачивания их в `HttpContent` с кодировкой и заголовком Content-Type

Не рекомендуется менять, так как объект "заголовков" запроса имеет фиксированный формат.

### GetContent(...)

> Логика чтения содержимого `HttpResponseMessage`

## Services

### RpcIdGenerator

> Значение по умолчанию: `JsonRpcIdGenerator`

> Сервис для генерации `id` для запросов в перегрузках, которые не принимают аргумент `id`

Может быть заменен в DI для переопределения логики генерации `id`.