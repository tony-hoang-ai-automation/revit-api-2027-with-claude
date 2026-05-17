# Shared Parameters và Extensible Storage

> Priority: **P2**  
> Source PDF: `Revit 2014 Platform API Developers Guidelines`  
> Source page hits: 5, 10, 24, 56, 60, 89, 128, 169, 315, 316, ... (+9 trang khác)
> Version note: Nội dung dựa trên Revit API 2014; khi áp dụng cho Revit 2025-2027 cần đối chiếu API docs hiện hành.

## Tóm tắt
Shared Parameters dùng cho dữ liệu BIM hiển thị/schedule; Extensible Storage dùng lưu schema custom ẩn/riêng cho add-in.

## Khái niệm chính
- Shared parameter cần definition file, group, external definition và binding.
- Instance/type binding quyết định dữ liệu nằm ở instance hay type.
- Extensible Storage lưu entity theo schema GUID trên element.
- Dữ liệu public cho người dùng nên là parameter; dữ liệu nội bộ nên cân nhắc storage.

## Class/API cần nhớ
`DefinitionFile`, `ExternalDefinitionCreationOptions`, `BindingMap`, `InstanceBinding`, `TypeBinding`, `Schema`, `Entity`

## Workflow
- Tạo/đọc definition file
- Tạo definition
- Chọn categories
- Bind parameter
- Set parameter hoặc Entity

## Lỗi thường gặp
- GUID/name trùng
- Binding sai category
- Lạm dụng hidden storage
- Không version schema

## Ví dụ Revit API thực tế
Lưu mã QA public bằng shared parameter; lưu metadata scan nội bộ bằng Extensible Storage schema versioned.

## Infographic
- PNG: `../04-infographics-png/25-shared-parameters-extensible-storage.png`
- Prompt: `../03-infographic-prompts/25-shared-parameters-extensible-storage.md`
