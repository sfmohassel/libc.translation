# libc.translation

A library to help you implement translation using json files to replace .resx files in .net standard projects

## Breaking and important changes:

- From version 3.0.0 onwards the library only depends on [System.Text.Json](https://www.nuget.org/packages/System.Text.Json).
- From version 5.0.0 onwards `LocalizationSource` is renamed to `JsonLocalizationSource` to better reflect the purpose of the class. From this version we support nested objects (look below the example)

## Understand the code:

You can read this [article](https://medium.com/@saeidfarahi/c-how-to-use-a-simple-form-of-i18n-instead-of-resource-files-resx-files-26eec9460a88) to understand how the code works.

## Why we need this?

Have you ever got tired of using .resx files in visual studio?
<br/>
Do you want to have i18n json files for localizing your projects?
<br/>
Do you hate sattelite assemblies alongside your application files like me?
<br/>
If yes, then this is for you :-)

## How does it work?

- Add [this nuget package](https://www.nuget.org/packages/libc.translation/)
- Then we need an `ILocalizationSource` instance. This instance can be created using the default `JsonLocalizationSource` class.
<br/>
`JsonLocalizationSource` class enables us to load a json file containing all our translations from three different sources:

  - A file on disk
  
  ```csharp
  ILocalizationSource source = new JsonLocalizationSource(new FileInfo("<path to json file>"), PropertyCaseSensitivity.CaseSensitive);
  ```
  
  - An stream object
  
  ```csharp
  Stream stream = new FileInfo("<path to json file>").OpenRead();
  ILocalizationSource source = new JsonLocalizationSource(stream, PropertyCaseSensitivity.CaseSensitive);
  ```
  
  - An embedded json file in assembly
  
  ```csharp
  Assembly assembly = Assembly.GetExecutingAssembly();
  string resourceId = "libc.translations.tests.embedded.json";
  ILocalizationSource source = new JsonLocalizationSource(assembly, resourceId, PropertyCaseSensitivity.CaseSensitive);
  ```

- Then we need an `ILocalizer` instance. This instance can be created using the default `Localizer` class.
<br/>
`Localizer` class enables us to pass an `ILocalizationSource` object and a __fallback culture__ (which defaults to "en" value).

```csharp
ILocalizer localizer = new Localizer(source, "en");
```

- Now there are some methods to obtain desired translation text using a culture and a key.

## Example

Suppose we have created an i18n json file and a localizer with English as the fallback language like this:

```csharp
ILocalizer localizer = new Localizer(source, "en");
```

```json
{
  "ar": {
    "InvalidInput": "إدخال غير صالح",
    "UnknownError": "خطأ غير معروف {0}",
    "home-page": {
      "title": "أهلا بك",
      "body": {
        "text": "بعض النصوص"
      } 
    } 
  },
  "de": {
    "InvalidInput": "Ungültige Eingabe",
    "UnknownError": "Unbekannter Fehler {0}",
    "home-page": {
      "title": "Willkommen",
      "body": {
        "text": "Etwas Text"
      }
    }
  },
  "en": {
    "InvalidInput": "Invalid input",
    "UnknownError": "Unknown error {0}",
    "home-page": {
      "title": "Welcome",
      "body": {
        "text": "Some text"
      }
    }
  }
}
```

- Set current thread's culture (this is actually not needed):

```csharp
CultureInfo.CurrentCulture = new CultureInfo("ar");
```

- Get a translation text for a culture and key. If the text for the given culture is not found, fallback culture is used:

```csharp
var text = localizer.Get("es", "InvalidInput");
// text is (English is our fallback culture): Invalid input
```

- Get a translation text for a key and thread's current culture. If the text for the given culture is not found, fallback culture is used:

```csharp
var text = localizer.Get("InvalidInput");
// text is (thread culture is "ar"): إدخال غير صالح
```

- Get a formatted translation text for a culture and key. If the text for the given culture is not found, fallback culture is used:

```csharp
var text = localizer.GetFormat("de", "unknownerror", "!!!");
// text is: Unbekannter Fehler !!!
```

- Get a formatted translation text for a key and thread's current culture. If the text for the given culture is not found, fallback culture is used:

```csharp
var text = localizer.GetFormat("unknownerror", "!!!");
// text is: خطأ غير معروف !!!
```

- Get a nested translation text for a key:

```csharp
var text = localizer.Get("de", "home-page.title");
// text is: Willkommen

text = localizer.Get("de", "home-page.body.text");
// text is: Etwas Text
```

## Contribution

This repository belongs to the community and will always appreciate any contribution
