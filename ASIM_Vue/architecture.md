src/
├── assets/                 # Lưu trữ tài nguyên tĩnh
│   ├── fonts/              # Font Inter (tải từ CDN hoặc local) [cite: 34]
│   ├── icons/              # Các icon từ tài nguyên MISA cung cấp [cite: 33]
│   └── styles/             # Global CSS/SCSS, variables (Màu sắc theo Style Guide) [cite: 33, 35]
│
├── components/             # Các UI Components (Chỉ nhận Props và emit Events)
│   ├── base/               # Các UI control cơ bản dùng đi dùng lại
│   │   ├── MsButton.vue    
│   │   ├── MsInput.vue     
│   │   └── MsFormula.vue   # Component nhập công thức (dùng vue-prism-editor + prismjs) [cite: 22, 29]
│   ├── common/             # Các khối UI thông báo, điều hướng
│   │   ├── MsModal.vue     # Modal cảnh báo xóa, xác nhận 
│   │   ├── MsToast.vue     # Toast thông báo thành công/lỗi 
│   │   └── MsTooltip.vue   # Tooltip cho các button icon 
│   └── layout/             # Khung layout tĩnh
│       ├── TheHeader.vue   # Header tĩnh [cite: 15, 21]
│       └── TheSidebar.vue  # Menu trái tĩnh [cite: 15, 21]
│
├── views/                  # Các màn hình chính (Chứa logic nghiệp vụ)
│   └── SalaryComposition/  # Phân hệ Thành phần lương
│       ├── SalaryCompositionList.vue    # Màn hình danh sách chính (DevExtreme Table) [cite: 16, 27]
│       ├── SalaryCompositionForm.vue    # Form Thêm/Sửa/Nhân bản [cite: 17, 22]
│       └── SystemDictionaryModal.vue    # Modal danh mục hệ thống [cite: 17]
│
├── services/               # Tầng gọi API (Tách biệt logic fetch data khỏi component)
│   ├── api.js              # Cấu hình Axios base (interceptors)
│   └── salaryService.js    # Các hàm CRUD API (get, post, put, delete) 
│
├── utils/                  # Các hàm tiện ích dùng chung
│   ├── formatters.js       # Hàm format tiền tệ, ngày tháng 
│   ├── validators.js       # Logic validate (chống rỗng, unique, độ dài) 
│   └── constants.js        # Lưu trữ các biến hằng (Enum trạng thái, kiểu giá trị)
│
├── router/                 # Cấu hình Vue Router
│   └── index.js
│
├── store/                  # (Tùy chọn) Pinia/Vuex quản lý State toàn cục
│   └── index.js
│
├── App.vue                 # Component gốc
└── main.js                 # Entry point, khởi tạo Vue app