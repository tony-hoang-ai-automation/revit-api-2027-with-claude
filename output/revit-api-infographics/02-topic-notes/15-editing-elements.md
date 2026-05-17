# Editing Elements: Move, Copy, Rotate, Mirror, Array, Delete, Pinned

> Priority: **P1**  
> Source PDF: `Revit 2014 Platform API Developers Guidelines`  
> Source page hits: 3, 9, 52, 64, 97, 98, 99, 100, 101, 106, ... (+3 trang khác)
> Version note: Nội dung dựa trên Revit API 2014; khi áp dụng cho Revit 2025-2027 cần đối chiếu API docs hiện hành.

## Tóm tắt
Editing API thao tác element bằng transform và document operations; cần kiểm tra pinned, constraints, groups và transaction.

## Khái niệm chính
- `ElementTransformUtils` xử lý move/copy/rotate/mirror cho nhiều element.
- Delete trả về tập ElementId bị xóa, có thể nhiều hơn input do dependency.
- Pinned/constraints/group membership có thể chặn chỉnh sửa.
- Array/group mirror tạo element mới nên cần quản lý id kết quả.

## Class/API cần nhớ
`ElementTransformUtils`, `MoveElement`, `CopyElement`, `RotateElement`, `MirrorElement`, `Document.Delete`, `Pinned`

## Workflow
- Collect ids
- Check pinned/group
- Start transaction
- Apply transform
- Capture new/deleted ids
- Commit

## Lỗi thường gặp
- Xóa dependency ngoài ý muốn
- Mirror đổi handedness family
- Move element pinned
- Group member không sửa trực tiếp

## Ví dụ Revit API thực tế
Copy một dãy family instance theo vector tầng, sau đó gán lại parameter Mark cho từng instance mới.

## Infographic
- PNG: `../04-infographics-png/15-editing-elements.png`
- Prompt: `../03-infographic-prompts/15-editing-elements.md`
