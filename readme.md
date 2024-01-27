## Convert Swagger to Document

Swagger2document can easily convert swagger.json to Word, HTML, Excel, or any supported file format.

#### Only Supported on the Windows Platform

swagger2document is a program based on net8 but it references the [ReportViewerCore.NETCore](https://github.com/lkosson/reportviewercore) package.

### Usage

`dotnet swagger2document.dll d:/mywebapi.json 3 b`

`dotnet swagger2document.dll https://127.0.0.1:7291/swagger/v1/swagger.json 3 a`

arg[1]: json source. localfile or url(http/https) is supported.

arg[2]: File Format.

* 1=PDF
* 2=EXCELOPENXML
* 3=WORDOPENXML *default*
* 4=HTML4.0
* 5=MHTML

arg[3]: Document Template.

* [a](./assets/0.png)
* [b](./assets/1.png) *default*
