# Core Concepts - Revit API

Tài liệu này gom các khái niệm nền tảng cần nắm trước khi viết add-in thực tế.

## Tổng Quan Revit API Platform
Revit API là lớp .NET cho phép add-in đọc dữ liệu BIM, tạo/chỉnh sửa element, mở rộng UI và tự động hóa workflow trong Revit.

API trọng tâm: `Autodesk.Revit.DB`, `Autodesk.Revit.UI`, `IExternalCommand`, `IExternalApplication`, `Document`, `Element`

## Cài Môi Trường, SDK, Reference DLL, .addin
Một add-in Revit cần project Class Library, reference đúng `RevitAPI.dll`/`RevitAPIUI.dll`, manifest `.addin` và cấu hình debug mở `Revit.exe`.

API trọng tâm: `RevitAPI.dll`, `RevitAPIUI.dll`, `.addin`, `AddInId`, `FullClassName`, `VendorId`

## Hello World ExternalCommand
`IExternalCommand` là entry point đơn giản nhất: user bấm lệnh, Revit gọi `Execute`, command trả về `Succeeded`, `Failed` hoặc `Cancelled`.

API trọng tâm: `IExternalCommand`, `Execute`, `ExternalCommandData`, `Result.Succeeded`, `TaskDialog`, `TransactionAttribute`

## ExternalCommand vs ExternalApplication vs DB ExternalApplication
Ba loại entry point giải quyết ba nhu cầu: lệnh một lần, bootstrap UI/event khi Revit khởi động, hoặc xử lý database không UI.

API trọng tâm: `IExternalCommand`, `IExternalApplication`, `IExternalDBApplication`, `OnStartup`, `OnShutdown`, `UIControlledApplication`

## Add-in Manifest, Loading, Debugging
Manifest là hợp đồng để Revit tìm DLL và class entry point; debug hiệu quả cần build đúng path, mở đúng Revit version và đặt breakpoint.

API trọng tâm: `RevitAddIns`, `AddIn Type`, `Assembly`, `AddInId`, `FullClassName`, `VendorDescription`

## Ribbon, Panel, PushButton, SplitButton, StackedButton
Ribbon API giúp đưa workflow add-in vào UI Revit bằng tab, panel, button, split button, radio group và contextual help.

API trọng tâm: `CreateRibbonTab`, `CreateRibbonPanel`, `PushButtonData`, `SplitButtonData`, `RibbonItem`, `ContextualHelp`

## Application, UIApplication, Document, UIDocument
Application đại diện môi trường Revit; Document là model file; UIDocument là cầu nối thao tác UI như selection và active view.

API trọng tâm: `Application`, `UIApplication`, `ControlledApplication`, `Document`, `UIDocument`, `ActiveUIDocument`

## Element Essentials: Element, ElementId, Category, Type/Instance
Element là đơn vị dữ liệu cốt lõi trong Revit; muốn làm API vững phải phân biệt instance/type, category, id, unique id, level, location và parameters.

API trọng tâm: `Element`, `ElementId`, `ElementType`, `Category`, `BuiltInCategory`, `Location`

## Selection: Current Selection, PickObject, PickObjects, PickPoint
Selection API xử lý cả selection hiện tại và prompt người dùng chọn element, face, edge hoặc point trong giao diện Revit.

API trọng tâm: `UIDocument.Selection`, `Selection.GetElementIds`, `PickObject`, `PickObjects`, `PickPoint`, `ObjectType`

## Selection Filter: ISelectionFilter
`ISelectionFilter` giới hạn thứ user có thể chọn, giúp workflow chính xác hơn và giảm lỗi input.

API trọng tâm: `ISelectionFilter`, `AllowElement`, `AllowReference`, `ObjectType.Face`, `Reference`, `PlanarFace`

## FilteredElementCollector Foundation
`FilteredElementCollector` là công cụ chính để lấy element từ Document hoặc từ một View, kết hợp class/category/filter để có tập dữ liệu cần xử lý.

API trọng tâm: `FilteredElementCollector`, `OfClass`, `OfCategory`, `WherePasses`, `ToElements`, `ToElementIds`

## Filtering Nâng Cao: Class, Category, Rule, LINQ, Bounding Box, Intersection
Advanced filtering kết hợp quick filters, slow filters, parameter rules, bounding box và intersection để tìm đúng phần tử mà vẫn giữ hiệu năng.

API trọng tâm: `ElementClassFilter`, `ElementCategoryFilter`, `ElementParameterFilter`, `BoundingBoxIntersectsFilter`, `ElementIntersectsElementFilter`, `LogicalAndFilter`

## Transactions: Transaction, SubTransaction, TransactionGroup
Mọi thay đổi model cần Transaction hợp lệ; TransactionGroup gom nhiều thao tác, SubTransaction chia nhỏ rollback trong một transaction lớn.

API trọng tâm: `Transaction`, `SubTransaction`, `TransactionGroup`, `TransactionMode.Manual`, `FailureHandlingOptions`, `Commit`

## Roadmap Học Revit API Từ Beginner Đến Production Add-in
Roadmap học hiệu quả bắt đầu từ add-in lifecycle, document/element/parameter, sau đó selection/filtering/transaction, rồi geometry và domain-specific APIs.

API trọng tâm: `IExternalCommand`, `FilteredElementCollector`, `Transaction`, `Parameter`, `GeometryElement`, `Rebar`
