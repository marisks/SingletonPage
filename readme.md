# SingletonPage

SingletonPage is a library of extensions for EPiServer CMS which allows to easily load the page which is a single instance of a type.

## Installing

The Current version does not have _NuGet_ package, but as it contains only three files - [DefaultContentReferenceCache.cs](src/SingletonPage/DefaultContentReferenceCache.cs), [Extensions.cs](src/SingletonPage/Extensions.cs), and [IContentReferenceCache.cs](src/SingletonPage/IContentReferenceCache.cs), those can be copied over to your project.

## Usage

Loading the page of a type by parent _ContentReference_.

```
var testPage1 = ContentReference.StartPage.GetSingletonPage<TestPage>();
```

Loading the page of a type by a parent page.

```
var startPage = _contentLoader.Get<StartPage>(ContentReference.StartPage);
var testPage2 = startPage.GetSingletonPage<TestPage>();
```
