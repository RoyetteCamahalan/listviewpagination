# List View Paginator
Custom control derived from Listview which performs pagination for large lists.
###### *I love to hear suggestions/feedbacks from you. Kindly post issue if encountered or request new features. Thank You!*


###### *This library may not work on via nuget. If not, copy BootstrapPage.ascx to your project then register user control.*

```
<%@ Register TagName="Bootstrap" Src="BootstrapPage.ascx" TagPrefix="paginator"%>
```

## Version 1.0.3

### Installation
-Via **[Package Manager Console](https://docs.microsoft.com/en-us/nuget/consume-packages/install-use-packages-powershell)**
```
PM> Install-Package ListView.Pagination -Version 1.0.3
```
-Via **[Dot Net CLI](https://docs.microsoft.com/en-us/dotnet/core/tools/)**
```
> dotnet add package ListView.Pagination --version 1.0.3
```

#### Sample Output
![Sample Image](https://github.com/RoyetteCamahalan/listviewpagination/blob/main/assets/sampleimg.png?raw=true)

#### Supported data source types
- Datatable
- List (List<T>)

#### Properties
###### - pageSize
*(int)* Indicate number of items per page. Default is 10.
###### - ItemPlaceholderID
*(string)* Specify place holder ID inside LayoutTemplate. Not required if LayoutTemplate is empty.
###### - lengthChange
*(boolean)* Allow page size change *(10 | 25 | 50 | 100)*. Default value is `true`.
###### - allowSearch
*(Coming Soon)*

#### Method
###### - Bind
Accepts parameter of type *object*. *object* must be a supported datasource types as indicated above.
    
    
#### Usage

Register assembly to page
    
```
<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" MaintainScrollPositionOnPostback="true" AutoEventWireup="true" CodeBehind="MainPage.aspx.cs" Inherits="Demo.Views.MainPage" %>    
        
<%@ Register Namespace="ListviewPaginator" Assembly="ListviewPaginator" TagPrefix="paginator" %>
```
Adding Control to page 
    
```
<paginator:Bootstrap ID="lstitems" runat="server" ItemPlaceholderID="itemPlaceHolder1" pageSize="20">
    <LayoutTemplate>
        <table class="table table-bordered table-hover dataTable dtr-inline responsive-table">
            <thead>
                <th class="text-center">Code</th>
                <th class="text-center">Name</th>
            </thead>
            <tbody>
                <asp:PlaceHolder runat="server" ID="itemPlaceHolder1"></asp:PlaceHolder>
            </tbody>
        </table>
    </LayoutTemplate>
    <ItemTemplate>
        <tr>
            <td class="text-center"><span><%# Eval("code") %></span></td>
            <td class="text-center"><span><%# Eval("name") %></span></td>
        </tr>
    </ItemTemplate>
</paginator:Bootstrap>    
```

Binding `DataSource`
```
lstitems.Bind(listOfItems);
```
