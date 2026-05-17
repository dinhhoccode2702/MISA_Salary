<template>
  <div class="ms-page-header flex justify-between align-center">
    <div class="header-left flex align-center">
      <div v-if="showBack" class="btn-back flex align-center justify-center m-r-12" @click="$emit('back')">
        <span class="ms-icon-base ms-icon--back back-icon"></span>
      </div>
      <slot name="title">
        <h1 class="page-title">{{ title }}</h1>
      </slot>
    </div>
    <div class="header-right flex align-center">
      <slot name="actions"></slot>
    </div>
  </div>
</template>

<script setup>
defineProps({
  title: {
    type: String,
    required: true
  },
  showBack: {
    type: Boolean,
    default: false
  }
});

defineEmits(['back']);
</script>

<style scoped>
.ms-page-header {
  height: 64px; /* Tăng nhẹ chiều cao để thoáng hơn */
  min-height: 64px;
  padding: 0 24px;
  background-color: transparent; /* Để lộ nền xám của trang */
}

.page-title {
  font-size: 20px;
  font-weight: 700;
  color: #111;
  margin: 0;
  line-height: 24px; /* Khớp với chiều cao icon để căn giữa hoàn hảo */
}

.btn-back {
  width: 24px;
  height: 24px;
  cursor: pointer;
  border-radius: 50%;
  display: flex;
  align-items: center;
  justify-content: center;
  transition: background-color 0.2s;
  margin-right: 12px;
}

.btn-back:hover {
  background-color: rgba(0, 0, 0, 0.05);
}

.back-icon {
  width: 24px; /* Khớp với kích thước icon trong icons.css */
  height: 24px;
  background-color: #555;
}
</style>
