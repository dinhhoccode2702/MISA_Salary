<template>
  <Teleport to="body">
    <Transition name="modal">
      <div v-if="show" class="ms-modal-overlay flex align-center justify-center">
        <div class="ms-modal-container" :style="{ width }" role="dialog" aria-modal="true">
          <div class="ms-modal-header flex justify-between align-center">
            <h3 class="modal-title">{{ title }}</h3>
            <button
              type="button"
              class="icon-close-modal"
              data-tooltip="Đóng"
              @click="$emit('close')"
              aria-label="Đóng"
            >
              ×
            </button>
          </div>
          <div class="ms-modal-body">
            <slot></slot>
          </div>
          <div class="ms-modal-footer flex justify-end">
            <slot name="footer">
              <MsButton type="outline" @click="$emit('close')" class="m-r-8">Hủy bỏ</MsButton>
              <MsButton type="primary" @click="$emit('submit')">Đồng ý</MsButton>
            </slot>
          </div>
        </div>
      </div>
    </Transition>
  </Teleport>
</template>

<script setup>
import MsButton from '../base/MsButton.vue';

defineProps({
  show: Boolean,
  title: String,
  width: { type: String, default: '500px' }
});

defineEmits(['close', 'submit']);
</script>

<style scoped>
.ms-modal-overlay {
  position: fixed;
  inset: 0;
  width: 100%;
  height: 100%;
  background-color: rgba(0, 0, 0, 0.45);
  z-index: 2000;
}

.ms-modal-container {
  background-color: white;
  border-radius: 4px;
  box-shadow: 0 4px 24px rgba(0, 0, 0, 0.2);
  display: flex;
  flex-direction: column;
  max-width: calc(100vw - 48px);
  max-height: 90vh;
  border: 1px solid var(--border-color);
}

.ms-modal-header {
  padding: 16px 24px;
  border-bottom: 1px solid var(--border-color);
}

.modal-title {
  font-size: 18px;
  font-weight: 700;
}

.icon-close-modal {
  width: 32px;
  height: 32px;
  border-radius: 4px;
  display: inline-flex;
  align-items: center;
  justify-content: center;
  font-size: 22px;
  line-height: 1;
  color: var(--text-secondary);
  background: transparent;
  cursor: pointer;
}

.icon-close-modal:hover {
  background-color: var(--border-color);
  color: var(--text-primary);
}

.ms-modal-body {
  padding: 24px;
  overflow-y: auto;
  flex: 1;
}

.ms-modal-footer {
  padding: 16px 24px;
  background-color: #ffffff;
  border-top: 1px solid var(--border-color);
  border-radius: 0 0 4px 4px;
  display: flex;
  align-items: center;
  justify-content: flex-end;
}

.modal-enter-active,
.modal-leave-active {
  transition: opacity 0.3s ease;
}

.modal-enter-from,
.modal-leave-to {
  opacity: 0;
}
</style>
