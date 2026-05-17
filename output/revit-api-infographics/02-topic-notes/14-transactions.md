# Transactions: Transaction, SubTransaction, TransactionGroup

> Priority: **P0**  
> Source PDF: `Revit 2014 Platform API Developers Guidelines`  
> Source page hits: 5, 39, 260, 271, 275, 283, 324, 325, 326, 327, ... (+10 trang khác)
> Version note: Nội dung dựa trên Revit API 2014; khi áp dụng cho Revit 2025-2027 cần đối chiếu API docs hiện hành.

## Tóm tắt
Mọi thay đổi model cần Transaction hợp lệ; TransactionGroup gom nhiều thao tác, SubTransaction chia nhỏ rollback trong một transaction lớn.

## Khái niệm chính
- `TransactionMode.Manual` cho phép code tự kiểm soát transaction.
- `Transaction` phải Start rồi Commit hoặc RollBack.
- `SubTransaction` hữu ích khi thử từng thao tác trong transaction đang mở.
- Failure handling quyết định cách xử lý warning/error khi commit.

## Class/API cần nhớ
`Transaction`, `SubTransaction`, `TransactionGroup`, `TransactionMode.Manual`, `FailureHandlingOptions`, `Commit`

## Workflow
- Validate input
- Start transaction
- Create/edit elements
- Handle failures
- Commit/RollBack
- Report result

## Lỗi thường gặp
- ModificationOutsideTransactionException
- Nested transaction sai
- Không rollback khi lỗi
- Commit warning không kiểm soát

## Ví dụ Revit API thực tế
Tạo rebar cho nhiều cột trong một TransactionGroup; rollback từng cột lỗi bằng SubTransaction.

## Infographic
- PNG: `../04-infographics-png/14-transactions.png`
- Prompt: `../03-infographic-prompts/14-transactions.md`
