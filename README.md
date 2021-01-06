# libc.translation
A library to help you implement translation using json files to replace .resx files in .net standard projects

## Breaking Changes:
From version 3.0.0 onwards the library only depends on [System.Text.Json](https://www.nuget.org/packages/System.Text.Json).

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
1- Add [this nuget package](https://www.nuget.org/packages/libc.translation/)
2- Then we need an `ILocalizationSource` instance. This instance can be created using the default `LocalizationSource` class.
<br/>
`LocalizationSource` class enables us to load a json file containing all our translations from three different sources:
- A file on disk
```csharp
ILocalizationSource source = new LocalizationSource(new FileInfo("<path to json file>"));
```
- An stream object
```csharp
Stream stream = new FileInfo("<path to json file>").OpenRead();
ILocalizationSource source = new LocalizationSource(stream);
```
- An embedded json file in assembly
```csharp
Assembly assembly = Assembly.GetExecutingAssembly();
string resourceId = "libc.translations.tests.embedded.json";
ILocalizationSource source = new LocalizationSource(assembly, resourceId);
```

3- Then we need an `ILocalizer` instance. This instance can be created using the default `Localizer` class.
<br/>
`Localizer` class enables us to pass an `ILocalizationSource` object and a __fallback culture__ (which defaults to "en" value).
```csharp
ILocalizer localizer = new Localizer(source, "en");
```

4- Now there are some methods to obtain desired translation text using a culture and a key.
- Suppose we have create an i18n json file like this:
```json
{
  "ar": {
    "InvalidInput": "إدخال غير صالح",
    "UnknownError": "خطأ غير معروف {0}"
  },
  "fa": {
    "InvalidInput": "خطا در اطلاعات ورودی",
    "UnknownError": "خطای نامشخص {0}"
  },
  "en": {
    "InvalidInput": "Invalid input",
    "UnknownError": "Unknown error {0}"
  }
}
```
- Set current thread's culture (this is actually not needed):
```csharp
CultureInfo.CurrentCulture = new CultureInfo("ar");
```
- Get a translation text for a culture and key. If the text for the given culture is not found, fallback culture is used: (key is case-insensitive)
```csharp
var text = localizer.Get("fa", "InvalidInput");
// text is: خطا در اطلاعات ورودی
```
- Get a translation text for a key and thread's current culture. If the text for the given culture is not found, fallback culture is used: (key is case-insensitive)
```csharp
var text = localizer.Get("InvalidInput");
// text is (if thread culture is "ar"): إدخال غير صالح
```
- Get a formatted translation text for a culture and key. If the text for the given culture is not found, fallback culture is used: (key is case-insensitive)
```csharp
var text = localizer.GetFormat("en", "unknownerror", "!!!");
// text is: Unknown error !!!
```
- Get a formatted translation text for a key and thread's current culture. If the text for the given culture is not found, fallback culture is used: (key is case-insensitive)
```csharp
var text = localizer.GetFormat("unknownerror", "!!!");
// text is: خطأ غير معروف !!!
```

## Contribution

This repository belongs to the community and will always appreciate any contribution
