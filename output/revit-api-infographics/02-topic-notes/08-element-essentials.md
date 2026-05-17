# Element Essentials: Element, ElementId, Category, Type/Instance

> Priority: **P0**  
> Source PDF: `Revit 2014 Platform API Developers Guidelines`  
> Source page hits: 2, 9, 55, 56, 64, 70, 71, 143, 183, 190, ... (+2 trang khác)
> Version note: Nội dung dựa trên Revit API 2014; khi áp dụng cho Revit 2025-2027 cần đối chiếu API docs hiện hành.

## Tóm tắt
Element là đơn vị dữ liệu cốt lõi trong Revit; muốn làm API vững phải phân biệt instance/type, category, id, unique id, level, location và parameters.

## Khái niệm chính
- Instance là phần tử đặt trong model; Type định nghĩa kiểu dùng chung.
- `ElementId` ổn trong session/model, `UniqueId` hữu ích khi lưu liên kết ngoài.
- Category giúp lọc và hiểu ý nghĩa BIM của element.
- Một số element có Location, một số chỉ có geometry hoặc data.

## Class/API cần nhớ
`Element`, `ElementId`, `ElementType`, `Category`, `BuiltInCategory`, `Location`, `UniqueId`

## Workflow
- Lấy ElementId
- GetElement
- Kiểm tra Category/Class
- Đọc type
- Đọc location
- Đọc parameters

## Lỗi thường gặp
- Nhầm Type với Instance
- Assume element nào cũng có Location
- Lưu ElementId ra ngoài lâu dài không kiểm chứng

## Ví dụ Revit API thực tế
Từ Wall instance lấy `WallType` qua type id để đọc cấu tạo và thông số dùng chung.

## Infographic
- PNG: `../04-infographics-png/08-element-essentials.png`
- Prompt: `../03-infographic-prompts/08-element-essentials.md`
