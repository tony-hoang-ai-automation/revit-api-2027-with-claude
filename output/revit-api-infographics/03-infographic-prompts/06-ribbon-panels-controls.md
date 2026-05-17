# Prompt Infographic: Ribbon, Panel, PushButton, SplitButton, StackedButton

## Output đã render
`../04-infographics-png/06-ribbon-panels-controls.png`

## ImageGen Prompt Chi Tiết

```text
Use case: infographic-diagram
Asset type: vertical Vietnamese educational infographic poster for BIM developers learning Autodesk Revit API.

Primary request:
Create a polished vertical technical infographic titled "Ribbon, Panel, PushButton, SplitButton, StackedButton". The poster must explain the topic clearly for a developer who knows C# but is learning Revit API. The visual should feel like a premium developer documentation poster, not a marketing landing page.

Audience:
Vietnamese BIM developers, Revit API add-in developers, C# engineers working with Autodesk Revit.

Subject:
Revit API learning poster about "Ribbon, Panel, PushButton, SplitButton, StackedButton".

Core message:
Ribbon API giúp đưa workflow add-in vào UI Revit bằng tab, panel, button, split button, radio group và contextual help.

Required visible text, verbatim:
"Ribbon, Panel, PushButton, SplitButton, StackedButton"
"Nền tảng Revit API 2014"
"Kiểm tra lại với Revit API version đang dùng"
"Khái niệm chính"
"API cần nhớ"
"Workflow"
"Lỗi thường gặp"
"Ví dụ Revit API"

Required concept bullets, keep short and readable:
- Ribbon thường được tạo trong `OnStartup` của `IExternalApplication`.
- Mỗi control có lớp Data để tạo và lớp Item sau khi add vào panel.
- Button trỏ tới assembly và class `IExternalCommand`.
- Availability, tooltip, large image và contextual help cải thiện UX.

Required API/class/method names, keep spelling exactly:
CreateRibbonTab, CreateRibbonPanel, PushButtonData, SplitButtonData, RibbonItem, ContextualHelp

Required workflow bullets:
- Create tab
- Create panel
- Create button data
- Add item
- Set icon/tooltip/help
- Bind command

Required common mistakes:
- Tên tab trùng
- Icon resource sai path
- Button quá nhiều gây rối ribbon
- Thiếu availability cho command theo context

Required example:
Panel 'Rebar Tools' có PushButton 'Create Stirrup' gọi command tạo thép đai cho column đang chọn.

Composition/framing:
- Portrait poster, 9:16 or 2:3 aspect ratio.
- Top header band with a large topic number, large title, and a small badge "Revit API 2014".
- Main body uses 5 stacked sections/cards:
  1. Tóm tắt: one short paragraph.
  2. Khái niệm chính: 3-4 large bullets with simple icons.
  3. API cần nhớ: pill-shaped code chips for class/method names.
  4. Workflow: left-to-right or top-to-bottom numbered process.
  5. Lỗi thường gặp + Ví dụ: warning callouts and one practical Revit API example.
- Add thin connector lines between sections to show learning flow.
- Leave generous margins; no crowded layout; all text must fit inside its panel.

Visual style:
- Modern flat technical infographic, crisp vector-like raster artwork.
- Clean Swiss/documentation typography.
- Subtle BIM/Revit visual language: generic building model wireframe, C# brackets, database/document icon, ribbon/command icon when relevant.
- Use generic icons only. Do not use official Autodesk/Revit/GitHub logos unless already allowed by the model; generic line icons are preferred.
- No photorealistic people. No cartoon characters. No fake software screenshots.

Typography:
- Vietnamese-compatible sans-serif font similar to Be Vietnam Pro, Inter, Arial, or SF Pro.
- Code/API chips in monospace style similar to JetBrains Mono.
- Large readable text: title very large, section headers medium-large, body bullets short.
- Preserve Vietnamese diacritics perfectly: ă, â, ê, ô, ơ, ư, đ, á, à, ả, ã, ạ, ấ, ầ, ẩ, ẫ, ậ.
- Preserve API names exactly, including casing and punctuation.

Color palette:
- Background: deep charcoal/navy #0F172A with subtle grid lines.
- Cards: white or near-white #F8FAFC.
- Text: dark slate #111827 on cards, white on header.
- Accents: cyan #38BDF8, green #22C55E, amber #F59E0B, red #EF4444, violet #A78BFA.
- Avoid a one-color blue/purple gradient. Use balanced accent colors.

Layout constraints:
- No overlapping text.
- No clipped title.
- No tiny paragraphs.
- No lorem ipsum.
- No gibberish pseudo-code.
- Do not invent unrelated API names.
- Keep each text block concise; if too much text, summarize visually instead of shrinking font.
- Every panel must have enough breathing room.

Negative prompt / avoid:
Avoid unreadable text, misspelled Vietnamese, broken diacritics, fake code, malformed API names, overstuffed poster, blurry typography, watermark, signature, random logos, decorative bokeh blobs, dark low-contrast text, overlapping cards, cropped footer, excessive 3D clutter.

Quality target:
High-resolution clean educational poster suitable for a Vietnamese Revit API training deck and printed handout.
```

## Ghi chú sử dụng
Prompt này phù hợp để tạo biến thể bằng ImageGen. Nếu model ảnh render chữ chưa đạt, hãy giảm số bullet hoặc chỉ giữ title + API chips + diagram, rồi dùng bản PNG deterministic trong `04-infographics-png` cho nội dung chính xác.
