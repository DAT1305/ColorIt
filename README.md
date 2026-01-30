# ColorIt - Windows Folder Colorizer 🎨

Ứng dụng đổi màu folder trên Windows thông qua menu chuột phải.

[English version](README.en.md)

## Tính năng

- ✅ Đổi màu folder bằng cách nhấn chuột phải
- ✅ 12+ màu sắc để lựa chọn (Đỏ, Cam, Vàng, Xanh lá, Xanh dương, Tím, v.v.)
- ✅ Tùy chọn màu tùy chỉnh với Color Picker
- ✅ Khôi phục màu folder gốc
- ✅ **Tự động refresh** - Đổi màu ngay lập tức không cần nhấn F5
- ✅ **Song ngữ** - Hỗ trợ Tiếng Việt và Tiếng Anh
- ✅ **Lịch sử folder** - Xem danh sách các folder đã đổi màu
- ✅ Tích hợp hoàn toàn vào Windows Explorer Context Menu

## Yêu cầu hệ thống

- Windows 10/11
- .NET 8.0 Runtime (hoặc sử dụng bản self-contained)
- Quyền Administrator (để cài đặt context menu)

## Cài đặt nhanh

1. Mở thư mục `publish`
2. **Chuột phải** vào `install.bat` → **Run as administrator**
3. Xong! Bạn có thể nhấn chuột phải vào folder để đổi màu

## Build từ source

```powershell
# Clone hoặc download source
cd ColorIt

# Build
dotnet build

# Publish (self-contained)
dotnet publish -c Release -r win-x64 --self-contained -o publish
```

## Gỡ cài đặt

**Chuột phải** vào `uninstall.bat` → **Run as administrator**

Hoặc:
```powershell
ColorIt.exe --uninstall
```

## Cách sử dụng

1. **Nhấn chuột phải** vào bất kỳ folder nào trong Windows Explorer
2. Chọn **"🎨 Change Folder Color"**
3. Chọn màu bạn muốn từ 12 màu có sẵn hoặc chọn màu tùy chỉnh
4. Folder sẽ đổi màu **ngay lập tức**!

### Khôi phục màu gốc
- Nhấn chuột phải vào folder → "🎨 Change Folder Color" → "↩️ Khôi phục mặc định"

### Xem lịch sử folder đã đổi màu
- Mở ứng dụng ColorIt → Nhấn "📋 Folder đã đổi màu" để xem danh sách

### Đổi ngôn ngữ
- Mở ứng dụng ColorIt → Chọn "VI" hoặc "EN" ở góc trên bên phải

## Cách hoạt động

ColorIt hoạt động bằng cách:
1. Tạo một file icon tùy chỉnh (`folder.ico`) với màu bạn chọn
2. Tạo file `desktop.ini` để Windows sử dụng icon tùy chỉnh
3. Đặt thuộc tính System cho folder để kích hoạt custom icon
4. Tự động refresh tất cả cửa sổ Explorer để thay đổi hiển thị ngay lập tức

Tất cả các file được ẩn tự động và không ảnh hưởng đến nội dung folder.

## Screenshots

### Main Window
![Main Window](screenshots/main.png)

### Color Picker
![Color Picker](screenshots/picker.png)

### Context Menu
![Context Menu](screenshots/contextmenu.png)

*(Screenshots sẽ được thêm sau)*

## Troubleshooting

### Icon không thay đổi?
- Nhấn F5 để refresh Windows Explorer
- Đóng và mở lại folder
- Restart Windows Explorer

### Không thấy menu chuột phải?
- Đảm bảo đã chạy `install.bat` với quyền Administrator
- Restart Windows Explorer hoặc restart máy tính

## License

MIT License

---

Made with ❤️ for Windows users
