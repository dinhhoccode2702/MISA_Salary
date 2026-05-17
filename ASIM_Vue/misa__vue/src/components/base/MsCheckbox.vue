<template>
  <div class="ms-checkbox-container" :class="{ 'has-error': error }">
    <label class="ms-checkbox-item flex align-center cursor-pointer">
      <input 
        type="checkbox" 
        :checked="modelValue" 
        @change="$emit('update:modelValue', $event.target.checked)"
        class="ms-checkbox-input"
        :disabled="disabled"
      />
      <span class="ms-checkbox-box">
        <div class="ms-checkbox-inner"></div>
      </span>
      <span v-if="label" class="ms-checkbox-label m-l-8">{{ label }}</span>
    </label>
    <span v-if="error" class="error-msg">{{ error }}</span>
  </div>
</template>

<script setup>
defineProps({
  modelValue: Boolean,
  label: String,
  disabled: Boolean,
  error: String
});

defineEmits(['update:modelValue']);
</script>

<style scoped>
.ms-checkbox-item {
  user-select: none;
  padding: 4px 0;
  display: flex; /* Đảm bảo ô vuông và chữ nằm trên 1 hàng ngang */
  align-items: center;
  cursor: pointer;
}

.ms-checkbox-input {
  position: absolute;
  opacity: 0;
  cursor: pointer;
  height: 0;
  width: 0;
}

/* 1. TÙY CHỈNH LÚC CHƯA CLICK (HÌNH VUÔNG BO GÓC) */
.ms-checkbox-box {
  height: 20px; /* Tăng kích thước lên 1 chút cho giống ảnh */
  width: 20px;
  background-color: #fff;
  border: 2px solid #8e95a5; /* Màu viền xám xanh giống ảnh */
  border-radius: 4px; /* Bo góc lớn hơn (4px) để tạo hình vuông bo tròn */
  display: flex;
  align-items: center;
  justify-content: center;
  transition: all 0.2s ease;
  box-sizing: border-box; /* Giữ cho viền không làm méo kích thước */
}

/* Hiệu ứng khi di chuột qua */
.ms-checkbox-item:hover .ms-checkbox-box {
  /* Dùng màu xanh lá (#2ca04b) nếu biến --color-primary bị lỗi */
  border-color: var(--color-primary, #2ca04b); 
}

/* 2. TÙY CHỈNH LÚC ĐÃ CLICK (NỀN XANH) */
.ms-checkbox-input:checked ~ .ms-checkbox-box {
  border-color: var(--color-primary, #2ca04b);
  background-color: var(--color-primary, #2ca04b);
}

/* 3. TÙY CHỈNH DẤU TICK TRẮNG */
.ms-checkbox-inner {
  display: none;
  width: 5px;
  height: 10px;
  border: solid white;
  border-width: 0 2px 2px 0; /* Vẽ 2 cạnh dưới và phải của hình chữ nhật */
  transform: rotate(45deg); /* Xoay nghiêng để thành dấu tick */
  margin-bottom: 2px; /* Căn lên 1 chút để dấu tick nằm ở tâm đẹp nhất */
}

.ms-checkbox-input:checked ~ .ms-checkbox-box .ms-checkbox-inner {
  display: block;
}

.ms-checkbox-label {
  font-size: 14px;
  color: #111;
  margin-left: 8px; /* Cách ô checkbox ra 8px */
}

/* ── Error Styles ── */
.has-error .ms-checkbox-box {
  border-color: #eb5757;
}

.error-msg {
  color: #eb5757;
  font-size: 12px;
  margin-top: 4px;
  display: block;
}
</style>